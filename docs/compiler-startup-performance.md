(**
---
title: Compiler Startup Performance
category: Compiler
categoryindex: 1
index: 7
---
*)
# Compiler Startup Performance

Compiler startup performance is a key factor affecting happiness of F# users. If the compiler took 10sec to start up, then far fewer people would use F#.

On all platforms, the following factors affect startup performance:

* Time to load compiler binaries. This depends on the size of the generated binaries, whether they are pre-compiled (for example, using NGEN or CrossGen), and the way the .NET implementation loads them.

* Time to open referenced assemblies (for example, `mscorlib.dll`, `FSharp.Core.dll`) and analyze them for the types and namespaces defined. This depends particularly on whether this is correctly done in an on-demand way.

* Time to process "open" declarations are the top of each file. Processing these declarations have been observed to take time in some cases of  F# compilation.

* Factors specific to the specific files being compiled.

On Windows, the compiler delivered with Visual Studio currently uses NGEN to pre-compile `fsc`, `fsi`, and some assemblies used in Visual Studio tooling. For .NET Core, the CrossGen tool is used to accomplish the same thing. Visual Studio will use _delayed_ NGEN, meaning that it does not run NGEN on every binary up front. This means that F# compilation through Visual Studio may be slow for a few times before it gets NGENed.

