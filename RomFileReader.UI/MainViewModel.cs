using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using RomFileReader.Libraries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RomFileReader.UI
{
    public class MainViewModel : ReactiveObject, IActivatableViewModel
    {
        private readonly IFileManager fileManager;
        private readonly IRomDataExtractor dataExtractor;

        public MainViewModel(IFileManager fileManager, IRomDataExtractor dataExtractor, ISettingsValidatable settings)
        {
            this.WhenActivated(d =>
            {
                var validPathObservable = settings.WhenPropertyChanged(s => s.LocalGameFilePath)
                    .Where(_ => fileManager.SfcExists())
                    .Select(s => s.Value)
                    .DistinctUntilChanged()
                    .Select(_ => Unit.Default);

                Observable.Return(Unit.Default).Merge(validPathObservable)
                    .Do(_ => IsLoading = true)
                    .ObserveOn(RxApp.TaskpoolScheduler)
                    .SelectMany(_ => LoadGames())
                    .Select(ToViewModel)
                    .Select(x => new ObservableCollectionExtended<RomInfoViewModel>(x))
                    .Select(x =>
                    {
                        var view = CollectionViewSource.GetDefaultView(x);
                        view.Filter = Filter;
                        return view;
                    })
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Finally(() => IsLoading = false)
                    .Subscribe(x =>
                    {
                        Games = x;
                        IsLoading = false;
                    })
                    .DisposeWith(d);

                this.WhenAnyValue(vm => vm.GameTitle, vm => vm.Country)
                    .Select(_ => Games as ICollectionView)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(s => s?.Refresh())
                    .DisposeWith(d);

                AreSettingsFine = settings.ValidationContext.GetIsValid();
            });
            this.fileManager = fileManager;
            this.dataExtractor = dataExtractor;
            Settings = settings;
        }

        [Reactive]
        public string? GameTitle { get; set; }

        [Reactive]
        public string? Country { get; set; }

        [Reactive]
        public bool IsLoading { get; set; }

        [Reactive]
        public IEnumerable? Games { get; set; }

        [Reactive]
        public bool AreSettingsFine { get; set; }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public ISettingsValidatable Settings { get; }

        private async Task<List<RomInfo>> LoadGames()
        {
            List<RomInfo> list = new List<RomInfo>();
            if (!fileManager.SfcExists())
            {
                return list;
            }
            foreach (FileInfo file in fileManager.GetSuperNesFiles())
            {
                var rom = await dataExtractor.GetName(file);
                if (rom == null) continue;
                list.Add(rom);
            }
            return list;
        }

        private IEnumerable<RomInfoViewModel> ToViewModel(IEnumerable<RomInfo> roms)
        {
            return roms.Select(r => new RomInfoViewModel(r, fileManager));
        }

        private bool Filter(object obj)
        {
            if (obj is RomInfoViewModel ri)
            {
                return (string.IsNullOrEmpty(GameTitle) || ri.Title.Contains(GameTitle, StringComparison.CurrentCultureIgnoreCase))
                    && (string.IsNullOrEmpty(Country) || ri.Country.Contains(Country, StringComparison.CurrentCultureIgnoreCase));
            }
            return false;
        }

    }
}
