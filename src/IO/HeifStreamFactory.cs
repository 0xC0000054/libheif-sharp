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

namespace LibHeifSharp
{
    internal static class HeifStreamFactory
    {
        /// <summary>
        /// Creates a <see cref="HeifStreamIO" /> instance from the specified file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="fileMode">The file mode.</param>
        /// <param name="fileAccess">The file access.</param>
        /// <param name="fileShare">The file share mode.</param>
        /// <returns>
        /// The created <see cref="HeifStreamIO" /> instance.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="path" /> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="path" /> is empty, contains only whitespace or contains invalid characters.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="fileMode" /> contains an invalid value.</exception>
        /// <exception cref="FileNotFoundException">The file specified by <paramref name="path" /> does not exist.</exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="UnauthorizedAccessException">The access requested is not permitted by the operating system for the specified path.</exception>
        public static HeifStreamIO CreateFromFile(string path, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            HeifStreamIO heifStreamIO;

            FileStream fileStream = null;

            try
            {
                fileStream = new FileStream(path, fileMode, fileAccess, fileShare);

                heifStreamIO = new HeifStreamIO(fileStream, ownsStream: true);

                fileStream = null;
            }
            finally
            {
                fileStream?.Dispose();
            }

            return heifStreamIO;
        }

        /// <summary>
        /// Creates a <see cref="HeifStreamIO"/> instance from the specified byte array.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        /// <returns>The created <see cref="HeifStreamIO"/> instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        public static HeifStreamIO CreateFromMemory(byte[] bytes)
        {
            HeifStreamIO heifStreamIO;

            MemoryStream memoryStream = null;

            try
            {
                memoryStream = new MemoryStream(bytes);

                heifStreamIO = new HeifStreamIO(memoryStream, ownsStream: true);

                memoryStream = null;
            }
            finally
            {
                memoryStream?.Dispose();
            }

            return heifStreamIO;
        }
    }
}
