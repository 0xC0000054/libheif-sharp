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
    internal static partial class LibHeifNative
    {
        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_image_add_plane(SafeHeifImage image,
                                                               HeifChannel channel,
                                                               int width,
                                                               int height,
                                                               int bitDepth);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_image_create(int width,
                                                            int height,
                                                            HeifColorspace colorspace,
                                                            HeifChroma chroma,
                                                            out SafeHeifImage image);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern void heif_image_release(IntPtr handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern HeifColorspace heif_image_get_colorspace(SafeHeifImage handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern HeifChroma heif_image_get_chroma_format(SafeHeifImage handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern int heif_image_get_width(SafeHeifImage handle, HeifChannel channel);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern int heif_image_get_height(SafeHeifImage handle, HeifChannel channel);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern IntPtr heif_image_get_plane(SafeHeifImage handle, HeifChannel channel, out int planeStride);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern int heif_image_get_primary_width(SafeHeifImage handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern int heif_image_get_primary_height(SafeHeifImage handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool heif_image_has_channel(SafeHeifImage handle, HeifChannel channel);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool heif_image_is_premultiplied_alpha(SafeHeifImage handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern void heif_image_set_premultiplied_alpha(SafeHeifImage handle, [MarshalAs(UnmanagedType.Bool)] bool value);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_color_profile_type heif_image_get_color_profile_type(SafeHeifImage handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern UIntPtr heif_image_get_raw_color_profile_size(SafeHeifImage handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe heif_error heif_image_get_raw_color_profile(SafeHeifImage image, byte* data);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_image_get_nclx_color_profile(SafeHeifImage image,
                                                                                   out SafeHeifNclxColorProfile profile);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe heif_error heif_image_set_raw_color_profile(SafeHeifImage handle,
                                                                                  [MarshalAs(UnmanagedType.LPStr)] string profileFourCC,
                                                                                  byte* profileData,
                                                                                  UIntPtr profileSize);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_image_set_nclx_color_profile(SafeHeifImage handle,
                                                                                   SafeHeifNclxColorProfile nclxColorProfile);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_image_scale_image(SafeHeifImage handle,
                                                                 out SafeHeifImage scaledImage,
                                                                 int newWidth,
                                                                 int newHeight,
                                                                 IntPtr options_MustBeZero);
    }
}
