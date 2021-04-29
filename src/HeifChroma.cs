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
    /// The LibHeif image pixel format
    /// </summary>
    public enum HeifChroma
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        Undefined = 99,

        /// <summary>
        /// Monochrome.
        /// </summary>
        Monochrome = 0,

        /// <summary>
        /// YUV 4:2:0.
        /// </summary>
        Yuv420 = 1,

        /// <summary>
        /// YUV 4:2:2.
        /// </summary>
        Yuv422 = 2,

        /// <summary>
        /// YUV 4:4:4.
        /// </summary>
        Yuv444 = 3,

        /// <summary>
        /// An interleaved 24 bits-per-pixel RGB format. Each channel is allocated 8 bits.
        /// </summary>
        InterleavedRgb24 = 10,

        /// <summary>
        /// An interleaved 32 bits-per-pixel RGBA format. Each channel is allocated 8 bits.
        /// </summary>
        InterleavedRgba32 = 11,

        /// <summary>
        /// An interleaved 48 bits-per-pixel RGB format using big-endian byte order. Each channel is allocated 16 bits.
        /// </summary>
        InterleavedRgb48BE = 12,

        /// <summary>
        /// An interleaved 64 bits-per-pixel RGBA format using big-endian byte order. Each channel is allocated 16 bits.
        /// </summary>
        InterleavedRgba64BE = 13,

        /// <summary>
        /// An interleaved 48 bits-per-pixel RGB format using little-endian byte order. Each channel is allocated 16 bits.
        /// </summary>
        InterleavedRgb48LE = 14,

        /// <summary>
        /// An interleaved 64 bits-per-pixel RGBA format using little-endian byte order. Each channel is allocated 16 bits.
        /// </summary>
        InterleavedRgba64LE = 15
    }
}
