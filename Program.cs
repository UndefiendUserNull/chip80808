namespace Chip80808
{
    internal static class Program
    {
        static byte[] memory = new byte[4096];

        static void Main(string[] args)
        {
            var romPath = args[0];
            var rom = File.ReadAllBytes(romPath);
            Array.Copy(rom, 0, memory, 0x200, rom.Length);


            for (int i = 0; i < memory.Length; i++)
            {
                Console.WriteLine($"{i:X4}: {memory[i]:X2}");
            }
        }
    }
}
