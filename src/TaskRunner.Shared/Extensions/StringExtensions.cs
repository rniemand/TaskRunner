using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace TaskRunner.Shared.Extensions
{
  public static class StringExtensions
  {
    public static string StripTrailingCharacter(this string input, string slash = "\\")
    {
      if (string.IsNullOrWhiteSpace(input))
        return string.Empty;

      // ReSharper disable once ConvertIfStatementToReturnStatement
      if (input.EndsWith(slash))
        return input.Substring(0, input.Length - slash.Length);

      return input;
    }

    public static bool MatchesRxPattern(
      this string input,
      string rxPattern,
      RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline)
    {
      // TODO: [TESTS] (StringExtensions) Add tests

      if (string.IsNullOrWhiteSpace(input))
        return false;

      if (string.IsNullOrWhiteSpace(rxPattern))
        throw new ArgumentNullException(nameof(rxPattern));

      return Regex.IsMatch(input, rxPattern, options);
    }

    public static MatchCollection GetRxMatches(
      this string input,
      string rxPattern,
      RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline)
    {
      // TODO: [TESTS] (StringExtensions) Add tests

      if (string.IsNullOrWhiteSpace(input))
        return null;

      if (string.IsNullOrWhiteSpace(rxPattern))
        throw new ArgumentNullException(nameof(rxPattern));

      return Regex.Matches(input, rxPattern, options);
    }

    public static string TrimAndLower(this string input)
    {
      // TODO: [TESTS] (StringExtensions) Add tests

      if (string.IsNullOrWhiteSpace(input))
        return string.Empty;

      return input.ToLower().Trim();
    }

    public static string CleanStepName(this string input)
    {
      // TODO: [TESTS] (StringExtensions) Add tests
      // TODO: [DOCS] (StringExtensions) Document this

      if (input.Contains("."))
      {
        input = input.Split('.').Last();
      }

      return input
        .Replace(".", "_") // "." are used for placeholders
        .Replace(" ", "_") // do not want any spaces in step names
        .TrimAndLower();
    }
  }
}
