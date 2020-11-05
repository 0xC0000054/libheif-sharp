/*
 * .NET bindings for libheif.
 * Copyright (c) 2020 Nicholas Hayes
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
        internal static extern SafeHeifContext heif_context_alloc();

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern void heif_context_free(IntPtr handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_context_read_from_reader(SafeHeifContext context,
                                                                        SafeHandle reader,
                                                                        IntPtr userData,
                                                                        IntPtr options);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern int heif_context_get_number_of_top_level_images(SafeHeifContext context);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe int heif_context_get_list_of_top_level_image_IDs(SafeHeifContext context, HeifItemId* idArray, int count);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_context_get_primary_image_handle(SafeHeifContext context,
                                                                                out SafeHeifImageHandle primaryImageHandle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_context_get_image_handle(SafeHeifContext context,
                                                                        HeifItemId id,
                                                                        out SafeHeifImageHandle imageHandle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_context_encode_image(SafeHeifContext context,
                                                                    SafeHeifImage image,
                                                                    SafeHeifEncoder encoder,
                                                                    SafeHeifEncodingOptions options,
                                                                    out SafeHeifImageHandle imageHandle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_context_encode_image(SafeHeifContext context,
                                                                    SafeHeifImage image,
                                                                    SafeHeifEncoder encoder,
                                                                    SafeHeifEncodingOptions options,
                                                                    IntPtr outImageHandle_MustBeZero);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_context_encode_image(SafeHeifContext context,
                                                                   SafeHeifImage image,
                                                                   SafeHeifEncoder encoder,
                                                                   IntPtr options_MustBeZero,
                                                                   out SafeHeifImageHandle imageHandle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_context_encode_image(SafeHeifContext context,
                                                                    SafeHeifImage image,
                                                                    SafeHeifEncoder encoder,
                                                                    IntPtr options_MustBeZero,
                                                                    IntPtr outImageHandle_MustBeZero);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_context_get_encoder_for_format(SafeHeifContext context,
                                                                              HeifCompressionFormat format,
                                                                              out SafeHeifEncoder encoder);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_context_set_primary_image(SafeHeifContext context, SafeHeifImageHandle handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_context_write(SafeHeifContext context,
                                                             SafeHandle writer,
                                                             IntPtr userData);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe heif_error heif_context_add_exif_metadata(SafeHeifContext context,
                                                                                SafeHeifImageHandle handle,
                                                                                byte* data,
                                                                                int dataSize);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe heif_error heif_context_add_XMP_metadata(SafeHeifContext context,
                                                                               SafeHeifImageHandle handle,
                                                                               byte* data,
                                                                               int dataSize);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe heif_error heif_context_add_generic_metadata(SafeHeifContext context,
                                                                                   SafeHeifImageHandle handle,
                                                                                   byte* data,
                                                                                   int dataSize,
                                                                                   [MarshalAs(UnmanagedType.LPStr)] string type,
                                                                                   [MarshalAs(UnmanagedType.LPStr)] string contentType);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_context_encode_thumbnail(SafeHeifContext context,
                                                                        SafeHeifImage thumbnail,
                                                                        SafeHeifImageHandle parentImageHandle,
                                                                        SafeHeifEncoder encoder,
                                                                        SafeHeifEncodingOptions options,
                                                                        int boundingBoxSize,
                                                                        IntPtr outImageHandle_MustBeZero);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_context_encode_thumbnail(SafeHeifContext context,
                                                                        SafeHeifImage thumbnail,
                                                                        SafeHeifImageHandle parentImageHandle,
                                                                        SafeHeifEncoder encoder,
                                                                        IntPtr options_MustBeZero,
                                                                        int boundingBoxSize,
                                                                        IntPtr outImageHandle_MustBeZero);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe int heif_context_get_encoder_descriptors(SafeHeifContext context,
                                                                               HeifCompressionFormat compressionFormat,
                                                                               [MarshalAs(UnmanagedType.LPStr)] string nameFilter,
                                                                               heif_encoder_descriptor* items,
                                                                               int count);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_context_get_encoder(SafeHeifContext context,
                                                                   heif_encoder_descriptor descriptor,
                                                                   out SafeHeifEncoder encoder);

    }
}
