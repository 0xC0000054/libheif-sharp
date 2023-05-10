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
using LibHeifSharp.ResourceManagement;

namespace LibHeifSharp
{
    /// <summary>
    /// Represents a LibHeif image handle.
    /// </summary>
    /// <seealso cref="Disposable" />
    /// <threadsafety static="true" instance="false"/>
    public sealed class HeifImageHandle : Disposable
    {
        private SafeHeifImageHandle imageHandle;
        private HeifIccColorProfile iccColorProfile;
        private HeifNclxColorProfile nclxColorProfile;
        private bool fetchedColorProfilesFromImageHandle;
        private readonly HeifContext.ImageDecodeErrorDelegate decodeErrorHandler;
        private readonly AuxiliaryImageType auxiliaryImageType;
        private readonly object sync;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifImageHandle"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="decodeErrorHandler">The decode error handler.</param>
        /// <exception cref="ArgumentNullException"><paramref name="handle"/> is null.</exception>
        internal HeifImageHandle(SafeHeifImageHandle handle, HeifContext.ImageDecodeErrorDelegate decodeErrorHandler = null)
            : this(handle, decodeErrorHandler, AuxiliaryImageType.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifImageHandle"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="decodeErrorHandler">The decode error handler.</param>
        /// <param name="auxiliaryImageType">The auxiliary image type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="handle"/> is null.</exception>
        private HeifImageHandle(SafeHeifImageHandle handle,
                                HeifContext.ImageDecodeErrorDelegate decodeErrorHandler,
                                AuxiliaryImageType auxiliaryImageType)
        {
            Validate.IsNotNull(handle, nameof(handle));

            this.imageHandle = handle;
            this.iccColorProfile = null;
            this.nclxColorProfile = null;
            this.fetchedColorProfilesFromImageHandle = false;
            this.decodeErrorHandler = decodeErrorHandler;
            this.auxiliaryImageType = auxiliaryImageType;
            this.sync = new object();
        }

        /// <summary>
        /// Gets the image width.
        /// </summary>
        /// <value>
        /// The image width.
        /// </value>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public int Width
        {
            get
            {
                VerifyNotDisposed();

                return LibHeifNative.heif_image_handle_get_width(this.imageHandle);
            }
        }

        /// <summary>
        /// Gets the image height.
        /// </summary>
        /// <value>
        /// The image height.
        /// </value>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public int Height
        {
            get
            {
                VerifyNotDisposed();

                return LibHeifNative.heif_image_handle_get_height(this.imageHandle);
            }
        }

        /// <summary>
        /// Gets the image bit depth.
        /// </summary>
        /// <value>
        /// The image bit depth.
        /// </value>
        /// <exception cref="HeifException">The image has an undefined bit depth.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public int BitDepth
        {
            get
            {
                VerifyNotDisposed();

                int value = LibHeifNative.heif_image_handle_get_luma_bits_per_pixel(this.imageHandle);

                if (value == -1)
                {
                    ExceptionUtil.ThrowHeifException(Resources.ImageUndefinedBitDepth);
                }

                return value;
            }
        }

        /// <summary>
        /// Gets the type of auxiliary image that this instance represents.
        /// </summary>
        /// <value>
        /// The type of auxiliary image that this instance represents.
        /// </value>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public AuxiliaryImageType AuxiliaryImageType
        {
            get
            {
                VerifyNotDisposed();

                return this.auxiliaryImageType;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has an alpha channel.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if this instance has an alpha channel; otherwise, <see langword="false"/>.
        /// </value>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public bool HasAlphaChannel
        {
            get
            {
                VerifyNotDisposed();

                return LibHeifNative.heif_image_handle_has_alpha_channel(this.imageHandle);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has a depth image.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if this instance has a depth image; otherwise, <see langword="false"/>.
        /// </value>
        public bool HasDepthImage
        {
            get
            {
                VerifyNotDisposed();

                return LibHeifNative.heif_image_handle_has_depth_image(this.imageHandle);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the alpha channel is premultiplied.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the alpha channel is premultiplied; otherwise, <see langword="false"/>.
        /// </value>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <remarks>
        /// This property is supported starting with LibHeif 1.12.0, it will return <see langword="false"/>
        /// on earlier versions.
        /// </remarks>
        public bool IsPremultipliedAlpha
        {
            get
            {
                VerifyNotDisposed();

                return LibHeifVersion.Is1Point12OrLater
                       && LibHeifNative.heif_image_handle_has_alpha_channel(this.imageHandle)
                       && LibHeifNative.heif_image_handle_is_premultiplied_alpha(this.imageHandle);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is the primary image.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if this instance is the primary image; otherwise, <see langword="false"/>.
        /// </value>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public bool IsPrimaryImage
        {
            get
            {
                VerifyNotDisposed();

                return LibHeifNative.heif_image_handle_is_primary_image(this.imageHandle);
            }
        }

        /// <summary>
        /// Gets the image handle ICC color profile.
        /// </summary>
        /// <value>
        /// The image handle ICC color profile.
        /// </value>
        /// <exception cref="HeifException">
        /// A LibHeif error occurred.
        ///
        /// -or-
        ///
        /// The color profile type is not supported.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public HeifIccColorProfile IccColorProfile
        {
            get
            {
                VerifyNotDisposed();

                if (!this.fetchedColorProfilesFromImageHandle)
                {
                    lock (this.sync)
                    {
                        if (!this.fetchedColorProfilesFromImageHandle)
                        {
                            CacheColorProfilesWhileLocked();
                            this.fetchedColorProfilesFromImageHandle = true;
                        }
                    }
                }

                return this.iccColorProfile;
            }
        }

        /// <summary>
        /// Gets the image handle NCLX color profile.
        /// </summary>
        /// <value>
        /// The image handle NCLX color profile.
        /// </value>
        /// <exception cref="HeifException">
        /// A LibHeif error occurred.
        ///
        /// -or-
        ///
        /// The color profile type is not supported.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public HeifNclxColorProfile NclxColorProfile
        {
            get
            {
                VerifyNotDisposed();

                if (!this.fetchedColorProfilesFromImageHandle)
                {
                    lock (this.sync)
                    {
                        if (!this.fetchedColorProfilesFromImageHandle)
                        {
                            CacheColorProfilesWhileLocked();
                            this.fetchedColorProfilesFromImageHandle = true;
                        }
                    }
                }

                return this.nclxColorProfile;
            }
        }

        /// <summary>
        /// Gets the image handle.
        /// </summary>
        /// <value>
        /// The safe handle.
        /// </value>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        internal SafeHeifImageHandle SafeHeifImageHandle
        {
            get
            {
                VerifyNotDisposed();

                return this.imageHandle;
            }
        }

        /// <summary>
        /// Gets the height of the image before transformations have been applied.
        /// </summary>
        /// <value>
        /// The height of the image before transformations have been applied.
        /// </value>
        /// <exception cref="HeifException">The ISPE height is undefined.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        internal int UntransformedHeight
        {
            get
            {
                VerifyNotDisposed();

                int height = LibHeifNative.heif_image_handle_get_ispe_height(this.imageHandle);

                if (height == 0)
                {
                    ExceptionUtil.ThrowHeifException(Resources.ImageUndefinedIspeHeight);
                }

                return height;
            }
        }

        /// <summary>
        /// Gets the width of the image before transformations have been applied.
        /// </summary>
        /// <value>
        /// The width of the image before transformations have been applied.
        /// </value>
        /// <exception cref="HeifException">The ISPE width is undefined.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        internal int UntransformedWidth
        {
            get
            {
                VerifyNotDisposed();

                int width = LibHeifNative.heif_image_handle_get_ispe_width(this.imageHandle);

                if (width == 0)
                {
                    ExceptionUtil.ThrowHeifException(Resources.ImageUndefinedIspeWidth);
                }

                return width;
            }
        }

        /// <summary>
        /// Adds a region to the image handle.
        /// </summary>
        /// <param name="referenceWidth">The reference width.</param>
        /// <param name="referenceHeight">The reference height.</param>
        /// <returns>
        /// The region item.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="referenceWidth"/> must be in the range of [0, 4294967295].
        /// -or-
        /// <paramref name="referenceHeight"/> must be in the range of [0, 4294967295].
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public HeifRegionItem AddRegion(long referenceWidth, long referenceHeight)
        {
            Validate.IsInRange(referenceWidth, nameof(referenceWidth), uint.MinValue, uint.MaxValue);
            Validate.IsInRange(referenceHeight, nameof(referenceHeight), uint.MinValue, uint.MaxValue);

            VerifyNotDisposed();

            if (LibHeifVersion.Is1Point16OrLater)
            {
                var error = LibHeifNative.heif_image_handle_add_region_item(this.imageHandle,
                                                                                (uint)referenceWidth,
                                                                                (uint)referenceHeight,
                                                                                out var regionItem);
                error.ThrowIfError();

                var imageHandleId = LibHeifNative.heif_image_handle_get_item_id(this.imageHandle);

                return new HeifRegionItem(regionItem, imageHandleId, referenceWidth, referenceHeight);
            }
            else
            {
                throw new HeifException(Resources.RegionAPINotSupported);
            }
        }

        /// <summary>
        /// Decodes this instance to a <see cref="HeifImage"/>.
        /// </summary>
        /// <param name="colorspace">The destination image color space.</param>
        /// <param name="chroma">The chroma.</param>
        /// <param name="options">The decoding options.</param>
        /// <returns>The decoded image.</returns>
        /// <exception cref="HeifException">
        /// A LibHeif error occurred.
        ///
        /// -or-
        ///
        /// The color profile type is not supported.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public HeifImage Decode(HeifColorspace colorspace, HeifChroma chroma, HeifDecodingOptions options = null)
        {
            VerifyNotDisposed();

            HeifImage image = null;
            SafeHeifImage safeHeifImage = null;

            try
            {
                heif_error error;

                if (options != null)
                {
                    using (var nativeOptions = options.CreateDecodingOptions())
                    {
                        error = LibHeifNative.heif_decode_image(this.imageHandle,
                                                                out safeHeifImage,
                                                                colorspace,
                                                                chroma,
                                                                nativeOptions.DecodingOptions);
                    }
                }
                else
                {
                    error = LibHeifNative.heif_decode_image(this.imageHandle,
                                                            out safeHeifImage,
                                                            colorspace,
                                                            chroma,
                                                            IntPtr.Zero);
                }

                if (error.IsError)
                {
                    if (this.decodeErrorHandler != null)
                    {
                        this.decodeErrorHandler.Invoke(error);
                    }
                    else
                    {
                        error.ThrowIfError();
                    }
                }

                // Passing the image handle width and height works around a bug with the
                // heif_image_get_primary_height method in some versions of libheif.
                image = new HeifImage(safeHeifImage,
                                      this.Width,
                                      this.Height,
                                      this.IccColorProfile,
                                      this.NclxColorProfile);
                safeHeifImage = null;
            }
            finally
            {
                safeHeifImage?.Dispose();
            }

            return image;
        }

        /// <summary>
        /// Gets the auxiliary image handle.
        /// </summary>
        /// <param name="id">The auxiliary image id.</param>
        /// <returns>The auxiliary image handle.</returns>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <seealso cref="GetAuxiliaryImageIds"/>
        public HeifImageHandle GetAuxiliaryImage(HeifItemId id)
        {
            VerifyNotDisposed();

            if (LibHeifVersion.Is1Point11OrLater)
            {
                HeifImageHandle aux = null;
                SafeHeifImageHandle auxSafeHandle = null;

                try
                {
                    var error = LibHeifNative.heif_image_handle_get_auxiliary_image_handle(this.imageHandle, id, out auxSafeHandle);
                    error.ThrowIfError();

                    aux = new HeifImageHandle(auxSafeHandle, this.decodeErrorHandler, AuxiliaryImageType.VendorSpecific);
                    auxSafeHandle = null;
                }
                finally
                {
                    auxSafeHandle?.Dispose();
                }

                return aux;
            }
            else
            {
                throw new HeifException(Resources.AuxiliaryImageAPINotSupported);
            }
        }

        /// <summary>
        /// Gets a list of the auxiliary image ids.
        /// </summary>
        /// <returns>A list of the auxiliary image ids.</returns>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <remarks>
        /// <para>The alpha and/or depth images are omitted from this list.</para>
        /// <para>
        /// LibHeif will include the alpha image (if present) when you call <see cref="Decode(HeifColorspace, HeifChroma, HeifDecodingOptions)"/>
        /// with the <see cref="HeifChroma"/> parameter set to one of the <c>InterleavedRgba</c> values.
        /// </para>
        /// <para>
        /// To read the depth images use <see cref="GetDepthImageIds"/> to get a list of the depth image item ids and
        /// <see cref="GetDepthImage(HeifItemId)"/> to convert the item id to a <see cref="HeifImageHandle"/>.
        /// </para>
        /// <para>
        /// This method is supported starting with LibHeif version 1.11.0, it will return an empty collection on older versions.
        /// </para>
        /// </remarks>
        /// <seealso cref="GetAuxiliaryImage(HeifItemId)"/>
        /// <seealso cref="GetAuxiliaryType"/>
        public IReadOnlyList<HeifItemId> GetAuxiliaryImageIds()
        {
            VerifyNotDisposed();

            if (LibHeifVersion.Is1Point11OrLater)
            {
                const heif_auxiliary_image_filter filter = heif_auxiliary_image_filter.OmitAlpha | heif_auxiliary_image_filter.OmitDepth;

                int count = LibHeifNative.heif_image_handle_get_number_of_auxiliary_images(this.imageHandle, filter);

                if (count == 0)
                {
                    return Array.Empty<HeifItemId>();
                }

                var ids = new HeifItemId[count];

                unsafe
                {
                    fixed (HeifItemId* ptr = ids)
                    {
                        int filledCount = LibHeifNative.heif_image_handle_get_list_of_auxiliary_image_IDs(this.imageHandle,
                                                                                                          filter,
                                                                                                          ptr,
                                                                                                          count);
                        if (filledCount != count)
                        {
                            ExceptionUtil.ThrowHeifException(Resources.CannotGetAllAuxillaryImages);
                        }
                    }
                }

                return ids;
            }
            else
            {
                return Array.Empty<HeifItemId>();
            }
        }

        /// <summary>
        /// Gets the type identifier for the auxiliary image.
        /// </summary>
        /// <returns>The type identifier for the auxiliary image.</returns>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public string GetAuxiliaryType()
        {
            VerifyNotDisposed();

            string type = string.Empty;

            if (this.auxiliaryImageType == AuxiliaryImageType.VendorSpecific)
            {
                var error = LibHeifNative.heif_image_handle_get_auxiliary_type(this.imageHandle,
                                                                               out var typePtr);
                error.ThrowIfError();

                try
                {
                    type = typePtr.GetStringValue();
                }
                finally
                {
                    LibHeifNative.heif_image_handle_free_auxiliary_types(this.imageHandle,
                                                                         ref typePtr);
                }
            }

            return type;
        }

        /// <summary>
        /// Gets the depth images.
        /// </summary>
        /// <param name="id">The depth image id.</param>
        /// <returns>The depth image handle.</returns>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <seealso cref="GetDepthImageIds"/>
        public HeifImageHandle GetDepthImage(HeifItemId id)
        {
            VerifyNotDisposed();

            HeifImageHandle depth = null;
            SafeHeifImageHandle depthSafeHandle = null;

            try
            {
                var error = LibHeifNative.heif_image_handle_get_depth_image_handle(this.imageHandle, id, out depthSafeHandle);
                error.ThrowIfError();

                depth = new HeifImageHandle(depthSafeHandle, this.decodeErrorHandler, AuxiliaryImageType.Depth);
                depthSafeHandle = null;
            }
            finally
            {
                depthSafeHandle?.Dispose();
            }

            return depth;
        }

        /// <summary>
        /// Gets a list of the depth image ids.
        /// </summary>
        /// <returns>A list of the depth image ids.</returns>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <seealso cref="GetDepthImage(HeifItemId)"/>
        public IReadOnlyList<HeifItemId> GetDepthImageIds()
        {
            VerifyNotDisposed();

            int count = LibHeifNative.heif_image_handle_get_number_of_depth_images(this.imageHandle);

            if (count == 0)
            {
                return Array.Empty<HeifItemId>();
            }

            var ids = new HeifItemId[count];

            unsafe
            {
                fixed (HeifItemId* ptr = ids)
                {
                    int filledCount = LibHeifNative.heif_image_handle_get_list_of_depth_image_IDs(this.imageHandle,
                                                                                                  ptr,
                                                                                                  count);
                    if (filledCount != count)
                    {
                        ExceptionUtil.ThrowHeifException(Resources.CannotGetAllMetadataBlockIds);
                    }
                }
            }

            return ids;
        }

        /// <summary>
        /// Gets the depth representation information.
        /// </summary>
        /// <param name="id">The depth image id.</param>
        /// <returns>The depth representation information.</returns>
        /// <exception cref="HeifException">
        /// A LibHeif error occurred.
        ///
        /// -or-
        ///
        /// The non-linear representation model is larger than 2 GB.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public HeifDepthRepresentationInfo GetDepthRepresentationInfo(HeifItemId id)
        {
            VerifyNotDisposed();

            HeifDepthRepresentationInfo depthRepresentationInfo = null;
            SafeDepthRepresentationInfo safeDepthRepresentationInfo = null;

            try
            {
                if (LibHeifNative.heif_image_handle_get_depth_image_representation_info(this.imageHandle,
                                                                                        id,
                                                                                        out safeDepthRepresentationInfo))
                {
                    unsafe
                    {
                        var info = (heif_depth_representation_info*)safeDepthRepresentationInfo.DangerousGetHandle();

                        depthRepresentationInfo = new HeifDepthRepresentationInfo(info);
                    }
                }
            }
            finally
            {
                safeDepthRepresentationInfo?.Dispose();
            }

            return depthRepresentationInfo;
        }

        /// <summary>
        /// Gets the EXIF meta-data.
        /// </summary>
        /// <returns>The EXIF meta-data bytes.</returns>
        /// <exception cref="HeifException">
        /// A LibHeif error occurred.
        ///
        /// -or-
        ///
        /// The meta-data block is larger than 2 GB.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <remarks>
        /// If the image contains more than one EXIF block, this method will only return the first one.
        /// </remarks>
        public byte[] GetExifMetadata()
        {
            VerifyNotDisposed();

            var exifBlockIds = GetMetadataBlockIds("Exif");

            if (exifBlockIds.Count == 0)
            {
                return null;
            }

            byte[] exifBlob = GetMetadata(exifBlockIds[0]);

            // The EXIF data block length should always be > 4 because the first 4 bytes
            // specify the offset to the start of the TIFF header as a big-endian Int32 value.
            // See ISO/IEC 23008-12:2017 section A.2.1.
            if (exifBlob is null || exifBlob.Length <= 4)
            {
                return null;
            }

            int tiffHeaderStartOffset = (exifBlob[0] << 24) | (exifBlob[1] << 16) | (exifBlob[2] << 8) | exifBlob[3];

            int startIndex = 4 + tiffHeaderStartOffset;

            // The start index must be within the buffer.
            if ((uint)startIndex >= (uint)exifBlob.Length)
            {
                return null;
            }

            int length = exifBlob.Length - startIndex;

            byte[] exif = new byte[length];
            Array.Copy(exifBlob, startIndex, exif, 0, length);

            return exif;
        }

        /// <summary>
        /// Gets the meta-data bytes.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The meta-data bytes.</returns>
        /// <exception cref="HeifException">
        /// A LibHeif error occurred.
        ///
        /// -or-
        ///
        /// The meta-data block is larger than 2 GB.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <seealso cref="GetMetadataBlockIds(string, string)"/>
        public byte[] GetMetadata(HeifItemId id)
        {
            VerifyNotDisposed();

            ulong size = LibHeifNative.heif_image_handle_get_metadata_size(this.imageHandle, id).ToUInt64();

            if (size == 0)
            {
                return null;
            }

            if (size > int.MaxValue)
            {
                ExceptionUtil.ThrowHeifException(Resources.MetadataBlockLargerThan2Gb);
            }

            byte[] metadata = new byte[size];

            unsafe
            {
                fixed (byte* ptr = metadata)
                {
                    var error = LibHeifNative.heif_image_handle_get_metadata(this.imageHandle, id, ptr);
                    error.ThrowIfError();
                }
            }

            return metadata;
        }

        /// <summary>
        /// Gets the meta-data bytes.
        /// </summary>
        /// <param name="info">The meta-data block information.</param>
        /// <returns>The meta-data bytes.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="info"/> is <see langword="null"/>.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <seealso cref="GetMetadataBlockInfo(HeifItemId)"/>
        public byte[] GetMetadata(HeifMetadataBlockInfo info)
        {
            Validate.IsNotNull(info, nameof(info));
            VerifyNotDisposed();

            byte[] metadata = null;
            int size = info.Size;

            if (size > 0)
            {
                metadata = new byte[size];
                unsafe
                {
                    fixed (byte* ptr = metadata)
                    {
                        var error = LibHeifNative.heif_image_handle_get_metadata(this.imageHandle,
                                                                                 info.Id,
                                                                                 ptr);
                        error.ThrowIfError();
                    }
                }
            }

            return metadata;
        }

        /// <summary>
        /// Gets the meta-data information.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The meta-data information.</returns>
        /// <exception cref="HeifException">
        /// A LibHeif error occurred.
        ///
        /// -or-
        ///
        /// The meta-data block is larger than 2 GB.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <seealso cref="GetMetadataBlockIds(string, string)"/>
        public HeifMetadataBlockInfo GetMetadataBlockInfo(HeifItemId id)
        {
            VerifyNotDisposed();

            string itemType = LibHeifNative.heif_image_handle_get_metadata_type(this.imageHandle, id);
            string contentType = LibHeifNative.heif_image_handle_get_metadata_content_type(this.imageHandle, id);
            ulong size = LibHeifNative.heif_image_handle_get_metadata_size(this.imageHandle, id).ToUInt64();

            if (size > int.MaxValue)
            {
                ExceptionUtil.ThrowHeifException(Resources.MetadataBlockLargerThan2Gb);
            }

            return new HeifMetadataBlockInfo(id, itemType, contentType, (int)size);
        }

        /// <summary>
        /// Gets a list of the meta-data block ids.
        /// </summary>
        /// <param name="type">The meta-data block type.</param>
        /// <param name="contentType">The meta-data block content type.</param>
        /// <returns>A list of the meta-data block ids.</returns>
        /// <exception cref="HeifException">Could not get all of the meta-data block ids.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <seealso cref="GetMetadata(HeifItemId)"/>
        /// <seealso cref="GetMetadataBlockInfo(HeifItemId)"/>
        public IReadOnlyList<HeifItemId> GetMetadataBlockIds(string type = null, string contentType = null)
        {
            VerifyNotDisposed();

            int count = LibHeifNative.heif_image_handle_get_number_of_metadata_blocks(this.imageHandle, type);

            if (count == 0)
            {
                return Array.Empty<HeifItemId>();
            }

            var ids = new HeifItemId[count];

            unsafe
            {
                fixed (HeifItemId* ptr = ids)
                {
                    int filledCount = LibHeifNative.heif_image_handle_get_list_of_metadata_block_IDs(this.imageHandle,
                                                                                                     type,
                                                                                                     ptr,
                                                                                                     count);
                    if (filledCount != count)
                    {
                        ExceptionUtil.ThrowHeifException(Resources.CannotGetAllMetadataBlockIds);
                    }
                }
            }

            // The type must be defined in order to filter by content type.
            if (type != null && contentType != null)
            {
                var matchingItems = new List<HeifItemId>();

                for (int i = 0; i < ids.Length; i++)
                {
                    var id = ids[i];
                    string metadataContentType = LibHeifNative.heif_image_handle_get_metadata_content_type(this.imageHandle, id);

                    if (contentType.Equals(metadataContentType, StringComparison.Ordinal))
                    {
                        matchingItems.Add(id);
                    }
                }

                return matchingItems;
            }
            else
            {
                return ids;
            }
        }

        /// <summary>
        /// Gets a list of the region item ids.
        /// </summary>
        /// <returns>A list of the region item ids.</returns>
        /// <exception cref="HeifException">Could not get all of the region item ids.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <remarks>
        /// This method is supported starting with LibHeif version 1.16.0, it will return an empty collection on older versions.
        /// </remarks>
        /// <seealso cref="HeifContext.GetRegionItem(HeifRegionItemId)"/>
        public IReadOnlyList<HeifRegionItemId> GetRegionItemIds()
        {
            VerifyNotDisposed();

            var ids = Array.Empty<HeifRegionItemId>();

            if (LibHeifVersion.Is1Point16OrLater)
            {
                int count = LibHeifNative.heif_image_handle_get_number_of_region_items(this.imageHandle);

                if (count > 0)
                {
                    var regionItemIds = new HeifItemId[count];

                    unsafe
                    {
                        fixed (HeifItemId* ptr = regionItemIds)
                        {
                            int filledCount = LibHeifNative.heif_image_handle_get_list_of_region_item_ids(this.imageHandle,
                                                                                                          ptr,
                                                                                                          count);

                            if (filledCount != count)
                            {
                                ExceptionUtil.ThrowHeifException(Resources.CannotGetAllRegionItemIds);
                            }
                        }
                    }

                    var imageHandleId = LibHeifNative.heif_image_handle_get_item_id(this.imageHandle);

                    ids = new HeifRegionItemId[count];

                    for (int i = 0; i < ids.Length; i++)
                    {
                        ids[i] = new HeifRegionItemId(imageHandleId, regionItemIds[i]);
                    }
                }
            }

            return ids;
        }

        /// <summary>
        /// Gets the thumbnail image handle.
        /// </summary>
        /// <param name="id">The thumbnail image id.</param>
        /// <returns>The thumbnail image handle.</returns>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <seealso cref="GetThumbnailImageIds"/>
        public HeifImageHandle GetThumbnailImage(HeifItemId id)
        {
            VerifyNotDisposed();

            HeifImageHandle thumbnail = null;
            SafeHeifImageHandle thumbnailSafeHandle = null;

            try
            {
                var error = LibHeifNative.heif_image_handle_get_thumbnail(this.imageHandle, id, out thumbnailSafeHandle);
                error.ThrowIfError();

                thumbnail = new HeifImageHandle(thumbnailSafeHandle, this.decodeErrorHandler, AuxiliaryImageType.Thumbnail);
                thumbnailSafeHandle = null;
            }
            finally
            {
                thumbnailSafeHandle?.Dispose();
            }

            return thumbnail;
        }

        /// <summary>
        /// Gets a list of the thumbnail image ids.
        /// </summary>
        /// <returns>A list of the thumbnail image ids.</returns>
        /// <exception cref="HeifException">Could not get all of the thumbnail image ids.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <seealso cref="GetThumbnailImage(HeifItemId)"/>
        public IReadOnlyList<HeifItemId> GetThumbnailImageIds()
        {
            VerifyNotDisposed();

            int count = LibHeifNative.heif_image_handle_get_number_of_thumbnails(this.imageHandle);

            if (count == 0)
            {
                return Array.Empty<HeifItemId>();
            }

            var ids = new HeifItemId[count];

            unsafe
            {
                fixed (HeifItemId* ptr = ids)
                {
                    int filledCount = LibHeifNative.heif_image_handle_get_list_of_thumbnail_IDs(this.imageHandle,
                                                                                                ptr,
                                                                                                count);
                    if (filledCount != count)
                    {
                        ExceptionUtil.ThrowHeifException(Resources.CannotGetAllThumbnailIds);
                    }
                }
            }

            return ids;
        }

        /// <summary>
        /// Gets the XMP meta-data.
        /// </summary>
        /// <returns>The XMP meta-data bytes.</returns>
        /// <remarks>
        /// If the image contains more than one XMP block, this method will only return the first one.
        /// </remarks>
        /// <exception cref="HeifException">
        /// A LibHeif error occurred.
        ///
        /// -or-
        ///
        /// The meta-data block is larger than 2 GB.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public byte[] GetXmpMetadata()
        {
            VerifyNotDisposed();

            var xmpBlockIds = GetMetadataBlockIds("mime", "application/rdf+xml");

            if (xmpBlockIds.Count == 0)
            {
                return null;
            }

            return GetMetadata(xmpBlockIds[0]);
        }

        /// <summary>
        /// Gets the item identifier.
        /// </summary>
        /// <returns>The item identifier.</returns>
        /// <exception cref="HeifException">This method requires LibHeif version 1.16.0 or later.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        internal HeifItemId GetItemId()
        {
            VerifyNotDisposed();

            if (!LibHeifVersion.Is1Point16OrLater)
            {
                ExceptionUtil.ThrowHeifException(Resources.PropertyAPIsNotSupported);
            }

            return LibHeifNative.heif_image_handle_get_item_id(this.imageHandle);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposableUtil.Free(ref this.imageHandle);
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Caches the image handle color profiles.
        /// </summary>
        /// <exception cref="HeifException">
        /// The color profile type is not supported.
        ///
        /// -or-
        ///
        /// A LibHeif error occurred.
        /// </exception>
        private unsafe void CacheColorProfilesWhileLocked()
        {
            if (LibHeifVersion.Is1Point10OrLater)
            {
                this.iccColorProfile = HeifIccColorProfile.TryCreate(this.imageHandle);
                this.nclxColorProfile = HeifNclxColorProfile.TryCreate(this.imageHandle);
            }
            else
            {
                // LibHeif versions prior to 1.10.0 only support one color profile per image.
                var colorProfileType = LibHeifNative.heif_image_handle_get_color_profile_type(this.imageHandle);

                switch (colorProfileType)
                {
                    case heif_color_profile_type.None:
                        this.iccColorProfile = null;
                        this.nclxColorProfile = null;
                        break;
                    case heif_color_profile_type.Nclx:
                        this.iccColorProfile = null;
                        this.nclxColorProfile = HeifNclxColorProfile.TryCreate(this.imageHandle);
                        break;
                    case heif_color_profile_type.IccProfile:
                    case heif_color_profile_type.RestrictedIcc:
                        this.iccColorProfile = HeifIccColorProfile.TryCreate(this.imageHandle);
                        this.nclxColorProfile = null;
                        break;
                    default:
                        throw new HeifException(Resources.ColorProfileTypeNotSupported);
                }
            }
        }
    }
}
