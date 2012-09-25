using System.Text.RegularExpressions;

namespace VsDebugFx
{
  internal static class CodeUtils
  {
    private static readonly Regex _SingleQuoteRegex = new Regex(@"(?<!')'(?!')", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex _TwoSingleQuotesRegex = new Regex(@"''", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static string PreprocessCode(string predicateExpression)
    {
      predicateExpression =
        _SingleQuoteRegex.Replace(predicateExpression, "\"");

      predicateExpression =
        _TwoSingleQuotesRegex.Replace(predicateExpression, "'");

      return predicateExpression;
    }
  }
}
