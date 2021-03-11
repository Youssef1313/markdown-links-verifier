using System.IO;

namespace MarkdownLinksVerifier.LinkValidator
{
    internal class LocalLinkValidator : ILinkValidator
    {
        private readonly string _baseDirectory;

        public LocalLinkValidator(string baseDirectory) => _baseDirectory = baseDirectory;

        public bool IsValid(string link)
        {
            var path = Path.GetRelativePath(relativeTo: _baseDirectory, path: link);
            return File.Exists(path) || Directory.Exists(path);
        }
    }
}
