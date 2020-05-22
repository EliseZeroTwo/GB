using System;
using System.Collections.Generic;

namespace GB
{
    public static class LDSTMV16
    {
        public static void LD(Cpu cpu, List<string> args)
        {
            ushort operandAddr = (ushort)(cpu.Registers.PC + 1);
            switch(args[0])
            {
                case "BC":
                {
                    if (args[1] == "d16")
                        cpu.Memory.Read(out cpu.Registers.BC, operandAddr);
                    break;
                }
                case "DE":
                {
                    if (args[1] == "d16")
                        cpu.Memory.Read(out cpu.Registers.DE, operandAddr);
                    break;
                }
                case "HL":
                {
                    if (args[1] == "d16")
                        cpu.Memory.Read(out cpu.Registers.HL, operandAddr);
                    else if (args[1] == "SP+r8")
                    {
                        cpu.Memory.Read(out byte b, operandAddr);
                        cpu.Memory.Read(out cpu.Registers.HL, (ushort)(cpu.Registers.SP + (ushort)unchecked((sbyte)(b))));
                    }
                    break;
                }
                case "SP":
                {
                    if (args[1] == "d16")
                        cpu.Memory.Read(out cpu.Registers.SP, operandAddr);
                    break;
                }
                case "(a16)":
                {
                    if (args[1] == "SP")
                    {
                        cpu.Memory.Read(out ushort destAddr, operandAddr);
                        cpu.Memory.Write(cpu.Registers.SP, destAddr);
                    }
                    break;
                }
            }
        }
    }
}