using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace MarkdownLinksVerifier.Configuration
{
    internal static class ConfigurationReader
    {
        private const string ConfigurationFileName = "markdown-links-verifier-config.json";
        private static readonly JsonSerializerOptions s_options = new() { AllowTrailingCommas = true };

        public static async Task<MarkdownLinksVerifierConfiguration> GetConfigurationAsync()
        {
            if (!File.Exists(ConfigurationFileName))
            {
                return MarkdownLinksVerifierConfiguration.Empty;
            }

            string json = await File.ReadAllTextAsync(ConfigurationFileName);
            MarkdownLinksVerifierConfiguration? config = JsonSerializer.Deserialize<MarkdownLinksVerifierConfiguration>(json, s_options);
            if (config is null)
            {
                throw new InvalidOperationException($"Failed to read '{ConfigurationFileName}'.");
            }

            return config;
        }
    }
}
