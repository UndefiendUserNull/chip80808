using Raylib_cs;

namespace Chip80808;

public class Renderer
{
    private const int SCALE = 10;
    private const int WINDOW_WIDTH = 64 * SCALE;
    private const int WINDOW_HEIGHT = 32 * SCALE;

    private Chip8? chip8 = null;
    public void StartDisplay(Chip8 p_chip8, string romPath, int cycles = 15)
    {
        chip8 = p_chip8;

        Raylib.InitWindow(WINDOW_WIDTH, WINDOW_HEIGHT, "CHIP 80808");
        Raylib.SetTargetFPS(60);

        chip8.LoadRom(romPath);

        while (!Raylib.WindowShouldClose())
        {
            UpdateKeys();

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

    private void UpdateKeys()
    {
        // 2 girls 1 cup is cleaner than this.

        if (chip8 == null) return;

        // row 0: Up 2 3 4  ->  CHIP-8 1 2 3 C
        chip8.SetKey(0x1, Raylib.IsKeyDown(KeyboardKey.Up));
        chip8.SetKey(0x2, Raylib.IsKeyDown(KeyboardKey.Two));
        chip8.SetKey(0x3, Raylib.IsKeyDown(KeyboardKey.Three));
        chip8.SetKey(0xC, Raylib.IsKeyDown(KeyboardKey.Four));

        // row 1: P1-Down P2-Down E R  ->  CHIP-8 4 5 6 D
        chip8.SetKey(0x4, Raylib.IsKeyDown(KeyboardKey.Down));
        chip8.SetKey(0x5, Raylib.IsKeyDown(KeyboardKey.W));
        chip8.SetKey(0x6, Raylib.IsKeyDown(KeyboardKey.E));
        chip8.SetKey(0xD, Raylib.IsKeyDown(KeyboardKey.R));

        // row 2: A S D F  ->  CHIP-8 7 8 9 E
        chip8.SetKey(0x7, Raylib.IsKeyDown(KeyboardKey.A));
        chip8.SetKey(0x8, Raylib.IsKeyDown(KeyboardKey.S));
        chip8.SetKey(0x9, Raylib.IsKeyDown(KeyboardKey.D));
        chip8.SetKey(0xE, Raylib.IsKeyDown(KeyboardKey.F));

        // row 3: Z X C V  ->  CHIP-8 A 0 B F
        chip8.SetKey(0xA, Raylib.IsKeyDown(KeyboardKey.Z));
        chip8.SetKey(0x0, Raylib.IsKeyDown(KeyboardKey.X));
        chip8.SetKey(0xB, Raylib.IsKeyDown(KeyboardKey.C));
        chip8.SetKey(0xF, Raylib.IsKeyDown(KeyboardKey.V));
    }
}

/*

CHIP-8:    Keyboard:
1 2 3 C    1 2 3 4
4 5 6 D    Q W E R
7 8 9 E    A S D F
A 0 B F    Z X C V

 */
