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

using System.Diagnostics;

namespace LibHeifSharp
{
    /// <summary>
    /// The base class for the LibHeif encoder parameters
    /// </summary>
    /// <typeparam name="T">The encoder parameter type.</typeparam>
    /// <seealso cref="IHeifEncoderParameter" />
    [DebuggerDisplay("{" + nameof(Name) + ",nq}")]
    public abstract class HeifEncoderParameter<T> : IHeifEncoderParameter
    {
        private protected HeifEncoderParameter(string name, bool hasDefault, T defaultValue)
        {
            this.Name = name ?? string.Empty;
            this.HasDefault = hasDefault;
            this.DefaultValue = defaultValue;
        }

        /// <summary>
        /// Gets the default value of this parameter.
        /// </summary>
        /// <value>
        /// The default value of this parameter.
        /// </value>
        /// <remarks>
        /// The value of this property is only meaningful when <see cref="HasDefault"/> is <see langword="true"/>.
        /// </remarks>
        public T DefaultValue { get; }

        /// <summary>
        /// Gets a value indicating whether this parameter has a default value.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if this parameter has a default value; otherwise, <see langword="false"/>.
        /// </value>
        public bool HasDefault { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public abstract HeifEncoderParameterType ParameterType { get; }
    }
}
