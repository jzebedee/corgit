using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using static corgit.Git;

namespace corgit.tests
{
    partial class GitTests
    {
        [Fact]
        public void ParseSingleParentCommit()
        {
            const string GIT_OUTPUT_SINGLE_PARENT = "52c293a05038d865604c2284aa8698bd087915a1\n"
                                                    + "john.doe@mail.com\n"
                                                    + "8e5a374372b8393906c7e380dbb09349c5385554\n"
                                                    + "This is a commit message.";

            var expected = new GitCommit("52c293a05038d865604c2284aa8698bd087915a1",
                                         "This is a commit message.",
                                         new[] { "8e5a374372b8393906c7e380dbb09349c5385554" },
                                         "john.doe@mail.com");
            var actual = Git.ParseCommit(GIT_OUTPUT_SINGLE_PARENT);
            Assert.StrictEqual(expected, actual);
        }
    }
}
