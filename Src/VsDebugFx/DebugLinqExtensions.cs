using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Roslyn.Scripting;
using VsDebugFx;

public static class DebugLinqExtensions
{
  #region Public methods

  public static List<object> SelectFx<TSource>(this IEnumerable<TSource> source, string selectorExpression)
  {
    if (source == null)
    {
      throw new ArgumentNullException("source");
    }

    if (string.IsNullOrEmpty(selectorExpression))
    {
      throw new ArgumentException("Argument can't be null nor empty.", "selectorExpression");
    }

    selectorExpression = CodeUtils.PreprocessCode(selectorExpression);

    Session session = RoslynUtils.CreateSession();

    Exception exception;

    Func<TSource, object> selector =
      TryExecute<Func<TSource, object>>(selectorExpression, session, out exception);

    if (selector != null)
    {
      return source.Select(selector).ToList();
    }

    throw new ArgumentException(string.Format("Couldn't parse selector expression ('{0}') as Func<T, object>. TSource: '{1}'.", selectorExpression, typeof(TSource)), "selectorExpression", exception);
  }

  public static List<TKey> SelectFx<TSource, TKey>(this IEnumerable<TSource> source, string selectorExpression)
  {
    if (source == null)
    {
      throw new ArgumentNullException("source");
    }

    if (string.IsNullOrEmpty(selectorExpression))
    {
      throw new ArgumentException("Argument can't be null nor empty.", "selectorExpression");
    }

    return source.SelectFx(selectorExpression).Cast<TKey>().ToList();
  }

  public static List<TSource> WhereFx<TSource>(this IEnumerable<TSource> source, string predicateExpression)
  {
    if (source == null)
    {
      throw new ArgumentNullException("source");
    }

    if (string.IsNullOrEmpty(predicateExpression))
    {
      throw new ArgumentException("Argument can't be null nor empty.", "predicateExpression");
    }

    predicateExpression = CodeUtils.PreprocessCode(predicateExpression);

    Session session = RoslynUtils.CreateSession();

    Exception exception;

    Func<TSource, bool> predicate =
      TryExecute<Func<TSource, bool>>(predicateExpression, session, out exception);

    if (predicate != null)
    {
      return source.Where(predicate).ToList();
    }

    Func<TSource, int, bool> predicateWithIndex =
      TryExecute<Func<TSource, int, bool>>(predicateExpression, session, out exception);

    if (predicateWithIndex != null)
    {
      return source.Where(predicateWithIndex).ToList();
    }

    throw new ArgumentException(string.Format("Couldn't parse predicate expression ('{0}') as neither Func<T, bool> nor Func<T, int, bool>. TSource: '{1}'.", predicateExpression, typeof(TSource)), "predicateExpression", exception);
  }

  public static List<TSource> OrderByFx<TSource, TKey>(this IEnumerable<TSource> source, string keySelectorExpression)
  {
    if (source == null)
    {
      throw new ArgumentNullException("source");
    }

    if (string.IsNullOrEmpty(keySelectorExpression))
    {
      throw new ArgumentException("Argument can't be null nor empty.", "keySelectorExpression");
    }

    return DoOrderByFx(source, keySelectorExpression, typeof(TKey)).ToList();
  }

  public static List<TSource> OrderByFx<TSource>(this IEnumerable<TSource> source, string keySelectorExpression, Type keyType)
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

