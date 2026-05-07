namespace ChatApp.Web.Services.Common;

using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;

public static class LinkParser
{
    private static readonly Regex UrlRegex = new Regex(
        @"(https?:\/\/[^\s]+)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static MarkupString Parse(string text, string baseAppUrl)
    {
        if (string.IsNullOrWhiteSpace(text)) return new MarkupString(string.Empty);

        var escapedText = System.Net.WebUtility.HtmlEncode(text);

        var linkedText = UrlRegex.Replace(escapedText, match =>
        {
            var originalUrl = match.Value;
            bool isInternal = originalUrl.StartsWith(baseAppUrl, StringComparison.OrdinalIgnoreCase);
            if (isInternal)
            {
                return $"<a href=\"{originalUrl}\" class=\"chat-link internal\">{originalUrl}</a>";
            }
            else
            {
                var encodedUrl = System.Net.WebUtility.UrlEncode(originalUrl);
                return $"<a href=\"/exit-warning?url={encodedUrl}\" target=\"_blank\" rel=\"noopener noreferrer\" class=\"chat-link external\">{originalUrl}</a>";
            }
        });

        return new MarkupString(linkedText);
    }
}