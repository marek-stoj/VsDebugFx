using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace VsDebugFx
{
  internal static class ReflectionUtils
  {
    /// <remarks>
    /// Credits go to Jon Skeet (see: http://stackoverflow.com/a/269992/139298).
    /// </remarks>>
    public static MethodInfo GetGenericMethodDefinition(Type declaringType, string methodName, BindingFlags bindingFlags, Type[] parameterTypes)
    {
      if (declaringType == null)
      {
        throw new ArgumentNullException("declaringType");
      }

      if (String.IsNullOrEmpty(methodName))
      {
        throw new ArgumentException("Argument can't be null nor empty.", "methodName");
      }

      if (parameterTypes == null)
      {
        throw new ArgumentNullException("parameterTypes");
      }

      return
        declaringType
          .GetMethods(bindingFlags)
          .Where(mi => mi.Name == methodName)
          .SingleOrDefault(
            mi =>
            mi.GetParameters()
              .Select(pi => pi.ParameterType.IsGenericType ? pi.ParameterType.GetGenericTypeDefinition() : pi.ParameterType)
              .SequenceEqual(parameterTypes));
    }

    /// <remarks>
    /// Credits go to Ricardo Lacerda Castelo Branco (see: http://stackoverflow.com/a/1650895/139298).
    /// </remarks>>
    public static bool IsAnonymousType(this Type type)
    {
      bool hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any();
      bool nameContainsAnonymousType = type.FullName != null && type.FullName.Contains("AnonymousType");
      
      return hasCompilerGeneratedAttribute && nameContainsAnonymousType;
    }

    public static Type CreateFuncType2(Type paramType, Type resultType)
    {
      return typeof(Func<,>).MakeGenericType(paramType, resultType);
    }
  }
}
