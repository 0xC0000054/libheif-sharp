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
    /// The options that can be set when encoding an image.
    /// </summary>
    public sealed class HeifEncodingOptions
    {
        private bool writeTwoColorProfiles;
        private bool writeNclxColorProfile;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifEncodingOptions"/> class.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        public HeifEncodingOptions()
        {
            this.SaveAlphaChannel = true;
            this.CropWithImageGrid = true;
            this.WriteTwoColorProfiles = false;
            this.WriteNclxColorProfile = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the alpha channel should be saved.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the alpha channel should be saved; otherwise, <see langword="false"/>.
        /// </value>
        public bool SaveAlphaChannel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether LibHeif should use an image grid for cropping.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if LibHeif should use an image grid for cropping; otherwise, <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// This value was added in LibHeif version 1.9.2, it will be ignored on older versions.
        /// <para>It is used as a compatibility workaround for macOS.</para>
        /// </remarks>
        public bool CropWithImageGrid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether two color profiles will be written when both
        /// ICC and NCLX profiles are available.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if two color profiles should be written; otherwise, <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// This value was added in LibHeif version 1.10.0, it will be ignored on older versions.
        /// </remarks>
        public bool WriteTwoColorProfiles
        {
            get => this.writeTwoColorProfiles;
            set
            {
                if (this.writeTwoColorProfiles != value)
                {
                    this.writeTwoColorProfiles = value;
                    if (value && !this.writeNclxColorProfile)
                    {
                        this.writeNclxColorProfile = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a NCLX color profile will be written.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if a NCLX color profile will be written; otherwise, <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// This value was added in LibHeif version 1.11.0, it will be ignored on older versions.
        /// <para>
        /// It is used as a compatibility workaround for some versions of macOS and iOS
        /// that cannot read images with a NCLX color profile.
        /// </para>
        /// </remarks>
        public bool WriteNclxColorProfile
        {
            get => this.writeNclxColorProfile;
            set
            {
                if (this.writeNclxColorProfile != value)
                {
                    this.writeNclxColorProfile = value;
                    if (!value && this.writeTwoColorProfiles)
                    {
                        this.writeTwoColorProfiles = false;
                    }
                }
            }
        }

        /// <summary>
        /// Creates the encoding options.
        /// </summary>
        /// <returns>The encoding options.</returns>
        /// <exception cref="HeifException">Unable to create the native HeifEncodingOptions.</exception>
        internal unsafe SafeHeifEncodingOptions CreateEncodingOptions()
        {
            var encodingOptions = LibHeifNative.heif_encoding_options_alloc();

            if (encodingOptions.IsInvalid)
            {
                ExceptionUtil.ThrowHeifException(Properties.Resources.HeifEncodingOptionsCreationFailed);
            }

            var options = (EncodingOptionsVersion1*)encodingOptions.DangerousGetHandle();

            options->save_alpha_channel = (byte)(this.SaveAlphaChannel ? 1 : 0);

            if (options->version >= 4)
            {
                var optionsVersion4 = (EncodingOptionsVersion4*)encodingOptions.DangerousGetHandle();

                optionsVersion4->macOS_compatibility_workaround = (byte)(this.CropWithImageGrid ? 1 : 0);
                optionsVersion4->save_two_colr_boxes_when_ICC_and_nclx_available = (byte)(this.WriteTwoColorProfiles ? 1 : 0);
                optionsVersion4->macOS_compatibility_workaround_no_nclx_profile = (byte)(this.WriteNclxColorProfile ? 0 : 1);
            }
            else if (options->version == 3)
            {
                var optionsVersion3 = (EncodingOptionsVersion3*)encodingOptions.DangerousGetHandle();

                optionsVersion3->macOS_compatibility_workaround = (byte)(this.CropWithImageGrid ? 1 : 0);
                optionsVersion3->save_two_colr_boxes_when_ICC_and_nclx_available = (byte)(this.WriteTwoColorProfiles ? 1 : 0);
            }
            else if (options->version == 2)
            {
                var optionsVersion2 = (EncodingOptionsVersion2*)encodingOptions.DangerousGetHandle();

                optionsVersion2->macOS_compatibility_workaround = (byte)(this.CropWithImageGrid ? 1 : 0);
            }

            return encodingOptions;
        }
    }
}
