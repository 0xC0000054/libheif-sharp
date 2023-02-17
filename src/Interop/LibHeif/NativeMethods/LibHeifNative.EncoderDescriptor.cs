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

using System.Runtime.InteropServices;

namespace LibHeifSharp.Interop
{
    internal static partial class LibHeifNative
    {
        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern LibHeifOwnedString heif_encoder_descriptor_get_name(heif_encoder_descriptor descriptor);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern LibHeifOwnedString heif_encoder_descriptor_get_id_name(heif_encoder_descriptor descriptor);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern HeifCompressionFormat heif_encoder_descriptor_get_compression_format(heif_encoder_descriptor descriptor);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool heif_encoder_descriptor_supports_lossy_compression(heif_encoder_descriptor descriptor);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool heif_encoder_descriptor_supports_lossless_compression(heif_encoder_descriptor descriptor);
    }
}
