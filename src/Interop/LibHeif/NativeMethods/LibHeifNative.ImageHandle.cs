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
        internal static extern void heif_image_handle_release(IntPtr handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern int heif_image_handle_get_width(SafeHeifImageHandle handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern int heif_image_handle_get_height(SafeHeifImageHandle handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern int heif_image_handle_get_luma_bits_per_pixel(SafeHeifImageHandle handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool heif_image_handle_has_alpha_channel(SafeHeifImageHandle handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool heif_image_handle_is_premultiplied_alpha(SafeHeifImageHandle handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_decode_image(SafeHeifImageHandle inHandle,
                                                            out SafeHeifImage outImage,
                                                            HeifColorspace colorspace,
                                                            HeifChroma chroma,
                                                            SafeHeifDecodingOptions options);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_decode_image(SafeHeifImageHandle inHandle,
                                                            out SafeHeifImage outImage,
                                                            HeifColorspace colorspace,
                                                            HeifChroma chroma,
                                                            IntPtr options_MustBeZero);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool heif_image_handle_is_primary_image(SafeHeifImageHandle handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_color_profile_type heif_image_handle_get_color_profile_type(SafeHeifImageHandle handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern UIntPtr heif_image_handle_get_raw_color_profile_size(SafeHeifImageHandle handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe heif_error heif_image_handle_get_raw_color_profile(SafeHeifImageHandle handle, byte* data);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe heif_error heif_image_handle_get_nclx_color_profile(SafeHeifImageHandle handle,
                                                                                          out SafeHeifNclxColorProfile profile);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern int heif_image_handle_get_number_of_metadata_blocks(SafeHeifImageHandle handle,
                                                                                   [MarshalAs(UnmanagedType.LPStr)] string type);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe int heif_image_handle_get_list_of_metadata_block_IDs(SafeHeifImageHandle handle,
                                                                                           [MarshalAs(UnmanagedType.LPStr)] string type,
                                                                                           HeifItemId* idArrayPtr,
                                                                                           int idArrayCount);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern LibHeifOwnedString heif_image_handle_get_metadata_content_type(SafeHeifImageHandle handle, HeifItemId id);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern UIntPtr heif_image_handle_get_metadata_size(SafeHeifImageHandle handle, HeifItemId id);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe heif_error heif_image_handle_get_metadata(SafeHeifImageHandle handle, HeifItemId id, byte* buffer);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern int heif_image_handle_get_number_of_thumbnails(SafeHeifImageHandle handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe int heif_image_handle_get_list_of_thumbnail_IDs(SafeHeifImageHandle handle,
                                                                                      HeifItemId* idArrayPtr,
                                                                                      int idArrayCount);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_image_handle_get_thumbnail(SafeHeifImageHandle handle,
                                                                          HeifItemId id,
                                                                          out SafeHeifImageHandle thumbnailImageHandle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool heif_image_handle_has_depth_image(SafeHeifImageHandle handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern int heif_image_handle_get_number_of_depth_images(SafeHeifImageHandle handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe int heif_image_handle_get_list_of_depth_image_IDs(SafeHeifImageHandle handle,
                                                                                        HeifItemId* idArrayPtr,
                                                                                        int idArrayCount);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_image_handle_get_depth_image_handle(SafeHeifImageHandle handle,
                                                                                   HeifItemId id,
                                                                                   out SafeHeifImageHandle depthImageHandle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool heif_image_handle_get_depth_image_representation_info(SafeHeifImageHandle handle,
                                                                                          HeifItemId id,
                                                                                          out SafeDepthRepresentationInfo depthInfoHandle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern int heif_image_handle_get_number_of_auxiliary_images(SafeHeifImageHandle handle,
                                                                                    heif_auxiliary_image_filter filter);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe int heif_image_handle_get_list_of_auxiliary_image_IDs(SafeHeifImageHandle handle,
                                                                                            heif_auxiliary_image_filter filter,
                                                                                            HeifItemId* idArrayPtr,
                                                                                            int idArrayCount);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_image_handle_get_auxiliary_image_handle(SafeHeifImageHandle handle,
                                                                                       HeifItemId id,
                                                                                       out SafeHeifImageHandle auxiliaryImageHandle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe heif_error heif_image_handle_get_auxiliary_type(SafeHeifImageHandle handle,
                                                                                      out AuxiliaryType outType);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe void heif_image_handle_free_auxiliary_types(SafeHeifImageHandle handle,
                                                                                  ref AuxiliaryType outType);
    }
}
