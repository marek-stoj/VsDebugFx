using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

namespace VsDebugFx
{
  public static class DebugLinqExtensions
  {
    private static readonly Regex _SingleQuoteRegex = new Regex(@"(?<!')'(?!')", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex _TwoSingleQuotesRegex = new Regex(@"''", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    #region Public methods

    public static IEnumerable<object> SelectEx<TSource>(this IEnumerable<TSource> source, string selectorExpression)
    {
      if (source == null)
      {
        throw new ArgumentNullException("source");
      }

      if (string.IsNullOrEmpty(selectorExpression))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "selectorExpression");
      }

      selectorExpression = PreprocessPredicateExpression(selectorExpression);

      Session session = CreateSession();

      Exception exception;

      Func<TSource, object> selector =
        TryExecute<Func<TSource, object>>(selectorExpression, session, out exception);

      if (selector != null)
      {
        return source.Select(selector);
      }

      throw new ArgumentException(string.Format("Couldn't parse selector expression ('{0}') as Func<T, object>. TSource: '{1}'.", selectorExpression, typeof(TSource)), "selectorExpression", exception);
    }

    public static IEnumerable<TSource> WhereEx<TSource>(this IEnumerable<TSource> source, string predicateExpression)
    {
      if (source == null)
      {
        throw new ArgumentNullException("source");
      }

      if (string.IsNullOrEmpty(predicateExpression))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "predicateExpression");
      }

      predicateExpression = PreprocessPredicateExpression(predicateExpression);

      Session session = CreateSession();

      Exception exception;

      Func<TSource, bool> predicate =
        TryExecute<Func<TSource, bool>>(predicateExpression, session, out exception);

      if (predicate != null)
      {
        return source.Where(predicate);
      }

      Func<TSource, int, bool> predicateWithIndex =
        TryExecute<Func<TSource, int, bool>>(predicateExpression, session, out exception);

      if (predicateWithIndex != null)
      {
        return source.Where(predicateWithIndex);
      }

