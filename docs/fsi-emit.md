---
title: F# Interactive Emit
category: Compiler Internals
categoryindex: 200
index: 375
---
# F# Interactive Code Generation

F# Interactive (`dotnet fsi`) accepts incremental code fragments. This capability is also used by [hosted execution capability of the FSharp.Compiler.Service API](fcs/interactive.fsx) which is used to build the F# kernel for .NET Interactive notebooks.

Historically F# Interactive code was  emitted into a single dynamic assembly using Reflection.Emit and ilreflect.fs (meaning one assembly that is continually growing). However, .NET Core Reflection.Emit does not support the emit of debug symbols for dynamic assemblies, so in Feb 2022 we switched to emitting multiple non-dynamic assemblies (meaning assemblies dynamically created in-memory using ilwrite.fs, and loaded, but not growing).

The assemblies are named:

`FSI-ASSEMBLY1`
`FSI-ASSEMBLY2`

etc.

## Compat switch

There is a switch `fsi --multiemit` that turns on the use of multi-assembly generation (when it is off, we use Reflection Emit for  single-dynamic-assembly generation).  This is on by default for .NET Core, and off by default for .NET Framework for compat reasons.

## Are multiple assemblies too costly?

There is general assumption in this that on modern dev machines (where users execute multiple interactions) then generating 50, 100 or 1,000 or 10,000 dynamic assemblies by repeated manual execution of code is not a problem: the extra overheads of multiple assemblies compared to one dynamic assembly is of no real significance in developer REPL scenarios given the vast amount of memory available on modern 64-bit machines.

Quick check: adding 10,000 `let x = 1;;` interactions to .NET Core `dotnet fsi` adds about 300MB to the FSI.EXE process, meaning 30K/interaction. A budget of 1GB for interactive fragments (reasonable on a 64-bit machine), and an expected maximum of 10000 fragments before restart (that's a lot!), then each fragment can take up to 100K. This is well below the cost of a new assembly.

Additionally, these costs are not substantially reduced if `--multiemit` is disabled, so they've always been the approximate costs of F# Interactive fragment generation.

## Internals and accessibility across fragments

Generating into multiple assemblies raises issues for some things that are assembly bound such as "internals" accessibility. In a first iteration of this we had a failing case here:

```fsharp
> artifacts\bin\fsi\Debug\net50\fsi.exe --optimize-
...
// Fragment 1
> let internal f() = 1;;
val internal f: unit -> int

// Fragment 2 - according to existing rules it is allowed to access internal things of the first
f();; 
System.MethodAccessException: Attempt by method '<StartupCode$FSI_0003>.$FSI_0003.main@()' to access method 'FSI_0002.f()' failed.
   at <StartupCode$FSI_0003>.$FSI_0003.main@()
```

This is because we are now generating into multiple assemblies. Another bug was this:

```fsharp
> artifacts\bin\fsi\Debug\net50\fsi.exe --optimize+
...
// Fragment 1 - not `x` becomes an internal field of the class
> type C() =
>    let mutable x = 1
>    member _.M() = x
> ;;
...
// Fragment 2 - inlining 'M()' gave an access to the internal field `x`
> C().M();;
...<bang>...
```

According to the current F# scripting programming model (the one checked in the editor), the "internal" thing should be accessible in subsequent fragments. Should this be changed? No:

* It's very hard to adjust the implementation of the editor scripting model to consider fragments delimited by `;;` to be different assemblies, whether in the editor or in F# Interactive. 
* And would we even want to?  It's common enough for people to debug code scattered with "internal" declarations.  
* In scripts, the `;;` aren't actually accurate markers for what will or won't be sent to F# Interactive, which get added implicitly.

  For example, consider the script

  ```fsharp
  let internal f() = 1;;
  f();; 
  ```

  In the editor should this be given an error or not?  That is, should the `;;` be seen as accurate indicators of separate script fragments? (Answer: yes if we know the script will be piped-to-input, no if the script is used as a single file entry - when the `;;` are ignored)

* Further, this would be a breaking change, e.g. it could arise in an automated compat situation if people are piping into standard input and the input contains `;;` markers.

Because of this we emit IVTs for the next 30 `FSI-ASSEMBLYnnn` assemblies on each assembly fragment, giving a warning when an internal thing is accessed across assembly boundaries within that 30 (reporting it as a deprecated feature), and give an error if internal access happens after that.

From a compat perspective this seems reasonable, and the compat flag is available to return the whole system to generate-one-assembly behavior.
