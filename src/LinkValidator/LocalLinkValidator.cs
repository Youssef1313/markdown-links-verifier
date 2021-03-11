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

            // Temporary workaround for https://github.com/Youssef1313/markdown-links-verifier/issues/20
            // TODO: Check for the heading validity.
            var lastIndex = link.LastIndexOf('#');
            if (lastIndex != -1)
                link = link.Substring(0, lastIndex);

            var path = Path.GetFullPath(Path.Combine(_baseDirectory, link));
            return File.Exists(path) || Directory.Exists(path);
        }
    }
}
