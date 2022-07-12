namespace FSharp.Test

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

    let normalizeName name =
        let invalidPathChars = Array.concat [Path.GetInvalidPathChars(); [| ':'; '\\'; '/'; ' '; '.' |]]
        let result = invalidPathChars |> Array.fold(fun (acc:string) (c:char) -> acc.Replace(string(c), "_")) name
        result

    let dirInfo = Path.GetFullPath(dir)
    let outputDirectory name =
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
            let testlibraryLocation=Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            let pos = testlibraryLocation.IndexOf(@"artifacts\bin",StringComparison.OrdinalIgnoreCase)
            if pos > 0 then
                // Running under CI or dev build
                let testRoot = Path.Combine(testlibraryLocation.Substring(0, pos), @"tests\")
                let testSourceDirectory =
                    let testPaths = dirInfo.Replace(testRoot, "").Split('\\')
                    testPaths[0] <- "tests"
                    Path.Combine(testPaths)
                let n = Path.Combine(testlibraryLocation, testSourceDirectory.Trim('\\'), normalizeName name)
                let outputDirectory = new DirectoryInfo(n)
                Some outputDirectory
            else
                None

        with | _ ->
            raise (new InvalidOperationException("Can't get the location of the executing assembly"))

    let mutable includes = Array.empty<string>

    let readFileOrDefault (path: string) : string option =
        match FileSystem.FileExistsShim(path) with
            | true -> Some <| File.ReadAllText path
            | _ -> None

    let createCompilationUnit path fs name =
        let outputDirectory =  outputDirectory name
        let filePath = path ++ fs
        let fsSource = File.ReadAllText filePath
        let bslFilePath = filePath + ".bsl"
        let ilFilePath  =
            if outputDirectory.IsSome then
                outputDirectory.Value.FullName + fs + ".il"
            else
                filePath + ".il"

        let bslSource = readFileOrDefault bslFilePath
        let ilSource = readFileOrDefault ilFilePath

        { Source         = Text fsSource
          Baseline       =
                Some { SourceFilename = Some filePath
                       OutputBaseline = { FilePath = bslFilePath; Content = bslSource }
                       ILBaseline     = { FilePath = ilFilePath;  Content = ilSource  } }
          Options        = []
          OutputType     = Library
          SourceKind     = SourceKind.Fsx
          Name           = Some fs
          IgnoreWarnings = false
          References     = []
          OutputDirectory = outputDirectory } |> FS

    member _.Includes with get() = includes and set v = includes <- v

    override _.GetData(method: MethodInfo) =
        if not (Directory.Exists(dirInfo)) then
            failwith (sprintf "Directory does not exist: \"%s\"." dirInfo)

        let allFiles : string[] = Directory.GetFiles(dirInfo, "*.fs")

        let filteredFiles =
            match (includes |> Array.map (fun f -> dirInfo ++ f)) with
                | [||] -> allFiles
                | incl -> incl

        let fsFiles = filteredFiles |> Array.map Path.GetFileName

        if fsFiles |> Array.length < 1 then
            failwith (sprintf "No required files found in \"%s\".\nAll files: %A.\nIncludes:%A." dirInfo allFiles includes)

        for f in filteredFiles do
            if not <| FileSystem.FileExistsShim(f) then
                failwithf "Requested file \"%s\" not found.\nAll files: %A.\nIncludes:%A." f allFiles includes

        fsFiles
        |> Array.map (fun fs -> createCompilationUnit dirInfo fs method.Name)
        |> Seq.map (fun c -> [| c |])
