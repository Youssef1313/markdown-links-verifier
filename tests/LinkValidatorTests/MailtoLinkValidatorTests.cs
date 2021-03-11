using MarkdownLinksVerifier.LinkValidator;
using Xunit;

namespace MarkdownLinksVerifier.UnitTests.LinkValidatorTests
{
    public class MailtoLinkValidatorTests
    {
        [Fact]
        public void TestEmptyMailto()
        {
            Assert.False(new MailtoLinkValidator().IsValid("mailto:"));
        }

        [Fact]
        public void TestInvalidEmail()
        {
            Assert.False(new MailtoLinkValidator().IsValid("mailto:person"));
        }

        [Fact]
        public void TestValidEmail()
        {
            Assert.True(new MailtoLinkValidator().IsValid("mailto:person@company.com"));
        }
    }
}
