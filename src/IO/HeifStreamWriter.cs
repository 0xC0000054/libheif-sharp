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
using System.Runtime.InteropServices;

namespace LibHeifSharp
{
    internal sealed class HeifStreamWriter : HeifWriter
    {
        // 81920 is the largest multiple of 4096 that is below the large object heap threshold.
        private const int MaxWriteBufferSize = 81920;

        private Stream stream;

        private readonly bool ownsStream;
        private readonly byte[] streamBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifStreamWriter"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="ownsStream"><see langword="true"/> if the writer owns the stream; otherwise, <see langword="false"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is null.</exception>
        public HeifStreamWriter(Stream stream, bool ownsStream = false)
        {
            Validate.IsNotNull(stream, nameof(stream));

            this.stream = stream;
            this.ownsStream = ownsStream;
            this.streamBuffer = new byte[MaxWriteBufferSize];
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

        protected override void WriteCore(IntPtr data, long count)
        {
            long offset = 0;
            long remaining = count;

            while (remaining > 0)
            {
                int copySize = (int)Math.Min(MaxWriteBufferSize, remaining);

                Marshal.Copy(new IntPtr(data.ToInt64() + offset), this.streamBuffer, 0, copySize);

                this.stream.Write(this.streamBuffer, 0, copySize);

                offset += copySize;
                remaining -= copySize;
            }
        }
    }
}
