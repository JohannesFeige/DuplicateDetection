using System.Collections.Generic;

namespace DuplicateDetection.Abstractions
{
    public interface IFileCrawler
    {
        IEnumerable<File> CrawlFiles(string directoryPath);
    }
}
