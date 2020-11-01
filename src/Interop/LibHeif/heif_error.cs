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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LibHeifSharp.Interop
{
    [DebuggerDisplay("{" + nameof(ErrorCode) + "}")]
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct heif_error
    {
#pragma warning disable IDE0032 // Use auto property
        private readonly heif_error_code errorCode;
        private readonly heif_suberror_code suberrorCode;
        private readonly IntPtr message;
#pragma warning restore IDE0032 // Use auto property

        public heif_error(heif_error_code errorCode, heif_suberror_code suberrorCode, IntPtr message)
        {
            this.errorCode = errorCode;
            this.suberrorCode = suberrorCode;
            this.message = message;
        }

        public heif_error_code ErrorCode => this.errorCode;

        public heif_suberror_code Suberror => this.suberrorCode;

        public bool IsError => this.errorCode != heif_error_code.Ok;

        public void ThrowIfError()
        {
            if (this.IsError)
            {
                ExceptionUtil.ThrowHeifException(GetErrorMessage());
            }
        }

        private string GetErrorMessage()
        {
            return Marshal.PtrToStringAnsi(this.message) ?? Properties.Resources.UnspecifiedError;
        }
    }
}
