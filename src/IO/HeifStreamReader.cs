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
using System.IO;
using System.Runtime.InteropServices;
using LibHeifSharp.Interop;

namespace LibHeifSharp
{
    internal sealed class HeifStreamReader : HeifReader
    {
        // 81920 is the largest multiple of 4096 that is below the large object heap threshold.
        internal const int MaxReadBufferSize = 81920;

        private Stream stream;

        private readonly bool ownsStream;
        private readonly byte[] streamBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifStreamReader"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="ownsStream"><see langword="true"/> if the writer owns the stream; otherwise, <see langword="false"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
        public HeifStreamReader(Stream stream, bool ownsStream = false)
        {
            Validate.IsNotNull(stream, nameof(stream));

            this.stream = stream;
            this.ownsStream = ownsStream;
            this.streamBuffer = new byte[MaxReadBufferSize];
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
            }

            base.Dispose(disposing);
        }

        protected override long GetPositionCore()
        {
            return this.stream.Position;
        }

        protected override unsafe bool ReadCore(IntPtr data, long count)
        {
            long totalBytesRead = 0;
            long remaining = count;

            while (remaining > 0)
            {
                int streamBytesRead = this.stream.Read(this.streamBuffer, 0, (int)Math.Min(MaxReadBufferSize, remaining));

                if (streamBytesRead == 0)
                {
                    break;
                }

                Marshal.Copy(this.streamBuffer, 0, new IntPtr((byte*)data + totalBytesRead), streamBytesRead);

                totalBytesRead += streamBytesRead;
                remaining -= streamBytesRead;
            }

            return remaining == 0;
        }

        protected override bool SeekCore(long position)
        {
            if (position < 0 || position > this.stream.Length)
            {
                return false;
            }

            this.stream.Position = position;
            return true;
        }

        protected override heif_reader_grow_status WaitForFileSizeCore(long targetSize)
        {
            return targetSize > this.stream.Length ? heif_reader_grow_status.size_beyond_eof : heif_reader_grow_status.size_reached;
        }
    }
}
