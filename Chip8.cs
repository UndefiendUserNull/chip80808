namespace Chip80808;

using System.Collections.Generic;
public class Chip8
{
    private readonly byte[] memory = new byte[4096];
    private byte[] V = new byte[16]; // Registers
    private UInt16 I = 0; // Index register
    private UInt16 PC = 0x200; // Program Counter
    private int type = 0; // 0xF000
    private int X = 0; // 0x0F00
    private int Y = 0; // 0x00F0
    private int N = 0; // 0x000F
    private int NN = 0; // 0x00FF
    private int NNN = 0; // 0x0FFF
    private int F = 0; // Overflow
    private readonly Stack<ushort> stack = [];
    private Random rng = new();
    private byte delayTimer = 0;
    private byte soundTimer = 0;
    private readonly bool[] keypad = new bool[16];
    private bool[,] display = new bool[64, 32];

    public void LoadRom(string romPath)
    {
        var rom = File.ReadAllBytes(romPath);
        Array.Copy(rom, 0, memory, 0x200, rom.Length);

    }
    public bool IsPixelOn(int x, int y)
    {
        return display[x, y];
    }
    public void Start(int cycels)
    {
        for (int i = 0; i < cycels; i++)
        {
            Cycle();
        }
    }

    public void TickTimers()
    {
        if (delayTimer > 0) delayTimer--;
        if (soundTimer > 0) soundTimer--;
    }

    private void Cycle()
    {
        ushort opcode = (ushort)((memory[PC] << 8) | memory[PC + 1]);
        //PrintInstruction(PC, opcode);

        type = opcode & 0xF000;
        X = (opcode & 0x0F00) >> 8;
        Y = (opcode & 0x00F0) >> 4;
        N = opcode & 0x000F;
        NN = opcode & 0x00FF;
        NNN = opcode & 0x0FFF;

        IncrementPC();

        switch (type)
        {
            case 0x0000: // SYS CALLS
                switch (opcode)
                {
                    case 0x00E0: // CLEAR
                        Clear();
                        break;
                    case 0x00EE: // RETURN
                        Return();
                        break;
                    default:
                        Console.WriteLine($"{opcode:X4}");
                        break;
                }
                break;
            case 0xF000:
                switch (NN)
                {
                    case 0x15: // SET DELAY TIMER
                        delayTimer = V[X];
                        break;
                    case 0x07: // SET VX = NN
                        V[X] = delayTimer;
                        break;
                }
                break;
            case 0x1000: // JUMP
                PC = (ushort)NNN;
                Console.WriteLine($"PC = {NNN}");
                break;
            case 0x2000: // CALL
                stack.Push(PC);
                PC = (ushort)NNN;
                break;
            case 0x3000: // VX EQUAL NN
                if (V[X] == NN) IncrementPC();
                break;
            case 0x4000: // VX NOT EQUAL NN
                if (V[X] != NN) IncrementPC();
                break;
            case 0x5000: // VX NOT EQUAL VY
                if (V[X] == V[Y]) IncrementPC();
                break;
            case 0x6000: // SET
                V[X] = (byte)NN;
                Console.WriteLine($"V{X:X} = 0x{NN:X2}");
                break;
            case 0x7000: // ADDITION
                V[X] += (byte)NN;
                Console.WriteLine($"V{X:X} += 0x{NN:X2}");
                break;
            case 0x8000:
                switch (N)
                {
                    case 0x0: // SET VX = VY
                        V[X] = V[Y];
                        break;
                    case 0x1: // BITWISE OR
                        V[X] |= V[Y];
                        break;
                    case 0x2: // BITWISE AND
                        V[X] &= V[Y];
                        break;
                    case 0x3: // BITWISE XOR
                        V[X] ^= V[Y];
                        break;
                    case 0x4: // ADD ON CARRY
                        int sum = V[X] + V[Y];
                        V[F] = (byte)(sum > 255 ? 1 : 0);
                        V[X] = (byte)sum;
                        break;

                    case 0x5: // SUBTRACT ON BORROW
                        int diff = V[X] - V[Y];
                        V[F] = (byte)(diff >= 0 ? 1 : 0);
                        V[X] = (byte)diff;
                        break;

                    case 0x6: // SHR
                        V[F] = (byte)(V[Y] & 0x1);
                        V[X] = (byte)(V[Y] >> 1);
                        break;

                    case 0x7: // SUBN
                        int rdiff = V[Y] - V[X];
                        V[F] = (byte)(rdiff >= 0 ? 1 : 0);
                        V[X] = (byte)rdiff;
                        break;

                    case 0xE: // SHL
                        V[F] = (byte)((V[Y] >> 7) & 0x1);
                        V[X] = (byte)(V[Y] << 1);
                        break;
                }
                break;
            case 0x9000: // V[X] EQUAL V[Y]
                if (V[X] != V[Y]) IncrementPC();
                break;
            case 0xA000: // SET INDEX REGISTER
                I = (ushort)NNN;
                break;
            case 0xB000: // JUMP TO NNN + V[0]
                PC = (ushort)(NNN + V[0]);
                break;
            case 0xC000: // RANDOM 0-255 AND NN
                V[X] = (byte)(rng.Next(0, 256) & NN);
                break;
            case 0xD000:
                DrawSprite();
                break;
            default:
                Console.WriteLine($"Unhandled opcode: 0x{opcode:X4} at PC=0x{(PC - 2):X3}");
                break;

        }
    }

    private void DrawSprite()
    {
        V[0xF] = 0;
        for (int r = 0; r < N; r++)
        {
            byte sprite = memory[I + r];
            for (int c = 0; c <= 7; c++)
            {
                var bit = (sprite >> (7 - c)) & 1;
                if (bit == 1)
                {
                    int px = (V[X] + c) % 64;
                    int py = (V[Y] + r) % 32;

                    if (display[px, py]) // Collision happen
                    {
                        V[0xF] = 1;
                    }
                    display[px, py] = !display[px, py]; // Flip
                }
            }
        }
    }

    private void IncrementPC(ushort amount = 2)
    {
        PC += amount;
    }

    private void Return()
    {
        PC = stack.Pop();
    }

    private void Clear()
    {
        display = new bool[64, 32];
    }

    private static void PrintInstruction(ushort pc, ushort opcode)
    {
        Console.WriteLine($"PC=0x{pc:X3}  opcode=0x{opcode:X4}");
    }

    private static void PrintAllInstructions(ushort opcode)
    {
        Console.WriteLine($"Type: 0x{((opcode & 0xF000)):X2}");
        Console.WriteLine($"X: 0x{((opcode & 0x0F00) >> 8):X2}");
        Console.WriteLine($"Y: 0x{((opcode & 0x00F0) >> 4):X2}");
        Console.WriteLine($"N: 0x{((opcode & 0x000F)):X2}");
    }
}


/*
 * 
 0x000
  ^^^^
  ||||
  |||+-- N   (last digit,  bits 0–3)
  ||+--- Y   (3rd digit,   bits 4–7)
  |+---- X   (2nd digit,   bits 8–11)
  +----- type (1st digit,  bits 12–15)

  0xTXYN
 */