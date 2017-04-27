
[![Join the chat at https://gitter.im/Microsoft/visualfsharp](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Microsoft/visualfsharp?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

# The Combined F# Language, Library, and Tools Repository

## Build Status

|            | Ubuntu (Build) | Windows (Debug Build) | Windows (Release Tests 1) | Windows (Release Tests 2) | Windows  (Release Tests 3) | Windows (Release Tests 4) |
|:----------:|:----------------:|:----------------:|:------------------:|:-----------------------:|:---------------------:|:----------:|
|**master**  |[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/master/release_ubuntu14.04)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/master/job/release_ubuntu14.04/)|[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/master/debug_windows_nt)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/master/job/debug_windows_nt/)|[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/master/release_ci_part1_windows_nt)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/master/job/release_ci_part1_windows_nt/)|[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/master/release_ci_part2_windows_nt)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/master/job/release_ci_part2_windows_nt/)|[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/master/release_ci_part3_windows_nt)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/master/job/release_ci_part3_windows_nt/)|[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/master/release_ci_part4_windows_nt)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/master/job/release_ci_part4_windows_nt/)|

### Contributing to the F# Language, Library, and Tools

You are invited to to help produce future releases of the F# language compiler, library, and tools.

F# is a mature, open source, cross-platform, functional-first programming language which empowers users and organizations to tackle complex computing problems with simple, maintainable, and robust code. F# is used in a wide range of application areas and is supported by Microsoft and other industry-leading companies providing professional tools, and by an active open community. You can find out more about F# at http://fsharp.org.

Changes contributed here are eventually included in all packagings of F# and open source F# editing tools. Microsoft coordinates packaging this repository
as part of the Visual F# Tools, and the F# community coordinates packaging [other editions of F#](https://github.com/fsharp/fsharp/) for use on Linux, macOS, Android, iOS, and other platforms.

### License

This project is subject to the Apache Licence, Version 2.0. A copy of the license can be found in [License.txt](License.txt) at the root of this repo.

### Development and Testing

See [DEVGUIDE.md](DEVGUIDE.md) and [TESTGUIDE.md](TESTGUIDE.md) in the root of the repo for details on build, development, and testing.

#### Contributing

Guidelines for contributions to the F# compiler, library, and Visual F# IDE tools can be found in the [CONTRIBUTING.md](CONTRIBUTING.md) document.

How we label issues and PRs:  https://github.com/dotnet/roslyn/wiki/Labels-used-for-issues  

If you would like to contribute to the F# ecosystem more generally see the F# Software Foundation's [Community Projects](http://fsharp.org/community/projects/) pages.


### Using Nightly Releases of Visual F# Tools

To setup Visual Studio to use the latest nightly releases of the Visual F# Tools, follow the [Using CI Builds](https://github.com/Microsoft/visualfsharp/wiki/Using-CI-Builds) instructions.

### Using CI Builds

To install F#, see http://fsharp.org.

To download the bits for the latest CI builds see [these instructions](https://github.com/Microsoft/visualfsharp/wiki/Using-CI-Builds). This includes and ZIPs containing the F# compiler and VSIX installers for the Visual F# IDE Tools.

### Code Flow to Other Platforms

This repository enables development on Windows, Linux and macOS.  It enables automated CI testing primarily on Windows.

If using Android, or iOS, and would like to contribute, please see the instructions provided at the [Open Edition repo](https://github.com/fsharp/fsharp/#the-open-edition-of-the-f-compiler-core-library--tools).

Although the primary focus of this repo is F# for Windows and the Visual Studio F# tools, contributions here flow directly to the F# Open Edition repo.  More details can be found [here](https://github.com/Microsoft/visualfsharp/wiki/Code-Flow-to-Open-Edition).

### Using F# on a buildserver or computer without VS 2017 or without the optional F# tools

If you wish to use the latest F# compiler on a computer without Visual Studio 2017 installed, you can add the nuget package ``FSharp.Compiler.Tools`` to your projects. This will replace the in-box compiler with the version contained in the package.
The actual package is built in https://github.com/fsharp/fsharp.

You will need to adjust the targets reference on your project file to use the targets file from the installed ``FSharp.Compiler.Tools`` package.
See https://github.com/fsharp/fsharp/issues/676 for how to modify your project file.

### Code of Conduct

This project has adopted the code of conduct defined by the [Contributor Covenant](http://contributor-covenant.org/) to clarify expected behavior in our community. This code of conduct has been [adopted by many other projects](http://contributor-covenant.org/adopters/). For more information see the [Code of conduct](https://github.com/Microsoft/visualfsharp/wiki/Code-of-Conduct).

### Get In Touch

Follow [@VisualFSharp](https://twitter.com/VisualFSharp) on twitter or subscribe to the [.NET Blog](https://blogs.msdn.microsoft.com/dotnet/).

