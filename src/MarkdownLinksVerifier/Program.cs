using System;
using MarkdownLinksVerifier.Configuration;

[assembly: CLSCompliant(true)]

MarkdownLinksVerifierConfiguration configuration = await ConfigurationReader.GetConfigurationAsync();
return await MarkdownFilesAnalyzer.WriteResultsAsync(Console.Out, configuration);
