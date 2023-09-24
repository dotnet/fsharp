namespace FSharp.Test

open System
open System.IO
open System.Reflection

open Xunit.Sdk

open FSharp.Compiler.IO
open FSharp.Test.Compiler
open FSharp.Test.Utilities

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
    let outputDirectory methodName extraDirectory = getTestOutputDirectory dir methodName extraDirectory 
    let mutable baselineSuffix = ""
    let mutable includes = Array.empty<string>
    
    let createCompilationUnit path (filename: string) methodName multipleFiles =
        // if there are multiple files being processed, add extra directory for each test to avoid reference file conflicts
        let extraDirectory =
            if multipleFiles then
                filename.Substring(0, filename.Length - 3) // remove .fs
                |> normalizeName
            else ""
        let outputDirectory = outputDirectory methodName extraDirectory
        let _outputDirectoryPath =
            match outputDirectory with
            | Some path -> path.FullName
            | None -> failwith "Can't set the output directory"
        let sourceFilePath = normalizePathSeparator (path ++ filename)
        
        {   Source            = SourceCodeFileKind.Create(sourceFilePath)
            AdditionalSources = []
            Baseline          = Some (BaseLineHelper.makeLegacyBaseLine(sourceFilePath, baselineSuffix))
            Options           = []
            OutputType        = Library
            Name              = Some filename
            IgnoreWarnings    = false
            References        = []
            OutputDirectory   = outputDirectory
            TargetFramework   = TargetFramework.Current
            StaticLink = false
            } |> FS

    new([<ParamArray>] dirs: string[]) = DirectoryAttribute(Path.Combine(dirs) : string)
    
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
