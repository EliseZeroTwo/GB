using System;
using System.Collections.Generic;

namespace GB
{
    public static class ARTHLOG16
    {
        public static void DEC(Cpu cpu, List<string> args)
        {
            cpu.SetRegisterByName(args[0], (ushort)(cpu.GetRegisterByName<ushort>(args[0]) - 1));
        }

        public static void INC(Cpu cpu, List<string> args)
        {
            cpu.SetRegisterByName(args[0], (ushort)(cpu.GetRegisterByName<ushort>(args[0]) + 1));
        }
    }
}