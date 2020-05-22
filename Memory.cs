using System;
using System.IO;

namespace GB
{
    public class Memory
    {

        public MemoryStream GBMem = new MemoryStream(0xFFFF);

        public void Write(Stream stream, ushort length, ushort addr)
        {
            GBMem.Seek(addr, SeekOrigin.Begin);
            byte[] tempStreamBuffer = new byte[length];
            stream.Read(tempStreamBuffer, 0, length);
            GBMem.Write(tempStreamBuffer, 0, length);
        } 
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

        public byte this[ushort addr]
        {
            get
            {
                if ((addr <= 0x7FFF) || (addr >= 0xA000 && addr <= 0xBFFF) || ((addr >= 0xC000 && addr <= 0xCFFF) || (addr >= 0xE000 && addr <= 0xEFFF)) || ((addr >= 0xD000 && addr <= 0xDFFF) || (addr >= 0xF000 && addr <= 0xFDFF)) || (addr >= 0xFF80 && addr <= 0xFFFE) || (addr == 0xFFFF))
                {
                    GBMem.Seek(addr, SeekOrigin.Begin);
                    return (byte)GBMem.ReadByte();
                }
                else if (addr >= 0xFF00 && addr <= 0xFF7F) // IO Regs
                {
                    switch (addr)
                    {
                        case 0xFF0F:
                        case 0xFF42:
                        case 0xFF43:
                        {
                            GBMem.Seek(addr, SeekOrigin.Begin);
                            return (byte)GBMem.ReadByte();
                        }
                        default:
                        {
                            Program.DumpStuffException();
                            throw new NotImplementedException($"Memory Access Violation: I/O 0x{addr:x} Read not implemented yet!");
                        }
                    }
                }
                else
                {
                    Program.DumpStuffException();
                    throw new NotImplementedException($"Memory Access Violation: Address 0x{addr:x} not implemented yet!");
                }
            }
            set
            {
                if ((addr >= 0xA000 && addr <= 0xBFFF) || ((addr >= 0xC000 && addr <= 0xCFFF) || (addr >= 0xE000 && addr <= 0xEFFF)) || ((addr >= 0xD000 && addr <= 0xDFFF) || (addr >= 0xF000 && addr <= 0xFDFF)) || (addr >= 0xFF80 && addr <= 0xFFFE) || (addr == 0xFFFF))
                {
                    GBMem.Seek(addr, SeekOrigin.Begin);
                    GBMem.WriteByte(value);
                }
                else if (addr >= 0xFF00 && addr <= 0xFF7F) // IO Regs
                {
                    switch (addr)
                    {
                        case 0xFF41:
                        {
                            if ((value & 0x3) != (this[addr] & 0x3))
                            {
                                GBMem.Seek(addr, SeekOrigin.Begin);
                                GBMem.WriteByte(value);
                            }
                            break;
                        }
                        case 0xFF0F:
                        case 0xFF42:
                        case 0xFF43:
                        {
                            GBMem.Seek(addr, SeekOrigin.Begin);
                            GBMem.WriteByte(value);
                            break;
                        }
                        default:
                        {
                            Program.DumpStuffException();
                            throw new NotImplementedException($"Memory Access Violation: I/O 0x{addr:x} attempted to set to {Convert.ToString(value, 2)} not implemented yet!");
                        }
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