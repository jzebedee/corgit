using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace corgit.tests
{
    public class CommitTests
    {
        [Fact]
        public void ParseSingleParentCommit()
        {
            const string GIT_OUTPUT_SINGLE_PARENT = "52c293a05038d865604c2284aa8698bd087915a1\n"
                                                    + "1234567\n"
                                                    + "john.doe@mail.com\n"
                                                    + "8e5a374372b8393906c7e380dbb09349c5385554\n"
                                                    + "This is a commit message.";

            var expected = new GitCommit("52c293a05038d865604c2284aa8698bd087915a1",
                                         "This is a commit message.",
                                         new[] { "8e5a374372b8393906c7e380dbb09349c5385554" },
                                         "john.doe@mail.com",
                                         DateTimeOffset.FromUnixTimeSeconds(1234567));
            var actual = GitParsing.ParseCommit(GIT_OUTPUT_SINGLE_PARENT);
            Assert.StrictEqual(expected, actual);
        }

        [Fact]
        public void ParseSingleRefCommit()
        {
            const string GIT_OUTPUT_SINGLE_REF = "d01ec56a830e10afe94d0fce6e61e9837eef381b somebranchname123\n"
                                                    + "1234567\n"
                                                    + "john.doe@mail.com\n"
                                                    + "8e5a374372b8393906c7e380dbb09349c5385554\n"
                                                    + "This is a commit message.";

            var expected = new GitCommit("52c293a05038d865604c2284aa8698bd087915a1",
                                         "This is a commit message.",
                                         new[] { "8e5a374372b8393906c7e380dbb09349c5385554" },
                                         "john.doe@mail.com",
                                         DateTimeOffset.FromUnixTimeSeconds(1234567));
            var actual = GitParsing.ParseCommit(GIT_OUTPUT_SINGLE_REF);
            Assert.StrictEqual(expected, actual);
        }

        [Fact]
        public void ParseMultipleParentCommit()
        {
            const string GIT_OUTPUT_MULTIPLE_PARENTS = "52c293a05038d865604c2284aa8698bd087915a1\n"
                                                       + "1234567\n"
                                                       + "john.doe@mail.com\n"
                                                       + "8e5a374372b8393906c7e380dbb09349c5385554 df27d8c75b129ab9b178b386077da2822101b217\n"
                                                       + "This is a commit message.";

            var expected = new GitCommit("52c293a05038d865604c2284aa8698bd087915a1",
                                         "This is a commit message.",
                                         new[] { "8e5a374372b8393906c7e380dbb09349c5385554", "df27d8c75b129ab9b178b386077da2822101b217" },
                                         "john.doe@mail.com",
                                         DateTimeOffset.FromUnixTimeSeconds(1234567));
            var actual = GitParsing.ParseCommit(GIT_OUTPUT_MULTIPLE_PARENTS);
            Assert.StrictEqual(expected, actual);
        }

        [Fact]
        public void ParseMultipleRefCommit()
        {
            const string GIT_OUTPUT_MULTIPLE_REFS = "1e1c799a41204d17181d98403dd4f581b369aea9 restore1, restore2, restore3, restore4\n"
                                                       + "1234567\n"
                                                       + "john.doe@mail.com\n"
                                                       + "8e5a374372b8393906c7e380dbb09349c5385554 df27d8c75b129ab9b178b386077da2822101b217\n"
                                                       + "This is a commit message.";

            var expected = new GitCommit("52c293a05038d865604c2284aa8698bd087915a1",
                                         "This is a commit message.",
                                         new[] { "8e5a374372b8393906c7e380dbb09349c5385554", "df27d8c75b129ab9b178b386077da2822101b217" },
                                         "john.doe@mail.com",
                                         DateTimeOffset.FromUnixTimeSeconds(1234567));
            var actual = GitParsing.ParseCommit(GIT_OUTPUT_MULTIPLE_REFS);
            Assert.StrictEqual(expected, actual);
        }

        [Fact]
        public void ParseNoParentCommit()
        {
            const string GIT_OUTPUT_NO_PARENTS = "52c293a05038d865604c2284aa8698bd087915a1\n"
                                                 + "1234567\n"
                                                 + "john.doe@mail.com\n"
                                                 + "\n"
                                                 + "This is a commit message.";

            var expected = new GitCommit("52c293a05038d865604c2284aa8698bd087915a1",
                                         "This is a commit message.",
                                         null,
                                         "john.doe@mail.com",
                                         DateTimeOffset.FromUnixTimeSeconds(1234567));
            var actual = GitParsing.ParseCommit(GIT_OUTPUT_NO_PARENTS);
            Assert.StrictEqual(expected, actual);
        }
    }
}
