﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace corgit
{
    public sealed class GitCommit : IEquatable<GitCommit>
    {
        public GitCommit(string hash,
                         string message,
                         string[] parents,
                         string[] refs,
                         string authorEmail,
                         DateTimeOffset authorDate)
        {
            if (hash?.Length != 40)
                throw new ArgumentException("Invalid SHA1 hash", nameof(hash));

            this.Hash = hash;
            this.Message = message;
            this.Parents = parents;
            this.Refs = refs;
            this.AuthorEmail = authorEmail;
            this.AuthorDate = authorDate;
        }

        public string Hash { get; }
        public string Message { get; }
        public string[] Parents { get; }
        public string[] Refs { get; }
        public string AuthorEmail { get; }
        public DateTimeOffset AuthorDate { get; }

        public string Subject
        {
            get
            {
                var end = Message.IndexOf('\n');
                return Message.Substring(0, end == -1 ? Message.Length : end);
            }
        }

        public override bool Equals(object obj)
            => obj is GitCommit commit && Equals(commit);

        public bool Equals(GitCommit other)
            => other != null
               && other.Hash == Hash
               && other.Message == Message
               && StructuralComparisons.StructuralEqualityComparer.Equals(other.Parents, Parents)
               && StructuralComparisons.StructuralEqualityComparer.Equals(other.Refs, Refs)
               && other.AuthorEmail == AuthorEmail
               && other.AuthorDate == AuthorDate;

        public override int GetHashCode()
        {
            var hashCode = -1219683383;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Hash);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Message);
            hashCode = hashCode * -1521134295 + StructuralComparisons.StructuralEqualityComparer.GetHashCode(Parents);
            hashCode = hashCode * -1521134295 + StructuralComparisons.StructuralEqualityComparer.GetHashCode(Refs);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AuthorEmail);
            hashCode = hashCode * -1521134295 + EqualityComparer<DateTimeOffset>.Default.GetHashCode(AuthorDate);
            return hashCode;
        }

        public override string ToString()
            => $"{Hash} {Subject}";

        public static bool operator ==(GitCommit left, GitCommit right)
            => EqualityComparer<GitCommit>.Default.Equals(left, right);

        public static bool operator !=(GitCommit left, GitCommit right)
            => !(left == right);
    }
}
