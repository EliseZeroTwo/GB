using System.Collections.Generic;

namespace GB
{
    public static class LDSTMV8
    {
        private static void _LD_REG8_x8(Cpu cpu, ref byte reg, string arg)
        {
            switch (arg)
            {
                case "a8":
                case "d8":
                {
                    cpu.Memory.Read(out reg, (ushort)(cpu.Registers.PC + 1));
                    break;
                }
                case "A":
                {
                    reg = cpu.Registers.A;
                    break;
                }
                case "B":
                {
                    reg = cpu.Registers.B;
                    break;
                }
                case "(C)":
                {
                    cpu.Memory.Read(out reg, cpu.Registers.C);
                    break;
                }
                case "C":
                {
                    reg = cpu.Registers.C;
                    break;
                }
                case "D":
                {
                    reg = cpu.Registers.D;
                    break;
                }
                case "E":
                {
                    reg = cpu.Registers.E;
                    break;
                }
                case "H":
                {
                    reg = cpu.Registers.H;
                    break;
                }
                case "L":
                {
                    reg = cpu.Registers.L;
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
                {
                    _LD_REG8_x8(cpu, ref cpu.Registers.A, argList[1]);
                    break;
                }
                case "B":
                {
                    _LD_REG8_x8(cpu, ref cpu.Registers.B, argList[1]);
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
                case "C":
                {
                    _LD_REG8_x8(cpu, ref cpu.Registers.C, argList[1]);
                    break;
                }
                case "D":
                {
                    _LD_REG8_x8(cpu, ref cpu.Registers.D, argList[1]);
                    break;
                }
                case "E":
                {
                    _LD_REG8_x8(cpu, ref cpu.Registers.E, argList[1]);
                    break;
                }
                case "H":
                {
                    _LD_REG8_x8(cpu, ref cpu.Registers.H, argList[1]);
                    break;
                }
                case "L":
                {
                    _LD_REG8_x8(cpu, ref cpu.Registers.L, argList[1]);
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
        
    }
}