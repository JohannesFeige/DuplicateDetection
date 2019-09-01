using System;
using System.Collections.Generic;
using System.Text;

namespace DuplicateDetection.Abstractions
{
    public interface IDuplicateDetectionService
    {
        IEnumerable<IDuplicateFile> CollectCandidates(string path);
        IEnumerable<IDuplicateFile> CollectCandidates(string path, ComparisonMode mode);
        IEnumerable<IDuplicateFile> VerifyCandiates(IEnumerable<IDuplicateFile> candidates);
    }
}
