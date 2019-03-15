using System;
using System.Collections.Generic;
using System.Linq;
using static corgit.GitParsing;

namespace corgit
{
    public static class GitArguments
    {
        public struct CommitOptions
        {
            public readonly bool? All;
            public readonly bool? Amend;
            public readonly bool? Signoff;
            public readonly bool? SignCommit;
            public readonly bool? Empty;

            public CommitOptions(bool? all = default,
                                 bool? amend = default,
                                 bool? signoff = default,
                                 bool? signCommit = default,
                                 bool? empty = default)
            {
                All = all;
                Amend = amend;
                Signoff = signoff;
                SignCommit = signCommit;
                Empty = empty;
            }
        }
        public static IEnumerable<string> Commit(CommitOptions options = default)
        {
            yield return "commit";
            yield return "--quiet";
            yield return "--allow-empty-message";
            yield return "--file";
            yield return "-";

            if (options.All == true)
            {
                yield return "--all";
            }

            if (options.Amend == true)
            {
                yield return "--amend";
            }

            if (options.Signoff == true)
            {
                yield return "--signoff";
            }

            if (options.SignCommit == true)
            {
                yield return "-S";
            }

            if (options.Empty == true)
            {
                yield return "--allow-empty";
            }
        }

        public struct LogOptions
        {
            public readonly int? MaxEntries;

            public LogOptions(int? maxEntries = 32)
            {
                MaxEntries = maxEntries;
            }
        }
        public static IEnumerable<string> Log(LogOptions options = default)
        {
            const string CommitFormat = "%H\n%ae\n%P\n%B";
            const string Separator = "%x00%x00";

            yield return "log";

            if (options.MaxEntries.HasValue)
            {
                yield return $"-{options.MaxEntries}";
            }

            yield return $"--pretty=format:{CommitFormat}{Separator}";
        }

        public static IEnumerable<string> Add(IEnumerable<string> paths = null)
        {
            yield return "add";
            if (paths == null)
            {
                yield return "--all";
            }
            else
            {
                yield return "--";
                foreach (var path in paths)
                {
                    yield return path;
                }
            }
        }

        public static IEnumerable<string> Remove(IEnumerable<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException(nameof(paths));

            yield return "rm";
            yield return "--";
            foreach (var path in paths)
            {
                yield return path;
            }
        }

        public static IEnumerable<string> Config(string key, string value = null, string scope = null)
        {
            yield return "config";
            if (!string.IsNullOrEmpty(scope))
            {
                yield return $"--{scope}";
            }
            yield return key;
            if (!string.IsNullOrEmpty(value))
            {
                yield return value;
            }
        }

        public static IEnumerable<string> Status()
        {
            yield return "--no-optional-locks";
            yield return "status";
            yield return "-z";
            yield return "-u";
        }

        public static IEnumerable<string> Init()
        {
            yield return "init";
        }

        public static IEnumerable<string> CountObjects()
        {
            yield return "count-objects";
            yield return "-v";
        }

        public static IEnumerable<string> ArchiveFormatList()
        {
            yield return "archive";
            yield return "--list";
        }

        public struct ArchiveOptions
        {
            public readonly string Format;
            public readonly string Prefix;
            public readonly bool? WorktreeAttributes;
            public readonly IEnumerable<string> Extra;

            public readonly string Remote;
            public readonly string Exec;

            public readonly IEnumerable<string> Paths;

            public ArchiveOptions(string format = null, string prefix = null, bool? worktreeAttributes = null,
                                  IEnumerable<string> extra = null, string remote = null, string exec = null,
                                  IEnumerable<string> paths = null)
            {
                Format = format;
                Prefix = prefix;
                WorktreeAttributes = worktreeAttributes;
                Extra = extra;
                Remote = remote;
                Exec = exec;
                Paths = paths;
            }
        }
        public static IEnumerable<string> Archive(string treeish, string output, ArchiveOptions options = default)
        {
            if (string.IsNullOrWhiteSpace(treeish))
                throw new ArgumentNullException(nameof(treeish));

            if (string.IsNullOrWhiteSpace(output))
                throw new ArgumentNullException(nameof(output));

            yield return "archive";

            if (!string.IsNullOrEmpty(options.Format))
            {
                yield return $"--format={options.Format}";
            }

            if (!string.IsNullOrEmpty(options.Prefix))
            {
                yield return $"--prefix={options.Prefix}";
            }

            yield return $"--output={output}";

            if (options.WorktreeAttributes.GetValueOrDefault())
            {
                yield return "--worktree-attributes";
            }

            if (options.Extra != null)
            {
                foreach (var extraArg in options.Extra)
                {
                    yield return $"-{extraArg}";
                }
            }

            if (!string.IsNullOrEmpty(options.Remote))
            {
                yield return $"--remote={options.Remote}";
            }

            if (!string.IsNullOrEmpty(options.Exec))
            {
                yield return $"--exec={options.Exec}";
            }

            yield return treeish;

            if (options.Paths != null)
            {
                yield return "--";
                foreach (var path in options.Paths)
                {
                    yield return path;
                }
            }
        }
    }
}
