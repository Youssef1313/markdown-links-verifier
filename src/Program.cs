using System;
using System.IO;
using System.Threading.Tasks;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using MarkdownLinksVerifier.Configuration;
using MarkdownLinksVerifier.LinkClassifier;
using MarkdownLinksVerifier.LinkValidator;

[assembly: CLSCompliant(true)]

MarkdownLinksVerifierConfiguration configuration = await ConfigurationReader.GetConfigurationAsync();
return await MarkdownFilesAnalyzer.WriteResultsAsync(Console.Out, configuration);

internal static class MarkdownFilesAnalyzer
{
    public static async Task<int> WriteResultsAsync(TextWriter writer, MarkdownLinksVerifierConfiguration config, string? rootDirectory = null)
    {
        var returnCode = 0;
        rootDirectory ??= Directory.GetCurrentDirectory();
        await writer.WriteLineAsync($"Starting Markdown Links Verifier in '{rootDirectory}'.");

        foreach (string file in Directory.EnumerateFiles(rootDirectory, "*.md", SearchOption.AllDirectories))
        {
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
                if (!config.IsLinkExcluded(link.Url) && !validator.IsValid(link.Url))
                {
                    await writer.WriteLineAsync($"::error::In file '{file}': Invalid link: '{link.Url}' relative to '{directory}'.");
                    returnCode = 1;
                }
            }
        }

        return returnCode;
    }
}
