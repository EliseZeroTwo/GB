using GB.IO;
using SDL2;
using System;
using System.IO;

namespace GB
{
    public class Memory
    {

        public enum MBC : byte
        {
            ROMOnly = 0,
        }

        public MemoryStream GBMem = new MemoryStream(0xFFFF);

        public void Write(ushort s, ushort addr)
        {
            this[addr] = (byte)(s);
            this[(ushort)(addr + 1)] = (byte)(s >> 1);
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

        public void Write(Stream stream, ushort length, ushort addr)
        {
            GBMem.Seek(addr, SeekOrigin.Begin);
            byte[] tempStreamBuffer = new byte[length];
            stream.Read(tempStreamBuffer, 0, length);
            GBMem.Write(tempStreamBuffer, 0, length);
        }

        public byte[] Read(ushort offset, ushort length)
        {
            byte[] retArray = new byte[length];
            GBMem.Seek(offset, SeekOrigin.Begin);
            GBMem.Read(retArray, 0, length);
            return retArray;
        }

        public void Write(byte[] byteArray, ushort memOffset, ushort length=0, ushort arrayOffset=0)
        {
            GBMem.Seek(memOffset, SeekOrigin.Begin);
            GBMem.Write(byteArray, 0, length == 0 ? byteArray.Length : length);
        }

        public void RawWrite(byte b, ushort addr)
        {
            GBMem.Seek(addr, SeekOrigin.Begin);
            GBMem.WriteByte(b);
        }
        
        public byte RawRead(ushort addr)
        {
            GBMem.Seek(addr, SeekOrigin.Begin);
            return (byte)GBMem.ReadByte();
        }


        public byte this[ushort addr]
        {
            get
            {
                if ((addr <= 0x7FFF) || ((addr >= 0xC000 && addr <= 0xCFFF) || (addr >= 0xE000 && addr <= 0xEFFF)) || ((addr >= 0xD000 && addr <= 0xDFFF) || (addr >= 0xF000 && addr <= 0xFDFF)) || (addr >= 0xFEA0 && addr <= 0xFEFF) || (addr >= 0xFF00 && addr <= 0xFFFF))
                {
                    GBMem.Seek(addr, SeekOrigin.Begin);
                    return (byte)GBMem.ReadByte();
                }
                else if (addr >= 0x8000 && addr <= 0x9FFF)
                {
                    if ((this[0xFF41] & 0b11) != 3)
                    {
                        GBMem.Seek(addr, SeekOrigin.Begin);
                        return (byte)GBMem.ReadByte();
                    }
                    else
                        return 0;
                }
                else
                {
                    Program.DumpStuffException();
                    throw new NotImplementedException($"Memory Access Violation: Address 0x{addr:x} not implemented yet!");
                }
            }
            set
            {
                if ((addr <= 0x7FFF) || ((addr >= 0xC000 && addr <= 0xCFFF) || (addr >= 0xE000 && addr <= 0xEFFF)) || ((addr >= 0xD000 && addr <= 0xDFFF) || (addr >= 0xF000 && addr <= 0xFDFF)) || ((addr >= 0xD000 && addr <= 0xDFFF) || (addr >= 0xF000 && addr <= 0xFDFF)) || (addr >= 0xFEA0 && addr <= 0xFEFF) || (addr >= 0xFF00 && addr <= 0xFFFF))
                {
                    GBMem.Seek(addr, SeekOrigin.Begin);
                    GBMem.WriteByte(value);
                }
                else if (addr >= 0x8000 && addr <= 0x9FFF)
                {
                    if ((this[0xFF41] & 0b11) != 3)
                    {
                        GBMem.Seek(addr, SeekOrigin.Begin);
                        GBMem.WriteByte(value);
                    }
                }
                else
                {
                    Program.DumpStuffException();
                    throw new NotImplementedException($"Memory Access Violation: Address 0x{addr:x} not implemented yet!");
                }
            }
        }
    
        public void DumpMemory(Stream outStream)
        {
            GBMem.Seek(0, SeekOrigin.Begin);
            GBMem.CopyTo(outStream);
        }
    }
}