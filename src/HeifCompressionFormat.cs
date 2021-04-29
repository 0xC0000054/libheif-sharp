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
    /// The LibHeif compression format.
    /// </summary>
    /// <seealso cref="HeifContext.GetEncoder(HeifCompressionFormat)"/>
    /// <seealso cref="LibHeifInfo.HaveDecoder(HeifCompressionFormat)"/>
    /// <seealso cref="LibHeifInfo.HaveEncoder(HeifCompressionFormat)"/>
    public enum HeifCompressionFormat
    {
        /// <summary>
        /// The compression format is not defined.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The compression format is HEVC.
        /// </summary>
        Hevc = 1,

        /// <summary>
        /// The compression format is AVC.
        /// </summary>
        Avc = 2,

        /// <summary>
        /// The compression format is JPEG.
        /// </summary>
        Jpeg = 3,

        /// <summary>
        /// The compression format is AV1.
        /// </summary>
        Av1 = 4
    }
}
