using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DuplicateDetection.Abstractions;
using Shouldly;
using Xunit;

namespace DuplicateDetection.Test
{
    public class IntegrationTest : IDisposable
    {
        private readonly IDuplicateDetectionService service = new DuplicateDetectionService(
            new FileCrawler(),
            new CashingFileHashService(
                new FileHashService()
            )
        );

        private string createdDirectoryPath;

        public void Dispose()
        {
            Directory.Delete(createdDirectoryPath, true);
        }

        [Fact]
        public void ShouldDetectDuplicates()
        {
            var directoryPath = SetupTests();

            var duplicates = service.CollectCandidates(directoryPath).ToList();

            duplicates.Count.ShouldBe(2);
            duplicates.ShouldContain(x => x.FilePaths.Count() == 3 && x.FilePaths.All(p => p.EndsWith("foo.txt")));
            duplicates.ShouldContain(x => x.FilePaths.Count() == 2 && x.FilePaths.All(p => p.EndsWith("bar.txt")));
        }

        [Fact]
        public void ShouldVerifyCandidates()
        {
            var directoryPath = SetupTests();

            var candidates = service.CollectCandidates(directoryPath);
            var files = service.VerifyCandidates(candidates).ToList();

            files.Count.ShouldBe(2);
            files.ShouldContain(x => x.FilePaths.Count() == 3 && x.FilePaths.All(p => p.EndsWith("foo.txt")));
            files.ShouldContain(x => x.FilePaths.Count() == 2 && x.FilePaths.All(p => p.EndsWith("bar.txt")));
        }

        private string SetupTests()
            => CreateFiles(new List<(string content, string path)>
            {
                (content: "foo", path: @"foo\foo.txt"),
                (content: "foo", path: @"bar\foo.txt"),
                (content: "foo", path: @"baz\foo.txt"),
                (content: "bar", path: @"foo\bar.txt"),
                (content: "bar", path: @"bar\bar.txt"),
            });

        private string CreateFiles(IEnumerable<(string content, string path)> files)
        {
            createdDirectoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(Path.Combine(createdDirectoryPath, file.path));
                fileInfo.Directory.Create();

                using (var writer = new StreamWriter(fileInfo.Create()))
                {
                    writer.Write(file.content);
                }
            }

            return createdDirectoryPath;
        }
    }
}
