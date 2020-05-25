using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GB.IO
{
    [StructLayout(LayoutKind.Sequential, Pack=0)]
    public struct OAMEntry
    {
        byte YPos;
        byte XPos;
        byte TileNo;
        byte Flags;
    }
    public class OAM
    {
        Memory internalMemory;

        public OAMEntry this[byte b]
        {
            get
            {
                byte[] oamArray = new byte[4];
                internalMemory.Read(oamArray, (ushort)((b * 4) + 0xFE00), 4);
                return Utils.ReadStruct<OAMEntry>(oamArray);
            }
            set
            {
                internalMemory.Write(Utils.MarshalStructToArray<OAMEntry>(value), (ushort)((b * 4) + 0xFE00), 4);
            }
        }

        public OAM(Memory mem)
        {
            internalMemory = mem;
        }
    }
}