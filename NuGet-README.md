# libheif-sharp

libheif-sharp provides .NET bindings for [libheif](https://github.com/strukturag/libheif).   
It is built against [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard), and should work on any supported .NET platform that has a libheif shared library.

### Features

* Supports decoding
    * Top-level images
    * Thumbnails
    * Depth images
	* Vendor-specific auxiliary images
* Supports encoding
    *  Top-level images
    *  Thumbnails
* Supports reading and writing meta-data

### Documentation

[API Documentation](https://0xc0000054.github.io/libheif-sharp/API/index.html)   
[Building libheif on Windows with vcpkg](https://0xc0000054.github.io/libheif-sharp/libheif_windows_build_vcpkg.html)

### Sample Applications

The [libheif-sharp-samples](https://github.com/0xC0000054/libheif-sharp-samples) repository contains sample applications that demonstrate the use of the library.

### Requirements

The libheif shared library must be named `libheif` in order for it to be found by P/Invoke.

The minimum supported libheif version is 1.9.0.
