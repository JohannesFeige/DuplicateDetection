using DuplicateDetection.Abstractions;
using System.Collections.Generic;

namespace DuplicateDetection
{
    class DuplicateFile : IDuplicateFile
    {
        public IEnumerable<string> FilePaths { get; internal set; }
    }
}
