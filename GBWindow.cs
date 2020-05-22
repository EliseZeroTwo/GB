using SDL2;
using System;
using System.Threading;

namespace GB
{
    public class GBWindow
    {
        public readonly IntPtr Window = IntPtr.Zero;
        public readonly Thread Thread;
        private Memory internalMemory;

        public void Refresh()
        {
            SDL.SDL_Event e;
            
            byte SCY = internalMemory[0xFF42];
            byte SCX = internalMemory[0xFF43];


            while (SDL.SDL_PollEvent(out e) != 0)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                        break;
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        switch (e.key.keysym.sym)
                        {
                            case SDL.SDL_Keycode.SDLK_q:
                                SDL.SDL_DestroyWindow(Window);            
                                SDL.SDL_Quit();
                                break;
                        }
                        break;
                }
            }
        }
        public GBWindow(ref Memory memory)
        {
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO|SDL.SDL_INIT_AUDIO) != 0)
                throw new Exception($"Failed to initialise SDL2");

            Window = SDL.SDL_CreateWindow("GB:tm:",
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                160,
                144,
                0
            );

            if (Window == IntPtr.Zero)
                throw new Exception($"Failed to initialise window");

            this.internalMemory = memory;
        }
    }
}