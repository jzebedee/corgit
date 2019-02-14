using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace corgit
{
    public partial class Git
    {
        public sealed class GitCommit : IEquatable<GitCommit>
        {
            public GitCommit(string hash,
                             string message,
                             string[] parents,
                             string authorEmail)
            {
                this.Hash = hash;
                this.Message = message;
                this.Parents = parents;
                this.AuthorEmail = authorEmail;
            }

            public string Hash { get; }
            public string Message { get; }
            public string[] Parents { get; }
            public string AuthorEmail { get; }

            public override bool Equals(object obj)
                => obj is GitCommit commit && Equals(commit);

            public bool Equals(GitCommit other)
                => other != null
                   && other.Hash == Hash
                   && other.Message == Message
                   && StructuralComparisons.StructuralEqualityComparer.Equals(other.Parents, Parents)
                   && other.AuthorEmail == AuthorEmail;

            public override int GetHashCode()
            {
                var hashCode = -1219683383;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Hash);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Message);
                hashCode = hashCode * -1521134295 + StructuralComparisons.StructuralEqualityComparer.GetHashCode(Parents);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AuthorEmail);
                return hashCode;
            }

            public override string ToString()
                => (Hash, Message, Parents, AuthorEmail).ToString();

            public static bool operator ==(GitCommit left, GitCommit right)
                => EqualityComparer<GitCommit>.Default.Equals(left, right);

            public static bool operator !=(GitCommit left, GitCommit right)
                => !(left == right);
        }
    }
}
