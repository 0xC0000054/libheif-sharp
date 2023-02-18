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
using LibHeifSharp.Interop;
using LibHeifSharp.ResourceManagement;

namespace LibHeifSharp
{
    public sealed partial class HeifDecodingOptions
    {
        internal sealed class NativeOptions : Disposable
        {
            private SafeHeifDecodingOptions nativeDecodingOptions;
            private SafeCoTaskMemHandle nativeDecoderId;

            public NativeOptions(SafeHeifDecodingOptions nativeDecodingOptions, SafeCoTaskMemHandle nativeDecoderId)
            {
                this.nativeDecodingOptions = nativeDecodingOptions ?? throw new ArgumentNullException(nameof(nativeDecodingOptions));
                this.nativeDecoderId = nativeDecoderId;
            }

            internal SafeHeifDecodingOptions DecodingOptions
            {
                get
                {
                    VerifyNotDisposed();

                    return this.nativeDecodingOptions;
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (this.nativeDecodingOptions != null)
                    {
                        this.nativeDecodingOptions.Dispose();
                        this.nativeDecodingOptions = null;
                    }

                    if (this.nativeDecoderId != null)
                    {
                        this.nativeDecoderId.Dispose();
                        this.nativeDecoderId = null;
                    }
                }

                base.Dispose(disposing);
            }
        }
    }
}
