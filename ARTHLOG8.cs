using System.Collections.Generic;
namespace GB
{
    public static class ARTHLOG8
    {
        private static void _DEC(Cpu cpu, ref byte reg)
        {
            byte oldVal = reg;
                    
            reg--;
            
            if (reg == 0)
                cpu.ZeroFlag.Value = true;
            
            if ((oldVal & (1<<3)) != (reg & (1<<3)))
                cpu.HalfCarryFlag.Value = true;
        }
        public static void DEC(Cpu cpu, List<string> args)
        {
            cpu.SubtractFlag.Value = true;
            switch (args[0])
            {
                case "A":
                {
                    _DEC(cpu, ref cpu.Registers.A);
                    break;
                }
                case "B":
                {
                    _DEC(cpu, ref cpu.Registers.B);
                    break;
                }
                case "C":
                {
                    _DEC(cpu, ref cpu.Registers.C);
                    break;
                }
                case "D":
                {
                    _DEC(cpu, ref cpu.Registers.D);
                    break;
                }
                case "E":
                {
                    _DEC(cpu, ref cpu.Registers.E);
                    break;
                }
                case "H":
                {
                    _DEC(cpu, ref cpu.Registers.H);
                    break;
                }
                case "L":
                {
                    _DEC(cpu, ref cpu.Registers.L);
                    break;
                }
            }
        }
        public static void JR(Cpu cpu, List<string> args)
        {
            ushort operandAddr = (ushort)(cpu.Registers.PC + 1);
            if(args[0] == "r8")
            {
                cpu.Registers.PC += cpu.Memory[operandAddr];
            }
            else if (args.Count == 2)
            {
                bool doJmp = false;
                switch(args[0])
                {
                    case "Z":
                    {
                        doJmp = cpu.ZeroFlag.Value;
                        break;
                    }
                    case "C":
                    {
                        doJmp = cpu.CarryFlag.Value;
                        break;
                    }
                    case "NZ":
                    {
                        doJmp = !cpu.ZeroFlag.Value;
                        break;
                    }
                    case "NC":
                    {
                        doJmp = !cpu.CarryFlag.Value;
                        break;
                    }
                }
                if (doJmp)
                    cpu.Registers.PC += (ushort)unchecked((sbyte)(cpu.Memory[operandAddr]));
                else
                    cpu.Registers.PC += 2;
            }
        }
        public static void XOR(Cpu cpu, List<string> args)
        {
            cpu.SubtractFlag.Value = false;
            cpu.HalfCarryFlag.Value = false;
            cpu.CarryFlag.Value = false;

            switch (args[0])
            {
                case "A":
                {
                    cpu.Registers.A ^= cpu.Registers.A;
                    break;
                }
                case "B":
                {
                    cpu.Registers.A ^= cpu.Registers.B;
                    break;
                }
                case "C":
                {
                    cpu.Registers.A ^= cpu.Registers.C;
                    break;
                }
                case "D":
                {
                    cpu.Registers.A ^= cpu.Registers.D;
                    break;
                }
                case "E":
                {
                    cpu.Registers.A ^= cpu.Registers.E;
                    break;
                }
                case "H":
                {
                    cpu.Registers.A ^= cpu.Registers.H;
                    break;
                }
                case "L":
                {
                    cpu.Registers.A ^= cpu.Registers.L;
                    break;
                }
                case "(HL)":
                {
                    cpu.Registers.A ^= cpu.Memory[cpu.Registers.HL];
                    break;
                }
                case "d8":
                {
                    ushort operandAddr = (ushort)(cpu.Registers.PC + 1);
                    cpu.Registers.A ^= cpu.Memory[operandAddr];
                    break;
                }
            }
            if (cpu.Registers.A == 0)
                cpu.ZeroFlag.Value = true;
            else
                cpu.ZeroFlag.Value = false;
        }
    }
}