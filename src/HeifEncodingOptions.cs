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
    /// The options that can be set when encoding an image.
    /// </summary>
    public sealed class HeifEncodingOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeifEncodingOptions"/> class.
        /// </summary>
        /// <threadsafety static="true" instance="false"/>
        public HeifEncodingOptions()
        {
            this.SaveAlphaChannel = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the alpha channel should be saved.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the alpha channel should be saved; otherwise, <see langword="false"/>.
        /// </value>
        public bool SaveAlphaChannel { get; set; }

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

            return encodingOptions;
        }
    }
}
