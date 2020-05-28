using System;
using System.Collections.Generic;

namespace GB
{
    public static class ARTHLOG16
    {
        public static ushort GetRegByOpcode(Cpu cpu)
        {
            switch (cpu.Memory[cpu.Registers.PC] & 0xF0)
            {
                case 0x00:
                    return cpu.Registers.BC;
                case 0x10:
                    return cpu.Registers.DE;
                case 0x20:
                    return cpu.Registers.HL;
                case 0x30:
                    return cpu.Registers.SP;
            }
            throw new Exception($"Failed to parse OpCode 0x{cpu.Memory[cpu.Registers.PC]:X}");
        }

        public static void SetRegByOpcode(Cpu cpu, ushort value)
        {
            switch (cpu.Memory[cpu.Registers.PC] & 0xF0)
            {
                case 0x00:
                    cpu.Registers.BC = value;
                    return;
                case 0x10:
                    cpu.Registers.DE = value;
                    return;
                case 0x20:
                    cpu.Registers.HL = value;
                    return;
                case 0x30:
                    cpu.Registers.SP = value;
                    return;
            }
            throw new Exception($"Failed to parse OpCode 0x{cpu.Memory[cpu.Registers.PC]:X}");
        }
        public static void DEC(Cpu cpu, List<string> args)
        {
            SetRegByOpcode(cpu, (ushort)(GetRegByOpcode(cpu) - 1));
        }

        public static void INC(Cpu cpu, List<string> args)
        {
            SetRegByOpcode(cpu, (ushort)(GetRegByOpcode(cpu) + 1));
        }

        public static void ADD(Cpu cpu, List<string> args)
        {
            cpu.SubtractFlag = false;
            ushort origValue;
            ushort newValue;
            if ((cpu.Memory[cpu.Registers.PC] & 0xF0) != 0xE0) // First Operand is HL
            {
                origValue = cpu.Registers.HL;
                cpu.Registers.HL += GetRegByOpcode(cpu);
                newValue = cpu.Registers.HL;
            }
            else // First Operand is SP
            {
                origValue = cpu.Registers.SP;
                byte jumpDistanceByte = cpu.Memory[(ushort)(cpu.Registers.PC + 1)];
                if (jumpDistanceByte > 127) // Two's Compliment
                {
                    jumpDistanceByte ^= 0xFF;
                    jumpDistanceByte--;
                    cpu.Registers.SP -= jumpDistanceByte;
                }
                else
                    cpu.Registers.SP += jumpDistanceByte;

                newValue = cpu.Registers.SP;
            }
        }
    }
}