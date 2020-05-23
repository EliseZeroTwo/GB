using SDL2;


namespace GB.IO
{
    public class JOYP
    {
        private bool UpPressed = false;
        private bool DownPressed = false;
        private bool LeftPressed = false;
        private bool RightPressed = false;
        private bool StartPressed = false;
        private bool SelectPressed = false;
        private bool APressed = false;
        private bool BPressed = false;

        private bool UseDpad = false;
        private bool UseButtons = false;

        private void UpdateInput()
        {
            while (SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0)
            {
                switch (e.type)
                {
                    case SDL.SDL_EventType.SDL_KEYUP:
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                        switch (e.key.keysym.sym)
                        {
                            case SDL.SDL_Keycode.SDLK_w:
                                UpPressed = e.type == SDL.SDL_EventType.SDL_KEYDOWN;
                                break;
                            case SDL.SDL_Keycode.SDLK_a:
                                LeftPressed = e.type == SDL.SDL_EventType.SDL_KEYDOWN;
                                break;
                            case SDL.SDL_Keycode.SDLK_s:
                                DownPressed = e.type == SDL.SDL_EventType.SDL_KEYDOWN;
                                break;
                            case SDL.SDL_Keycode.SDLK_d:
                                RightPressed = e.type == SDL.SDL_EventType.SDL_KEYDOWN;
                                break;
                            case SDL.SDL_Keycode.SDLK_q:
                                BPressed = e.type == SDL.SDL_EventType.SDL_KEYDOWN;
                                break;
                            case SDL.SDL_Keycode.SDLK_e:
                                APressed = e.type == SDL.SDL_EventType.SDL_KEYDOWN;
                                break;
                            case SDL.SDL_Keycode.SDLK_z:
                                SelectPressed = e.type == SDL.SDL_EventType.SDL_KEYDOWN;
                                break;
                            case SDL.SDL_Keycode.SDLK_x:
                                StartPressed = e.type == SDL.SDL_EventType.SDL_KEYDOWN;
                                break;
                        }
                        break;
                }
            }
        }

        public byte Read(bool readInput=true)
        {
            byte b = 0b1111;

            if (readInput)
                UpdateInput();

            if ((DownPressed && UseDpad) || (StartPressed && UseButtons))
                b ^= (1 << 3);
            
            if ((UpPressed && UseDpad) || (SelectPressed && UseButtons))
                b ^= (1 << 2);

            if ((LeftPressed && UseDpad) || (BPressed && UseButtons))
                b ^= (1 << 1);
            
            if ((RightPressed && UseDpad) || (APressed && UseButtons))
                b ^= (1 << 0);

            if (!UseDpad)
                b |= (1 << 4);
            
            if (!UseButtons)
                b |= (1 << 5);
            
            return b;
            
        }

        public void Write(byte b)
        {
            if ((b & (1 << 5)) != 0)
                UseButtons = false;
            else
                UseButtons = true;
            
            if ((b & (1 << 4)) != 0)
                UseDpad = false;
            else
                UseDpad = true;

        }
    }
}