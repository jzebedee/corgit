using System;
using System.Collections.Generic;

namespace corgit
{
    /// <remarks>https://git-scm.com/docs/git-count-objects</remarks>
    public sealed class GitObjectCount : IEquatable<GitObjectCount>
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

        public override bool Equals(object obj)
        {
            return Equals(obj as GitObjectCount);
        }

        public bool Equals(GitObjectCount other)
        {
            return other != null &&
                   Count == other.Count &&
                   Size == other.Size &&
                   EqualityComparer<int?>.Default.Equals(InPack, other.InPack) &&
                   EqualityComparer<int?>.Default.Equals(Packs, other.Packs) &&
                   EqualityComparer<long?>.Default.Equals(PackSize, other.PackSize) &&
                   EqualityComparer<int?>.Default.Equals(PrunePackable, other.PrunePackable) &&
                   EqualityComparer<int?>.Default.Equals(Garbage, other.Garbage) &&
                   EqualityComparer<long?>.Default.Equals(GarbageSize, other.GarbageSize);
        }

        public override int GetHashCode()
        {
            var hashCode = 779125064;
            hashCode = hashCode * -1521134295 + Count.GetHashCode();
            hashCode = hashCode * -1521134295 + Size.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(InPack);
            hashCode = hashCode * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(Packs);
            hashCode = hashCode * -1521134295 + EqualityComparer<long?>.Default.GetHashCode(PackSize);
            hashCode = hashCode * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(PrunePackable);
            hashCode = hashCode * -1521134295 + EqualityComparer<int?>.Default.GetHashCode(Garbage);
            hashCode = hashCode * -1521134295 + EqualityComparer<long?>.Default.GetHashCode(GarbageSize);
            return hashCode;
        }

        public static bool operator ==(GitObjectCount left, GitObjectCount right)
        {
            return EqualityComparer<GitObjectCount>.Default.Equals(left, right);
        }

        public static bool operator !=(GitObjectCount left, GitObjectCount right)
        {
            return !(left == right);
        }
    }
}
