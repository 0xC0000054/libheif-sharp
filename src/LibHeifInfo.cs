/*
 * .NET bindings for libheif.
 * Copyright (c) 2020 Nicholas Hayes
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
using LibHeifSharp.Interop;

namespace LibHeifSharp
{
    /// <summary>
    /// Provides information about LibHeif.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public static class LibHeifInfo
    {
        private static readonly Lazy<Version> libheifVersion = new Lazy<Version>(GetLibHeifVersion);
        private static readonly Lazy<uint> libheifVersionNumber = new Lazy<uint>(GetLibHeifVersionNumber);
        private static readonly object nativeCallLock = new object();

        /// <summary>
        /// Gets the LibHeif version.
        /// </summary>
        /// <value>
        /// The LibHeif version.
        /// </value>
        public static Version Version => libheifVersion.Value;

        internal static uint VersionNumber => libheifVersionNumber.Value;

        /// <summary>
        /// Determines whether LibHief supports the specified decoder.
        /// </summary>
        /// <param name="format">The compression format.</param>
        /// <returns>
        /// <see langword="true" /> if LibHief supports the specified decoder; otherwise, <see langword="false" />.
        /// </returns>
        public static bool HaveDecoder(HeifCompressionFormat format)
        {
            lock (nativeCallLock)
            {
                return LibHeifNative.heif_have_decoder_for_format(format);
            }
        }

        /// <summary>
        /// Determines whether LibHief supports the specified encoder.
        /// </summary>
        /// <param name="format">The compression format.</param>
        /// <returns>
        /// <see langword="true" /> if LibHief supports the specified encoder; otherwise, <see langword="false" />.
        /// </returns>
        public static bool HaveEncoder(HeifCompressionFormat format)
        {
            lock (nativeCallLock)
            {
                return LibHeifNative.heif_have_encoder_for_format(format);
            }
        }

        private static Version GetLibHeifVersion()
        {
            uint version = libheifVersionNumber.Value;

            int major = (int)((version >> 24) & 0xff);
            int minor = (int)((version >> 16) & 0xff);
            int maintenance = (int)((version >> 8) & 0xff);

            return new Version(major, minor, maintenance);
        }

        private static uint GetLibHeifVersionNumber()
        {
            return LibHeifNative.heif_get_version_number();
        }
    }
}
