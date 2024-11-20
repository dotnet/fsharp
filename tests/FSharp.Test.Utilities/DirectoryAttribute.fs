namespace FSharp.Test

open System
open System.IO
open System.Reflection

open Xunit.Sdk

open FSharp.Compiler.IO
open FSharp.Test.Compiler
open FSharp.Test.Utilities
open TestFramework

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

    let dirInfo = normalizePathSeparator (Path.GetFullPath(dir))
    let mutable baselineSuffix = ""
    let mutable includes = Array.empty<string>
    
    let readFileOrDefault (path: string) : string option =
        match FileSystem.FileExistsShim(path) with
        | true -> Some (File.ReadAllText path)
        | _ -> None

    let createCompilationUnit path (filename: string) =
        let outputDirectoryPath = createTemporaryDirectory().FullName
        let sourceFilePath = normalizePathSeparator (path ++ filename)
        let fsBslFilePath = sourceFilePath + baselineSuffix + ".err.bsl"
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
        let ilOutFilePath = normalizePathSeparator (Path.ChangeExtension(outputDirectoryPath ++ filename, ".il.err"))
        let fsBslSource = readFileOrDefault fsBslFilePath
        let ilBslSource = readFileOrDefault ilBslFilePath

        {   Source            = SourceCodeFileKind.Create(sourceFilePath)
            AdditionalSources = []
            Baseline          =
                Some
                    {
                        SourceFilename = Some sourceFilePath
                        FSBaseline = { FilePath = fsOutFilePath; BslSource = fsBslFilePath; Content = fsBslSource }
                        ILBaseline = { FilePath = ilOutFilePath; BslSource = ilBslFilePath; Content = ilBslSource }
                    }
            Options           = Compiler.defaultOptions
            OutputType        = Library
            Name              = Some filename
            IgnoreWarnings    = false
            References        = []
            OutputDirectory   = Some (DirectoryInfo(outputDirectoryPath))
            TargetFramework   = TargetFramework.Current
            StaticLink        = false
            } |> FS

    new([<ParamArray>] dirs: string[]) = DirectoryAttribute(Path.Combine(dirs) : string)
    
    member _.BaselineSuffix with get() = baselineSuffix and set v = baselineSuffix <- v
    member _.Includes with get() = includes and set v = includes <- v

    override _.GetData _ =
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

        fsFiles
        |> Array.map (fun fs -> createCompilationUnit dirInfo fs)
        |> Seq.map (fun c -> [| c |])
