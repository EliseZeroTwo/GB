using System;
using System.Collections.Generic;

namespace GB
{
    public static class ARTHLOG16
    {
        public static void JP(Cpu cpu, List<string> args)
        {
            ushort operandAddr = (ushort)(cpu.Registers.PC + 1);
            bool doJmp = false;
            switch(args[0])
            {
                case "a16":
                {
                    doJmp = true;
                    break;
                }
                case "HL":
                {
                    cpu.Registers.PC = cpu.Registers.HL;
                    return;
                }
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
                cpu.Memory.Read(out cpu.Registers.PC, operandAddr);
            else
                cpu.Registers.PC += 3;
        }    
    }
}