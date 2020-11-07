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
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using LibHeifSharp.Interop;
using LibHeifSharp.ResourceManagement;

namespace LibHeifSharp
{
    internal sealed class HeifStreamIO : Disposable
    {
        // 81920 is the largest multiple of 4096 that is below the large object heap threshold.
        private const int MaxBufferSize = 81920;
        private const int Success = 0;
        private const int Failure = 1;

        private Stream stream;
        private DisposableLazy<SafeCoTaskMemHandle> heifReaderHandle;
        private DisposableLazy<SafeCoTaskMemHandle> heifWriterHandle;
        private WriterErrors writerErrors;

        private readonly bool ownsStream;
        private readonly byte[] streamBuffer;
        private readonly GetPositionDelegate getPositionDelegate;
        private readonly ReadDelegate readDelegate;
        private readonly SeekDelegate seekDelegate;
        private readonly WaitForFileSizeDelegate waitForFileSizeDelegate;
        private readonly WriteDelegate writeDelegate;

        public HeifStreamIO(Stream stream, bool ownsStream = false)
        {
            Validate.IsNotNull(stream, nameof(stream));

            this.stream = stream;
            this.ownsStream = ownsStream;
            this.heifReaderHandle = new DisposableLazy<SafeCoTaskMemHandle>(CreateHeifReader);
            this.heifWriterHandle = new DisposableLazy<SafeCoTaskMemHandle>(CreateHeifWriter);
            this.streamBuffer = new byte[MaxBufferSize];
            this.getPositionDelegate = GetPosition;
            this.readDelegate = Read;
            this.seekDelegate = Seek;
            this.waitForFileSizeDelegate = WaitForFileSize;
            this.writeDelegate = Write;
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
                if (this.stream != null)
                {
                    if (this.ownsStream)
                    {
                        this.stream.Dispose();
                    }
                    this.stream = null;
                }

                DisposableUtil.Free(ref this.heifReaderHandle);
                DisposableUtil.Free(ref this.heifWriterHandle);
                DisposableUtil.Free(ref this.writerErrors);
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Creates the HEIF reader.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="IOException">The stream must support reading and seeking.</exception>
        private SafeCoTaskMemHandle CreateHeifReader()
        {
            if (!this.stream.CanRead || !this.stream.CanSeek)
            {
                throw new IOException("The stream must support reading and seeking.");
            }

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

        private SafeCoTaskMemHandle CreateHeifWriter()
        {
            if (!this.stream.CanWrite)
            {
                throw new IOException("The stream must support writing.");
            }

            this.writerErrors = new WriterErrors();
            var writerHandle = SafeCoTaskMemHandle.Allocate(MarshalHelper.SizeOf<heif_writer>());

            unsafe
            {
                var writer = (heif_writer*)writerHandle.DangerousGetHandle();

                writer->heif_writer_version = 1;
                writer->write = Marshal.GetFunctionPointerForDelegate(this.writeDelegate);
            }

            return writerHandle;
        }

        private long GetPosition(IntPtr userData)
        {
            try
            {
                return this.stream.Position;
            }
            catch (Exception ex)
            {
                this.CallbackExceptionInfo = ExceptionDispatchInfo.Capture(ex);
                return 0;
            }
        }

        private int Read(IntPtr data, UIntPtr size, IntPtr userData)
        {
            if (data == IntPtr.Zero)
            {
                return Failure;
            }

            ulong count = size.ToUInt64();

            if (count == 0)
            {
                return Success;
            }

            try
            {
                long totalBytesRead = 0;
                long remaining = checked((long)count);

                do
                {
                    int streamBytesRead = this.stream.Read(this.streamBuffer, 0, (int)Math.Min(MaxBufferSize, remaining));

                    if (streamBytesRead == 0)
                    {
                        break;
                    }

                    Marshal.Copy(this.streamBuffer, 0, new IntPtr(data.ToInt64() + totalBytesRead), streamBytesRead);

                    totalBytesRead += streamBytesRead;
                    remaining -= streamBytesRead;

                } while (remaining > 0);

                return Success;
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
                this.stream.Position = position;
            }
            catch (Exception ex)
            {
                this.CallbackExceptionInfo = ExceptionDispatchInfo.Capture(ex);
                return Failure;
            }

            return Success;
        }

        private heif_reader_grow_status WaitForFileSize(long targetSize, IntPtr userData)
        {
            try
            {
                return targetSize > this.stream.Length ? heif_reader_grow_status.size_beyond_eof : heif_reader_grow_status.size_reached;
            }
            catch (Exception ex)
            {
                this.CallbackExceptionInfo = ExceptionDispatchInfo.Capture(ex);
                return heif_reader_grow_status.size_beyond_eof;
            }
        }

        private heif_error Write(IntPtr ctx, IntPtr data, UIntPtr size, IntPtr userData)
        {
            ulong count = size.ToUInt64();

            if (count > 0)
            {
                try
                {
                    long offset = 0;
                    long remaining = checked((long)count);

                    do
                    {
                        int copySize = (int)Math.Min(MaxBufferSize, remaining);

                        Marshal.Copy(new IntPtr(data.ToInt64() + offset), this.streamBuffer, 0, copySize);

                        this.stream.Write(this.streamBuffer, 0, copySize);

                        offset += copySize;
                        remaining -= copySize;

                    } while (remaining > 0);
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
