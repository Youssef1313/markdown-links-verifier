using System;
using MarkdownLinksVerifier.LinkClassifier;

namespace MarkdownLinksVerifier.LinkValidator
{
    // TODO: Have a caching mechanism.
    internal static class LinkValidatorCreator
    {
        public static ILinkValidator Create(LinkClassification classification, string baseDirectory)
            => classification switch
            {
                LinkClassification.Online => new OnlineLinkValidator(),
                LinkClassification.Local => new LocalLinkValidator(baseDirectory),
                LinkClassification.Mailto => new MailtoLinkValidator(),
                _ => throw new ArgumentException($"Invalid {nameof(classification)}.", nameof(classification))
            };
    }
}
