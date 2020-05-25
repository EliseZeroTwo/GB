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
                return Utils.ReadStruct<OAMEntry>(internalMemory.Read((ushort)((b * 4) + 0xFE00), 4));
            }
            set
            {
                internalMemory.Write(Utils.MarshalStructToArray<OAMEntry>(value), (ushort)((b * 4) + 0xFE00));
            }
        }

        public OAM(Memory mem)
        {
            internalMemory = mem;
        }
    }
}