# libheif-sharp

libheif-sharp provides .NET bindings for [libheif](https://github.com/strukturag/libheif).   
It is built against [.NET Standard 1.3](https://docs.microsoft.com/en-us/dotnet/standard/net-standard), and should work on any supported .NET platform that has a libheif shared library.

### Features

* Supports decoding
    * Top-level images
    * Thumbnails
    * Depth images
* Supports encoding
    *  Top-level images
    *  Thumbnails
* Supports reading and writing meta-data

### Sample Applications

The [libheif-sharp-samples](https://github.com/0xC0000054/libheif-sharp-samples) repository contains decoder and encoder samples that demonstrate the use of the library.

### Requirements

The libheif shared library must be named `libheif` in order for it to be found by P/Invoke.
If your program or OS uses a different library name, you will need to use [SetDllImportResolver](https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.nativelibrary.setdllimportresolver?view=netcore-3.1) to redirect searches for `libheif` to the appropriate library.

The minimum supported libheif version is 1.9.0.

## License

This project is licensed under the terms of the GNU Lesser General Public License version 3.0.   
See [License.md](License.md) for more information.