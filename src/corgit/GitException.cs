using System;
using System.Collections.Generic;
using System.Text;
using static corgit.Git;

namespace corgit
{
    public class GitException : Exception
    {
        internal GitException(string message,
                              string error,
                              string output,
                              int exitCode,
                              GitErrorCode? gitError,
                              string gitCommand) : base(message)
        {
            this.Error = error;
            this.Output = output;
            this.ExitCode = exitCode;
            this.GitError = gitError;
            this.GitCommand = gitCommand;
        }

        public string Error { get; }

        public string Output { get; }

        public int ExitCode { get; }

        public GitErrorCode? GitError { get; }

        public string GitCommand { get; }

        public override string Message => base.Message ?? "Git error";
    }
}
