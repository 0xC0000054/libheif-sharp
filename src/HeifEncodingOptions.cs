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
using System.ComponentModel;
using LibHeifSharp.Interop;

namespace LibHeifSharp
{
    /// <summary>
    /// The options that can be set when encoding an image.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    public sealed class HeifEncodingOptions
    {
        private bool writeTwoColorProfiles;
        private bool writeNclxColorProfile;
        private HeifOrientation imageOrientation;
        private HeifColorConversionOptions colorConversionOptions;

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
            this.ImageOrientation = HeifOrientation.Normal;
            this.colorConversionOptions = new HeifColorConversionOptions();
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
        /// Gets or sets a value describing the transformations that will be applied
        /// to the decoded image before it is displayed.
        /// </summary>
        /// <value>
        /// The transformations that will be applied to the decoded image before it is displayed.
        /// </value>
        /// <exception cref="InvalidEnumArgumentException">
        /// Image orientation cannot be set because it does not use a valid value, as defined in
        /// the <see cref="HeifOrientation"/> enumeration.
        /// </exception>
        /// <remarks>
        /// This value was added in LibHeif version 1.14.0, it will be ignored on older versions.
        /// </remarks>
        public HeifOrientation ImageOrientation
        {
            get => this.imageOrientation;
            set
            {
                if (this.imageOrientation != value)
                {
                    if (value < HeifOrientation.Normal || value > HeifOrientation.Rotate270Clockwise)
                    {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(HeifOrientation));
                    }

                    this.imageOrientation = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the color conversion options.
        /// </summary>
        /// <value>
        /// The color conversion options.
        /// </value>
        /// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
        /// <remarks>
        /// This property is supported starting with LibHeif 1.16.0, it is ignored on earlier versions.
        /// </remarks>
        /// <seealso cref="HeifColorConversionOptions"/>
        public HeifColorConversionOptions ColorConversionOptions
        {
            get => this.colorConversionOptions;
            set
            {
                Validate.IsNotNull(value, nameof(value));

                this.colorConversionOptions = value;
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

            options->save_alpha_channel = this.SaveAlphaChannel.ToByte();

            if (options->version >= 6)
            {
                var optionsVersion6 = (EncodingOptionsVersion6*)encodingOptions.DangerousGetHandle();

                optionsVersion6->macOS_compatibility_workaround = this.CropWithImageGrid.ToByte();
                optionsVersion6->save_two_colr_boxes_when_ICC_and_nclx_available = this.WriteTwoColorProfiles.ToByte();
                optionsVersion6->macOS_compatibility_workaround_no_nclx_profile = BooleanExtensions.ToByte(!this.WriteNclxColorProfile);
                optionsVersion6->image_orientation = this.ImageOrientation;
                optionsVersion6->color_conversion_options.preferred_chroma_downsampling_algorithm = this.colorConversionOptions.PreferredChromaDownsamplingAlgorithm;
                optionsVersion6->color_conversion_options.preferred_chroma_upsampling_algorithm = this.colorConversionOptions.PreferredChromaUpsamplingAlgorithm;
                optionsVersion6->color_conversion_options.only_use_preferred_chroma_algorithm = this.colorConversionOptions.UseOnlyPreferredChromaAlgorithm.ToByte();
            }
            else if (options->version == 5)
            {
                var optionsVersion5 = (EncodingOptionsVersion5*)encodingOptions.DangerousGetHandle();

                optionsVersion5->macOS_compatibility_workaround = this.CropWithImageGrid.ToByte();
                optionsVersion5->save_two_colr_boxes_when_ICC_and_nclx_available = this.WriteTwoColorProfiles.ToByte();
                optionsVersion5->macOS_compatibility_workaround_no_nclx_profile = BooleanExtensions.ToByte(!this.WriteNclxColorProfile);
                optionsVersion5->image_orientation = this.ImageOrientation;
            }
            else if (options->version == 4)
            {
                var optionsVersion4 = (EncodingOptionsVersion4*)encodingOptions.DangerousGetHandle();

                optionsVersion4->macOS_compatibility_workaround = this.CropWithImageGrid.ToByte();
                optionsVersion4->save_two_colr_boxes_when_ICC_and_nclx_available = this.WriteTwoColorProfiles.ToByte();
                optionsVersion4->macOS_compatibility_workaround_no_nclx_profile = BooleanExtensions.ToByte(!this.WriteNclxColorProfile);
            }
            else if (options->version == 3)
            {
                var optionsVersion3 = (EncodingOptionsVersion3*)encodingOptions.DangerousGetHandle();

                optionsVersion3->macOS_compatibility_workaround = this.CropWithImageGrid.ToByte();
                optionsVersion3->save_two_colr_boxes_when_ICC_and_nclx_available = this.WriteTwoColorProfiles.ToByte();
            }
            else if (options->version == 2)
            {
                var optionsVersion2 = (EncodingOptionsVersion2*)encodingOptions.DangerousGetHandle();

                optionsVersion2->macOS_compatibility_workaround = this.CropWithImageGrid.ToByte();
            }

            return encodingOptions;
        }
    }
}
