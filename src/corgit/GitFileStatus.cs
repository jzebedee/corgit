//https://github.com/Microsoft/vscode/tree/master/extensions/git

using System;
using System.Collections.Generic;

namespace corgit
{
    public struct GitFileStatus : IEquatable<GitFileStatus>
    {
        public readonly char X;
        public readonly char Y;
        public readonly string Rename;
        public readonly string Path;

        public GitFileStatus(char x, char y, string rename, string path)
        {
            X = x;
            Y = y;
            Rename = rename;
            Path = path;
        }

        public override bool Equals(object obj)
            => obj is GitFileStatus other && Equals(other);

        public bool Equals(GitFileStatus other)
            => X == other.X
            && Y == other.Y
            && Rename == other.Rename
            && Path == other.Path;

        public override int GetHashCode()
        {
            var hashCode = 540761540;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Rename);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Path);
            return hashCode;
        }

        public void Deconstruct(out char x, out char y, out string rename, out string path)
        {
            x = X;
            y = Y;
            rename = Rename;
            path = Path;
        }

        public static implicit operator (char X, char Y, string Rename, string Path) (GitFileStatus value)
            => (value.X, value.Y, value.Rename, value.Path);

        public static implicit operator GitFileStatus((char X, char Y, string Rename, string Path) value)
            => new GitFileStatus(value.X, value.Y, value.Rename, value.Path);

        public static bool operator ==(GitFileStatus left, GitFileStatus right)
            => Equals(left, right);

        public static bool operator !=(GitFileStatus left, GitFileStatus right)
            => !(left == right);
    }
}
