module FSharp.Compiler.Service.Tests.GeneratedCodeSymbolsTests

open Xunit
open System
open System.Diagnostics
open System.IO
open System.Threading
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.IO
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Symbols
open TestFramework


[<AutoOpen>]
module internal Utils =
    let getTempPath dir =
        Path.Combine(Path.GetTempPath(), dir)

    /// Returns the file name part of a temp file name created with tryCreateTemporaryFileName ()
    /// and an added process id and thread id to ensure uniqueness between threads.
    let getTempFileName() =
        let tempFileName = tryCreateTemporaryFileName ()
        try
            let tempFile, tempExt = Path.GetFileNameWithoutExtension tempFileName, Path.GetExtension tempFileName
            let procId, threadId = Process.GetCurrentProcess().Id, Thread.CurrentThread.ManagedThreadId
            String.concat "" [tempFile; "_"; string procId; "_"; string threadId; tempExt]  // ext includes dot
        finally
            try
                FileSystem.FileDeleteShim tempFileName
            with _ -> ()

    /// Given just a file name, returns it with changed extension located in %TEMP%\ExprTests
    let getTempFilePathChangeExt dir tmp ext =
        Path.Combine(getTempPath dir, Path.ChangeExtension(tmp, ext))

    /// If it doesn't exists, create a folder 'ExprTests' in local user's %TEMP% folder
    let createTempDir dirName =
        let tempPath = getTempPath dirName
        do
            if Directory.Exists tempPath then ()
            else Directory.CreateDirectory tempPath |> ignore

    /// Clean up after a test is run. If you need to inspect the create *.fs files, change this function to do nothing, or just break here.
    let cleanupTempFiles dirName files =
        { new IDisposable with
            member _.Dispose() =
                for fileName in files do
                    try
                        // cleanup: only the source file is written to the temp dir.
                        FileSystem.FileDeleteShim fileName
                    with _ -> ()

                try
                    // remove the dir when empty
                    let tempPath = getTempPath dirName
                    if Directory.GetFiles tempPath |> Array.isEmpty then
                        Directory.Delete tempPath
                with _ -> () }

    let createOptionsAux fileSources extraArgs =
        let dirName = "GeneratedCodeSymbolsTests"
        let fileNames = fileSources |> List.map (fun _ -> getTempFileName())
        let temp2 = getTempFileName()
        let fileNames = fileNames |> List.map (fun temp1 -> getTempFilePathChangeExt dirName temp1 ".fs")
        let dllName = getTempFilePathChangeExt dirName temp2 ".dll"
        let projFileName = getTempFilePathChangeExt dirName temp2 ".fsproj"

        createTempDir dirName
        for fileSource: string, fileName in List.zip fileSources fileNames do
             FileSystem.OpenFileForWriteShim(fileName).Write(fileSource)
        let args = [| yield! extraArgs; yield! mkProjectCommandLineArgs (dllName, []) |]
        let options = { checker.GetProjectOptionsFromCommandLineArgs (projFileName, args) with SourceFiles = fileNames |> List.toArray }

        cleanupTempFiles dirName (fileNames @ [dllName; projFileName]), options

[<Fact>]
let ``IsUnionCaseTester in generated file`` () =
    let source = """
module Lib

type T () =
    member x.IsM = 1
"""
    let cleanup, options = Utils.createOptionsAux [ source ] [ "--langversion:preview" ]
    use _holder = cleanup
    let exprChecker = FSharpChecker.Create(keepAssemblyContents=true, useTransparentCompiler=false)
    let wholeProjectResults = exprChecker.ParseAndCheckProject(options) |> Async.RunImmediate

    let mfvs =
        seq {
            for implFile in wholeProjectResults.AssemblyContents.ImplementationFiles do
                for decl in implFile.Declarations do
                    match decl with
                    | FSharpImplementationFileDeclaration.Entity(e,ds) ->
                        for d in ds do
                            match d with
                            | FSharpImplementationFileDeclaration.MemberOrFunctionOrValue (mfv, args, body) ->
                                yield mfv
                            | _ -> ()
                    | _ -> ()
        }

    Assert.Contains(mfvs, fun x -> x.LogicalName = "get_IsM")
    let mfv = mfvs |> Seq.filter (fun x -> x.LogicalName = "get_IsM") |> Seq.exactlyOne
    Assert.False(mfv.IsUnionCaseTester, $"get_IsM has IsUnionCaseTester = {mfv.IsUnionCaseTester}")

[<Fact>]
let ``IsUnionCaseTester in generated file 2`` () =
    let source = """
module Lib

type T = A | B
"""
    let cleanup, options = Utils.createOptionsAux [ source ] [ "--langversion:preview" ]
    use _holder = cleanup
    let exprChecker = FSharpChecker.Create(keepAssemblyContents=true, useTransparentCompiler=false)
    let wholeProjectResults = exprChecker.ParseAndCheckProject(options) |> Async.RunImmediate

    let mfvs =
        seq {
            for implFile in wholeProjectResults.AssemblyContents.ImplementationFiles do
                for decl in implFile.Declarations do
                    match decl with
                    | FSharpImplementationFileDeclaration.Entity(e,ds) ->
                        for d in ds do
                            match d with
                            | FSharpImplementationFileDeclaration.MemberOrFunctionOrValue (mfv, args, body) ->
                                yield mfv
                            | _ -> ()
                    | _ -> ()
        }

    Assert.Contains(mfvs, fun x -> x.LogicalName = "get_IsA")
    let mfv = mfvs |> Seq.filter (fun x -> x.LogicalName = "get_IsA") |> Seq.exactlyOne
    Assert.True(mfv.IsUnionCaseTester, $"get_IsA has IsUnionCaseTester = {mfv.IsUnionCaseTester}")