# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed

* The `SetParameter(string, string)` overload will now try to convert Boolean and integer values to the correct type, an exception will be thrown if the conversion fails.
* `GetExifMetadata()` will now return `null` if the TIFF header offset is not valid.
* Mark the `HeifItemId` structure as readonly.
* The name of the image parameter in the `EncodeImage` method. **(breaking change)**

### Fixed

* A number of issues with the documentation. 

## v1.0.0

### Added

First version

