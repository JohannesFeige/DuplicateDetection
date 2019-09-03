using DuplicateDetection.Abstractions;
using System.Security.Cryptography;

namespace DuplicateDetection
{
    public class FileHashService : IFileHashService
    {
        /// <summary>
        /// Calculates a MD5 hash of a file.
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns></returns>
        public byte[] CalculateHash(string path)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = System.IO.File.OpenRead(path))
                {
                    return md5.ComputeHash(stream);
                }
            }
        }
    }
}
