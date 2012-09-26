VsDebugFx
=========

Ever got one of these messages in Visual Studio while debugging?

 - "Expression cannot contain lambda expressions"
 - "Expression cannot contain anonymous types"
 - "Expression cannot contain implicitly-typed arrays"
 - "Expression cannot contain query expressions"
 
Even though there are perfectly valid reasons why we're not allowed to use some of the
nicest features of C# during debugging (see [this][reasons-1] and [this][reasons-2]),
it can be a little frustrating.

VsDebugFx is a project that aims to bring back those features by working around the
limitations of the IDE, albeit it does so in a weakly-typed manner. What this means
is that, while you're debugging, you can evaluate expressions like these:

    ```csharp
    items.WhereFx("x => x.Prop > 100")
    items.SelectFx("x => new { x.Prop }", 0)
    DebugFx.Run("new[] { 1, 2, 3 }")
    DebuFx.Run("from x in Enumerable.Range(1, 3) select x")
    ```
How is this possible? It's thanks to [Microsoft Roslyn][roslyn] - a set of APIs for exposing
C# and VB.NET compilers as services available at runtime. Roslyn is a project I've been
meaning to take a look at for a while now and VsDebugFx is an attempt to make something
(hopefully) useful with it. Feel free to fork and experiment with the code and
[drop me a line](mailto:marek.stoj@gmail.com) if you have any questions or suggestions.

VsDebugFx is distributed as a NuGet package: [VsDebugFx on NuGet.org][nuget-vsdebugfx].

Usage
-----

1. In Package Manager Console type:

    ```
    Install-Package VsDebugFx
    ```

2. Start your debugging session.

3. Open a Watch window.

3. Load the VsDebugFx assembly into current application domain:

    ```csharp
    System.AppDomain.CurrentDomain.Load("VsDebugFx")
    ```

   or using reflection:
   
    ```csharp
    System.Reflection.Assembly.Load("VsDebugFx")
    ```

4. Now you can start using the extensions VsDebugFx provides.

5. When you're done, you can remove VsDebugFx package (and all of its dependencies) from your project
   by typing this in Package Manager Console:
   
    ```
    Uninstall-Package VsDebugFx -RemoveDependencies
    ```

Supported extensions
--------------------

Below are couple of sample expressions that you can use with VsDebugFx. If you want the complete list
of supported scenarios, it's best to [skim through the unit tests][unit-tests].

 - Select:

    ```csharp
    // you can use single quotes for string literals
    items.SelectFx("i => 'Item ' + i")
    ```

    ```csharp
    // if you need single quote, type it twice
    items.SelectFx("ch => ch == ''X''")
    ```
 
    ```csharp
    // anonymous objects are also supported
    items.SelectFx("x => new { x.Prop1, x.Prop2 }")
    ```
    
    ```csharp
    // normally you get a List<object> out of SelectFx() which you can either cast using ordinary LINQ
    // or just use a different overload of SelectFx()
    items.SelectFx("x => x.Length").Cast<int>()
    items.SelectFx<string, int>("x => x.Length")
    ```
    
 - Where:
 
    ```csharp
    // you can use arbitrary C# code in expressions
    items.WhereFx("x => Math.Floor(x) % 2 == 0")
    ```
    
    ```csharp
    // expressions involving types defined in the assembly being debugged are supported
    public class TestClass { public int Prop { get; set; } }
    items.WhereFx("x => x.Prop == 0")
    ```

 - OrderBy:
 
    ```csharp
    // when ordering, you have to specify the type of the selector; you can do this in a couple of different ways
    items.OrderByFx<string, int>("x => x.Length")
    items.OrderByFx("x => x.Length", typeof(int))
    items.OrderByFx("x => x.Length", 0)
    ```
 - Run:
 
    ```csharp
    // you can run arbitrary C# code within the context of the project being debugged
    namespace MyProject { public class TestClass { public int Prop { get; set; } } }
    DebugFx.Run("using MyProject; new[] { new TestClass { Prop = 1 } , new TestClass { Prop = 2 }, }")
    ```
    
    ```csharp
    // even LINQ queries will work
    DebugFx.Run("from x in new[] { 1, 2, 3 } select x")
    ```

Limitations
-----------

 - Your project must target at least version 4.5 of the .NET Framework (Roslyn needs it).
 
 - Before you can use VsDebugFx extensions during your debugging session, you have to load its assembly
   into current application domain. It's because Visual Studio won't automatically load all assemblies
   that are referenced by the project being debugged unless you've explicitly used it in code. You can
   read about the reasons behind such behavior here: [Extension Methods and the Debugger][loading-assemblies].
 
 - When you enter an expression that uses VsDebugFx extensions, you'll see that Visual Studio will
   display this message: "The function evaluation requires all threads to run". You'll have to manually
   trigger the evaluation by either clicking the icon to the right of the expression or by pressing
   the spacebar while the expression is focused. This is because Roslyn uses multiple threads to do its
   magic. For more explanation see this article: [How to: Refresh Watch Values][all-threads].

[roslyn]: http://msdn.microsoft.com/en-US/Roslyn/
[nuget-vsdebugfx]: https://nuget.org/packages/VsDebugFx
[reasons-1]: http://blogs.msdn.com/b/jaredpar/archive/2009/08/26/why-no-linq-in-debugger-windows.aspx
[reasons-2]: http://blogs.msdn.com/b/jaredpar/archive/2010/06/02/why-is-linq-absent-from-debugger-windows-part-2.aspx
[unit-tests]: https://github.com/marek-stoj/VsDebugFx/tree/master/Src/VsDebugFx.Tests
[all-threads]: http://msdn.microsoft.com/en-us/library/z4ecfxd9.aspx
[loading-assemblies]: http://blogs.msdn.com/b/jaredpar/archive/2010/07/22/extension-methods-and-the-debugger.aspx
