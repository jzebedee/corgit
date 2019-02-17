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
        public void ParseCountObjectsEmpty()
        {
            const string countObjects = "";

            var result = GitParsing.ParseCountObjects(countObjects);
            Assert.Null(result);
        }

        [Fact]
        public void ParseCountObjects()
        {
            const string countObjects = @"count: 123
size: 1099511627776
in-pack: 1
packs: 2
size-pack: 4398046511104
prune-packable: 3
garbage: 4
size-garbage: 2199023255552
";

            var expected = new GitObjectCount(count: 123, size: 1099511627776, inPack: 1, packs: 2,
                                              packSize: 4398046511104, prunePackable: 3, garbage: 4,
                                              garbageSize: 2199023255552);
            var result = GitParsing.ParseCountObjects(countObjects);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ParseCountObjectsUnknownFields()
        {
            const string countObjects = @"count: 123
size: 1099511627776
in-pack: 1
packs: 2
size-pack: 4398046511104
prune-packable: 3
garbage: 4
size-garbage: 2199023255552
future-value: 99
weird: hello
";

            var expected = new GitObjectCount(count: 123, size: 1099511627776, inPack: 1, packs: 2,
                                              packSize: 4398046511104, prunePackable: 3, garbage: 4,
                                              garbageSize: 2199023255552);
            var result = GitParsing.ParseCountObjects(countObjects);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ParseCountObjectsMissingFields()
        {
            const string countObjects = @"count: 123
size: 1099511627776
garbage: 4
size-garbage: 2199023255552
";

            var expected = new GitObjectCount(count: 123,
                                              size: 1099511627776,
                                              garbage: 4,
                                              garbageSize: 2199023255552);
            var result = GitParsing.ParseCountObjects(countObjects);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ParseCountObjectsMissingImportantFields()
        {
            const string countObjects = @"count: 123
garbage: 4
";

            Assert.Throws<KeyNotFoundException>(() => GitParsing.ParseCountObjects(countObjects));
        }
    }
}
