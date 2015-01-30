
    Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  

    Licensed under the Apache License, Version 2.0.  
    See License.txt in the project root for license information.


Visual F<span>#</span>
======================

All notable changes to this project will be documented in this file.

### [3.1.2] - 20 August 2014

#### Language, compiler, runtime, interactive

* Allow arbitrary-dimensional slicing
* Bugfix [#72](https://visualfsharp.codeplex.com/workitem/72): Indexer properties with more than 4 arguments cannot be accessed
* Bugfix [#113](https://visualfsharp.codeplex.com/workitem/113): `Async.Sleep` in .NETCore profiles does not invoke error continuation
* Ship versions FSharp.Core.dll built on portable profiles 78 and 259
* Bugfix [#91](https://visualfsharp.codeplex.com/workitem/91): String module documentation is false
* Support "shebang" (`#!`) in F# source files
* Bugfix [#78](https://visualfsharp.codeplex.com/workitem/78): Allow space characters in active pattern case identifiers
* Vertical pipes disallowed in active pattern case identifiers
* Bugfix: Invalid code generated when calling VB methods with optional byref args
* Bugfix [#69](https://visualfsharp.codeplex.com/workitem/69): Invalid code generated when calling C# method with optional nullable args
* Bugfix [#9](https://visualfsharp.codeplex.com/workitem/9): XML doc comments on F# record type fields do not appear when accessing in C#
* Bugfix [#59](https://visualfsharp.codeplex.com/workitem/59): Compiler always requires System.Runtime.InteropServices, this is not present in all portable profiles
* Bugfix [#17](https://visualfsharp.codeplex.com/workitem/17): Incorrect generation of XML from doc comments for Record fields
* Enable non-locking shadow copy of reference assemblies in fsi/fsianycpu
* Inline codegen optimization using structs
* Perf improvement for `Seq.windowed`
* Bugfix [#7](https://visualfsharp.codeplex.com/workitem/17): NullRef in list comprehension, when for loop works
* Bugfix [#1](https://visualfsharp.codeplex.com/workitem/1): Type inference involving generic param arrays
* Bugfix [#37](https://visualfsharp.codeplex.com/workitem/37): Perf regression in 3.1.0 related to resolving extension methods
* Bugfix: Can't run F# console application with 'update' in name
* Bugfix: Slicing and range expression inconsistent
* exe.config files for fsc, fsi, fsianycpu now use simple version range instead of long set of explicit version redirects
* Bugfix: Invalid code is generated when using field initializers in struct constructor

#### Visual Studio

* Bugfix [#126](https://visualfsharp.codeplex.com/workitem/126): F# package installer does not honor custom install paths for express SKUs
* Bugfix [#75](https://visualfsharp.codeplex.com/workitem/75): Microsoft.FSharp.targets shim not deployed with F# SDK
* Bugfix: Fix crash in smart indent provider
* Bugfix [#55](https://visualfsharp.codeplex.com/workitem/55): Cannot add reference to F# PCL project
* Project templates for F# portable libraries targeting profiles 78 and 259
* Bugfix: Typos in tutorial project script
* Enable non-locking shadow copy of reference assemblies in fsi/fsianycpu (VS options added)
* Allow breakpoints to be set inside of quotations
* Support "Publish" action in project system for web, Azure
* Bugfix: Required C# event members do not appear in intellisense when signature is (object, byref)


### [3.1.1] - 24 January 2014

#### Language, compiler, runtime, interactive

* Improve F# compiler telemetry
* Bugfix: Improper treatment of * in AssemblyVersion attribute
* Bugfix: ``sprintf "%%"`` returns `"%%"` in F# 3.1.0, previously returned `"%"` in F# 3.0 and earlier
* Bugfix: F# 3.0 1D slice setter does not compile in F# 3.1.0

#### Visual Studio

* Bugfix: Errors when attempting to add reference to .NET core library
* Bugfix: Crash in `FSComp.SR.RunStartupValidation()`
* Enable installation of Visual F# on VS Desktop Express
* Added support for showing xml doc comments for named arguments
* Visual F# package deployable on non-VS machines. Deploys compiler and runtime toolchain plus msbuild targets


[3.1.2]: http://blogs.msdn.com/b/fsharpteam/archive/2014/08/20/announcing-the-release-of-visual-f-tools-3-1-2.aspx
[3.1.1]: http://blogs.msdn.com/b/fsharpteam/archive/2014/01/22/announcing-visual-f-3-1-1-and-support-for-desktop-express.aspx
