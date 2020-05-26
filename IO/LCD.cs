using System;
using SDL2;

namespace GB.IO
{
    [Flags]
    public enum StatFlags : byte
    {
        None = 0,
        ModeFlag = (1 << 0) | (1 << 1),
        CoincidenceFlag = (1 << 2),
        Mode0Intr = (1 << 3),
        Mode1Intr = (1 << 4),
        Mode2Intr = (1 << 5),
        LYCLYCoincidenceIntr = (1 << 6),
    }

    public enum LCDModeFlag : byte
    {
        HBlank = 0b0,
        VBlank = 0b1,
        SearchingOAM = 0b10,
        Transferring = 0b11,
    }

    public enum Shade : byte
    {
        White = 0,
        LightGray = 1,
        DarkGray = 2,
        Black = 3,
    }

    public class LCD
    {
        private Memory internalMemory;
        private IntPtr targetWindow;
        private IntPtr targetSurface;
        private IntPtr renderer;
        private IntPtr texture;
        public bool LYCLYCoincidenceIntr
        {
            get
            {
                return (internalMemory[0xFF41] & (byte)StatFlags.LYCLYCoincidenceIntr) != 0;
            }
            set
            {
                internalMemory[0xFF41] ^= (byte)StatFlags.LYCLYCoincidenceIntr;
                internalMemory[0xFF41] |= (byte)(value == true ? (byte)StatFlags.LYCLYCoincidenceIntr : 0);
            }
        }
        public bool M2Intr
        {
            get
            {
                return (internalMemory[0xFF41] & (byte)StatFlags.Mode2Intr) != 0;
            }
            set
            {
                internalMemory[0xFF41] ^= (byte)StatFlags.Mode2Intr;
                internalMemory[0xFF41] |= (byte)(value == true ? (byte)StatFlags.Mode2Intr : 0);
            }
        }
        public bool M1Intr
        {
            get
            {
                return (internalMemory[0xFF41] & (byte)StatFlags.Mode1Intr) != 0;
            }
            set
            {
                internalMemory[0xFF41] ^= (byte)StatFlags.Mode1Intr;
                internalMemory[0xFF41] |= (byte)(value == true ? (byte)StatFlags.Mode1Intr : 0);
            }
        }
        public bool M0Intr
        {
            get
            {
                return (internalMemory[0xFF41] & (byte)StatFlags.Mode0Intr) != 0;
            }
            set
            {
                internalMemory[0xFF41] ^= (byte)StatFlags.Mode0Intr;
                internalMemory[0xFF41] |= (byte)(value == true ? (byte)StatFlags.Mode0Intr : 0);
            }
        }

        public bool CoincidenceFlag
        {
            get
            {
                return (internalMemory[0xFF41] & (byte)StatFlags.CoincidenceFlag) != 0;
            }
            set
            {
                internalMemory[0xFF41] ^= (byte)StatFlags.CoincidenceFlag;
                internalMemory[0xFF41] |= (byte)(value == true ? (byte)StatFlags.CoincidenceFlag : 0);
            }
        }
        
        public LCDModeFlag Mode
        {
            get
            {
                return (LCDModeFlag)(internalMemory[0xFF41] & (byte)StatFlags.ModeFlag);
            }
            set
            {
                internalMemory[0xFF41] ^= (byte)StatFlags.ModeFlag;
                internalMemory[0xFF41] |= (byte)value;
            }
        }

        public byte SCY
        {
            get
            {
                return internalMemory[0xFF42];
            }
            set
            {
                internalMemory[0xFF42] = value;
            }
        }

        public byte SCX
        {
            get
            {
                return internalMemory[0xFF43];
            }
            set
            {
                internalMemory[0xFF43] = value;
            }
        }

        public byte LY
        {
            get
            {
                return internalMemory[0xFF44];
            }
            set
            {
                internalMemory[0xFF44] = value;
            }
        }

        public byte LYC
        {
            get
            {
                return internalMemory[0xFF45];
            }
            set
            {
                internalMemory[0xFF45] = value;
            }
        }

        public byte WY
        {
            get
            {
                return internalMemory[0xFF4A];
            }
            set
            {
                internalMemory[0xFF4A] = value;
            }
        }

        public byte WX
        {
            get
            {
                return internalMemory[0xFF4B];
            }
            set
            {
                internalMemory[0xFF4B] = value;
            }
        }

