using System;
using System.Collections.Generic;

namespace GB
{
    public static class Opcodes
    {
        public static readonly Dictionary<byte, Opcode> List = new Dictionary<byte, Opcode>
        {
            { 0x00, new Opcode("NOP", 1, null, null) },
            { 0x05, new Opcode("DEC B", 1, new OpcodeHandler(ARTHLOG8.DEC), new List<string>{"B"}) },
            { 0x06, new Opcode("LD B,d8", 2, new OpcodeHandler(LDSTMV8.LD), new List<string>{"B", "d8"}) },
            { 0x0D, new Opcode("DEC C", 1, new OpcodeHandler(ARTHLOG8.DEC), new List<string>{"C"}) },
            { 0x0E, new Opcode("LD C,d8", 2, new OpcodeHandler(LDSTMV8.LD), new List<string>{"D", "d8"}) },
            { 0x18, new Opcode("JR r8", 2, 0, new OpcodeHandler(ARTHLOG8.DEC), new List<string>{"r8"}) },
            { 0x20, new Opcode("JR NZ,r8", 2, 0, new OpcodeHandler(ARTHLOG8.JR), new List<string>{"NZ", "r8"}) },
            { 0x21, new Opcode("LD HL,d16", 3, new OpcodeHandler(LDSTMV16.LD), new List<string>{"HL", "d16"}) },
            { 0x32, new Opcode("LD (HL-),A", 1, new OpcodeHandler(LDSTMV8.LD), new List<string>{"(HL-)", "A"}) },
            { 0x3E, new Opcode("LD A,d8", 2, new OpcodeHandler(LDSTMV8.LD), new List<string>{"A", "d8"}) },
            { 0xC3, new Opcode("JP a16", 3, 0, new OpcodeHandler(ARTHLOG16.JP), new List<string>{"a16"}) },
            { 0xAF, new Opcode("XOR A", 1, new OpcodeHandler(ARTHLOG8.XOR), new List<string>(){"A"}) },
            { 0xF3, new Opcode("LD A,(C)", 1, new OpcodeHandler(LDSTMV8.LD), new List<string>{"A", "(C)"}) },
        };
    }
}