using System;
using System.Collections.Generic;

namespace GB
{
    public static class LDSTMV8
    {
        private static void _LD_REG8_x8(Cpu cpu, string reg, string arg)
        {
            switch (arg)
            {
                case "a8":
                case "d8":
                case "(C)":
                {
                    ushort addr = (ushort)(cpu.Registers.PC + 1);
                    if (arg == "(C)")
                        addr = cpu.Registers.C;
                    cpu.Memory.Read(out byte b, addr);
                    cpu.SetRegisterByName(reg, b);
                    break;
                }
                case "A":
                case "B":
                case "C":
                case "D":
                case "E":
                case "H":
                case "L":
                {
                    cpu.SetRegisterByName(reg, cpu.GetRegisterByName<byte>(arg));
                    break;
                }
            }
        }

        private static void _LD_MEM_x8(Cpu cpu, ushort offset, string arg )
        {
            ushort operandAddr = (ushort)(cpu.Registers.PC + 1);
            switch (arg)
            {
                case "A":
                {
                    cpu.Memory.Write(cpu.Registers.A, cpu.Registers.HL);
                    break;
                }
                case "B":
                {
                    cpu.Memory.Write(cpu.Registers.B, cpu.Registers.HL);
                    break;
                }
                case "C":
                {
                    cpu.Memory.Write(cpu.Registers.C, cpu.Registers.HL);
                    break;
                }
                case "D":
                {
                    cpu.Memory.Write(cpu.Registers.D, cpu.Registers.HL);
                    break;
                }
                case "E":
                {
                    cpu.Memory.Write(cpu.Registers.E, cpu.Registers.HL);
                    break;
                }
                case "H":
                {
                    cpu.Memory.Write(cpu.Registers.H, cpu.Registers.HL);
                    break;
                }
                case "L":
                {
                    cpu.Memory.Write(cpu.Registers.L, cpu.Registers.HL);
                    break;
                }
                case "d8":
                {
                    cpu.Memory.Read(out byte b, operandAddr);
                    cpu.Memory.Write(cpu.Registers.L, cpu.Registers.HL);
                    break;
                }
            }
        }
        public static void LD(Cpu cpu, List<string> argList)
        {
            switch (argList[0])
            {
                case "A":
                case "B":
                case "C":
                case "D":
                case "E":
                case "H":
                case "L":
                {
                    _LD_REG8_x8(cpu, argList[0], argList[1]);
                    break;
                }
                case "(C)":
                {
                    if (argList[1] == "A")
                    {
                        cpu.Memory.Write(cpu.Registers.A, cpu.Registers.C); 
                    }
                    break;
                }
                case "(BC)":
                {
                    cpu.Memory.Write(cpu.Registers.A, cpu.Registers.BC);
                    break;
                }
                case "(DE)":
                {
                    cpu.Memory.Write(cpu.Registers.A, cpu.Registers.DE);
                    break;
                }
                case "(HL)":
                {
                    _LD_MEM_x8(cpu, cpu.Registers.HL, argList[1]);               
                    break;
                }
                case "(HL-)":
                {
                    cpu.Memory.Write(cpu.Registers.A, cpu.Registers.HL--); 
                    break;
                }
                case "(HL+)":
                {
                    cpu.Memory.Write(cpu.Registers.A, cpu.Registers.HL++); 
                    break;
                }
            }
        }
        public static void LDH(Cpu cpu, List<string> args)
        {
            ushort operandAddr = (ushort)(cpu.Registers.PC + 1);
            ushort addr = (ushort)(0xFF00 + cpu.Memory[operandAddr]);
            switch (args[0])
            {
                case "A":
                {
                    cpu.Memory.Read(out cpu.Registers.A, addr);
                    break;
                }
                case "(a8)":
                {
                    cpu.Memory.Write(cpu.Registers.A, addr);
                    break;
                }
            }
        }
    }
}