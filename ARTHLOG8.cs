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
            
            cpu.ZeroFlag.Value = cpu.ShouldSetZeroFlag(cpu.Registers.A);
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
    }
}
