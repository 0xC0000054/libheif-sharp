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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace LibHeifSharp
{
    /// <summary>
    /// The depth representation type.
    /// </summary>
    public enum HeifDepthRepresentationType
    {
        /// <summary>
        /// Uniform inverse Z.
        /// </summary>
        UniformInverseZ = 0,

        /// <summary>
        /// Uniform disparity.
        /// </summary>
        UniformDisparity = 1,

        /// <summary>
        /// Uniform Z
        /// </summary>
        UniformZ = 2,

        /// <summary>
        /// Non-uniform disparity.
        /// </summary>
        NonuniformDisparity = 3
    }

    /// <summary>
    /// The depth representation information.
    /// </summary>
    public sealed class HeifDepthRepresentationInfo
    {
        private readonly ReadOnlyCollection<byte> nonlinearRepresentationModel;

        internal unsafe HeifDepthRepresentationInfo(Interop.heif_depth_representation_info* info)
        {
            this.ZNear = info->has_z_near != 0 ? info->z_near : (double?)null;
            this.ZFar = info->has_z_far != 0 ? info->z_far : (double?)null;
            this.DMin = info->has_d_min != 0 ? info->d_min : (double?)null;
            this.DMax = info->has_d_max != 0 ? info->d_max : (double?)null;
            this.DepthRepresentationType = info->depth_representation_type;
            this.DisparityReferenceView = info->disparity_reference_view;

            if (info->depth_nonlinear_representation_model_size > 0
                && info->depth_nonlinear_representation_model_size <= int.MaxValue)
            {
                byte[] bytes = new byte[info->depth_nonlinear_representation_model_size];

                Marshal.Copy(info->depth_nonlinear_representation_model, bytes, 0, bytes.Length);

                this.nonlinearRepresentationModel = new ReadOnlyCollection<byte>(bytes);
            }
            else
            {
                this.nonlinearRepresentationModel = new ReadOnlyCollection<byte>(Array.Empty<byte>());
            }
        }

        /// <summary>
        /// Gets the Z near.
        /// </summary>
        /// <value>
        /// The z near.
        /// </value>
        public double? ZNear { get; }

        /// <summary>
        /// Gets the Z far.
        /// </summary>
        /// <value>
        /// The Z far.
        /// </value>
        public double? ZFar { get; }

        /// <summary>
        /// Gets the D minimum.
        /// </summary>
        /// <value>
        /// The D minimum.
        /// </value>
        public double? DMin { get; }

        /// <summary>
        /// Gets the D maximum.
        /// </summary>
        /// <value>
        /// The D maximum.
        /// </value>
        public double? DMax { get; }

        /// <summary>
        /// Gets the type of the depth representation.
        /// </summary>
        /// <value>
        /// The type of the depth representation.
        /// </value>
        public HeifDepthRepresentationType DepthRepresentationType { get; }

        /// <summary>
        /// Gets the disparity reference view.
        /// </summary>
        /// <value>
        /// The disparity reference view.
        /// </value>
        public long DisparityReferenceView { get; }

        /// <summary>
        /// Gets the non-linear representation model.
        /// </summary>
        /// <value>
        /// The non-linear representation model.
        /// </value>
        public IReadOnlyList<byte> NonlinearRepresentationModel => this.nonlinearRepresentationModel;
    }
}
