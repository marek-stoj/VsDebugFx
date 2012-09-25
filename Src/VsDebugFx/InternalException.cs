using System;

namespace VsDebugFx
{
  [Serializable]
  internal class InternalException : Exception
  {
    public InternalException(string message, Exception innerException = null)
      : base(message, innerException)
    {
    }
  }
}
