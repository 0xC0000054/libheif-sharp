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
using System.Collections.ObjectModel;
using System.Text;
using LibHeifSharp.Interop;

namespace LibHeifSharp
{
    /// <summary>
    /// The polyline region geometry.
    /// </summary>
    /// <seealso cref="RegionGeometry" />
    public sealed class RegionPolyline : RegionGeometry
    {
        private readonly ReadOnlyCollection<PolygonPoint> points;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionPolyline"/> class.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <exception cref="HeifException">
        /// A LibHeif error occurred.
        /// -or-
        /// Overflow when getting the polyline points.
        /// </exception>
        internal RegionPolyline(heif_region region) : base(RegionGeometryType.Polyline)
        {
            var points = Array.Empty<PolygonPoint>();

            int count = LibHeifNative.heif_region_get_polyline_num_points(region);

            if (count > 0)
            {
                points = new PolygonPoint[count];

                try
                {
                    int pointArrayLength = checked(count * 2);

                    int[] pointArray = new int[pointArrayLength];

                    unsafe
                    {
                        fixed (int* ptr = pointArray)
                        {
                            var error = LibHeifNative.heif_region_get_polyline_points(region, ptr);
                            error.ThrowIfError();
                        }
                    }

                    for (int i = 0; i < points.Length; i++)
                    {
                        points[i] = new PolygonPoint(pointArray[i * 2], pointArray[(i * 2) + 1]);
                    }
                }
                catch (OverflowException ex)
                {
                    throw new HeifException("Overflow when getting the polyline points.", ex);
                }
            }

            this.points = new ReadOnlyCollection<PolygonPoint>(points);
        }

        /// <summary>
        /// Gets a list of the polyline points.
        /// </summary>
        /// <value>
        /// The list of polyline points.
        /// </value>
        public IReadOnlyList<PolygonPoint> Points => this.points;

        /// <inheritdoc/>
        public override string ToString()
        {
            var builder = new StringBuilder("polyline [");

            int count = this.points.Count;
            int lastItemIndex = count - 1;

            for (int i = 0; i < count; i++)
            {
                var point = this.points[i];

                builder.AppendFormat("({0},{1})", point.X, point.Y);

                if (i < lastItemIndex)
                {
                    builder.Append(", ");
                }
            }

            builder.Append(']');

            return builder.ToString();
        }
    }
}
