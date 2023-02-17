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

namespace LibHeifSharp
{
    internal sealed class HeifByteArrayReader : HeifReader
    {
        private long position;

        private readonly byte[] buffer;
        private readonly long origin;
        private readonly long length;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifByteArrayReader"/> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="buffer"/> is null.</exception>
        public HeifByteArrayReader(byte[] buffer)
        {
            Validate.IsNotNull(buffer, nameof(buffer));

            this.buffer = buffer;
            this.origin = this.position = 0;
            this.length = buffer.Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeifByteArrayReader"/> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public HeifByteArrayReader(ArraySegment<byte> buffer)
        {
            this.buffer = buffer.Array;
            this.origin = this.position = buffer.Offset;
            this.length = buffer.Count;
        }

        protected override long GetPositionCore()
        {
            return this.position - this.origin;
        }

        protected override bool ReadCore(IntPtr data, long count)
        {
            if (((ulong)this.position + (ulong)count) > (ulong)this.length)
            {
                return false;
            }

            unsafe
            {
                fixed (byte* source = this.buffer)
                {
                    Buffer.MemoryCopy(source + this.position, data.ToPointer(), count, count);
                }
            }
            this.position += count;

            return true;
        }

        protected override bool SeekCore(long position)
        {
            if (position < this.origin || position > this.length)
            {
                return false;
            }

            this.position = position;
            return true;
        }

        protected override heif_reader_grow_status WaitForFileSizeCore(long targetSize)
        {
            return targetSize > this.length ? heif_reader_grow_status.size_beyond_eof : heif_reader_grow_status.size_reached;
        }
    }
}
