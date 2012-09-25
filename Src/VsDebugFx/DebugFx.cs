using System;
using Roslyn.Scripting;
using VsDebugFx;

public static class DebugFx
{
  public static object Run(string code)
  {
    if (code == null)
    {
      throw new ArgumentNullException("code");
    }

    code = CodeUtils.PreprocessCode(code);

    Session session = RoslynUtils.CreateSession();

    return session.Execute(code);
  }
}
