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

    public static void EnumerablesAreEqual<T>(IEnumerable<T> expectedEnumerable, IEnumerable<T> actualEnumerable)
    {
      if (expectedEnumerable == null)
      {
        throw new ArgumentNullException("expectedEnumerable");
      }

      if (actualEnumerable == null)
      {
        throw new ArgumentNullException("actualEnumerable");
      }

      List<T> list1 = expectedEnumerable.ToList();
      List<T> list2 = actualEnumerable.ToList();

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

    private static bool ObjectsAreEqual(object expectedObject, object actualObject)
    {
      if (expectedObject == null && actualObject == null)
      {
        return true;
      }

      if (expectedObject == null || actualObject == null)
      {
        return false;
      }

      bool obj1IsAnonymous = expectedObject.GetType().IsAnonymousType();
      bool obj2IsAnonymous = actualObject.GetType().IsAnonymousType();

      if (obj1IsAnonymous && obj2IsAnonymous)
      {
        return NonNullObjectsOfAnonymousTypeAreEqual(expectedObject, actualObject);
      }

      if (obj1IsAnonymous || obj2IsAnonymous)
      {
        return false;
      }

      return Equals(expectedObject, actualObject);
    }

    private static bool NonNullObjectsOfAnonymousTypeAreEqual(object expectedObject, object actualObject)
    {
      Type expectedElementType = expectedObject.GetType();
      Type actualElementType = actualObject.GetType();
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
                        && ObjectsAreEqual(actualElementTypeProperties[expectedKvp.Key].GetValue(actualObject), expectedKvp.Value.GetValue(expectedObject)));
    }

    #endregion
  }
}
