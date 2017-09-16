# Attributions

F# and the Visual F# tools have had significant community contributions.  This document exists to attribute work and let people know who is behind their favorite features.

This document may not be exhaustive or attribute everyone.  If you have implemented a feature or done some improvements in an area, please feel free to make a pull request on this document to attribute yourself!

## Language, Compiler, and FSharp.Core

This is for those who contributed language features, compiler improvements, or improvements or additions to FSharp.Core.

### F# 4.1 Language Features

* Separators in numeric literals - [Avi Avni](https://github.com/aviavni)
* Caller Info Argument Attributes - [Lincoln Atkinson](https://github.com/latkin) and [Avi Avni](https://github.com/aviavni)
* Struct records - [Will Smith](https://github.com/tihan)
* `Result` type and associated functions - [Oskar Gewalli](https://github.com/wallymathieu)

### F# 4.1 Compiler improvements

**Error Message Improvements**

- [Steffen Forkmann](https://github.com/forki)
- [Isaac Abraham](https://github.com/isaacabraham)
- [Libo Zeng](https://github.com/liboz)
- [Gauthier Segay](https://github.com/smoothdeveloper)
- [Richard Minerich](https://github.com/Rickasaurus)
- [Jared Hester](https://github.com/cloudroutine)

**Performance Improvements**

- [Gustavo Leon](https://github.com/gmpl)
- [Steffen Forkmann](https://github.com/forki)
- [Libo Zeng](https://github.com/liboz)
- [Rikki Gibson](https://github.com/RikkiGibson)

**SRTP Improvements**

- [Gustavo Leon](https://github.com/gmpl)


### F# 4.2 Compiler Improvements

- Deterministic compilation via `--deterministic`, by [David Glassborow](https://github.com/davidglassborow)

### FSharp.Core

**Performance Improvements**

- [Jack Mott](https://github.com/jackmott)
- [Steffen Forkmann](https://github.com/forki)
- [Libo Zeng](https://github.com/liboz)
- [Paul Westcott](https://github.com/manofstick)
- [Zp Babbi](https://github.com/zpbappi)
- [Victor Baybekov](https://github.com/buybackoff)
- [Saul Rennison](https://github.com/saul)

**Interop Improvements**

- [Eirik Tsarpalis](https://github.com/eiriktsarpalis)

**General Improvements**

- [Jérémie Chassaing](https://github.com/thinkbeforecoding)

## Tooling - Visual Studio and Platform Support

This is for those who contributed Visual Studio IDE features and platform support for F#.

### Visual F# for Visual Studio 2017

**Editor Features**

* Semantic Colorization - [Vasily Kirichenko](https://github.com/vasily-kirichenko) and [Saul Rennison](https://github.com/saul)
* Autocompletion - [Vasily Kirichenko](https://github.com/vasily-kirichenko) and [Saul Rennison](https://github.com/saul)
* IntelliSense Filters and Glyph improvements - [Vasily Kirichenko](https://github.com/vasily-kirichenko)
* IntelliSense accuracy Improvements - [Vasily Kirichenko](https://github.com/vasily-kirichenko)
* Go To All - [Vasily Kirichenko](https://github.com/vasily-kirichenko) and [Jared Hester](github.com/cloudroutine)
* Find All References - [Vasily Kirichenko](https://github.com/vasily-kirichenko)
* Re-enabled multiple F# project support - [Ahn-Dung Phan](https://github.com/dungpa)
* QuickInfo (hover tooltips) Improvements - [Vasily Kirichenko](https://github.com/vasily-kirichenko) and [Jared Hester](github.com/cloudroutine)
* Module and Namespace colorization in the editor - [Vasily Kirichenko](https://github.com/vasily-kirichenko)
* Navigation Bar Support re-enabled and improved - [Vasily Kirichenko](https://github.com/vasily-kirichenko)
* Semantic highlighting of tokens - [Vasily Kirichenko](https://github.com/vasily-kirichenko)
* Structured Guidelines - [Vasily Kirichenko](https://github.com/vasily-kirichenko) and [Jared Hester](https://github.com/cloudRoutine)
* F1 Help Service re-enabled - [Robert Jeppesen](https://github.com/rojepp)
* Colorization in QuickInfo and Signature Help - [Vladimir Matveev](https://github.com/vladima)
* Code Indentation Improvements - [Ahn-Dung Phan](https://github.com/dungpa)
* Error Reporting Improvements in the IDE - [Ahn-Dung Phan](https://github.com/dungpa)
* Inline Rename - [Vasily Kirichenko](https://github.com/vasily-kirichenko)
* Go to Definition Improvements - [Vasily Kirichenko](https://github.com/vasily-kirichenko) and [Ahn-Dung Phan](https://github.com/dungpa)
* Breakpoint resolution improvements - Vasily Kirichenko](https://github.com/vasily-kirichenko) and [Steffen Forkmann](https://github.com/forki)
* Respecting `EditorBrowsable(EditorBrowsableState.Never)` attribute - [Vasily Kirichenko](https://github.com/vasily-kirichenko)
* XML Documentation Generation - [Ahn-Dung Phan](https://github.com/dungpa)
* Clickable items in QuickInfo (hover tooltips) which invoke Go to Definition - [Jakub Majocha](https://github.com/majocha), [Jared Hester](https://github.com/cloudRoutine), and [Vasily Kirichenko](https://github.com/vasily-kirichenko)
* Separate color themes for light and dark mode - [Jakub Majocha](https://github.com/majocha)
* Semantic highlighting - [Vasily Kirichenko](https://github.com/vasily-kirichenko) and [Jared Hester](github.com/cloudroutine)
* ReSharper-like ordering in Completion lists - [Vasily Kirichenko](https://github.com/vasily-kirichenko)
* Collapse to Definition - [Vasily Kirichenko](https://github.com/vasily-kirichenko)
* Support for Editor Settings - [Jakub Majocha](https://github.com/majocha)
* Localized Go to Definition Status Bar - [Saul Rennison](https://github.com/saul)
* R#-like completion for items in unopened namespaces - [Vasily Kirichenko](https://github.com/vasily-kirichenko)
* Wrapped XML docs in QuickInfo - [Jakub Majocha](https://github.com/majocha)
* Smart indent and de-indent - [Duc Nghiem Xuan](https://github.com/xuanduc987) and [Saul Rennison](https://github.com/saul)
* F# to C# navigation - [Saul Rennison](https://github.com/saul) and [Vasily Kirichenko](https://github.com/vasily-kirichenko)
* Support for Blue Theme (High Contrast) in semantic colorization - [Vasily Kirichenko](https://github.com/vasily-kirichenko)
* Configuration-driven in-memory cross-project references and project cache size - [Vasily Kirichenko](https://github.com/vasily-kirichenko)


**Project System**

* Improved solution load time - [Saul Rennison](https://github.com/saul)
* General improvements - [Jakub Majocha](https://github.com/majocha)
* Move Up/Move Down on Solution folder nodes - [Saul Rennison](https://github.com/saul)
* Folder support - [Saul Rennison](https://github.com/saul)

**F# Interactive**

* Colorized FSI.exe - [Saul Rennison](https://github.com/saul)

**Code Fixes and analyzers**

* Uppercase Identifiers for Record Labels and Unions Cases Analyzer and codefix - [Steffen Forkmann](https://github.com/forki)
* Implement Interface Analyzer and Codefix - [Ahn-Dung Phan](https://github.com/dungpa)
* Replacements for Unknown Identifiers Codefix (by Steffen Forkmann).
* Prefix or Replace Unused Value with Underscore Analyzer and Codefix - [Vasily Kirichenko](https://github.com/vasily-kirichenko)
* Add new Keyword Analyzer and Codefix - [Vasily Kirichenko](https://github.com/vasily-kirichenko)
* Add open Statement Analyzer and Codefix - [Vasily Kirichenko](https://github.com/vasily-kirichenko)
* Simplify Name Analyzer and Codefix - [Vasily Kirichenko](https://github.com/vasily-kirichenko)
* Gray Out Unused Values - [Vasily Kirichenko](https://github.com/vasily-kirichenko)
* Unused Declarations Analyzer and Codefix - [Vasily Kirichenko](https://github.com/vasily-kirichenko)
* Add reference to \<assembly\> analyzer and codefix - [Saul Rennison](https://github.com/saul)

### .NET Core Support

* F# support on the .NET Core SDK - [Enrico Sada](https://github.com/enricosada)
* F# templates for .NET Core - [Enrico Sada](https://github.com/enricosada)

## Infrastructure

Infrastructure isn't the sexiest stuff in the world, but it's absolutely necessary to the success of F#.  The following community members helped F# and Visual F# infrastructure.

* [Jack Pappas](https://github.com/jack-pappas)
* [Enrico Sada](https://github.com/enricosada)
* [Saul Rennison](https://github.com/saul)
* [Alfonso Garcia-Caro](https://github.com/alfonsogarciacaro)
* [Zp Babbi](https://github.com/zpbappi)
* [Gauthier Segay](https://github.com/smoothdeveloper)
* [Jared Hester](github.com/cloudroutine)
* [Cameron Taggert](https://github.com/ctaggart)
