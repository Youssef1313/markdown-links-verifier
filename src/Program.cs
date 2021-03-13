﻿using System;
using System.IO;
using System.Threading.Tasks;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using MarkdownLinksVerifier.LinkClassifier;
using MarkdownLinksVerifier.LinkValidator;

[assembly: CLSCompliant(true)]

return await MarkdownFilesAnalyzer.WriteResultsAsync(Console.Out);

internal static class MarkdownFilesAnalyzer
{
    public static async Task<int> WriteResultsAsync(TextWriter writer, string? rootDirectory = null)
    {
        int returnCode = 0;
        rootDirectory ??= Directory.GetCurrentDirectory();

        foreach (string file in Directory.EnumerateFiles(rootDirectory, "*.md", SearchOption.AllDirectories))
        {
            writer.WriteLine($"Validating links in: {file}.");
            string? directory = Path.GetDirectoryName(file);
            if (directory is null)
            {
                throw new InvalidOperationException($"Cannot get directory for '{file}'.");
            }

            MarkdownDocument document = Markdown.Parse(await File.ReadAllTextAsync(file));
            foreach (LinkInline link in document.Descendants<LinkInline>())
            {
                LinkClassification classification = Classifier.Classify(link.Url);
                ILinkValidator validator = LinkValidatorCreator.Create(classification, directory);
                if (!validator.IsValid(link.Url))
                {
                    writer.WriteLine($"::error::Invalid link: '{link.Url}' relative to '{directory}'.");
                    returnCode = 1;
                }
            }

            writer.WriteLine();
        }

        return returnCode;
    }
}
