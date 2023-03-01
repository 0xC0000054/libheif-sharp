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
    /// Represents the HDR mastering display color volume.
    /// </summary>
    /// <seealso cref="HeifImage.MasteringDisplayColourVolume"/>
    public sealed class HeifMasteringDisplayColourVolume
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeifMasteringDisplayColourVolume" /> class.
        /// </summary>
        /// <param name="displayPrimariesX">The display primaries x.</param>
        /// <param name="displayPrimariesY">The display primaries y.</param>
        /// <param name="whitePointX">The white point x.</param>
        /// <param name="whitePointY">The white point y.</param>
        /// <param name="maxDisplayMasteringLuminance">The maximum display mastering luminance.</param>
        /// <param name="minDisplayMasteringLuminance">The minimum display mastering luminance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="displayPrimariesX"/> is <see langword="null"/>.
        ///
        /// -or-
        ///
        /// <paramref name="displayPrimariesY"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="displayPrimariesX"/> count does not equal 3.
        ///
        /// -or-
        ///
        /// <paramref name="displayPrimariesY"/> count does not equal 3.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="whitePointX"/> must be in the range of [0, 65535].
        ///
        /// -or-
        ///
        /// <paramref name="whitePointY"/> must be in the range of [0, 65535].
        ///
        /// -or-
        ///
        /// <paramref name="maxDisplayMasteringLuminance"/> must be in the range of [0, 4294967295].
        ///
        /// -or-
        ///
        /// <paramref name="minDisplayMasteringLuminance"/> must be in the range of [0, 4294967295].
        /// </exception>
        public HeifMasteringDisplayColourVolume(IReadOnlyList<int> displayPrimariesX,
                                                IReadOnlyList<int> displayPrimariesY,
                                                int whitePointX,
                                                int whitePointY,
                                                long maxDisplayMasteringLuminance,
                                                long minDisplayMasteringLuminance)
        {
            Validate.HasSizeEqualTo(displayPrimariesX, nameof(displayPrimariesX), requiredSize: 3);
            Validate.HasSizeEqualTo(displayPrimariesY, nameof(displayPrimariesY), requiredSize: 3);
            Validate.IsInRange(whitePointX, nameof(whitePointX), ushort.MinValue, ushort.MaxValue);
            Validate.IsInRange(whitePointY, nameof(whitePointY), ushort.MinValue, ushort.MaxValue);
            Validate.IsInRange(maxDisplayMasteringLuminance, nameof(maxDisplayMasteringLuminance), uint.MinValue, uint.MaxValue);
            Validate.IsInRange(minDisplayMasteringLuminance, nameof(minDisplayMasteringLuminance), uint.MinValue, uint.MaxValue);

            this.DisplayPrimariesX = displayPrimariesX;
            this.DisplayPrimariesY = displayPrimariesY;
            this.WhitePointX = whitePointX;
            this.WhitePointY = whitePointY;
            this.MaxDisplayMasteringLuminance = maxDisplayMasteringLuminance;
            this.MinDisplayMasteringLuminance = minDisplayMasteringLuminance;
        }

        internal unsafe HeifMasteringDisplayColourVolume(in heif_mastering_display_colour_volume value)
        {
            int[] displayPrimariesX = new int[3]
            {
                value.display_primaries_x[0],
                value.display_primaries_x[1],
                value.display_primaries_x[2]
            };

            int[] displayPrimariesY = new int[3]
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
        /// </value>
        public IReadOnlyList<int> DisplayPrimariesX { get; }

        /// <summary>
        /// The y chromaticity coordinates of the mastering display.
        /// </summary>
        /// <value>
        /// The y chromaticity coordinates of the mastering display.
        /// </value>
        public IReadOnlyList<int> DisplayPrimariesY { get; }

        /// <summary>
        /// The x chromaticity coordinate of the mastering display white point.
        /// </summary>
        /// <value>
        /// The x chromaticity coordinate of the mastering display white point.
        /// </value>
        public int WhitePointX { get; }

        /// <summary>
        /// The y chromaticity coordinate of the mastering display white point.
        /// </summary>
        /// <value>
        /// The y chromaticity coordinate of the mastering display white point.
        /// </value>
        public int WhitePointY { get; }

        /// <summary>
        /// The nominal maximum display luminance of the mastering display.
        /// </summary>
        /// <value>
        /// The nominal maximum display luminance of the mastering display.
        /// </value>
        public long MaxDisplayMasteringLuminance { get; }

        /// <summary>
        /// The nominal minimum display luminance of the mastering display.
        /// </summary>
        /// <value>
        /// The nominal minimum display luminance of the mastering display.
        /// </value>
        public long MinDisplayMasteringLuminance { get; }

        /// <summary>
        /// Decodes this instance.
        /// </summary>
        /// <returns>
        /// A new <see cref="HeifDecodedMasteringDisplayColourVolume"/> instance.
        /// </returns>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        public HeifDecodedMasteringDisplayColourVolume Decode()
        {
            var original = ToNativeStructure();
            heif_decoded_mastering_display_colour_volume decoded;

            unsafe
            {
                var error = LibHeifNative.heif_mastering_display_colour_volume_decode(original, &decoded);
                error.ThrowIfError();
            }

            return new HeifDecodedMasteringDisplayColourVolume(decoded);
        }

        internal unsafe void SetImageMasteringDisplayColourVolume(SafeHeifImage image)
        {
            var native = ToNativeStructure();
            LibHeifNative.heif_image_set_mastering_display_colour_volume(image, &native);
        }

        private unsafe heif_mastering_display_colour_volume ToNativeStructure()
        {
            heif_mastering_display_colour_volume result;

            var displayPrimariesX = this.DisplayPrimariesX;
            var displayPrimariesY = this.DisplayPrimariesY;

            for (int i = 0; i < 3; i++)
            {
                result.display_primaries_x[i] = (ushort)displayPrimariesX[i];
                result.display_primaries_y[i] = (ushort)displayPrimariesY[i];
            }
            result.white_point_x = (ushort)this.WhitePointX;
            result.white_point_y = (ushort)this.WhitePointY;
            result.max_display_mastering_luminance = (uint)this.MaxDisplayMasteringLuminance;
            result.min_display_mastering_luminance = (uint)this.MinDisplayMasteringLuminance;

            return result;
        }
    }
}
