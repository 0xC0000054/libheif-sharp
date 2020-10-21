/*
 * .NET bindings for libheif.
 * Copyright (c) 2020 Nicholas Hayes
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
        private readonly HeifContext.ImageDecodeErrorDelegate decodeErrorHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifImageHandle"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="decodeErrorHandler">The decode error handler.</param>
        /// <exception cref="ArgumentNullException"><paramref name="handle"/> is null.</exception>
        internal HeifImageHandle(SafeHeifImageHandle handle, HeifContext.ImageDecodeErrorDelegate decodeErrorHandler = null)
        {
            if (handle is null)
            {
                ExceptionUtil.ThrowArgumentNullException(nameof(handle));
            }

            this.imageHandle = handle;
            this.decodeErrorHandler = decodeErrorHandler;
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

            var imageHandleColorProfile = GetImageHandleColorProfile();

            try
            {
                heif_error error;

                if (options != null)
                {
                    using (var safeHeifDecodingOptions = options.CreateDecodingOptions())
                    {
                        error = LibHeifNative.heif_decode_image(this.imageHandle,
                                                                out safeHeifImage,
                                                                colorspace,
                                                                chroma,
                                                                safeHeifDecodingOptions);
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
                                      imageHandleColorProfile);
                safeHeifImage = null;
            }
            finally
            {
                safeHeifImage?.Dispose();
            }

            return image;
        }

        /// <summary>
        /// Gets the depth images.
        /// </summary>
        /// <param name="id">The depth image id.</param>
        /// <returns>The meta-data bytes.</returns>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public HeifImageHandle GetDepthImage(HeifItemId id)
        {
            VerifyNotDisposed();

            HeifImageHandle depth = null;
            SafeHeifImageHandle depthSafeHandle = null;

            try
            {
                var error = LibHeifNative.heif_image_handle_get_depth_image_handle(this.imageHandle, id, out depthSafeHandle);
                error.ThrowIfError();

                depth = new HeifImageHandle(depthSafeHandle, this.decodeErrorHandler);
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
        public IReadOnlyList<HeifItemId> GetDepthImageIds()
        {
            VerifyNotDisposed();

            int count = LibHeifNative.heif_image_handle_get_number_of_depth_images(this.imageHandle);

            if (count == 0)
            {
                return Array.Empty<HeifItemId>();
            }

            HeifItemId[] ids = new HeifItemId[count];

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
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
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

            int startIndex = checked(4 + tiffHeaderStartOffset);
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
        /// Gets a list of the meta-data block ids.
        /// </summary>
        /// <param name="type">The meta-data block type.</param>
        /// <param name="contentType">The meta-data block content type.</param>
        /// <returns>A list of the meta-data block ids.</returns>
        /// <exception cref="HeifException">Could not get all of the meta-data block ids.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public IReadOnlyList<HeifItemId> GetMetadataBlockIds(string type = null, string contentType = null)
        {
            VerifyNotDisposed();

            int count = LibHeifNative.heif_image_handle_get_number_of_metadata_blocks(this.imageHandle, type);

            if (count == 0)
            {
                return Array.Empty<HeifItemId>();
            }


            HeifItemId[] ids = new HeifItemId[count];

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
                    HeifItemId id = ids[i];
                    var metadataContentType = LibHeifNative.heif_image_handle_get_metadata_content_type(this.imageHandle, id);

                    if (contentType.Equals(metadataContentType.GetStringValue(), StringComparison.Ordinal))
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
        /// Gets the thumbnail image handle.
        /// </summary>
        /// <param name="id">The thumbnail image id.</param>
        /// <returns>The thumbnail image handle.</returns>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public HeifImageHandle GetThumbnailImage(HeifItemId id)
        {
            VerifyNotDisposed();

            HeifImageHandle thumbnail = null;
            SafeHeifImageHandle thumbnailSafeHandle = null;

            try
            {
                var error = LibHeifNative.heif_image_handle_get_thumbnail(this.imageHandle, id, out thumbnailSafeHandle);
                error.ThrowIfError();

                thumbnail = new HeifImageHandle(thumbnailSafeHandle, this.decodeErrorHandler);
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
        public IReadOnlyList<HeifItemId> GetThumbnailImageIds()
        {
            VerifyNotDisposed();

            int count = LibHeifNative.heif_image_handle_get_number_of_thumbnails(this.imageHandle);

            if (count == 0)
            {
                return Array.Empty<HeifItemId>();
            }


            HeifItemId[] ids = new HeifItemId[count];

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
        /// Gets image handle color profile.
        /// </summary>
        /// <returns>The image handle color profile.</returns>
        /// <exception cref="HeifException">
        /// The color profile type is not supported.
        ///
        /// -or-
        ///
        /// A LibHeif error occurred.
        /// </exception>
        private unsafe HeifColorProfile GetImageHandleColorProfile()
        {
            HeifColorProfile profile = null;

            var colorProfileType = LibHeifNative.heif_image_handle_get_color_profile_type(this.imageHandle);

            switch (colorProfileType)
            {
                case heif_color_profile_type.None:
                    break;
                case heif_color_profile_type.Nclx:
                    profile = new HeifNclxColorProfile(this.imageHandle);
                    break;
                case heif_color_profile_type.IccProfile:
                case heif_color_profile_type.RestrictedIcc:
                    profile = new HeifIccColorProfile(this.imageHandle);
                    break;
                default:
                    throw new HeifException(Resources.ColorProfileTypeNotSupported);
            }

            return profile;
        }
    }
}
