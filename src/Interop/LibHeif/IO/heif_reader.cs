/*
 * .NET bindings for libheif.
 * Copyright (c) 2020, 2021, 2022 Nicholas Hayes
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
    internal enum heif_reader_grow_status
    {
        size_reached,   // requested size has been reached, we can read until this point
        timeout,        // size has not been reached yet, but it may still grow further
        size_beyond_eof // size has not been reached and never will. The file has grown to its full size
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate long GetPositionDelegate(IntPtr userData);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate int ReadDelegate(IntPtr data, UIntPtr size, IntPtr userData);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate int SeekDelegate(long position, IntPtr userData);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate heif_reader_grow_status WaitForFileSizeDelegate(long targetSize, IntPtr userData);

    [StructLayout(LayoutKind.Sequential)]
    internal struct heif_reader
    {
        // API version supported by this reader
        public int reader_api_version;

        // --- version 1 functions ---
        public IntPtr get_position;

        // The functions read(), and seek() return 0 on success.
        // Generally, libheif will make sure that we do not read past the file size.
        public IntPtr read;

        public IntPtr seek;

        // When calling this function, libheif wants to make sure that it can read the file
        // up to 'target_size'. This is useful when the file is currently downloaded and may
        // grow with time. You may, for example, extract the image sizes even before the actual
        // compressed image data has been completely downloaded.
        //
        // Even if your input files will not grow, you will have to implement at least
        // detection whether the target_size is above the (fixed) file length
        // (in this case, return 'size_beyond_eof').
        public IntPtr wait_for_file_size;
    }
}
