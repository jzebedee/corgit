using corgit.Git;
using Xunit;

namespace corgit.tests
{
    public partial class GitTests
    {
        [Fact]
        public void ParseVersion()
        {
            var result = GitParsing.ParseVersion("git version 2.18.0.windows.1");
            Assert.Equal("2.18.0.windows.1", result);
        }
    }
}
