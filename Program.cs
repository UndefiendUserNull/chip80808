namespace Chip80808
{
    internal static class Program
    {
        static Chip8 chip = new();

        public static void Main(string[] args)
        {
            var romPath = args[1];

            chip.Start(romPath);
        }
    }
}
