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
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct heif_depth_representation_info
    {
        public byte version;

        // version 1 fields

        public byte has_z_near;
        public byte has_z_far;
        public byte has_d_min;
        public byte has_d_max;

        public double z_near;
        public double z_far;
        public double d_min;
        public double d_max;

        public HeifDepthRepresentationType depth_representation_type;
        public uint disparity_reference_view;

        public uint depth_nonlinear_representation_model_size;
        public IntPtr depth_nonlinear_representation_model;

        // version 2 fields below
    }
}
