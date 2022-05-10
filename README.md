# The F# compiler, F# core library, and F# editor tools

You're invited to contribute to future releases of the F# compiler, core library, and tools. Development of this repository can be done on any OS supported by [.NET](https://dotnet.microsoft.com/).

You will also need the latest .NET 6 SDK installed from [here](https://dotnet.microsoft.com/download/dotnet/6.0).

## Contributing

### Quickstart on Windows

Build from the command line:

```shell
build.cmd
```

The build depends on an installation of Visual Studio. To build the compiler without this dependency use:

```shell
build.cmd -noVisualStudio
```

After it's finished, open either `FSharp.sln` or `VisualFSharp.sln` in your editor of choice. The latter solution is larger but includes the F# tools for Visual Studio and its associated infrastructure.

### Quickstart on Linux or macOS

Build from the command line:

```shell
./build.sh
```

After it's finished, open `FSharp.sln` in your editor of choice.

### Documentation for contributors

* The [Compiler Documentation](docs/index.md) is essential reading for any larger contributions to the F# compiler codebase and contains links to learning videos, architecture diagrams and other resources.

* The same docs are also published as the [The F# Compiler Guide](https://fsharp.github.io/fsharp-compiler-docs/). It also contains the public searchable docs for FSharp.Compiler.Service component.

* See [DEVGUIDE.md](DEVGUIDE.md) for more details on configurations for building the codebase. In practice, you only really need to run `build.cmd`/`build.sh`.

* See [TESTGUIDE.md](TESTGUIDE.md) for information about the various test suites in this codebase and how to run them individually.

### Documentation for F# community

* [The F# Documentation](https://docs.microsoft.com/dotnet/fsharp/) is the primary documentation for F#. The source for the content is [here](https://github.com/dotnet/docs/tree/main/docs/fsharp).

* [The F# Language Design Process](https://github.com/fsharp/fslang-design/) is the fundamental design process for the language, from [suggestions](https://github.com/fsharp/fslang-suggestions) to completed RFCs.  There are also [tooling RFCs](https://github.com/fsharp/fslang-design/tree/main/tooling) for some topics where cross-community co-operation and visibility is most useful.

* [The F# Language Specification](https://fsharp.org/specs/language-spec/) is an in-depth description of the F# language. This is essential for understanding some behaviors of the F# compiler and some of the rules within the compiler codebase. For example, the order and way name resolution happens is specified here, which greatly impacts how the code in Name Resolutions works and why certain decisions are made.

### No contribution is too small

Even if you find a single-character typo, we're happy to take the change! Although the codebase can feel daunting for beginners, we and other contributors are happy to help you along.

## Build Status

| Branch | Status |
|:------:|:------:|
|main|[![Build Status](https://dev.azure.com/dnceng/public/_apis/build/status/dotnet/fsharp/fsharp-ci?branchName=main)](https://dev.azure.com/dnceng/public/_build/latest?definitionId=496&branchName=main)|

## Per-build NuGet packages

Per-build [versions](https://dev.azure.com/dnceng/public/_packaging?_a=package&feed=dotnet-tools&view=versions&package=FSharp.Compiler.Service&protocolType=NuGet) of our NuGet packages are available via this URL: `https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-tools/nuget/v3/index.json`

## Branches

These are the branches in use:

* `main`
  * Almost all contributions go here.
  * Able to be built, installed and used in the latest public Visual Studio release.
  * May contain updated F# features and logic.
  * Used to build nightly VSIX (see above).

* `release/dev15.9`
  * Long-term servicing branch for VS 2017 update 15.9.x. We do not expect to service that release, but if we do, that's where the changes will go.

* `release/dev17.x`
  * Latest release branch for the particular point release of Visual Studio.
  * Incorporates features and fixes from main up to a particular branch point, then selective cherry-picks.
  * May contain new features that depend on new things or fixes in the corresponding forthcoming Visual Studio release.
  * Gets integrated back into main once the corresponding Visual Studio release is made.

## F# language and core library evolution

Evolution of the F# language and core library follows a process spanning two additional repositories. The process is as follows:

1. Use the [F# language suggestions repo](https://github.com/fsharp/fslang-suggestions/) to search for ideas, vote on ones you like, submit new ideas, and discuss details with the F# community.
2. Ideas that are "approved in principle" are eligible for a new RFC in the [F# language design repo](https://github.com/fsharp/fslang-design). This is where the technical specification and discussion of approved suggestions go.
3. Implementations and testing of an RFC are submitted to this repository.

## License

This project is subject to the MIT License. A copy of this license is in [License.txt](License.txt).

## Code of Conduct

This project has adopted the [Contributor Covenant](https://contributor-covenant.org/) code of conduct to clarify expected behavior in our community. You can read it at [CODE_OF_CONDUCT](CODE_OF_CONDUCT.md).

## Get In Touch

Members of the [F# Software Foundation](https://fsharp.org) are invited to the [FSSF Slack](https://fsharp.org/guides/slack/). You can find support from other contributors in the `#compiler` and `#editor-support` channels.

Additionally, you can use the `#fsharp` tag on Twitter if you have general F# questions, including about this repository. Chances are you'll get multiple responses.

## About F\#

If you're curious about F# itself, check out these links:

* [What is F#](https://docs.microsoft.com/dotnet/fsharp/what-is-fsharp)
* [Get started with F#](https://docs.microsoft.com/dotnet/fsharp/get-started/)
* [F# Software Foundation](https://fsharp.org)
* [F# Testimonials](https://fsharp.org/testimonials)
