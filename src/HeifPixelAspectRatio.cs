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
using System.Diagnostics;

namespace LibHeifSharp
{
    /// <summary>
    /// Represents the pixel aspect ratio of an image
    /// </summary>
    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    public readonly struct HeifPixelAspectRatio : IEquatable<HeifPixelAspectRatio>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeifPixelAspectRatio"/> class.
        /// </summary>
        /// <param name="horizontalSpacing">The horizontal spacing.</param>
        /// <param name="verticalSpacing">The vertical spacing.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="verticalSpacing"/> must be in the range of [1, 4294967295].
        ///
        /// -or-
        ///
        /// <paramref name="verticalSpacing"/> must be in the range of [1, 4294967295].
        /// </exception>
        public HeifPixelAspectRatio(long horizontalSpacing, long verticalSpacing)
        {
            Validate.IsInRange(horizontalSpacing, nameof(horizontalSpacing), 1, uint.MaxValue);
            Validate.IsInRange(verticalSpacing, nameof(verticalSpacing), 1, uint.MaxValue);

            this.HorizontalSpacing = horizontalSpacing;
            this.VerticalSpacing = verticalSpacing;
        }

        internal HeifPixelAspectRatio(uint aspectH, uint aspectV)
        {
            this.HorizontalSpacing = aspectH;
            this.VerticalSpacing = aspectV;
        }

        /// <summary>
        /// Gets the relative width of a pixel.
        /// </summary>
        /// <value>
        /// The relative width of a pixel.
        /// </value>
        public long HorizontalSpacing { get; }

        /// <summary>
        /// Gets the relative height of a pixel.
        /// </summary>
        /// <value>
        /// The relative height of a pixel.
        /// </value>
        public long VerticalSpacing { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has a square aspect ratio.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if this instance has a square aspect ratio;
        /// otherwise, <see langword="false"/>.
        /// </value>
        public bool HasSquareAspectRatio => this.HorizontalSpacing == this.VerticalSpacing;

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is HeifPixelAspectRatio other && Equals(other);
        }

        /// <inheritdoc/>
        public bool Equals(HeifPixelAspectRatio other)
        {
            return this.HorizontalSpacing == other.HorizontalSpacing
                && this.VerticalSpacing == other.VerticalSpacing;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 2082263674;

            hashCode = hashCode * -1521134295 + this.HorizontalSpacing.GetHashCode();
            hashCode = hashCode * -1521134295 + this.VerticalSpacing.GetHashCode();

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
        public static bool operator ==(HeifPixelAspectRatio left, HeifPixelAspectRatio right)
        {
            return EqualityComparer<HeifPixelAspectRatio>.Default.Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(HeifPixelAspectRatio left, HeifPixelAspectRatio right)
        {
            return !(left == right);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.HorizontalSpacing.ToString() + "/" + this.VerticalSpacing.ToString();
        }
    }
}
