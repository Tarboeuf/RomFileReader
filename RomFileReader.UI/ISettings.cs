namespace RomFileReader.UI
{
    public interface ISettings
    {
        string? LocalGameFilePath { get; set; }
        string? SdCardGameFilePath { get; set; }
    }
}
