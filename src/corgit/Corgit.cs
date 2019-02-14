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
        private readonly string _gitPath;
        private readonly string _workingDirectory;

        private readonly Git _git = new Git();

        private Process CreateGitProcess(string arguments = "") => new Process()
        {
            StartInfo = new ProcessStartInfo(_gitPath, arguments)
            {
                WorkingDirectory = _workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };

        public struct ExecutionResult
        {
            internal ExecutionResult(int exitCode, string output, string error)
            {
                this.ExitCode = exitCode;
                this.Output = output;
                this.Error = error;
            }

            public int ExitCode { get; }

            public string Output { get; }

            public string Error { get; }

            public override string ToString() => (ExitCode, Output, Error).ToString();
        }

        public Task<ExecutionResult> RunGitAsync(IEnumerable<string> arguments, string stdin = null)
            => RunGitAsync(string.Join(" ", arguments), stdin);

        public async Task<ExecutionResult> RunGitAsync(string arguments = "", string stdin = null)
        {
            var tcs = new TaskCompletionSource<int>();

            using (var proc = CreateGitProcess(arguments))
            {
                proc.StartInfo.RedirectStandardInput = stdin != null;
                proc.EnableRaisingEvents = true;
                proc.Exited += (sender, e) =>
                {
                    if (proc.ExitCode >= 0)
                    {
                        tcs.SetResult(proc.ExitCode);
                    }
                    else
                    {
                        var error = proc.StandardError.ReadToEnd();
                        var output = proc.StandardOutput.ReadToEnd();
                        tcs.SetException(new GitException("Failed to execute git",
                            error,
                            output,
                            proc.ExitCode,
                            _git.ParseErrorCode(error),
                            proc.StartInfo.Arguments));
                    }
                };
                proc.Start();
                if (!string.IsNullOrEmpty(stdin))
                {
                    using (proc.StandardInput)
                    {
                        await proc.StandardInput.WriteAsync(stdin);
                    }
                }

                return new ExecutionResult(await tcs.Task,
                    await proc.StandardOutput.ReadToEndAsync(),
                    await proc.StandardError.ReadToEndAsync());
            }
        }

        public Corgit(string gitPath, string workingDirectory)
        {
            _gitPath = gitPath;
            _workingDirectory = workingDirectory;
        }

        public async Task<ExecutionResult> StatusAsync()
            => await RunGitAsync("status");

        public async Task<ExecutionResult> InitAsync()
            => await RunGitAsync("init");

        public async Task<IEnumerable<GitCommit>> LogAsync(Git.LogOptions options = default)
        {
            var result = await RunGitAsync(_git.Log(options));
            if (result.ExitCode == 1)
            {
                return Enumerable.Empty<GitCommit>();
            }
            else
            {
                return _git.ParseLog(result.Output);
            }
        }

        public async Task<ExecutionResult> CommitAsync(string message = "", Git.CommitOptions options = default)
            => await RunGitAsync(_git.Commit(options), message);

        public Task<ExecutionResult> AddAsync(params string[] paths)
            => AddAsync(paths.AsEnumerable());

        public async Task<ExecutionResult> AddAsync(IEnumerable<string> paths)
            => await RunGitAsync(_git.Add(paths));

        public async Task<ExecutionResult> ConfigAsync(string key, string value = null, string scope = null)
            => await RunGitAsync(_git.Config(key, value, scope));
    }
}
