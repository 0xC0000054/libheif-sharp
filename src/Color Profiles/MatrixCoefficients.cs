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

namespace LibHeifSharp
{
    // These values are from the ITU-T H.273 (2016) specification.
    // https://www.itu.int/rec/T-REC-H.273-201612-I/en

    /// <summary>
    /// The NCLX matrix coefficients
    /// </summary>
    public enum MatrixCoefficients
    {
        /// <summary>
        /// Identity matrix
        /// </summary>
        Identity = 0,

        /// <summary>
        /// BT.709
        /// </summary>
        BT709 = 1,

        /// <summary>
        /// Unspecified
        /// </summary>
        Unspecified = 2,

        /// <summary>
        /// For future use
        /// </summary>
        Reserved3 = 3,

        /// <summary>
        /// US FCC 73.628
        /// </summary>
        FCC = 4,

        /// <summary>
        /// BT.470 System B, G (historical)
        /// </summary>
        BT470BG = 5,

        /// <summary>
        /// BT.601
        /// </summary>
        BT601 = 6,

        /// <summary>
        /// SMPTE 240 M
        /// </summary>
        Smpte240 = 7,

        /// <summary>
        /// YCgCo
        /// </summary>
        YCgCo = 8,

        /// <summary>
        /// BT.2020-2 non-constant luminance, BT.2100-0 YCbCr
        /// </summary>
        BT2020NCL = 9,

        /// <summary>
        /// BT.2020-2 constant luminance
        /// </summary>
        BT2020CL = 10,

        /// <summary>
        /// SMPTE ST 2085
        /// </summary>
        Smpte2085 = 11,

        /// <summary>
        /// Chromaticity-derived non-constant luminance
        /// </summary>
        CromatNCL = 12,

        /// <summary>
        /// Chromaticity-derived constant luminance
        /// </summary>
        CromatCL = 13,

        /// <summary>
        /// BT.2100-0 ICtCp
        /// </summary>
        ICtCp = 14
    }
}
