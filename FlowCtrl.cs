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
                    return cpu.ZeroFlag;
                case "C":
                    return cpu.CarryFlag;
                case "NZ":
                    return !cpu.ZeroFlag;
                case "NC":
                    return !cpu.CarryFlag;
                default:
                    return true;
            }
        }

        public static void RET(Cpu cpu, List<string> args)
        {
            bool doRet = args.Count != 0 ? ConditionMet(cpu, args[0]) : true;
            if (doRet)
            {
                cpu.StackPop(out ushort addr);
                cpu.Registers.PC = addr;
            }
            else
                cpu.Registers.PC += 1;
        }

        public static void RETI(Cpu cpu, List<string> args)
        {
            cpu.StackPop(out ushort addr);
            cpu.Registers.PC = addr;
            cpu.IME = true;
        }

        public static void CALL(Cpu cpu, List<string> args)
        {
            bool doJmp = ConditionMet(cpu, args[0]);
            ushort operandAddr = (ushort)(cpu.Registers.PC + 1);
            cpu.Memory.Read(out ushort dstAddr, operandAddr);

            if (doJmp)
            {
                cpu.StackPush((ushort)(cpu.Registers.PC + 3));
                cpu.Registers.PC = dstAddr;
            } 
            else
                cpu.Registers.PC += 3;
        }

        public static void JMP(Cpu cpu, List<string> args)
        {
            ushort operandAddr = (ushort)(cpu.Registers.PC + 1);
            bool doJmp = ConditionMet(cpu, args[0]);
            
            if((args[0] == "r8" || (args.Count == 2 && args[1] == "r8")))
            {
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