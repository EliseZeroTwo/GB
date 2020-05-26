using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;

namespace GB
{
    [Flags]
    public enum Flags : byte
    {
        None = 0,
        Carry = (1 << 4),
        HalfCarry = (1 << 5),
        Subtract = (1 << 6),
        Zero = (1 << 7),
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct RegisterStruct
    {
        [FieldOffset(0)] public byte F;
        [FieldOffset(1)] public byte A;
        [FieldOffset(2)] public byte C;
        [FieldOffset(3)] public byte B;
        [FieldOffset(4)] public byte E;
        [FieldOffset(5)] public byte D;
        [FieldOffset(6)] public byte L;
        [FieldOffset(7)] public byte H;

        [FieldOffset(0)] public ushort AF;
        [FieldOffset(2)] public ushort BC;
        [FieldOffset(4)] public ushort DE;
        [FieldOffset(6)] public ushort HL;

        [FieldOffset(8)] public ushort SP;
        [FieldOffset(10)] public ushort PC;
    }
    
    public class Cpu
    {
        public bool SingleStep = false;
        public const uint ClockSpeed = 4194304;
        public const uint ClocksPerFrame = 70224;
        public Memory Memory = new Memory();
        public RegisterStruct Registers = new RegisterStruct();

        public List<ushort> Breakpoints = new List<ushort>();

        public bool CarryFlag
        {
            get
            {
                return (Registers.F & (byte)Flags.Carry) != 0;
            }
            set
            {
                if (value)
                    Registers.F |= (byte)Flags.Carry;
                else
                    Registers.F &= (byte)Flags.Carry ^ 0xFF;
            }
        }
        public bool HalfCarryFlag
        {
            get
            {
                return (Registers.F & (byte)Flags.HalfCarry) != 0;
            }
            set
            {
                if (value)
                    Registers.F |= (byte)Flags.HalfCarry;
                else
                    Registers.F &= (byte)Flags.HalfCarry ^ 0xFF;
            }
        }
        public bool SubtractFlag
        {
            get
            {
                return (Registers.F & (byte)Flags.Subtract) != 0;
            }
            set
            {
                if (value)
                    Registers.F |= (byte)Flags.Subtract;
                else
                    Registers.F &= (byte)Flags.Subtract ^ 0xFF;
            }
        }
        public bool ZeroFlag
        {
            get
            {
                return (Registers.F & (byte)Flags.Zero) != 0;
            }
            set
            {
                if (value)
                    Registers.F |= (byte)Flags.Zero;
                else
                    Registers.F &= (byte)Flags.Zero ^ 0xFF;
            }
        }

        public bool IME = false;
        public bool IEVBlank
        {
            get
            {
                return (Memory[0xFFFF] & (1 << 0)) != 0;
            }
            set
            {
                Memory[0xFFFF] &= 0b11110;
                Memory[0xFFFF] |= (byte)(value == true ? (1 << 0) : 0);
            }
        }

        public bool IELCDStat
        {
            get
            {
                return (Memory[0xFFFF] & (1 << 1)) != 0;
            }
            set
            {
                Memory[0xFFFF] &= 0b11101;
                Memory[0xFFFF] |= (byte)(value == true ? (1 << 1) : 0);
            }
        }

        public bool IETimer
        {
            get
            {
                return (Memory[0xFFFF] & (1 << 2)) != 0;
            }
            set
            {
                Memory[0xFFFF] &= 0b11011;
                Memory[0xFFFF] |= (byte)(value == true ? (1 << 2) : 0);
            }
        }

        public bool IESerial
        {
            get
            {
                return (Memory[0xFFFF] & (1 << 3)) != 0;
            }
            set
            {
                Memory[0xFFFF] &= 0b10111;
                Memory[0xFFFF] |= (byte)(value == true ? (1 << 3) : 0);
            }
        }

        public bool IEJoypad
        {
            get
            {
                return (Memory[0xFFFF] & (1 << 4)) != 0;
            }
            set
            {
                Memory[0xFFFF] &= 0b01111;
                Memory[0xFFFF] |= (byte)(value == true ? (1 << 4) : 0);
            }
        }

