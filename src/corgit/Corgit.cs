using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace corgit
{
    public class Corgit
    {
        private static readonly Encoding Utf8NoBOM = new UTF8Encoding(false);

        private readonly string _gitPath;
        private readonly string _workingDirectory;

        private Process CreateGitProcess(string arguments = "", IReadOnlyDictionary<string, string> env = null)
        {
            var proc = new Process()
            {
                StartInfo = new ProcessStartInfo(_gitPath, arguments)
                {
                    WorkingDirectory = _workingDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardErrorEncoding = Utf8NoBOM,
                    StandardOutputEncoding = Utf8NoBOM
                },
            };

            if (env != null)
            {
                foreach (var kvp in env)
                {
                    proc.StartInfo.Environment[kvp.Key] = kvp.Value;
                }
            }

            return proc;
        }

        public struct ExecutionResult
        {
            internal ExecutionResult(int exitCode, string output, string error)
            {
                this.ExitCode = exitCode;
                this.Output = output;
                this.Error = error;
            }

            public readonly int ExitCode;

            public readonly string Output;

            public readonly string Error;

            public override string ToString() => (ExitCode, Output, Error).ToString();
        }

        private async Task<ExecutionResult> StartGitProcessAsync(Process proc)
        {
            var tcsFinished = new TaskCompletionSource<int>();
            var tcsStarted = new TaskCompletionSource<bool>();

            var error = tcsStarted.Task.ContinueWith(t => proc.StandardError.ReadToEndAsync()).Unwrap();
            var output = tcsStarted.Task.ContinueWith(t => proc.StandardOutput.ReadToEndAsync()).Unwrap();

            proc.EnableRaisingEvents = true;
            proc.Exited += async (sender, e) =>
            {
                if (proc.ExitCode >= 0)
                {
                    tcsFinished.SetResult(proc.ExitCode);
                }
                else
                {
                    tcsFinished.SetException(new GitException("Failed to execute git",
                        await error,
                        await output,
                        proc.ExitCode,
                        proc.StartInfo.Arguments));
                }
            };
            proc.Start();
            tcsStarted.SetResult(true);

            return new ExecutionResult(await tcsFinished.Task, await output, await error);
        }

        public async Task<ExecutionResult> RunGitAsync(string arguments = "",
                                                       string stdin = null,
                                                       IReadOnlyDictionary<string, string> env = null)
        {
            using var proc = CreateGitProcess(arguments, env);
            if (!string.IsNullOrEmpty(stdin))
            {
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.StandardInputEncoding = Utf8NoBOM;
            }

            var t = StartGitProcessAsync(proc);
            if (!string.IsNullOrEmpty(stdin))
            {
                //necessary to flush and close or git will continue waiting for more input
                using (proc.StandardInput)
                {
                    await proc.StandardInput.WriteAsync(stdin);
                }
            }

            return await t;
        }

        public Task<ExecutionResult> RunGitAsync(IEnumerable<string> arguments,
                                                string stdin = null,
                                                IReadOnlyDictionary<string, string> env = null)
           => RunGitAsync(string.Join(' ', arguments), stdin, env);

        public Corgit(string gitPath, string workingDirectory)
        {
            _gitPath = gitPath;
            _workingDirectory = workingDirectory;
        }

        public async Task<IEnumerable<GitFileStatus>> StatusAsync()
            => GitParsing.ParseStatus((await RunGitAsync(GitArguments.Status())).Output);

        public Task<ExecutionResult> InitAsync()
            => RunGitAsync(GitArguments.Init());

        public Task<IEnumerable<GitCommit>> LogAsync(GitArguments.LogOptions options = default, params string[] paths)
            => LogAsync(options: options, paths: paths.AsEnumerable());

        public async Task<IEnumerable<GitCommit>> LogAsync(GitArguments.LogOptions options = default, IEnumerable<string> paths = null)
        {
            var result = await RunGitAsync(GitArguments.Log(options, paths));
            if (result.ExitCode == 1)
            {
                return Enumerable.Empty<GitCommit>();
            }
            else
            {
                return GitParsing.ParseLog(result.Output);
            }
        }

        public Task<ExecutionResult> CommitAsync(string message = "", GitArguments.CommitOptions options = default)
            => RunGitAsync(GitArguments.Commit(options), message);

        public Task<ExecutionResult> CheckoutAsync(string treeish, IEnumerable<string> paths = null, GitArguments.CheckoutOptions options = default)
            => RunGitAsync(GitArguments.Checkout(treeish, paths, options));

        public Task<ExecutionResult> CheckoutNewBranchAsync(string branchName, bool force = false, string startPoint = null)
            => RunGitAsync(GitArguments.CheckoutNewBranch(branchName, force, startPoint));

        public Task<ExecutionResult> AddAsync(IEnumerable<string> paths)
            => RunGitAsync(GitArguments.Add(paths));

        public Task<ExecutionResult> AddAsync(params string[] paths)
            => AddAsync(paths.AsEnumerable());

        public Task<ExecutionResult> RemoveAsync(IEnumerable<string> paths)
            => RunGitAsync(GitArguments.Remove(paths));

        public Task<ExecutionResult> RemoveAsync(params string[] paths)
            => RemoveAsync(paths.AsEnumerable());

        public Task<ExecutionResult> ConfigAsync(string key, string value = null, string scope = null)
            => RunGitAsync(GitArguments.Config(key, value, scope));

        public async Task<GitObjectCount> CountObjectsAsync()
            => GitParsing.ParseCountObjects((await RunGitAsync(GitArguments.CountObjects())).Output);

        public async Task<IEnumerable<string>> ArchiveFormatListAsync()
            => GitParsing.ParseArchiveFormatList((await RunGitAsync(GitArguments.ArchiveFormatList())).Output);

        public Task<ExecutionResult> ArchiveAsync(string treeish, string output, GitArguments.ArchiveOptions options = default)
            => RunGitAsync(GitArguments.Archive(treeish, output, options));
    }
}
