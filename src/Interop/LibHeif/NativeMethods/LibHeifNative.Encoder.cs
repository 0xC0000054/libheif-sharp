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
        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern void heif_encoder_release(IntPtr handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_encoder_parameter_list heif_encoder_list_parameters(SafeHeifEncoder encoder);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe LibHeifOwnedString heif_encoder_parameter_get_name(heif_encoder_parameter parameter);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe heif_encoder_parameter_type heif_encoder_parameter_get_type(heif_encoder_parameter parameter);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe heif_error heif_encoder_parameter_get_valid_integer_range(heif_encoder_parameter parameter,
                                                                                                [MarshalAs(UnmanagedType.Bool)] out bool haveMinimumMaximum,
                                                                                                ref int minimum,
                                                                                                ref int maximum);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe heif_error heif_encoder_parameter_get_valid_integer_values(heif_encoder_parameter parameter,
                                                                                                 [MarshalAs(UnmanagedType.Bool)] out bool haveMinimum,
                                                                                                 [MarshalAs(UnmanagedType.Bool)] out bool haveMaximum,
                                                                                                 ref int minimum,
                                                                                                 ref int maximum,
                                                                                                 out int numValidValues,
                                                                                                 out IntPtr validValuesArray);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe heif_error heif_encoder_parameter_get_valid_string_values(heif_encoder_parameter parameter,
                                                                                                out OutputStringArray array);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_encoder_set_lossy_quality(SafeHeifEncoder encoder, int quality);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_encoder_set_lossless(SafeHeifEncoder encoder,
                                                                    [MarshalAs(UnmanagedType.Bool)] bool lossless);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_encoder_set_parameter_boolean(SafeHeifEncoder encoder,
                                                                             [MarshalAs(UnmanagedType.LPStr)] string name,
                                                                             [MarshalAs(UnmanagedType.Bool)] bool value);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_encoder_set_parameter_integer(SafeHeifEncoder encoder,
                                                                            [MarshalAs(UnmanagedType.LPStr)] string name,
                                                                            int value);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_encoder_set_parameter_string(SafeHeifEncoder encoder,
                                                                            [MarshalAs(UnmanagedType.LPStr)] string name,
                                                                            [MarshalAs(UnmanagedType.LPStr)] string value);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_encoder_get_parameter_boolean(SafeHeifEncoder encoder,
                                                                            [MarshalAs(UnmanagedType.LPStr)] string name,
                                                                            [MarshalAs(UnmanagedType.Bool)] out bool value);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_encoder_get_parameter_integer(SafeHeifEncoder encoder,
                                                                            [MarshalAs(UnmanagedType.LPStr)] string name,
                                                                            out int value);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe heif_error heif_encoder_get_parameter_string(SafeHeifEncoder encoder,
                                                                                   [MarshalAs(UnmanagedType.LPStr)] string name,
                                                                                   byte* ptr,
                                                                                   int length);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool heif_encoder_has_default(SafeHeifEncoder encoder,
                                                             [MarshalAs(UnmanagedType.LPStr)] string name);
    }
}
