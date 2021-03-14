using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace MarkdownLinksVerifier.UnitTests.LinkValidatorTests
{
    public class LocalLinkValidatorTests
    {
        private static async Task<int> WriteResultsAndGetExitCodeAsync(StringWriter writer)
            => await MarkdownFilesAnalyzer.WriteResultsAsync(writer, $".{Path.DirectorySeparatorChar}WorkspaceTests");

        private static void Verify(string[] actual, (string File, string Link, string RelativeTo)[] expected)
        {
            Assert.True(actual.Length > 2, $"The actual output is expected to have at least two lines. Found {actual.Length} lines.");

            char separator = Path.DirectorySeparatorChar;
            // The first line is always expected to be that.
            Assert.Equal($"Starting Markdown Links Verifier in '.{separator}WorkspaceTests'.", actual[0]);

            // The last line is always an empty line.
            Assert.Equal("", actual[^1]);

            for (var expectedIndex = 0; expectedIndex < expected.Length; expectedIndex++)
            {
                int actualIndex = expectedIndex + 1;
                Assert.Equal(
                    $"::error::In file '{expected[expectedIndex].File}': Invalid link: '{expected[expectedIndex].Link}' relative to '{expected[expectedIndex].RelativeTo}'.",
                    actual[actualIndex]);
            }

            Assert.True(actual.Length == expected.Length + 2, $"Expected length doesn't match actual. Expected: {expected.Length + 2}, Actual: {actual.Length}.");
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
            var expected = new (string File, string Link, string RelativeTo)[]
            {
                ($".{separator}WorkspaceTests{separator}README.md", "README-2.md", $".{separator}WorkspaceTests")
            };

            Verify(writer.ToString().Split(writer.NewLine), expected);
            Assert.Equal(expected: 1, actual: returnCode);
        }
    }
}
