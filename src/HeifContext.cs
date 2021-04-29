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
    public sealed class HeifContext : Disposable
    {
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

            this.context = CreateNativeContext();
            this.reader = null;
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

            this.context = CreateNativeContext();
            try
            {
                this.reader = HeifReaderFactory.CreateFromFile(path);
                InitializeContextFromReader();
            }
            catch
            {
                this.context.Dispose();
                this.reader?.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifContext"/> class, with the specified byte array to read from.
        /// </summary>
        /// <param name="bytes">A byte array that contains the HEIF image.</param>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="bytes"/> is an empty array.</exception>
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

            this.context = CreateNativeContext();
            try
            {
                this.reader = HeifReaderFactory.CreateFromMemory(bytes);
                InitializeContextFromReader();
            }
            catch
            {
                this.context.Dispose();
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

            this.context = CreateNativeContext();
            try
            {
                this.reader = HeifReaderFactory.CreateFromStream(stream, !leaveOpen);
                InitializeContextFromReader();
            }
            catch (Exception ex)
            {
                this.context.Dispose();
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
        /// <exception cref="ArgumentException"><paramref name="exif"/> is an empty array.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void AddExifMetadata(HeifImageHandle imageHandle, byte[] exif)
        {
            Validate.IsNotNull(imageHandle, nameof(imageHandle));
            Validate.IsNotNullOrEmptyArray(exif, nameof(exif));
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
        /// <paramref name="data"/> is an empty array.
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
        /// <paramref name="data"/> is an empty array.
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void AddGenericMetadata(HeifImageHandle imageHandle, string type, string contentType, byte[] data)
        {
            Validate.IsNotNull(imageHandle, nameof(imageHandle));
            Validate.IsNotNullOrWhiteSpace(type, nameof(type));
            Validate.IsNotEmptyOrWhiteSpace(contentType, nameof(contentType));
            Validate.IsNotNullOrEmptyArray(data, nameof(data));
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
        /// <exception cref="ArgumentException"><paramref name="xmp"/> is an empty array.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void AddXmpMetadata(HeifImageHandle imageHandle, byte[] xmp)
        {
            Validate.IsNotNull(imageHandle, nameof(imageHandle));
            Validate.IsNotNullOrEmptyArray(xmp, nameof(xmp));
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

            // LibHeif only has 5 built-in encoders as of version 1.9.0, we use 10 in case more
            // built-in encoders are added in future versions.
            var nativeEncoderDescriptors = new heif_encoder_descriptor[10];
            int returnedEncoderCount;

            unsafe
            {
                fixed (heif_encoder_descriptor* ptr = nativeEncoderDescriptors)
                {
                    returnedEncoderCount = LibHeifNative.heif_context_get_encoder_descriptors(this.context,
                                                                                              format,
                                                                                              nameFilter,
                                                                                              ptr,
                                                                                              nativeEncoderDescriptors.Length);
                }
            }

            if (returnedEncoderCount == 0)
            {
                return Array.Empty<HeifEncoderDescriptor>();
            }
            else
            {
                var encoderDescriptors = new HeifEncoderDescriptor[returnedEncoderCount];

                for (int i = 0; i < returnedEncoderCount; i++)
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
        [Obsolete("Please use the HeifContext(string) constructor overload instead.")]
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
        /// <exception cref="ArgumentException"><paramref name="bytes"/> is an empty array.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="InvalidOperationException">This HeifContext already has an associated reader.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Please use the HeifContext(byte[]) constructor overload instead.")]
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

            LibHeifNative.set_maximum_image_size_limit(this.context, maxImageSizeLimit);
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

        /// <summary>
        /// Handles the file IO error.
        /// </summary>
        /// <param name="heifIOError">The IO error.</param>
        /// <param name="error">The error.</param>
        /// <exception cref="HeifException">The exception that is thrown with the error details.</exception>
        private static void HandleFileIOError(IHeifIOError heifIOError, in heif_error error)
        {
            if (error.IsError)
            {
                if (heifIOError != null && heifIOError.CallbackExceptionInfo != null)
                {
                    var inner = heifIOError.CallbackExceptionInfo.SourceException;

                    throw new HeifException(inner.Message, inner);
                }
                else
                {
                    error.ThrowIfError();
                }
            }
        }

        /// <summary>
        /// Handles the file read error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <exception cref="HeifException">The exception that is thrown with the error details.</exception>
        private void HandleFileReadError(in heif_error error)
        {
            HandleFileIOError(this.reader, error);
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
        private void WriteToStreamInternal(Stream stream)
        {
            using (var writerStreamIO = new HeifStreamWriter(stream, ownsStream: false))
            {
                var error = LibHeifNative.heif_context_write(this.context,
                                                             writerStreamIO.WriterHandle,
                                                             IntPtr.Zero);
                HandleFileIOError(writerStreamIO, error);
            }
        }
    }
}
