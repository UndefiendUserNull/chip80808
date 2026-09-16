namespace Chip80808
{
    internal class Program
    {
        byte[] memory = new byte[4096];

        static void Main(string[] args)
        {
            var romPath = args[1];
            Console.Write(File.ReadAllBytes(romPath));
        }
    }
}
