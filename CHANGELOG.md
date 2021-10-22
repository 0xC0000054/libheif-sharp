# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

* An `IsPremultipliedAlpha` property to the `HeifImage` and `HeifImageHandle` classes.
  * This property is supported on LibHeif 1.12 and later.

### Fixed

* A potential crash when creating the `HeifDepthRepresentationInfo`.

## v2.1.0 - 2021-05-19

### Deprecated

* The `ColorProfile` property in the `HeifImage` class, use the `IccColorProfile` and `NclxColorProfile` properties instead.

### Added

* A `ValidValues` property to the `HeifIntegerEncoderParameter` class.
  * This is used to get the supported values for the rav1e `tile-cols` and `tile-rows` parameters.
  * Requires LibHeif version 1.10 or later.
* Support for the new encoding options in LibHeif versions 1.9.2, 1.10 and 1.11.
* Support for reading and writing images with two color profiles (one ICC profile and one NCLX profile).
   * This feature requires LibHeif version 1.10 or later, you can use the `CanWriteTwoColorProfiles` property in the `LibHeifInfo` class for runtime checks.
   * It must be enabled in the `HeifEncodingOptions` instance by setting the `WriteTwoColorProfiles` property to `true`.
* Support for reading the vendor-specific auxiliary images.
  * Added `GetAuxiliaryImage`, `GetAuxiliaryImageIds` and `GetAuxiliaryType` methods to the `HeifImageHandle` class.
  * These methods require LibHeif version 1.11 or later.
* An `AuxiliaryImageType` property to the `HeifImageHandle` class.

### Fixed

* A corrupted image bug when using lossless encoding.
* A few issues with the documentation.

## v2.0.2 - 2020-12-29

### Fixed

A missing marshaling attribute on the write callback.

## v2.0.1 - 2020-11-24

### Changed

* Reduced memory usage when reading small files.

### Fixed

* A few issues with the documentation.
* A compatibility issue with some classes that are derived from `MemoryStream`.

## v2.0.0 - 2020-11-11

### Breaking

* The `thumbnail` parameter in the `EncodeImage` method has been renamed to `image`.
* `ReadFromMemory` now throws an `ArgumentException` if the array length is zero.
* `AddPlane` now throws an `ArgumentOutOfRangeException` if the `width`, `height` or `bitDepth` parameters are less than or equal to 0.

### Deprecated

* The `ReadFromFile` method, use the `HeifContext(string)` constructor overload instead.
* The `ReadFromMemory` method, use the `HeifContext(byte[])` constructor overload instead.

### Added

* A `SetMaximumImageSizeLimit` method to the `HeifContext` class.
* A `WriteToStream` method to the `HeifContext` class.
* A few `HeifContext` constructor overloads that read the image data.
  * `HeifContext(string)` replaces the `ReadFromFile` method.
  * `HeifContext(byte[])` replaces the `ReadFromMemory` method.
  * `HeifContext(Stream, bool)` reads from a Stream, and optionally leaves the stream open after the HeifContext has been disposed.

### Changed

* The `SetParameter(string, string)` overload will now try to convert Boolean and integer values to the correct type, an exception will be thrown if the conversion fails.
* `GetExifMetadata()` will now return `null` if the TIFF header offset is not valid.
* Mark the `HeifItemId` structure as readonly.
* Reduce memory usage when reading a HEIF image from a byte array.

### Fixed

* A number of issues with the documentation. 

## v1.0.0 - 2020-10-21

### Added

First version

