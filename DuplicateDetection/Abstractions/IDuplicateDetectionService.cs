using System.Collections.Generic;

namespace DuplicateDetection.Abstractions
{
    public interface IDuplicateDetectionService
    {
        /// <summary>
        /// Collects all files in path and compares with SizeAndName as default mode.
        /// </summary>
        /// <param name="path">directory path</param>
        /// <returns></returns>
        IEnumerable<IDuplicateFile> CollectCandidates(string path);
        /// <summary>
        /// Collects all files in path and compares with given mode.
        /// </summary>
        /// <param name="path">directory path</param>
        /// <param name="mode">mode for comparison</param>
        /// <returns></returns>
        IEnumerable<IDuplicateFile> CollectCandidates(string path, ComparisonMode mode);
        /// <summary>
        /// Verifies that possible duplicates are identical by checking MD5 hash.
        /// </summary>
        /// <param name="candidates">possible duplicates</param>
        /// <returns></returns>
        IEnumerable<IDuplicateFile> VerifyCandidates(IEnumerable<IDuplicateFile> candidates);
    }
}
