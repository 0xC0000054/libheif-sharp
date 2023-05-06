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
    /// Represents a transformed polygon point.
    /// </summary>
    public readonly struct TransformedPolygonPoint : IEquatable<TransformedPolygonPoint>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformedPolygonPoint"/> structure.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        public TransformedPolygonPoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Gets the x coordinate.
        /// </summary>
        /// <value>
        /// The x coordinate.
        /// </value>
        public double X { get; }

        /// <summary>
        /// Gets the y coordinate.
        /// </summary>
        /// <value>
        /// The y coordinate.
        /// </value>
        public double Y { get; }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is TransformedPolygonPoint other && Equals(other);

        /// <inheritdoc/>
        public bool Equals(TransformedPolygonPoint other) => this.X == other.X && this.Y == other.Y;

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 1861411795;

            unchecked
            {
                hashCode = hashCode * -1521134295 + this.X.GetHashCode();
                hashCode = hashCode * -1521134295 + this.Y.GetHashCode();
            }

            return hashCode;
        }

        /// <inheritdoc/>
        public override string ToString() => $"{{X={this.X},Y={this.Y}}}";

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(TransformedPolygonPoint left, TransformedPolygonPoint right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(TransformedPolygonPoint left, TransformedPolygonPoint right)
        {
            return !(left == right);
        }
    }
}
