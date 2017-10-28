# The F# Language, Library, and  Visual F# Tools Repository

You are invited to help produce future releases of the F# language compiler, library, and tools. This repository enables development on Linux, macOS and Windows, along with some automated CI testing for these.

* [About F#](http://fsharp.org)
* [Testimonials](http://fsharp.org/testimonials)
* [Contributing](#contributing)
* [Using](#using)

The F# Compiler and Tools are also mirrored in [the corresponding repository](http://github.com/fsharp/fsharp) of the F# Software Foundation.

Changes contributed here are eventually propagated to this repository and are included in all packagings of F# and open source F# editing tools. The process for doing this is explained in this guide by the [F# Core Engineering Group](https://fsharp.github.io/2014/06/18/fsharp-contributions.html). Currently, the F# community coordinates packaging [other editions of F#](https://github.com/fsharp/fsharp/) for use on Linux, macOS, Android, iOS, and other platforms, and Microsoft coordinates packaging this repository as part of the Visual F# Tools. 

For historical reasons this repository is called "visualfsharp" and currently also contains the Visual F# IDE Tools. The eventual plan is to split these repositories into "fsharp" and "visualfsharp".


## Build Status

|            | Ubuntu (Build) | Windows (Debug Build) | Windows (Release Tests 1) | Windows (Release Tests 2) | Windows  (Release Tests 3) |
|:----------:|:----------------:|:----------------:|:------------------:|:-----------------------:|:---------------------:|
|**master**  |[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/master/release_ubuntu14.04)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/master/job/release_ubuntu14.04/)|[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/master/debug_windows_nt)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/master/job/debug_windows_nt/)|[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/master/release_ci_part1_windows_nt)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/master/job/release_ci_part1_windows_nt/)|[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/master/release_ci_part2_windows_nt)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/master/job/release_ci_part2_windows_nt/)|[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/master/release_ci_part3_windows_nt)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/master/job/release_ci_part3_windows_nt/)|
|**dev15.5**  |[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/dev15.5/release_ubuntu14.04)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/dev15.5/job/release_ubuntu14.04/)|[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/dev15.5/debug_windows_nt)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/dev15.5/job/debug_windows_nt/)|[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/dev15.5/release_ci_part1_windows_nt)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/dev15.5/job/release_ci_part1_windows_nt/)|[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/dev15.5/release_ci_part2_windows_nt)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/dev15.5/job/release_ci_part2_windows_nt/)|[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/dev15.5/release_ci_part3_windows_nt)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/dev15.5/job/release_ci_part3_windows_nt/)|
|**dev15.6**  |[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/dev15.6/release_ubuntu14.04)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/dev15.6/job/release_ubuntu14.04/)|[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/dev15.6/debug_windows_nt)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/dev15.6/job/debug_windows_nt/)|[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/dev15.6/release_ci_part1_windows_nt)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/dev15.6/job/release_ci_part1_windows_nt/)|[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/dev15.6/release_ci_part2_windows_nt)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/dev15.6/job/release_ci_part2_windows_nt/)|[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/dev15.6/release_ci_part3_windows_nt)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/dev15.6/job/release_ci_part3_windows_nt/)|


## Help improve the Quality of the Tools by Using the Nightly Releases of Visual F# Tools
To setup Visual Studio to use the latest nightly releases of the Visual F# Tools:
https://blogs.msdn.microsoft.com/dotnet/2017/03/14/announcing-nightly-releases-for-the-visual-f-tools/


## Contributing

See [DEVGUIDE.md](DEVGUIDE.md) and [TESTGUIDE.md](TESTGUIDE.md) for details on build, development, and testing.

See [CONTRIBUTING.md](CONTRIBUTING.md) for general guidelines on the contribution process, also [how we label issues and PRs](https://github.com/dotnet/roslyn/wiki/Labels-used-for-issues)

To contribute to the F# ecosystem more generally see the F# Software Foundation's [Community Projects](http://fsharp.org/community/projects/) pages.


### Technical Documentation

* [The F# Language and Core Library RFC Process](http://fsharp.github.io/2016/09/26/fsharp-rfc-process.html)

* [The F# Language Specification](http://fsharp.org/specs/language-spec/)

* [The F# Compiler Technical Guide](http://fsharp.github.io/2015/09/29/fsharp-compiler-guide.html) 
  maintained by contributors to this repository.  Please read
  and contribute to that guide.

### License

This project is subject to the MIT License. A copy of this license can be found in [License.txt](License.txt) at the root of this repo.


## Using

For typical installs of  F#, see http://fsharp.org.

### Using Nightly Releases of Visual F# Tools

To setup Visual Studio to use the latest nightly releases of the Visual F# Tools:

https://blogs.msdn.microsoft.com/dotnet/2017/03/14/announcing-nightly-releases-for-the-visual-f-tools/

### Using CI Builds

To install F#, see http://fsharp.org.

To download the bits for the latest CI builds see [these instructions](https://github.com/Microsoft/visualfsharp/wiki/Using-CI-Builds). This includes and ZIPs containing the F# compiler and VSIX installers for the Visual F# IDE Tools.

### Using F# on a build server or computer without an F# installation

If you wish to use the latest F# compiler on a computer without Visual Studio 2017 installed, you can add the nuget package ``FSharp.Compiler.Tools`` to your projects. This will replace the in-box compiler with the version contained in the package.
The actual package is built in https://github.com/fsharp/fsharp.

You will need to adjust the targets reference on your project file to use the targets file from the installed ``FSharp.Compiler.Tools`` package.
See https://github.com/fsharp/fsharp/issues/676 for how to modify your project file.

## Code of Conduct

This project has adopted the code of conduct defined by the [Contributor Covenant](http://contributor-covenant.org/) to clarify expected behavior in our community. This code of conduct has been [adopted by many other projects](http://contributor-covenant.org/adopters/). For more information see the [Code of conduct](https://github.com/Microsoft/visualfsharp/wiki/Code-of-Conduct).

## Get In Touch

Follow [@VisualFSharp](https://twitter.com/VisualFSharp) and [@fsharporg](https://twitter.com/fsharporg) on twitter and subscribe to the [.NET Blog](https://blogs.msdn.microsoft.com/dotnet/).

Members of the F# Software Foundation can be invited to the "F# Software Foundation" discussion rooms on slack. More details at http://fsharp.org/guides/slack/.
