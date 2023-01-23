﻿namespace FSharp.Test

open System
open System.IO
open System.Reflection

open Xunit.Sdk

open FSharp.Compiler.IO
open FSharp.Test.Compiler

/// Attribute to use with Xunit's TheoryAttribute.
/// Takes a directory, relative to current test suite's root.
/// Returns a CompilationUnit with encapsulated source code, error baseline and IL baseline (if any).
[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)>]
[<NoComparison; NoEquality>]
type DirectoryAttribute(dir: string) =
    inherit DataAttribute()
    do
        if String.IsNullOrWhiteSpace(dir) then
            invalidArg "dir" "Directory cannot be null, empty or whitespace only."

    let normalizePathSeparator (text:string) = text.Replace(@"\", "/")

    let normalizeName name =
        let invalidPathChars = Array.concat [Path.GetInvalidPathChars(); [| ':'; '\\'; '/'; ' '; '.' |]]
        let result = invalidPathChars |> Array.fold(fun (acc:string) (c:char) -> acc.Replace(string(c), "_")) name
        result

    let dirInfo = normalizePathSeparator (Path.GetFullPath(dir))
    let outputDirectory methodName extraDirectory =
        // If the executing assembly has 'artifacts\bin' in it's path then we are operating normally in the CI or dev tests
        // Thus the output directory will be in a subdirectory below where we are executing.
        // The subdirectory will be relative to the source directory containing the test source file,
        // E.g
        //    When the source code is in:
        //        $(repo-root)\tests\FSharp.Compiler.ComponentTests\Conformance\PseudoCustomAttributes
        //    and the test is running in the FSharp.Compiler.ComponentTeststest library
        //    The output directory will be:
        //        artifacts\bin\FSharp.Compiler.ComponentTests\$(Flavour)\$(TargetFramework)\tests\FSharp.Compiler.ComponentTests\Conformance\PseudoCustomAttributes
        //
        //    If we can't find anything then we execute in the directory containing the source
        //
        try
            let testlibraryLocation = normalizePathSeparator (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
            let pos = testlibraryLocation.IndexOf("artifacts/bin",StringComparison.OrdinalIgnoreCase)
            if pos > 0 then
                // Running under CI or dev build
                let testRoot = Path.Combine(testlibraryLocation.Substring(0, pos), @"tests/")
                let testSourceDirectory =
                    let testPaths = dirInfo.Replace(testRoot, "").Split('/')
                    testPaths[0] <- "tests"
                    Path.Combine(testPaths)
                let n = Path.Combine(testlibraryLocation, testSourceDirectory.Trim('/'), normalizeName methodName, extraDirectory)
                let outputDirectory = new DirectoryInfo(n)
                Some outputDirectory
            else
                raise (new InvalidOperationException($"Failed to find the test output directory:\nTest Library Location: '{testlibraryLocation}'\n Pos: {pos}"))
                None

        with | e ->
            raise (new InvalidOperationException($" '{e.Message}'.  Can't get the location of the executing assembly"))

    let mutable baselineSuffix = ""
    let mutable includes = Array.empty<string>

    let readFileOrDefault (path: string) : string option =
        match FileSystem.FileExistsShim(path) with
        | true -> Some (File.ReadAllText path)
        | _ -> None

    let createCompilationUnit path (filename: string) methodName multipleFiles =
        // if there are multiple files being processed, add extra directory for each test to avoid reference file conflicts
        let extraDirectory =
            if multipleFiles then
                filename.Substring(0, filename.Length - 3) // remove .fs
                |> normalizeName
            else ""
        let outputDirectory = outputDirectory methodName extraDirectory
        let outputDirectoryPath =
            match outputDirectory with
            | Some path -> path.FullName
            | None -> failwith "Can't set the output directory"
        let sourceFilePath = normalizePathSeparator (path ++ filename)
        let fsBslFilePath = sourceFilePath + ".err.bsl"
        let ilBslFilePath =
            let ilBslPaths = [|
#if DEBUG
    #if NETCOREAPP
                yield sourceFilePath + baselineSuffix + ".il.netcore.debug.bsl"
                yield sourceFilePath + baselineSuffix + ".il.netcore.bsl"
    #else
                yield sourceFilePath + baselineSuffix + ".il.net472.debug.bsl"
                yield sourceFilePath + baselineSuffix + ".il.net472.bsl"
    #endif
                yield sourceFilePath + baselineSuffix + ".il.debug.bsl"
                yield sourceFilePath + baselineSuffix + ".il.bsl"
#else
    #if NETCOREAPP
                yield sourceFilePath + baselineSuffix + ".il.netcore.release.bsl"
                yield sourceFilePath + baselineSuffix + ".il.netcore.bsl"
    #else
                yield sourceFilePath + baselineSuffix + ".il.net472.release.bsl"
                yield sourceFilePath + baselineSuffix + ".il.net472.bsl"
    #endif
                yield sourceFilePath + baselineSuffix + ".il.release.bsl"
                yield sourceFilePath + baselineSuffix + ".il.bsl"
#endif
            |]

            let findBaseline =
                ilBslPaths
                |> Array.tryPick(fun p -> if File.Exists(p) then Some p else None)
            match findBaseline with
            | Some s -> s
            | None -> sourceFilePath + baselineSuffix + ".il.bsl"

        let fsOutFilePath = normalizePathSeparator (Path.ChangeExtension(outputDirectoryPath ++ filename, ".err"))
        let ilOutFilePath = normalizePathSeparator ( Path.ChangeExtension(outputDirectoryPath ++ filename, ".il"))
        let fsBslSource = readFileOrDefault fsBslFilePath
        let ilBslSource = readFileOrDefault ilBslFilePath

        {
            Source              = SourceCodeFileKind.Create(sourceFilePath)
            AdditionalSources   = []
            Baseline            =
                Some
                    {
                        SourceFilename = Some sourceFilePath
                        FSBaseline = { FilePath = fsOutFilePath; BslSource=fsBslFilePath; Content = fsBslSource }
                        ILBaseline = { FilePath = ilOutFilePath; BslSource=ilBslFilePath ; Content = ilBslSource  }
                    }
            Options             = []
            OutputType          = Library
            Name                = Some filename
            IgnoreWarnings      = false
            References          = []
            OutputDirectory     = outputDirectory } |> FS

    member _.BaselineSuffix with get() = baselineSuffix and set v = baselineSuffix <- v
    member _.Includes with get() = includes and set v = includes <- v

    override _.GetData(method: MethodInfo) =
        if not (Directory.Exists(dirInfo)) then
            failwith (sprintf "Directory does not exist: \"%s\"." dirInfo)

        let allFiles : string[] = Directory.GetFiles(dirInfo, "*.fs")

        let filteredFiles =
            match includes |> Array.map (fun f -> normalizePathSeparator (dirInfo ++ f)) with
                | [||] -> allFiles
                | incl -> incl

        let fsFiles = filteredFiles |> Array.map Path.GetFileName

        if fsFiles |> Array.length < 1 then
            failwith (sprintf "No required files found in \"%s\".\nAll files: %A.\nIncludes:%A." dirInfo allFiles includes)

        for f in filteredFiles do
            if not <| FileSystem.FileExistsShim(f) then
                failwithf "Requested file \"%s\" not found.\nAll files: %A.\nIncludes:%A." f allFiles includes

        let multipleFiles = fsFiles |> Array.length > 1

        fsFiles
        |> Array.map (fun fs -> createCompilationUnit dirInfo fs method.Name multipleFiles)
        |> Seq.map (fun c -> [| c |])
