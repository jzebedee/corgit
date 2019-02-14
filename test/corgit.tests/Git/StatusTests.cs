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
                ('?', '?', null, "file.txt")
            };
            var results = Git.ParseStatus(status);
            Assert.Equal(expected, results);
        }

        [Fact]
        public void ParseSimpleLongerStatus()
        {
            const string status = "?? file.txt\0"
                                  + "?? file2.txt\0"
                                  + "?? file3.txt\0";

            var expected = new List<GitFileStatus>()
            {
                ('?', '?', null, "file.txt"),
                ('?', '?', null, "file2.txt"),
                ('?', '?', null, "file3.txt"),
            };
            var results = Git.ParseStatus(status);
            Assert.Equal(expected, results);
        }

        [Fact]
        public void ParseRenameStatus()
        {
            const string status = "R  newfile.txt\0file.txt\0?? file2.txt\0?? file3.txt\0";

            var expected = new List<GitFileStatus>()
            {
                ('R', ' ', "newfile.txt", "file.txt"),
                ('?', '?', null, "file2.txt"),
                ('?', '?', null, "file3.txt"),
            };
            var results = Git.ParseStatus(status);
            Assert.Equal(expected, results);
        }
    }
}
