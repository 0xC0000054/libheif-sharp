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

namespace LibHeifSharp.Interop
{
#pragma warning disable 0649
    internal struct DecodeOptionsVersion3
    {
        public byte version;

        // version 1 options

        // Ignore geometric transformations like cropping, rotation, mirroring.
        // Default: false (do not ignore).
        public byte ignore_transformations;

        public IntPtr start_progress;

        public IntPtr on_progress;

        public IntPtr end_progress;

        public IntPtr progress_user_data;

        // version 2 options

        public byte convert_hdr_to_8bit;

        // version 3 options

        // When enabled, an error is returned for invalid input. Otherwise, it will try its best and
        // add decoding warnings to the decoded heif_image. Default is non-strict.
        public byte strict_decoding;
    }
#pragma warning restore 0649
}
