using System;
using System.Collections.Generic;
using System.Text;

namespace corgit
{
    public class GitException : Exception
    {
        internal GitException(string message,
                              string error,
                              string output,
                              int exitCode,
                              string gitCommand) : base(message)
        {
            this.Error = error;
            this.Output = output;
            this.ExitCode = exitCode;
            this.GitError = GitParsing.ParseErrorCode(error);
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
