/*
 * .NET bindings for libheif.
 * Copyright (c) 2020, 2021, 2022 Nicholas Hayes
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
    /// Represents a string LibHeif encoder parameter
    /// </summary>
    /// <seealso cref="HeifEncoderParameter{T}" />
    public sealed class HeifStringEncoderParameter : HeifEncoderParameter<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeifStringEncoderParameter"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="hasDefaultValue"><see langword="true"/> if the parameter has a default value; otherwise, <see langword="false"/>.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="validValues">The valid values.</param>
        internal HeifStringEncoderParameter(string name,
                                            bool hasDefaultValue,
                                            string defaultValue,
                                            ReadOnlyCollection<string> validValues)
            : base(name, hasDefaultValue, defaultValue)
        {
            this.ValidValues = validValues;
        }

        ///<inheritdoc/>
        public override HeifEncoderParameterType ParameterType => HeifEncoderParameterType.String;

        /// <summary>
        /// Gets the valid values.
        /// </summary>
        /// <value>
        /// The valid values.
        /// </value>
        public IReadOnlyList<string> ValidValues { get; }
    }
}
