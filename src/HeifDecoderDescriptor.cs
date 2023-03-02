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

using System.Diagnostics;
using LibHeifSharp.Interop;

namespace LibHeifSharp
{
    /// <summary>
    /// Represents a LibHeif decoder descriptor.
    /// </summary>
    /// <seealso cref="LibHeifInfo.GetDecoderDescriptors(HeifCompressionFormat)"/>
    /// <threadsafety static="true" instance="true"/>
    [DebuggerDisplay("{" + nameof(Name) + ",nq}")]
    public sealed class HeifDecoderDescriptor
    {
        internal HeifDecoderDescriptor(heif_decoder_descriptor descriptor)
        {
            this.Name = LibHeifNative.heif_decoder_descriptor_get_name(descriptor).GetStringValue();
            this.IdName = LibHeifNative.heif_decoder_descriptor_get_id_name(descriptor).GetStringValue();
        }

        /// <summary>
        /// Gets the decoder name.
        /// </summary>
        /// <value>
        /// The decoder name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the decoder id name.
        /// </summary>
        /// <value>
        /// The decoder id name.
        /// </value>
        /// <seealso cref="HeifDecodingOptions.DecoderId"/>
        public string IdName { get; }
    }
}
