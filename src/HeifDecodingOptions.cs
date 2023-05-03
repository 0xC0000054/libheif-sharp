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

using LibHeifSharp.Interop;
using System;

namespace LibHeifSharp
{
    /// <summary>
    /// The options that can be set when decoding an image.
    /// </summary>
    /// <seealso cref="HeifImageHandle.Decode(HeifColorspace, HeifChroma, HeifDecodingOptions)"/>
    /// <threadsafety static="true" instance="false"/>
    public sealed partial class HeifDecodingOptions
    {
        private HeifColorConversionOptions colorConversionOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifDecodingOptions"/> class.
        /// </summary>
        public HeifDecodingOptions()
        {
            this.IgnoreTransformations = false;
            this.ConvertHdrToEightBit = false;
            this.Strict = false;
            this.DecoderId = null;
            this.colorConversionOptions = new HeifColorConversionOptions();
        }

        /// <summary>
        /// Gets or sets a value indicating whether transformations are ignored when decoding the image.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if transformations are ignored when decoding the image; otherwise, <see langword="false"/>.
        /// </value>
        public bool IgnoreTransformations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether high bit-depth images should be converted to 8-bits-per-channel.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if high bit-depth images should be converted to 8-bits-per-channel; otherwise, <see langword="false"/>.
        /// </value>
        public bool ConvertHdrToEightBit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an error is returned for invalid input.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if an error is returned for invalid input; otherwise, <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// This property is supported starting with LibHeif 1.13.0, it is ignored on earlier versions.
        /// </remarks>
        public bool Strict { get; set; }

        /// <summary>
        /// Gets or sets a value that identifies the decoder to use.
        /// </summary>
        /// <value>
        /// One of the <see cref="HeifDecoderDescriptor.IdName"/> values; otherwise, <see langword="null"/> to use the default decoder.
        /// </value>
        /// <remarks>
        /// This property is supported starting with LibHeif 1.15.0, it is ignored on earlier versions.
        /// </remarks>
        /// <seealso cref="LibHeifInfo.GetDecoderDescriptors(HeifCompressionFormat)"/>
        public string DecoderId { get; set; }

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

        internal unsafe NativeOptions CreateDecodingOptions()
        {
            var decodingOptions = LibHeifNative.heif_decoding_options_alloc();
            SafeCoTaskMemHandle safeDecoderId = null;

            if (decodingOptions.IsInvalid)
            {
                ExceptionUtil.ThrowHeifException(Properties.Resources.HeifDecodingOptionsCreationFailed);
            }

            var options = (DecodeOptionsVersion1*)decodingOptions.DangerousGetHandle();
            options->ignore_transformations = this.IgnoreTransformations.ToByte();

            if (options->version >= 5)
            {
                var optionsVersion5 = (DecodeOptionsVersion5*)decodingOptions.DangerousGetHandle();

                optionsVersion5->convert_hdr_to_8bit = this.ConvertHdrToEightBit.ToByte();
                optionsVersion5->strict_decoding = this.Strict.ToByte();

                if (!string.IsNullOrWhiteSpace(this.DecoderId))
                {
                    safeDecoderId = SafeCoTaskMemHandle.FromStringAnsi(this.DecoderId);

                    optionsVersion5->decoder_id = safeDecoderId.DangerousGetHandle();
                }

                optionsVersion5->color_conversion_options.preferred_chroma_downsampling_algorithm = this.colorConversionOptions.PreferredChromaDownsamplingAlgorithm;
                optionsVersion5->color_conversion_options.preferred_chroma_upsampling_algorithm = this.colorConversionOptions.PreferredChromaUpsamplingAlgorithm;
                optionsVersion5->color_conversion_options.only_use_preferred_chroma_algorithm = this.colorConversionOptions.UseOnlyPreferredChromaAlgorithm.ToByte();
            }
            else if (options->version == 4)
            {
                var optionsVersion4 = (DecodeOptionsVersion4*)decodingOptions.DangerousGetHandle();

                optionsVersion4->convert_hdr_to_8bit = this.ConvertHdrToEightBit.ToByte();
                optionsVersion4->strict_decoding = this.Strict.ToByte();

                if (!string.IsNullOrWhiteSpace(this.DecoderId))
                {
                    safeDecoderId = SafeCoTaskMemHandle.FromStringAnsi(this.DecoderId);

                    optionsVersion4->decoder_id = safeDecoderId.DangerousGetHandle();
                }
            }
            else if (options->version == 3)
            {
                var optionsVersion3 = (DecodeOptionsVersion3*)decodingOptions.DangerousGetHandle();

                optionsVersion3->convert_hdr_to_8bit = this.ConvertHdrToEightBit.ToByte();
                optionsVersion3->strict_decoding = this.Strict.ToByte();
            }
            else if (options->version == 2)
            {
                var optionsVersion2 = (DecodeOptionsVersion2*)decodingOptions.DangerousGetHandle();

                optionsVersion2->convert_hdr_to_8bit = this.ConvertHdrToEightBit.ToByte();
            }

            return new NativeOptions(decodingOptions, safeDecoderId);
        }
    }
}
