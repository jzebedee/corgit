using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace corgit.tests
{
    public class ArchiveTests
    {
        [Fact]
        public void ParseFormatList()
        {
            const string archiveFormatList = "tar\ntgz\ntar.gz\nzip\n";

            var expected = new[] { "tar", "tgz", "tar.gz", "zip" };
            var actual = GitParsing.ParseArchiveFormatList(archiveFormatList);
            Assert.Equal(expected, actual);
        }
    }
}