      throw new ArgumentException(string.Format("Couldn't parse predicate expression ('{0}') as neither Func<T, bool> nor Func<T, int, bool>. TSource: '{1}'.", predicateExpression, typeof(TSource)), "predicateExpression", exception);
    }

    public static IEnumerable<TSource> OrderByEx<TSource, TKey>(this IEnumerable<TSource> source, string keySelectorExpression)
    {
      if (source == null)
      {
        throw new ArgumentNullException("source");
      }

      if (string.IsNullOrEmpty(keySelectorExpression))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "keySelectorExpression");
      }

      return DoOrderByEx(source, keySelectorExpression, typeof(TKey));
    }

    public static IEnumerable<TSource> OrderByEx<TSource>(this IEnumerable<TSource> source, string keySelectorExpression, Type keyType)
    {
      if (source == null)
      {
        throw new ArgumentNullException("source");
      }

      if (string.IsNullOrEmpty(keySelectorExpression))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "keySelectorExpression");
      }

      if (keyType == null)
      {
        throw new ArgumentNullException("keyType");
      }

      return DoOrderByEx(source, keySelectorExpression, keyType);
    }

    public static IEnumerable<TSource> OrderByEx<TSource>(this IEnumerable<TSource> source, string keySelectorExpression, object keyTypeIndicator)
    {
      if (source == null)
      {
        throw new ArgumentNullException("source");
      }

      if (string.IsNullOrEmpty(keySelectorExpression))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "keySelectorExpression");
      }

      if (keyTypeIndicator == null)
      {
        throw new ArgumentNullException("keyTypeIndicator");
      }

      return DoOrderByEx(source, keySelectorExpression, keyTypeIndicator.GetType());
    }

    public static IEnumerable<TSource> OrderByDescendingEx<TSource, TKey>(this IEnumerable<TSource> source, string keySelectorExpression)
    {
      if (source == null)
      {
        throw new ArgumentNullException("source");
      }

      if (string.IsNullOrEmpty(keySelectorExpression))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "keySelectorExpression");
      }

      return
        DoOrderByEx(source, keySelectorExpression, typeof(TKey))
          .Reverse();
    }

    public static IEnumerable<TSource> OrderByDescendingEx<TSource>(this IEnumerable<TSource> source, string keySelectorExpression, Type keyType)
    {
      if (source == null)
      {
        throw new ArgumentNullException("source");
      }

      if (string.IsNullOrEmpty(keySelectorExpression))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "keySelectorExpression");
      }

      if (keyType == null)
      {
        throw new ArgumentNullException("keyType");
      }

      return
        DoOrderByEx(source, keySelectorExpression, keyType)
          .Reverse();
    }

    public static IEnumerable<TSource> OrderByDescendingEx<TSource>(this IEnumerable<TSource> source, string keySelectorExpression, object keyTypeIndicator)
    {
      if (source == null)
      {
        throw new ArgumentNullException("source");
      }

      if (string.IsNullOrEmpty(keySelectorExpression))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "keySelectorExpression");
      }

      if (keyTypeIndicator == null)
      {
        throw new ArgumentNullException("keyTypeIndicator");
      }

      return
        DoOrderByEx(source, keySelectorExpression, keyTypeIndicator.GetType())
          .Reverse();
    }

    #endregion

    #region Private methods

    private static IEnumerable<TSource> DoOrderByEx<TSource>(IEnumerable<TSource> source, string keySelectorExpression, Type keyType)
    {
      if (source == null)
      {
        throw new ArgumentNullException("source");
      }

      if (string.IsNullOrEmpty(keySelectorExpression))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "keySelectorExpression");
      }

      if (keyType == null)
      {
        throw new ArgumentNullException("keyType");
      }

      keySelectorExpression = PreprocessPredicateExpression(keySelectorExpression);

      Session session = CreateSession();
      Type keySelectorType = CreateFuncType2(typeof(TSource), keyType);
      Exception exception;

      object keySelector =
        TryExecute(keySelectorExpression, session, keySelectorType, out exception);

      if (keySelector != null && keySelectorType.IsInstanceOfType(keySelector))
      {
        return InvokeOrderBy(keyType, source, keySelector);
      }

      throw new ArgumentException(string.Format("Couldn't parse key selector expression ('{0}') as Func<TSource, TKey>. TSource: '{1}'. TKey: '{2}'.", keySelectorExpression, typeof(TSource), keyType), "keySelectorExpression");
    }

    private static IOrderedEnumerable<TSource> InvokeOrderBy<TSource>(Type keyType, IEnumerable<TSource> source, object keySelector)
    {
      Type sourceType = typeof(TSource);

      MethodInfo methodDefinition =
        ReflectionUtils.GetGenericMethodDefinition(
          typeof(Enumerable),
          "OrderBy",
          BindingFlags.Public | BindingFlags.Static,
          new[] { typeof(IEnumerable<>), typeof(Func<,>) });

      if (methodDefinition == null)
      {
        throw new InternalException("Couldn't get OrderBy<TSource, TKey> method using reflection.");
      }

      MethodInfo method = methodDefinition.MakeGenericMethod(sourceType, keyType);

      return (IOrderedEnumerable<TSource>)method.Invoke(null, new[] { source, keySelector });
    }

    private static string PreprocessPredicateExpression(string predicateExpression)
    {
      predicateExpression =
        _SingleQuoteRegex.Replace(predicateExpression, "\"");

      predicateExpression =
        _TwoSingleQuotesRegex.Replace(predicateExpression, "'");

      return predicateExpression;
    }

    private static object TryExecute(string predicateExpression, Session session, Type resultType, out Exception exception)
    {
      try
      {
        exception = null;

        MethodInfo methodDefinition =
          session.GetType()
            .GetMethods()
            .SingleOrDefault(mi => mi.Name == "Execute" && mi.IsGenericMethodDefinition);

        if (methodDefinition == null)
        {
          throw new InternalException("Couldn't get Execute<T> method using reflection.");
        }

        MethodInfo method = methodDefinition.MakeGenericMethod(resultType);

        return method.Invoke(session, new object[] { predicateExpression });
      }
      catch (TargetInvocationException exc)
      {
        exception = exc.InnerException ?? exc;

        return null;
      }
      catch (Exception exc)
      {
        exception = exc;

        return null;
      }
    }

    private static T TryExecute<T>(string predicateExpression, Session session, out Exception exception)
    {
      return (T)TryExecute(predicateExpression, session, typeof(T), out exception);
    }

    private static Session CreateSession()
    {
      // TODO IMM HI: we have to create script engine every time because otherwise we get "Duplicate type name within an assembly" exception when using anonymous types in string-expressions; see if we can do something about this
      var scriptEngine = new ScriptEngine();

      Session session = scriptEngine.CreateSession();

      return session;
    }

    private static Type CreateFuncType2(Type paramType, Type resultType)
    {
      return typeof(Func<,>).MakeGenericType(paramType, resultType);
    }

    #endregion
  }
}
