using DuplicateDetection.Abstractions;
using System.Collections.Generic;

namespace DuplicateDetection
{
    public class CashingFileHashService : IFileHashService
    {
        private readonly IFileHashService fileHashService;
        private readonly Dictionary<string, byte[]> hashCache = new Dictionary<string, byte[]>();

        public CashingFileHashService(IFileHashService fileHashService)
        {
            this.fileHashService = fileHashService;
        }

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
