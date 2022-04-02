using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WHS.Infrastructure.Utils
{
    public static class MarshalUtil
    {
        public static readonly int IntPtrSize = 0;
        static MarshalUtil()
        {
            IntPtrSize = Marshal.SizeOf(typeof(IntPtr));
        }

        public static void PtrToStructure<T>(IntPtr ptr, ref T p) where T : struct
        {
            p = (T)Marshal.PtrToStructure(ptr, typeof(T));
        }

        public static T[] PtrToArray<T>(IntPtr ptr)
        {
            if (ptr != IntPtr.Zero)
            {
                int len = 0;
                for (int i = 0; Marshal.ReadIntPtr(IntPtr.Add(ptr, i)) != IntPtr.Zero; i += IntPtrSize)
                    ++len;
                if (len > 0)
                {
                    T[] arr = new T[len];
                    for (int i = 0; i < len; ++i)
                    {
                        arr[i] = (T)Marshal.PtrToStructure(Marshal.ReadIntPtr(IntPtr.Add(ptr, i * IntPtrSize)), typeof(T));
                    }
                    return arr;
                }
            }
            return new T[0];
        }

        public static byte[] StructToBytes(object structObj)
        {
            int size = Marshal.SizeOf(structObj);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structObj, buffer, false);
                byte[] bytes = new byte[size];
                Marshal.Copy(buffer, bytes, 0, size);
                return bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        public static object BytesToStruct(byte[] bytes, Type type)
        {
            int size = Marshal.SizeOf(type);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, buffer, size);
                return Marshal.PtrToStructure(buffer, type);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        public static IntPtr BytesToIntPtr(byte[] bytes)
        {
            int size = bytes.Length;
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, buffer, size);
                return buffer;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }


        public static byte[] PtrToBytes(IntPtr ptr,int bufflength)
        {
            byte[] byData = new byte[bufflength];
            Marshal.Copy(ptr, byData, 0, bufflength);
            return byData;
        }
    }
}
