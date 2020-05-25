using System;
using System.IO;
using System.Runtime.InteropServices;

namespace GB
{
    public static class Utils
    {
        public static T ReadStruct<T>(byte[] bytes)
        {
            var byteLength = Marshal.SizeOf(typeof (T));
            var pinned = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var stt = (T) Marshal.PtrToStructure(
                pinned.AddrOfPinnedObject(),
                typeof (T));
            pinned.Free();
            return stt;
        }
        
        public static byte[] MarshalStructToArray<T>(T t)
        {
            var sizeOfT = Marshal.SizeOf(typeof(T));
            var ptr = Marshal.AllocHGlobal(sizeOfT);
            Marshal.StructureToPtr(t, ptr, false);
            var bytes = new byte[sizeOfT];
            Marshal.Copy(ptr, bytes, 0, bytes.Length);
            Marshal.FreeHGlobal(ptr);
            return bytes;
        }


    }
}