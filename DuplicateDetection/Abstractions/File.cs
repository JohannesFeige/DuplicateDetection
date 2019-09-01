namespace DuplicateDetection.Abstractions
{
    public class File
    {
        public string Name { get; }
        public long Size { get; }

        public string Path { get; }

        public File(string name, long size, string path)
        {
            this.Name = name;
            this.Size = size;
            this.Path = path;
        }
    }
}
