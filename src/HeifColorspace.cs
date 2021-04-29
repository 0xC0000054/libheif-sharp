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

namespace LibHeifSharp
{
    /// <summary>
    /// The color space of the image
    /// </summary>
    public enum HeifColorspace
    {
        /// <summary>
        /// The color space is not defined.
        /// </summary>
        Undefined = 99,

        /// <summary>
        /// The color space is YCbCr.
        /// </summary>
        YCbCr = 0,

        /// <summary>
        /// The color space is Rgb.
        /// </summary>
        Rgb = 1,

        /// <summary>
        /// The color space is monochrome.
        /// </summary>
        Monochrome = 2
    }
}
