using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace MarkdownLinksVerifier.UnitTests.LinkValidatorTests
{
    public class LocalLinkValidatorTests
    {
        private static async Task<int> WriteResultsAndGetExitCodeAsync(StringWriter writer)
            => await MarkdownFilesAnalyzer.WriteResultsAsync(writer, $".{Path.DirectorySeparatorChar}WorkspaceTests");

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
            var expected = new[]
            {
                $"Starting Markdown Links Verifier in '.{separator}WorkspaceTests'.",
                $"::error::In file '.{separator}WorkspaceTests{separator}README.md': Invalid link: 'README-2.md' relative to '.{separator}WorkspaceTests'.",
                ""
            };

            Assert.Equal(expected, actual: writer.ToString().Split(writer.NewLine));
            Assert.Equal(expected: 1, actual: returnCode);
        }
    }
}
