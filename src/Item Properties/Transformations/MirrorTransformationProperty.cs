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
    /// The mirror transformation property.
    /// </summary>
    /// <seealso cref="TransformationProperty"/>
    /// <threadsafety static="true" instance="false"/>
    public sealed class MirrorTransformationProperty : TransformationProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MirrorTransformationProperty"/> class.
        /// </summary>
        /// <param name="direction">The mirror direction.</param>
        internal MirrorTransformationProperty(MirrorDirection direction) : base(TransformationPropertyType.Mirror)
        {
            this.Direction = direction;
        }

        /// <summary>
        /// Gets the mirror direction.
        /// </summary>
        /// <value>
        /// The mirror direction.
        /// </value>
        public MirrorDirection Direction { get; }

        /// <inheritdoc/>
        public override string ToString() => $"mirror: {this.Direction.ToString().ToLowerInvariant()}";
    }
}
