using System.Collections.Generic;
using System.Linq;
using DuplicateDetection.Abstractions;

namespace DuplicateDetection.Console
{
    public class Application
    {
        private readonly IDuplicateDetectionService duplicateDetectionService;

        public Application(IDuplicateDetectionService duplicateDetectionService)
        {
            this.duplicateDetectionService = duplicateDetectionService;
        }

        public void Run(string path, ComparisonMode mode)
        {
            PerformFirstPass(path, mode);
        }

        private void PerformFirstPass(string path, ComparisonMode mode)
        {
            var duplicateFiles = duplicateDetectionService.CollectCandidates(path, mode).ToList();
            if (duplicateFiles.Any())
            {
                PerformSecondPass(duplicateFiles, path, mode);
            }
            else
            {
                System.Console.WriteLine($"No duplicate files found in {path} for mode ${mode} in first pass.");
            }
        }

        private void PerformSecondPass(IEnumerable<IDuplicateFile> duplicateFiles, string path, ComparisonMode mode)
        {
            var candidates = duplicateDetectionService.VerifyCandidates(duplicateFiles).ToList();
            if (candidates.Any())
            {
                System.Console.WriteLine($"Found {candidates.Count} duplicate(s):");
                System.Console.WriteLine();

                foreach (var candidate in candidates)
                {
                    foreach (var filePath in candidate.FilePaths)
                    {
                        System.Console.WriteLine($"{filePath}");
                    }

                    System.Console.WriteLine();
                }
            }
            else
            {
                System.Console.WriteLine($"No duplicate files found in {path} for mode ${mode} in second pass.");
            }
        }
    }
}
