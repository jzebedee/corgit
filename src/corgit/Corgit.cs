using corgit.Git;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace corgit
{
    public class Corgit
    {
        private readonly string _gitPath;
        private readonly string _workingDirectory;

        private Process CreateGitProcess(string arguments = "", IEnumerable<KeyValuePair<string, string>> env = null)
        {
            var proc = new Process()
            {
                StartInfo = new ProcessStartInfo(_gitPath, arguments)
                {
                    WorkingDirectory = _workingDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
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
                                                       IEnumerable<KeyValuePair<string, string>> env = null)
        {
            using (var proc = CreateGitProcess(arguments, env))
            {
                proc.StartInfo.RedirectStandardInput = stdin != null;
                var t = StartGitProcessAsync(proc);

                if (!string.IsNullOrEmpty(stdin))
                {
                    using (proc.StandardInput)
                    {
                        await proc.StandardInput.WriteAsync(stdin);
                    }
                }

                return await t;
            }
        }

        public Task<ExecutionResult> RunGitAsync(IEnumerable<string> arguments,
                                                string stdin = null,
                                                IEnumerable<KeyValuePair<string, string>> env = null)
           => RunGitAsync(string.Join(" ", arguments), stdin, env);

        public Corgit(string gitPath, string workingDirectory)
        {
            _gitPath = gitPath;
            _workingDirectory = workingDirectory;
        }

        public async Task<IEnumerable<GitFileStatus>> StatusAsync()
            => GitParsing.ParseStatus((await RunGitAsync(GitArguments.Status())).Output);

        public async Task<ExecutionResult> InitAsync()
            => await RunGitAsync(GitArguments.Init());

        public async Task<IEnumerable<GitCommit>> LogAsync(GitArguments.LogOptions options = default)
        {
            var result = await RunGitAsync(GitArguments.Log(options));
            if (result.ExitCode == 1)
            {
                return Enumerable.Empty<GitCommit>();
            }
            else
            {
                return GitParsing.ParseLog(result.Output);
            }
        }

        public async Task<ExecutionResult> CommitAsync(string message = "", GitArguments.CommitOptions options = default)
            => await RunGitAsync(GitArguments.Commit(options), message);

        public Task<ExecutionResult> AddAsync(params string[] paths)
            => AddAsync(paths.AsEnumerable());

        public async Task<ExecutionResult> AddAsync(IEnumerable<string> paths)
            => await RunGitAsync(GitArguments.Add(paths));

        public async Task<ExecutionResult> ConfigAsync(string key, string value = null, string scope = null)
            => await RunGitAsync(GitArguments.Config(key, value, scope));
    }
}
