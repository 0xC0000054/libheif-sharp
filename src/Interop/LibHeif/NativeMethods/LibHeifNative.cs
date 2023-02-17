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
    internal static partial class LibHeifNative
    {
        private const string DllName = "libheif";
        private const CallingConvention DllCallingConvention = CallingConvention.Cdecl;

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern uint heif_get_version_number();

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool heif_have_decoder_for_format(HeifCompressionFormat format);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool heif_have_encoder_for_format(HeifCompressionFormat format);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern SafeHeifDecodingOptions heif_decoding_options_alloc();

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern void heif_decoding_options_free(IntPtr handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern SafeHeifEncodingOptions heif_encoding_options_alloc();

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern void heif_encoding_options_free(IntPtr handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern SafeHeifNclxColorProfile heif_nclx_color_profile_alloc();

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern void heif_nclx_color_profile_free(IntPtr handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern void heif_depth_representation_info_free(IntPtr handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_init(IntPtr heifInitParams_MustBeZero);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern void heif_deinit();
    }
}
