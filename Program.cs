using System;
using System.IO;

namespace GB
{
    static class Program
    {
        public static GameBoy gameBoy;
        public static void DumpStuffException()
        {
            Console.WriteLine($"PC: 0x{gameBoy.Cpu.Registers.PC:x}\nSP: 0x{gameBoy.Cpu.Registers.SP:x}");
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            gameBoy = new GameBoy();
            using (FileStream fs = File.OpenRead(args[0]))
                gameBoy.LoadRom(fs);
            

            gameBoy.Start();


            using (FileStream fs = File.Create(args[1]))
                gameBoy.Cpu.Memory.DumpMemory(fs);
        
        }
    }
}
