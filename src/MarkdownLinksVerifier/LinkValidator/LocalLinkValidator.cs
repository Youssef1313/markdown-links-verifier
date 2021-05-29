using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace MarkdownLinksVerifier.LinkValidator
{
    internal class LocalLinkValidator : ILinkValidator
    {
        private readonly string _baseDirectory;

        public LocalLinkValidator(string baseDirectory) => _baseDirectory = baseDirectory;

        public bool IsValid(string link, string filePath)
        {
            // Consider [text]() as valid. It redirects to the current directory.
            if (string.IsNullOrEmpty(link))
            {
                return true;
            }

            if (link.StartsWith('#'))
            {
                return IsHeadingValid(filePath, link[1..]);
            }

            link = link.Replace("%20", " ", StringComparison.Ordinal);
            link = AdjustLinkPath(link, _baseDirectory);

            string? headingIdWithoutHash = null;
            int lastIndex = link.LastIndexOf('#');
            if (lastIndex != -1)
            {
                // TODO: Add a warning if headingIdWithoutHash is empty here?
                // e.g: [Text](file.md#)
                headingIdWithoutHash = link[(lastIndex + 1)..];
                link = link.Substring(0, lastIndex);
            }

            // Remove query parameters.
            lastIndex = link.LastIndexOf('?');
            if (lastIndex != -1)
            {
                link = link.Substring(0, lastIndex);
            }

            if (headingIdWithoutHash is null)
            {
                return File.Exists(link) || Directory.Exists(link);
            }
            else
            {
                return File.Exists(link) && IsHeadingValid(link, headingIdWithoutHash);
            }
        }
        private static string AdjustLinkPath(string link, string baseDirectory)
        {
            string relativeTo = baseDirectory;
            if (link.StartsWith('/'))
            {
                // Links that start with / are relative to the repository root.
                // TODO: Does it work locally (e.g. in VS Code)? Consider a warning for it.
                relativeTo = Directory.GetCurrentDirectory();
            }

            return Path.GetFullPath(Path.Join(relativeTo, link));
        }

        private static bool IsHeadingValid(string path, string headingIdWithoutHash)
        {
            if (!path.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAutoIdentifiers(AutoIdentifierOptions.GitHub).Build(); // TODO: Is AutoIdentifierOptions.GitHub the correct value to use?
            MarkdownDocument document = Markdown.Parse(File.ReadAllText(path), pipeline);
            return document.Descendants<HeadingBlock>().Any(heading => headingIdWithoutHash == heading.GetAttributes().Id) ||
                document.Descendants<HtmlInline>().Any(html => IsValidHtml(html.Tag, headingIdWithoutHash)) ||
                document.Descendants<HtmlBlock>().Any(block => block.Lines.Lines.Any(line => IsValidHtml(line.Slice.ToString(), headingIdWithoutHash)));

            // Hacky approach!
            static bool IsValidHtml(string tag, string headingIdWithoutHawsh)
            {
                return Regex.Match(tag, @"^<(a|span)\s+?(name|id)\s*?=\s*?""(.+?)""").Groups[3].Value == headingIdWithoutHawsh;
            }
        }
    }
}
