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
using LibHeifSharp.Properties;
using LibHeifSharp.ResourceManagement;

namespace LibHeifSharp
{
    public sealed partial class UserDescriptionProperty
    {
        internal sealed class NativeData : Disposable
        {
            private SafeCoTaskMemHandle description;
            private SafeCoTaskMemHandle language;
            private SafeCoTaskMemHandle name;
            private SafeCoTaskMemHandle tags;

            private readonly bool descriptionHandleNeedsRelease;
            private readonly bool languageHandleNeedsRelease;
            private readonly bool nameHandleNeedsRelease;
            private readonly bool tagsHandleNeedsRelease;

            public NativeData(ref SafeCoTaskMemHandle descriptionStringHandle,
                              ref SafeCoTaskMemHandle languageStringHandle,
                              ref SafeCoTaskMemHandle nameStringHandle,
                              ref SafeCoTaskMemHandle tagsStringHandle)
            {
                this.description = descriptionStringHandle;
                this.language = languageStringHandle;
                this.name = nameStringHandle;
                this.tags = tagsStringHandle;

                this.description.DangerousAddRef(ref this.descriptionHandleNeedsRelease);
                this.language.DangerousAddRef(ref this.languageHandleNeedsRelease);
                this.name.DangerousAddRef(ref this.nameHandleNeedsRelease);
                this.tags.DangerousAddRef(ref this.tagsHandleNeedsRelease);

                // Inform the caller that this class has taken ownership of the handles.
                descriptionStringHandle = null;
                languageStringHandle = null;
                nameStringHandle = null;
                tagsStringHandle = null;
            }

            /// <summary>
            /// Creates the native structure.
            /// </summary>
            /// <returns>The native structure.</returns>
            /// <exception cref="HeifException">
            /// Failed to increment the reference count on the native string data.
            /// </exception>
            /// <exception cref="ObjectDisposedException">The object has been disposed.</exception>
            public heif_property_user_description CreateNativeStructure()
            {
                VerifyNotDisposed();

                if (this.descriptionHandleNeedsRelease
                    && this.languageHandleNeedsRelease
                    && this.nameHandleNeedsRelease
                    && this.tagsHandleNeedsRelease)
                {
                    return new heif_property_user_description()
                    {
                        version = 1,
                        description = this.description.DangerousGetHandle(),
                        language = this.language.DangerousGetHandle(),
                        name = this.name.DangerousGetHandle(),
                        tags = this.tags.DangerousGetHandle(),
                    };
                }
                else
                {
                    throw new HeifException(Resources.UserDescriptionAddRefFailed);
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (this.descriptionHandleNeedsRelease)
                    {
                        this.description.DangerousRelease();
                    }

                    if (this.languageHandleNeedsRelease)
                    {
                        this.language.DangerousRelease();
                    }

                    if (this.nameHandleNeedsRelease)
                    {
                        this.name.DangerousRelease();
                    }

                    if (this.tagsHandleNeedsRelease)
                    {
                        this.tags.DangerousRelease();
                    }

                    DisposableUtil.Free(ref this.description);
                    DisposableUtil.Free(ref this.language);
                    DisposableUtil.Free(ref this.name);
                    DisposableUtil.Free(ref this.tags);
                }

                base.Dispose(disposing);
            }
        }
    }
}
