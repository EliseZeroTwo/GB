using System;

namespace GB
{
    public class Memory
    {
        private byte[] ERAM = new byte[0xFFF * 2];
        private byte[] WRAM1 = new byte[0xFFF];
        private byte[] WRAM2 = new byte[0xFFF];
        private byte[] HRAM = new byte[0x7E];

        public void Write(ushort s, ushort addr)
        {
            this[addr] = (byte)(s);
            this[++addr] = (byte)(s >> 1);
        }
        
        public void Read(out ushort s, ushort addr)
        {
            s = (ushort)(this[addr] | (this[(ushort)(addr + 1)] << 8));
        }

        public void Write(byte b, ushort addr)
        {
            this[addr] = b;
        }

        public void Read(out byte b, ushort addr)
        {
            b = this[addr];
        }

        public byte this[ushort addr]
        {
            get
            {
                if (addr >= 0xA000 && addr <= 0xBFFF)
                {
                    return ERAM[addr & 0x1FFF];
                }
                else if ((addr >= 0xC000 && addr <= 0xCFFF) || (addr >= 0xE000 && addr <= 0xEFFF))
                {
                    return WRAM1[addr & 0xFFF];
                }
                else if ((addr >= 0xD000 && addr <= 0xDFFF) || (addr >= 0xF000 && addr <= 0xFDFF))
                {
                    return WRAM2[addr & 0xFFF];
                }
                else if (addr >= 0xFF80 && addr <= 0xFFFE)
                {
                    return HRAM[(addr & 0xFF) - 0x80];
                }
                else
                {
                    throw new NotImplementedException($"Memory Access Violation: Address 0x{addr:x} not implemented yet!");
                }
            }
            set
            {
                if (addr >= 0xA000 && addr <= 0xBFFF)
                {
                    ERAM[addr & 0x1FFF] = value;
                }
                else if ((addr >= 0xC000 && addr <= 0xCFFF) || (addr >= 0xE000 && addr <= 0xEFFF))
                {
                    WRAM1[addr & 0xFFF] = value;
                }
                else if ((addr >= 0xD000 && addr <= 0xDFFF) || (addr >= 0xF000 && addr <= 0xFDFF))
                {
                    WRAM2[addr & 0xFFF] = value;
                }
                else if (addr >= 0xFF80 && addr <= 0xFFFE)
                {
                    HRAM[(addr & 0xFF) - 0x80] = value;
                }
                else
                {
                    throw new NotImplementedException($"Memory Access Violation: Address 0x{addr:x} not implemented yet!");
                }
            }
        }
    }
}