        public bool IFVBlank
        {
            get
            {
                return (Memory[0xFF0F] & (1 << 0)) != 0;
            }
            set
            {
                Memory[0xFF0F] &= 0b11110;
                Memory[0xFF0F] |= (byte)(value == true ? (1 << 0) : 0);
            }
        }

        public bool IFLCDStat
        {
            get
            {
                return (Memory[0xFF0F] & (1 << 1)) != 0;
            }
            set
            {
                Memory[0xFF0F] &= 0b11101;
                Memory[0xFF0F] |= (byte)(value == true ? (1 << 1) : 0);
            }
        }

        public bool IFTimer
        {
            get
            {
                return (Memory[0xFF0F] & (1 << 2)) != 0;
            }
            set
            {
                Memory[0xFF0F] &= 0b11011;
                Memory[0xFF0F] |= (byte)(value == true ? (1 << 2) : 0);
            }
        }

        public bool IFSerial
        {
            get
            {
                return (Memory[0xFF0F] & (1 << 3)) != 0;
            }
            set
            {
                Memory[0xFF0F] &= 0b10111;
                Memory[0xFF0F] |= (byte)(value == true ? (1 << 3) : 0);
            }
        }

        public bool IFJoypad
        {
            get
            {
                return (Memory[0xFF0F] & (1 << 4)) != 0;
            }
            set
            {
                Memory[0xFF0F] &= 0b01111;
                Memory[0xFF0F] |= (byte)(value == true ? (1 << 4) : 0);
            }
        }

        public bool ShouldSetZeroFlag(byte value)
        {
            return value == 0;
        }
        public bool ShouldSetHalfCarry(byte valueOne, byte valueTwo)
        {
            return (((valueOne&0xf) + (valueTwo&0xf))&0x10) != 0;
        }
        
        public bool ShouldSetCarryFlag(byte valueOne, byte valueTwo)
        {
            return (((valueOne&0xff) + (valueTwo&0xff))&0x100) != 0;
        }

