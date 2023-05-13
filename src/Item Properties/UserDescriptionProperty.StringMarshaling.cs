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
using System.Text;
using LibHeifSharp.Interop;
using LibHeifSharp.Properties;

namespace LibHeifSharp
{
    public sealed partial class UserDescriptionProperty
    {
        private static class StringMarshaling
        {
            private static readonly UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false,
                                                                 throwOnInvalidBytes: true);

            /// <summary>
            /// Create a managed string from the specified unmanaged string pointer.
            /// </summary>
            /// <param name="nativeString">The native string.</param>
            /// <returns>The converted string.</returns>
            /// <exception cref="HeifException">
            /// The string contains characters that are not valid for the encoding.
            /// </exception>
            public static unsafe string FromNative(IntPtr nativeString)
            {
                int length = GetNativeStringLength(nativeString);

                string result;

                if (length == 0)
                {
                    result = string.Empty;
                }
                else
                {
                    try
                    {
                        result = encoding.GetString((byte*)nativeString, length);
                    }
                    catch (ArgumentException ex)
                    {
                        // The string contains invalid UTF-8 sequences.
                        throw new HeifException(ex.Message, ex);
                    }
                }

                return result;
            }

            /// <summary>
            /// Converts the string to its unmanaged format.
            /// </summary>
            /// <param name="value">The string to convert.</param>
            /// <returns>A <see cref="SafeCoTaskMemHandle"/> containing the unmanaged memory.</returns>
            /// <exception cref="HeifException">
            /// The string contains characters that are not valid for the encoding.
            /// </exception>
            public static SafeCoTaskMemHandle ToNative(string value)
            {
                try
                {
                    return SafeCoTaskMemHandle.FromStringUtf8(value, encoding);
                }
                catch (ArgumentException ex)
                {
                    // The string contains invalid UTF-8 sequences.
                    throw new HeifException(ex.Message, ex);
                }
            }

            private static unsafe int GetNativeStringLength(IntPtr nativeString)
            {
                int length;

                if (nativeString != IntPtr.Zero)
                {
                    const int MaxStringLength = int.MaxValue;

                    byte* ptr = (byte*)nativeString;
                    int maxLength = MaxStringLength;

                    while (*ptr != 0 && maxLength > 0)
                    {
                        ptr++;
                        maxLength--;
                    }

                    if (maxLength == 0)
                    {
                        // The string is not null-terminated or is longer than 2147483647 bytes.
                        ExceptionUtil.ThrowHeifException(Resources.UserDescriptionStringTooLong);
                    }

                    length = MaxStringLength - maxLength;
                }
                else
                {
                    length = 0;
                }

                return length;
            }
        }
    }
}
