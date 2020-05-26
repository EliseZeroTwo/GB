using GB.IO;
using SDL2;
using System;
using System.Collections.Generic;
using System.IO;

namespace GB
{
    public class Memory : MemoryStream
    {
        public enum MBC : byte
        {
            ROMOnly = 0,
        }

        public byte dmaCyclesLeft = 0;
        public Memory() : base(0x10000) 
        {
            this.Seek(0, SeekOrigin.Begin);
            this.Write(new byte[0xFFFF]);
        }

        public void Write(ushort s, ushort addr)
        {
            this[addr] = (byte)(s);
            this[(ushort)(addr + 1)] = (byte)(s >> 8);
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

        public override void Write(byte[] buffer, int memoffset, int count) 
        {
            this.Seek(memoffset, SeekOrigin.Begin);
            base.Write(buffer, 0, count);
        }

        public override int Read(byte[] buffer, int memoffset, int count) 
        {
            this.Seek(memoffset, SeekOrigin.Begin);
            return base.Read(buffer, 0, count);
        }

        public byte this[ushort addr]
        {
            get
            {
                if (addr <= 0x8000 || (addr >= 0xC000 && addr <= 0xFE9F) || addr >= 0xFF00)
                {
                    if ((addr >= 0xE000 && addr <= 0xEFFF))
                        addr = (ushort)(addr - 0x1000);
                    this.Seek(addr, SeekOrigin.Begin);
                    return (byte)this.ReadByte();
                }
                else if (addr >= 0x8000 && addr <= 0x9FFF)
                {
                    if ((this[0xFF41] & 0b11) != 3) // Check LCD mode
                    {
                        this.Seek(addr, SeekOrigin.Begin);
                        return (byte)this.ReadByte();
                    }
                    else
                        return 0;
                }
                else if (addr >= 0xFEA0 && addr <= 0xFEFF)
                    return 0;
                else
                {
                    Program.DumpStuffException();
                    throw new NotImplementedException($"Memory Access Violation: Address 0x{addr:x} not implemented yet!");
                }
            }
            set
            {
                if (addr <= 0x8000 || (addr >= 0xC000 && addr <= 0xFF45) || (addr >= 0xFF47 && addr <= 0xFFFF))
                {
                    this.Seek(addr, SeekOrigin.Begin);
                    this.WriteByte(value);
                }
                else if (addr >= 0x8000 && addr <= 0x9FFF)
                {
                    if ((this[0xFF41] & 0b11) != 3)
                    {
                        this.Seek(addr, SeekOrigin.Begin);
                        this.WriteByte(value);
                    }
                }
                else if (addr == 0xFF46)
                {
                    byte[] dmaBuffer = new byte[0x9F];
                    ushort srcAddr = (ushort)(value << 8);
                    this.Read(dmaBuffer, srcAddr, 0x9F);
                    this.Write(dmaBuffer, 0xFE00, 0x9F);
                    throw new Exception($"{srcAddr:X}");
                    using (FileStream fs = File.Create("outdumpdma"))
                        DumpMemory(fs);
                    this.Seek(0xFF46, SeekOrigin.Begin);
                    this.WriteByte(value);
                }
                else if (addr >= 0xFEA0 && addr <= 0xFEFF)
                    return;
                else
                {
                    Program.DumpStuffException();
                    throw new NotImplementedException($"Memory Access Violation: Address 0x{addr:x} not implemented yet!");
                }
            }
        }
    
        public void DumpMemory(Stream outStream)
        {
            this.Seek(0, SeekOrigin.Begin);
            byte[] tempArray = new byte[0xffff];
            this.Read(tempArray, 0, 0xFFFF);
            outStream.Write(tempArray, 0, 0xFFFF);
        }
    }
}