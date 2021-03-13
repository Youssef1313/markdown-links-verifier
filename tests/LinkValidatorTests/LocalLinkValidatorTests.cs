using System.Threading.Tasks;
using Xunit;

namespace MarkdownLinksVerifier.UnitTests.LinkValidatorTests
{
    public class LocalLinkValidatorTests
    {
        [Fact]
        public async Task TestMethodAsync()
        {
            using var workspace = new Workspace
            {
                Files =
                {
                    { "/README.md", "[text](README.md)" }
                }
            };

            var workspacePath = await workspace.InitializeAsync();
            // TODO: Needs a way to get the output from the Console Application and verify it.
        }
    }
}
