
[![Join the chat at https://gitter.im/Microsoft/visualfsharp](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Microsoft/visualfsharp?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

# Visual F# Tools

## Windows build

|            |Debug (Build only)|Release (Tests Part 1)|Release (Tests Part 2)|Release (Tests Part 3)|
|:----------:|:----------------:|:------------------:|:-----------------------:|:---------------------:|
|**master**  |[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/master/debug_windows_nt)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/master/job/debug_windows_nt/)|[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/master/release_ci_part1_windows_nt)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/master/job/release_ci_part1_windows_nt/)|[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/master/release_ci_part2_windows_nt)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/master/job/release_ci_part2_windows_nt/)|[![Build Status](https://ci2.dot.net/buildStatus/icon?job=Microsoft_visualfsharp/master/release_ci_part3_windows_nt)](https://ci2.dot.net/job/Microsoft_visualfsharp/job/master/job/release_ci_part3_windows_nt/)|

### Contributing to the F# Language, Library, and Tools

You are invited to join with the F# Community and the Visual F# Tools team to help produce future releases of the F# language compiler, library, and tools.

F# is a mature, open source, cross-platform, functional-first programming language which empowers users and organizations to tackle complex computing problems with simple, maintainable, and robust code. F# is used in a wide range of application areas and is supported by Microsoft and other industry-leading companies providing professional tools, and by an active open community. You can find out more about F# at http://fsharp.org.

Changes contributed here are packaged into the Visual F# Tools, the F# Open Edition, and other open source F# editing tools. Microsoft coordinates packaging this repository as part of the Visual F# Tools, while the F# community coordinates packaging it as the Open Edition of F# for use on Linux, OSX, Android, iOS, and other platforms, via the [fsharp/fsharp GitHub repo](https://github.com/fsharp/fsharp/).

### Engineering status

[F# for CoreCLR status](https://github.com/Microsoft/visualfsharp/wiki/F%23-for-CoreCLR---Status)

[F# 4.0 status](https://github.com/Microsoft/visualfsharp/wiki/F%23-4.0-Status)   --- Completed, shipped.

### License

This project is subject to the Apache Licence, Version 2.0. A copy of the license can be found in [License.txt](License.txt) at the root of this repo.

### Development and Testing

See [DEVGUIDE.md](DEVGUIDE.md) and [TESTGUIDE.md](TESTGUIDE.md) in the root of the repo for details on build, development, and testing.
 
### Required Tools for Windows Development and Testing

#### Development tools

For F# Compiler on Windows (``build net40``)

- [.NET 4.5.1](http://www.microsoft.com/en-us/download/details.aspx?id=40779)
- [MSBuild 12.0](http://www.microsoft.com/en-us/download/details.aspx?id=40760)

For F# Compiler on OSX and Linux (see .travis.yml for build steps)

- [Mono latest](http://www.mono-project.com/download/#download-lin)
- If building for .NET Core, then .NET Core will be downloaded from Linux packages


For Visual F# IDE Tools 4.1 development (Windows)

- [Visual Studio 2017](https://www.visualstudio.com/downloads/)
  - Under the "Windows" workloads, select ".NET desktop development".
    - Select "F# language suport" under the optional components.
  - Under the "Other Toolsets" workloads, select "Visual Studio extension development".
  - Under the "Individual Components" tab select "Windows 10 SDK" as shown below (needed for compiling RC resource, see #2556):
  ![image](https://cloud.githubusercontent.com/assets/1249087/23730261/5c78c850-041b-11e7-9d9d-62766351fd0f.png)


#### Additional frameworks

- [Git for windows](http://msysgit.github.io/)
- [Perl](http://www.perl.org/get.html#win32) (ActiveState 5.16.3 is known to be supported)
- [.NET 3.5](http://www.microsoft.com/en-us/download/details.aspx?id=21)
- [.NET 4.5](http://www.microsoft.com/en-us/download/details.aspx?id=30653)
- [.NET 4.5.1](http://www.microsoft.com/en-us/download/details.aspx?id=40779)
- [.NET 4.6](http://www.microsoft.com/en-us/download/details.aspx?id=48137)
- [MSBuild 12.0](http://www.microsoft.com/en-us/download/details.aspx?id=40760)
- [Windows 7 SDK](http://www.microsoft.com/en-us/download/details.aspx?id=8279)
- [Windows 8 SDK](http://msdn.microsoft.com/en-us/windows/desktop/hh852363.aspx)
- [Windows 8.1 SDK](http://msdn.microsoft.com/en-us/library/windows/desktop/bg162891.aspx)
- [Windows 10 SDK](https://developer.microsoft.com/en-US/windows/downloads/windows-10-sdk)

#### Contributing

Guidelines for contributions to the F# compiler, library, and Visual F# IDE tools can be found [here](CONTRIBUTING.md).

How we label issues and PRs:  https://github.com/dotnet/roslyn/wiki/Labels-used-for-issues  

This project has adopted the code of conduct defined by the [Contributor Covenant](http://contributor-covenant.org/) to clarify expected behavior in our community. This code of conduct has been [adopted by many other projects](http://contributor-covenant.org/adopters/). For more information see the [Code of conduct](https://github.com/Microsoft/visualfsharp/wiki/Code-of-Conduct).

If you would like to contribute to the F# ecosystem more generally see the F# Software Foundation's [Community Projects](http://fsharp.org/community/projects/) pages.

### Using CI Builds

To install F#, see http://fsharp.org.

To download the bits for the latest CI builds see [these instructions](https://github.com/Microsoft/visualfsharp/wiki/Using-CI-Builds). This includes and ZIPs containing the F# compiler and VSIX installers for the Visual F# IDE Tools.

### Code Flow to Other Platforms

This repository enables development on Windows, Linux and OSX.  It enables automated CI testing primarily on Windows.

If using Android, or iOS, and would like to contribute, please see the instructions provided at the [Open Edition repo](https://github.com/fsharp/fsharp/#the-open-edition-of-the-f-compiler-core-library--tools).

Although the primary focus of this repo is F# for Windows and the Visual Studio F# tools, contributions here flow directly to the F# Open Edition repo.  More details can be found [here](https://github.com/Microsoft/visualfsharp/wiki/Code-Flow-to-Open-Edition).

###Using F# on a buildserver or computer without VS 2017 or without the optional F# tools

If you wish to use the latest F# compiler on a computer without Visual Studio 2017 installed, you can add the nuget package ``FSharp.Compiler.Tools`` to your projects. This will replace the in-box compiler with the version contained in the package.
The actual package is built in https://github.com/fsharp/fsharp.

Note that while this will remove the dependency on VS 2017, you will still need to have MSBuild and the required targets files installed, which come with any older version of VS (e.g. 2013 or 2015).
#### ... With an older version of VS
Just install the nuget package, it will then use MSBuild and the targets files from the older version. If you get an error, see below.
#### ... With VS (any version) installed, but without the optional F# tools installed
The currently distributed F# templates depend on machine-wide installed .targets files. You can manually modify your project to instead use the .targets file from the nuget package. This will allow you to build your project on a computer with VS but without the optional F# tools installed. See https://github.com/fsharp/fsharp/issues/676 for how to modify your project file.

### Get In Touch

Keep up with the Visual F# Team and the development of the Visual F# Tools by following us [@VisualFSharp](https://twitter.com/VisualFSharp) or subscribing to our [team blog](http://blogs.msdn.com/b/fsharpteam/).

