﻿/*
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
using System.Collections.ObjectModel;
using LibHeifSharp.Interop;
using LibHeifSharp.ResourceManagement;

namespace LibHeifSharp
{
    /// <summary>
    /// A LibHeif encoder instance
    /// </summary>
    /// <seealso cref="Disposable" />
    /// <threadsafety static="true" instance="false"/>
    public sealed class HeifEncoder : Disposable
    {
        private SafeHeifEncoder encoder;
        private readonly Lazy<ReadOnlyCollection<IHeifEncoderParameter>> encoderParameterList;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifEncoder"/> class.
        /// </summary>
        /// <param name="encoder">The encoder.</param>
        internal HeifEncoder(SafeHeifEncoder encoder)
        {
            if (encoder is null)
            {
                ExceptionUtil.ThrowArgumentNullException(nameof(encoder));
            }

            this.encoder = encoder;
            this.encoderParameterList = new Lazy<ReadOnlyCollection<IHeifEncoderParameter>>(GetEncoderParameterList);
        }

        /// <summary>
        /// Gets a list of the supported encoder parameters.
        /// </summary>
        /// <value>
        /// A list of the supported encoder parameters.
        /// </value>
        /// <exception cref="HeifException">An error occurred when creating the encoder parameters list.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public IReadOnlyList<IHeifEncoderParameter> EncoderParameters
        {
            get
            {
                VerifyNotDisposed();

                return this.encoderParameterList.Value;
            }
        }

        /// <summary>
        /// Gets the encoder handle.
        /// </summary>
        /// <value>
        /// The encoder handle.
        /// </value>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        internal SafeHeifEncoder SafeHeifEncoder
        {
            get
            {
                VerifyNotDisposed();

                return this.encoder;
            }
        }

        /// <summary>
        /// Sets the encoder lossy quality.
        /// </summary>
        /// <param name="quality">The quality.</param>
        /// <exception cref="ArgumentOutOfRangeException">The quality parameter is not in the range of [0, 100] inclusive.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void SetLossyQuality(int quality)
        {
            if (quality < 0 || quality > 100)
            {
                ExceptionUtil.ThrowArgumentOutOfRangeException(nameof(quality), "Must be in the range of [0, 100].");
            }

            VerifyNotDisposed();

            var error = LibHeifNative.heif_encoder_set_lossy_quality(this.encoder, quality);
            error.ThrowIfError();
        }

        /// <summary>
        /// Sets the lossless compression support.
        /// </summary>
        /// <param name="lossless"><c>true</c> [lossless].</param>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void SetLossless(bool lossless)
        {
            VerifyNotDisposed();

            var error = LibHeifNative.heif_encoder_set_lossless(this.encoder, lossless);
            error.ThrowIfError();
        }

        /// <summary>
        /// Sets an encoder parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The parameter value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="parameter"/> is null.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void SetParameter(HeifBooleanEncoderParameter parameter, bool value)
        {
            if (parameter is null)
            {
                ExceptionUtil.ThrowArgumentNullException(nameof(parameter));
            }

            SetParameter(parameter.Name, value);
        }

        /// <summary>
        /// Sets an encoder parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The parameter value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="parameter"/> is null.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void SetParameter(HeifIntegerEncoderParameter parameter, int value)
        {
            if (parameter is null)
            {
                ExceptionUtil.ThrowArgumentNullException(nameof(parameter));
            }

            SetParameter(parameter.Name, value);
        }

        /// <summary>
        /// Sets an encoder parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The parameter value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="parameter"/> is null.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void SetParameter(HeifStringEncoderParameter parameter, string value)
        {
            if (parameter is null)
            {
                ExceptionUtil.ThrowArgumentNullException(nameof(parameter));
            }

            SetParameter(parameter.Name, value);
        }

        /// <summary>
        /// Sets an encoder parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void SetParameter(string name, bool value)
        {
            if (name is null)
            {
                ExceptionUtil.ThrowArgumentNullException(nameof(name));
            }

            VerifyNotDisposed();

            var error = LibHeifNative.heif_encoder_set_parameter_boolean(this.encoder, name, value);
            error.ThrowIfError();
        }

        /// <summary>
        /// Sets an encoder parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void SetParameter(string name, int value)
        {
            if (name is null)
            {
                ExceptionUtil.ThrowArgumentNullException(nameof(name));
            }

            VerifyNotDisposed();

            var error = LibHeifNative.heif_encoder_set_parameter_integer(this.encoder, name, value);
            error.ThrowIfError();
        }

        /// <summary>
        /// Sets an encoder parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void SetParameter(string name, string value)
        {
            if (name is null)
            {
                ExceptionUtil.ThrowArgumentNullException(nameof(name));
            }

            if (value is null)
            {
                ExceptionUtil.ThrowArgumentNullException(nameof(value));
            }

            VerifyNotDisposed();

            var error = LibHeifNative.heif_encoder_set_parameter_string(this.encoder, name, value);
            error.ThrowIfError();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposableUtil.Free(ref this.encoder);
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the encoder parameter list.
        /// </summary>
        /// <returns>The encoder parameter list.</returns>
        /// <exception cref="HeifException">An error occurred when creating the encoder parameter.</exception>
        private unsafe ReadOnlyCollection<IHeifEncoderParameter> GetEncoderParameterList()
        {
            var encoderParameters = new List<IHeifEncoderParameter>();

            var parameterList = LibHeifNative.heif_encoder_list_parameters(this.encoder);

            if (parameterList.Value != IntPtr.Zero)
            {
                var encoderParameter = (heif_encoder_parameter*)parameterList.Value;

                while (*encoderParameter != heif_encoder_parameter.Null)
                {
                    encoderParameters.Add(HeifEncoderParameterFactory.Create(this.encoder, *encoderParameter));

                    encoderParameter++;
                }
            }

            return encoderParameters.AsReadOnly();
        }
    }
}