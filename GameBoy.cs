using System.IO;
using System.Threading;

namespace GB
{
    public class GameBoy
    {
        public Cpu Cpu = new Cpu();

        public void Start()
        {
            Cpu.Running = true;
        }

        public void LoadRom(Stream stream)
        {
            Cpu.Memory.Write(stream, 0x3FFF, 0);
        }

        public GameBoy()
        {
            
        }
    }
}