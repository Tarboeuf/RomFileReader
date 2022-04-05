namespace RomFileReader.UI
{
    public class SettingsDto : ISettings
    {
        public string? LocalGameFilePath { get; set; }
        public string? SdCardGameFilePath { get; set; }
    }
}
