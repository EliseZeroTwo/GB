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
        public void Execute(Cpu cpu)
        {
            if (Handler != null && Args != null)
                Handler(cpu, Args);
        }
        
        public Opcode(string mneu, ushort length, ushort effectiveLength, OpcodeHandler handler, List<string> args)
        {
            Length = length;
            EffectiveLength = effectiveLength;
            Mneumonic = mneu;
            Handler = handler;
            Args = args;
        }
        public Opcode(string mneu, ushort length, OpcodeHandler handler, List<string> args)
        {
            Length = length;
            EffectiveLength = Length;
            Mneumonic = mneu;
            Handler = handler;
            Args = args;
        }
    }
}