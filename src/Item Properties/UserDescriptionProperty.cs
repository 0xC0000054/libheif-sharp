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

using LibHeifSharp.Interop;

namespace LibHeifSharp
{
    /// <summary>
    /// The user description property
    /// </summary>
    /// <seealso cref="HeifContext.AddUserDescriptionProperty(HeifItemId, UserDescriptionProperty)"/>
    /// <seealso cref="HeifContext.AddUserDescriptionProperty(HeifImageHandle, UserDescriptionProperty)"/>
    /// <seealso cref="HeifContext.AddUserDescriptionProperty(HeifRegionItem, UserDescriptionProperty)"/>
    /// <seealso cref="HeifContext.GetUserDescriptionProperties(HeifItemId)"/>
    /// <seealso cref="HeifContext.GetUserDescriptionProperties(HeifImageHandle)"/>
    /// <seealso cref="HeifContext.GetUserDescriptionProperties(HeifRegionItem)"/>
    /// <threadsafety static="true" instance="false"/>
    public sealed partial class UserDescriptionProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDescriptionProperty"/> class.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="language">The language.</param>
        /// <param name="name">The name.</param>
        /// <param name="tags">The tags.</param>
        public UserDescriptionProperty(string description,
                                       string language,
                                       string name,
                                       string tags)
        {
            this.Description = description ?? string.Empty;
            this.Language = language ?? string.Empty;
            this.Name = name ?? string.Empty;
            this.Tags = tags ?? string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDescriptionProperty"/> class.
        /// </summary>
        /// <param name="native">The native structure.</param>
        /// <exception cref="HeifException">
        /// The string contains characters that are not valid for the encoding.
        /// </exception>
        internal unsafe UserDescriptionProperty(heif_property_user_description* native)
        {
            this.Description = StringMarshaling.FromNative(native->description);
            this.Language = StringMarshaling.FromNative(native->language);
            this.Name = StringMarshaling.FromNative(native->name);
            this.Tags = StringMarshaling.FromNative(native->tags);
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; }

        /// <summary>
        /// Gets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Language { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the tags.
        /// </summary>
        /// <value>
        /// The tags.
        /// </value>
        public string Tags { get; }

        /// <summary>
        /// Converts the data to its native format.
        /// </summary>
        /// <returns>A class containing the converted data.</returns>
        /// <exception cref="HeifException">
        /// The string contains characters that are not valid for the encoding.
        /// </exception>
        internal NativeData ToNative()
        {
            NativeData nativeData;

            SafeCoTaskMemHandle descriptionStringHandle = null;
            SafeCoTaskMemHandle languageStringHandle = null;
            SafeCoTaskMemHandle nameStringHandle = null;
            SafeCoTaskMemHandle tagsStringHandle = null;

            try
            {
                descriptionStringHandle = StringMarshaling.ToNative(this.Description);
                languageStringHandle = StringMarshaling.ToNative(this.Language);
                nameStringHandle = StringMarshaling.ToNative(this.Name);
                tagsStringHandle = StringMarshaling.ToNative(this.Tags);

                nativeData = new NativeData(ref descriptionStringHandle,
                                            ref languageStringHandle,
                                            ref nameStringHandle,
                                            ref tagsStringHandle);
            }
            finally
            {
                descriptionStringHandle?.Dispose();
                languageStringHandle?.Dispose();
                nameStringHandle?.Dispose();
                tagsStringHandle?.Dispose();
            }

            return nativeData;
        }
    }
}
