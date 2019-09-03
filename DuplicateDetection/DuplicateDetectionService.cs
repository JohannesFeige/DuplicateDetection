using System;
using System.Collections.Generic;
using System.Linq;
using DuplicateDetection.Abstractions;
using DuplicateDetection.Models;

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
        public IEnumerable<IDuplicateFile> CollectCandidates(string path, ComparisonMode mode) =>
            fileCrawler
                .CrawlFiles(path)
                .GroupBy(x => x, new SimpleComparer(mode))
                .Where(grouping => grouping.Count() > 1)
                .Select(grouping => new DuplicateFile
                {
                    FilePaths = grouping.Select(file => file.Path)
                });

        /// <inheritdoc />
        public IEnumerable<IDuplicateFile> VerifyCandidates(IEnumerable<IDuplicateFile> candidates)
        {
            return candidates
                .SelectMany(candidate => candidate.FilePaths
                    .GroupBy(filePath => filePath, new HashComparer(fileHashService))
                    .Where(innerGrouping => innerGrouping.Count() > 1)
                    .Select(innerGrouping => new DuplicateFile
                    {
                        FilePaths = innerGrouping
                    }));
        }

        /// <summary>
        /// Compares files depending on comparison mode.
        /// </summary>
        private class SimpleComparer : IEqualityComparer<File>
        {
            private readonly ComparisonMode mode;

            public SimpleComparer(ComparisonMode mode)
            {
                this.mode = mode;
            }

            public bool Equals(File left, File right)
                => mode == ComparisonMode.SizeAndName
                    ? left.Name == right.Name && left.Size == right.Size
                    : left.Size == right.Size;

            public int GetHashCode(File obj)
            {
                var hash = obj.Size.GetHashCode();

                if (mode == ComparisonMode.SizeAndName)
                {
                    hash ^= obj.Name.GetHashCode();
                }

                return hash;
            }
        }

        /// <summary>
        /// Compares files by hash using FileHashService.
        /// </summary>
        private class HashComparer : IEqualityComparer<string>
        {
            private readonly IFileHashService fileHashService;

            public HashComparer(IFileHashService fileHashService)
            {
                this.fileHashService = fileHashService;
            }

            public bool Equals(string left, string right)
            {
                var leftHash = fileHashService.CalculateHash(left);
                var rightHash = fileHashService.CalculateHash(right);
                return leftHash.SequenceEqual(rightHash);
            }

            public int GetHashCode(string path)
            {
                var rawHash = fileHashService.CalculateHash(path);
                // fileHashService return byte[1] for empty files:
                return rawHash.Length >= 4
                    ? BitConverter.ToInt32(rawHash, 0)
                    : 0;
            }
        }
    }
}
