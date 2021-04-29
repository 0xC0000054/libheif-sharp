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
    // These values are from the ITU-T H.273 (2016) specification.
    // https://www.itu.int/rec/T-REC-H.273-201612-I/en

    /// <summary>
    /// The NCLX transfer characteristics
    /// </summary>
    public enum TransferCharacteristics
    {
        /// <summary>
        /// For future use
        /// </summary>
        Reserved0 = 0,

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
        /// BT.470 System M (historical)
        /// </summary>
        BT470M = 4,

        /// <summary>
        /// BT.470 System B, G (historical)
        /// </summary>
        BT470BG = 5,

        /// <summary>
        /// BT.601
        /// </summary>
        BT601 = 6,

        /// <summary>
        /// SMPTE 240
        /// </summary>
        Smpte240 = 7,

        /// <summary>
        /// Linear
        /// </summary>
        Linear = 8,

        /// <summary>
        /// Logarithmic (100 : 1 range)
        /// </summary>
        Log100 = 9,

        /// <summary>
        /// Logarithmic (100 * Sqrt(10) : 1 range)
        /// </summary>
        Log100Sqrt10 = 10,

        /// <summary>
        /// IEC 61966-2-4
        /// </summary>
        IEC61966 = 11,

        /// <summary>
        /// BT.1361
        /// </summary>
        BT1361 = 12,

        /// <summary>
        /// sRGB or sYCC
        /// </summary>
        Srgb = 13,

        /// <summary>
        /// BT.2020-2 10-bit systems
        /// </summary>
        BT2020TenBit = 14,

        /// <summary>
        /// BT.2020-2 12-bit systems
        /// </summary>
        BT2020TwelveBit = 15,

        /// <summary>
        /// SMPTE ST 2084, ITU BT.2100-0 PQ
        /// </summary>
        Smpte2084 = 16,

        /// <summary>
        /// SMPTE ST 428-1
        /// </summary>
        Smpte428 = 17,

        /// <summary>
        /// BT.2100-0 HLG, ARIB STD-B67
        /// </summary>
        HLG = 18
    }
}
