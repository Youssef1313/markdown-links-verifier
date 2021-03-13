using System;
using System.IO;

namespace MarkdownLinksVerifier.LinkValidator
{
    internal class LocalLinkValidator : ILinkValidator
    {
        private readonly string _baseDirectory;
        private const char RootSymbol = '~';

        public LocalLinkValidator(string baseDirectory) => _baseDirectory = baseDirectory;

        public bool IsValid(string link)
        {
            // Consider [text]() as valid. It redirects to the current directory.
            if (string.IsNullOrEmpty(link))
            {
                return true;
            }

            link = link.Replace("%20", " ", StringComparison.Ordinal);

            if (link.StartsWith(RootSymbol) &&
                (link[1] is '\\' or '/'))
            {
                link = Path.Join(Directory.GetCurrentDirectory(), link[1..]);
            }

            // Temporary workaround for https://github.com/Youssef1313/markdown-links-verifier/issues/20
            // TODO: Check for the heading validity.
            var lastIndex = link.LastIndexOf('#');
            if (lastIndex != -1)
                link = link.Substring(0, lastIndex);

            var path = Path.GetFullPath(Path.Join(_baseDirectory, link));
            return File.Exists(path) || Directory.Exists(path);
        }
    }
}
