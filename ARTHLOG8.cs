using System;
using System.Collections.Generic;
namespace GB
{
    public static class ARTHLOG8
    {
        public static byte INCDECGetOperandFromOpcode(Cpu cpu)
        {
            byte opcode = cpu.Memory[cpu.Registers.PC];
            bool opcodeLeft = ((opcode & 0xF) < 7);
            switch (opcode & 0xF0)
            {
                case 0x00:
                    return opcodeLeft ? cpu.Registers.B : cpu.Registers.C;
                case 0x10:
                    return opcodeLeft ? cpu.Registers.D : cpu.Registers.E;
                case 0x20:
                    return opcodeLeft ? cpu.Registers.H : cpu.Registers.L;
                case 0x30:
                    return opcodeLeft ? cpu.Memory[cpu.Registers.HL] : cpu.Registers.A;
            }
            throw new Exception($"Failed to parse opcode 0x{opcode:X}");
        }

        public static void INCDECSetOperandFromOpcode(Cpu cpu, byte value)
        {
            byte opcode = cpu.Memory[cpu.Registers.PC];
            bool opcodeLeft = ((opcode & 0xF) < 7);
            switch (opcode & 0xF0)
            {
                case 0x00:
                    if (opcodeLeft)
                        cpu.Registers.B = value;
                    else
                        cpu.Registers.C = value;
                    return;
                case 0x10:
                    if (opcodeLeft)
                        cpu.Registers.D = value;
                    else
                        cpu.Registers.E = value;
                    return;
                case 0x20:
                    if (opcodeLeft)
                        cpu.Registers.H = value;
                    else
                        cpu.Registers.L = value;
                    return;
                case 0x30:
                    if (opcodeLeft)
                        cpu.Memory[cpu.Registers.HL] = value;
                    else
                        cpu.Registers.A = value;
                    return;
            }
            throw new Exception($"Failed to parse opcode 0x{opcode:X}");
        }

        public static byte GenericGetOperandFromOpcode(Cpu cpu)
        {
            byte opcode = cpu.Memory[cpu.Registers.PC];
            switch (opcode & 0x0F)
            {
                case 0x0:
                case 0x8:
                    return cpu.Registers.B;
                case 0x1:
                case 0x9:
                    return cpu.Registers.C;
                case 0x2:
                case 0xA:
                    return cpu.Registers.D;
                case 0x3:
                case 0xB:
                    return cpu.Registers.E;
                case 0x4:
                case 0xC:
                    return cpu.Registers.H;
                case 0x5:
                case 0xD:
                    return cpu.Registers.L;
                case 0x6:
                case 0xE:
                    if (opcode < 0xC0)
                        return cpu.Memory[cpu.Registers.HL];
                    else
                        return cpu.Memory[(ushort)(cpu.Registers.PC + 1)];
                case 0x7:
                case 0xF:
                    return cpu.Registers.A;
            }
            throw new Exception($"Failed to parse opcode 0x{opcode:X}");
        }

        public static void GenericSetOperandFromOpcode(Cpu cpu, byte value)
        {
            byte opcode = cpu.Memory[cpu.Registers.PC];
            switch (opcode & 0x0F)
            {
                case 0x0:
                case 0x8:
                    cpu.Registers.B = value;
                    return;
                case 0x1:
                case 0x9:
                    cpu.Registers.C = value;
                    return;
                case 0x2:
                case 0xA:
                    cpu.Registers.D = value;
                    return;
                case 0x3:
                case 0xB:
                    cpu.Registers.E = value;
                    return;
                case 0x4:
                case 0xC:
                    cpu.Registers.H = value;
                    return;
                case 0x5:
                case 0xD:
                    cpu.Registers.L = value;
                    return;
                case 0x6:
                case 0xE:
                    cpu.Memory[cpu.Registers.HL] = value;
                    return;
                case 0x7:
                case 0xF:
                    cpu.Registers.A = value;
                    return;
            }
            throw new Exception($"Failed to parse opcode 0x{opcode:X}");
        }

        public static void DEC(Cpu cpu, List<string> args)
        {
            byte val = INCDECGetOperandFromOpcode(cpu);
            byte res = (byte)(val - 1);

            INCDECSetOperandFromOpcode(cpu, res);
            
            cpu.ZeroFlag = cpu.ShouldSetZeroFlag(res);
            cpu.SubtractFlag = true;
            cpu.HalfCarryFlag = cpu.ShouldSetHalfCarry(val, res);
        }

        public static void INC(Cpu cpu, List<string> args)
        {
            byte val = INCDECGetOperandFromOpcode(cpu);
            byte res = (byte)(val + 1);

            INCDECSetOperandFromOpcode(cpu, res);

            cpu.ZeroFlag = cpu.ShouldSetZeroFlag(res);
            cpu.SubtractFlag = false;
            cpu.HalfCarryFlag = cpu.ShouldSetHalfCarry(val, res);
        }

