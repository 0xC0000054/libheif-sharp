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

namespace LibHeifSharp
{
    /// <summary>
    /// The LibHeif color conversion options.
    /// </summary>
    /// <seealso cref="HeifDecodingOptions.ColorConversionOptions"/>
    public sealed class HeifColorConversionOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeifColorConversionOptions"/> class.
        /// </summary>
        public HeifColorConversionOptions()
        {
            this.PreferredChromaDownsamplingAlgorithm = HeifChromaDownsamplingAlgorithm.Average;
            this.PreferredChromaUpsamplingAlgorithm = HeifChromaUpsamplingAlgorithm.Bilinear;
            this.UseOnlyPreferredChromaAlgorithm = false;
        }

        /// <summary>
        /// Gets or sets the preferred chroma down-sampling algorithm.
        /// </summary>
        /// <value>
        /// The preferred chroma down-sampling algorithm.
        /// </value>
        public HeifChromaDownsamplingAlgorithm PreferredChromaDownsamplingAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets the preferred chroma up-sampling algorithm.
        /// </summary>
        /// <value>
        /// The preferred chroma up-sampling algorithm.
        /// </value>
        public HeifChromaUpsamplingAlgorithm PreferredChromaUpsamplingAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether only the preferred chroma algorithm should be used.
        /// </summary>
        /// <value>
        /// <see langword="true"/>if only the preferred chroma algorithm should be used; otherwise, <see langword="false"/>.
        /// </value>
        public bool UseOnlyPreferredChromaAlgorithm { get; set; }
    }
}
