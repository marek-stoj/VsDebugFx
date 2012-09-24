using System;

namespace VsDebugFx
{
  [Serializable]
  public class InternalException : Exception
  {
    public InternalException(string message, Exception innerException = null)
      : base(message, innerException)
    {
    }
  }
}
