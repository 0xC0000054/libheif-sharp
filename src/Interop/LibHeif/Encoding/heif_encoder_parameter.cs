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
using System.Runtime.InteropServices;

namespace LibHeifSharp.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct heif_encoder_parameter : IEquatable<heif_encoder_parameter>
    {
        public static readonly heif_encoder_parameter Null = new heif_encoder_parameter(IntPtr.Zero);

        // This structure is a type-safe wrapper for
        // an opaque native structure.
        private readonly IntPtr value;

        private heif_encoder_parameter(IntPtr value)
        {
            this.value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is heif_encoder_parameter other && Equals(other);
        }

        public bool Equals(heif_encoder_parameter other)
        {
            return this.value == other.value;
        }

        public override int GetHashCode()
        {
            return -1584136870 + this.value.GetHashCode();
        }

        public static bool operator ==(heif_encoder_parameter left, heif_encoder_parameter right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(heif_encoder_parameter left, heif_encoder_parameter right)
        {
            return !(left == right);
        }
    }
}
