/*
 * .NET bindings for libheif.
 * Copyright (c) 2020, 2021 Nicholas Hayes
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

namespace LibHeifSharp.Interop
{
#pragma warning disable 0649
    internal struct EncodingOptionsVersion4
    {
        public byte version;

        // version 1 options

        public byte save_alpha_channel;

        // version 2 options

        // Crops heif images with a grid wrapper instead of a 'clap' transform.
        // Results in slightly larger file size.
        // Default: on.
        public byte macOS_compatibility_workaround;

        // version 3 options

        public byte save_two_colr_boxes_when_ICC_and_nclx_available; // default: false

        // version 4 options

        // Set this to the NCLX parameters to be used in the output image or set to NULL
        // when the same parameters as in the input image should be used.
        public IntPtr output_nclx_profile;

        // Some versions of macOS and iOS cannot read files with a NCLX profile
        public byte macOS_compatibility_workaround_no_nclx_profile; // default: true
    }
#pragma warning restore 0649
}
