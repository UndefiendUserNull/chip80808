using Raylib_cs;

namespace Chip80808;

public class Renderer
{
    private const int SCALE = 10;
    private const int WINDOW_WIDTH = 64 * SCALE;
    private const int WINDOW_HEIGHT = 32 * SCALE;
    public void StartDisplay(Chip8 chip8, string romPath, int cycles = 15)
    {
        Raylib.InitWindow(WINDOW_WIDTH, WINDOW_HEIGHT, "CHIP 80808");
        Raylib.SetTargetFPS(60);
        chip8.LoadRom(romPath);

        while (!Raylib.WindowShouldClose())
        {
            chip8.Start(cycles);
            chip8.TickTimers();

            Raylib.BeginDrawing();

            Raylib.ClearBackground(Color.Black);

            for (int x = 0; x < 64; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    if (chip8.IsPixelOn(x, y))
                    {
                        Raylib.DrawRectangle(x * SCALE, y * SCALE, SCALE, SCALE, Color.White);
                    }
                }
            }

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}

/*

CHIP-8:    Keyboard:
1 2 3 C    1 2 3 4
4 5 6 D    Q W E R
7 8 9 E    A S D F
A 0 B F    Z X C V

 */
