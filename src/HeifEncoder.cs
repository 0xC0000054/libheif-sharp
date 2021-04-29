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
using System.Collections.ObjectModel;
using System.Globalization;
using LibHeifSharp.Interop;
using LibHeifSharp.Properties;
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
        private readonly Lazy<IReadOnlyDictionary<string, HeifEncoderParameterType>> encoderParameterTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifEncoder"/> class.
        /// </summary>
        /// <param name="encoder">The encoder.</param>
        internal HeifEncoder(SafeHeifEncoder encoder)
        {
            Validate.IsNotNull(encoder, nameof(encoder));

            this.encoder = encoder;
            this.encoderParameterList = new Lazy<ReadOnlyCollection<IHeifEncoderParameter>>(GetEncoderParameterList);
            this.encoderParameterTypes = new Lazy<IReadOnlyDictionary<string, HeifEncoderParameterType>>(GetEncoderParameterTypes);
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
        /// <param name="quality">The quality value.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="quality"/> must be in the range of [0, 100] inclusive.</exception>
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
        /// <param name="lossless">
        /// <see langword="true"/> if the encoder should use lossless compression; otherwise, <see langword="false"/>.
        /// </param>
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
            Validate.IsNotNull(parameter, nameof(parameter));

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
            Validate.IsNotNull(parameter, nameof(parameter));

            SetParameter(parameter.Name, value);
        }

        /// <summary>
        /// Sets an encoder parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The parameter value.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="parameter"/> is null.
        ///
        /// -or-
        ///
        /// <paramref name="value"/> is null.
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void SetParameter(HeifStringEncoderParameter parameter, string value)
        {
            Validate.IsNotNull(parameter, nameof(parameter));

            SetStringParameter(parameter.Name, value, coerceParameterType: false);
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
            Validate.IsNotNull(name, nameof(name));
            VerifyNotDisposed();

            SetBooleanParameter(name, value);
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
            Validate.IsNotNull(name, nameof(name));
            VerifyNotDisposed();

            SetIntegerParameter(name, value);
        }

        /// <summary>
        /// Sets an encoder parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> is null.
        ///
        /// -or-
        ///
        /// <paramref name="value"/> is null.
        /// </exception>
        /// <exception cref="HeifException">
        /// A LibHeif error occurred.
        ///
        /// -or-
        ///
        /// Unable to convert the parameter to the correct type.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void SetParameter(string name, string value)
        {
            SetStringParameter(name, value, coerceParameterType: true);
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

        /// <summary>
        /// Gets the encoder parameter type dictionary.
        /// </summary>
        /// <returns>The encoder parameter type dictionary.</returns>
        /// <exception cref="HeifException">An error occurred when creating the encoder parameter.</exception>
        private IReadOnlyDictionary<string, HeifEncoderParameterType> GetEncoderParameterTypes()
        {
            var encoderParameterList = this.encoderParameterList.Value;

            var parameterTypes = new Dictionary<string, HeifEncoderParameterType>(encoderParameterList.Count,
                                                                                  StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < encoderParameterList.Count; i++)
            {
                var parameter = encoderParameterList[i];

                parameterTypes.Add(parameter.Name, parameter.ParameterType);
            }

            return parameterTypes;
        }

        /// <summary>
        /// Sets a Boolean encoder parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        private void SetBooleanParameter(string name, bool value)
        {
            var error = LibHeifNative.heif_encoder_set_parameter_boolean(this.encoder, name, value);
            error.ThrowIfError();
        }

        /// <summary>
        /// Sets an integer encoder parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        private void SetIntegerParameter(string name, int value)
        {
            var error = LibHeifNative.heif_encoder_set_parameter_integer(this.encoder, name, value);
            error.ThrowIfError();
        }

        /// <summary>
        /// Sets a string encoder parameter.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <param name="coerceParameterType">
        /// <see langword="true"/> if the parameter should be coerced to the correct type; otherwise, <see langword="false"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> is null.
        ///
        /// -or-
        ///
        /// <paramref name="value"/> is null.
        /// </exception>
        /// <exception cref="HeifException">
        /// A LibHeif error occurred.
        ///
        /// -or-
        ///
        /// Unable to convert the parameter to the correct type.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        private void SetStringParameter(string name, string value, bool coerceParameterType)
        {
            Validate.IsNotNull(name, nameof(name));
            Validate.IsNotNull(value, nameof(value));
            VerifyNotDisposed();

            if (coerceParameterType)
            {
                // Some encoders expect the method that is called to match the parameter type.
                // Attempting to set a Boolean or Integer parameter as a string will cause an invalid parameter error.
                if (this.encoderParameterTypes.Value.TryGetValue(name, out var type))
                {
                    switch (type)
                    {
                        case HeifEncoderParameterType.Boolean:
                            if (TryConvertStringToBoolean(value, out bool boolValue))
                            {
                                SetBooleanParameter(name, boolValue);
                            }
                            else
                            {
                                throw new HeifException(string.Format(CultureInfo.CurrentCulture,
                                                                      Resources.CoerceStringValueToBooleanFailedFormat,
                                                                      nameof(value)));
                            }
                            return;
                        case HeifEncoderParameterType.Integer:
                            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue))
                            {
                                SetIntegerParameter(name, intValue);
                            }
                            else
                            {
                                throw new HeifException(string.Format(CultureInfo.CurrentCulture,
                                                                      Resources.CoerceStringValueToIntegerFailedFormat,
                                                                      nameof(value)));
                            }
                            return;
                        case HeifEncoderParameterType.String:
                            // The parameter is the correct type.
                            break;
                        default:
                            throw new HeifException($"Unknown { nameof(HeifEncoderParameterType) }: { type }.");
                    }
                }
            }

            var error = LibHeifNative.heif_encoder_set_parameter_string(this.encoder, name, value);
            error.ThrowIfError();
        }

        private static bool TryConvertStringToBoolean(string value, out bool result)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (bool.TryParse(value, out result))
                {
                    return true;
                }
                else if (value.Equals("1", StringComparison.Ordinal))
                {
                    result = true;
                    return true;
                }
                else if (value.Equals("0", StringComparison.Ordinal))
                {
                    result = false;
                    return true;
                }
            }

            result = false;
            return false;
        }
    }
}
