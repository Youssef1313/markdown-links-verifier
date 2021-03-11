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

            var path = Path.GetRelativePath(relativeTo: _baseDirectory, path: link);
            return File.Exists(path) || Directory.Exists(path);
        }
    }
}
