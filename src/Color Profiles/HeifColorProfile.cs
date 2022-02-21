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

using LibHeifSharp.Interop;

namespace LibHeifSharp
{
    /// <summary>
    /// The base class for a HEIF image color profile.
    /// </summary>
    public abstract class HeifColorProfile
    {
        private protected HeifColorProfile(ColorProfileType profileType)
        {
            this.ProfileType = profileType;
        }

        /// <summary>
        /// Gets the type of the color profile.
        /// </summary>
        /// <value>
        /// The type of the color profile.
        /// </value>
        public ColorProfileType ProfileType { get; }

        internal abstract unsafe void SetImageColorProfile(SafeHeifImage image);
    }
}
