using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace GB
{
    [Flags]
    public enum Flags : byte
    {
        None = 0,
        Carry = 0b10000,
        HalfCarry = 0b100000,
        Subtract = 0b1000000,
        Zero = 0b10000000
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Registers
    {
        [FieldOffset(0)] public byte A;
        [FieldOffset(1)] public Flags F;
        [FieldOffset(2)] public byte B;
        [FieldOffset(3)] public byte C;
        [FieldOffset(4)] public byte D;
        [FieldOffset(5)] public byte E;
        [FieldOffset(6)] public byte H;
        [FieldOffset(7)] public byte L;

        [FieldOffset(0)] public ushort AF;
        [FieldOffset(2)] public ushort BC;
        [FieldOffset(4)] public ushort DE;
        [FieldOffset(6)] public ushort HL;

        [FieldOffset(8)] public ushort SP;
        [FieldOffset(10)] public ushort PC;
    }

    public class Flag
    {
        Flags flag;
        Registers regs;
        public bool Value
        {
            get
            {
                return (regs.F & flag) != 0;
            }
            set
            {
                if (value == true)
                    regs.F |= flag;
                else
                    regs.F &= (flag ^ (Flags)0xFF);
            }
        }
        public Flag(Flags flag, ref Registers regs)
        {
            this.flag = flag;
            this.regs = regs;
        }
    }

    public class Cpu
    {
        public Memory Memory = new Memory();
        public Registers Registers = new Registers();
        public Thread CpuThread;

        public Flag CarryFlag;
        public Flag HalfCarryFlag;
        public Flag SubtractFlag;
        public Flag ZeroFlag;

        private bool _running = false;

        public bool Running
        {
            get
            {
                return _running;
            }
            set
            {
                if (value == true)
                {
                    if (!_running)
                    {
                        CpuThread.Start();
                        _running = true;
                    }
                }
                else
                {
                    if (_running)
                    {
                        CpuThread.Abort();
                        _running = false;
                    }
                }
            }
        }

        public void MainLoop()
        {
            while(true)
            {
                byte opcode = Memory[Registers.PC];
                byte opcodeSize = 0;
                ushort operandAddr = (ushort)(Registers.PC + 1);
                switch (opcode)
                {
                    case 0x0: // NOP
                    {
                        opcodeSize = 1;
                        break;
                    }
                    case 0x5: // DEC B
                    {
                        opcodeSize = 1;
                        Registers.B--;
                        break;
                    }
                    case 0x6: // LD B, d8
                    {
                        opcodeSize = 2;
                        Memory.Read(out Registers.B, operandAddr);
                        break;
                    }
                    case 0xD: // DEC C
                    {
                        opcodeSize = 1;
                        Registers.D--;
                        break;
                    }
                    case 0xE: // LD C, d8
                    {
                        opcodeSize = 2;
                        Memory.Read(out Registers.C, operandAddr);
                        break;
                    }
                    case 0x18: // JR r8
                    {
                        opcodeSize = 0;
                        Registers.PC += Memory[operandAddr];
                        break;
                    }
                    case 0x20: // JR NZ,r8
                    {
                        opcodeSize = 2;
                        if ((Registers.F & Flags.Zero) != 0)
                        {
                            Registers.PC += Memory[operandAddr];
                            opcodeSize = 0;
                        }
                        break;
                    }
                    case 0x21: // LD HL,d16
                    {
                        opcodeSize = 3;
                        Memory.Read(out Registers.HL, operandAddr);
                        break;
                    }
                    case 0x28: // JR Z,r8
                    {
                        opcodeSize = 2;
                        if (ZeroFlag.Value)
                            Registers.PC += Memory[operandAddr];
                        break;
                    }
                    case 0x32: // LD (HL-),A
                    {
                        opcodeSize = 1;
                        Memory.Write(Registers.A, Registers.HL--);
                        break;
                    }
                    case 0x3E: // LD A, d8 
                    {
                        opcodeSize = 2;
                        Registers.A = Memory[operandAddr];
                        break;
                    }
                    case 0xAF: // XOR A
                    {
                        opcodeSize = 1;
                        Registers.A = 0;
                        ZeroFlag.Value = true;
                        break;
                    }
                    case 0xC3: // JP a16
                    {
                        opcodeSize = 0; // Effectivley
                        Memory.Read(out Registers.PC, operandAddr);
                        break;
                    }
                    case 0xE0: // LDH (a8),A
                    {
                        opcodeSize = 2;
                        byte operand = Memory[operandAddr];
                        Memory[(ushort)(0xFF00 + operand)] = Registers.A; 
                        break;
                    }
                    case 0xEA: // LD (a16),A
                    {
                        opcodeSize = 3;
                        Memory.Read(out ushort address, operandAddr);
                        Memory[address] = Registers.A;
                        break;
                    }
                    case 0xF3: // DI
                    {
                        opcodeSize = 1;
                        // TODO: Disable Interrupts
                        break;
                    }
                    case 0xFE: // CP d8
                    {
                        opcodeSize = 2;
                        byte value = Memory[operandAddr];
                        if (value == Registers.A)
                            ZeroFlag.Value = true;
                        else
                        {
                            ZeroFlag.Value = false;
                            if (Registers.A < value)
                                CarryFlag.Value = true;
                            else
                                CarryFlag.Value = false;
                        }
                        break;
                    }
                    default:
                        Program.DumpStuffException();
                        throw new NotImplementedException($"OPCode 0x{opcode:X}");
                }
                Registers.PC += opcodeSize;
            }
        }

        public Cpu()
        {
            CpuThread = new Thread(new ThreadStart(MainLoop));

            Registers.AF = 0x1B0;
            Registers.BC = 0x13;
            Registers.DE = 0xD8;
            Registers.HL = 0x14D;

            Registers.SP = 0xFFFE;
            Registers.PC = 0x100;
            
            CarryFlag = new Flag(Flags.Carry, ref Registers);
            HalfCarryFlag = new Flag(Flags.HalfCarry, ref Registers);
            SubtractFlag = new Flag(Flags.Subtract, ref Registers);
            ZeroFlag = new Flag(Flags.Zero, ref Registers);
        }       
    }
}