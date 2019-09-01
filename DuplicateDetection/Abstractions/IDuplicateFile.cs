using System.Collections.Generic;

namespace DuplicateDetection.Abstractions
{
    public interface IDuplicateFile
    {
        IEnumerable<string> FilePaths { get; }
    }
}
