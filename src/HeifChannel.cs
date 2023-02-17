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
    /// The LibHeif image channels
    /// </summary>
    public enum HeifChannel
    {
        /// <summary>
        /// The Y channel in YCbCr.
        /// </summary>
        Y = 0,

        /// <summary>
        /// The Cb channel in YCbCr.
        /// </summary>
        Cb = 1,

        /// <summary>
        /// The Cr channel in YCbCr.
        /// </summary>
        Cr = 2,

        /// <summary>
        /// The red channel in RGB and RGBA.
        /// </summary>
        R = 3,

        /// <summary>
        /// The green channel in RGB and RGBA.
        /// </summary>
        G = 4,

        /// <summary>
        /// The blue channel in RGB and RGBA.
        /// </summary>
        B = 5,

        /// <summary>
        /// The alpha channel.
        /// </summary>
        Alpha = 6,

        /// <summary>
        /// The image uses interleaved channels
        /// </summary>
        Interleaved = 10
    }
}
