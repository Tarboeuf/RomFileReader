using System.Collections.Generic;
using System.IO;

namespace RomFileReader.UI
{
    public interface IFileManager
    {
        IEnumerable<FileInfo> GetSuperNesFiles();

        void Copy(string fileName);

        void Remove(string fileName);

        bool Exists(string fileName);

        bool SfcExists();

        string? GetImagePath(string fileName);
    }
}
