﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace corgit
{
    public static partial class GitParsing
    {
        private static readonly Regex r_parseVersion = new Regex(@"^git version ", RegexOptions.Compiled);
        public static string ParseVersion(string versionString)
        {
            return r_parseVersion.Replace(versionString, "");
        }

        private static readonly Dictionary<Regex, GitErrorCode> _gitErrorRegexes = new Dictionary<Regex, GitErrorCode>
        {
            {new Regex(@"Another git process seems to be running in this repository|If no other git process is currently running", RegexOptions.Compiled), GitErrorCode.RepositoryIsLocked },
            {new Regex(@"Authentication failed", RegexOptions.Compiled), GitErrorCode.AuthenticationFailed },
            {new Regex(@"Not a git repository", RegexOptions.IgnoreCase | RegexOptions.Compiled), GitErrorCode.NotAGitRepository },
            {new Regex(@"bad config file", RegexOptions.Compiled), GitErrorCode.BadConfigFile },
            {new Regex(@"cannot make pipe for command substitution|cannot create standard input pipe", RegexOptions.Compiled), GitErrorCode.CantCreatePipe },
            {new Regex(@"Repository not found", RegexOptions.Compiled), GitErrorCode.RepositoryNotFound },
            {new Regex(@"unable to access", RegexOptions.Compiled), GitErrorCode.CantAccessRemote },
            {new Regex(@"branch '.+' is not fully merged", RegexOptions.Compiled), GitErrorCode.BranchNotFullyMerged },
            {new Regex(@"Couldn't find remote ref", RegexOptions.Compiled), GitErrorCode.NoRemoteReference },
            {new Regex(@"A branch named '.+' already exists", RegexOptions.Compiled), GitErrorCode.BranchAlreadyExists },
            {new Regex(@"'.+' is not a valid branch name", RegexOptions.Compiled), GitErrorCode.InvalidBranchName },
            //mine:
            //{ new Regex(@"pathspec '.+' did not match any files", RegexOptions.Compiled), GitErrorCode.NoPathFound },
            //{ new Regex(@"current branch '.+' does not have any commits yet", RegexOptions.Compiled), GitErrorCode. },
            //template:
            //{new Regex(@"", RegexOptions.Compiled), GitErrorCode. },
        };

        private static readonly Regex r_parseCountObjects = new Regex(@"(\d+) objects, (\d+) kilobytes", RegexOptions.Compiled);
        public static GitObjectCount ParseCountObjects(string countObjects)
        {
            if (string.IsNullOrWhiteSpace(countObjects))
                return null;

            if (countObjects.IndexOf('\n') == -1)
            {
                var match = r_parseCountObjects.Match(countObjects);
                if (!match.Success)
                    throw new InvalidOperationException("Could not parse object count");

                return new GitObjectCount(count: int.Parse(match.Groups[0].Value),
                                          size: long.Parse(match.Groups[1].Value));
            }

            var dict = countObjects.Split('\n')
                .Where(l => !string.IsNullOrEmpty(l))
                .Select(l => l.Split(':'))
                .ToDictionary(f => f[0], f => f[1].Trim());

            return new GitObjectCount
            (
                count: int.Parse(dict["count"]),
                size: long.Parse(dict["size"]),
                inPack: int.Parse(dict["in-pack"]),
                packs: int.Parse(dict["packs"]),
                packSize: long.Parse(dict["size-pack"]),
                prunePackable: int.Parse(dict["prune-packable"]),
                garbage: int.Parse(dict["garbage"]),
                garbageSize: long.Parse(dict["size-garbage"])
            );
        }

        public static GitErrorCode? ParseErrorCode(string gitError)
        {
            foreach (var kvp in _gitErrorRegexes)
            {
                var regex = kvp.Key;
                if (regex.IsMatch(gitError))
                {
                    return kvp.Value;
                }
            }

            return null;
        }

        public static List<GitCommit> ParseLog(string log)
        {
            const string Separator = "\x00\x00";

            var s = log.AsSpan();
            var cs = Separator.AsSpan();

            var commits = new List<GitCommit>();
            while (!s.IsEmpty)
            {
                var nextIndex = s.IndexOf(cs);
                if (nextIndex == -1) nextIndex = s.Length;

                var entry = s.Slice(0, nextIndex).TrimStart('\n');

                var commit = ParseCommit(entry.ToString());
                if (commit == null) break; //maybe throw?

                commits.Add(commit);
                s = s.Slice(nextIndex + cs.Length);
            }

            return commits;
        }

        private static readonly Regex r_parseCommit = new Regex(@"^([0-9a-f]{40})\n(.*)\n(.*)\n([\s\S]*)$", RegexOptions.Multiline | RegexOptions.Compiled);
        public static GitCommit ParseCommit(string commit)
        {
            var match = r_parseCommit.Match(commit.Trim());
            if (!match.Success)
            {
                return null;
            }

            var parents = (match.Groups[3].Success && !string.IsNullOrEmpty(match.Groups[3].Value)) ? match.Groups[3].Value.Split(' ') : null;
            return new GitCommit(match.Groups[1].Value, match.Groups[4].Value, parents, match.Groups[2].Value);
        }

        public static ReadOnlySpan<char> ParseStatusEntry(ReadOnlySpan<char> entry, out GitFileStatus fileStatus)
        {
            fileStatus = default;
            if (entry.Length <= 4)
            {
                return null;
            }

            var X = (GitChangeType)entry[0];
            var Y = (GitChangeType)entry[1];
            //space = entry[2]
            string Path;
            string OriginalPath = null;
            entry = entry.Slice(3); //X + Y + space

            int lastIndex;
            switch (X)
            {
                case GitChangeType.Renamed:
                case GitChangeType.Copied:
                    lastIndex = entry.IndexOf('\0');
                    if (lastIndex == -1)
                    {
                        return null;
                    }

                    OriginalPath = entry.Slice(0, lastIndex).ToString();
                    entry = entry.Slice(lastIndex + 1);
                    break;
            }

            lastIndex = entry.IndexOf('\0');
            if (lastIndex == -1)
            {
                return null;
            }

            Path = entry.Slice(0, lastIndex).ToString();

            //from git.ts
            // If path ends with slash, it must be a nested git repo
            if (entry[lastIndex - 1] != '/')
            {
                fileStatus = (X: X, Y: Y, Path: Path, OriginalPath: OriginalPath);
            }

            return entry.Slice(lastIndex + 1);
        }

        public static List<GitFileStatus> ParseStatus(string status)
        {
            var parsed = new List<GitFileStatus>();

            ReadOnlySpan<char> s = status.AsSpan();
            while ((s = ParseStatusEntry(s, out GitFileStatus fileStatus)) != null)
                parsed.Add(fileStatus);

            return parsed;
        }
    }
}
