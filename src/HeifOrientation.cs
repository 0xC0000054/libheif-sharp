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

namespace LibHeifSharp
{
    /// <summary>
    /// Specifies how the decoder should transform the image before it is displayed.
    /// </summary>
    /// <remarks>
    /// These values match the EXIF Orientation tag.
    /// </remarks>
    /// <seealso cref="HeifEncodingOptions.ImageOrientation"/>
    public enum HeifOrientation
    {
        /// <summary>
        /// The image will not be rotated or flipped.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// The image will be flipped horizontally.
        /// </summary>
        FlipHorizontally = 2,

        /// <summary>
        /// The image will be rotated 180 degrees.
        /// </summary>
        Rotate180 = 3,

        /// <summary>
        /// The image will be flipped vertically
        /// </summary>
        FlipVertically = 4,

        /// <summary>
        /// The image will be rotated 90 degrees clockwise followed by a horizontal flip.
        /// </summary>
        Rotate90ClockwiseThenFlipHorizontally = 5,

        /// <summary>
        /// The image will be rotated 90 degrees clockwise.
        /// </summary>
        Rotate90Clockwise = 6,

        /// <summary>
        /// The image will be rotated 90 degrees clockwise followed by a vertical flip.
        /// </summary>
        Rotate90ClockwiseThenFlipVertically = 7,

        /// <summary>
        /// The image will be rotated 270 degrees clockwise.
        /// </summary>
        Rotate270Clockwise = 8
    }
}