        public Shade ColorShade0
        {
            get
            {
                return (Shade)((internalMemory[0xFF47] >> 0) & 0b11);
            }
            set
            {
                internalMemory[0xFF47] = (byte)(((byte)value & 0b11) << 0);
            }
        }

        public Shade ColorShade1
        {
            get
            {
                return (Shade)((internalMemory[0xFF47] >> 2) & 0b11);
            }
            set
            {
                internalMemory[0xFF47] = (byte)(((byte)value & 0b11) << 2);
            }
        }

        public Shade ColorShade2
        {
            get
            {
                return (Shade)((internalMemory[0xFF47] >> 4) & 0b11);
            }
            set
            {
                internalMemory[0xFF47] = (byte)(((byte)value & 0b11) << 4);
            }
        }

        public Shade ColorShade3
        {
            get
            {
                return (Shade)((internalMemory[0xFF47] >> 6) & 0b11);
            }
            set
            {
                internalMemory[0xFF47] = (byte)(((byte)value & 0b11) << 6);
            }
        }

        private void Reload()
        {
            SDL.SDL_RenderPresent(renderer);
        }
        public void MapShade(Shade shade, out byte R, out byte G, out byte B)
        {
            switch (shade)
            {
                case Shade.Black:
                    R = 0x2a;
                    G = 0x45;
                    B = 0x3b;
                    break;
                case Shade.DarkGray:
                    R = 0x36;
                    G = 0x5d;
                    B = 0x48;
                    break;
                case Shade.LightGray:
                    R = 0x57;
                    G = 0x7C;
                    B = 0x44;
                    break;
                default:
                    R = 0;
                    G = 0;
                    B = 0;
                    break;
            }
        }

        public void SetPixel(byte r, byte g, byte b, byte x, byte y, bool reload=false)
        {
            SDL.SDL_SetRenderTarget(renderer, texture);
            SDL.SDL_SetRenderDrawColor(renderer, r, g, b, 0);
            SDL.SDL_Rect rect = new SDL.SDL_Rect();
            rect.w = 1;
            rect.h = 1;
            rect.x = x;
            rect.y = y;
            SDL.SDL_RenderFillRect(renderer, ref rect);
        }

        public void SetPixel(Shade shade, byte x, byte y, bool reload=false)
        {
            MapShade(shade, out byte r, out byte g, out byte b);
            SetPixel(r, g, b, x, y, reload);
        }


        public void DrawLine()
        {

            LY++;
            if (LY >= 144 && LY <= 153)
            {
                internalMemory[0xFF0F] |= 1; // Set VBlank Interrupt
                Mode = LCDModeFlag.VBlank;
                if (M1Intr)
                {
                    internalMemory[0xFF0F] |= (1 << 1);
                }
            }
            else
            {
                internalMemory[0xFF0F] ^= 1;
                Mode = LCDModeFlag.HBlank;
                if (M0Intr)
                {
                    internalMemory[0xFF0F] |= (1 << 1);
                }
            }
        }

        public void ShowFrame()
        {
            SDL.SDL_SetRenderTarget(renderer, IntPtr.Zero);
            SDL.SDL_RenderCopy(renderer, texture, IntPtr.Zero, IntPtr.Zero);
            SDL.SDL_RenderPresent(renderer);
            MapShade(Shade.Black, out byte r, out byte g, out byte b);
            SDL.SDL_SetRenderDrawColor(renderer, r, g, b, 0x00);
            SDL.SDL_RenderClear(renderer);
        }

        public LCD(Memory memory)
        {
            internalMemory = memory;

            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) != 0)
                throw new Exception("Failed to init SDL");
            
            targetWindow = SDL.SDL_CreateWindow("GB", SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, 160, 144, 0);
            renderer = SDL.SDL_CreateRenderer(targetWindow, -1, 0);
            texture = SDL.SDL_CreateTexture(renderer, SDL.SDL_PIXELFORMAT_RGBA8888, 2, 160, 144);

            SDL.SDL_SetRenderTarget(renderer, texture);
            MapShade(Shade.Black, out byte blackR, out byte blackG, out byte blackB);
            SDL.SDL_SetRenderDrawColor(renderer, blackR, blackG, blackB, 0);
            SDL.SDL_RenderClear(renderer);
            ShowFrame();
        }
    }
}