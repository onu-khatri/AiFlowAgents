using System.Text.RegularExpressions;

namespace AiFlow.Core.Utilities;

/// <summary>
/// Provides methods for extracting fenced code blocks from text.
/// </summary>
public static class CodeBlocks
{
    /// <summary>
    /// Extracts the first fenced code block content with the specified language/tag.
    /// Example: ```analysis\n{...}\n```
    /// Returns null if not found.
    /// </summary>
    public static string? Extract(string text, string tag)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;

        var pattern = $"```{Regex.Escape(tag)}\\s*(.*?)```";
        var match = Regex.Match(text, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
        if (!match.Success) return null;
        return match.Groups[1].Value.Trim();
    }
}
