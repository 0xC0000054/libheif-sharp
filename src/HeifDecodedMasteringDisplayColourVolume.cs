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
using System.Collections.Generic;
using LibHeifSharp.Interop;

namespace LibHeifSharp
{
    /// <summary>
    /// Represents the decoded HDR mastering display color volume.
    /// </summary>
    public sealed class HeifDecodedMasteringDisplayColourVolume
    {
        internal unsafe HeifDecodedMasteringDisplayColourVolume(in heif_decoded_mastering_display_colour_volume value)
        {
            float[] displayPrimariesX = new float[3]
            {
                value.display_primaries_x[0],
                value.display_primaries_x[1],
                value.display_primaries_x[2]
            };

            float[] displayPrimariesY = new float[3]
            {
                value.display_primaries_y[0],
                value.display_primaries_y[1],
                value.display_primaries_y[2]
            };

            this.DisplayPrimariesX = Array.AsReadOnly(displayPrimariesX);
            this.DisplayPrimariesY = Array.AsReadOnly(displayPrimariesY);
            this.WhitePointX = value.white_point_x;
            this.WhitePointY = value.white_point_y;
            this.MaxDisplayMasteringLuminance = value.max_display_mastering_luminance;
            this.MinDisplayMasteringLuminance = value.min_display_mastering_luminance;
        }

        /// <summary>
        /// The x chromaticity coordinates of the mastering display.
        /// </summary>
        /// <value>
        /// The x chromaticity coordinates of the mastering display.
        /// A value of 0.0 indicates that the value is undefined.
        /// </value>
        public IReadOnlyList<float> DisplayPrimariesX { get; }

        /// <summary>
        /// The y chromaticity coordinates of the mastering display.
        /// </summary>
        /// <value>
        /// The y chromaticity coordinates of the mastering display.
        /// A value of 0.0 indicates that the value is undefined.
        /// </value>
        public IReadOnlyList<float> DisplayPrimariesY { get; }

        /// <summary>
        /// The x chromaticity coordinate of the mastering display white point.
        /// </summary>
        /// <value>
        /// The x chromaticity coordinate of the mastering display white point.
        /// A value of 0.0 indicates that the value is undefined.
        /// </value>
        public float WhitePointX { get; }

        /// <summary>
        /// The y chromaticity coordinate of the mastering display white point.
        /// </summary>
        /// <value>
        /// The y chromaticity coordinate of the mastering display white point.
        /// A value of 0.0 indicates that the value is undefined.
        /// </value>
        public float WhitePointY { get; }

        /// <summary>
        /// The nominal maximum display luminance of the mastering display.
        /// </summary>
        /// <value>
        /// The nominal maximum display luminance of the mastering display.
        /// A value of 0.0 indicates that the value is undefined.
        /// </value>
        public double MaxDisplayMasteringLuminance { get; }

        /// <summary>
        /// The nominal minimum display luminance of the mastering display.
        /// </summary>
        /// <value>
        /// The nominal minimum display luminance of the mastering display.
        /// A value of 0.0 indicates that the value is undefined.
        /// </value>
        public double MinDisplayMasteringLuminance { get; }
    }
}
