using System;
using System.Collections.Generic;
namespace GB
{
    public static class ARTHLOG8
    {
        public static void DEC(Cpu cpu, List<string> args)
        {
            cpu.SubtractFlag.Value = true;
            switch (args[0])
            {
                case "A":
                case "B":
                case "C":
                case "D":
                case "E":
                case "H":
                case "L":
                {
                    byte oldRegVal = cpu.GetRegisterByName<byte>(args[0]);
                    cpu.SetRegisterByName(args[0], --oldRegVal);
                    cpu.HalfCarryFlag.Value = cpu.ShouldSetHalfCarry(oldRegVal, (byte)(oldRegVal + 1));
                    cpu.ZeroFlag.Value = cpu.ShouldSetZeroFlag(oldRegVal);
                    break;
                }
                case "(HL)":
                {
                    byte oldVal = cpu. Memory[cpu.Registers.HL];
                    cpu. Memory[cpu.Registers.HL]--;
                    cpu.ZeroFlag.Value = cpu.ShouldSetZeroFlag((byte)(oldVal - 1));
                    cpu.HalfCarryFlag.Value = cpu.ShouldSetHalfCarry(oldVal, (byte)(oldVal - 1));
                   break;
                }
            }
        }

        public static void INC(Cpu cpu, List<string> args)
        {
            cpu.SubtractFlag.Value = false;
            switch (args[0])
            {
                case "A":
                case "B":
                case "C":
                case "D":
                case "E":
                case "H":
                case "L":
                {
                    byte regVal = cpu.GetRegisterByName<byte>(args[0]);
                    cpu.SetRegisterByName(args[0], ++regVal);
                    cpu.HalfCarryFlag.Value = cpu.ShouldSetHalfCarry(regVal, (byte)(regVal - 1));
                    cpu.ZeroFlag.Value = cpu.ShouldSetZeroFlag(regVal);
                    break;
                }
                case "(HL)":
                {
                    byte oldVal = cpu. Memory[cpu.Registers.HL];
                    cpu. Memory[cpu.Registers.HL]++;
                    cpu.ZeroFlag.Value = cpu.ShouldSetZeroFlag((byte)(oldVal + 1));
                    cpu.HalfCarryFlag.Value = cpu.ShouldSetHalfCarry(oldVal, (byte)(oldVal + 1));
                   break;
                }
            }
        }

        public static void CP(Cpu cpu, List<string> args)
        {
            cpu.SubtractFlag.Value = true;

            byte value = 0;

            switch(args[0])
            {
                case "A":
                case "B":
                case "C":
                case "D":
                case "E":
                case "H":
                case "L":
                {
                    value = cpu.GetRegisterByName<byte>(args[0]);
                    break;
                }
                case "d8":
                case "(HL)":
                {
                    ushort addr = (ushort)(cpu.Registers.PC + 1);
                    if (args[0] == "(HL)")
                        addr = cpu.Registers.HL;
                    value = cpu.Memory[addr]; 
                    break;
                }
            }

            cpu.ZeroFlag.Value = cpu.ShouldSetZeroFlag((byte)(cpu.Registers.A - value));
            cpu.HalfCarryFlag.Value = cpu.ShouldSetHalfCarry(cpu.Registers.A, value);
            cpu.CarryFlag.Value = cpu.ShouldSetCarryFlag(cpu.Registers.A, value);
            
        }

        public static void ADC(Cpu cpu, List<string> args)
        {
            byte oldVal = cpu.Registers.A;
            if (args[1] == "(HL)")
                cpu.Registers.A += (byte)(cpu.Memory[cpu.Registers.HL] + (byte)(cpu.Registers.F & Flags.Carry));
            else if (args[1] == "d8")
                cpu.Registers.A += (byte)(cpu.Memory[(ushort)(cpu.Registers.PC + 1)] + (byte)(cpu.Registers.F & Flags.Carry));
            else
                cpu.Registers.A += (byte)(cpu.GetRegisterByName<byte>(args[1]) + (byte)(cpu.Registers.F & Flags.Carry));

            cpu.ZeroFlag.Value = cpu.ShouldSetZeroFlag(cpu.Registers.A);
            cpu.SubtractFlag.Value = false;
            cpu.CarryFlag.Value = cpu.ShouldSetCarryFlag(oldVal, cpu.Registers.A);
            cpu.HalfCarryFlag.Value = cpu.ShouldSetHalfCarry(oldVal, cpu.Registers.A);
        }

