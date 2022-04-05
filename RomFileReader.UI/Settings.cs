using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using System;
using System.IO;
using System.Text.Json;

namespace RomFileReader.UI
{

    public class Settings : ReactiveObject, ISettingsValidatable, IValidatableViewModel
    {
        private Lazy<SettingsDto> _settings;
        private const string SettingsFilePath = @"appsettings.json";

        public Settings()
        {
            _settings = new Lazy<SettingsDto>(GetSettings);

            this.ValidationRule(vm => vm.LocalGameFilePath, fp => Directory.Exists(fp), "Directory doesn't exist");
            this.ValidationRule(vm => vm.SdCardGameFilePath, fp => Directory.Exists(fp), "Directory doesn't exist");
        }

        private SettingsDto GetSettings()
        {
            FileInfo file = new FileInfo(SettingsFilePath);
            if(!file.Exists)
            {
                return new SettingsDto();
            }
            try
            {
                using Stream stream = file.OpenRead();
                var settings = JsonSerializer.Deserialize<SettingsDto>(stream);
                return settings ?? new SettingsDto();
            }
            catch (Exception)
            {
                return new SettingsDto();
            }
        }

        public string? LocalGameFilePath
        {
            get => _settings.Value.LocalGameFilePath;
            set
            {
                if (_settings.Value.LocalGameFilePath != value)
                {
                    _settings.Value.LocalGameFilePath = value;
                    SaveSettings();
                    this.RaisePropertyChanged();
                }
            }
        }

        public string? SdCardGameFilePath
        {
            get => _settings.Value.SdCardGameFilePath;
            set
            {
                if (_settings.Value.SdCardGameFilePath != value)
                {
                    _settings.Value.SdCardGameFilePath = value;
                    SaveSettings();
                    this.RaisePropertyChanged();
                }
            }
        }

        public ValidationContext ValidationContext { get; }  = new ValidationContext();

        private void SaveSettings()
        {
            FileInfo file = new FileInfo(SettingsFilePath);
            if(file.Exists) file.Delete();
            using Stream stream = file.Open(FileMode.OpenOrCreate);
            JsonSerializer.Serialize(stream, _settings.Value);
        }

        private Lazy<string?> GetSetting(Func<ISettings, string?> getProperty)
        {
            return new Lazy<string?>(() => getProperty(_settings.Value));
        }
    }
}
