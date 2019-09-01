using DuplicateDetection.Abstractions;
using Shouldly;
using System.IO;
using Xunit;

namespace DuplicateDetection.Test
{
    public class FileHashServiceTest
    {
        private readonly IFileHashService fileHashService = new FileHashService();

        [Fact]
        public void ShouldGetHash()
        {
            var filePath = CreateFile("foo.txt");
            var hash = fileHashService.CalculateHash(filePath);

            var expectation = new byte[] { 0xB1, 0x0A, 0x8D, 0xB1, 0x64, 0xE0, 0x75, 0x41, 0x05, 0xB7, 0xA9, 0x9B, 0xE7, 0x2E, 0x3F, 0xE5 };
            hash.ShouldBe(expectation);
        }

        private string CreateFile(string filePath)
        {
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());            
            var fileInfo = new FileInfo(Path.Combine(path, filePath));
            fileInfo.Directory.Create();

            using (var writer = new StreamWriter(fileInfo.Create()))
            {
                writer.Write("Hello World");
            }            

            return fileInfo.FullName;
        }
    }
}
