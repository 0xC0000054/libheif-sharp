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

using LibHeifSharp.Interop;

namespace LibHeifSharp
{
    /// <summary>
    /// Represents a NCLX color profile.
    /// </summary>
    /// <seealso cref="HeifColorProfile" />
    public sealed class HeifNclxColorProfile : HeifColorProfile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeifNclxColorProfile"/> class.
        /// </summary>
        /// <param name="colorPrimaries">The color primaries.</param>
        /// <param name="transferCharacteristics">The transfer characteristics.</param>
        /// <param name="matrixCoefficients">The matrix coefficients.</param>
        /// <param name="fullRange"><see langword="true"/> if the full color range is used; otherwise, <see langword="false"/>.</param>
        public HeifNclxColorProfile(ColorPrimaries colorPrimaries,
                                    TransferCharacteristics transferCharacteristics,
                                    MatrixCoefficients matrixCoefficients,
                                    bool fullRange) : base(ColorProfileType.Nclx)
        {
            this.ColorPrimaries = colorPrimaries;
            this.TransferCharacteristics = transferCharacteristics;
            this.MatrixCoefficients = matrixCoefficients;
            this.FullRange = fullRange;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifNclxColorProfile"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        internal unsafe HeifNclxColorProfile(SafeHeifImageHandle handle) : base(ColorProfileType.Nclx)
        {
            SafeHeifNclxColorProfile safeNclxProfile = null;

            try
            {
                var error = LibHeifNative.heif_image_handle_get_nclx_color_profile(handle, out safeNclxProfile);
                error.ThrowIfError();

                var nclxProfileV1 = (heif_nclx_color_profile_v1*)safeNclxProfile.DangerousGetHandle();

                this.ColorPrimaries = nclxProfileV1->colorPrimaries;
                this.TransferCharacteristics = nclxProfileV1->transferCharacteristics;
                this.MatrixCoefficients = nclxProfileV1->matrixCoefficients;
                this.FullRange = nclxProfileV1->fullRange != 0;
            }
            finally
            {
                safeNclxProfile?.Dispose();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifNclxColorProfile"/> class.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        internal unsafe HeifNclxColorProfile(SafeHeifImage image) : base(ColorProfileType.Nclx)
        {
            SafeHeifNclxColorProfile safeNclxProfile = null;

            try
            {
                var error = LibHeifNative.heif_image_get_nclx_color_profile(image, out safeNclxProfile);
                error.ThrowIfError();

                var nclxProfileV1 = (heif_nclx_color_profile_v1*)safeNclxProfile.DangerousGetHandle();

                this.ColorPrimaries = nclxProfileV1->colorPrimaries;
                this.TransferCharacteristics = nclxProfileV1->transferCharacteristics;
                this.MatrixCoefficients = nclxProfileV1->matrixCoefficients;
                this.FullRange = nclxProfileV1->fullRange != 0;
            }
            finally
            {
                safeNclxProfile?.Dispose();
            }
        }

        /// <summary>
        /// Gets the color primaries.
        /// </summary>
        /// <value>
        /// The color primaries.
        /// </value>
        public ColorPrimaries ColorPrimaries { get; }

        /// <summary>
        /// Gets the transfer characteristics.
        /// </summary>
        /// <value>
        /// The transfer characteristics.
        /// </value>
        public TransferCharacteristics TransferCharacteristics { get; }

        /// <summary>
        /// Gets the matrix coefficients.
        /// </summary>
        /// <value>
        /// The matrix coefficients.
        /// </value>
        public MatrixCoefficients MatrixCoefficients { get; }

        /// <summary>
        /// Gets a value indicating whether the full color range is used.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the full color range is used; otherwise, <see langword="false"/>.
        /// </value>
        public bool FullRange { get; }

        /// <summary>
        /// Sets the image color profile.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <exception cref="HeifException">
        /// The native NCLX profile creation failed.
        ///
        /// -or-
        ///
        /// A LibHeif error occurred.
        /// </exception>
        internal override unsafe void SetImageColorProfile(SafeHeifImage image)
        {
            using (var safeNclxProfile = LibHeifNative.heif_nclx_color_profile_alloc())
            {
                if (safeNclxProfile.IsInvalid)
                {
                    throw new HeifException(Properties.Resources.NclxProfileCreationFailed);
                }

                var nclxProfileV1 = (heif_nclx_color_profile_v1*)safeNclxProfile.DangerousGetHandle();

                nclxProfileV1->colorPrimaries = this.ColorPrimaries;
                nclxProfileV1->transferCharacteristics = this.TransferCharacteristics;
                nclxProfileV1->matrixCoefficients = this.MatrixCoefficients;
                nclxProfileV1->fullRange = (byte)(this.FullRange ? 1 : 0);

                var error = LibHeifNative.heif_image_set_nclx_color_profile(image, safeNclxProfile);
                error.ThrowIfError();
            }
        }
    }
}
