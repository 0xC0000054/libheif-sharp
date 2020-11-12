# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Fixed

* A few issues with the documentation.

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

