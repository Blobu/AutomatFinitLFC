using System.IO;

class FileMethods
{
    public string filePath { get; set; }
    public string fileContent {get;}

    public FileMethods(string filePath)
    {
        this.filePath = filePath;
        this.fileContent = File.ReadAllText(filePath);
    }
}