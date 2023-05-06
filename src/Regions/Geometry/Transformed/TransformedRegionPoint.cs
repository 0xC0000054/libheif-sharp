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

using LibHeifSharp.Interop;

namespace LibHeifSharp
{
    /// <summary>
    /// The transformed region point geometry.
    /// </summary>
    /// <seealso cref="TransformedRegionGeometry" />
    public sealed class TransformedRegionPoint : TransformedRegionGeometry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformedRegionPoint"/> class.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="imageId">The image identifier.</param>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        internal TransformedRegionPoint(heif_region region, HeifItemId imageId) : base(RegionGeometryType.Point)
        {
            var error = LibHeifNative.heif_region_get_point_transformed(region,
                                                                        out double x,
                                                                        out double y,
                                                                        imageId);
            error.ThrowIfError();

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
        public override string ToString() => $"transformed_point [X={this.X}, Y={this.Y}]";
    }
}
