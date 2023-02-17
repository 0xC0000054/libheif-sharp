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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace LibHeifSharp.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct OutputStringArray
    {
        // A pointer to an array of const char* strings.
        private readonly IntPtr arrayPointer;

        /// <summary>
        /// Converts the string array to a read-only collection of strings.
        /// </summary>
        /// <returns>A read-only collection containing the string array values.</returns>
        public unsafe ReadOnlyCollection<string> ToReadOnlyCollection()
        {
            var items = new List<string>();

            if (this.arrayPointer != IntPtr.Zero)
            {
                var stringMemoryAdddress = (IntPtr*)this.arrayPointer;

                while (*stringMemoryAdddress != IntPtr.Zero)
                {
                    items.Add(Marshal.PtrToStringAnsi(*stringMemoryAdddress));

                    stringMemoryAdddress++;
                }
            }

            return items.AsReadOnly();
        }
    }
}
