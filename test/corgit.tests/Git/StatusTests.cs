using corgit.Git;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace corgit.tests
{
    public class StatusTests
    {
        [Fact]
        public void ParseEmptyStatus()
        {
            const string status = "";

            var expected = new List<GitFileStatus>();
            var results = GitParsing.ParseStatus(status);
            Assert.Equal(expected, results);
        }

        [Fact]
        public void ParseSimpleStatus()
        {
            const string status = "?? file.txt\0";

            var expected = new List<GitFileStatus>()
            {
                (GitChangeType.Untracked, GitChangeType.Untracked, Path: "file.txt", null)
            };
            var results = GitParsing.ParseStatus(status);
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
                (GitChangeType.Untracked, GitChangeType.Untracked, Path: "file.txt", null),
                (GitChangeType.Untracked, GitChangeType.Untracked, Path: "file2.txt", null),
                (GitChangeType.Untracked, GitChangeType.Untracked, Path: "file3.txt", null),
            };
            var results = GitParsing.ParseStatus(status);
            Assert.Equal(expected, results);
        }

        [Fact]
        public void ParseRenameStatus()
        {
            const string status = "R  newfile.txt\0file.txt\0?? file2.txt\0?? file3.txt\0";

            var expected = new List<GitFileStatus>()
            {
                (GitChangeType.Renamed, GitChangeType.Unmodified, Path: "file.txt", OriginalPath: "newfile.txt"),
                (GitChangeType.Untracked, GitChangeType.Untracked, Path: "file2.txt", OriginalPath: null),
                (GitChangeType.Untracked, GitChangeType.Untracked, Path: "file3.txt", OriginalPath: null),
            };
            var results = GitParsing.ParseStatus(status);
            Assert.Equal(expected, results);
        }

        [Fact]
        public void FileStatusLooksLikePorcelain()
        {
            const string status = "A  newfile.txt\0"
                                  + "R  oldfile.txt\0renamedfile.txt\0"
                                  + "D  deletedfile.txt\0"
                                  + "?? untrackedfile.txt\0";

            var expected = new List<GitFileStatus>()
            {
                (GitChangeType.Added, GitChangeType.Unmodified, Path: "newfile.txt", OriginalPath: null),
                (GitChangeType.Renamed, GitChangeType.Unmodified, Path: "renamedfile.txt", OriginalPath: "oldfile.txt"),
                (GitChangeType.Deleted, GitChangeType.Unmodified, Path: "deletedfile.txt", OriginalPath: null),
                (GitChangeType.Untracked, GitChangeType.Untracked, Path: "untrackedfile.txt", OriginalPath: null),
            };
            var results = GitParsing.ParseStatus(status).ToList();
            Assert.Equal(expected, results);

            Assert.Equal("A  newfile.txt", results[0].ToString());
            Assert.Equal("R  oldfile.txt -> renamedfile.txt", results[1].ToString());
            Assert.Equal("D  deletedfile.txt", results[2].ToString());
            Assert.Equal("?? untrackedfile.txt", results[3].ToString());
        }
    }
}
