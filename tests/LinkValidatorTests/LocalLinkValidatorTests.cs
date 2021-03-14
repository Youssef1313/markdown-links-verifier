using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace MarkdownLinksVerifier.UnitTests.LinkValidatorTests
{
    public class LocalLinkValidatorTests
    {
        private static async Task<int> WriteResultsAndGetExitCodeAsync(StringWriter writer)
            => await MarkdownFilesAnalyzer.WriteResultsAsync(writer, $".{Path.DirectorySeparatorChar}WorkspaceTests");

        private static void CustomAssert(string expected, string actual)
        {
            try
            {
                Assert.Equal(expected, actual, ignoreLineEndingDifferences: true);
            }
            catch (Exception)
            {
                throw new XunitException($@"
Expected:
{expected}

Actual:
{actual}
");
            }
        }

        [Fact]
        public async Task TestSimpleCase_FileDoesNotExist()
        {
            using var workspace = new Workspace
            {
                Files =
                {
                    { "/README.md", "[text](README-2.md)" }
                }
            };

            char separator = Path.DirectorySeparatorChar;

            var workspacePath = await workspace.InitializeAsync();
            using var writer = new StringWriter();
            int returnCode = await WriteResultsAndGetExitCodeAsync(writer);
            Assert.Equal<object>(expected: @"Starting Markdown Links Verifier in .\WorkspaceTests.
::error::In file '.\WorkspaceTests\README.md': Invalid link: 'README-2.md' relative to '.\WorkspaceTests'.
", actual: writer.ToString());
            Assert.Equal(expected: 1, actual: returnCode);
        }
    }
}
