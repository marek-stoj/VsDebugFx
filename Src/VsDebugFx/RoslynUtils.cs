using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;

namespace VsDebugFx
{
  internal static class RoslynUtils
  {
    public static Session CreateSession()
    {
      // TODO IMM HI: we have to create script engine every time because otherwise we get "Duplicate type name within an assembly" exception when using anonymous types in string-expressions; see if we can do something about this
      var scriptEngine = new ScriptEngine();

      scriptEngine.AddReference("System.Core");

      var stackTrace = new StackTrace();
      StackFrame[] stackFrames = stackTrace.GetFrames() ?? new StackFrame[0];
      var processedAssemblies = new HashSet<string>();

      foreach (StackFrame stackFrame in stackFrames)
      {
        string assemblyName = stackFrame.GetMethod().ReflectedType.Assembly.FullName;

        if (processedAssemblies.Contains(assemblyName))
        {
          continue;
        }

        // ReSharper disable EmptyGeneralCatchClause
        try
        {
          scriptEngine.AddReference(Assembly.Load(assemblyName));
        }
        catch
        {
          // do nothing
        }
        finally
        {
          processedAssemblies.Add(assemblyName);
        }
        // ReSharper restore EmptyGeneralCatchClause
      }

      Session session = scriptEngine.CreateSession();

      session.ImportNamespace("System");
      session.ImportNamespace("System.Collections.Generic");
      session.ImportNamespace("System.Linq");

      return session;
    }
  }
}
