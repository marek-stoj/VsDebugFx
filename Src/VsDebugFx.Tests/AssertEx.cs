using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace VsDebugFx.Tests
{
  public static class AssertEx
  {
    #region Public methods

    public static void EnumerablesAreEqual<T>(IEnumerable<T> enumerable1, IEnumerable<T> enumerable2)
    {
      if (enumerable1 == null)
      {
        throw new ArgumentNullException("enumerable1");
      }

      if (enumerable2 == null)
      {
        throw new ArgumentNullException("enumerable2");
      }

      List<T> list1 = enumerable1.ToList();
      List<T> list2 = enumerable2.ToList();

      Assert.AreEqual(list1.Count, list2.Count, "Lists are of different length.");

      for (int i = 0; i < list2.Count; i++)
      {
        T expectedElement = list1[i];
        T actualElement = list2[i];

        if (Equals(expectedElement, default(T)) && Equals(actualElement, default(T)))
        {
          continue;
        }

        string message = string.Format("Elements at index '{0}' differ. Expected: '{1}'. Actual: '{2}'.", i, expectedElement, actualElement);

        if (Equals(expectedElement, default(T)) || Equals(actualElement, default(T)))
        {
          Assert.Fail(message);
        }

        Assert.IsTrue(ObjectsAreEqual(expectedElement, actualElement), message);
      }
    }

    #endregion

    #region Private methods

    private static bool ObjectsAreEqual(object obj1, object obj2)
    {
      if (obj1 == null && obj2 == null)
      {
        return true;
      }

      if (obj1 == null || obj2 == null)
      {
        return false;
      }

      bool obj1IsAnonymous = obj1.GetType().IsAnonymousType();
      bool obj2IsAnonymous = obj2.GetType().IsAnonymousType();

      if (obj1IsAnonymous && obj2IsAnonymous)
      {
        return NonNullObjectsOfAnonymousTypeAreEqual(obj1, obj2);
      }

      if (obj1IsAnonymous || obj2IsAnonymous)
      {
        return false;
      }

      return Equals(obj1, obj2);
    }

    private static bool NonNullObjectsOfAnonymousTypeAreEqual(object obj1, object obj2)
    {
      Type expectedElementType = obj1.GetType();
      Type actualElementType = obj2.GetType();
      BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

      var expectedElementTypePropertyNamesAndTypes =
        new HashSet<Tuple<string, Type>>(
          expectedElementType.GetProperties(bindingFlags)
            .Select(pi => new Tuple<string, Type>(pi.Name, pi.PropertyType)));

      var actualElementTypePropertyNamesAndTypes =
        new HashSet<Tuple<string, Type>>(
          actualElementType.GetProperties(bindingFlags)
            .Select(pi => new Tuple<string, Type>(pi.Name, pi.PropertyType)));

      if (!expectedElementTypePropertyNamesAndTypes.SetEquals(actualElementTypePropertyNamesAndTypes))
      {
        return false;
      }

      var expectedElementTypeProperties =
        expectedElementType.GetProperties()
          .ToDictionary(pi => pi.Name, pi => pi);

      var actualElementTypeProperties =
        actualElementType.GetProperties()
          .ToDictionary(pi => pi.Name, pi => pi);

      return
        expectedElementTypeProperties
          .All(
            expectedKvp => actualElementTypeProperties.ContainsKey(expectedKvp.Key)
                           && ObjectsAreEqual(actualElementTypeProperties[expectedKvp.Key].GetValue(obj2), expectedKvp.Value.GetValue(obj1)));
    }

    #endregion
  }
}
