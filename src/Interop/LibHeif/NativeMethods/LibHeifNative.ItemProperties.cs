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
        internal static extern unsafe int heif_item_get_properties_of_type(SafeHeifContext context,
                                                                           HeifItemId id,
                                                                           heif_item_property_type type,
                                                                           HeifPropertyId* outProperties,
                                                                           int count);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe int heif_item_get_transformation_properties(SafeHeifContext context,
                                                                                  HeifItemId id,
                                                                                  HeifPropertyId* outProperties,
                                                                                  int count);
        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_item_property_type heif_item_get_property_type(SafeHeifContext context,
                                                                                   HeifItemId id,
                                                                                   HeifPropertyId propertyId);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_item_get_property_user_description(SafeHeifContext context,
                                                                                  HeifItemId itemId,
                                                                                  HeifPropertyId propertyId,
                                                                                  out SafeHeifPropertyUserDescription description);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe heif_error heif_item_add_property_user_description(SafeHeifContext context,
                                                                                         HeifItemId itemId,
                                                                                         heif_property_user_description* description,
                                                                                         IntPtr outPropertyId);
        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern void heif_property_user_description_release(IntPtr handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern MirrorDirection heif_item_get_property_transform_mirror(SafeHeifContext context,
                                                                                       HeifItemId id,
                                                                                       HeifPropertyId propertyId);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern int heif_item_get_property_transform_rotation_ccw(SafeHeifContext context,
                                                                                 HeifItemId id,
                                                                                 HeifPropertyId propertyId);
        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern void heif_item_get_property_transform_crop_borders(SafeHeifContext context,
                                                                                  HeifItemId id,
                                                                                  HeifPropertyId propertyId,
                                                                                  int imageWidth,
                                                                                  int imageHeight,
                                                                                  out int left,
                                                                                  out int top,
                                                                                  out int right,
                                                                                  out int bottom);
    }
}
