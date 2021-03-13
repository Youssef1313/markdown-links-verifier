using System;
using System.IO;

namespace MarkdownLinksVerifier.LinkValidator
{
    internal class LocalLinkValidator : ILinkValidator
    {
        private readonly string _baseDirectory;

        public LocalLinkValidator(string baseDirectory) => _baseDirectory = baseDirectory;

        public bool IsValid(string link)
        {
            // Consider [text]() as valid. It redirects to the current directory.
            if (string.IsNullOrEmpty(link))
            {
                return true;
            }

            link = link.Replace("%20", " ", StringComparison.Ordinal);
            string relativeTo = _baseDirectory;

            if (link.StartsWith('/'))
            {
                // Links that start with / are relative to the repository root.
                // TODO: Does it work locally (e.g. in VS Code)? Consider a warning for it.
                relativeTo = Directory.GetCurrentDirectory();
            }

            // Temporary workaround for https://github.com/Youssef1313/markdown-links-verifier/issues/20
            // TODO: Check for the heading validity.
            int lastIndex = link.LastIndexOf('#');
            if (lastIndex != -1)
            {
                link = link.Substring(0, lastIndex);
            }

            string path = Path.GetFullPath(Path.Join(relativeTo, link));
            return File.Exists(path) || Directory.Exists(path);
        }
    }
}
