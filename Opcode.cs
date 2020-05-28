using System.Collections.Generic;

namespace GB
{
    public delegate void OpcodeHandler(Cpu cpu, List<string> args);


    public class Opcode
    {
        public readonly string Mneumonic;
        public readonly ushort Length;
        public readonly ushort EffectiveLength;
        public readonly OpcodeHandler Handler;
        public readonly List<string> Args;
        public int Cycles;
        public void Execute(Cpu cpu)
        {
            if (Handler != null && Args != null)
                Handler(cpu, Args);
        }
        
        public Opcode(string mneu, ushort length, ushort effectiveLength, int cycles, OpcodeHandler handler, List<string> args)
        {
            Length = length;
            EffectiveLength = effectiveLength;
            Mneumonic = mneu;
            Handler = handler;
            Args = args;
            Cycles = cycles;
        }
        public Opcode(string mneu, ushort length, int cycles, OpcodeHandler handler, List<string> args)
        {
            Length = length;
            EffectiveLength = Length;
            Mneumonic = mneu;
            Handler = handler;
            Args = args;
            Cycles = cycles;
        }
    }
}