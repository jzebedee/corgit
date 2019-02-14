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

            var expected = new List<GitFileStatus>();
            var results = Git.ParseStatus(status);
            Assert.Equal(expected, results);
        }

        [Fact]
        public void ParseSimpleStatus()
        {
            const string status = "?? file.txt\0";

            var expected = new List<GitFileStatus>()
            {
                new GitFileStatus()
                {
                    X = '?',
                    Y = '?',
                    Rename = null,
                    Path = "file.txt"
                }
            };
            var results = Git.ParseStatus(status);
            Assert.Equal(expected, results);
        }
    }
}
