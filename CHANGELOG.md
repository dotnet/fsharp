
    Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  

    Licensed under the Apache License, Version 2.0.  
    See License.txt in the project root for license information.


# Visual F<span>#</span>
All notable changes to this project will be documented in this file.

This project **doesn't** use [Semantic Versioning](http://semver.org/)

## [4.0.0] / Unreleased

### Language, compiler, runtime, interactive

- `FEATURE` Constructors as first-class functions
- `FEATURE` Simplified use of mutable values
- `FEATURE` Support for high-dimensional arrays
- `FEATURE` Support for static parameters to provided methods
- `FEATURE` Slicing syntax support for F# lists
- `FEATURE` Simplified usage of units of measure with printf-family functions
- `PERFORMANCE` Faster generic comparison
- `ENHANCEMENT` Better async stack traces
- `PERFORMANCE` Modified GC settings on the compiler for better performance
- `FEATURE` Normalized collections modules

### Visual Studio

- `FEATURE` Assembly metadata in project templates
- `PERFORMANCE` Improved startup time for FSI
- `FEATURE` New hotkeys for FSI

## [3.1.2] / 2014-08-20

### Language, compiler, runtime, interactive

- `FEATURE` Ship versions FSharp.Core.dll built on portable profiles 78 and 259
- `FEATURE` Allow arbitrary-dimensional slicing
- `FEATURE` Support "shebang" ( `#!` ) in F# source files
- `FEATURE` Enable non-locking shadow copy of reference assemblies in fsi/fsianycpu
- `PERFORMANCE` Inline codegen optimization using structs
- `PERFORMANCE` Perf improvement for `Seq.windowed`

- `REMOVED` Vertical pipes disallowed in active pattern case identifiers

- `CHANGED` exe.config files for fsc, fsi, fsianycpu now use simple version range instead of long set of explicit version redirects

- `BUGFIX` Indexer properties with more than 4 arguments cannot be accessed [#72](https://visualfsharp.codeplex.com/workitem/72)
- `BUGFIX` Async.Sleep in .NETCore profiles does not invoke error continuation [#113](https://visualfsharp.codeplex.com/workitem/113)
- `BUGFIX` String module documentation is false [#91](https://visualfsharp.codeplex.com/workitem/91)
- `BUGFIX` Allow space characters in active pattern case identifiers [#78](https://visualfsharp.codeplex.com/workitem/78)
- `BUGFIX` Invalid code generated when calling VB methods with optional byref args
- `BUGFIX` Invalid code generated when calling C# method with optional nullable args [#69](https://visualfsharp.codeplex.com/workitem/69)
- `BUGFIX` XML doc comments on F# record type fields do not appear when accessing in C# [#9](https://visualfsharp.codeplex.com/workitem/9)
- `BUGFIX` Compiler always requires System.Runtime.InteropServices, this is not present in all portable profiles [#59](https://visualfsharp.codeplex.com/workitem/59)
- `BUGFIX` Incorrect generation of XML from doc comments for Record fields [#17](https://visualfsharp.codeplex.com/workitem/17)
- `BUGFIX` NullRef in list comprehension, when for loop works [#7](https://visualfsharp.codeplex.com/workitem/17)
- `BUGFIX` Type inference involving generic param arrays [#1](https://visualfsharp.codeplex.com/workitem/1)
- `BUGFIX` Perf regression in 3.1.0 related to resolving extension methods [#37](https://visualfsharp.codeplex.com/workitem/37)
- `BUGFIX` Can't run F# console application with 'update' in name
- `BUGFIX` Slicing and range expression inconsistent
- `BUGFIX` Invalid code is generated when using field initializers in struct constructor

### Visual Studio

- `FEATURE` Project templates for F# portable libraries targeting profiles 78 and 259
- `FEATURE` Enable non-locking shadow copy of reference assemblies in fsi/fsianycpu (VS options added)
- `FEATURE` Allow breakpoints to be set inside of quotations
- `FEATURE` Support "Publish" action in project system for web, Azure

- `BUGFIX` F# package installer does not honor custom install paths for express SKUs [#126](https://visualfsharp.codeplex.com/workitem/126)
- `BUGFIX` Microsoft.FSharp.targets shim not deployed with F# SDK [#75](https://visualfsharp.codeplex.com/workitem/75)
- `BUGFIX` Fix crash in smart indent provider
- `BUGFIX` Cannot add reference to F# PCL project [#55](https://visualfsharp.codeplex.com/workitem/55)
- `BUGFIX` Typos in tutorial project script
- `BUGFIX` Required C# event members do not appear in intellisense when signature is (object, byref)


## [3.1.1] / 2014-01-24

### Language, compiler, runtime, interactive

- `FEATURE` Improve F# compiler telemetry

- `BUGFIX` Improper treatment of `*` in `AssemblyVersionAttribute` attribute
- `BUGFIX` `sprintf "%%"` returns `"%%"` in F# 3.1.0, previously returned `"%"` in F# 3.0 and earlier
- `BUGFIX` F# 3.0 1D slice setter does not compile in F# 3.1.0

### Visual Studio

- `FEATURE` Enable installation of Visual F# on VS Desktop Express
- `FEATURE` Added support for showing xml doc comments for named arguments
- `FEATURE` Visual F# package deployable on non-VS machines. Deploys compiler and runtime toolchain plus msbuild targets

- `BUGFIX` Errors when attempting to add reference to .NET core library
- `BUGFIX` Crash in `FSComp.SR.RunStartupValidation`


[4.0.0]: https://github.com/Microsoft/visualfsharp/compare/5caef3cf1eac2f295b89bfdb2fbff481f60ffbdb...fsharp4
[3.1.2]: http://blogs.msdn.com/b/fsharpteam/archive/2014/08/20/announcing-the-release-of-visual-f-tools-3-1-2.aspx
[3.1.1]: http://blogs.msdn.com/b/fsharpteam/archive/2014/01/22/announcing-visual-f-3-1-1-and-support-for-desktop-express.aspx