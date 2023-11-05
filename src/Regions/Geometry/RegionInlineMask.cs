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
using System.Globalization;
using LibHeifSharp.Interop;
using LibHeifSharp.Properties;

namespace LibHeifSharp
{
    /// <summary>
    /// The inline mask region geometry.
    /// </summary>
    /// <seealso cref="RegionGeometry" />
    public sealed class RegionInlineMask : RegionGeometry
    {
        private readonly byte[] maskData;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionInlineMask"/> class.
        /// </summary>
        /// <param name="region">The region.</param>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        internal unsafe RegionInlineMask(heif_region region) : base(RegionGeometryType.InlineMask)
        {
            if (!LibHeifVersion.Is1Point17OrLater)
            {
                ExceptionUtil.ThrowHeifException(Resources.RegionMaskAPINotSupported);
            }

            ulong maskDataSize = LibHeifNative.heif_region_get_inline_mask_data_len(region).ToUInt64();

            if (maskDataSize == 0)
            {
                ExceptionUtil.ThrowHeifException(Resources.CannotGetInlineMaskDataSize);
            }
            else if (maskDataSize > int.MaxValue)
            {
                ExceptionUtil.ThrowHeifException(Resources.InlineMaskDataLargerThan2GB);
            }
            
            this.maskData = new byte[maskDataSize];
            int x;
            int y;
            uint width;
            uint height;

            fixed (byte* ptr = this.maskData)
            {
                var error = LibHeifNative.heif_region_get_inline_mask_data(region, out x, out y, out width, out height, ptr);
                error.ThrowIfError();
            }

            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

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
        /// Creates the inline mask data from the specified image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>The inline mask data array.</returns>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="OutOfMemoryException">
        /// Insufficient memory to create the inline mask data array.
        /// </exception>
        internal static unsafe byte[] MaskDataFromImage(HeifImage image)
        {
            Validate.IsNotNull(image, nameof(image));

            if (!image.HasChannel(HeifChannel.Y))
            {
                ExceptionUtil.ThrowHeifException(Resources.InlineMaskImageMustHaveYChannel);
            }
            else if (LibHeifNative.heif_image_get_bits_per_pixel_range(image.SafeHeifImage, HeifChannel.Y) != 8)
            {
                ExceptionUtil.ThrowHeifException(Resources.InlineMaskImageMustBeEightBitsPerChannel);
            }

            ulong dataLength = (((ulong)image.Width * (ulong)image.Height) + 7) / 8;

            byte[] data = new byte[dataLength];

            var planeData = image.GetPlane(HeifChannel.Y);

            byte* scan0 = (byte*)planeData.Scan0;
            int stride = planeData.Stride;
            ulong pixelIndex = 0;

            for (int y = 0; y < planeData.Height; y++)
            {
                byte* src = scan0 + ((long)y * stride);

                for (int x = 0; x < planeData.Width; x++)
                {
                    int byteIndex = (int)(pixelIndex >> 3);
                    int shift = (int)(pixelIndex & 7);

                    // Use the high order bit as the binary mask value.
                    // Pixels >= 128 will be selected/active, and pixels < 128 will be unselected/inactive.
                    int maskBit = src[x] & 128;

                    data[byteIndex] |= (byte)(maskBit >> shift);
                    pixelIndex++;
                }
            }

            return data;
        }

        /// <summary>
        /// Gets the mask data.
        /// </summary>
        /// <returns>The mask data.</returns>
        /// <remarks>
        /// <para>
        /// The mask data format is one bit per pixel, most significant bit first pixel, no padding.
        /// If the bit value is 1, the corresponding pixel is part of the region. If the bit value is 0,
        /// the corresponding pixel is not part of the region.
        /// </para>
        /// </remarks>
        /// <seealso cref="GetMaskImage"/>
        public byte[] GetMaskData()
        {
            byte[] data = new byte[this.maskData.Length];
            this.maskData.CopyTo(data, 0);           

            return data;
        }

        /// <summary>
        /// Gets the region mask as a <see cref="HeifImage"/>.
        /// </summary>
        /// <returns>
        /// The region mask image.
        /// </returns>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <seealso cref="GetMaskData"/>
        public HeifImage GetMaskImage()
        {
            if (this.Width > int.MaxValue || this.Height > int.MaxValue)
            {
                ExceptionUtil.ThrowHeifException(string.Format(CultureInfo.CurrentCulture,
                                                               Resources.RegionMaskBitmapTooLargeFormat,
                                                               this.Width,
                                                               this.Height));
            }

            var image = new HeifImage((int)this.Width, (int)this.Height, HeifColorspace.Monochrome, HeifChroma.Monochrome);

            image.AddPlane(HeifChannel.Y, image.Width, image.Height, 8);

            unsafe
            {
                var planeData = image.GetPlane(HeifChannel.Y);

                byte* scan0 = (byte*)planeData.Scan0;
                int stride = planeData.Stride;
                ulong pixelIndex = 0;

                for (int y = 0; y < planeData.Height; y++)
                {
                    byte* dst = scan0 + ((long)y * stride);

                    for (int x = 0; x < planeData.Width; x++)
                    {
                        int byteIndex = (int)(pixelIndex >> 3);
                        int shift = (int)(pixelIndex & 7);

                        int mask = 128 >> shift;

                        dst[x] = (byte)((this.maskData[byteIndex] & mask) != 0 ? 255 : 0);
                        pixelIndex++;
                    }
                }
            }

            return image;
        }

        /// <inheritdoc/>
        public override string ToString()
            => $"inline mask [x={this.X}, y={this.Y}, width={this.Width}, height={this.Height}, data length={this.maskData.Length}]";
    }
}
