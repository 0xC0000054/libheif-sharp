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
using LibHeifSharp.Interop;
using LibHeifSharp.Properties;

namespace LibHeifSharp
{
    /// <summary>
    /// Represents an International Color Consortium (ICC) color profile.
    /// </summary>
    /// <seealso cref="HeifImage.IccColorProfile" qualifyHint="true"/>
    /// <seealso cref="HeifImageHandle.IccColorProfile" qualifyHint="true"/>
    /// <seealso cref="HeifColorProfile" />
    public sealed class HeifIccColorProfile : HeifColorProfile
    {
        private readonly byte[] iccProfileBytes;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifIccColorProfile"/> class.
        /// </summary>
        /// <param name="iccProfile">The ICC profile.</param>
        /// <exception cref="ArgumentNullException"><paramref name="iccProfile"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="iccProfile"/> is an empty array.</exception>
        public HeifIccColorProfile(byte[] iccProfile) : this(iccProfile, copyToNewArray: true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifIccColorProfile"/> class.
        /// </summary>
        /// <param name="iccProfile">The ICC profile.</param>
        /// <param name="copyToNewArray">
        /// <see langword="true"/> if the parameter should be copied to a new array; otherwise, <see langword="false"/>
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="iccProfile"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="iccProfile"/> is an empty array.</exception>
        private HeifIccColorProfile(byte[] iccProfile, bool copyToNewArray) : base(ColorProfileType.Icc)
        {
            Validate.IsNotNullOrEmptyArray(iccProfile, nameof(iccProfile));

            if (copyToNewArray)
            {
                this.iccProfileBytes = new byte[iccProfile.Length];
                iccProfile.CopyTo(this.iccProfileBytes, 0);
            }
            else
            {
                this.iccProfileBytes = iccProfile;
            }
        }

        /// <summary>
        /// Gets the ICC profile bytes.
        /// </summary>
        /// <returns>A clone of the ICC color profile.</returns>
        public byte[] GetIccProfileBytes()
        {
            byte[] clone = new byte[this.iccProfileBytes.Length];
            this.iccProfileBytes.CopyTo(clone, 0);

            return clone;
        }

        /// <summary>
        /// Create a <see cref="HeifIccColorProfile"/> from the specified image handle.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <returns>The created profile.</returns>
        /// <exception cref="HeifException">
        /// A LibHeif error occurred.
        ///
        /// -or-
        ///
        /// The ICC profile is larger than 2 GB.
        /// </exception>
        internal static unsafe HeifIccColorProfile TryCreate(SafeHeifImageHandle handle)
        {
            HeifIccColorProfile profile = null;

            ulong iccProfileSize = LibHeifNative.heif_image_handle_get_raw_color_profile_size(handle).ToUInt64();

            if (iccProfileSize > 0)
            {
                if (iccProfileSize > int.MaxValue)
                {
                    ExceptionUtil.ThrowHeifException(Resources.IccProfileLargerThan2Gb);
                }

                byte[] iccProfileBytes = new byte[iccProfileSize];

                fixed (byte* ptr = iccProfileBytes)
                {
                    var error = LibHeifNative.heif_image_handle_get_raw_color_profile(handle, ptr);
                    error.ThrowIfError();
                }

                profile = new HeifIccColorProfile(iccProfileBytes, copyToNewArray: false);
            }

            return profile;
        }

        /// <summary>
        /// Create a <see cref="HeifIccColorProfile"/> from the specified image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>The created profile.</returns>
        /// <exception cref="HeifException">
        /// A LibHeif error occurred.
        ///
        /// -or-
        ///
        /// The ICC profile is larger than 2 GB.
        /// </exception>
        internal static unsafe HeifIccColorProfile TryCreate(SafeHeifImage image)
        {
            HeifIccColorProfile profile = null;

            ulong iccProfileSize = LibHeifNative.heif_image_get_raw_color_profile_size(image).ToUInt64();

            if (iccProfileSize > 0)
            {
                if (iccProfileSize > int.MaxValue)
                {
                    ExceptionUtil.ThrowHeifException(Resources.IccProfileLargerThan2Gb);
                }

                byte[] iccProfileBytes = new byte[iccProfileSize];

                fixed (byte* ptr = iccProfileBytes)
                {
                    var error = LibHeifNative.heif_image_get_raw_color_profile(image, ptr);
                    error.ThrowIfError();
                }

                profile = new HeifIccColorProfile(iccProfileBytes, copyToNewArray: false);
            }

            return profile;
        }

        /// <summary>
        /// Sets the image color profile.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        internal override unsafe void SetImageColorProfile(SafeHeifImage image)
        {
            fixed (byte* ptr = this.iccProfileBytes)
            {
                var profileSize = new UIntPtr((uint)this.iccProfileBytes.Length);
                var error = LibHeifNative.heif_image_set_raw_color_profile(image, "prof", ptr, profileSize);
                error.ThrowIfError();
            }
        }
    }
}
