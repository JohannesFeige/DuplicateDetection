using System.Collections.Generic;
using System.Linq;
using DuplicateDetection.Abstractions;

namespace DuplicateDetection
{
    public class DuplicateDetectionService : IDuplicateDetectionService
    {
        private readonly IFileCrawler fileCrawler;
        private readonly IFileHashService fileHashService;

        public DuplicateDetectionService(IFileCrawler fileCrawler, IFileHashService fileHashService)
        {
            this.fileCrawler = fileCrawler;
            this.fileHashService = fileHashService;
        }

        /// <inheritdoc />
        public IEnumerable<IDuplicateFile> CollectCandidates(string path)
            => CollectCandidates(path, ComparisonMode.SizeAndName);

        /// <inheritdoc />
        public IEnumerable<IDuplicateFile> CollectCandidates(string path, ComparisonMode mode)
        {
            var result = new HashSet<IDuplicateFile>(new DuplicateFileComparer());
            var files = fileCrawler.CrawlFiles(path).ToList();

            foreach (var file in files)
            {
                var duplicates = files.Where(x => SimpleCompare(x, file, mode));
                if (duplicates.Count() > 1)
                {
                    result.Add(new DuplicateFile
                    {
                        FilePaths = duplicates.Select(x => x.Path)
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Compares files depending on comparison mode.
        /// </summary>
        /// <param name="left">left file</param>
        /// <param name="right">right file</param>
        /// <param name="mode">comparison mode</param>
        /// <returns></returns>
        private bool SimpleCompare(File left, File right, ComparisonMode mode)
            => mode == ComparisonMode.SizeAndName
                ? left.Name == right.Name && left.Size == right.Size
                : left.Size == right.Size;

        /// <inheritdoc />
        public IEnumerable<IDuplicateFile> VerifyCandidates(IEnumerable<IDuplicateFile> candidates)
        {
            var result = new HashSet<IDuplicateFile>(new DuplicateFileComparer());

            foreach (var candidate in candidates)
            {
                foreach (var filePath in candidate.FilePaths)
                {
                    var duplicatePaths = candidate.FilePaths.Where(x => HashCompare(x, filePath));
                    if (duplicatePaths.Count() > 1)
                    {
                        result.Add(new DuplicateFile
                        {
                            FilePaths = duplicatePaths
                        });
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Compares files by hash using FileHashService.
        /// </summary>
        /// <param name="left">left file</param>
        /// <param name="right">right file</param>
        /// <returns></returns>
        private bool HashCompare(string left, string right)
        {
            var leftHash = fileHashService.CalculateHash(left);
            var rightHash = fileHashService.CalculateHash(right);
            return leftHash.SequenceEqual(rightHash);
        }

        /// <inheritdoc />
        private class DuplicateFileComparer : IEqualityComparer<IDuplicateFile>
        {
            /// <inheritdoc />
            public bool Equals(IDuplicateFile left, IDuplicateFile right)
                => left.FilePaths.Count() == right.FilePaths.Count()
                    && left.FilePaths.All(x => right.FilePaths.Contains(x));

            /// <inheritdoc />
            public int GetHashCode(IDuplicateFile obj)
                => obj.FilePaths.Aggregate(0, (current, path) => current ^ path.GetHashCode());
        }
    }
}
