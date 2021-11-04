(**
---
title: Hosting the compiler
category: Compiler Service
categoryindex: 2
index: 9
---
*)
(*** hide ***)
#I "../../artifacts/bin/FSharp.Compiler.Service/Debug/netstandard2.0"
(**
Hosted Compiler
===============

This tutorial demonstrates how to host the F# compiler.

> **NOTE:** The FSharp.Compiler.Service API is subject to change when later versions of the nuget package are published
*)

(**
> **NOTE:** There are several options for hosting the F# compiler. The easiest one is to use the 
`fsc.exe` process and pass arguments. 
*)

(**

> **NOTE:** By default [compilations using FSharp.Compiler.Service reference FSharp.Core 4.3.0.0](https://github.com/fsharp/FSharp.Compiler.Service/issues/156) (matching F# 3.0).  You can override
this choice by passing a reference to FSharp.Core for 4.3.1.0 or later explicitly in your command-line arguments.

*)

(**
---------------------------

First, we need to reference the libraries that contain F# interactive service:
*)

#r "FSharp.Compiler.Service.dll"
open System.IO
open FSharp.Compiler.CodeAnalysis

// Create an interactive checker instance 
let checker = FSharpChecker.Create()

(**
Now write content to a temporary file:

*)
let fn = Path.GetTempFileName()
let fn2 = Path.ChangeExtension(fn, ".fsx")
let fn3 = Path.ChangeExtension(fn, ".dll")

File.WriteAllText(fn2, """
module M

type C() = 
   member x.P = 1

let x = 3 + 4
""")

(**
Now invoke the compiler:
*)

let errors1, exitCode1 = 
    checker.Compile([| "fsc.exe"; "-o"; fn3; "-a"; fn2 |]) 
    |> Async.RunSynchronously

(** 

If errors occur you can see this in the 'exitCode' and the returned array of errors:

*)
File.WriteAllText(fn2, """
module M

let x = 1.0 + "" // a type error
""")

let errors1b, exitCode1b = 
    checker.Compile([| "fsc.exe"; "-o"; fn3; "-a"; fn2 |])
    |> Async.RunSynchronously

(**

Compiling to a dynamic assembly
===============================

You can also compile to a dynamic assembly, which uses the F# Interactive code generator.
This can be useful if you are, for example, in a situation where writing to the file system
is not really an option.

You still have to pass the "-o" option to name the output file, but the output file is not actually written to disk.

The 'None' option indicates that the initialization code for the assembly is not executed. 
*)
let errors2, exitCode2, dynAssembly2 = 
    checker.CompileToDynamicAssembly([| "-o"; fn3; "-a"; fn2 |], execute=None)
     |> Async.RunSynchronously

(*
Passing 'Some' for the 'execute' parameter executes  the initialization code for the assembly.
*)
let errors3, exitCode3, dynAssembly3 = 
    checker.CompileToDynamicAssembly([| "-o"; fn3; "-a"; fn2 |], Some(stdout,stderr))
     |> Async.RunSynchronously

