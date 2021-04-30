/*
 * .NET bindings for libheif.
 * Copyright (c) 2020, 2021 Nicholas Hayes
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

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LibHeifSharp
{
    /// <summary>
    /// Represents an Integer LibHeif encoder parameter
    /// </summary>
    /// <seealso cref="HeifEncoderParameter{T}" />
    public sealed class HeifIntegerEncoderParameter : HeifEncoderParameter<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeifIntegerEncoderParameter"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="hasDefaultValue"><see langword="true"/> if this instance has a default value; otherwise, <see langword="false"/></param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="hasMinimumMaximum"><see langword="true"/> if this instance has minimum and maximum values; otherwise, <see langword="false"/></param>
        /// <param name="minimum">The minimum.</param>
        /// <param name="maximum">The maximum.</param>
        /// <param name="validValues">The valid values.</param>
        internal HeifIntegerEncoderParameter(string name,
                                             bool hasDefaultValue,
                                             int defaultValue,
                                             bool hasMinimumMaximum,
                                             int minimum,
                                             int maximum,
                                             ReadOnlyCollection<int> validValues)
            : base(name, hasDefaultValue, defaultValue)
        {
            this.HasMinimumMaximum = hasMinimumMaximum;
            this.Minimum = minimum;
            this.Maximum = maximum;
            this.ValidValues = validValues;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has minimum and maximum values.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if this instance has minimum and maximum values; otherwise, <see langword="false"/>.
        /// </value>
        public bool HasMinimumMaximum { get; }

        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        /// <value>
        /// The minimum value.
        /// </value>
        /// <remarks>
        /// The value of this property is only meaningful when <see cref="HasMinimumMaximum"/> is <see langword="true"/>.
        /// </remarks>
        public int Minimum { get; }

        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        /// <remarks>
        /// The value of this property is only meaningful when <see cref="HasMinimumMaximum"/> is <see langword="true"/>.
        /// </remarks>
        public int Maximum { get; }

        /// <summary>
        /// Gets the valid values.
        /// </summary>
        /// <value>
        /// The valid values.
        /// </value>
        public IReadOnlyList<int> ValidValues { get; }

        ///<inheritdoc/>
        public override HeifEncoderParameterType ParameterType => HeifEncoderParameterType.Integer;
    }
}
