using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace VsDebugFx.Tests
{
  [TestFixture]
  public class DebugFxTests
  {
    [Test]
    public void Run_can_create_anonymous_objects()
    {
      object obj = DebugFx.Run("new { Prop = 1 }");

      Assert.IsNotNull(obj);
      Assert.AreEqual(1, ((dynamic)obj).Prop);
    }

    [Test]
    public void Run_allows_to_use_single_quotes_for_string_literals()
    {
      object obj = DebugFx.Run("'hello'");

      Assert.IsNotNull(obj);
      Assert.AreEqual("hello", obj);
    }

    [Test]
    public void Run_allows_to_use_single_quotes_as_single_quotes_when_escaped()
    {
      object obj = DebugFx.Run("''X''");

      Assert.IsNotNull(obj);
      Assert.AreEqual('X', obj);
    }

    [Test]
    public void Run_allows_running_arbitrary_code()
    {
      object result = new object();

      Assert.DoesNotThrow(() => result = DebugFx.Run("for (int i = 0; i < 3; i++) { Console.WriteLine(i); }"));

      Assert.IsNull(result);
    }

    [Test]
    public void Run_allows_using_collections_and_linq_without_using_statements()
    {
      Assert.DoesNotThrow(() => DebugFx.Run("Console.WriteLine(string.Join(', ', new List<int>(new [] { 1, 2, 3 }).ToList() ))"));
    }

    [Test]
    public void Run_allows_using_linq_queries()
    {
      object obj = DebugFx.Run("from i in Enumerable.Range(1, 5) select i");

      Assert.IsNotNull(obj);
      Assert.IsInstanceOf<IEnumerable<int>>(obj);
      
      AssertEx.EnumerablesAreEqual(
        Enumerable.Range(1, 5),
        (IEnumerable<int>)obj);
    }
  }
}
