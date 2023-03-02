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
    /// Represents the type information of a meta-data item.
    /// </summary>
    /// <seealso cref="HeifImageHandle.GetMetadataBlockInfo(HeifItemId)"/>
    /// <threadsafety static="true" instance="true"/>
    public sealed class HeifMetadataBlockInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeifMetadataBlockInfo"/> class.
        /// </summary>
        /// <param name="id">The item identifier.</param>
        /// <param name="itemType">The item type.</param>
        /// <param name="contentType">The item content type.</param>
        /// <param name="size">The item size in bytes.</param>
        internal HeifMetadataBlockInfo(HeifItemId id,
                                       string itemType,
                                       string contentType,
                                       int size)
        {
            this.Id = id;
            this.ItemType = itemType ?? string.Empty;
            this.ContentType = contentType ?? string.Empty;
            this.Size = size;
        }

        /// <summary>
        /// Gets the meta-data item identifier.
        /// </summary>
        /// <value>
        /// The meta-data item identifier.
        /// </value>
        public HeifItemId Id { get; }

        /// <summary>
        /// Gets a string describing the meta-data item type.
        /// </summary>
        /// <value>
        /// The meta-data item type.
        /// </value>
        public string ItemType { get; }

        /// <summary>
        /// Gets a string describing the meta-data content type.
        /// </summary>
        /// <value>
        /// The meta-data content type.
        /// </value>
        public string ContentType { get; }

        /// <summary>
        /// Gets the size in bytes of the meta-data block.
        /// </summary>
        /// <value>
        /// The size in bytes of the meta-data block.
        /// </value>
        public int Size { get; }
    }
}
