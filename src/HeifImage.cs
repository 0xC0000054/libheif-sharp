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
using System.ComponentModel;
using LibHeifSharp.Interop;
using LibHeifSharp.Properties;
using LibHeifSharp.ResourceManagement;

namespace LibHeifSharp
{
    /// <summary>
    /// Represents a LibHeif image.
    /// </summary>
    /// <seealso cref="Disposable"/>
    /// <threadsafety static="true" instance="false"/>
    public sealed class HeifImage : Disposable
    {
        private SafeHeifImage image;
        private HeifIccColorProfile cachedIccColorProfile;
        private HeifNclxColorProfile cachedNclxColorProfile;
        private bool fetchedColorProfilesFromImage;
        private readonly object sync;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifImage"/> class.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="colorspace">The color space.</param>
        /// <param name="chroma">The chroma.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="width"/> is less than or equal to zero.
        ///
        /// -or-
        ///
        /// <paramref name="height"/> is less than or equal to zero.
        /// </exception>
        /// <exception cref="HeifException">
        /// A LibHeif error occurred.
        ///
        /// -or-
        ///
        /// The LibHeif version is not supported.
        /// </exception>
        public HeifImage(int width, int height, HeifColorspace colorspace, HeifChroma chroma)
        {
            Validate.IsPositive(width, nameof(width));
            Validate.IsPositive(height, nameof(height));

            LibHeifVersion.ThrowIfNotSupported();

            var error = LibHeifNative.heif_image_create(width,
                                                        height,
                                                        colorspace,
                                                        chroma,
                                                        out this.image);
            error.ThrowIfError();
            // The caller can set a color profile after the image has been created.
            this.cachedIccColorProfile = null;
            this.cachedNclxColorProfile = null;
            this.fetchedColorProfilesFromImage = true;
            this.sync = new object();
            this.Width = width;
            this.Height = height;
            this.Colorspace = colorspace;
            this.Chroma = chroma;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifImage" /> class.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="imageHandleIccColorProfile">The image handle ICC color profile.</param>
        /// <param name="imageHandleNclxColorProfile">The image handle NCLX color profile.</param>
        internal HeifImage(SafeHeifImage image,
                           int width,
                           int height,
                           HeifIccColorProfile imageHandleIccColorProfile,
                           HeifNclxColorProfile imageHandleNclxColorProfile)
        {
            Validate.IsNotNull(image, nameof(image));

            this.image = image;
            this.cachedIccColorProfile = imageHandleIccColorProfile;
            this.cachedNclxColorProfile = imageHandleNclxColorProfile;
            this.fetchedColorProfilesFromImage = this.cachedIccColorProfile != null || this.cachedNclxColorProfile != null;
            this.sync = new object();
            this.Width = width;
            this.Height = height;
            this.Colorspace = LibHeifNative.heif_image_get_colorspace(image);
            this.Chroma = LibHeifNative.heif_image_get_chroma_format(image);
        }

        /// <summary>
        /// Gets the image width.
        /// </summary>
        /// <value>
        /// The image width.
        /// </value>
        public int Width { get; }

        /// <summary>
        /// Gets the image height.
        /// </summary>
        /// <value>
        /// The image height.
        /// </value>
        public int Height { get; }

        /// <summary>
        /// Gets the image color space.
        /// </summary>
        /// <value>
        /// The image color space.
        /// </value>
        public HeifColorspace Colorspace { get; }

        /// <summary>
        /// Gets the image chroma.
        /// </summary>
        /// <value>
        /// The image chroma.
        /// </value>
        public HeifChroma Chroma { get; }

        /// <summary>
        /// Gets or sets the image color profile.
        /// </summary>
        /// <value>
        /// The image color profile.
        /// </value>
        /// <exception cref="ArgumentNullException">Value is null.</exception>
        /// <exception cref="HeifException">
        /// A LibHeif error occurred.
        ///
        /// -or-
        ///
        /// The color profile type is not supported.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use the IccColorProfile and NclxColorProfile properties instead.")]
        public HeifColorProfile ColorProfile
        {
            get
            {
                VerifyNotDisposed();

                if (!this.fetchedColorProfilesFromImage)
                {
                    lock (this.sync)
                    {
                        if (!this.fetchedColorProfilesFromImage)
                        {
                            UpdateCachedColorProfilesWhileLocked();
                            this.fetchedColorProfilesFromImage = true;
                        }
                    }
                }

                if (this.cachedIccColorProfile != null)
                {
                    return this.cachedIccColorProfile;
                }
                else
                {
                    return this.cachedNclxColorProfile;
                }
            }
            set
            {
                Validate.IsNotNull(value, nameof(value));
                VerifyNotDisposed();

                if (value is HeifIccColorProfile iccProfile)
                {
                    if (this.cachedIccColorProfile != iccProfile)
                    {
                        lock (this.sync)
                        {
                            if (this.cachedIccColorProfile != iccProfile)
                            {
                                this.cachedIccColorProfile = iccProfile;
                                this.cachedIccColorProfile.SetImageColorProfile(this.image);
                            }
                        }
                    }
                }
                else if (value is HeifNclxColorProfile nclxProfile)
                {
                    if (this.cachedNclxColorProfile != nclxProfile)
                    {
                        lock (this.sync)
                        {
                            if (this.cachedNclxColorProfile != nclxProfile)
                            {
                                this.cachedNclxColorProfile = nclxProfile;
                                this.cachedNclxColorProfile.SetImageColorProfile(this.image);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the image ICC color profile.
        /// </summary>
        /// <value>
        /// The image ICC color profile.
        /// </value>
        /// <exception cref="ArgumentNullException">Value is null.</exception>
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

                if (!this.fetchedColorProfilesFromImage)
                {
                    lock (this.sync)
                    {
                        if (!this.fetchedColorProfilesFromImage)
                        {
                            UpdateCachedColorProfilesWhileLocked();
                            this.fetchedColorProfilesFromImage = true;
                        }
                    }
                }

                return this.cachedIccColorProfile;
            }
            set
            {
                Validate.IsNotNull(value, nameof(value));
                VerifyNotDisposed();

                if (this.cachedIccColorProfile != value)
                {
                    lock (this.sync)
                    {
                        if (this.cachedIccColorProfile != value)
                        {
                            this.cachedIccColorProfile = value;
                            this.cachedIccColorProfile.SetImageColorProfile(this.image);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the image NCLX color profile.
        /// </summary>
        /// <value>
        /// The image NCLX color profile.
        /// </value>
        /// <exception cref="ArgumentNullException">Value is null.</exception>
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

                if (!this.fetchedColorProfilesFromImage)
                {
                    lock (this.sync)
                    {
                        if (!this.fetchedColorProfilesFromImage)
                        {
                            UpdateCachedColorProfilesWhileLocked();
                            this.fetchedColorProfilesFromImage = true;
                        }
                    }
                }

                return this.cachedNclxColorProfile;
            }
            set
            {
                Validate.IsNotNull(value, nameof(value));
                VerifyNotDisposed();

                if (this.cachedNclxColorProfile != value)
                {
                    lock (this.sync)
                    {
                        if (this.cachedNclxColorProfile != value)
                        {
                            this.cachedNclxColorProfile = value;
                            this.cachedNclxColorProfile.SetImageColorProfile(this.image);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the image has an alpha channel.
        /// </summary>
        /// <value>
        ///   <see langword="true"/> if the image has an alpha channel; otherwise, <see langword="false"/>.
        /// </value>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public bool HasAlphaChannel
        {
            get
            {
                switch (this.Chroma)
                {
                    case HeifChroma.InterleavedRgb24:
                    case HeifChroma.InterleavedRgb48BE:
                    case HeifChroma.InterleavedRgb48LE:
                        return false;
                    case HeifChroma.InterleavedRgba32:
                    case HeifChroma.InterleavedRgba64BE:
                    case HeifChroma.InterleavedRgba64LE:
                        return true;
                    default:
                        return HasChannel(HeifChannel.Alpha);
                }
            }
        }

        /// <summary>
        /// Gets the image handle.
        /// </summary>
        /// <value>
        /// The image handle.
        /// </value>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        internal SafeHeifImage SafeHeifImage
        {
            get
            {
                VerifyNotDisposed();

                return this.image;
            }
        }

        /// <summary>
        /// Adds a plane to the image.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="bitDepth">The bit depth.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="width"/> is less than or equal to zero.
        ///
        /// -or-
        ///
        /// <paramref name="height"/> is less than or equal to zero.
        ///
        /// -or-
        ///
        /// <paramref name="bitDepth"/> is less than or equal to zero.
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void AddPlane(HeifChannel channel,
                             int width,
                             int height,
                             int bitDepth)
        {
            Validate.IsPositive(width, nameof(width));
            Validate.IsPositive(height, nameof(height));
            Validate.IsPositive(bitDepth, nameof(bitDepth));
            VerifyNotDisposed();

            var error = LibHeifNative.heif_image_add_plane(this.image,
                                                           channel,
                                                           width,
                                                           height,
                                                           bitDepth);
            error.ThrowIfError();
        }

        /// <summary>
        /// Determines whether this image contains the specified channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>
        ///   <see langword="true"/> this image contains the specified channel; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public bool HasChannel(HeifChannel channel)
        {
            VerifyNotDisposed();

            return LibHeifNative.heif_image_has_channel(this.image, channel);
        }

        /// <summary>
        /// Gets the image data for the specified plane.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <returns>The image plane data.</returns>
        /// <exception cref="HeifException">The image does not contain the specified channel.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public HeifPlaneData GetPlane(HeifChannel channel)
        {
            VerifyNotDisposed();

            if (!LibHeifNative.heif_image_has_channel(this.image, channel))
            {
                ExceptionUtil.ThrowHeifException(Resources.ImageDoesNotContainChannel);
            }

            int width = LibHeifNative.heif_image_get_width(this.image, channel);
            int height = LibHeifNative.heif_image_get_height(this.image, channel);

            var scan0 = LibHeifNative.heif_image_get_plane(this.image, channel, out int stride);

            if (scan0 == IntPtr.Zero || width == -1 || height == -1)
            {
                ExceptionUtil.ThrowHeifException(Resources.ImageDoesNotContainChannel);
            }

            return new HeifPlaneData(width, height, stride, channel, scan0);
        }

        /// <summary>
        /// Scales the image.
        /// </summary>
        /// <param name="newWidth">The new width.</param>
        /// <param name="newHeight">The new height.</param>
        /// <returns>The scaled image.</returns>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public HeifImage ScaleImage(int newWidth, int newHeight)
        {
            VerifyNotDisposed();

            if (!this.fetchedColorProfilesFromImage)
            {
                lock (this.sync)
                {
                    if (!this.fetchedColorProfilesFromImage)
                    {
                        UpdateCachedColorProfilesWhileLocked();
                        this.fetchedColorProfilesFromImage = true;
                    }
                }
            }

            HeifImage scaledImage = null;
            SafeHeifImage safeHeifImage = null;

            try
            {
                var error = LibHeifNative.heif_image_scale_image(this.image, out safeHeifImage, newWidth, newHeight, IntPtr.Zero);
                error.ThrowIfError();

                scaledImage = new HeifImage(safeHeifImage,
                                            newWidth,
                                            newHeight,
                                            this.cachedIccColorProfile,
                                            this.cachedNclxColorProfile);
                safeHeifImage = null;
            }
            finally
            {
                safeHeifImage?.Dispose();
            }

            return scaledImage;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposableUtil.Free(ref this.image);
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Updates the cached color profiles while locked.
        /// </summary>
        /// <exception cref="HeifException">
        /// The color profile type is not supported.
        ///
        /// -or-
        ///
        /// A LibHeif error occurred.
        /// </exception>
        private void UpdateCachedColorProfilesWhileLocked()
        {
            if (LibHeifVersion.Is1Point12OrLater)
            {
                this.cachedIccColorProfile = HeifIccColorProfile.TryCreate(this.image);
                this.cachedNclxColorProfile = HeifNclxColorProfile.TryCreate(this.image);
            }
            else
            {
                // LibHeif version 1.11 and earlier will crash when retrieving the NCLX color
                // profile if the image does not have one.
                var colorProfileType = LibHeifNative.heif_image_get_color_profile_type(this.image);

                switch (colorProfileType)
                {
                    case heif_color_profile_type.None:
                        this.cachedIccColorProfile = null;
                        this.cachedNclxColorProfile = null;
                        break;
                    case heif_color_profile_type.Nclx:
                        this.cachedIccColorProfile = null;
                        this.cachedNclxColorProfile = HeifNclxColorProfile.TryCreate(this.image);
                        break;
                    case heif_color_profile_type.IccProfile:
                    case heif_color_profile_type.RestrictedIcc:
                        this.cachedIccColorProfile = HeifIccColorProfile.TryCreate(this.image);
                        this.cachedNclxColorProfile = null;
                        break;
                    default:
                        throw new HeifException(Resources.ColorProfileTypeNotSupported);
                }
            }
        }
    }
}
