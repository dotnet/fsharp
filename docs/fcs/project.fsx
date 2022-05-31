(**
---
title: Tutorial: Project analysis
category: FSharp.Compiler.Service
categoryindex: 300
index: 600
---
*)
(*** hide ***)
#I "../../artifacts/bin/FSharp.Compiler.Service/Debug/netstandard2.0"
(**
Compiler Services: Project Analysis
==================================

This tutorial demonstrates how you can analyze a whole project using services provided by the F# compiler.

> **NOTE:** The FSharp.Compiler.Service API is subject to change when later versions of the nuget package are published.

*)


(**

Getting whole-project results
-----------------------------

As in the [previous tutorial (using untyped AST)](untypedtree.html), we start by referencing
`FSharp.Compiler.Service.dll`, opening the relevant namespace and creating an instance
of `InteractiveChecker`:

*)
// Reference F# compiler API
#r "FSharp.Compiler.Service.dll"

open System
open System.Collections.Generic
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text

// Create an interactive checker instance
let checker = FSharpChecker.Create()

(**
Here are our sample inputs:
*)

module Inputs =
    open System.IO

    let base1 = Path.GetTempFileName()
    let fileName1 = Path.ChangeExtension(base1, ".fs")
    let base2 = Path.GetTempFileName()
    let fileName2 = Path.ChangeExtension(base2, ".fs")
    let dllName = Path.ChangeExtension(base2, ".dll")
    let projFileName = Path.ChangeExtension(base2, ".fsproj")

    let fileSource1 =
        """
module M

type C() =
    member x.P = 1

let xxx = 3 + 4
let fff () = xxx + xxx
    """

    File.WriteAllText(fileName1, fileSource1)

    let fileSource2 =
        """
module N

open M

type D1() =
    member x.SomeProperty = M.xxx

type D2() =
    member x.SomeProperty = M.fff() + D1().P

// Generate a warning
let y2 = match 1 with 1 -> M.xxx
    """

    File.WriteAllText(fileName2, fileSource2)


(**
We use `GetProjectOptionsFromCommandLineArgs` to treat two files as a project:
*)

let projectOptions =
    let sysLib nm =
        if System.Environment.OSVersion.Platform = System.PlatformID.Win32NT then
            // file references only valid on Windows
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86)
            + @"\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\"
            + nm
            + ".dll"
        else
            let sysDir =
                System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()

            let (++) a b = System.IO.Path.Combine(a, b)
            sysDir ++ nm + ".dll"

    let fsCore4300 () =
        if System.Environment.OSVersion.Platform = System.PlatformID.Win32NT then
            // file references only valid on Windows
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86)
            + @"\Reference Assemblies\Microsoft\FSharp\.NETFramework\v4.0\4.3.0.0\FSharp.Core.dll"
        else
            sysLib "FSharp.Core"

    checker.GetProjectOptionsFromCommandLineArgs(
        Inputs.projFileName,
        [| yield "--simpleresolution"
           yield "--noframework"
           yield "--debug:full"
           yield "--define:DEBUG"
           yield "--optimize-"
           yield "--out:" + Inputs.dllName
           yield "--doc:test.xml"
           yield "--warn:3"
           yield "--fullpaths"
           yield "--flaterrors"
           yield "--target:library"
           yield Inputs.fileName1
           yield Inputs.fileName2
           let references =
               [ sysLib "mscorlib"
                 sysLib "System"
                 sysLib "System.Core"
                 fsCore4300 () ]

           for r in references do
               yield "-r:" + r |]
    )

(**
Now check the entire project (using the files saved on disk):
*)

let wholeProjectResults =
    checker.ParseAndCheckProject(projectOptions)
    |> Async.RunSynchronously

(**
Now look at the errors and warnings:
*)
wholeProjectResults.Diagnostics.Length // 1

wholeProjectResults.Diagnostics.[0]
    .Message.Contains("Incomplete pattern matches on this expression") // yes it does

wholeProjectResults.Diagnostics.[0].StartLine
wholeProjectResults.Diagnostics.[0].EndLine
wholeProjectResults.Diagnostics.[0].StartColumn
wholeProjectResults.Diagnostics.[0].EndColumn

(**
Now look at the inferred signature for the project:
*)
[ for x in wholeProjectResults.AssemblySignature.Entities -> x.DisplayName ] // ["N"; "M"]

[ for x in
      wholeProjectResults.AssemblySignature.Entities.[0]
          .NestedEntities -> x.DisplayName ] // ["D1"; "D2"]

[ for x in
      wholeProjectResults.AssemblySignature.Entities.[1]
          .NestedEntities -> x.DisplayName ] // ["C"]

[ for x in
      wholeProjectResults.AssemblySignature.Entities.[0]
          .MembersFunctionsAndValues -> x.DisplayName ] // ["y"; "y2"]

(**
You can also get all symbols in the project:
*)
let rec allSymbolsInEntities (entities: IList<FSharpEntity>) =
    [ for e in entities do
          yield (e :> FSharpSymbol)

          for x in e.MembersFunctionsAndValues do
              yield (x :> FSharpSymbol)

          for x in e.UnionCases do
              yield (x :> FSharpSymbol)

          for x in e.FSharpFields do
              yield (x :> FSharpSymbol)

          yield! allSymbolsInEntities e.NestedEntities ]

