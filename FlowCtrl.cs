using System;
using System.Collections.Generic;

namespace GB
{
    public static class FlowCtrl
    {
        private static bool ConditionMet(Cpu cpu, string condition)
        {
            switch(condition)
            {
                case "Z":
                    return cpu.ZeroFlag.Value;
                case "C":
                    return cpu.CarryFlag.Value;
                case "NZ":
                    return !cpu.ZeroFlag.Value;
                case "NC":
                    return !cpu.CarryFlag.Value;
                default:
                    return true;
            }
        }

        public static void JMP(Cpu cpu, List<string> args)
        {
            ushort operandAddr = (ushort)(cpu.Registers.PC + 1);
            bool doJmp = ConditionMet(cpu, args[0]);
            Console.WriteLine(doJmp);
            
            if((args[0] == "r8" || (args.Count == 2 && args[1] == "r8")))
            {
                Console.WriteLine("owo");
                bool neg = false;
                byte jumpDistanceByte = cpu.Memory[operandAddr];
                if (jumpDistanceByte > 127) // Two's Compliment
                {
                    jumpDistanceByte ^= 0xFF;
                    jumpDistanceByte--;
                    neg = true;
                }

                if (doJmp)
                {
                    if (!neg)
                        cpu.Registers.PC += jumpDistanceByte;
                    else
                        cpu.Registers.PC -= jumpDistanceByte;
                }
                else
                    cpu.Registers.PC += 2;
            }
            else if((args[0] == "a16" || (args.Count == 2 && args[1] == "a16")))
            {
                cpu.Memory.Read(out ushort jumpOffset, operandAddr);
                if (doJmp)
                    cpu.Registers.PC = jumpOffset;
                else
                    cpu.Registers.PC += 3;
            }
        }
    }
}