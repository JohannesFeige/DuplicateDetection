using System;
using System.Collections.Generic;
using System.Linq;
using DuplicateDetection.Abstractions;

namespace DuplicateDetection
{
    public class DuplicateDetectionService : IDuplicateDetectionService
    {
        private readonly IFileCrawler fileCrawler;
        public DuplicateDetectionService(IFileCrawler fileCrawler)
        {
            this.fileCrawler = fileCrawler;
        }

        public IEnumerable<IDuplicateFile> CollectCandidates(string path)
            => this.CollectCandidates(path, ComparisonMode.SizeAndName);

        public IEnumerable<IDuplicateFile> CollectCandidates(string path, ComparisonMode mode)
        {
            var result = new HashSet<IDuplicateFile>(new DuplicateFileComparer());
            var files = this.fileCrawler.CrawlFiles(path).ToList();

            

            foreach (var file in files)
            {
                var duplicates = files.Where(x => this.CompareFiles(x, file, mode));
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

        private bool CompareFiles(File left, File right, ComparisonMode mode)
            => mode == ComparisonMode.SizeAndName
                ? left.Name == right.Name && left.Size == right.Size
                : left.Size == right.Size;        

        public IEnumerable<IDuplicateFile> VerifyCandiates(IEnumerable<IDuplicateFile> candidates)
        {
            throw new NotImplementedException();
        }        

        private class DuplicateFileComparer : IEqualityComparer<IDuplicateFile>
        {
            public bool Equals(IDuplicateFile left, IDuplicateFile right)
                => left.FilePaths.Count() == right.FilePaths.Count()
                    && left.FilePaths.All(x => right.FilePaths.Contains(x));

            public int GetHashCode(IDuplicateFile obj)
                => obj.FilePaths.Aggregate(0, (current, path) => current ^ path.GetHashCode());
        }
    }
}
