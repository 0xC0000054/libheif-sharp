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
    internal abstract class HeifReader : Disposable, IHeifIOError
    {
        private const int Success = 0;
        private const int Failure = 1;

        private DisposableLazy<SafeCoTaskMemHandle> heifReaderHandle;

        private readonly GetPositionDelegate getPositionDelegate;
        private readonly ReadDelegate readDelegate;
        private readonly SeekDelegate seekDelegate;
        private readonly WaitForFileSizeDelegate waitForFileSizeDelegate;

        protected HeifReader()
        {
            this.heifReaderHandle = new DisposableLazy<SafeCoTaskMemHandle>(CreateHeifReader);
            this.getPositionDelegate = GetPosition;
            this.readDelegate = Read;
            this.seekDelegate = Seek;
            this.waitForFileSizeDelegate = WaitForFileSize;
        }

        public ExceptionDispatchInfo CallbackExceptionInfo
        {
            get;
            private set;
        }

        public SafeHandle ReaderHandle
        {
            get
            {
                VerifyNotDisposed();

                return this.heifReaderHandle.Value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposableUtil.Free(ref this.heifReaderHandle);
            }

            base.Dispose(disposing);
        }

        protected abstract long GetPositionCore();

        protected abstract bool ReadCore(IntPtr data, long count);

        protected abstract bool SeekCore(long position);

        protected abstract heif_reader_grow_status WaitForFileSizeCore(long targetSize);

        private SafeCoTaskMemHandle CreateHeifReader()
        {
            // This must be allocated in unmanaged memory because LibHeif keeps a reference
            // to this structure.
            var readerHandle = SafeCoTaskMemHandle.Allocate(MarshalHelper.SizeOf<heif_reader>());

            unsafe
            {
                var reader = (heif_reader*)readerHandle.DangerousGetHandle();

                reader->reader_api_version = 1;
                reader->get_position = Marshal.GetFunctionPointerForDelegate(this.getPositionDelegate);
                reader->read = Marshal.GetFunctionPointerForDelegate(this.readDelegate);
                reader->seek = Marshal.GetFunctionPointerForDelegate(this.seekDelegate);
                reader->wait_for_file_size = Marshal.GetFunctionPointerForDelegate(this.waitForFileSizeDelegate);
            }

            return readerHandle;
        }

        private long GetPosition(IntPtr userData)
        {
            try
            {
                return GetPositionCore();
            }
            catch (Exception ex)
            {
                this.CallbackExceptionInfo = ExceptionDispatchInfo.Capture(ex);
                return -1;
            }
        }

        private int Read(IntPtr data, UIntPtr size, IntPtr userData)
        {
            ulong count = size.ToUInt64();

            if (count == 0)
            {
                return Success;
            }

            if (data == IntPtr.Zero)
            {
                return Failure;
            }

            try
            {
                return ReadCore(data, checked((long)count)) ? Success : Failure;
            }
            catch (Exception ex)
            {
                this.CallbackExceptionInfo = ExceptionDispatchInfo.Capture(ex);
                return Failure;
            }
        }

        private int Seek(long position, IntPtr userData)
        {
            try
            {
                return SeekCore(position) ? Success : Failure;
            }
            catch (Exception ex)
            {
                this.CallbackExceptionInfo = ExceptionDispatchInfo.Capture(ex);
                return Failure;
            }
        }

        private heif_reader_grow_status WaitForFileSize(long targetSize, IntPtr userData)
        {
            try
            {
                return WaitForFileSizeCore(targetSize);
            }
            catch (Exception ex)
            {
                this.CallbackExceptionInfo = ExceptionDispatchInfo.Capture(ex);
                return heif_reader_grow_status.size_beyond_eof;
            }
        }
    }
}
