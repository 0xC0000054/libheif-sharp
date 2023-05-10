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
    /// The rotation transformation property.
    /// </summary>
    /// <seealso cref="TransformationProperty"/>
    /// <threadsafety static="true" instance="false"/>
    public sealed class RotationTransformationProperty : TransformationProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RotationTransformationProperty"/> class.
        /// </summary>
        /// <param name="rotationAngle">The rotation angle.</param>
        /// <exception cref="HeifException">Unsupported rotation angle: {rotationAngle} degrees.</exception>
        internal RotationTransformationProperty(int rotationAngle) : base(TransformationPropertyType.Rotation)
        {
            switch (rotationAngle)
            {
                case 0:
                    this.Rotation = RotationType.None;
                    break;
                case 90:
                    this.Rotation = RotationType.Rotate90DegreesCounterClockwise;
                    break;
                case 180:
                    this.Rotation = RotationType.Rotate180Degrees;
                    break;
                case 270:
                    this.Rotation = RotationType.Rotate270DegreesCounterClockwise;
                    break;
                default:
                    throw new HeifException($"Unsupported rotation angle: {rotationAngle} degrees.");
            }
        }

        /// <summary>
        /// Gets the rotation that the image requires.
        /// </summary>
        /// <value>
        /// The rotation that the image requires.
        /// </value>
        public RotationType Rotation { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            string rotationType;

            switch (this.Rotation)
            {
                case RotationType.None:
                    rotationType = "none";
                    break;
                case RotationType.Rotate90DegreesCounterClockwise:
                    rotationType = "90 degrees counter-clockwise";
                    break;
                case RotationType.Rotate180Degrees:
                    rotationType = "180 degrees";
                    break;
                case RotationType.Rotate270DegreesCounterClockwise:
                    rotationType = "270 degrees counter-clockwise";
                    break;
                default:
                    rotationType = this.Rotation.ToString();
                    break;
            }

            return $"rotation: {rotationType}";
        }
    }
}
