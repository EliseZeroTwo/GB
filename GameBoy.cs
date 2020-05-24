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
            double vblankTarget = SDL.SDL_GetTicks() + vblankDelay * 1000 / Cpu.ClockSpeed;

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
                    if (vblankTarget > SDL.SDL_GetTicks())
                        SDL.SDL_Delay((uint) vblankTarget - SDL.SDL_GetTicks());
                    vblankTarget += vblankDelay * 1000 / Cpu.ClockSpeed;
                }

                Cpu.IFJoypad = Joyp.UpdateInput();
                CurrentCycle++;
            }
        }

        public void LoadRom(Stream stream)
        {
            Cpu.Memory.Write(stream, 0x3FFF, 0);
        }

        public GameBoy()
        {
            Window = new GBWindow(ref Cpu.Memory);
            Lcd = new IO.LCD(this.Cpu.Memory);
            Joyp = new IO.JOYP(this.Cpu.Memory);

        }
    }
}