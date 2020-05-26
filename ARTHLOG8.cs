using System;
using System.Collections.Generic;
namespace GB
{
    public static class ARTHLOG8
    {
        public static void DEC(Cpu cpu, List<string> args)
        {
            cpu.SubtractFlag = true;
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
                    cpu.SetRegisterByName(args[0], (byte)(regVal - 1));
                    cpu.HalfCarryFlag = cpu.ShouldSetHalfCarry(regVal, cpu.GetRegisterByName<byte>(args[0]));
                    cpu.ZeroFlag = cpu.ShouldSetZeroFlag(cpu.GetRegisterByName<byte>(args[0]));
                    break;
                }
                case "(HL)":
                {
                    byte oldVal = cpu.Memory[cpu.Registers.HL];
                    cpu.Memory[cpu.Registers.HL]--;
                    cpu.ZeroFlag = cpu.ShouldSetZeroFlag(cpu.Memory[cpu.Registers.HL]);
                    cpu.HalfCarryFlag = cpu.ShouldSetHalfCarry(cpu.Memory[cpu.Registers.HL], oldVal);
                   break;
                }
            }
        }

        public static void INC(Cpu cpu, List<string> args)
        {
            cpu.SubtractFlag = false;
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
                    cpu.SetRegisterByName(args[0], (byte)(regVal + 1));
                    cpu.HalfCarryFlag = cpu.ShouldSetHalfCarry(regVal, cpu.GetRegisterByName<byte>(args[0]));
                    cpu.ZeroFlag = cpu.ShouldSetZeroFlag(cpu.GetRegisterByName<byte>(args[0]));
                    break;
                }
                case "(HL)":
                {
                    byte oldVal = cpu.Memory[cpu.Registers.HL];
                    cpu.Memory[cpu.Registers.HL]++;
                    cpu.ZeroFlag = cpu.ShouldSetZeroFlag(cpu.Memory[cpu.Registers.HL]);
                    cpu.HalfCarryFlag = cpu.ShouldSetHalfCarry(cpu.Memory[cpu.Registers.HL], oldVal);
                   break;
                }
            }
        }

        public static void CP(Cpu cpu, List<string> args)
        {
            cpu.SubtractFlag = true;

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

            cpu.ZeroFlag = cpu.ShouldSetZeroFlag((byte)(cpu.Registers.A - value));
            cpu.HalfCarryFlag = cpu.ShouldSetHalfCarry(cpu.Registers.A, (byte)(cpu.Registers.A - value));
            cpu.CarryFlag = cpu.ShouldSetCarryFlag(cpu.Registers.A, (byte)(cpu.Registers.A - value));   
        }

        public static void ADC(Cpu cpu, List<string> args)
        {
            byte oldVal = cpu.Registers.A;
            if (args[1] == "(HL)")
                cpu.Registers.A += (byte)(cpu.Memory[cpu.Registers.HL] + (byte)(cpu.Registers.F & (byte)Flags.Carry));
            else if (args[1] == "d8")
                cpu.Registers.A += (byte)(cpu.Memory[(ushort)(cpu.Registers.PC + 1)] + (byte)(cpu.Registers.F & (byte)Flags.Carry));
            else
                cpu.Registers.A += (byte)(cpu.GetRegisterByName<byte>(args[1]) + (byte)(cpu.Registers.F & (byte)Flags.Carry));

            cpu.ZeroFlag = cpu.ShouldSetZeroFlag(cpu.Registers.A);
            cpu.SubtractFlag = false;
            cpu.CarryFlag = cpu.ShouldSetCarryFlag(oldVal, cpu.Registers.A);
            cpu.HalfCarryFlag = cpu.ShouldSetHalfCarry(oldVal, cpu.Registers.A);
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

            cpu.ZeroFlag = cpu.ShouldSetZeroFlag(cpu.Registers.A);
            cpu.SubtractFlag = false;
            cpu.CarryFlag = cpu.ShouldSetCarryFlag(oldVal, cpu.Registers.A);
            cpu.HalfCarryFlag = cpu.ShouldSetHalfCarry(oldVal, cpu.Registers.A);
        }

        public static void SBC(Cpu cpu, List<string> args)
        {
            byte oldVal = cpu.Registers.A;
            if (args[1] == "(HL)")
                cpu.Registers.A -= (byte)(cpu.Memory[cpu.Registers.HL] - (byte)(cpu.Registers.F & (byte)Flags.Carry));
            else if (args[1] == "d8")
                cpu.Registers.A -= (byte)(cpu.Memory[(ushort)(cpu.Registers.PC + 1)] - (byte)(cpu.Registers.F & (byte)Flags.Carry));
            else
                cpu.Registers.A -= (byte)(cpu.GetRegisterByName<byte>(args[1]) - (byte)(cpu.Registers.F & (byte)Flags.Carry));

            cpu.ZeroFlag = cpu.ShouldSetZeroFlag(cpu.Registers.A);
            cpu.SubtractFlag = true;
            cpu.CarryFlag = cpu.ShouldSetCarryFlag(oldVal, cpu.Registers.A);
            cpu.HalfCarryFlag = cpu.ShouldSetHalfCarry(oldVal, cpu.Registers.A);
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

            cpu.ZeroFlag = cpu.ShouldSetZeroFlag(cpu.Registers.A);
            cpu.SubtractFlag = true;
            cpu.CarryFlag = cpu.ShouldSetCarryFlag(oldVal, cpu.Registers.A);
            cpu.HalfCarryFlag = cpu.ShouldSetHalfCarry(oldVal, cpu.Registers.A);
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

            cpu.ZeroFlag = cpu.ShouldSetZeroFlag(cpu.Registers.A);
            cpu.SubtractFlag = false;
            cpu.CarryFlag = false;
            cpu.HalfCarryFlag = cpu.ShouldSetHalfCarry(oldVal, cpu.Registers.A);
        }

        public static void XOR(Cpu cpu, List<string> args)
        {
            if (args[0] == "(HL)")
                cpu.Registers.A ^= cpu.Memory[cpu.Registers.HL];
            else if (args[0] == "d8")
                cpu.Registers.A ^= cpu.Memory[(ushort)(cpu.Registers.PC + 1)];
            else
                cpu.Registers.A ^= cpu.GetRegisterByName<byte>(args[0]);

            cpu.ZeroFlag = cpu.ShouldSetZeroFlag(cpu.Registers.A);
            cpu.SubtractFlag = false;
            cpu.CarryFlag = false;
            cpu.HalfCarryFlag = false;
        }

        public static void OR(Cpu cpu, List<string> args)
        {
            if (args[0] == "(HL)")
                cpu.Registers.A |= cpu.Memory[cpu.Registers.HL];
            else if (args[0] == "d8")
                cpu.Registers.A |= cpu.Memory[(ushort)(cpu.Registers.PC + 1)];
            else
                cpu.Registers.A |= cpu.GetRegisterByName<byte>(args[0]);

            cpu.ZeroFlag = cpu.ShouldSetZeroFlag(cpu.Registers.A);
            cpu.SubtractFlag = false;
            cpu.CarryFlag = false;
            cpu.HalfCarryFlag = false;
        }
        // Referenced khedoros's code
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
                    cpu.Registers.A += 0x6;
                
            }
            else
            {
                if (cpu.CarryFlag)
                {
                    cpu.Registers.A -= 0x60;
                    cpu.CarryFlag = true;
                }

                if (cpu.HalfCarryFlag)
                    cpu.Registers.A -= 0x6;
            }

            cpu.HalfCarryFlag = false;
            cpu.ZeroFlag = cpu.ShouldSetZeroFlag(cpu.Registers.A);
        }

        public static void CPL(Cpu cpu, List<string> args)
        {
            cpu.SubtractFlag = true;
            cpu.HalfCarryFlag = true;
            cpu.Registers.A = (byte)~cpu.Registers.A;
        }
    }
}
