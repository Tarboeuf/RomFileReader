using System.Text;

namespace RomFileReader.Libraries
{
    public class RomDataExtractor : IRomDataExtractor
    {
        const int StartName = 32704;
        const int StartLongName = 65472;

        public async Task<RomInfo?> GetName(FileInfo file)
        {
            using var stream = file.OpenRead();
            Memory<byte> buffer = new Memory<byte>(new byte[StartLongName + 32]);

            var size = await stream.ReadAsync(buffer);

           
            //int indexOfNul = Array.IndxOf(array, Nul);
            return ParseRom(buffer, StartLongName, file.Name) ?? ParseRom(buffer, StartName, file.Name);
        }

        private RomInfo? ParseRom(Memory<byte> buffer, int offset, string fileName)
        {
            RomInfo romInfo = new RomInfo
            {
                FileName = fileName,
            };

            var array = buffer.Slice(offset, 32).Span.ToArray();
            // Game Title
            romInfo.GameTitle = Encoding.UTF8.GetString(array.Take(20).ToArray()).Trim();

            // Rom Speed/Bank Size
            int index = 21;
            int temp = array[index++];
            romInfo.BankSize = (byte)(temp & 0xF);
            romInfo.RomSpeed = (byte)((temp & 0xF0) >> 4);

            // Various other fields
            romInfo.RomType = array[index++];
            romInfo.RomSize = array[index++];
            romInfo.SramSize = array[index++];
            romInfo.Country = array[index++];
            romInfo.License = array[index++];
            romInfo.GameVersion = array[index++];
            romInfo.InverseChecksum = (array[index++] & 0xFF | (array[index++] << 8 & 0xFF00)) & 0xFFFF;
            romInfo.Checksum = (array[index++] & 0xFF | (array[index++] << 8 & 0xFF00)) & 0xFFFF;


            if ((~romInfo.Checksum & 0xFFFF) != (romInfo.InverseChecksum & 0xFFFF))
            {
                Console.WriteLine("Error validating checksum/inverse checksum.");
                Console.WriteLine("This means it could be a LoRom");
                return null;
            }
            if(romInfo.RomSize == 0)
            {
                return null;
            }
            //if (romChecksum != romInfo.Checksum)
            //{
            //    Console.WriteLine("Calculated checksum does not match stored checksum.");
            //}
            RomInfo.printRomInfo(romInfo);

            return romInfo;
        }
    }
}
