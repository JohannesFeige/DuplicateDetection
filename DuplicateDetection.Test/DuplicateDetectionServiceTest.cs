using DuplicateDetection.Abstractions;
using NSubstitute;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
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

            //todo: check for file paths
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
            var duplicates = service.CollectCandidates("path");
            duplicates.Count().ShouldBe(2);

            // todo: check for filepaths
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
        public void ShouldVerfiyCandidates()
        {
            var firstFilePath = "foo.txt";
            var secondFilePath = "bar.txt";
            var hash = new byte[] { 0x00 };

            fileHashService.CalculateHash(firstFilePath).Returns(hash);
            fileHashService.CalculateHash(secondFilePath).Returns(hash);

            var candiates = Substitute.For<IDuplicateFile>();
            candiates.FilePaths.Returns(new List<string> { firstFilePath, secondFilePath });

            var files = service.VerifyCandiates(new List<IDuplicateFile> { candiates });

            files.Count().ShouldBe(1);
            files.Single().FilePaths.ShouldContain(firstFilePath);
            files.Single().FilePaths.ShouldContain(secondFilePath);
        }
        #endregion
    }
}
