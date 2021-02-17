---
layout: default
title: Notes on FSharp.Core
subtitle: This technical guide discusses the FSharp.Core library.  
---

# Notes and Guidance on FSharp.Core
{:.no_toc}

This technical guide discusses the FSharp.Core library.  Please help improve this guide by editing it and submitting a pull-request.

Much of the guidance below applies to any .NET library respecting binary compatibility.

### FSharp.Core is binary compatible

FSharp.Core is binary compatible across versions of the F# language. For example, FSharp.Core `5.0.0.0` (F# 5.0) is binary compatible with
`4.7.0.0` (F# 4.7), `4.6.0.0` (F# 3.6), `4.4.0.0` (F# 4.0) , `4.4.1.0` (F# 4.1), , `4.4.3.0` (F# 4.1+)  and so on.

Likewise, FSharp.Core is binary compatible from "netstandard" profiles to actual runtime implementations.
For example, FSharp.Core  for `netstandard2.0` is binary compatible with the runtime implementation assembly `4.7.0.0` and so on.

Binary compatibility means that a component built for X can instead bind to Y at runtime.
It doesn't mean that Y behaves 100% the same as X (some bug fixes may have been made, and Y may have more functionality than X).

### Application v. Library v. Script
{:.no_toc}

Each project is either an *application* or a *library*.

* Examples of application are `.exe` project or a `.dll` project that is a test project, addin, website, or an app.

* Libraries are just ordinary `.dll` components (excluding those above which are applications). 

* Scripts are not projects, just `.fsx` files, possibly referring to other files using `#load` and Libraries using `#r`

### Do *not* bundle FSharp.Core with a library 

Do _not_ include a copy of FSharp.Core with your library or package.  If you do, you will create havoc for users of
your library.

The decision about which `FSharp.Core` a library binds to is up to the application hosting of the library.
The library and/or library package can place constraints on this, but it doesn't decide it.

Especially, do _not_ include FSharp.Core in the ``lib`` folder of a NuGet package.

### Always deploy FSharp.Core as part of a compiled application

For applications, FSharp.Core is normally part of the application itself (so-called "xcopy deploy" of FSharp.Core).  

For modern templates, this is the default. For older templates, you may need to use ``<Private>true</Private>`` in your project file. In  Visual Studio this is equivalent to setting the `CopyLocal` property to `true` properties for the `FSharp.Core` reference.

FSharp.Core.dll will normally appear in the `bin` output folder for your application. For example:

```
    Directory of ...\ConsoleApplication3\bin\Debug\net5.0
    
    18/04/2020  13:20             5,632 ConsoleApplication3.exe
    14/10/2020  12:12         1,400,472 FSharp.Core.dll
```

### Always reference FSharp.Core via the NuGet package.

FSharp.Core is now always referenced via [the NuGet package](http://www.nuget.org/packages/FSharp.Core). 

### Make your FSharp.Core references explicit

Templates for F# libraries use an **implicit** FSharp.Core package reference where the .NET SDK chooses one.  Consider
using an **explicit** reference, especially when creating libraries.

To select a particular FSharp.Core use `Update`:

    <PackageReference Update="FSharp.Core" Version="4.7.2" />

In C# projects use:

    <PackageReference Include="FSharp.Core" Version="4.7.2" />

If you make your FSharp.Core dependency explicit, you will have to explicitly upgrade your FSharp.Core reference in order to use
new F# language or library features should those features depend on a particular minimal FSharp.Core version.

### Libraries should target lower versions of FSharp.Core

F# ecosystem libraries should generally target the *earliest, most portable* profile of FSharp.Core feasible, within reason.

If your library is part of an ecosystem, it can be helpful to target the _earliest, most widespread language version_ 
and the _earliest_ and _most supported_ profiles of the .NET Framework feasible.

The version you choose should be based on the minimum F# language version you want to support. The minimum FSharp.Core version for each language version is listed below:

|Minimum F# language version|Minimum FSharp.Core version|
|------------------------------|------------------------------|
|F# 4.1|4.3.4|
|F# 4.5|4.5.2|
|F# 4.6|4.6.2|
|F# 4.7|4.7.2|
|F# 5.0|5.0.0|

A good choice for libraries is to target `netstandard2.0` and FSharp.Core 4.7.2.

    <PackageReference Update="FSharp.Core" Version="4.7.2" />

For "libraries" that are effectively part of an application, you can just target
the latest language version and the framework you're using in your application.

### Applications should target higher versions of FSharp.Core

F# applications should generally use the *highest* language version and the most *platform-specific* version of FSharp.Core.

Generally, when writing an application, you want to use the highest version of FSharp.Core available for the platform
you are targeting.

If your application in being developed by people using multiple versions of F# tooling (common
in open source working) you may need to target a lower version of the language and a correspondingly earlier version
of FSharp.Core.

### The FSharp.Core used by a script depends on the tool processing the script

If you run a script with `dotnet fsi` then the tool will decide which FSharp.Core is used, and which implementation assemblies are used.

When editing a script, the editing tools will decide which FSharp.Core is referenced. 

### FSharp.Core and static linking
{:.no_toc}

The ILMerge tool and the F# compiler both allow static linking of assemblies including static linking of FSharp.Core.
This can be useful to build a single standalone file for a tool.

However, these options must be used with caution. 

* Only use this option for applications, not libraries. If it's not a .EXE (or a library that is effectively an application) then don't even try using this option.

Searching on stackoverflow reveals further guidance on this topic.

### FSharp.Core in components using FSharp.Compiler.Service
{:.no_toc}

If your application of component uses FSharp.Compiler.Service, 
see [this guide](http://fsharp.github.io/FSharp.Compiler.Service/corelib.html). This scenario is more complicated
because FSharp.Core is used both to run your script or application, and is referenced during compilation.

Likewise, if you have a script or library using FSharp.Formatting, then beware that is using FSharp.Compiler.Service.
For scripts that is normally OK because they are processed using F# Interactive, and the default FSharp.Core is used.
If you have an application using FSharp.Formatting as a component then see the guide linked above.

### FSharp.Core and new language features
{:.no_toc}

New versions of FSharp.Core must generally be consumable by previous generations of F# compiler tooling. There is nothing stopping
older tooling from adding a reference to the new nuget package.

This sometimes limits the new language features that can be used in FSharp.Core or requires careful coding in the serializing/deserializing of
F# metadata stored in the FSharp.Core.dll binary as resources.

## Reference: FSharp.Core version and NuGet package numbers

See [the F# version information RFC](https://github.com/fsharp/fslang-design/blob/master/tooling/FST-1004-versioning-plan.md).


