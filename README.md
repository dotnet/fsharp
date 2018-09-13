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

| Branch | Status |
|:------:|:------:|
|master|[![Build Status](https://dnceng.visualstudio.com/_apis/public/build/definitions/9ee6d478-d288-47f7-aacc-f6e6d082ae6d/106/badge?branchname=master)](https://dnceng.visualstudio.com/public/public%20Team/_build?definitionId=106&_a=history)|
|dev15.9|[![Build Status](https://dnceng.visualstudio.com/_apis/public/build/definitions/9ee6d478-d288-47f7-aacc-f6e6d082ae6d/106/badge?branchname=dev15.9)](https://dnceng.visualstudio.com/public/public%20Team/_build?definitionId=106&_a=history)|
|dev16.0|[![Build Status](https://dnceng.visualstudio.com/_apis/public/build/definitions/9ee6d478-d288-47f7-aacc-f6e6d082ae6d/106/badge?branchname=dev16.0)](https://dnceng.visualstudio.com/public/public%20Team/_build?definitionId=106&_a=history)|

## Help improve the Quality of the Tools by Using the Nightly Releases of Visual F# Tools
To setup Visual Studio to use the latest nightly releases of the Visual F# Tools:
https://blogs.msdn.microsoft.com/dotnet/2017/03/14/announcing-nightly-releases-for-the-visual-f-tools/


## Contributing

See [DEVGUIDE.md](DEVGUIDE.md) and [TESTGUIDE.md](TESTGUIDE.md) for details on build, development, and testing.

See [CONTRIBUTING.md](CONTRIBUTING.md) for general guidelines on the contribution process, also [how we label issues and PRs](https://github.com/dotnet/roslyn/wiki/Labels-used-for-issues)

To contribute to the F# ecosystem more generally see the F# Software Foundation's [Community Projects](http://fsharp.org/community/projects/) pages.

## Branches

These are the branches in use:

* `master` = Latest branch for OSS developers and nightly users.  
  - Most contributions go here.
  - Able to be built, installed and used in the latest public Visual Studio release.
  - May contain updated F# features and logic.
  - Used to build nightly VSIX (see above).
  - Gets integrated into https://github.com/fsharp/fsharp to form the basis of Mono releases
  - Gets integrated into https://github.com/fsharp/FSharp.Compiler.Service to form the basis of FSharp.Compiler.Service releases

* `dev15.x`
  - Latest release branch for the particular point release of Visual Studio.
  - Incorporates features and fixes from master up to a particular branch point, then selective cherry-picks.
  - May contain new features that depend on new things or fixes in the corresponding Visual Studio release.
  - Gets integrated back into master once the corresponding Visual Studio release is made.
  - Used to build Visual F# Tool updates


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

If you wish to set up a *Preview* nightly atop Visual Studio preview builds, you can either [download a VSIX Manually from here](https://dotnet.myget.org/feed/fsharp-preview/package/vsix/VisualFSharp) or set up a VSIX feed in visual studio from [here](https://dotnet.myget.org/F/fsharp-preview/vsix).

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
