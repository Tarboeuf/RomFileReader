// See https://aka.ms/new-console-template for more information
using RomFileReader.Libraries;

string sfcPath = @"C:\Users\Tarboeuf\Documents\Roms\game\sfc";
IRomDataExtractor dataExtractor = new RomDataExtractor();

DirectoryInfo directory = new DirectoryInfo(sfcPath);
int index = 1;
foreach (FileInfo file in directory.EnumerateFiles("*.sfc"))
{
    //if(!file.Name.ToLower().Contains("mario"))
    //{
    //    continue;
    //}
    Console.WriteLine($"{index++} {await dataExtractor.GetName(file)}");
}