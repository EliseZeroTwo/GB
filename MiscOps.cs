using System;
using System.Collections.Generic;

namespace GB
{
    public static class MiscOps
    {

        public static void DI(Cpu cpu, List<string> args)
        {
            cpu.IME = false;
        }

        public static void EI(Cpu cpu, List<string> args)
        {
            cpu.IME = true;
        }
        public static void HALT(Cpu cpu, List<string> args)
        {
            Console.WriteLine("-- CPU HALT! --");

            Environment.Exit(1);
        }
    }
}