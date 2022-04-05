namespace RomFileReader.Libraries
{
    public interface IRomDataExtractor
    {
        Task<RomInfo?> GetName(FileInfo file);
    }
}
