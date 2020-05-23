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
    public struct RegisterStruct
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
        RegisterStruct regs;
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
        public Flag(Flags flag, ref RegisterStruct regs)
        {
            this.flag = flag;
            this.regs = regs;
        }
    }

    public class Cpu
    {
        public const uint ClockSpeed = 4194304;
        public const uint ClocksPerFrame = 70224;
        public Memory Memory = new Memory();
        public RegisterStruct Registers = new RegisterStruct();

        public Flag CarryFlag;
        public Flag HalfCarryFlag;
        public Flag SubtractFlag;
        public Flag ZeroFlag;


        public int ExecuteInstruction()
        {     
            byte opcodeRaw = Memory[Registers.PC];
            ushort operandAddr = (ushort)(Registers.PC + 1);

            if (Opcodes.List.TryGetValue(opcodeRaw, out Opcode opcode))
            {
                Memory.Read(out ushort maybeUshortArgLog, operandAddr);
                Console.WriteLine($"0x{Registers.PC:x}: {opcode.Mneumonic}".Replace("a8", $"a8<0x{Memory[operandAddr]:x}>").Replace("a16", $"a16<0x{maybeUshortArgLog:x}>"));
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

        public Cpu()
        {
           // CpuThread = new Thread(new ThreadStart(MainLoop));

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
    }
}