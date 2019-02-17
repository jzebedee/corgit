using System;
using System.Collections.Generic;
using System.Text;
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

        [Fact]
        public void ParseCountObjects()
        {
            const string countObjects = "";

            var result = GitParsing.ParseCountObjects(countObjects);
            Assert.Equal(null, countObjects);
        }
    }
}
