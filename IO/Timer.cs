using System;
using System.IO;

namespace GB.IO
{
    public class Timer
    {
        private Memory internalMemory;
        private byte prevDIV = 0;
        public byte DIV
        {
            get
            {   
                return internalMemory[0xFF04];
            }
            set
            {
                internalMemory[0xFF04] = value;
            }
        }

        public byte TIMA
        {
            get
            {
                return internalMemory[0xFF05];
            }
            set
            {
                internalMemory[0xFF05] = value;
            }
        }

        public byte TMA
        {
            get
            {
                return internalMemory[0xFF06];
            }
            set
            {
                internalMemory[0xFF06] = value;
            }
        }

        public byte TAC
        {
            get
            {
                return internalMemory[0xFF07];
            }
            set
            {
                internalMemory[0xFF07] = value;
            }
        }

        private int DIVCounter = 255;
        private int TIMACounterMax = 0;
        private int TIMACounter = 0;
        public void Tick()
        {
            if (DIV != prevDIV)
                DIV = 0;
            
            if (DIVCounter-- == 0)
                DIV++;

            int currentTIMACMax = TIMACounterMax;
            switch (TAC & 0b11)
            {
                case 0:
                    TIMACounterMax = 1023;
                    break;
                case 1:
                    TIMACounterMax = 15;
                    break;
                case 2:
                    TIMACounterMax = 63;
                    break;
                case 3:
                    TIMACounterMax = 255;
                    break;
            }

            if (currentTIMACMax != TIMACounterMax)
            {
                TIMACounter = TIMACounterMax;
            }

            if ((TAC & 0b100) != 0 && TIMACounter-- == 0)
            {
                TIMA++;
                if (TIMA == 0)
                {
                    TIMA = TMA;
                    internalMemory[0xFF0F] |= (1 << 2);
                }
                
            }
        }
        public Timer(Memory memory)
        {
            internalMemory = memory;
            prevDIV = internalMemory[0xFF04];
            TIMACounterMax = TAC & 0b11;
        }
    }
}