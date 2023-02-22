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
using LibHeifSharp.Interop;
using LibHeifSharp.Properties;

namespace LibHeifSharp
{
    /// <summary>
    /// Represents the Content Light Level values used by HDR images.
    /// </summary>
    /// <seealso cref="IEquatable{T}" />
    public sealed class HeifContentLightLevel : IEquatable<HeifContentLightLevel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeifContentLightLevel"/> class.
        /// </summary>
        /// <param name="maxContentLightLevel">
        /// The maximum content light level in candelas per square meter.
        /// A value of 0 indicates that the value is undefined.
        /// </param>
        /// <param name="maxPictureAverageLightLevel">
        /// The maximum picture average light level in candelas per square meter.
        /// A value of 0 indicates that the value is undefined.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Both <paramref name="maxContentLightLevel"/> and <paramref name="maxPictureAverageLightLevel"/> are zero.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maxContentLightLevel"/> must be in the range of [0, 65535].
        /// <paramref name="maxPictureAverageLightLevel"/> must be in the range of [0, 65535].
        /// </exception>
        public HeifContentLightLevel(int maxContentLightLevel, int maxPictureAverageLightLevel)
        {
            Validate.IsInRange(maxContentLightLevel,
                               nameof(maxContentLightLevel),
                               ushort.MinValue,
                               ushort.MaxValue);
            Validate.IsInRange(maxPictureAverageLightLevel,
                               nameof(maxPictureAverageLightLevel),
                               ushort.MinValue,
                               ushort.MaxValue);

            if (maxContentLightLevel == 0 && maxPictureAverageLightLevel == 0)
            {
                ExceptionUtil.ThrowArgumentException(Resources.AtLeastOneParameterMustBePositive);
            }

            this.MaxContentLightLevel = maxContentLightLevel;
            this.MaxPictureAverageLightLevel = maxPictureAverageLightLevel;
        }

        internal HeifContentLightLevel(heif_content_light_level value)
        {
            this.MaxContentLightLevel = value.max_content_light_level;
            this.MaxPictureAverageLightLevel = value.max_pic_average_light_level;
        }

        /// <summary>
        /// Gets the maximum content light level.
        /// </summary>
        /// <value>
        /// The maximum content light level.
        /// A value of 0 indicates that the value is undefined.
        /// </value>
        public int MaxContentLightLevel { get; }

        /// <summary>
        /// Gets the maximum picture average light level.
        /// </summary>
        /// <value>
        /// The maximum picture average light level.
        /// A value of 0 indicates that the value is undefined.
        /// </value>
        public int MaxPictureAverageLightLevel { get; }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is HeifContentLightLevel other && Equals(other);
        }

        /// <inheritdoc/>
        public bool Equals(HeifContentLightLevel other)
        {
            if (other is null)
            {
                return false;
            }

            return this.MaxContentLightLevel == other.MaxContentLightLevel
                && this.MaxPictureAverageLightLevel == other.MaxPictureAverageLightLevel;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = -969852998;

            hashCode = hashCode * -1521134295 + this.MaxContentLightLevel.GetHashCode();
            hashCode = hashCode * -1521134295 + this.MaxPictureAverageLightLevel.GetHashCode();

            return hashCode;
        }

        internal unsafe void SetImageContentLightLevel(SafeHeifImage image)
        {
            Validate.IsNotNull(image, nameof(image));

            heif_content_light_level nativeContentLightLevel;
            nativeContentLightLevel.max_content_light_level = (ushort)this.MaxContentLightLevel;
            nativeContentLightLevel.max_pic_average_light_level = (ushort)this.MaxPictureAverageLightLevel;

            LibHeifNative.heif_image_set_content_light_level(image, &nativeContentLightLevel);
        }

        /// <inheritdoc/>
        public static bool operator ==(HeifContentLightLevel left, HeifContentLightLevel right)
        {
            return EqualityComparer<HeifContentLightLevel>.Default.Equals(left, right);
        }

        /// <inheritdoc/>
        public static bool operator !=(HeifContentLightLevel left, HeifContentLightLevel right)
        {
            return !(left == right);
        }
    }
}
