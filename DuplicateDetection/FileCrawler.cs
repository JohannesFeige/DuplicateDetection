using DuplicateDetection.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using File = DuplicateDetection.Abstractions.File;

namespace DuplicateDetection
{
    public class FileCrawler : IFileCrawler
    {
        public IEnumerable<File> CrawlFiles(string directoryPath)
        {
            var directory = new DirectoryInfo(directoryPath);

            return CrawlFilesImpl(directory);
        }

        private IEnumerable<File> CrawlFilesImpl(DirectoryInfo directory)
        {
            var files = new List<File>();
            foreach(var childDirectory in directory.GetDirectories())
            {
                files.AddRange(CrawlFilesImpl(childDirectory));
            }

            files.AddRange(directory.GetFiles().Select(x => new File(x.Name, x.Length, x.FullName)));
            return files;
        }
    }
}
