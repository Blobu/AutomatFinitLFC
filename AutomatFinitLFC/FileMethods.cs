using System.IO;

namespace AutomatFinitLFC
{
    public class FileMethods
    {
        public string FilePath { get; set; }
        public string FileContent { get; }

        public FileMethods(string filePath)
        {
            this.FilePath = filePath;
            this.FileContent = File.ReadAllText(filePath);
        }
    }
}