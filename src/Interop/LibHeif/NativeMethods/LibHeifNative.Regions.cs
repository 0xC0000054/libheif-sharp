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
        internal static extern void heif_region_item_release(IntPtr handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern void heif_region_item_get_reference_size(SafeHeifRegionItem handle,
                                                                        out uint width,
                                                                        out uint height);
        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern int heif_region_item_get_number_of_regions(SafeHeifRegionItem handle);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern int heif_region_item_get_list_of_regions(SafeHeifRegionItem handle,
                                                                        [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] heif_region[] regions,
                                                                        int count);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern void heif_region_release_many([In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] heif_region[] regions,
                                                             int count);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern RegionGeometryType heif_region_get_type(heif_region region);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_region_get_point(heif_region region,
                                                                out int x,
                                                                out int y);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_region_get_point_transformed(heif_region region,
                                                                            out double x,
                                                                            out double y,
                                                                            HeifItemId imageId);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_region_get_rectangle(heif_region region,
                                                                    out int x,
                                                                    out int y,
                                                                    out uint width,
                                                                    out uint height);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_region_get_rectangle_transformed(heif_region region,
                                                                                out double x,
                                                                                out double y,
                                                                                out double width,
                                                                                out double height,
                                                                                HeifItemId imageId);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_region_get_ellipse(heif_region region,
                                                                  out int x,
                                                                  out int y,
                                                                  out uint radiusX,
                                                                  out uint radiusY);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_region_get_ellipse_transformed(heif_region region,
                                                                              out double x,
                                                                              out double y,
                                                                              out double radiusX,
                                                                              out double radiusY,
                                                                              HeifItemId imageId);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern int heif_region_get_polygon_num_points(heif_region region);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe heif_error heif_region_get_polygon_points(heif_region region,
                                                                                int* points);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe heif_error heif_region_get_polygon_points_transformed(heif_region region,
                                                                                            double* points,
                                                                                            HeifItemId imageId);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern int heif_region_get_polyline_num_points(heif_region region);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe heif_error heif_region_get_polyline_points(heif_region region,
                                                                                 int* points);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe heif_error heif_region_get_polyline_points_transformed(heif_region region,
                                                                                             double* points,
                                                                                             HeifItemId imageId);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_region_item_add_region_point(SafeHeifRegionItem handle,
                                                                            int x,
                                                                            int y,
                                                                            IntPtr outRegion);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_region_item_add_region_rectangle(SafeHeifRegionItem handle,
                                                                                int x,
                                                                                int y,
                                                                                uint width,
                                                                                uint height,
                                                                                IntPtr outRegion);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern heif_error heif_region_item_add_region_ellipse(SafeHeifRegionItem handle,
                                                                              int x,
                                                                              int y,
                                                                              uint radiusX,
                                                                              uint radiusY,
                                                                              IntPtr outRegion);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe heif_error heif_region_item_add_region_polygon(SafeHeifRegionItem handle,
                                                                                     int* points,
                                                                                     int pointCount,
                                                                                     IntPtr outRegion);

        [DllImport(DllName, CallingConvention = DllCallingConvention)]
        internal static extern unsafe heif_error heif_region_item_add_region_polyline(SafeHeifRegionItem handle,
                                                                                      int* points,
                                                                                      int pointCount,
                                                                                      IntPtr outRegion);
    }
}
