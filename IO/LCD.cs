using System;

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
                return (Shade)(internalMemory[0xFF47] >> 0);
            }
            set
            {
                internalMemory[0xFF47] = (byte)((byte)value << 0);
            }
        }

        public Shade ColorShade1
        {
            get
            {
                return (Shade)(internalMemory[0xFF47] >> 2);
            }
            set
            {
                internalMemory[0xFF47] = (byte)((byte)value << 2);
            }
        }

        public Shade ColorShade2
        {
            get
            {
                return (Shade)(internalMemory[0xFF47] >> 4);
            }
            set
            {
                internalMemory[0xFF47] = (byte)((byte)value << 4);
            }
        }

        public Shade ColorShade3
        {
            get
            {
                return (Shade)(internalMemory[0xFF47] >> 6);
            }
            set
            {
                internalMemory[0xFF47] = (byte)((byte)value << 6);
            }
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

        public LCD(Memory memory)
        {
            internalMemory = memory;
        }
    }
}