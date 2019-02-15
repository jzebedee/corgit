namespace corgit.Git
{
    /// <summary>
    /// Modifier representing the status of a path
    /// </summary>
    /// <remarks>https://git-scm.com/docs/git-status#_short_format</remarks>
    public enum GitChangeType : byte
    {
        Unmodified = (byte)' ',
        Untracked = (byte)'?',
        Ignored = (byte)'!',
        Added = (byte)'A',
        Modified = (byte)'M',
        Deleted = (byte)'D',
        Renamed = (byte)'R',
        Copied = (byte)'C',
        /// <summary>
        /// Updated but unmerged
        /// </summary>
        Updated = (byte)'U',
    }
}
