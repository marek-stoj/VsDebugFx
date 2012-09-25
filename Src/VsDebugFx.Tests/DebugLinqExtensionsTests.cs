using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace VsDebugFx.Tests
{
  [TestFixture]
  public class DebugLinqExtensionsTests
  {
    [Test]
    public void WhereFx_can_filter_ints_by_simple_predicate()
    {
      List<int> items = Enumerable.Range(1, 4).ToList();

      AssertEx.EnumerablesAreEqual(
        items.Where(x => x % 2 == 0),
        items.WhereFx("x => x % 2 == 0"));
    }

    [Test]
    public void WhereFx_can_filter_strings_by_simple_predicate()
    {
      List<string> items = Enumerable.Range(1, 4).Select(x => x.ToString()).ToList();

      AssertEx.EnumerablesAreEqual(
        items.Where(x => int.Parse(x) % 2 == 0),
        items.WhereFx("x => int.Parse(x) % 2 == 0"));
    }

    [Test]
    public void WhereFx_can_filter_strings_by_predicate_with_string_literal()
    {
      List<string> items = Enumerable.Range(1, 4).Select(x => x.ToString()).ToList();

      AssertEx.EnumerablesAreEqual(
        items.Where(x => x == "2"),
        items.WhereFx("x => x == \"2\""));
    }

    [Test]
    public void WhereFx_can_filter_strings_by_predicate_with_single_quote_string_literal()
    {
      List<string> items = Enumerable.Range(1, 4).Select(x => x.ToString()).ToList();

      AssertEx.EnumerablesAreEqual(
        items.Where(x => x == "2"),
        items.WhereFx("x => x == '2'"));
    }

    [Test]
    public void WhereFx_can_filter_characters_by_predicate_with_single_quote_escaping()
    {
      List<char> items = new[] { 'a', 'b', 'c' }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.Where(x => x == 'b'),
        items.WhereFx("x => x == ''b''"));
    }

    [Test]
    public void WhereFx_can_filter_custom_types()
    {
      List<TestClass> items = new[] { new TestClass { Prop = 1 }, new TestClass { Prop = 2 } }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.Where(x => x.Prop == 1).Cast<object>(),
        items.WhereFx("x => x.Prop == 1"));
    }

    [Test]
    public void WhereFx_can_filter_by_predicate_with_index()
    {
      List<string> items = new[] { "one", "two", "three", "four" }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.Where((x, i) => i % 2 == 0),
        items.WhereFx("(x, i) => i % 2 == 0"));
    }

    [Test]
    public void WhereFx_throws_when_expression_is_invalid()
    {
      try
      {
        new[] { 1, 2, 3 }.WhereFx("(x, i) => 'qwe'");
        
        Assert.Fail("Exception was expected.");
      }
      catch (ArgumentException exc)
      {
        Console.WriteLine(exc);

        Assert.Pass();
      }
    }

    [Test]
    public void OrderByFx_can_order_by_element_value()
    {
      List<string> items = new[] { "c", "b", "a" }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.OrderBy(x => x),
        items.OrderByFx<string, string>("x => x"));
    }

    [Test]
    public void OrderByFx_can_order_by_a_property()
    {
      List<string> items = new[] { "aaa", "bb", "c" }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.OrderBy(x => x.Length),
        items.OrderByFx<string, int>("x => x.Length"));
    }

    [Test]
    public void OrderByFx_can_order_by_element_value_while_specifying_key_type()
    {
      List<string> items = new[] { "aaa", "bb", "c" }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.OrderBy(x => x.Length),
        items.OrderByFx("x => x.Length", typeof(int)));
    }

    [Test]
    public void OrderByFx_can_order_by_element_value_while_specifying_key_type_indicator()
    {
      List<string> items = new[] { "aaa", "bb", "c" }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.OrderBy(x => x.Length),
        items.OrderByFx("x => x.Length", 0));
    }

    [Test]
    public void OrderByFx_throws_when_expression_is_invalid()
    {
      try
      {
        new[] { "aaa", "bb", "c" }.OrderByFx("x => x.Length", typeof(char));

        Assert.Fail("Exception was expected.");
      }
      catch (ArgumentException exc)
      {
        Console.WriteLine(exc);

        Assert.Pass();
      }
    }

    [Test]
    public void OrderByDescendingFx_can_order_by_element_value()
    {
      List<string> items = new[] { "a", "b", "c" }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.OrderByDescending(x => x),
        items.OrderByDescendingFx<string, string>("x => x"));
    }

    [Test]
    public void OrderByDescendingFx_can_order_by_a_property()
    {
      List<string> items = new[] { "a", "bb", "ccc" }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.OrderByDescending(x => x.Length),
        items.OrderByDescendingFx<string, int>("x => x.Length"));
    }

    [Test]
    public void OrderByDescendingFx_can_order_by_element_value_while_specifying_key_type()
    {
      List<string> items = new[] { "a", "bb", "ccc" }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.OrderByDescending(x => x.Length),
        items.OrderByDescendingFx("x => x.Length", typeof(int)));
    }

    [Test]
    public void OrderByDescendingFx_can_order_by_element_value_while_specifying_key_type_indicator()
    {
      List<string> items = new[] { "a", "bb", "ccc" }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.OrderByDescending(x => x.Length),
        items.OrderByDescendingFx("x => x.Length", 0));
    }

    [Test]
    public void OrderByDescendingFx_throws_when_expression_is_invalid()
    {
      try
      {
        new[] { "a", "bb", "ccc" }.OrderByDescendingFx("x => x.Length", typeof(char));

        Assert.Fail("Exception was expected.");
      }
      catch (ArgumentException exc)
      {
        Console.WriteLine(exc);

        Assert.Pass();
      }
    }

    [Test]
    public void SelectFx_can_select_ints()
    {
      List<int> items = new[] { 1, 2, 3 }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.Select(x => x).Cast<object>(),
        items.SelectFx("x => x"));
    }

    [Test]
    public void SelectFx_can_select_strings()
    {
      List<string> items = new[] { "a", "aa", "aaa" }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.Select(x => x.Length).Cast<object>(),
        items.SelectFx("x => x.Length"));
    }

    [Test]
    public void SelectFx_can_select_custom_types()
    {
      List<TestClass> items = new[] { new TestClass { Prop = 1 }, new TestClass { Prop = 2 } }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.Select(x => x.Prop).Cast<object>(),
        items.SelectFx("x => x.Prop"));
    }

    [Test]
    public void SelectFx_can_select_anonymous_types()
    {
      List<string> items = new[] { "a", "aa", "aaa" }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.Select(x => new { Element = x, Length = x.Length }),
        items.SelectFx("x => new { Element = x, Length = x.Length }"));
    }

    [Test]
    public void SelectFx_can_allow_to_specify_result_type()
    {
      List<TestClass> items = new[] { new TestClass { Prop = 1 }, new TestClass { Prop = 2 } }.ToList();

      Assert.AreEqual(
        items.Select(x => x.Prop).Sum(),
        items.SelectFx<TestClass, int>("x => x.Prop").Sum());
    }

    [Test]
    public void FirstFx_without_predicate_works()
    {
      List<int> items = Enumerable.Range(1, 3).ToList();

      Assert.AreEqual(1, items.FirstFx());
    }

    [Test]
    public void FirstFx_with_predicate_works()
    {
      List<int> items = Enumerable.Range(1, 3).ToList();

      Assert.AreEqual(2, items.FirstFx("x => x % 2 == 0"));
    }

    [Test]
    public void FirstOrDefaultFx_without_predicate_works()
    {
      var items = new List<int>();

      Assert.AreEqual(0, items.FirstOrDefaultFx());
    }

    [Test]
    public void FirstOrDefaultFx_with_predicate_works()
    {
      List<int> items = new[] { 1 }.ToList();

      Assert.AreEqual(0, items.FirstOrDefaultFx("x => false"));
    }

    [Test]
    public void SingleFx_without_predicate_works()
    {
      List<int> items = new[] { 1 }.ToList();

      Assert.AreEqual(1, items.SingleFx());
    }

    [Test]
    public void SingleFx_with_predicate_works()
    {
      List<int> items = Enumerable.Range(1, 3).ToList();

      Assert.AreEqual(2, items.SingleFx("x => x % 2 == 0"));
    }

    [Test]
    public void SingleFx_without_predicate_throws_if_there_are_more_than_one_elements()
    {
      List<int> items = new[] { 1, 2 }.ToList();

      Assert.Throws<InvalidOperationException>(() => items.SingleFx());
    }

    [Test]
    public void SingleFx_with_predicate_throws_if_there_are_more_than_one_elements()
    {
      List<int> items = Enumerable.Range(1, 3).ToList();

      Assert.Throws<InvalidOperationException>(() => items.SingleFx("x => x % 2 == 1"));
    }

    [Test]
    public void SingleOrDefaultFx_without_predicate_works()
    {
      var items = new List<int>();

      Assert.AreEqual(0, items.SingleOrDefaultFx());
    }

    [Test]
    public void SingleOrDefaultFx_with_predicate_works()
    {
      List<int> items = new[] { 1 }.ToList();

      Assert.AreEqual(0, items.SingleOrDefaultFx("x => false"));
    }

    [Test]
    public void SingleOrDefaultFx_without_predicate_throws_if_there_are_more_than_one_elements()
    {
      List<int> items = new[] { 1, 2 }.ToList();

      Assert.Throws<InvalidOperationException>(() => items.SingleOrDefaultFx());
    }

    [Test]
    public void SingleOrDefaultFx_with_predicate_throws_if_there_are_more_than_one_elements()
    {
      List<int> items = Enumerable.Range(1, 4).ToList();

      Assert.Throws<InvalidOperationException>(() => items.SingleOrDefaultFx("x => x % 2 == 1"));
    }

    [Test]
    public void SingleOrDefaultFx_without_predicate_doesnt_throw_if_there_are_no_elements()
    {
      List<int> items = new List<int>();

      Assert.AreEqual(0, items.SingleOrDefaultFx());
    }

    [Test]
    public void SingleOrDefaultFx_with_predicate_doesnt_throw_if_there_are_no_elements()
    {
      List<int> items = Enumerable.Range(1, 4).ToList();

      Assert.AreEqual(0, items.SingleOrDefaultFx("x => x % 2 == 3"));
    }
  }
}
