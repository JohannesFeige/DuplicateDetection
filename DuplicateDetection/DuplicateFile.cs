using DuplicateDetection.Abstractions;
using System.Collections.Generic;

namespace DuplicateDetection
{
    /// <inheritdoc />
    internal class DuplicateFile : IDuplicateFile
    {
        public IEnumerable<string> FilePaths { get; internal set; }
    }
}
