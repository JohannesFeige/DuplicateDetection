namespace DuplicateDetection.Abstractions
{
    public interface IFileHashService
    {
        /// <summary>
        /// Calculates a hash of a file.
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns></returns>
        byte[] CalculateHash(string path);
    }
}
