﻿/*
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
using LibHeifSharp.Interop;
using LibHeifSharp.Properties;
using LibHeifSharp.ResourceManagement;

namespace LibHeifSharp
{
    /// <summary>
    /// Represents a LibHeif region item.
    /// </summary>
    /// <seealso cref="HeifContext.GetRegionItem(HeifRegionItemId)"/>
    /// <seealso cref="HeifImageHandle.AddRegion(long, long)"/>
    public sealed class HeifRegionItem : Disposable
    {
        private SafeHeifRegionItem regionItem;
        private readonly HeifItemId imageHandleId;
        private readonly IHeifContext context;

        internal HeifRegionItem(SafeHeifRegionItem regionItem, HeifRegionItemId regionItemId, IHeifContext context)
        {
            this.regionItem = regionItem;
            this.imageHandleId = regionItemId.ImageHandleId;
            this.Id = regionItemId.RegionItemId;

            LibHeifNative.heif_region_item_get_reference_size(regionItem,
                                                              out uint referenceWidth,
                                                              out uint referenceHeight);
            this.ReferenceWidth = referenceWidth;
            this.ReferenceHeight = referenceHeight;
            this.context = context;
        }

        internal HeifRegionItem(SafeHeifRegionItem regionItem,
                                HeifItemId imageHandleId,
                                long referenceWidth,
                                long referenceHeight)
        {
            this.regionItem = regionItem;
            this.imageHandleId = imageHandleId;
            this.Id = LibHeifNative.heif_region_item_get_id(regionItem);
            this.ReferenceWidth = referenceWidth;
            this.ReferenceHeight = referenceHeight;
        }

        /// <summary>
        /// The region item identifier.
        /// </summary>
        public HeifItemId Id { get; }

        /// <summary>
        /// The width of the encoded image prior to any transformations.
        /// </summary>
        public long ReferenceWidth { get; }

        /// <summary>
        /// The height of the encoded image prior to any transformations.
        /// </summary>
        public long ReferenceHeight { get; }

        /// <summary>
        /// Adds a point to the region geometry.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void AddPoint(int x, int y)
        {
            VerifyNotDisposed();

            var error = LibHeifNative.heif_region_item_add_region_point(this.regionItem, x, y, IntPtr.Zero);
            error.ThrowIfError();
        }

        /// <summary>
        /// Adds a rectangle to the region geometry.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="width"/> must be in the range of [0, 4294967295].
        /// -or-
        /// <paramref name="height"/> must be in the range of [0, 4294967295].
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void AddRectangle(int x, int y, long width, long height)
        {
            Validate.IsInRange(width, nameof(width), uint.MinValue, uint.MaxValue);
            Validate.IsInRange(height, nameof(height), uint.MinValue, uint.MaxValue);

            VerifyNotDisposed();

            var error = LibHeifNative.heif_region_item_add_region_rectangle(this.regionItem,
                                                                            x,
                                                                            y,
                                                                            (uint)width,
                                                                            (uint)height,
                                                                            IntPtr.Zero);
            error.ThrowIfError();
        }

        /// <summary>
        /// Adds an ellipse to the region geometry.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="radiusX">The x radius.</param>
        /// <param name="radiusY">The y radius.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="radiusX"/> must be in the range of [0, 4294967295].
        /// -or-
        /// <paramref name="radiusY"/> must be in the range of [0, 4294967295].
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void AddEllipse(int x, int y, long radiusX, long radiusY)
        {
            Validate.IsInRange(radiusX, nameof(radiusX), uint.MinValue, uint.MaxValue);
            Validate.IsInRange(radiusY, nameof(radiusY), uint.MinValue, uint.MaxValue);

            VerifyNotDisposed();

            var error = LibHeifNative.heif_region_item_add_region_ellipse(this.regionItem,
                                                                          x,
                                                                          y,
                                                                          (uint)radiusX,
                                                                          (uint)radiusY,
                                                                          IntPtr.Zero);
            error.ThrowIfError();
        }

        /// <summary>
        /// Adds an inline mask to the region geometry.
        /// </summary>
        /// <param name="maskData">The mask data.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <exception cref="ArgumentNullException"><paramref name="maskData"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="maskData"/> is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="x"/> must be greater than or equal to 0.
        /// -or-
        /// <paramref name="y"/> must be greater than or equal to 0.
        /// -or-
        /// <paramref name="width"/> must be in the range of [0, 4294967295].
        /// -or-
        /// <paramref name="height"/> must be in the range of [0, 4294967295].
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void AddInlineMask(byte[] maskData, int x, int y, long width, long height)
        {
            Validate.IsNotNull(maskData, nameof(maskData));
            
            AddInlineMask(new ReadOnlySpan<byte>(maskData), x, y, width, height);
        }

        /// <summary>
        /// Adds an inline mask to the region geometry.
        /// </summary>
        /// <param name="maskData">The mask data.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <exception cref="ArgumentException"><paramref name="maskData"/> is empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="x"/> must be greater than or equal to 0.
        /// -or-
        /// <paramref name="y"/> must be greater than or equal to 0.
        /// -or-
        /// <paramref name="width"/> must be in the range of [0, 4294967295].
        /// -or-
        /// <paramref name="height"/> must be in the range of [0, 4294967295].
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void AddInlineMask(ReadOnlySpan<byte> maskData, int x, int y, long width, long height)
        {
            Validate.IsNotEmpty(maskData, nameof(maskData));
            Validate.IsGreaterThanOrEqualTo(x, 0, nameof(x));
            Validate.IsGreaterThanOrEqualTo(y, 0, nameof(y));
            Validate.IsInRange(width, nameof(width), uint.MinValue, uint.MaxValue);
            Validate.IsInRange(height, nameof(height), uint.MinValue, uint.MaxValue);

            VerifyNotDisposed();

            if (!LibHeifVersion.Is1Point17OrLater)
            {
                ExceptionUtil.ThrowHeifException(Resources.RegionMaskAPINotSupported);
            }

            unsafe
            {
                fixed (byte* ptr = maskData)
                {
                    var error = LibHeifNative.heif_region_item_add_region_inline_mask_data(this.regionItem,
                                                                                           x,
                                                                                           y,
                                                                                           (uint)width,
                                                                                           (uint)height,
                                                                                           ptr,
                                                                                           new UIntPtr((uint)maskData.Length),
                                                                                           IntPtr.Zero);
                    error.ThrowIfError();
                }
            }
        }

        /// <summary>
        /// Adds an inline mask to the region geometry.
        /// </summary>  
        /// <param name="image">The mask image.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <exception cref="ArgumentNullException"><paramref name="image"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="x"/> must be greater than or equal to 0.
        /// -or-
        /// <paramref name="y"/> must be greater than or equal to 0.
        /// -or-
        /// <paramref name="width"/> must be in the range of [0, 4294967295].
        /// -or-
        /// <paramref name="height"/> must be in the range of [0, 4294967295].
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <exception cref="OutOfMemoryException">
        /// Insufficient memory to create the inline mask data array.
        /// </exception>
        public void AddInlineMask(HeifImage image, int x, int y, long width, long height)
        {
            Validate.IsNotNull(image, nameof(image));
            Validate.IsGreaterThanOrEqualTo(x, 0, nameof(x));
            Validate.IsGreaterThanOrEqualTo(y, 0, nameof(y));
            Validate.IsInRange(width, nameof(width), uint.MinValue, uint.MaxValue);
            Validate.IsInRange(height, nameof(height), uint.MinValue, uint.MaxValue);

            VerifyNotDisposed();

            if (!LibHeifVersion.Is1Point17OrLater)
            {
                ExceptionUtil.ThrowHeifException(Resources.RegionMaskAPINotSupported);
            }

            unsafe
            {
                byte[] maskData = RegionInlineMask.MaskDataFromImage(image);
                fixed (byte* ptr = maskData)
                {
                    var error = LibHeifNative.heif_region_item_add_region_inline_mask_data(this.regionItem,
                                                                                           x,
                                                                                           y,
                                                                                           (uint)width,
                                                                                           (uint)height,
                                                                                           ptr,
                                                                                           new UIntPtr((uint)maskData.Length),
                                                                                           IntPtr.Zero);
                    error.ThrowIfError();
                }
            }
        }

        /// <summary>
        /// Adds an inline mask to the region geometry.
        /// </summary>
        /// <param name="image">The mask data.</param>
        /// <exception cref="ArgumentNullException"><paramref name="image"/> is <see langword="null"/>.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        /// <exception cref="OutOfMemoryException">
        /// Insufficient memory to create the inline mask data array.
        /// </exception>
        public void AddInlineMask(HeifImage image)
        {
            Validate.IsNotNull(image, nameof(image));

            VerifyNotDisposed();

            if (!LibHeifVersion.Is1Point17OrLater)
            {
                ExceptionUtil.ThrowHeifException(Resources.RegionMaskAPINotSupported);
            }

            unsafe
            {
                byte[] maskData = RegionInlineMask.MaskDataFromImage(image);
                fixed (byte* ptr = maskData)
                {
                    var error = LibHeifNative.heif_region_item_add_region_inline_mask_data(this.regionItem,
                                                                                           0,
                                                                                           0,
                                                                                           (uint)image.Width,
                                                                                           (uint)image.Height,
                                                                                           ptr,
                                                                                           new UIntPtr((uint)maskData.Length),
                                                                                           IntPtr.Zero);
                    error.ThrowIfError();
                }
            }
        }

        /// <summary>
        /// Adds a polygon to the region geometry.
        /// </summary>
        /// <param name="points">The points used in the polygon.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="points"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void AddPolygon(IReadOnlyList<PolygonPoint> points)
        {
            Validate.IsNotNull(points, nameof(points));

            VerifyNotDisposed();

            int count = points.Count;
            if (count > 0)
            {
                try
                {
                    int pointArrayLength = checked(count * 2);

                    int[] pointArray = new int[pointArrayLength];

                    for (int i = 0; i < count; i++)
                    {
                        var point = points[i];

                        pointArray[i * 2] = point.X;
                        pointArray[(i * 2) + 1] = point.Y;
                    }

                    unsafe
                    {
                        fixed (int* ptr = pointArray)
                        {
                            var error = LibHeifNative.heif_region_item_add_region_polygon(this.regionItem, ptr, count, IntPtr.Zero);
                            error.ThrowIfError();
                        }
                    }
                }
                catch (OverflowException ex)
                {
                    throw new HeifException("Overflow when adding the polygon points.", ex);
                }
            }
        }

        /// <summary>
        /// Adds a polyline to the region geometry.
        /// </summary>
        /// <param name="points">The points used in the polyline.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="points"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void AddPolyline(IReadOnlyList<PolygonPoint> points)
        {
            Validate.IsNotNull(points, nameof(points));

            VerifyNotDisposed();

            int count = points.Count;
            if (count > 0)
            {
                try
                {
                    int pointArrayLength = checked(count * 2);

                    int[] pointArray = new int[pointArrayLength];

                    for (int i = 0; i < count; i++)
                    {
                        var point = points[i];

                        pointArray[i * 2] = point.X;
                        pointArray[(i * 2) + 1] = point.Y;
                    }

                    unsafe
                    {
                        fixed (int* ptr = pointArray)
                        {
                            var error = LibHeifNative.heif_region_item_add_region_polyline(this.regionItem, ptr, count, IntPtr.Zero);
                            error.ThrowIfError();
                        }
                    }
                }
                catch (OverflowException ex)
                {
                    throw new HeifException("Overflow when adding the polyline points.", ex);
                }
            }
        }

        /// <summary>
        /// Adds a referenced mask to the region geometry.
        /// </summary> 
        /// <param name="itemId">The mask image item id.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="x"/> must be greater than or equal to 0.
        /// -or-
        /// <paramref name="y"/> must be greater than or equal to 0.
        /// -or-
        /// <paramref name="width"/> must be in the range of [0, 4294967295].
        /// -or-
        /// <paramref name="height"/> must be in the range of [0, 4294967295].
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void AddReferencedMask(HeifItemId itemId, int x, int y, long width, long height)
        {
            Validate.IsGreaterThanOrEqualTo(x, 0, nameof(x));
            Validate.IsGreaterThanOrEqualTo(y, 0, nameof(y));
            Validate.IsInRange(width, nameof(width), uint.MinValue, uint.MaxValue);
            Validate.IsInRange(height, nameof(height), uint.MinValue, uint.MaxValue);

            VerifyNotDisposed();

            if (!LibHeifVersion.Is1Point17OrLater)
            {
                ExceptionUtil.ThrowHeifException(Resources.RegionMaskAPINotSupported);
            }

            var error = LibHeifNative.heif_region_item_add_region_referenced_mask(this.regionItem,
                                                                                  x,
                                                                                  y,
                                                                                  (uint)width,
                                                                                  (uint)height,
                                                                                  itemId,
                                                                                  IntPtr.Zero);
            error.ThrowIfError();
        }

        /// <summary>
        /// Adds an referenced mask to the region geometry.
        /// </summary>
        /// <param name="image">The mask image.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <exception cref="ArgumentNullException"><paramref name="image"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="x"/> must be greater than or equal to 0.
        /// -or-
        /// <paramref name="y"/> must be greater than or equal to 0.
        /// -or-
        /// <paramref name="width"/> must be in the range of [0, 4294967295].
        /// -or-
        /// <paramref name="height"/> must be in the range of [0, 4294967295].
        /// </exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void AddReferencedMask(HeifImageHandle image, int x, int y, long width, long height)
        {
            Validate.IsNotNull(image, nameof(image));
            Validate.IsGreaterThanOrEqualTo(x, 0, nameof(x));
            Validate.IsGreaterThanOrEqualTo(y, 0, nameof(y));
            Validate.IsInRange(width, nameof(width), uint.MinValue, uint.MaxValue);
            Validate.IsInRange(height, nameof(height), uint.MinValue, uint.MaxValue);

            VerifyNotDisposed();

            if (!LibHeifVersion.Is1Point17OrLater)
            {
                ExceptionUtil.ThrowHeifException(Resources.RegionMaskAPINotSupported);
            }

            var itemId = image.GetItemId();

            var error = LibHeifNative.heif_region_item_add_region_referenced_mask(this.regionItem,
                                                                                  x,
                                                                                  y,
                                                                                  (uint)width,
                                                                                  (uint)height,
                                                                                  itemId,
                                                                                  IntPtr.Zero);
            error.ThrowIfError();
        }

        /// <summary>
        /// Adds a referenced mask to the region geometry.
        /// </summary>
        /// <param name="image">The mask image.</param>
        /// <exception cref="ArgumentNullException"><paramref name="image"/> is <see langword="null"/>.</exception>
        /// <exception cref="HeifException">A LibHeif error occurred.</exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public void AddReferencedMask(HeifImageHandle image)
        {
            Validate.IsNotNull(image, nameof(image));

            VerifyNotDisposed();

            if (!LibHeifVersion.Is1Point17OrLater)
            {
                ExceptionUtil.ThrowHeifException(Resources.RegionMaskAPINotSupported);
            }

            var itemId = image.GetItemId();

            var error = LibHeifNative.heif_region_item_add_region_referenced_mask(this.regionItem,
                                                                                  0,
                                                                                  0,
                                                                                  (uint)image.Width,
                                                                                  (uint)image.Height,
                                                                                  itemId,
                                                                                  IntPtr.Zero);
            error.ThrowIfError();
        }

        /// <summary>
        /// Gets the region geometries.
        /// </summary>
        /// <returns>
        /// A collection containing the region geometries.
        /// </returns>
        /// <exception cref="HeifException">
        /// A LibHeif error occurred.
        /// -or-
        /// The <see cref="RegionGeometryType"/> is not supported.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public IReadOnlyList<RegionGeometry> GetRegionGeometries()
        {
            VerifyNotDisposed();

            var items = Array.Empty<RegionGeometry>();

            int count = LibHeifNative.heif_region_item_get_number_of_regions(this.regionItem);

            if (count > 0)
            {
                items = new RegionGeometry[count];
                var regions = GetRegionData(count);

                try
                {
                    for (int i = 0; i < regions.Length; i++)
                    {
                        var region = regions[i];

                        var geometryType = LibHeifNative.heif_region_get_type(region);

                        switch (geometryType)
                        {
                            case RegionGeometryType.Point:
                                items[i] = new RegionPoint(region);
                                break;
                            case RegionGeometryType.Rectangle:
                                items[i] = new RegionRectangle(region);
                                break;
                            case RegionGeometryType.Ellipse:
                                items[i] = new RegionEllipse(region);
                                break;
                            case RegionGeometryType.Polygon:
                                items[i] = new RegionPolygon(region);
                                break;
                            case RegionGeometryType.Polyline:
                                items[i] = new RegionPolyline(region);
                                break;
                            case RegionGeometryType.ReferencedMask:
                                items[i] = new RegionReferencedMask(region, this.context);
                                break;
                            case RegionGeometryType.InlineMask:
                                items[i] = new RegionInlineMask(region);
                                break;
                            default:
                                throw new HeifException($"Unsupported {nameof(RegionGeometryType)}: {geometryType}.");
                        }
                    }
                }
                finally
                {
                    LibHeifNative.heif_region_release_many(regions, count);
                }
            }

            return items;
        }

        /// <summary>
        /// Gets the transformed region geometries.
        /// </summary>
        /// <returns>
        /// A collection containing the transformed region geometries.
        /// </returns>
        /// <exception cref="HeifException">
        /// A LibHeif error occurred.
        /// -or-
        /// The <see cref="RegionGeometryType"/> is not supported.
        /// </exception>
        /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
        public IReadOnlyList<TransformedRegionGeometry> GetTransformedRegionGeometries()
        {
            VerifyNotDisposed();

            var items = new List<TransformedRegionGeometry>();

            int count = LibHeifNative.heif_region_item_get_number_of_regions(this.regionItem);

            if (count > 0)
            {
                var regions = GetRegionData(count);

                try
                {
                    for (int i = 0; i < regions.Length; i++)
                    {
                        var region = regions[i];

                        var geometryType = LibHeifNative.heif_region_get_type(region);

                        switch (geometryType)
                        {
                            case RegionGeometryType.Point:
                                items.Add(new TransformedRegionPoint(region, this.imageHandleId));
                                break;
                            case RegionGeometryType.Rectangle:
                                items.Add(new TransformedRegionRectangle(region, this.imageHandleId));
                                break;
                            case RegionGeometryType.Ellipse:
                                items.Add(new TransformedRegionEllipse(region, this.imageHandleId));
                                break;
                            case RegionGeometryType.Polygon:
                                items.Add(new TransformedRegionPolygon(region, this.imageHandleId));
                                break;
                            case RegionGeometryType.Polyline:
                                items.Add(new TransformedRegionPolyline(region, this.imageHandleId));
                                break;
                            case RegionGeometryType.ReferencedMask:
                            case RegionGeometryType.InlineMask:
                                break;
                            default:
                                throw new HeifException($"Unsupported {nameof(RegionGeometryType)}: {geometryType}.");
                        }
                    }
                }
                finally
                {
                    LibHeifNative.heif_region_release_many(regions, count);
                }
            }

            return items;
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposableUtil.Free(ref this.regionItem);
            }

            base.Dispose(disposing);
        }

        private heif_region[] GetRegionData(int count)
        {
            var regions = new heif_region[count];

            int filledCount = LibHeifNative.heif_region_item_get_list_of_regions(this.regionItem,
                                                                                 regions,
                                                                                 count);
            if (filledCount != count)
            {
                ExceptionUtil.ThrowHeifException(Resources.CannotGetAllRegionItems);
            }

            return regions;
        }
    }
}
