namespace DuplicateDetection.Abstractions
{
    public interface IFileHashService
    {
        byte[] CalculateHash(string path);
    }
}
