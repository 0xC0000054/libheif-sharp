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
using System.ComponentModel;
using System.IO;
using LibHeifSharp.Interop;
using LibHeifSharp.Properties;
using LibHeifSharp.ResourceManagement;

namespace LibHeifSharp
{
    /// <summary>
    /// The LibHeif context.
    /// </summary>
    /// <seealso cref="Disposable" />
    /// <threadsafety static="true" instance="false"/>
    public sealed class HeifContext : Disposable, IHeifContext
    {
        private LibHeifInitializationContext initializationContext;
        private SafeHeifContext context;
        private HeifReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifContext"/> class.
        /// </summary>
        /// <exception cref="HeifException">
        /// Unable to create the native HeifContext.
        ///
        /// -or-
        ///
        /// The LibHeif version is not supported.
        /// </exception>
        public HeifContext()
        {
            LibHeifVersion.ThrowIfNotSupported();

            this.initializationContext = new LibHeifInitializationContext();
            try
            {
                this.context = CreateNativeContext();
                this.reader = null;
            }
            catch
            {
                this.initializationContext.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifContext"/> class, with the specified file to read from.
        /// </summary>
        /// <param name="path">The file to read from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="path"/> is empty, contains only whitespace or contains invalid characters.
        /// </exception>
        /// <exception cref="FileNotFoundException">The file specified by <paramref name="path"/> does not exist.</exception>
        /// <exception cref="HeifException">
        /// Unable to create the native HeifContext.
        ///
        /// -or-
        ///
        /// The LibHeif version is not supported.
        ///
        /// -or-
        ///
        /// A LibHeif error occurred.
        /// </exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The access requested is not permitted by the operating system for the specified path.
        /// </exception>
        public HeifContext(string path)
        {
            Validate.IsNotNull(path, nameof(path));
            LibHeifVersion.ThrowIfNotSupported();

            this.initializationContext = new LibHeifInitializationContext();
            try
            {
                this.context = CreateNativeContext();
                this.reader = HeifReaderFactory.CreateFromFile(path);
                InitializeContextFromReader();
            }
            catch
            {
                this.initializationContext.Dispose();
                this.context?.Dispose();
                this.reader?.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifContext"/> class, with the specified byte array to read from.
        /// </summary>
        /// <param name="bytes">A byte array that contains the HEIF image.</param>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="bytes"/> is empty.</exception>
        /// <exception cref="HeifException">
        /// Unable to create the native HeifContext.
        ///
        /// -or-
        ///
        /// The LibHeif version is not supported.
        ///
        /// -or-
        ///
        /// A LibHeif error occurred.
        /// </exception>
        public HeifContext(byte[] bytes)
        {
            Validate.IsNotNullOrEmptyArray(bytes, nameof(bytes));
            LibHeifVersion.ThrowIfNotSupported();

            this.initializationContext = new LibHeifInitializationContext();
            try
            {
                this.context = CreateNativeContext();
                this.reader = HeifReaderFactory.CreateFromMemory(bytes);
                InitializeContextFromReader();
            }
            catch
            {
                this.initializationContext.Dispose();
                this.context?.Dispose();
                this.reader?.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifContext"/> class, with the specified memory to read from.
        /// </summary>
        /// <param name="bytes">A sequence of bytes that contains the HEIF image.</param>
        /// <exception cref="ArgumentException"><paramref name="bytes"/> is empty.</exception>
        /// <exception cref="HeifException">
        /// Unable to create the native HeifContext.
        ///
        /// -or-
        ///
        /// The LibHeif version is not supported.
        ///
        /// -or-
        ///
        /// A LibHeif error occurred.
        /// </exception>
        public HeifContext(ReadOnlyMemory<byte> bytes)
        {
            Validate.IsNotEmpty(bytes, nameof(bytes));
            LibHeifVersion.ThrowIfNotSupported();

            this.initializationContext = new LibHeifInitializationContext();
            try
            {
                this.context = CreateNativeContext();
                this.reader = HeifReaderFactory.CreateFromMemory(bytes);
                InitializeContextFromReader();
            }
            catch
            {
                this.initializationContext.Dispose();
                this.context?.Dispose();
                this.reader?.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifContext"/> class with the specified stream to read from, and optionally leaves the stream open.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="leaveOpen">
        /// <see langword="true"/> to leave the stream open after the <see cref="HeifContext"/> object is disposed; otherwise, <see langword="false"/>.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> must support reading and seeking.</exception>
        /// <exception cref="HeifException">
        /// Unable to create the native HeifContext.
        ///
        /// -or-
        ///
        /// The LibHeif version is not supported.
        ///
        /// -or-
        ///
        /// A LibHeif error occurred.
        /// </exception>
        public HeifContext(Stream stream, bool leaveOpen = false)
        {
            Validate.IsNotNull(stream, nameof(stream));
            LibHeifVersion.ThrowIfNotSupported();

            if (!stream.CanRead || !stream.CanSeek)
            {
                ExceptionUtil.ThrowArgumentException(Resources.StreamCannotReadAndSeek);
            }

            this.initializationContext = new LibHeifInitializationContext();
            try
            {
                this.context = CreateNativeContext();
                this.reader = HeifReaderFactory.CreateFromStream(stream, !leaveOpen);
                InitializeContextFromReader();
            }
            catch (Exception ex)
            {
                this.initializationContext.Dispose();
                this.context?.Dispose();
                this.reader?.Dispose();

                if (ex is ArgumentException || ex is HeifException)
                {
                    throw;
                }
                else
                {
                    throw new HeifException(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// The error callback that is used when decoding the image.
        /// </summary>
        /// <param name="error">The error.</param>
        internal delegate void ImageDecodeErrorDelegate(in heif_error error);

        /// <summary>
        /// Adds EXIF meta-data to the image.
        /// </summary>
        /// <param name="imageHandle">The image handle.</param>
        /// <param name="exif">The EXIF data.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="imageHandle"/> is null.
        ///
        /// -or-
        ///
        /// <paramref name="exif"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="exif"/> is empty.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void AddExifMetadata(HeifImageHandle imageHandle, byte[] exif)
        {
            Validate.IsNotNull(exif, nameof(exif));

            AddExifMetadata(imageHandle, new ReadOnlySpan<byte>(exif));
        }

        /// <summary>
        /// Adds EXIF meta-data to the image.
        /// </summary>
        /// <param name="imageHandle">The image handle.</param>
        /// <param name="exif">The EXIF data.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="imageHandle"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="exif"/> is empty.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void AddExifMetadata(HeifImageHandle imageHandle, ReadOnlySpan<byte> exif)
        {
            Validate.IsNotNull(imageHandle, nameof(imageHandle));
            Validate.IsNotEmpty(exif, nameof(exif));
            VerifyNotDisposed();

            unsafe
            {
                fixed (byte* ptr = exif)
                {
                    var error = LibHeifNative.heif_context_add_exif_metadata(this.context,
                                                                             imageHandle.SafeHeifImageHandle,
                                                                             ptr,
                                                                             exif.Length);
                    error.ThrowIfError();
                }
            }
        }

        /// <summary>
        /// Adds generic meta-data to the image.
        /// </summary>
        /// <param name="imageHandle">The image handle.</param>
        /// <param name="type">The meta-data type.</param>
        /// <param name="data">The generic meta-data.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="imageHandle"/> is null.
        ///
        /// -or-
        ///
        /// <paramref name="type"/> is null.
        ///
        /// -or-
        ///
        /// <paramref name="data"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="type"/> is empty or contains only whitespace characters.
        ///
        /// -or-
        ///
        /// <paramref name="data"/> is empty.
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void AddGenericMetadata(HeifImageHandle imageHandle, string type, byte[] data)
        {
            AddGenericMetadata(imageHandle, type, null, data);
        }

        /// <summary>
        /// Adds generic meta-data to the image.
        /// </summary>
        /// <param name="imageHandle">The image handle.</param>
        /// <param name="type">The meta-data type.</param>
        /// <param name="data">The generic meta-data.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="imageHandle"/> is null.
        ///
        /// -or-
        ///
        /// <paramref name="type"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="type"/> is empty or contains only whitespace characters.
        ///
        /// -or-
        ///
        /// <paramref name="data"/> is empty.
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void AddGenericMetadata(HeifImageHandle imageHandle, string type, ReadOnlySpan<byte> data)
        {
            AddGenericMetadata(imageHandle, type, null, data);
        }

        /// <summary>
        /// Adds generic meta-data to the image.
        /// </summary>
        /// <param name="imageHandle">The image handle.</param>
        /// <param name="type">The meta-data type.</param>
        /// <param name="contentType">The meta-data content type.</param>
        /// <param name="data">The generic meta-data.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="imageHandle"/> is null.
        ///
        /// -or-
        ///
        /// <paramref name="type"/> is null.
        /// -or-
        ///
        /// <paramref name="data"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="type"/> is empty or contains only whitespace characters.
        ///
        /// -or-
        ///
        /// <paramref name="contentType"/> is empty or contains only whitespace characters.
        ///
        /// -or-
        ///
        /// <paramref name="data"/> is empty.
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void AddGenericMetadata(HeifImageHandle imageHandle, string type, string contentType, byte[] data)
        {
            Validate.IsNotNull(data, nameof(data));
            
            AddGenericMetadata(imageHandle, type, contentType, new ReadOnlySpan<byte>(data));
        }

        /// <summary>
        /// Adds generic meta-data to the image.
        /// </summary>
        /// <param name="imageHandle">The image handle.</param>
        /// <param name="type">The meta-data type.</param>
        /// <param name="contentType">The meta-data content type.</param>
        /// <param name="data">The generic meta-data.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="imageHandle"/> is null.
        ///
        /// -or-
        ///
        /// <paramref name="type"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="type"/> is empty or contains only whitespace characters.
        ///
        /// -or-
        ///
        /// <paramref name="contentType"/> is empty or contains only whitespace characters.
        ///
        /// -or-
        ///
        /// <paramref name="data"/> is empty.
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void AddGenericMetadata(HeifImageHandle imageHandle, string type, string contentType, ReadOnlySpan<byte> data)
        {
            Validate.IsNotNull(imageHandle, nameof(imageHandle));
            Validate.IsNotNullOrWhiteSpace(type, nameof(type));
            Validate.IsNotEmptyOrWhiteSpace(contentType, nameof(contentType));
            Validate.IsNotEmpty(data, nameof(data));
            VerifyNotDisposed();

            unsafe
            {
                fixed (byte* ptr = data)
                {
                    var error = LibHeifNative.heif_context_add_generic_metadata(this.context,
                                                                                imageHandle.SafeHeifImageHandle,
                                                                                ptr,
                                                                                data.Length,
                                                                                type,
                                                                                contentType);
                    error.ThrowIfError();
                }
            }
        }

        /// <summary>
        /// Adds an user description property to the specified item.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="userDescription">The user description property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="userDescription"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <remarks>
        /// This method is supported starting with LibHeif version 1.16.0, it will throw an
        /// exception on older versions.
        /// </remarks>
        public void AddUserDescriptionProperty(HeifItemId itemId, UserDescriptionProperty userDescription)
        {
            Validate.IsNotNull(userDescription, nameof(userDescription));

            if (!LibHeifVersion.Is1Point16OrLater)
            {
                ExceptionUtil.ThrowHeifException(Resources.PropertyAPIsNotSupported);
            }

            VerifyNotDisposed();

            using (var data = userDescription.ToNative())
            {
                var nativeProperty = data.CreateNativeStructure();

                unsafe
                {
                    var error = LibHeifNative.heif_item_add_property_user_description(this.context,
                                                                                      itemId,
                                                                                      &nativeProperty,
                                                                                      IntPtr.Zero);
                    error.ThrowIfError();
                }
            }
        }

        /// <summary>
        /// Adds an user description property to the specified image.
        /// </summary>
        /// <param name="imageHandle">The image handle.</param>
        /// <param name="userDescription">The user description.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="imageHandle"/> is <see langword="null"/>.
        /// -or-
        /// <paramref name="userDescription"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <remarks>
        /// This method is supported starting with LibHeif version 1.16.0, it will throw an
        /// exception on older versions.
        /// </remarks>
        public void AddUserDescriptionProperty(HeifImageHandle imageHandle, UserDescriptionProperty userDescription)
        {
            Validate.IsNotNull(imageHandle, nameof(imageHandle));

            AddUserDescriptionProperty(imageHandle.GetItemId(), userDescription);
        }

        /// <summary>
        /// Adds an user description property to the specified region item.
        /// </summary>
        /// <param name="regionItem">The region item.</param>
        /// <param name="userDescription">The user description.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="regionItem"/> is <see langword="null"/>.
        /// -or-
        /// <paramref name="userDescription"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <remarks>
        /// This method is supported starting with LibHeif version 1.16.0, it will throw an
        /// exception on older versions.
        /// </remarks>
        public void AddUserDescriptionProperty(HeifRegionItem regionItem, UserDescriptionProperty userDescription)
        {
            Validate.IsNotNull(regionItem, nameof(regionItem));

            AddUserDescriptionProperty(regionItem.Id, userDescription);
        }

        /// <summary>
        /// Adds XMP meta-data to the image.
        /// </summary>
        /// <param name="imageHandle">The image handle.</param>
        /// <param name="xmp">The XMP data.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="imageHandle"/> is null.
        ///
        /// -or-
        ///
        /// <paramref name="xmp"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException"><paramref name="xmp"/> is empty.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void AddXmpMetadata(HeifImageHandle imageHandle, byte[] xmp)
        {
            Validate.IsNotNull(xmp, nameof(xmp));

            AddXmpMetadata(imageHandle, new ReadOnlySpan<byte>(xmp));
        }

        /// <summary>
        /// Adds XMP meta-data to the image.
        /// </summary>
        /// <param name="imageHandle">The image handle.</param>
        /// <param name="xmp">The XMP data.</param>
        /// <exception cref="ArgumentNullException"><paramref name="imageHandle"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="xmp"/> is empty.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void AddXmpMetadata(HeifImageHandle imageHandle, ReadOnlySpan<byte> xmp)
        {
            Validate.IsNotNull(imageHandle, nameof(imageHandle));
            Validate.IsNotEmpty(xmp, nameof(xmp));
            VerifyNotDisposed();

            unsafe
            {
                fixed (byte* ptr = xmp)
                {
                    var error = LibHeifNative.heif_context_add_XMP_metadata(this.context,
                                                                            imageHandle.SafeHeifImageHandle,
                                                                            ptr,
                                                                            xmp.Length);
                    error.ThrowIfError();
                }
            }
        }

        /// <summary>
        /// Encodes the image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="encoder">The encoder.</param>
        /// <param name="options">The encoder options.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="image"/> is null.
        ///
        /// -or-
        ///
        /// <paramref name="encoder"/> is null.
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void EncodeImage(HeifImage image, HeifEncoder encoder, HeifEncodingOptions options = null)
        {
            Validate.IsNotNull(image, nameof(image));
            Validate.IsNotNull(encoder, nameof(encoder));
            VerifyNotDisposed();

            heif_error error;

            if (options != null)
            {
                using (var heifEncodingOptions = options.CreateEncodingOptions())
                {
                    error = LibHeifNative.heif_context_encode_image(this.context,
                                                                    image.SafeHeifImage,
                                                                    encoder.SafeHeifEncoder,
                                                                    heifEncodingOptions,
                                                                    IntPtr.Zero);
                }
            }
            else
            {
                error = LibHeifNative.heif_context_encode_image(this.context,
                                                                image.SafeHeifImage,
                                                                encoder.SafeHeifEncoder,
                                                                IntPtr.Zero,
                                                                IntPtr.Zero);
            }
            error.ThrowIfError();
        }

        /// <summary>
        /// Encodes the image and returns the resulting image handle.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="encoder">The encoder.</param>
        /// <param name="options">The encoder options.</param>
        /// <returns>The image handle.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="image"/> is null.
        ///
        /// -or-
        ///
        /// <paramref name="encoder"/> is null.
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public HeifImageHandle EncodeImageAndReturnHandle(HeifImage image, HeifEncoder encoder, HeifEncodingOptions options = null)
        {
            Validate.IsNotNull(image, nameof(image));
            Validate.IsNotNull(encoder, nameof(encoder));
            VerifyNotDisposed();

            HeifImageHandle imageHandle = null;
            SafeHeifImageHandle safeImageHandle = null;

            try
            {
                heif_error error;

                if (options != null)
                {
                    using (var heifEncodingOptions = options.CreateEncodingOptions())
                    {
                        error = LibHeifNative.heif_context_encode_image(this.context,
                                                                        image.SafeHeifImage,
                                                                        encoder.SafeHeifEncoder,
                                                                        heifEncodingOptions,
                                                                        out safeImageHandle);
                    }
                }
                else
                {
                    error = LibHeifNative.heif_context_encode_image(this.context,
                                                                    image.SafeHeifImage,
                                                                    encoder.SafeHeifEncoder,
                                                                    IntPtr.Zero,
                                                                    out safeImageHandle);
                }
                error.ThrowIfError();

                imageHandle = new HeifImageHandle(safeImageHandle);
                safeImageHandle = null;
            }
            finally
            {
                safeImageHandle?.Dispose();
            }

            return imageHandle;
        }

        /// <summary>
        /// Encodes the image thumbnail.
        /// </summary>
        /// <param name="boundingBoxSize">The size of the thumbnail bounding box.</param>
        /// <param name="thumbnail">The thumbnail image.</param>
        /// <param name="parentImageHandle">The handle of the parent image that this thumbnail should be assigned to.</param>
        /// <param name="encoder">The encoder.</param>
        /// <param name="options">The encoder options.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="thumbnail" /> is null.
        ///
        /// -or-
        ///
        /// <paramref name="parentImageHandle" /> is null.
        ///
        /// -or-
        ///
        /// <paramref name="encoder" /> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="boundingBoxSize"/> is less than or equal to zero.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void EncodeThumbnail(int boundingBoxSize,
                                    HeifImage thumbnail,
                                    HeifImageHandle parentImageHandle,
                                    HeifEncoder encoder,
                                    HeifEncodingOptions options = null)
        {
            Validate.IsPositive(boundingBoxSize, nameof(boundingBoxSize));
            Validate.IsNotNull(thumbnail, nameof(thumbnail));
            Validate.IsNotNull(parentImageHandle, nameof(parentImageHandle));
            Validate.IsNotNull(encoder, nameof(encoder));
            VerifyNotDisposed();

            heif_error error;

            if (options != null)
            {
                using (var heifEncodingOptions = options.CreateEncodingOptions())
                {
                    error = LibHeifNative.heif_context_encode_thumbnail(this.context,
                                                                        thumbnail.SafeHeifImage,
                                                                        parentImageHandle.SafeHeifImageHandle,
                                                                        encoder.SafeHeifEncoder,
                                                                        heifEncodingOptions,
                                                                        boundingBoxSize,
                                                                        IntPtr.Zero);
                }
            }
            else
            {
                error = LibHeifNative.heif_context_encode_thumbnail(this.context,
                                                                    thumbnail.SafeHeifImage,
                                                                    parentImageHandle.SafeHeifImageHandle,
                                                                    encoder.SafeHeifEncoder,
                                                                    IntPtr.Zero,
                                                                    boundingBoxSize,
                                                                    IntPtr.Zero);
            }
            error.ThrowIfError();
        }

        /// <summary>
        /// Gets a list of the encoder descriptors.
        /// </summary>
        /// <param name="format">The compression format.</param>
        /// <param name="nameFilter">The encoder name filter.</param>
        /// <returns>The encoder descriptors.</returns>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public IReadOnlyList<HeifEncoderDescriptor> GetEncoderDescriptors(HeifCompressionFormat format = HeifCompressionFormat.Undefined,
                                                                          string nameFilter = null)
        {
            VerifyNotDisposed();

            var nativeEncoderDescriptors = Array.Empty<heif_encoder_descriptor>();
            int encoderCount;

            unsafe
            {
                if (LibHeifVersion.Is1Point16Point2OrLater)
                {
                    encoderCount = LibHeifNative.heif_context_get_encoder_descriptors(this.context,
                                                                                      format,
                                                                                      nameFilter,
                                                                                      null,
                                                                                      0);
                    if (encoderCount > 0)
                    {
                        nativeEncoderDescriptors = new heif_encoder_descriptor[encoderCount];

                        fixed (heif_encoder_descriptor* ptr = nativeEncoderDescriptors)
                        {
                            // The returned encoder count should match the number of encoders we allocated.
                            // Normally we would throw an exception if the two counts are different, but that would be a
                            // breaking change because this method is not documented to throw anything other than
                            // ObjectDisposedException.
                            encoderCount = LibHeifNative.heif_context_get_encoder_descriptors(this.context,
                                                                                              format,
                                                                                              nameFilter,
                                                                                              ptr,
                                                                                              nativeEncoderDescriptors.Length);
                        }
                    }
                }
                else
                {
                    // LibHeif only has 5 built-in encoders as of version 1.9.0, we use 10 in case more
                    // built-in encoders are added in future versions.
                    nativeEncoderDescriptors = new heif_encoder_descriptor[10];

                    fixed (heif_encoder_descriptor* ptr = nativeEncoderDescriptors)
                    {
                        encoderCount = LibHeifNative.heif_context_get_encoder_descriptors(this.context,
                                                                                          format,
                                                                                          nameFilter,
                                                                                          ptr,
                                                                                          nativeEncoderDescriptors.Length);
                    }
                }
            }

            if (encoderCount == 0)
            {
                return Array.Empty<HeifEncoderDescriptor>();
            }
            else
            {
                var encoderDescriptors = new HeifEncoderDescriptor[encoderCount];

                for (int i = 0; i < encoderCount; i++)
                {
                    encoderDescriptors[i] = new HeifEncoderDescriptor(nativeEncoderDescriptors[i]);
                }

                return encoderDescriptors;
            }
        }

        /// <summary>
        /// Gets the default encoder for the specified compression format.
        /// </summary>
        /// <param name="format">The compression format.</param>
        /// <returns>The default encoder for the specified compression format.</returns>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public HeifEncoder GetEncoder(HeifCompressionFormat format)
        {
            VerifyNotDisposed();

            HeifEncoder encoder = null;
            SafeHeifEncoder safeHeifEncoder = null;

            try
            {
                var error = LibHeifNative.heif_context_get_encoder_for_format(this.context,
                                                                              format,
                                                                              out safeHeifEncoder);
                error.ThrowIfError();

                encoder = new HeifEncoder(safeHeifEncoder);
                safeHeifEncoder = null;
            }
            finally
            {
                safeHeifEncoder?.Dispose();
            }

            return encoder;
        }

        /// <summary>
        /// Gets the encoder associated with the specified encoder descriptor.
        /// </summary>
        /// <param name="encoderDescriptor">The encoder descriptor.</param>
        /// <returns>The encoder associated with the specified encoder descriptor.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="encoderDescriptor"/> is null.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public HeifEncoder GetEncoder(HeifEncoderDescriptor encoderDescriptor)
        {
            Validate.IsNotNull(encoderDescriptor, nameof(encoderDescriptor));
            VerifyNotDisposed();

            HeifEncoder encoder = null;
            SafeHeifEncoder safeHeifEncoder = null;

            try
            {
                var error = LibHeifNative.heif_context_get_encoder(this.context,
                                                                   encoderDescriptor.Descriptor,
                                                                   out safeHeifEncoder);
                error.ThrowIfError();

                encoder = new HeifEncoder(safeHeifEncoder);
                safeHeifEncoder = null;
            }
            finally
            {
                safeHeifEncoder?.Dispose();
            }

            return encoder;
        }

        /// <summary>
        /// Gets the image handle for the specified image id.
        /// </summary>
        /// <param name="imageId">The image id.</param>
        /// <returns>The image handle for the specified image id.</returns>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="InvalidOperationException">A reader must be set before calling this method.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public HeifImageHandle GetImageHandle(HeifItemId imageId)
        {
            VerifyNotDisposed();
            EnsureReaderSet();

            HeifImageHandle imageHandle = null;
            SafeHeifImageHandle safeHeifImageHandle = null;

            try
            {
                var error = LibHeifNative.heif_context_get_image_handle(this.context,
                                                                        imageId,
                                                                        out safeHeifImageHandle);
                HandleFileReadError(error);

                imageHandle = new HeifImageHandle(safeHeifImageHandle, HandleFileReadError);
                safeHeifImageHandle = null;
            }
            finally
            {
                safeHeifImageHandle?.Dispose();
            }

            return imageHandle;
        }

        /// <summary>
        /// Gets the image handle for the primary image.
        /// </summary>
        /// <returns>The image handle for the primary image.</returns>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="InvalidOperationException">A reader must be set before calling this method.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public HeifImageHandle GetPrimaryImageHandle()
        {
            VerifyNotDisposed();
            EnsureReaderSet();

            HeifImageHandle imageHandle = null;
            SafeHeifImageHandle safeHeifImageHandle = null;

            try
            {
                var error = LibHeifNative.heif_context_get_primary_image_handle(this.context,
                                                                                out safeHeifImageHandle);
                HandleFileReadError(error);

                imageHandle = new HeifImageHandle(safeHeifImageHandle, HandleFileReadError);
                safeHeifImageHandle = null;
            }
            finally
            {
                safeHeifImageHandle?.Dispose();
            }

            return imageHandle;
        }

        /// <summary>
        /// Gets the region item.
        /// </summary>
        /// <param name="id">The region item identifier.</param>
        /// <returns>The region item.</returns>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <seealso cref="HeifImageHandle.GetRegionItemIds"/>
        public HeifRegionItem GetRegionItem(HeifRegionItemId id)
        {
            VerifyNotDisposed();

            if (LibHeifVersion.Is1Point16OrLater)
            {
                var error = LibHeifNative.heif_context_get_region_item(this.context,
                                                                       id.RegionItemId,
                                                                       out var regionItem);
                error.ThrowIfError();

                return new HeifRegionItem(regionItem, id, this);
            }
            else
            {
                throw new HeifException(Resources.RegionAPINotSupported);
            }
        }

        /// <summary>
        /// Gets a list of the top-level image ids.
        /// </summary>
        /// <returns>A list of the top-level image ids.</returns>
        /// <exception cref="HeifException">
        /// The file does not have any top-level images.
        ///
        /// -or-
        ///
        /// Could not get all of the top-level image ids.
        /// </exception>
        /// <exception cref="InvalidOperationException">A reader must be set before calling this method.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public IReadOnlyList<HeifItemId> GetTopLevelImageIds()
        {
            VerifyNotDisposed();
            EnsureReaderSet();

            int count = LibHeifNative.heif_context_get_number_of_top_level_images(this.context);

            if (count == 0)
            {
                ExceptionUtil.ThrowHeifException(Resources.NoTopLevelImages);
            }

            var ids = new HeifItemId[count];

            unsafe
            {
                fixed (HeifItemId* ptr = ids)
                {
                    int filledCount = LibHeifNative.heif_context_get_list_of_top_level_image_IDs(this.context,
                                                                                                 ptr,
                                                                                                 count);
                    if (filledCount != count)
                    {
                        ExceptionUtil.ThrowHeifException(Resources.CannotGetAllTopLevelImageIds);
                    }
                }
            }

            return ids;
        }

        /// <summary>
        /// Gets the transformation properties that should be applied to the specified image.
        /// </summary>
        /// <param name="imageHandle">The image handle.</param>
        /// <returns>The transformation properties that should be applied to the specified image.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="imageHandle"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <remarks>
        /// This method is supported starting with LibHeif version 1.16.0, it will throw an
        /// exception on older versions.
        /// </remarks>
        public IReadOnlyList<TransformationProperty> GetTransformationProperties(HeifImageHandle imageHandle)
        {
            Validate.IsNotNull(imageHandle, nameof(imageHandle));

            VerifyNotDisposed();

            if (!LibHeifVersion.Is1Point16OrLater)
            {
                ExceptionUtil.ThrowHeifException(Resources.PropertyAPIsNotSupported);
            }

            var properties = Array.Empty<TransformationProperty>();

            var itemId = imageHandle.GetItemId();

            var transformationProperties = GetTransformationPropertyIds(itemId);

            if (transformationProperties.Length > 0)
            {
                properties = new TransformationProperty[transformationProperties.Length];

                int imageWidth = imageHandle.UntransformedWidth;
                int imageHeight = imageHandle.UntransformedHeight;

                for (int i = 0; i < properties.Length; i++)
                {
                    var propertyId = transformationProperties[i];

                    var type = LibHeifNative.heif_item_get_property_type(this.context, itemId, propertyId);

                    if (type == heif_item_property_type.TransformMirror)
                    {
                        var direction = LibHeifNative.heif_item_get_property_transform_mirror(this.context,
                                                                                              itemId,
                                                                                              propertyId);
                        properties[i] = new MirrorTransformationProperty(direction);
                    }
                    else if (type == heif_item_property_type.TransformRotation)
                    {
                        int rotationAngle = LibHeifNative.heif_item_get_property_transform_rotation_ccw(this.context,
                                                                                                        itemId,
                                                                                                        propertyId);
                        if (rotationAngle == 90 || rotationAngle == 270)
                        {
                            // Swap the image width and height so that the size is correct when cropping.
                            (imageHeight, imageWidth) = (imageWidth, imageHeight);
                        }

                        properties[i] = new RotationTransformationProperty(rotationAngle);
                    }
                    else if (type == heif_item_property_type.TransformCrop)
                    {
                        LibHeifNative.heif_item_get_property_transform_crop_borders(this.context,
                                                                                    itemId,
                                                                                    propertyId,
                                                                                    imageWidth,
                                                                                    imageHeight,
                                                                                    out int left,
                                                                                    out int top,
                                                                                    out int right,
                                                                                    out int bottom);
                        properties[i] = new CropTransformationProperty(left, top, right, bottom);
                    }
                    else
                    {
                        throw new HeifException($"Unsupported item property type: {type}.");
                    }
                }
            }

            return properties;
        }

        /// <summary>
        /// Gets the user description properties that are associated with the specified item.
        /// </summary>
        /// <param name="itemId">The item identifier.</param>
        /// <returns>
        /// A list of the user description properties that are associated with the specified item.
        /// </returns>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <remarks>
        /// This method is supported starting with LibHeif version 1.16.0, it will throw an
        /// exception on older versions.
        /// </remarks>
        public IReadOnlyList<UserDescriptionProperty> GetUserDescriptionProperties(HeifItemId itemId)
        {
            VerifyNotDisposed();

            if (!LibHeifVersion.Is1Point16OrLater)
            {
                ExceptionUtil.ThrowHeifException(Resources.PropertyAPIsNotSupported);
            }

            var userDescriptions = Array.Empty<UserDescriptionProperty>();

            var propertyIds = GetPropertyIds(itemId, heif_item_property_type.UserDescription);

            if (propertyIds.Length > 0)
            {
                userDescriptions = new UserDescriptionProperty[propertyIds.Length];

                for (int i = 0; i < propertyIds.Length; i++)
                {
                    var error = LibHeifNative.heif_item_get_property_user_description(this.context,
                                                                                      itemId,
                                                                                      propertyIds[i],
                                                                                      out var native);
                    error.ThrowIfError();

                    try
                    {
                        unsafe
                        {
                            var nativeUserDescription = (heif_property_user_description*)native.DangerousGetHandle();

                            userDescriptions[i] = new UserDescriptionProperty(nativeUserDescription);
                        }
                    }
                    finally
                    {
                        native.Dispose();
                    }
                }
            }

            return userDescriptions;
        }

        /// <summary>
        /// Gets the user description properties that are associated with the specified image.
        /// </summary>
        /// <param name="imageHandle">The image handle.</param>
        /// <returns>
        /// A list of the user description properties that are associated with the specified image.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="imageHandle"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <remarks>
        /// This method is supported starting with LibHeif version 1.16.0, it will throw an
        /// exception on older versions.
        /// </remarks>
        public IReadOnlyList<UserDescriptionProperty> GetUserDescriptionProperties(HeifImageHandle imageHandle)
        {
            Validate.IsNotNull(imageHandle, nameof(imageHandle));

            return GetUserDescriptionProperties(imageHandle.GetItemId());
        }

        /// <summary>
        /// Gets the user description properties that are associated with the specified region item.
        /// </summary>
        /// <param name="regionItem">The region item.</param>
        /// <returns>
        /// A list of the user description properties that are associated with the specified region item.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="regionItem"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <remarks>
        /// This method is supported starting with LibHeif version 1.16.0, it will throw an
        /// exception on older versions.
        /// </remarks>
        public IReadOnlyList<UserDescriptionProperty> GetUserDescriptionProperties(HeifRegionItem regionItem)
        {
            Validate.IsNotNull(regionItem, nameof(regionItem));

            return GetUserDescriptionProperties(regionItem.Id);
        }

        /// <summary>
        /// Reads the specified file into this instance.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="path"/> is empty, contains only whitespace or contains invalid characters.
        /// </exception>
        /// <exception cref="FileNotFoundException">The file specified by <paramref name="path"/> does not exist.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <exception cref="InvalidOperationException">This HeifContext already has an associated reader.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The access requested is not permitted by the operating system for the specified path.
        /// </exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Please use the HeifContext(string) constructor overload instead.", error: true)]
        public void ReadFromFile(string path)
        {
            Validate.IsNotNull(path, nameof(path));
            VerifyNotDisposed();

            if (this.reader != null)
            {
                ExceptionUtil.ThrowInvalidOperationException(Resources.HeifContextAlreadyHasReader);
            }

            this.reader = HeifReaderFactory.CreateFromFile(path);
            InitializeContextFromReader();
        }

        /// <summary>
        /// Reads the specified byte array into this instance.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="bytes"/> is empty.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="InvalidOperationException">This HeifContext already has an associated reader.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Please use the HeifContext(byte[]) constructor overload instead.", error: true)]
        public void ReadFromMemory(byte[] bytes)
        {
            Validate.IsNotNullOrEmptyArray(bytes, nameof(bytes));
            VerifyNotDisposed();

            if (this.reader != null)
            {
                ExceptionUtil.ThrowInvalidOperationException(Resources.HeifContextAlreadyHasReader);
            }

            this.reader = HeifReaderFactory.CreateFromMemory(bytes);
            InitializeContextFromReader();
        }

        /// <summary>
        /// Sets the maximum number of threads that LibHeif can use for decoding images.
        /// </summary>
        /// <param name="maxDecodingThreads">The maximum number of threads that LibHeif can use for decoding images.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxDecodingThreads"/> is less than or equal to zero.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <remarks>
        /// <para>
        /// This method allows callers to limit the number of threads that LibHeif can use when decoding images.
        /// The value will replace the default limit that LibHeif uses when decoding images in this instance.
        /// </para>
        /// <para>
        /// It is supported starting with LibHeif version 1.13.0, and will be ignored on older versions.
        /// </para>
        /// </remarks>
        public void SetMaximumDecodingThreads(int maxDecodingThreads)
        {
            Validate.IsPositive(maxDecodingThreads, nameof(maxDecodingThreads));
            VerifyNotDisposed();

            if (LibHeifVersion.Is1Point13OrLater)
            {
                LibHeifNative.heif_context_set_max_decoding_threads(this.context, maxDecodingThreads);
            }
        }

        /// <summary>
        /// Sets the maximum image size limit.
        /// </summary>
        /// <param name="maxImageSizeLimit">The maximum image size limit.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxImageSizeLimit"/> is less than or equal to zero.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <remarks>
        /// This method allows callers to limit the maximum image width/height that LibHeif can load.
        /// The value will replace the default limit that LibHeif uses when loading images in this instance.
        /// </remarks>
        public void SetMaximumImageSizeLimit(int maxImageSizeLimit)
        {
            Validate.IsPositive(maxImageSizeLimit, nameof(maxImageSizeLimit));
            VerifyNotDisposed();

            LibHeifNative.heif_context_set_maximum_image_size_limit(this.context, maxImageSizeLimit);
        }

        /// <summary>
        /// Sets the primary image.
        /// </summary>
        /// <param name="primaryImage">The primary image.</param>
        /// <exception cref="ArgumentNullException"><paramref name="primaryImage"/> is null.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void SetPrimaryImage(HeifImageHandle primaryImage)
        {
            Validate.IsNotNull(primaryImage, nameof(primaryImage));
            VerifyNotDisposed();

            var error = LibHeifNative.heif_context_set_primary_image(this.context, primaryImage.SafeHeifImageHandle);
            error.ThrowIfError();
        }

        /// <summary>
        /// Writes this instance to the specified file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="path"/> is empty, contains only whitespace or contains invalid characters.
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="UnauthorizedAccessException">
        /// The access requested is not permitted by the operating system for the specified path.
        /// </exception>
        public void WriteToFile(string path)
        {
            Validate.IsNotNull(path, nameof(path));
            VerifyNotDisposed();

            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                WriteToStreamInternal(stream);
            }
        }

        /// <summary>
        /// Writes this instance to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> must support writing.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void WriteToStream(Stream stream)
        {
            Validate.IsNotNull(stream, nameof(stream));

            if (!stream.CanWrite)
            {
                ExceptionUtil.ThrowArgumentException(Resources.StreamCannotWrite);
            }

            VerifyNotDisposed();

            WriteToStreamInternal(stream);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposableUtil.Free(ref this.context);
                DisposableUtil.Free(ref this.reader);
                DisposableUtil.Free(ref this.initializationContext);
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Creates the native LibHeif context.
        /// </summary>
        /// <returns>The native LibHeif context.</returns>
        /// <exception cref="HeifException">Unable to create the native HeifContext.</exception>
        private static SafeHeifContext CreateNativeContext()
        {
            var context = LibHeifNative.heif_context_alloc();

            if (context.IsInvalid)
            {
                ExceptionUtil.ThrowHeifException(Resources.HeifContextCreationFailed);
            }

            return context;
        }

        /// <summary>
        /// Handles the file IO error.
        /// </summary>
        /// <param name="heifIOError">The IO error.</param>
        /// <param name="error">The error.</param>
        /// <param name="wrapIOExceptions">
        /// <see langword="true"/> if an IOException should be wrapped in a HeifException; otherwise, <see langword="false"/>.
        /// </param>
        /// <exception cref="HeifException">The exception that is thrown with the error details.</exception>
        private static void HandleFileIOError(IHeifIOError heifIOError,
                                              in heif_error error,
                                              bool wrapIOExceptions)
        {
            if (error.IsError)
            {
                if (heifIOError != null && heifIOError.CallbackExceptionInfo != null)
                {
                    var inner = heifIOError.CallbackExceptionInfo.SourceException;

                    if (inner is IOException && !wrapIOExceptions)
                    {
                        heifIOError.CallbackExceptionInfo.Throw();
                    }
                    else
                    {
                        throw new HeifException(inner.Message, inner);
                    }
                }
                else
                {
                    error.ThrowIfError();
                }
            }
        }

        /// <summary>
        /// Ensures that the reader has been set.
        /// </summary>
        /// <exception cref="InvalidOperationException">The reader has not been set.</exception>
        private void EnsureReaderSet()
        {
            if (this.reader is null)
            {
                ExceptionUtil.ThrowInvalidOperationException(Resources.ReaderNotSet);
            }
        }

        private unsafe HeifPropertyId[] GetPropertyIds(HeifItemId itemId, heif_item_property_type type)
        {
            var ids = Array.Empty<HeifPropertyId>();

            if (LibHeifVersion.Is1Point16Point2OrLater)
            {
                int propertyCount = LibHeifNative.heif_item_get_properties_of_type(this.context,
                                                                                   itemId,
                                                                                   type,
                                                                                   null,
                                                                                   0);
                if (propertyCount > 0)
                {
                    ids = new HeifPropertyId[propertyCount];

                    fixed (HeifPropertyId* ptr = ids)
                    {
                        int filledCount = LibHeifNative.heif_item_get_properties_of_type(this.context,
                                                                                         itemId,
                                                                                         type,
                                                                                         ptr,
                                                                                         propertyCount);
                        if (filledCount != propertyCount)
                        {
                            ExceptionUtil.ThrowHeifException(Resources.CannotGetAllPropertyIds);
                        }
                    }
                }
            }
            else
            {
                // LibHeif 1.16.1 and earlier do not allow callers to query the number of properties.
                // We assume that the item has no more than 50 properties of the requested type.
                const int MaxProperties = 50;

                var propertyIds = stackalloc HeifPropertyId[MaxProperties];

                int propertyCount = LibHeifNative.heif_item_get_properties_of_type(this.context,
                                                                                   itemId,
                                                                                   type,
                                                                                   propertyIds,
                                                                                   MaxProperties);
                if (propertyCount > 0)
                {
                    ids = new HeifPropertyId[propertyCount];

                    for (int i = 0; i < ids.Length; i++)
                    {
                        ids[i] = propertyIds[i];
                    }
                }
            }

            return ids;
        }

        private unsafe HeifPropertyId[] GetTransformationPropertyIds(HeifItemId itemId)
        {
            var ids = Array.Empty<HeifPropertyId>();

            if (LibHeifVersion.Is1Point16Point2OrLater)
            {
                int propertyCount = LibHeifNative.heif_item_get_transformation_properties(this.context,
                                                                                          itemId,
                                                                                          null,
                                                                                          0);
                if (propertyCount > 0)
                {
                    ids = new HeifPropertyId[propertyCount];

                    fixed (HeifPropertyId* ptr = ids)
                    {
                        int filledCount = LibHeifNative.heif_item_get_transformation_properties(this.context,
                                                                                                itemId,
                                                                                                ptr,
                                                                                                propertyCount);
                        if (filledCount != propertyCount)
                        {
                            ExceptionUtil.ThrowHeifException(Resources.CannotGetAllPropertyIds);
                        }
                    }
                }
            }
            else
            {
                // LibHeif 1.16.1 and earlier do not allow callers to query the number of properties.
                // We assume that the item has no more than 50 properties.
                const int MaxProperties = 50;

                var propertyIds = stackalloc HeifPropertyId[MaxProperties];

                int propertyCount = LibHeifNative.heif_item_get_transformation_properties(this.context,
                                                                                          itemId,
                                                                                          propertyIds,
                                                                                          MaxProperties);
                if (propertyCount > 0)
                {
                    ids = new HeifPropertyId[propertyCount];

                    for (int i = 0; i < ids.Length; i++)
                    {
                        ids[i] = propertyIds[i];
                    }
                }
            }

            return ids;
        }

        /// <summary>
        /// Handles the file read error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <exception cref="HeifException">The exception that is thrown with the error details.</exception>
        private void HandleFileReadError(in heif_error error)
        {
            HandleFileIOError(this.reader, error, wrapIOExceptions: true);
        }

        /// <summary>
        /// Initializes the native context from the current reader.
        /// </summary>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        private void InitializeContextFromReader()
        {
            var error = LibHeifNative.heif_context_read_from_reader(this.context,
                                                                    this.reader.ReaderHandle,
                                                                    IntPtr.Zero,
                                                                    IntPtr.Zero);
            HandleFileReadError(error);
        }

        /// <summary>
        /// Writes the HeifContext to the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        private void WriteToStreamInternal(Stream stream)
        {
            using (var writerStreamIO = new HeifStreamWriter(stream, ownsStream: false))
            {
                var error = LibHeifNative.heif_context_write(this.context,
                                                             writerStreamIO.WriterHandle,
                                                             IntPtr.Zero);
                HandleFileIOError(writerStreamIO, error, wrapIOExceptions: false);
            }
        }
    }
}
