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

namespace LibHeifSharp
{
    /// <summary>
    /// Represents a LibHeif region item id.
    /// </summary>
    /// <threadsafety static="true" instance="true"/>
    public readonly struct HeifRegionItemId : IEquatable<HeifRegionItemId>
    {
        internal HeifRegionItemId(HeifItemId imageHandleId, HeifItemId regionItemId)
        {
            this.ImageHandleId = imageHandleId;
            this.RegionItemId = regionItemId;
        }

        /// <summary>
        /// Gets the image handle identifier that is associated with this region item.
        /// </summary>
        /// <value>
        /// The image handle identifier.
        /// </value>
        internal HeifItemId ImageHandleId { get; }

        /// <summary>
        /// Gets the region item identifier.
        /// </summary>
        /// <value>
        /// The region item identifier.
        /// </value>
        internal HeifItemId RegionItemId { get; }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is HeifRegionItemId other && Equals(other);

        /// <inheritdoc/>
        public bool Equals(HeifRegionItemId other)
            => this.ImageHandleId == other.ImageHandleId && this.RegionItemId == other.RegionItemId;

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 798332308;

            unchecked
            {
                hashCode = hashCode * -1521134295 + this.ImageHandleId.GetHashCode();
                hashCode = hashCode * -1521134295 + this.RegionItemId.GetHashCode();
            }

            return hashCode;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(HeifRegionItemId left, HeifRegionItemId right) => left.Equals(right);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(HeifRegionItemId left, HeifRegionItemId right) => !(left == right);
    }
}
