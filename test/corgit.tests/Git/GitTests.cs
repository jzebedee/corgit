using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace corgit.tests
{
    public partial class GitTests
    {
        private Git Git { get; } = new Git();

        [Fact]
        public void ParseVersion()
        {
            var result = Git.ParseVersion("git version 2.18.0.windows.1");
            Assert.Equal("2.18.0.windows.1", result);
        }
    }
}