    return DoOrderByFx(source, keySelectorExpression, keyType).ToList();
  }

  public static List<TSource> OrderByFx<TSource>(this IEnumerable<TSource> source, string keySelectorExpression, object keyTypeIndicator)
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

    return DoOrderByFx(source, keySelectorExpression, keyTypeIndicator.GetType()).ToList();
  }

  public static List<TSource> OrderByDescendingFx<TSource, TKey>(this IEnumerable<TSource> source, string keySelectorExpression)
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
      DoOrderByFx(source, keySelectorExpression, typeof(TKey))
        .Reverse()
        .ToList();
  }

  public static List<TSource> OrderByDescendingFx<TSource>(this IEnumerable<TSource> source, string keySelectorExpression, Type keyType)
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
      DoOrderByFx(source, keySelectorExpression, keyType)
        .Reverse()
        .ToList();
  }

  public static List<TSource> OrderByDescendingFx<TSource>(this IEnumerable<TSource> source, string keySelectorExpression, object keyTypeIndicator)
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
      DoOrderByFx(source, keySelectorExpression, keyTypeIndicator.GetType())
        .Reverse()
        .ToList();
  }

  public static TSource FirstFx<TSource>(this IEnumerable<TSource> source)
  {
    if (source == null)
    {
      throw new ArgumentNullException("source");
    }

    return source.First();
  }

  public static TSource FirstFx<TSource>(this IEnumerable<TSource> source, string predicateExpression)
  {
    if (source == null)
    {
      throw new ArgumentNullException("source");
    }

    if (string.IsNullOrEmpty(predicateExpression))
    {
      throw new ArgumentException("Argument can't be null nor empty.", "predicateExpression");
    }

    return source.WhereFx(predicateExpression).First();
  }

  public static TSource FirstOrDefaultFx<TSource>(this IEnumerable<TSource> source)
  {
    if (source == null)
    {
      throw new ArgumentNullException("source");
    }

    return source.FirstOrDefault();
  }

  public static TSource FirstOrDefaultFx<TSource>(this IEnumerable<TSource> source, string predicateExpression)
  {
    if (source == null)
    {
      throw new ArgumentNullException("source");
    }

    if (string.IsNullOrEmpty(predicateExpression))
    {
      throw new ArgumentException("Argument can't be null nor empty.", "predicateExpression");
    }

    return source.WhereFx(predicateExpression).FirstOrDefault();
  }

  public static TSource SingleFx<TSource>(this IEnumerable<TSource> source)
  {
    if (source == null)
    {
      throw new ArgumentNullException("source");
    }

    return source.Single();
  }

  public static TSource SingleFx<TSource>(this IEnumerable<TSource> source, string predicateExpression)
  {
    if (source == null)
    {
      throw new ArgumentNullException("source");
    }

    if (string.IsNullOrEmpty(predicateExpression))
    {
      throw new ArgumentException("Argument can't be null nor empty.", "predicateExpression");
    }

    return source.WhereFx(predicateExpression).Single();
  }

  public static TSource SingleOrDefaultFx<TSource>(this IEnumerable<TSource> source)
  {
    if (source == null)
    {
      throw new ArgumentNullException("source");
    }

    return source.SingleOrDefault();
  }

  public static TSource SingleOrDefaultFx<TSource>(this IEnumerable<TSource> source, string predicateExpression)
  {
    if (source == null)
    {
      throw new ArgumentNullException("source");
    }

    if (string.IsNullOrEmpty(predicateExpression))
    {
      throw new ArgumentException("Argument can't be null nor empty.", "predicateExpression");
    }

    return source.WhereFx(predicateExpression).SingleOrDefault();
  }

  #endregion

  #region Private methods

  private static IEnumerable<TSource> DoOrderByFx<TSource>(IEnumerable<TSource> source, string keySelectorExpression, Type keyType)
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

    keySelectorExpression = CodeUtils.PreprocessCode(keySelectorExpression);

    Session session = RoslynUtils.CreateSession();
    Type keySelectorType = ReflectionUtils.CreateFuncType2(typeof(TSource), keyType);
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

  private static object TryExecute(string code, Session session, Type resultType, out Exception exception)
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

      return method.Invoke(session, new object[] { code });
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

  private static T TryExecute<T>(string code, Session session, out Exception exception)
  {
    return (T)TryExecute(code, session, typeof(T), out exception);
  }

  #endregion
}
