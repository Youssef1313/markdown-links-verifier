using System;
using MarkdownLinksVerifier;
using MarkdownLinksVerifier.Configuration;

MarkdownLinksVerifierConfiguration configuration = await ConfigurationReader.GetConfigurationAsync();
return await MarkdownFilesAnalyzer.WriteResultsAsync(Console.Out, configuration);
