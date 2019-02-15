//https://github.com/Microsoft/vscode/tree/master/extensions/git

using System;
using System.Collections.Generic;

namespace corgit
{
    public struct GitFileStatus : IEquatable<GitFileStatus>
    {
        public readonly GitChangeType X;
        public readonly GitChangeType Y;
        public readonly string Path;
        public readonly string OriginalPath;

        public GitFileStatus(GitChangeType x, GitChangeType y, string path, string originalPath)
        {
            X = x;
            Y = y;
            Path = path;
            OriginalPath = originalPath;
        }

        public override bool Equals(object obj)
            => obj is GitFileStatus other && Equals(other);

        public bool Equals(GitFileStatus other)
            => X == other.X
            && Y == other.Y
            && Path == other.Path
            && OriginalPath == other.OriginalPath;

        public override int GetHashCode()
        {
            var hashCode = 540761540;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Path);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(OriginalPath);
            return hashCode;
        }

        public override string ToString()
            => (((GitChangeType X, GitChangeType Y, string Path, string OriginalPath))this).ToString();

        public void Deconstruct(out GitChangeType x, out GitChangeType y, out string path, out string originalPath)
        {
            x = X;
            y = Y;
            path = Path;
            originalPath = OriginalPath;
        }

        public static implicit operator (GitChangeType X, GitChangeType Y, string Path, string OriginalPath) (GitFileStatus value)
            => (value.X, value.Y, value.Path, value.OriginalPath);

        public static implicit operator GitFileStatus((GitChangeType X, GitChangeType Y, string Path, string OriginalPath) value)
            => new GitFileStatus(value.X, value.Y, value.Path, value.OriginalPath);

        public static bool operator ==(GitFileStatus left, GitFileStatus right)
            => Equals(left, right);

        public static bool operator !=(GitFileStatus left, GitFileStatus right)
            => !(left == right);
    }
}
