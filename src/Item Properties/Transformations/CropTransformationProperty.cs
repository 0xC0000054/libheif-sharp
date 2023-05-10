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
    /// The crop transformation property.
    /// </summary>
    /// <seealso cref="TransformationProperty"/>
    /// <threadsafety static="true" instance="false"/>
    public sealed class CropTransformationProperty : TransformationProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CropTransformationProperty"/> class.
        /// </summary>
        /// <param name="left">The left edge of the crop rectangle.</param>
        /// <param name="top">The top edge of the crop rectangle.</param>
        /// <param name="right">The right edge of the crop rectangle.</param>
        /// <param name="bottom">The bottom edge of the crop rectangle.</param>
        internal CropTransformationProperty(int left, int top, int right, int bottom) : base(TransformationPropertyType.Crop)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }

        /// <summary>
        /// Gets the left edge of the crop rectangle.
        /// </summary>
        /// <value>
        /// The left edge of the crop rectangle.
        /// </value>
        public int Left { get; }

        /// <summary>
        /// Gets the top edge of the crop rectangle.
        /// </summary>
        /// <value>
        /// The top edge of the crop rectangle.
        /// </value>
        public int Top { get; }

        /// <summary>
        /// Gets the right edge of the crop rectangle.
        /// </summary>
        /// <value>
        /// The right edge of the crop rectangle.
        /// </value>
        public int Right { get; }

        /// <summary>
        /// Gets the bottom edge of the crop rectangle.
        /// </summary>
        /// <value>
        /// The bottom edge of the crop rectangle.
        /// </value>
        public int Bottom { get; }

        /// <inheritdoc/>
        public override string ToString()
            => $"crop: left={this.Left}, top={this.Top}, right={this.Right}, bottom={this.Bottom}";
    }
}
