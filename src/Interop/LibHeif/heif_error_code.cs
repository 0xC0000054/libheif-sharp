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

namespace LibHeifSharp.Interop
{
    internal enum heif_error_code
    {
        // Everything ok, no error occurred.
        Ok = 0,

        // Input file does not exist.
        Input_does_not_exist = 1,

        // Error in input file. Corrupted or invalid content.
        Invalid_input = 2,

        // Input file type is not supported.
        Unsupported_filetype = 3,

        // Image requires an unsupported decoder feature.
        Unsupported_feature = 4,

        // Library API has been used in an invalid way.
        Usage_error = 5,

        // Could not allocate enough memory.
        Memory_allocation_error = 6,

        // The decoder plugin generated an error
        Decoder_plugin_error = 7,

        // The encoder plugin generated an error
        Encoder_plugin_error = 8,

        // Error during encoding or when writing to the output
        Encoding_error = 9
    }
}
