using DuplicateDetection.Abstractions;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using DuplicateDetection.Models;
using Xunit;

namespace DuplicateDetection.Test
{
    public class DuplicateDetectionServiceTest
    {
        private readonly IDuplicateDetectionService service;
        private readonly IFileCrawler fileCrawler = Substitute.For<IFileCrawler>();
        private readonly IFileHashService fileHashService = Substitute.For<IFileHashService>();

        public DuplicateDetectionServiceTest()
        {
            service = new DuplicateDetectionService(fileCrawler, fileHashService);
        }

        #region CollectCandiates
        [Fact]
        public void ShouldDetectDuplicatesWithSameNameAndSizeInDefaultMode()
        {
            new FileCrawlerMockBuilder()
                .WithSameName()
                .WithSameSize()
                .Build(fileCrawler);

            var duplicates = service.CollectCandidates("path");
            duplicates.Count().ShouldBe(1);
        }

        [Fact]
        public void ShouldNotDetectDuplicatesWithDifferentNamesInDefaultMode()
        {
            new FileCrawlerMockBuilder()
                .WithDifferentNames()
                .WithSameSize()
                .Build(fileCrawler);

            var duplicates = service.CollectCandidates("path");
            duplicates.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldNotDetectDuplicatesWithDifferentSizeInDefaultMode()
        {
            new FileCrawlerMockBuilder()
                .WithSameName()
                .WithDifferentSizes()
                .Build(fileCrawler);
            var duplicates = service.CollectCandidates("path");
            duplicates.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldDetectDuplicatesWithSameNameAndSameSizeInSizeMode()
        {
            new FileCrawlerMockBuilder()
                .WithSameName()
                .WithSameSize()
                .Build(fileCrawler);

            var duplicates = service.CollectCandidates("path", ComparisonMode.Size);
            duplicates.Count().ShouldBe(1);
        }

        [Fact]
        public void ShouldDetectDuplicatesWithSameSizeInSizeMode()
        {
            new FileCrawlerMockBuilder()
                .WithDifferentNames()
                .WithSameSize()
                .Build(fileCrawler);
            
            var duplicates = service.CollectCandidates("path", ComparisonMode.Size);
            duplicates.Count().ShouldBe(1);
        }

        [Fact]
        public void ShouldNotDetectDuplicatesWithDifferentSizeInSizeMode()
        {
            new FileCrawlerMockBuilder()
                .WithDifferentNames()
                .WithDifferentSizes()                
                .Build(fileCrawler);

            var duplicates = service.CollectCandidates("path", ComparisonMode.Size);
            duplicates.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldDetectMultipleGroupsOfDuplicates()
        {
            fileCrawler.CrawlFiles("path").Returns(new List<File>
            {
                new File("foo", 13, @"c:\foo\foo.txt"),
                new File("foo", 13, @"c:\bar\foo.txt"),
                new File("foo", 13, @"c:\baz\foo.txt"),
                new File("bar", 14, @"c:\foo\bar.txt"),
                new File("bar", 14, @"c:\bar\bar.txt"),
            });

            var duplicates = service.CollectCandidates("path").ToList();

            duplicates.Count.ShouldBe(2);
            duplicates.ShouldContain(x => x.FilePaths.Count() == 3 && x.FilePaths.All(p => p.EndsWith("foo.txt")));
            duplicates.ShouldContain(x => x.FilePaths.Count() == 2 && x.FilePaths.All(p => p.EndsWith("bar.txt")));
        }

        private class FileCrawlerMockBuilder
        {
            private string file1, file2;
            private int size1, size2;

            internal FileCrawlerMockBuilder WithSameName()
            {
                file1 = file2 = "foo";
                return this;
            }

            internal FileCrawlerMockBuilder WithDifferentNames()
            {
                file1 = "foo";
                file2 = "bar";
                return this;
            }

            internal FileCrawlerMockBuilder WithSameSize()
            {
                size1 = size2 = 12;
                return this;
            }

            internal FileCrawlerMockBuilder WithDifferentSizes()
            {
                size1 = 13;
                size2 = 12;
                return this;
            }

            internal void Build(IFileCrawler crawler)
            {
                crawler.CrawlFiles("path").Returns(new List<File>
                {
                    new File(file1, size1, $@"c:\{file1}"),
                    new File(file2, size2, $@"c:\{file2}"),
                }); ;
            }
        }

        #endregion

        #region VerifyCandidates
        [Fact]
        public void ShouldVerifyCandidates()
        {
            var firstFilePath = "foo.txt";
            var secondFilePath = "bar.txt";
            var hash = new byte[] { 0x00 };

            fileHashService.CalculateHash(firstFilePath).Returns(hash);
            fileHashService.CalculateHash(secondFilePath).Returns(hash);

            var candidates = Substitute.For<IDuplicateFile>();
            candidates.FilePaths.Returns(new List<string> { firstFilePath, secondFilePath });

            var files = service.VerifyCandidates(new List<IDuplicateFile> { candidates }).ToList();

            files.Count.ShouldBe(1);
            files.Single().FilePaths.ShouldContain(firstFilePath);
            files.Single().FilePaths.ShouldContain(secondFilePath);
        }
        #endregion
    }
}
