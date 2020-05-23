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

        public LCD(Memory memory)
        {
            internalMemory = memory;
        }
    }
}