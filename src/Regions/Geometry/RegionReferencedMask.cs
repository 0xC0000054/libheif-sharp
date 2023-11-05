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
    /// The referenced mask region geometry.
    /// </summary>
    /// <seealso cref="RegionGeometry" />
    public sealed class RegionReferencedMask : RegionGeometry
    {
        private readonly IHeifContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionReferencedMask"/> class.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <param name="context">The parent file context.</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        internal RegionReferencedMask(heif_region region, IHeifContext context) : base(RegionGeometryType.ReferencedMask)
        {
            Validate.IsNotNull(context, nameof(context));

            if (!LibHeifVersion.Is1Point17OrLater)
            {
                ExceptionUtil.ThrowHeifException(Resources.RegionMaskAPINotSupported);
            }

            var error = LibHeifNative.heif_region_get_referenced_mask_ID(region,
                                                                         out int x,
                                                                         out int y,
                                                                         out uint width,
                                                                         out uint height,
                                                                         out var itemId);
            error.ThrowIfError();

            this.context = context;
            this.ItemId = itemId;
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Gets the item id of the mask image handle.
        /// </summary>
        /// <value>
        /// The item id of the mask image handle.
        /// </value>
        public HeifItemId ItemId { get; }

        /// <summary>
        /// Gets the x coordinate.
        /// </summary>
        /// <value>
        /// The x coordinate.
        /// </value>
        public int X { get; }

        /// <summary>
        /// Gets the y coordinate.
        /// </summary>
        /// <value>
        /// The y coordinate.
        /// </value>
        public int Y { get; }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public long Width { get; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public long Height { get; }

        /// <summary>
        /// Gets the region mask as a <see cref="HeifImage"/>.
        /// </summary>
        /// <returns>
        /// The region mask image.
        /// </returns>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The parent <see cref="HeifContext"/> has been disposed.</exception>
        public HeifImage GetMaskImage()
        {
            try
            {
                using (var imageHandle = this.context.GetImageHandle(this.ItemId))
                {
                    return imageHandle.Decode(HeifColorspace.Monochrome, HeifChroma.Monochrome);
                }
            }
            catch (Exception ex) when (!(ex is HeifException || ex is ObjectDisposedException))
            {
                throw new HeifException(ex.Message, ex);
            }
        }

        /// <inheritdoc/>
        public override string ToString()
            => $"referenced mask [x={this.X}, y={this.Y}, width={this.Width}, height={this.Height}, item id={this.ItemId}]";
    }
}
