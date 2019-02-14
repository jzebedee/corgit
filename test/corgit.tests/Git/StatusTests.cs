using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace corgit.tests
{
    public class StatusTests
    {
        private Git Git { get; } = new Git();

        [Fact]
        public void ParseEmptyStatus()
        {
            const string status = "";

            var results = Git.ParseStatus(status);
            Assert.Equal(new List<GitFileStatus>(), results);
        }
    }
}
