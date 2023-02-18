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
using LibHeifSharp.Properties;

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
        /// Gets a value indicating whether LibHeif can write two color profiles when both ICC and NCLX profiles are available.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if LibHeif can write two color profiles; otherwise, <see langword="false"/>.
        /// </value>
        public static bool CanWriteTwoColorProfiles => LibHeifVersion.Is1Point10OrLater;

        /// <summary>
        /// Gets the LibHeif version.
        /// </summary>
        /// <value>
        /// The LibHeif version.
        /// </value>
        public static Version Version => libheifVersion.Value;

        internal static uint VersionNumber => libheifVersionNumber.Value;

        /// <summary>
        /// Gets a list of the decoder descriptors.
        /// </summary>
        /// <param name="format">The compression format.</param>
        /// <returns>The decoder descriptors.</returns>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <remarks>
        /// This method is supported starting with LibHeif version 1.15.0, and will return an empty collection on older versions.
        /// </remarks>
        public static IReadOnlyList<HeifDecoderDescriptor> GetDecoderDescriptors(HeifCompressionFormat format = HeifCompressionFormat.Undefined)
        {
            var decoderDescriptors = Array.Empty<HeifDecoderDescriptor>();

            if (LibHeifVersion.Is1Point15OrLater)
            {
                lock (nativeCallLock)
                {
                    unsafe
                    {
                        using (var initializationContext = new LibHeifInitializationContext())
                        {
                            int count = LibHeifNative.heif_get_decoder_descriptors(format, null, 0);

                            if (count > 0)
                            {
                                var nativeDecoderDescriptors = new heif_decoder_descriptor[count];

                                fixed (heif_decoder_descriptor* ptr = nativeDecoderDescriptors)
                                {
                                    int returnedDecoderCount = LibHeifNative.heif_get_decoder_descriptors(format,
                                                                                                          ptr,
                                                                                                          nativeDecoderDescriptors.Length);

                                    if (returnedDecoderCount != count)
                                    {
                                        ExceptionUtil.ThrowHeifException(Resources.CannotGetAllDecoderDescriptors);
                                    }
                                }

                                decoderDescriptors = new HeifDecoderDescriptor[count];

                                for (int i = 0; i < decoderDescriptors.Length; i++)
                                {
                                    decoderDescriptors[i] = new HeifDecoderDescriptor(nativeDecoderDescriptors[i]);
                                }
                            }
                        }
                    }
                }
            }

            return decoderDescriptors;
        }

        /// <summary>
        /// Determines whether LibHeif has a decoder for the specified <see cref="HeifCompressionFormat"/>.
        /// </summary>
        /// <param name="format">The compression format.</param>
        /// <returns>
        /// <see langword="true" /> if LibHeif has a decoder for the specified <see cref="HeifCompressionFormat"/>;
        /// otherwise, <see langword="false" />.
        /// </returns>
        public static bool HaveDecoder(HeifCompressionFormat format)
        {
            bool result;
            lock (nativeCallLock)
            {
                try
                {
                    using (var initializationContext = new LibHeifInitializationContext())
                    {
                        result = LibHeifNative.heif_have_decoder_for_format(format);
                    }
                }
                catch (HeifException)
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Determines whether LibHeif has an encoder for the specified <see cref="HeifCompressionFormat"/>.
        /// </summary>
        /// <param name="format">The compression format.</param>
        /// <returns>
        /// <see langword="true" /> if LibHeif an encoder for the specified <see cref="HeifCompressionFormat"/>;
        /// otherwise, <see langword="false" />.
        /// </returns>
        public static bool HaveEncoder(HeifCompressionFormat format)
        {
            bool result;
            lock (nativeCallLock)
            {
                try
                {
                    using (var initializationContext = new LibHeifInitializationContext())
                    {
                        result = LibHeifNative.heif_have_encoder_for_format(format);
                    }
                }
                catch (HeifException)
                {
                    result = false;
                }
            }

            return result;
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
