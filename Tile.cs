using System;
using GB.IO;

namespace GB
{
    public class Tile
    {
        private byte[] byteArr;
        public Shade GetPixel(int x, int y)
        {
            byte pixelValOne = (byte)((byteArr[y*2] >> (7-x)) & 1);
            byte pixelValTwo = (byte)((byteArr[(y*2)+1] >> (7-x)) & 1);
            return (Shade)((pixelValOne << 1) | pixelValTwo);
        }
        public Tile(byte[] tileMemArray)
        {
            byteArr = tileMemArray;
        }
    }
}