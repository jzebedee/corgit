//https://github.com/Microsoft/vscode/tree/master/extensions/git

namespace corgit
{
    public class GitFileStatus
    {
        public char X { get; set; }
        public char Y { get; set; }
        public string Path { get; set; }
        public string Rename { get; set; }
    }
}