        public static void ADD(Cpu cpu, List<string> args)
        {
            byte oldVal = cpu.Registers.A;
            if (args[1] == "(HL)")
                cpu.Registers.A += cpu.Memory[cpu.Registers.HL];
            else if (args[1] == "d8")
                cpu.Registers.A += cpu.Memory[(ushort)(cpu.Registers.PC + 1)];
            else
                cpu.Registers.A += cpu.GetRegisterByName<byte>(args[1]);

            cpu.ZeroFlag.Value = cpu.ShouldSetZeroFlag(cpu.Registers.A);
            cpu.SubtractFlag.Value = false;
            cpu.CarryFlag.Value = cpu.ShouldSetCarryFlag(oldVal, cpu.Registers.A);
            cpu.HalfCarryFlag.Value = cpu.ShouldSetHalfCarry(oldVal, cpu.Registers.A);
        }

        public static void SBC(Cpu cpu, List<string> args)
        {
            byte oldVal = cpu.Registers.A;
            if (args[1] == "(HL)")
                cpu.Registers.A -= (byte)(cpu.Memory[cpu.Registers.HL] - (byte)(cpu.Registers.F & Flags.Carry));
            else if (args[1] == "d8")
                cpu.Registers.A -= (byte)(cpu.Memory[(ushort)(cpu.Registers.PC + 1)] - (byte)(cpu.Registers.F & Flags.Carry));
            else
                cpu.Registers.A -= (byte)(cpu.GetRegisterByName<byte>(args[1]) - (byte)(cpu.Registers.F & Flags.Carry));

            cpu.ZeroFlag.Value = cpu.ShouldSetZeroFlag(cpu.Registers.A);
            cpu.SubtractFlag.Value = true;
            cpu.CarryFlag.Value = cpu.ShouldSetCarryFlag(oldVal, cpu.Registers.A);
            cpu.HalfCarryFlag.Value = cpu.ShouldSetHalfCarry(oldVal, cpu.Registers.A);
        }

        public static void SUB(Cpu cpu, List<string> args)
        {
            byte oldVal = cpu.Registers.A;
            if (args[0] == "(HL)")
                cpu.Registers.A -= cpu.Memory[cpu.Registers.HL];
            else if (args[0] == "d8")
                cpu.Registers.A -= cpu.Memory[(ushort)(cpu.Registers.PC + 1)];
            else
                cpu.Registers.A -= cpu.GetRegisterByName<byte>(args[0]);

            cpu.ZeroFlag.Value = cpu.ShouldSetZeroFlag(cpu.Registers.A);
            cpu.SubtractFlag.Value = true;
            cpu.CarryFlag.Value = cpu.ShouldSetCarryFlag(oldVal, cpu.Registers.A);
            cpu.HalfCarryFlag.Value = cpu.ShouldSetHalfCarry(oldVal, cpu.Registers.A);
        }

        public static void AND(Cpu cpu, List<string> args)
        {
            byte oldVal = cpu.Registers.A;
            if (args[0] == "(HL)")
                cpu.Registers.A &= cpu.Memory[cpu.Registers.HL];
            else if (args[0] == "d8")
                cpu.Registers.A &= cpu.Memory[(ushort)(cpu.Registers.PC + 1)];
            else
                cpu.Registers.A &= cpu.GetRegisterByName<byte>(args[0]);

            cpu.ZeroFlag.Value = cpu.ShouldSetZeroFlag(cpu.Registers.A);
            cpu.SubtractFlag.Value = false;
            cpu.CarryFlag.Value = false;
            cpu.HalfCarryFlag.Value = cpu.ShouldSetHalfCarry(oldVal, cpu.Registers.A);
        }

        public static void XOR(Cpu cpu, List<string> args)
        {
            if (args[0] == "(HL)")
                cpu.Registers.A ^= cpu.Memory[cpu.Registers.HL];
            else if (args[0] == "d8")
                cpu.Registers.A ^= cpu.Memory[(ushort)(cpu.Registers.PC + 1)];
            else
                cpu.Registers.A ^= cpu.GetRegisterByName<byte>(args[0]);

            cpu.ZeroFlag.Value = cpu.ShouldSetZeroFlag(cpu.Registers.A);
            cpu.SubtractFlag.Value = false;
            cpu.CarryFlag.Value = false;
            cpu.HalfCarryFlag.Value = false;
        }

        public static void OR(Cpu cpu, List<string> args)
        {
            if (args[0] == "(HL)")
                cpu.Registers.A |= cpu.Memory[cpu.Registers.HL];
            else if (args[0] == "d8")
                cpu.Registers.A |= cpu.Memory[(ushort)(cpu.Registers.PC + 1)];
            else
                cpu.Registers.A |= cpu.GetRegisterByName<byte>(args[0]);

            cpu.ZeroFlag.Value = cpu.ShouldSetZeroFlag(cpu.Registers.A);
            cpu.SubtractFlag.Value = false;
            cpu.CarryFlag.Value = false;
            cpu.HalfCarryFlag.Value = false;
        }
    }
}
