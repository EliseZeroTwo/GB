using System;
using System.Collections.Generic;

namespace GB
{
    public static class PrefixedOps
    {
        public delegate byte PrefixedOpcodeHandler(Cpu cpu, byte opcode);
        public static readonly Dictionary<byte, PrefixedOpcodeHandler> List = new Dictionary<byte, PrefixedOpcodeHandler>
        {
            { 0x00, new PrefixedOpcodeHandler(RxC) },
            { 0x30, new PrefixedOpcodeHandler(SWAP) },
            { 0x40, new PrefixedOpcodeHandler(BIT) },
            { 0x50, new PrefixedOpcodeHandler(BIT) },
            { 0x60, new PrefixedOpcodeHandler(BIT) },
            { 0x70, new PrefixedOpcodeHandler(BIT) },
        };
        
        public static bool DerefHL(byte opcode)
        {
            byte opcodeLo = (byte)(opcode & 0xF);
            return (opcodeLo == 6 || opcodeLo == 0xE);
        }

        public static void SetOperand(Cpu cpu, byte opcode, byte value)
        {
            switch (opcode & 0xF)
            {
                case 0x0:
                case 0x8: // B
                    cpu.Registers.B = value;
                    break;
                case 0x1:
                case 0x9: // C
                    cpu.Registers.C = value;
                    break;
                case 0x2:
                case 0xA: // D
                    cpu.Registers.D = value;
                    break;
                case 0x3:
                case 0xB: // E
                    cpu.Registers.E = value;
                    break;
                case 0x4:
                case 0xC: // H
                    cpu.Registers.H = value;
                    break;
                case 0x5:
                case 0xD: // L
                    cpu.Registers.L = value;
                    break;
                case 0x6:
                case 0xE: // (HL)
                    cpu.Memory[cpu.Registers.HL] = value;
                    break;
                case 0x7:
                case 0xF: // A
                    cpu.Registers.A = value;
                    break;
            }
        }

        public static byte GetOperand(Cpu cpu, byte opcode)
        {
            switch (opcode & 0xF)
            {
                case 0x0:
                case 0x8: // B
                    return cpu.Registers.B;
                case 0x1:
                case 0x9: // C
                    return cpu.Registers.C;
                case 0x2:
                case 0xA: // D
                    return cpu.Registers.D;
                case 0x3:
                case 0xB: // E
                    return cpu.Registers.E;
                case 0x4:
                case 0xC: // H
                    return cpu.Registers.H;
                case 0x5:
                case 0xD: // L
                    return cpu.Registers.L;
                case 0x6:
                case 0xE: // (HL)
                    return cpu.Memory[cpu.Registers.HL];
                case 0x7:
                case 0xF: // A
                    return cpu.Registers.A;
            }
            throw new ApplicationException($"Failed to parse opcode {opcode}");
        }

        public static byte SWAP(Cpu cpu, byte opcode)
        {
            byte length = (byte)(DerefHL(opcode) ? 16 : 8);
            byte value = GetOperand(cpu, opcode);

            value = (byte)(((value & 0xF) << 4) | ((value & 0xF0) >> 4));
            
            SetOperand(cpu, opcode, value);

            cpu.SubtractFlag = false;
            cpu.HalfCarryFlag = false;
            cpu.CarryFlag = false;
            cpu.ZeroFlag = cpu.ShouldSetZeroFlag(value);

            return length;
        }

        public static byte RxC(Cpu cpu, byte opcode)
        {            
            bool left = opcode < 7;
            byte length = (byte)(DerefHL(opcode) ? 16 : 8);
            byte value = GetOperand(cpu, opcode);
            byte origVal = value;
            byte valueFinalBit = (byte)(value & (1 << (left ? 7 : 0)));

            value = (byte)(left ? (value << 1) : (value >> 1)); 
            value |= valueFinalBit;

            SetOperand(cpu, opcode, value);

            cpu.SubtractFlag = false;
            cpu.HalfCarryFlag = false;
            cpu.CarryFlag = cpu.ShouldSetCarryFlag(origVal, value);
            cpu.ZeroFlag = cpu.ShouldSetZeroFlag(value);

            return length;
        }

        public static byte BIT(Cpu cpu, byte opcode)
        {
            bool left = (opcode & 0xF) < 7;
            byte bit = (byte)((((opcode & 0xF0) >> 8) - 4) + (left ? 0 : 1));
            cpu.HalfCarryFlag = true;
            cpu.SubtractFlag = false;
            
            byte value = GetOperand(cpu, opcode);
            cpu.ZeroFlag = (value & (1 << bit)) == 0;
            return (byte)(DerefHL(opcode) ? 16 : 8); 
        }
        public static void PrefixedOp(Cpu cpu, List<string> args)
        {
            byte actualInst = cpu.Memory[(ushort)(cpu.Registers.PC + 1)];
            System.Console.WriteLine($"  -   0x{actualInst:X}");
            
            if (List.TryGetValue((byte)(actualInst & 0xF0), out PrefixedOpcodeHandler opcode))
            {
                cpu.CurrentInst.Cycles += (ushort)opcode(cpu, actualInst);
            }
            else
            {
                throw new Exception($"Prefixed Op 0x{actualInst:X} not implemented");
            }
        }
    }
}