using System;
using System.Collections.Generic;

namespace GB
{
    public static class LDSTMV8
    {
        public static void LD(Cpu cpu, List<string> argList)
        {
            ushort operandAddr = (ushort)(cpu.Registers.PC + 1);
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
                    byte value = 0;

                    if (argList[1] == "(C)")
                        value = cpu.Memory[cpu.Registers.C];
                    else if (argList[1] == "a8" || argList[1] == "d8")
                        value = cpu.Memory[operandAddr];
                    else if (argList[1] == "(a16)")
                    {
                        cpu.Memory.Read(out ushort addr, operandAddr);
                        value = cpu.Memory[addr];
                    }
                    cpu.SetRegisterByName(argList[0], value);
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
                    cpu.Memory[cpu.Registers.BC] = cpu.Registers.A;
                    break;
                }
                case "(DE)":
                {
                    cpu.Memory[cpu.Registers.DE] = cpu.Registers.A;
                    break;
                }
                case "(HL)":
                {     
                    cpu.Memory[cpu.Registers.HL] = cpu.Memory[operandAddr];       
                    break;
                }
                case "(HL-)":
                {
                    cpu.Memory[cpu.Registers.HL--] = cpu.Registers.A;
                    break;
                }
                case "(HL+)":
                {
                    cpu.Memory[cpu.Registers.HL++] = cpu.Registers.A;
                    break;
                }
                case "(a16)":
                {
                    cpu.Memory.Read(out ushort addr, operandAddr);
                    cpu.Memory[addr] = cpu.GetRegisterByName<byte>(argList[1]);             
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