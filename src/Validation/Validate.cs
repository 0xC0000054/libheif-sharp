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
using System.Globalization;
using LibHeifSharp.Properties;

namespace LibHeifSharp
{
    internal static class Validate
    {
        /// <summary>
        /// Determines whether the specified parameter is greater than or equal to the minimum value.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <exception cref="ArgumentOutOfRangeException">The parameter is outside of the required range.</exception>
        public static void IsGreaterThanOrEqualTo(int param, int min, string paramName)
        {
            if (param < min)
            {
                ExceptionUtil.ThrowArgumentOutOfRangeException(paramName, string.Format(CultureInfo.CurrentCulture,
                                                                                        Resources.ParameterMustBeGreaterThanOrEqualFormat,
                                                                                        paramName,
                                                                                        min));
            }
        }

        /// <summary>
        /// Determines whether the specified parameter is within the required range.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The inclusive maximum value.</param>
        /// <exception cref="ArgumentOutOfRangeException">The parameter is outside of the required range.</exception>
        public static void IsInRange(int param, string paramName, int min, int max)
        {
            if (param < min || param > max)
            {
                ExceptionUtil.ThrowArgumentOutOfRangeException(paramName, string.Format(CultureInfo.CurrentCulture,
                                                                                        Resources.ParameterOutOfRangeInclusiveFormat,
                                                                                        paramName,
                                                                                        min,
                                                                                        max));
            }
        }

        /// <summary>
        /// Determines whether the specified parameter is within the required range.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The inclusive maximum value.</param>
        /// <exception cref="ArgumentOutOfRangeException">The parameter is outside of the required range.</exception>
        public static void IsInRange(long param, string paramName, long min, long max)
        {
            if (param < min || param > max)
            {
                ExceptionUtil.ThrowArgumentOutOfRangeException(paramName, string.Format(CultureInfo.CurrentCulture,
                                                                                        Resources.ParameterOutOfRangeInclusiveFormat,
                                                                                        paramName,
                                                                                        min,
                                                                                        max));
            }
        }

        /// <summary>
        /// Determines whether the specified array is empty.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentException">The parameter is a empty.</exception>
        public static void IsNotEmpty<T>(ReadOnlyMemory<T> param, string paramName)
        {
            if (param.IsEmpty)
            {
                ExceptionUtil.ThrowArgumentException(string.Format(CultureInfo.CurrentCulture,
                                                                   Resources.ParameterIsEmptyFormat,
                                                                   paramName));
            }
        }

        /// <summary>
        /// Determines whether the specified array is empty.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentException">The parameter is empty.</exception>
        public static void IsNotEmpty<T>(ReadOnlySpan<T> param, string paramName)
        {
            if (param.IsEmpty)
            {
                ExceptionUtil.ThrowArgumentException(string.Format(CultureInfo.CurrentCulture,
                                                                   Resources.ParameterIsEmptyFormat,
                                                                   paramName));
            }
        }

        /// <summary>
        /// Determines whether the specified parameter is empty or contains only whitespace characters, the parameter can be null.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentException">The parameter is empty or contains only whitespace characters.</exception>
        public static void IsNotEmptyOrWhiteSpace(string param, string paramName)
        {
            if (param != null && param.IsEmptyOrWhiteSpace())
            {
                ExceptionUtil.ThrowArgumentException(string.Format(CultureInfo.CurrentCulture,
                                                                   Resources.ParameterStringIsEmptyOrWhitespaceFormat,
                                                                   paramName));
            }
        }

        /// <summary>
        /// Determines whether the specified parameter is null.
        /// </summary>
        /// <typeparam name="T">The parameter type.</typeparam>
        /// <param name="param">The parameter.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentNullException">The parameter is null.</exception>
        public static void IsNotNull<T>(T param, string paramName) where T : class
        {
            if (param is null)
            {
                ExceptionUtil.ThrowArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// Determines whether the specified array is null or empty.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentNullException">The parameter is null.</exception>
        /// <exception cref="ArgumentException">The parameter is a zero-length array.</exception>
        public static void IsNotNullOrEmptyArray<T>(T[] param, string paramName)
        {
            if (param is null)
            {
                ExceptionUtil.ThrowArgumentNullException(paramName);
            }
            else if (param.Length == 0)
            {
                ExceptionUtil.ThrowArgumentException(string.Format(CultureInfo.CurrentCulture,
                                                                   Resources.ParameterIsEmptyFormat,
                                                                   paramName));
            }
        }

        /// <summary>
        /// Determines whether the specified collection is null or does not have the required length.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <param name="requiredSize">The required size of the collection.</param>
        /// <exception cref="ArgumentNullException">The parameter is null.</exception>
        /// <exception cref="ArgumentException">The parameter does not have the required size.</exception>
        public static void HasSizeEqualTo<T>(IReadOnlyList<T> param, string paramName, int requiredSize)
        {
            if (param is null)
            {
                ExceptionUtil.ThrowArgumentNullException(paramName);
            }
            else if (param.Count != requiredSize)
            {
                ExceptionUtil.ThrowArgumentException(string.Format(CultureInfo.CurrentCulture,
                                                                   Resources.ParameterRequiredCollectionSizeFormat,
                                                                   paramName,
                                                                   requiredSize));
            }
        }

        /// <summary>
        /// Determines whether the specified parameter is null, empty or contains only whitespace characters.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentNullException">The parameter is null.</exception>
        /// <exception cref="ArgumentException">The parameter is empty or contains only whitespace characters.</exception>
        public static void IsNotNullOrWhiteSpace(string param, string paramName)
        {
            if (param is null)
            {
                ExceptionUtil.ThrowArgumentNullException(paramName);
            }
            else if (param.IsEmptyOrWhiteSpace())
            {
                ExceptionUtil.ThrowArgumentException(string.Format(CultureInfo.CurrentCulture,
                                                                   Resources.ParameterStringIsEmptyOrWhitespaceFormat,
                                                                   paramName));
            }
        }

        /// <summary>
        /// Determines whether the specified parameter is greater than zero.
        /// </summary>
        /// <param name="param">The parameter.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentOutOfRangeException">The parameter is less than or equal to zero.</exception>
        public static void IsPositive(int param, string paramName)
        {
            if (param <= 0)
            {
                ExceptionUtil.ThrowArgumentOutOfRangeException(paramName, Resources.ParameterMustBePositive);
            }
        }
    }
}
