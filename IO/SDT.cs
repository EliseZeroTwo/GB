using System;
using System.IO;

namespace GB.IO
{
    public class SDT
    {
        private Memory internalMemory;
        public bool TransferInProgress = false;
        public byte TransferRemainingBits = 0;


        public byte SB
        {
            get
            {
                return internalMemory[0xFF01];
            }
            set
            {
                internalMemory[0xFF01] = value;
            }
        }

        public byte SC
        {
            get
            {
                return internalMemory[0xFF02];
            }
            set
            {
                internalMemory[0xFF02] = value;
            }
        }

        public void Tick()
        {
            if (TransferInProgress)
            {
                SB = (byte)(SB << 1);
                if (--TransferRemainingBits == 0)
                    SC ^= (1 << 7);
            }
            else if ((SC & (1 << 7)) == 0x80)
            {
                TransferInProgress = true;
                TransferRemainingBits = 8;
                using (FileStream fs = new FileStream("./Serial.log", FileMode.Append))
                {
                    fs.WriteByte(SB);
                }
            }
        }
        public SDT(Memory memory)
        {
            internalMemory = memory;
            File.Create("./Serial.log");
        }
    }
}