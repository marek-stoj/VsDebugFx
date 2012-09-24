using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace VsDebugFx.Tests
{
  [TestFixture]
  public class DebugLinqExtensionsTests
  {
    [Test]
    public void WhereEx_can_filter_ints_by_simple_predicate()
    {
      List<int> items = Enumerable.Range(1, 4).ToList();

      AssertEx.EnumerablesAreEqual(
        items.WhereEx("x => x % 2 == 0"),
        items.Where(x => x % 2 == 0));
    }

    [Test]
    public void WhereEx_can_filter_strings_by_simple_predicate()
    {
      List<string> items = Enumerable.Range(1, 4).Select(x => x.ToString()).ToList();

      AssertEx.EnumerablesAreEqual(
        items.WhereEx("x => int.Parse(x) % 2 == 0"),
        items.Where(x => int.Parse(x) % 2 == 0));
    }

    [Test]
    public void WhereEx_can_filter_strings_by_predicate_with_string_literal()
    {
      List<string> items = Enumerable.Range(1, 4).Select(x => x.ToString()).ToList();

      AssertEx.EnumerablesAreEqual(
        items.WhereEx("x => x == \"2\""),
        items.Where(x => x == "2"));
    }

    [Test]
    public void WhereEx_can_filter_strings_by_predicate_with_single_quote_string_literal()
    {
      List<string> items = Enumerable.Range(1, 4).Select(x => x.ToString()).ToList();

      AssertEx.EnumerablesAreEqual(
        items.WhereEx("x => x == '2'"),
        items.Where(x => x == "2"));
    }

    [Test]
    public void WhereEx_can_filter_characters_by_predicate_with_single_quote_escaping()
    {
      List<char> items = new[] { 'a', 'b', 'c' }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.WhereEx("x => x == ''b''"),
        items.Where(x => x == 'b'));
    }

    [Test]
    public void WhereEx_can_filter_by_predicate_with_index()
    {
      List<string> items = new[] { "one", "two", "three", "four" }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.WhereEx("(x, i) => i % 2 == 0"),
        items.Where((x, i) => i % 2 == 0));
    }

    [Test]
    public void WhereEx_throws_when_expression_is_invalid()
    {
      try
      {
        new[] { 1, 2, 3 }.WhereEx("(x, i) => 'qwe'");
        
        Assert.Fail("Exception was expected.");
      }
      catch (ArgumentException exc)
      {
        Console.WriteLine(exc);

        Assert.Pass();
      }
    }

    [Test]
    public void OrderByEx_can_order_by_element_value()
    {
      List<string> items = new[] { "c", "b", "a" }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.OrderByEx<string, string>("x => x"),
        items.OrderBy(x => x));
    }

    [Test]
    public void OrderByEx_can_order_by_a_property()
    {
      List<string> items = new[] { "aaa", "bb", "c" }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.OrderByEx<string, int>("x => x.Length"),
        items.OrderBy(x => x.Length));
    }

    [Test]
    public void OrderByEx_can_order_by_element_value_while_specifying_key_type()
    {
      List<string> items = new[] { "aaa", "bb", "c" }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.OrderByEx("x => x.Length", typeof(int)),
        items.OrderBy(x => x.Length));
    }

    [Test]
    public void OrderByEx_can_order_by_element_value_while_specifying_key_type_indicator()
    {
      List<string> items = new[] { "aaa", "bb", "c" }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.OrderByEx("x => x.Length", 0),
        items.OrderBy(x => x.Length));
    }

    [Test]
    public void OrderByEx_throws_when_expression_is_invalid()
    {
      try
      {
        new[] { "aaa", "bb", "c" }.OrderByEx("x => x.Length", typeof(char));

        Assert.Fail("Exception was expected.");
      }
      catch (ArgumentException exc)
      {
        Console.WriteLine(exc);

        Assert.Pass();
      }
    }

    [Test]
    public void OrderByDescendingEx_can_order_by_element_value()
    {
      List<string> items = new[] { "a", "b", "c" }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.OrderByDescendingEx<string, string>("x => x"),
        items.OrderByDescending(x => x));
    }

    [Test]
    public void OrderByDescendingEx_can_order_by_a_property()
    {
      List<string> items = new[] { "a", "bb", "ccc" }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.OrderByDescendingEx<string, int>("x => x.Length"),
        items.OrderByDescending(x => x.Length));
    }

    [Test]
    public void OrderByDescendingEx_can_order_by_element_value_while_specifying_key_type()
    {
      List<string> items = new[] { "a", "bb", "ccc" }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.OrderByDescendingEx("x => x.Length", typeof(int)),
        items.OrderByDescending(x => x.Length));
    }

    [Test]
    public void OrderByDescendingEx_can_order_by_element_value_while_specifying_key_type_indicator()
    {
      List<string> items = new[] { "a", "bb", "ccc" }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.OrderByDescendingEx("x => x.Length", 0),
        items.OrderByDescending(x => x.Length));
    }

    [Test]
    public void OrderByDescendingEx_throws_when_expression_is_invalid()
    {
      try
      {
        new[] { "a", "bb", "ccc" }.OrderByDescendingEx("x => x.Length", typeof(char));

        Assert.Fail("Exception was expected.");
      }
      catch (ArgumentException exc)
      {
        Console.WriteLine(exc);

        Assert.Pass();
      }
    }

    [Test]
    public void SelectEx_can_select_primitive_types()
    {
      List<string> items = new[] { "a", "aa", "aaa" }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.SelectEx("x => x.Length"),
        items.Select(x => x.Length).Cast<object>());
    }

    [Test]
    public void SelectEx_can_select_anonymous_types()
    {
      List<string> items = new[] { "a", "aa", "aaa" }.ToList();

      AssertEx.EnumerablesAreEqual(
        items.SelectEx("x => new { Element = x, Length = x.Length }"),
        items.Select(x => new { Element = x, Length = x.Length }));
    }
  }
}
