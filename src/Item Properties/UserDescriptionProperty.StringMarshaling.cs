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
        private static readonly UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        private static class StringMarshaling
        {
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
                    result = Encoding.UTF8.GetString((byte*)nativeString, length);
                }

                return result;
            }

            public static SafeCoTaskMemHandle ToNative(string value)
                => SafeCoTaskMemHandle.FromStringUtf8(value, encoding);

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
