using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using RomFileReader.Libraries;
using System;
using System.Reactive.Linq;

namespace RomFileReader.UI
{
    public class RomInfoViewModel : ReactiveObject
    {
        public RomInfoViewModel(RomInfo romInfo, IFileManager fileManager)
        {
            Title = romInfo.GameTitle ?? "NA";
            Country = romInfo.CountryName ?? "NA";
            FileName = romInfo.FileName!;
            Exists = fileManager.Exists(FileName);
            ImagePath = fileManager.GetImagePath(FileName);

            this.WhenPropertyChanged(x => x.Exists)
                .Skip(1)
                .Subscribe(x =>
                {
                    if (x.Value)
                        fileManager.Copy(FileName);
                    else
                        fileManager.Remove(FileName);
                });
        }
        public string FileName { get; }

        public string Title { get; }

        public string Country { get; }

        public string? ImagePath { get; }

        [Reactive]
        public bool Exists { get; set; }
    }
}
