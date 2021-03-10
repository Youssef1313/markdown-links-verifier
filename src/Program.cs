using System;
using System.IO;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using MarkdownLinksVerifier.LinkClassifier;
using MarkdownLinksVerifier.LinkValidator;

bool hasErrors = false;

foreach (string file in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.md", SearchOption.AllDirectories))
{
    Console.WriteLine($"Validating links in: {file}.");
    string directory = Path.GetDirectoryName(file);
    MarkdownDocument document = Markdown.Parse(await File.ReadAllTextAsync(file));
    foreach (LinkInline link in document.Descendants<LinkInline>())
    {
        LinkClassification classification = LinkClassifier.Classify(link.Url);
        ILinkValidator validator = LinkValidatorCreator.Create(classification, directory);
        if (!validator.IsValid(link.Url))
        {
            Console.Error.WriteLine($"Invalid link: '{link.Url}' relative to '{directory}'.");
            hasErrors = true;
        }
    }
    Console.WriteLine();
}

return hasErrors ? 1 : 0;
