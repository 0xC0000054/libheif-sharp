/*
 * .NET bindings for libheif.
 * Copyright (c) 2020, 2021, 2022, 2023 Nicholas Hayes
 *
 * Portions Copyright (c) 2017 struktur AG, Dirk Farin <farin@struktur.de>
 *
 * This file is part of libheif-sharp.
 *
 * libheif-sharp is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version.
 *
 * libheif-sharp is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with libheif-sharp.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Runtime.InteropServices;

namespace LibHeifSharp.Interop
{
    internal sealed class LibHeifOwnedStringArrayMarshaler : ICustomMarshaler
    {
        private static readonly LibHeifOwnedStringArrayMarshaler instance = new LibHeifOwnedStringArrayMarshaler();

        private LibHeifOwnedStringArrayMarshaler()
        {
        }

        public static ICustomMarshaler GetInstance(string cookie)
        {
            return instance;
        }

        public void CleanUpManagedData(object ManagedObj)
        {
            throw new NotImplementedException();
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            // Nothing to do, the native pointer is owned by LibHeif.
        }

        public int GetNativeDataSize()
        {
            throw new NotImplementedException();
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            throw new NotImplementedException();
        }

        public unsafe object MarshalNativeToManaged(IntPtr pNativeData)
        {
            string[] items = Array.Empty<string>();

            if (pNativeData != IntPtr.Zero)
            {
                var stringMemoryAdddress = (IntPtr*)pNativeData;

                int count = 0;

                while (*stringMemoryAdddress != IntPtr.Zero)
                {
                    if (count == items.Length)
                    {
                        if (count == 0)
                        {
                            items = new string[4];
                        }
                        else
                        {
                            int newCount = (int)Math.Min((long)count * 2, int.MaxValue);
                            Array.Resize(ref items, newCount);
                        }
                    }

                    items[count++] = Marshal.PtrToStringAnsi(*stringMemoryAdddress);
                    stringMemoryAdddress++;
                }

                if (items.Length > count)
                {
                    Array.Resize(ref items, count);
                }
            }

            return items;
        }
    }
}
