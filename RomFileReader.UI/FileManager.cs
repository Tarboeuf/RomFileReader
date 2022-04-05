using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;

namespace RomFileReader.UI
{
    public class FileManager : IFileManager
    {
        private readonly ISettings settings;

        public FileManager(ISettings settings)
        {
            this.settings = settings;
        }

        public IEnumerable<FileInfo> GetSuperNesFiles()
        {
            DirectoryInfo directory = new DirectoryInfo(GetSfcPath());
            return directory.EnumerateFiles("*.sfc").Concat(directory.EnumerateFiles("*.smc"));
        }

        public void Copy(string fileName)
        {
            if(!Directory.Exists(GetSdCardSfcPath()))
            {
                throw new DirectoryNotFoundException();
            }
            string path = Path.Combine(GetSdCardSfcPath(), fileName);
            if (File.Exists(path)) return;

            File.Copy(Path.Combine(GetSfcPath(), fileName), path);
        }

        public void Remove(string fileName)
        {
            if (!Directory.Exists(GetSdCardSfcPath()))
            {
                throw new DirectoryNotFoundException();
            }
            string path = Path.Combine(GetSdCardSfcPath(), fileName);
            if (!File.Exists(path)) return;

            File.Delete(path);
        }

        public bool Exists(string fileName)
        {
            if (!Directory.Exists(GetSdCardSfcPath()))
            {
                return false;
            }
            string path = Path.Combine(GetSdCardSfcPath(), fileName);
            return File.Exists(path);
        }

        public string? GetImagePath(string fileName)
        {
            string imageFilePath = Path.Combine(GetSfcPath(), Path.ChangeExtension(fileName, ".png"));
            if(File.Exists(imageFilePath))
            {
                return imageFilePath;
            }
            var assembly = Assembly.GetExecutingAssembly();
            return Path.Combine(Path.GetDirectoryName(assembly.Location)!, "na.png");
        }

        private string GetSdCardSfcPath()
        {
            return Path.Combine(settings.SdCardGameFilePath!, "sfc");
        }
        private string GetSfcPath()
        {
            return Path.Combine(settings.LocalGameFilePath!, "sfc");
        }

        public bool SfcExists()
        {
            if(settings.LocalGameFilePath == null) return false;
            return Directory.Exists(GetSfcPath());
        }
    }
}
