namespace Chip80808;

internal static class Program
{
    private static readonly Chip8 chip = new();
    private static readonly Renderer display = new();

    public static void Main(string[] args)
    {
        var romPath = args[1];

        display.StartDisplay(chip, romPath);
    }
}
