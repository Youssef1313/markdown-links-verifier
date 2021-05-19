using System.IO;
using System.Threading.Tasks;
using MarkdownLinksVerifier.Configuration;
using Xunit;

namespace MarkdownLinksVerifier.UnitTests.LinkValidatorTests
{
    public class LocalLinkValidatorTests
    {
        private static async Task<int> WriteResultsAndGetExitCodeAsync(StringWriter writer, MarkdownLinksVerifierConfiguration? config = null)
            => await MarkdownFilesAnalyzer.WriteResultsAsync(writer, config ?? MarkdownLinksVerifierConfiguration.Empty, $".{Path.DirectorySeparatorChar}WorkspaceTests");

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

        private static void VerifyNoErrors(string[] actual)
        {
            Assert.True(actual.Length == 2, $"The actual output is expected to have exactly two lines. Found {actual.Length} lines.");

            char separator = Path.DirectorySeparatorChar;
            // The first line is always expected to be that.
            Assert.Equal($"Starting Markdown Links Verifier in '.{separator}WorkspaceTests'.", actual[0]);

            // The last line is always an empty line.
            Assert.Equal("", actual[1]);

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

            string workspacePath = await workspace.InitializeAsync();
            using var writer = new StringWriter();
            int returnCode = await WriteResultsAndGetExitCodeAsync(writer);
            (string File, string Link, string RelativeTo)[] expected = new[]
            {
                ($".{separator}WorkspaceTests{separator}README.md", "README-2.md", $".{separator}WorkspaceTests")
            };

            Verify(writer.ToString().Split(writer.NewLine), expected);
            Assert.Equal(expected: 1, actual: returnCode);
        }

        [Fact]
        public async Task TestValidHeadingIdInAnotherFile()
        {
            using var workspace = new Workspace
            {
                Files =
                {
                    { "/README.md", "[text](README2.md#hello)" },
                    { "/README2.md", "### HeLLo" }
                }
            };

            char separator = Path.DirectorySeparatorChar;

            string workspacePath = await workspace.InitializeAsync();
            using var writer = new StringWriter();
            int returnCode = await WriteResultsAndGetExitCodeAsync(writer);
            VerifyNoErrors(writer.ToString().Split(writer.NewLine));
            Assert.Equal(expected: 0, actual: returnCode);
        }

        [Fact]
        public async Task TestValidHeadingIdInAnotherFile_ComplexHeading()
        {
            using var workspace = new Workspace
            {
                Files =
                {
                    { "/README.md", "[text](README2.md#1-scope)" },
                    { "/README2.md", "### 1 Scope" }
                }
            };

            char separator = Path.DirectorySeparatorChar;

            string workspacePath = await workspace.InitializeAsync();
            using var writer = new StringWriter();
            int returnCode = await WriteResultsAndGetExitCodeAsync(writer);
            VerifyNoErrors(writer.ToString().Split(writer.NewLine));
            Assert.Equal(expected: 0, actual: returnCode);
        }

        [Fact]
        public async Task TestValidHeadingIdInSameFile()
        {
            using var workspace = new Workspace
            {
                Files =
                {
                    { "/README.md", @"## Heading1

Hello world.

## Heading 2
[text](#heading1)" },
                }
            };

            char separator = Path.DirectorySeparatorChar;

            string workspacePath = await workspace.InitializeAsync();
            using var writer = new StringWriter();
            int returnCode = await WriteResultsAndGetExitCodeAsync(writer);
            VerifyNoErrors(writer.ToString().Split(writer.NewLine));
            Assert.Equal(expected: 0, actual: returnCode);
        }

        [Fact]
        public async Task TestInvalidHeadingIdInAnotherFile()
        {
            using var workspace = new Workspace
            {
                Files =
                {
                    { "/README.md", "[text](README2.md#Hello)" },
                    { "/README2.md", "### HeLLo" }
                }
            };

            char separator = Path.DirectorySeparatorChar;

            string workspacePath = await workspace.InitializeAsync();
            using var writer = new StringWriter();
            int returnCode = await WriteResultsAndGetExitCodeAsync(writer);
            (string File, string Link, string RelativeTo)[] expected = new[]
            {
                ($".{separator}WorkspaceTests{separator}README.md", "README2.md#Hello", $".{separator}WorkspaceTests")
            };

            Verify(writer.ToString().Split(writer.NewLine), expected);
            Assert.Equal(expected: 1, actual: returnCode);
        }

        [Fact]
        public async Task TestInvalidHeadingIdInSameFile()
        {
            using var workspace = new Workspace
            {
                Files =
                {
                    { "/README.md", @"## Heading1

Hello world.

## Heading 2
[text](#Heading1)" },
                }
            };

            char separator = Path.DirectorySeparatorChar;

            string workspacePath = await workspace.InitializeAsync();
            using var writer = new StringWriter();
            int returnCode = await WriteResultsAndGetExitCodeAsync(writer);
            (string File, string Link, string RelativeTo)[] expected = new[]
{
                ($".{separator}WorkspaceTests{separator}README.md", "#Heading1", $".{separator}WorkspaceTests")
            };
            Verify(writer.ToString().Split(writer.NewLine), expected);
            Assert.Equal(expected: 1, actual: returnCode);
        }

        [Fact]
        public async Task TestHeadingReferenceToNonMarkdownFile()
        {
            using var workspace = new Workspace
            {
                Files =
                {
                    { "/README.md", "[text](Program.cs#Snippet1)" },
                    { "/Program.cs", "class Program {}" },
                }
            };

            char separator = Path.DirectorySeparatorChar;

            string workspacePath = await workspace.InitializeAsync();
            using var writer = new StringWriter();
            int returnCode = await WriteResultsAndGetExitCodeAsync(writer);

            VerifyNoErrors(writer.ToString().Split(writer.NewLine));
            Assert.Equal(expected: 0, actual: returnCode);
        }
    }
}