        public static void CP(Cpu cpu, List<string> args)
        {
            byte val = GenericGetOperandFromOpcode(cpu);
            byte res = (byte)(cpu.Registers.A - val);

            cpu.ZeroFlag = cpu.ShouldSetZeroFlag(res);
            cpu.SubtractFlag = true;
            cpu.HalfCarryFlag = cpu.ShouldSetHalfCarry(val, res);
            cpu.CarryFlag = cpu.ShouldSetCarryFlag(val, res);
        }

        public static void ADC(Cpu cpu, List<string> args)
        {
            byte val = GenericGetOperandFromOpcode(cpu);
            byte res = (byte)(cpu.Registers.A + val);

            if (cpu.CarryFlag)
                res++;
            cpu.Registers.A = res;

            cpu.ZeroFlag = cpu.ShouldSetZeroFlag(res);
            cpu.SubtractFlag = false;
            cpu.HalfCarryFlag = cpu.ShouldSetHalfCarry(val, res);
            cpu.CarryFlag = cpu.ShouldSetCarryFlag(val, res);
        }

        public static void ADD(Cpu cpu, List<string> args)
        {
            byte val = GenericGetOperandFromOpcode(cpu);
            byte res = (byte)(cpu.Registers.A + val);

            cpu.Registers.A = res;

            cpu.ZeroFlag = cpu.ShouldSetZeroFlag(res);
            cpu.SubtractFlag = false;
            cpu.HalfCarryFlag = cpu.ShouldSetHalfCarry(val, res);
            cpu.CarryFlag = cpu.ShouldSetCarryFlag(val, res);
        }


        public static void SBC(Cpu cpu, List<string> args)
        {
            byte val = GenericGetOperandFromOpcode(cpu);
            byte res = (byte)(cpu.Registers.A - val);

            if (cpu.CarryFlag)
                res--;
            cpu.Registers.A = res;

            cpu.ZeroFlag = cpu.ShouldSetZeroFlag(res);
            cpu.SubtractFlag = true;
            cpu.HalfCarryFlag = cpu.ShouldSetHalfCarry(val, res);
            cpu.CarryFlag = cpu.ShouldSetCarryFlag(val, res);
        }

        public static void SUB(Cpu cpu, List<string> args)
        {
            byte val = GenericGetOperandFromOpcode(cpu);
            byte res = (byte)(cpu.Registers.A - val);

            cpu.Registers.A = res;

            cpu.ZeroFlag = cpu.ShouldSetZeroFlag(res);
            cpu.SubtractFlag = true;
            cpu.HalfCarryFlag = cpu.ShouldSetHalfCarry(val, res);
            cpu.CarryFlag = cpu.ShouldSetCarryFlag(val, res);
        }

        public static void AND(Cpu cpu, List<string> args)
        {
            byte val = GenericGetOperandFromOpcode(cpu);
            byte res = (byte)(cpu.Registers.A & val);

            cpu.Registers.A = res;

            cpu.ZeroFlag = cpu.ShouldSetZeroFlag(res);
            cpu.SubtractFlag = false;
            cpu.HalfCarryFlag = true;
            cpu.CarryFlag = false;
        }

        public static void XOR(Cpu cpu, List<string> args)
        {
            byte val = GenericGetOperandFromOpcode(cpu);
            byte res = (byte)(cpu.Registers.A ^ val);

            cpu.Registers.A = res;

            cpu.ZeroFlag = cpu.ShouldSetZeroFlag(res);
            cpu.SubtractFlag = false;
            cpu.CarryFlag = false;
            cpu.HalfCarryFlag = false;
        }

        public static void OR(Cpu cpu, List<string> args)
        {
            byte val = GenericGetOperandFromOpcode(cpu);
            byte res = (byte)(cpu.Registers.A | val);

            cpu.Registers.A = res;

            cpu.ZeroFlag = cpu.ShouldSetZeroFlag(res);
            cpu.SubtractFlag = false;
            cpu.CarryFlag = false;
            cpu.HalfCarryFlag = false;
        }

        public static void DAA(Cpu cpu, List<string> args)
        {
            if (!cpu.SubtractFlag)
            {
                if (cpu.CarryFlag || cpu.Registers.A > 0x99)
                {
                    cpu.Registers.A += 0x60;
                    cpu.CarryFlag = true;
                }

                if (cpu.HalfCarryFlag || (cpu.Registers.A & 0xF) > 0x9)
                    cpu.Registers.A += 6;
            }
            else
            {
                if (cpu.CarryFlag)
                    cpu.Registers.A -= 0x60;
                
                if (cpu.HalfCarryFlag)
                    cpu.Registers.A -= 0x6;
            }

            cpu.ZeroFlag = cpu.ShouldSetZeroFlag(cpu.Registers.A);
            cpu.HalfCarryFlag = false;
        }

        public static void CPL(Cpu cpu, List<string> args)
        {
            cpu.Registers.A = (byte)(~cpu.Registers.A);
            cpu.SubtractFlag = true;
            cpu.HalfCarryFlag = true;
        }
    }
}
