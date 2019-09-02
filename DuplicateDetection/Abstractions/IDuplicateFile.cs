using System.Collections.Generic;

namespace DuplicateDetection.Abstractions
{
    /// <summary>
    /// Interface for group of duplicate files.
    /// </summary>
    public interface IDuplicateFile
    {
        IEnumerable<string> FilePaths { get; }
    }
}
