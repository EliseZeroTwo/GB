using System;
using System.IO;

namespace GB
{
    static class Program
    {
        public static GameBoy gameBoy;
        public static void DumpStuffException()
        {
            Console.WriteLine($"\n\n--- REG DUMP ---\nA: 0x{gameBoy.Cpu.Registers.A:x}\nB: 0x{gameBoy.Cpu.Registers.B:x}\nC: 0x{gameBoy.Cpu.Registers.C:x}\nD: 0x{gameBoy.Cpu.Registers.D:x}\nE: 0x{gameBoy.Cpu.Registers.E:x}\nH: 0x{gameBoy.Cpu.Registers.H:x}\nL: 0x{gameBoy.Cpu.Registers.L:x}\n----\nAF: 0x{gameBoy.Cpu.Registers.AF:x}\nBC: 0x{gameBoy.Cpu.Registers.BC:x}\nDE: 0x{gameBoy.Cpu.Registers.DE:x}\nHL: 0x{gameBoy.Cpu.Registers.HL:x}\n----\nPC: 0x{gameBoy.Cpu.Registers.PC:x}\nSP: 0x{gameBoy.Cpu.Registers.SP:x}\nZero: {gameBoy.Cpu.ZeroFlag.Value}\nSubtract: {gameBoy.Cpu.SubtractFlag.Value}\nHalf Carry: {gameBoy.Cpu.HalfCarryFlag.Value}\nCarry: {gameBoy.Cpu.CarryFlag.Value}\n----\nLCD Mode: {gameBoy.Lcd.Mode.ToString()}\n");
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("-- GBEmu by Elise --");
            gameBoy = new GameBoy();
            foreach (string s in args)
            {
                if (s == "--debug")
                {
                    gameBoy.Cpu.SingleStep = true;
                }
            }
            GBWindow window = new GBWindow(ref gameBoy.Cpu.Memory);
            using (FileStream fs = File.OpenRead(args[0]))
                gameBoy.LoadRom(fs);
            
            gameBoy.Run();

            using (FileStream fs = File.Create(args[1]))
                gameBoy.Cpu.Memory.DumpMemory(fs);
        }
    }
}
