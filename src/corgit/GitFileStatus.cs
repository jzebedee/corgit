//https://github.com/Microsoft/vscode/tree/master/extensions/git

using System;
using System.Collections.Generic;

namespace corgit
{
    public class GitFileStatus : IEquatable<GitFileStatus>
    {
        public GitFileStatus(char x, char y, string path, string rename)
        {
            X = x;
            Y = y;
            Path = path;
            Rename = rename;
        }

        public char X { get; }
        public char Y { get; }
        public string Path { get; }
        public string Rename { get; }

        public override bool Equals(object obj)
        {
            return Equals(obj as GitFileStatus);
        }

        public bool Equals(GitFileStatus other)
        {
            return other != null &&
                   X == other.X &&
                   Y == other.Y &&
                   Path == other.Path &&
                   Rename == other.Rename;
        }

        public override int GetHashCode()
        {
            var hashCode = -865002436;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Path);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Rename);
            return hashCode;
        }

        public static bool operator ==(GitFileStatus left, GitFileStatus right)
        {
            return EqualityComparer<GitFileStatus>.Default.Equals(left, right);
        }

        public static bool operator !=(GitFileStatus left, GitFileStatus right)
        {
            return !(left == right);
        }
    }
}
