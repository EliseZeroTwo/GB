using SDL2;
using System;
using System.IO;
using System.Threading;

namespace GB
{
    public class GameBoy
    {
        public ulong CurrentCycle = 0;
        public Cpu Cpu = new Cpu();
        public IO.LCD Lcd;
        public IO.JOYP Joyp;
        public IO.Timer Timer;
        public IO.SDT SDT;
        private GBWindow Window;
        private uint nextTime = 0;

        private uint TimeLeft()
        {
            uint now = SDL.SDL_GetTicks();
            if (nextTime <= now)
                return 0;
            else
                return nextTime - now;
        }

        public void Run()
        {
            int cpuDelay = 0;
            int hblankDelay = 456;
            uint vblankDelay = 70224;
            uint sdtDelay = 511;
            double vblankTarget = SDL.SDL_GetTicks() + vblankDelay * 1000 / Cpu.ClockSpeed;
            Joyp.UpdateInput();
            
            while(true)
            {
                if (cpuDelay-- == 0)
                    cpuDelay = Cpu.ExecuteInstruction();

                if (hblankDelay-- == 0)
                {
                    // Do hblanks
                    hblankDelay = 456;
                    Lcd.DrawLine();
                }

                if (vblankDelay-- == 0)
                {
                    vblankDelay = 70224;
                    Lcd.DrawTile(0);
                    if (vblankTarget > SDL.SDL_GetTicks())
                        SDL.SDL_Delay((uint) vblankTarget - SDL.SDL_GetTicks());
                    vblankTarget += vblankDelay * 1000 / Cpu.ClockSpeed;
                }

                if (sdtDelay-- == 0)
                {
                    sdtDelay = 511;
                    SDT.Tick();
                }

                Timer.Tick();

                Cpu.IFJoypad = Joyp.UpdateInput();
                CurrentCycle++;
            }
        }

        public void LoadRom(Stream stream)
        {
            byte[] romTemp = new byte[0x7FFF];
            stream.Read(romTemp, 0, stream.Length > 0x7FFF ? 0x7FFF : (int)stream.Length);
            Cpu.Memory.Write(romTemp, 0, 0x7FFF);
        }

        public GameBoy()
        {
            //Window = new GBWindow(ref Cpu.Memory);
            Lcd = new IO.LCD(this.Cpu.Memory);
            Joyp = new IO.JOYP(this.Cpu.Memory);
            Timer = new IO.Timer(this.Cpu.Memory);
            SDT = new IO.SDT(this.Cpu.Memory);
        }
    }
}