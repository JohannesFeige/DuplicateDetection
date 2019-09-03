namespace DuplicateDetection.Models
{
    /// <summary>
    /// DTO for file information
    /// </summary>
    public class File
    {
        public string Name { get; }
        public long Size { get; }

        public string Path { get; }

        public File(string name, long size, string path)
        {
            Name = name;
            Size = size;
            Path = path;
        }
    }
}
