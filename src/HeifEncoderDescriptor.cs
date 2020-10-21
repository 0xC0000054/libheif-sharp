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

using LibHeifSharp.Interop;

namespace LibHeifSharp
{
    /// <summary>
    /// Represents a LibHeif encoder descriptor.
    /// </summary>
    /// <seealso cref="HeifContext.GetEncoder(HeifEncoderDescriptor)"/>
    public sealed class HeifEncoderDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeifEncoderDescriptor"/> class.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        internal HeifEncoderDescriptor(heif_encoder_descriptor descriptor)
        {
            this.Name = LibHeifNative.heif_encoder_descriptor_get_name(descriptor).GetStringValue();
            this.IdName = LibHeifNative.heif_encoder_descriptor_get_id_name(descriptor).GetStringValue();
            this.CompressionFormat = LibHeifNative.heif_encoder_descriptor_get_compression_format(descriptor);
            this.SupportsLossyCompression = LibHeifNative.heif_encoder_descriptor_supports_lossy_compression(descriptor);
            this.SupportsLosslessCompression = LibHeifNative.heif_encoder_descriptor_supports_lossless_compression(descriptor);
            this.Descriptor = descriptor;
        }

        /// <summary>
        /// Gets the encoder name.
        /// </summary>
        /// <value>
        /// The encoder name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the encoder id name.
        /// </summary>
        /// <value>
        /// The encoder id name.
        /// </value>
        public string IdName { get; }

        /// <summary>
        /// Gets the encoder compression format.
        /// </summary>
        /// <value>
        /// The encoder compression format.
        /// </value>
        public HeifCompressionFormat CompressionFormat { get; }

        /// <summary>
        /// Gets a value indicating whether the encoder supports lossy compression.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the encoder supports lossy compression; otherwise, <see langword="false"/>.
        /// </value>
        public bool SupportsLossyCompression { get; }

        /// <summary>
        /// Gets a value indicating whether the encoder supports lossless compression.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the encoder supports lossless compression; otherwise, <see langword="false"/>.
        /// </value>
        public bool SupportsLosslessCompression { get; }

        /// <summary>
        /// Gets the encoder descriptor.
        /// </summary>
        /// <value>
        /// The encoder descriptor.
        /// </value>
        internal heif_encoder_descriptor Descriptor { get; }
    }
}