        public int ExecuteInstruction()
        {
            if (SingleStep || Breakpoints.Contains(Registers.PC))
            {
                if (Breakpoints.Contains(Registers.PC))
                    Console.WriteLine($"Hit breakpoint at 0x{Registers.PC:x}");
                SingleStep = true;
                bool waiting = true;
                while(waiting)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey();
                    int currentLineCursor = Console.CursorTop;
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(new string(' ', 3)); 
                    Console.SetCursorPosition(0, currentLineCursor);
                    switch(keyInfo.KeyChar)
                    {
                        case 'a':
                        {
                            Console.Write("Addr> 0x");
                            string strAddr = Console.ReadLine();
                            try
                            {
                                ushort readAddr = Convert.ToUInt16(strAddr, 16);
                                {
                                    byte bValue = Memory[readAddr];
                                    Memory.Read(out ushort val, readAddr);
                                    Console.WriteLine($"byte: 0x{bValue:X}\nushort: 0x{val:x}");
                                }
                            } catch { }
                            break;
                        }
                        case 'b':
                        {
                            Console.Write("Toggle breakpoint at> 0x");
                            string strAddr = Console.ReadLine();
                            try
                            {
                                ushort readAddr = Convert.ToUInt16(strAddr, 16);
                                {
                                    if (Breakpoints.Contains(readAddr))
                                    {
                                        Breakpoints.Remove(readAddr);
                                        Console.WriteLine($"Removed breakpoint at 0x{readAddr:x}");
                                    }
                                    else
                                    {
                                        Breakpoints.Add(readAddr);
                                        Console.WriteLine($"Added breakpoint at 0x{readAddr:x}");
                                    }
                                }
                            } catch { }
                            break;
                        }
                        case 'c':
                        {
                            SingleStep = false;
                            waiting = false;
                            break;
                        }
                        case 'd':
                        {
                            Console.Write("Path for dump> ");
                            string path = Console.ReadLine();
                            using (System.IO.FileStream fs = System.IO.File.Create(path))
                            {
                                Memory.DumpMemory(fs);
                            }
                            break;
                        }
                        case 'n':
                        {
                            waiting = false;
                            break;
                        }
                        case 'i':
                        {
                            Console.WriteLine($"Regs: A:{Registers.A:X} B:{Registers.B:X} C:{Registers.C:X} D:{Registers.D:X} E:{Registers.E:X} F:{Registers.F:X} H:{Registers.H:X} L:{Registers.L:X}\nRegs: AF:{Registers.AF:X} BC:{Registers.BC:X} DE:{Registers.DE:X} HL:{Registers.HL:X}\nRegs: PC:{Registers.PC:X} SP:{Registers.SP:X}");
                            break;
                        }
                        case 'q':
                        {
                            System.Environment.Exit(1);
                            break;
                        }
                    }
                    SDL2.SDL.SDL_Delay(100);
                }
            }
            if (((IFVBlank && IEVBlank) || (IFLCDStat && IELCDStat) || (IFTimer && IETimer) || (IFSerial && IESerial) || (IFJoypad && IEJoypad)) && IME)
            {
                IME = false;
                Memory.Write(Registers.PC, Registers.SP);
                Registers.SP -= 2;
                
                ushort target = 0;

                if (IFJoypad && IEJoypad)
                    target = 0x60;
                
                if (IFSerial && IESerial)
                    target = 0x58;
                
                if (IFTimer && IETimer)
                    target = 0x50;
                
                if (IFLCDStat && IELCDStat)
                    target = 0x48;
                
                if (IFVBlank && IEVBlank)
                    target = 0x40;

                Console.WriteLine($"Executing Interrupt! Target: 0x{target:x}");
                Memory.Read(out Registers.PC, target);
                return 5*4;
            }
            else
            {
                byte opcodeRaw = Memory[Registers.PC];
                ushort operandAddr = (ushort)(Registers.PC + 1);
                Memory.Read(out ushort maybeUshortArgLog, operandAddr);

                if (Opcodes.List.TryGetValue(opcodeRaw, out Opcode opcode))
                {
                    Console.WriteLine($"0x{Registers.PC:x}: {opcode.Mneumonic}".Replace("a8", $"a8<0x{Memory[operandAddr]:x}>").Replace("a16", $"a16<0x{maybeUshortArgLog:x}>").Replace("d8", $"d8<0x{Memory[operandAddr]:x}>").Replace("d16", $"d16<0x{maybeUshortArgLog:x}>"));
                    opcode.Execute(this);
                    Registers.PC += opcode.EffectiveLength;
                    return opcode.Cycles;
                }
                else
                {
                    Program.DumpStuffException();
                    throw new NotImplementedException($"OPCode 0x{opcodeRaw:X}");
                }
            }
        }

        public Cpu()
        {
           // CpuThread = new Thread(new ThreadStart(MainLoop));

            Registers.AF = 0x1B0;
            Registers.BC = 0x13;
            Registers.DE = 0xD8;
            Registers.HL = 0x14D;

            Registers.SP = 0xFFFE;
            Registers.PC = 0x100;
            
            CarryFlag = true;
            HalfCarryFlag = true;
            SubtractFlag = false;
            ZeroFlag = true;

            Memory[0xFFFF] = 0;
            IEJoypad = false;
            IELCDStat = false;
            IESerial = false;
            IETimer = false;
            IEVBlank = false;
        }       
    
        public T GetRegisterByName<T>(string registerName)
        {
            return (T)Registers.GetType().GetField(registerName).GetValue(Registers);
        }

        public void SetRegisterByName(string registerName, object value)
        {
            object boxed = (object)Registers;
            Registers.GetType().GetField(registerName).SetValue(boxed, value);
            Registers = (RegisterStruct)boxed;
        }

        public void StackPop(out ushort outVal)
        {
            Registers.SP += 2;
            Memory.Read(out outVal, Registers.SP);
        }

        public void StackPop(out byte outVal)
        {
            Registers.SP += 1;
            outVal = Memory[Registers.SP];
        }

        public void StackPush(ushort val)
        {
            Memory.Write(val, Registers.SP);
            Registers.SP -= 2;
        }
        
        public void StackPush(byte val)
        {
            Memory[Registers.SP] = val;
            Registers.SP -= 1;
        }
        
    }
}