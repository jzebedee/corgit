namespace corgit
{
    /// <remarks>https://git-scm.com/docs/git-count-objects</remarks>
    public sealed class GitObjectCount
    {
        public GitObjectCount(int count, long size, int? inPack = null, int? packs = null, long? packSize = null,
                              int? prunePackable = null, int? garbage = null, long? garbageSize = null)
        {
            Count = count;
            Size = size;
            InPack = inPack;
            Packs = packs;
            PackSize = packSize;
            PrunePackable = prunePackable;
            Garbage = garbage;
            GarbageSize = garbageSize;
        }

        /// <summary>
        /// The number of loose objects
        /// </summary>
        public readonly int Count;
        /// <summary>
        /// Disk space consumed by loose objects, in KiB 
        /// </summary>
        public readonly long Size;
        /// <summary>
        /// The number of in-pack objects
        /// </summary>
        public readonly int? InPack;
        /// <summary>
        /// Number of packs
        /// </summary>
        public readonly int? Packs;
        /// <summary>
        /// Disk space consumed by the packs, in KiB 
        /// </summary>
        public readonly long? PackSize;
        /// <summary>
        /// The number of loose objects that are also present in the packs, which can be removed by running "<c>git prune-packed</c>"
        /// </summary>
        public readonly int? PrunePackable;
        /// <summary>
        /// The number of files in object database that are neither valid loose objects nor valid packs
        /// </summary>
        public readonly int? Garbage;
        /// <summary>
        /// Disk space consumed by garbage files, in KiB 
        /// </summary>
        public readonly long? GarbageSize;
    }
}
