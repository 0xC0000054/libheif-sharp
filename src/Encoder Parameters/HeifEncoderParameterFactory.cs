/*
 * .NET bindings for libheif.
 * Copyright (c) 2020, 2021, 2022 Nicholas Hayes
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

namespace LibHeifSharp
{
    internal static class HeifEncoderParameterFactory
    {
        /// <summary>
        /// Creates the specified encoder parameter.
        /// </summary>
        /// <param name="encoder">The encoder.</param>
        /// <param name="nativeParameter">The native encoder parameter.</param>
        /// <returns></returns>
        /// <exception cref="HeifException">An error occurred when creating the encoder parameter.</exception>
        public static unsafe IHeifEncoderParameter Create(SafeHeifEncoder encoder, heif_encoder_parameter nativeParameter)
        {
            string name = LibHeifNative.heif_encoder_parameter_get_name(nativeParameter).GetStringValue();
            var type = LibHeifNative.heif_encoder_parameter_get_type(nativeParameter);

            bool hasDefaultValue = LibHeifNative.heif_encoder_has_default(encoder, name);

            switch (type)
            {
                case heif_encoder_parameter_type.Integer:
                    return CreateIntegerParameter(encoder, nativeParameter, name, hasDefaultValue);
                case heif_encoder_parameter_type.Boolean:
                    return CreateBooleanParameter(encoder, name, hasDefaultValue);
                case heif_encoder_parameter_type.String:
                    return CreateStringParameter(encoder, nativeParameter, name, hasDefaultValue);
                default:
                    throw new HeifException($"Unknown { nameof(heif_encoder_parameter_type)}: { type }.");
            }
        }

        private static HeifBooleanEncoderParameter CreateBooleanParameter(SafeHeifEncoder encoder,
                                                                          string name,
                                                                          bool hasDefaultValue)
        {
            bool defaultValue = false;

            if (hasDefaultValue)
            {
                // The error value is ignored because some encoders return an error
                // when getting the value of a valid command.
                _ = LibHeifNative.heif_encoder_get_parameter_boolean(encoder, name, out defaultValue);
            }

            return new HeifBooleanEncoderParameter(name, hasDefaultValue, defaultValue);
        }

        private static unsafe HeifIntegerEncoderParameter CreateIntegerParameter(SafeHeifEncoder encoder,
                                                                                 heif_encoder_parameter nativeParameter,
                                                                                 string name,
                                                                                 bool hasDefaultValue)
        {
            int defaultValue = 0;

            if (hasDefaultValue)
            {
                // The error value is ignored because some encoders return an error
                // when getting the value of a valid command.
                _ = LibHeifNative.heif_encoder_get_parameter_integer(encoder, name, out defaultValue);
            }

            bool haveMinimumMaximum;
            int minimum = 0;
            int maximum = 0;
            var validValues = new List<int>();

            if (LibHeifVersion.Is1Point10OrLater)
            {
                var error = LibHeifNative.heif_encoder_parameter_get_valid_integer_values(nativeParameter,
                                                                                          out bool haveMinimum,
                                                                                          out bool haveMaximum,
                                                                                          ref minimum,
                                                                                          ref maximum,
                                                                                          out int numValidValues,
                                                                                          out var validValuesArray);
                error.ThrowIfError();

                haveMinimumMaximum = haveMinimum && haveMaximum;

                if (numValidValues > 0 && validValuesArray != IntPtr.Zero)
                {
                    validValues.Capacity = numValidValues;

                    int* integerArray = (int*)validValuesArray;

                    for (int i = 0; i < numValidValues; i++)
                    {
                        validValues.Add(integerArray[i]);
                    }
                }
            }
            else
            {
                var error = LibHeifNative.heif_encoder_parameter_get_valid_integer_range(nativeParameter,
                                                                                         out haveMinimumMaximum,
                                                                                         ref minimum,
                                                                                         ref maximum);
                error.ThrowIfError();
            }

            return new HeifIntegerEncoderParameter(name,
                                                   hasDefaultValue,
                                                   defaultValue,
                                                   haveMinimumMaximum,
                                                   minimum,
                                                   maximum,
                                                   validValues.AsReadOnly());
        }

        private static unsafe HeifStringEncoderParameter CreateStringParameter(SafeHeifEncoder encoder,
                                                                               heif_encoder_parameter nativeParameter,
                                                                               string name,
                                                                               bool hasDefaultValue)
        {
            string defaultValue = string.Empty;

            if (hasDefaultValue)
            {
                const int ByteArrayLength = 256;

                byte* bytes = stackalloc byte[ByteArrayLength];

                // The error value is ignored because some encoders return an error
                // when getting the value of a valid command.
                _ = LibHeifNative.heif_encoder_get_parameter_string(encoder, name, bytes, ByteArrayLength);

                int count = ByteArrayLength;

                // Look for the NUL terminator at the end of the string.
                for (int i = 0; i < ByteArrayLength; i++)
                {
                    if (bytes[i] == 0)
                    {
                        count = i;
                        break;
                    }
                }

                defaultValue = System.Text.Encoding.ASCII.GetString(bytes, count);
            }

            var error = LibHeifNative.heif_encoder_parameter_get_valid_string_values(nativeParameter, out var stringArray);
            error.ThrowIfError();

            var validItems = stringArray.ToReadOnlyCollection();

            return new HeifStringEncoderParameter(name, hasDefaultValue, defaultValue, validItems);
        }
    }
}
