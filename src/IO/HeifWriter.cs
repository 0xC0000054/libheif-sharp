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
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using LibHeifSharp.Interop;
using LibHeifSharp.ResourceManagement;

namespace LibHeifSharp
{
    internal abstract class HeifWriter : Disposable, IHeifIOError
    {
        private DisposableLazy<SafeCoTaskMemHandle> heifWriterHandle;
        private WriterErrors writerErrors;

        private readonly WriteDelegate writeDelegate;

        protected HeifWriter()
        {
            this.heifWriterHandle = new DisposableLazy<SafeCoTaskMemHandle>(CreateHeifWriter);
            this.writerErrors = new WriterErrors();
            this.writeDelegate = Write;
        }

        public ExceptionDispatchInfo CallbackExceptionInfo
        {
            get;
            private set;
        }

        public SafeHandle WriterHandle
        {
            get
            {
                VerifyNotDisposed();

                return this.heifWriterHandle.Value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposableUtil.Free(ref this.heifWriterHandle);
                DisposableUtil.Free(ref this.writerErrors);
            }

            base.Dispose(disposing);
        }

        protected abstract void WriteCore(IntPtr data, long count);

        private SafeCoTaskMemHandle CreateHeifWriter()
        {
            var writerHandle = SafeCoTaskMemHandle.Allocate(MarshalHelper.SizeOf<heif_writer>());

            unsafe
            {
                var writer = (heif_writer*)writerHandle.DangerousGetHandle();

                writer->heif_writer_version = 1;
                writer->write = Marshal.GetFunctionPointerForDelegate(this.writeDelegate);
            }

            return writerHandle;
        }

        private heif_error Write(IntPtr context, IntPtr data, UIntPtr size, IntPtr userData)
        {
            ulong count = size.ToUInt64();

            if (count > 0)
            {
                try
                {
                    WriteCore(data, checked((long)count));
                }
                catch (Exception ex)
                {
                    this.CallbackExceptionInfo = ExceptionDispatchInfo.Capture(ex);
                    return this.writerErrors.WriteError;
                }
            }

            return this.writerErrors.Success;
        }

        private sealed class WriterErrors : Disposable
        {
            private SafeCoTaskMemHandle successMessage;
            private SafeCoTaskMemHandle writeErrorMessage;

            public WriterErrors()
            {
                this.successMessage = null;
                this.writeErrorMessage = null;

                try
                {
                    this.successMessage = SafeCoTaskMemHandle.FromStringAnsi("Success");
                    this.writeErrorMessage = SafeCoTaskMemHandle.FromStringAnsi("Write error");
                }
                catch (Exception)
                {
                    DisposableUtil.Free(ref this.successMessage);
                    throw;
                }
            }

            public heif_error Success
            {
                get
                {
                    VerifyNotDisposed();

                    return new heif_error(heif_error_code.Ok,
                                          heif_suberror_code.Unspecified,
                                          this.successMessage.DangerousGetHandle());
                }
            }

            public heif_error WriteError
            {
                get
                {
                    VerifyNotDisposed();

                    return new heif_error(heif_error_code.Encoding_error,
                                          heif_suberror_code.Cannot_write_output_data,
                                          this.writeErrorMessage.DangerousGetHandle());
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    DisposableUtil.Free(ref this.successMessage);
                    DisposableUtil.Free(ref this.writeErrorMessage);
                }

                base.Dispose(disposing);
            }
        }
    }
}
