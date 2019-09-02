using DuplicateDetection.Abstractions;
using System.Collections.Generic;

namespace DuplicateDetection
{
    /// <summary>
    /// Decorator for FileHashService with simple caching.
    /// </summary>
    public class CachingFileHashService : IFileHashService
    {
        private readonly IFileHashService fileHashService;
        private readonly Dictionary<string, byte[]> hashCache = new Dictionary<string, byte[]>();

        public CachingFileHashService(IFileHashService fileHashService)
        {
            this.fileHashService = fileHashService;
        }

        /// <inheritdoc />
        public byte[] CalculateHash(string path)
        {
            if (hashCache.TryGetValue(path, out var hash))
            {
                return hash;
            }

            var calculatedHash = fileHashService.CalculateHash(path);
            hashCache.Add(path, calculatedHash);
            return calculatedHash;
        }
    }
}