let allSymbols =
    allSymbolsInEntities wholeProjectResults.AssemblySignature.Entities
(**
After checking the whole project, you can access the background results for individual files
in the project. This will be fast and will not involve any additional checking.
*)

let backgroundParseResults1, backgroundTypedParse1 =
    checker.GetBackgroundCheckResultsForFileInProject(Inputs.fileName1, projectOptions)
    |> Async.RunSynchronously


(**
You can now resolve symbols in each file:
*)

let xSymbolUseOpt =
    backgroundTypedParse1.GetSymbolUseAtLocation(9, 9, "", [ "xxx" ])

let xSymbolUse = xSymbolUseOpt.Value

let xSymbol = xSymbolUse.Symbol

(**
You can find out more about a symbol by doing type checks on various symbol kinds:
*)

let xSymbolAsValue =
    match xSymbol with
    | :? FSharpMemberOrFunctionOrValue as xSymbolAsVal -> xSymbolAsVal
    | _ -> failwith "we expected this to be a member, function or value"


(**
For each symbol, you can look up the references to that symbol:
*)
let usesOfXSymbol =
    wholeProjectResults.GetUsesOfSymbol(xSymbol)

(**
You can iterate all the defined symbols in the inferred signature and find where they are used:
*)
let allUsesOfAllSignatureSymbols =
    [ for s in allSymbols do
          let uses = wholeProjectResults.GetUsesOfSymbol(s)
          yield s.ToString(), uses ]

(**
You can also look at all the symbols uses in the whole project (including uses of symbols with local scope)
*)
let allUsesOfAllSymbols =
    wholeProjectResults.GetAllUsesOfAllSymbols()

(**
You can also request checks of updated versions of files within the project (note that the other files
in the project are still read from disk, unless you are using the [FileSystem API](filesystem.html)):

*)

let parseResults1, checkAnswer1 =
    checker.ParseAndCheckFileInProject(Inputs.fileName1, 0, SourceText.ofString Inputs.fileSource1, projectOptions)
    |> Async.RunSynchronously

let checkResults1 =
    match checkAnswer1 with
    | FSharpCheckFileAnswer.Succeeded x -> x
    | _ -> failwith "unexpected aborted"

let parseResults2, checkAnswer2 =
    checker.ParseAndCheckFileInProject(Inputs.fileName2, 0, SourceText.ofString Inputs.fileSource2, projectOptions)
    |> Async.RunSynchronously

let checkResults2 =
    match checkAnswer2 with
    | FSharpCheckFileAnswer.Succeeded x -> x
    | _ -> failwith "unexpected aborted"

(**
Again, you can resolve symbols and ask for references:
*)

let xSymbolUse2Opt =
    checkResults1.GetSymbolUseAtLocation(9, 9, "", [ "xxx" ])

let xSymbolUse2 = xSymbolUse2Opt.Value

let xSymbol2 = xSymbolUse2.Symbol

let usesOfXSymbol2 =
    wholeProjectResults.GetUsesOfSymbol(xSymbol2)

(**
Or ask for all the symbols uses in the file (including uses of symbols with local scope)
*)
let allUsesOfAllSymbolsInFile1 =
    checkResults1.GetAllUsesOfAllSymbolsInFile()

(**
Or ask for all the uses of one symbol in one file:
*)
let allUsesOfXSymbolInFile1 =
    checkResults1.GetUsesOfSymbolInFile(xSymbol2)

let allUsesOfXSymbolInFile2 =
    checkResults2.GetUsesOfSymbolInFile(xSymbol2)

(**

Analyzing multiple projects
-----------------------------

If you have multiple F# projects to analyze which include references from some projects to others,
then the simplest way to do this is to build the projects and specify the cross-project references using
a `-r:path-to-output-of-project.dll` argument in the ProjectOptions. However, this requires the build
of each project to succeed, producing the DLL file on disk which can be referred to.

In some situations, e.g. in an IDE, you may wish to allow references to other F# projects prior to successful compilation to
a DLL. To do this, fill in the ProjectReferences entry in ProjectOptions, which recursively specifies the project
options for dependent projects. Each project reference still needs a corresponding `-r:path-to-output-of-project.dll`
command line argument in ProjectOptions, along with an entry in ProjectReferences.
The first element of each tuple in the ProjectReferences entry should be the DLL name, i.e. `path-to-output-of-project.dll`.
This should be the same as the text used in the `-r` project reference.

When a project reference is used, the analysis will make use of the results of incremental
analysis of the referenced F# project from source files, without requiring the compilation of these files to DLLs.

To efficiently analyze a set of F# projects which include cross-references, you should populate the ProjectReferences
correctly and then analyze each project in turn.

*)

(**

> **NOTE:** Project references are disabled if the assembly being referred to contains type provider components -
  specifying the project reference will have no effect beyond forcing the analysis of the project, and the DLL will
  still be required on disk.

*)

(**
Summary
-------

As you have seen, the `ParseAndCheckProject` lets you access results of project-wide analysis
such as symbol references. To learn more about working with symbols, see [Symbols](symbols.html).

Using the FSharpChecker component in multi-project, incremental and interactive editing situations may involve
knowledge of the [FSharpChecker operations queue](queue.html) and the [FSharpChecker caches](caches.html).

*)
