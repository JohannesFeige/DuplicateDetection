using System.Collections.Generic;
using DuplicateDetection.Models;

namespace DuplicateDetection.Abstractions
{
    public interface IFileCrawler
    {
        /// <summary>
        /// Seek files recursively in given directory path
        /// </summary>
        /// <param name="directoryPath">directory path</param>
        /// <returns></returns>
        IEnumerable<File> CrawlFiles(string directoryPath);
    }
}
