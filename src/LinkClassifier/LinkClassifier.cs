using System;

namespace MarkdownLinksVerifier.LinkClassifier
{
    internal static class LinkClassifier
    {
        public static LinkClassification Classify(string link)
        {
            if (link.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                link.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                return LinkClassification.Online;
            }

            return LinkClassification.Local;
        }
    }
}
