using System;
using System.Collections.Generic;
using System.Text;

namespace DuplicateDetection.Abstractions
{
    public interface IDuplicateFile
    {
        IEnumerable<string> FilePaths { get; }
    }
}
