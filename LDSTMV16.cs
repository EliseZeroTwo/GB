using System;
using System.Collections.Generic;

namespace GB
{
    public static class LDSTMV16
    {
        public static void LD(Cpu cpu, List<string> args)
        {
            ushort operandAddr = (ushort)(cpu.Registers.PC + 1);
            switch (args[0])
            {
                case "BC":
                case "DE":
                case "HL":
                case "SP":
                {
                    switch (args[1])
                    {
                        case "d16":
                        {
                            cpu.Memory.Read(out ushort data, operandAddr);
                            cpu.SetRegisterByName(args[0], data);
                            break;
                        }
                        case "SP+r8":
                        {
                            cpu.SetRegisterByName(args[0], (cpu.Registers.SP + cpu.Memory[operandAddr]));
                            break;
                        }
                        case "HL":
                        {
                            cpu.SetRegisterByName(args[0], (cpu.Registers.HL));
                            break;
                        }
                    }
                    break;
                }
                case "(a16)":
                {
                    if (args[1] == "SP")
                    {
                        cpu.Memory.Read(out ushort addr, operandAddr);
                        cpu.Memory.Write(cpu.Registers.SP, addr);
                    }
                    break;
                }
            }
        }

        public static void PUSH(Cpu cpu, List<string> args)
        {
            cpu.StackPush(cpu.GetRegisterByName<ushort>(args[0]));
        }

        public static void POP(Cpu cpu, List<string> args)
        {
            cpu.StackPop(out ushort value);
            cpu.SetRegisterByName(args[0], value);
        }
    }
}