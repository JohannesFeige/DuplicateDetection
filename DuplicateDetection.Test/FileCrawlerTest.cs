using DuplicateDetection.Abstractions;
using Shouldly;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace DuplicateDetection.Test
{
    public class FileCrawlerTest : IDisposable
    {
        private readonly IFileCrawler fileCrawler = new FileCrawler();
        private string directoryPath;

        public void Dispose()
        {
            Directory.Delete(directoryPath, true);
        }

        [Fact]
        public void ShouldCrawlFiles()
        {
            var directoryPath = this.CreateDirectory("foo.txt");

            var files = this.fileCrawler.CrawlFiles(directoryPath);

            files.Count().ShouldBe(1);
        }

        [Fact]
        public void ShouldCrawlRecursively()
        {
            var directoryPath = this.CreateDirectory("foo/foo.txt");

            var files = this.fileCrawler.CrawlFiles(directoryPath);

            files.Count().ShouldBe(1);
        }

        [Fact]
        public void ShouldCrawlRecursivelyWithDeepNesting()
        {
            var directoryPath = this.CreateDirectory("foo/foo/foo.txt", "foo/foo/bar.txt", "bar.txt");

            var files = this.fileCrawler.CrawlFiles(directoryPath);

            files.Count().ShouldBe(3);
        }

        private string CreateDirectory(params string[] filePaths)
        {
            var path = directoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            foreach(var filePath in filePaths)
            {
                var fileInfo = new FileInfo(Path.Combine(path, filePath));
                fileInfo.Directory.Create();
                fileInfo.Create().Close();
            }
            return path;
        }
    }
}
