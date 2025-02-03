/// Tools for generating synthetic projects where we can model dependencies between files.
///
/// Each file in the project has a string identifier. It then contains a type and a function.
/// The function calls functions from all the files the given file depends on and returns their
/// results + its own type in a tuple.
///
/// To model changes, we change the type name in a file which results in signatures of all the
/// dependent files also changing.
///
/// To model breaking changes we change the name of the function which will make dependent files
/// not compile.
///
/// To model changes to "private" code in a file we change the body of a second function which
/// no one calls.
///
module FSharp.Test.ProjectGeneration

open System
open System.Collections.Concurrent
open System.Diagnostics
open System.IO
open System.Text
open System.Text.RegularExpressions
open System.Threading.Tasks
open System.Xml

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text

open Xunit
open FSharp.Test.Utilities


open OpenTelemetry
open OpenTelemetry.Resources
open OpenTelemetry.Trace

#nowarn "57" // Experimental feature use

let private projectRoot = "test-projects"

let private defaultFunctionName = "f"

type Reference = {
    Name: string
    Version: string option }

module ReferenceHelpers =

    type Runtime =
        { Name: string
          Version: string
          Path: DirectoryInfo }

    module Seq =
        let filterOut predicate = Seq.filter (predicate >> not)

        let filterOutAny predicates =
            filterOut (fun x -> predicates |> Seq.exists ((|>) x))

    let getNugetReferences nugetSourceOpt references =
        seq {
            for nugetSource in nugetSourceOpt |> Option.toList do
                $"#i \"nuget:{nugetSource}\""

            for reference: Reference in references do
                let version = reference.Version |> Option.map (sprintf ", %s") |> Option.defaultValue ""
                $"#r \"nuget: %s{reference.Name}{version}\""
        }
        |> String.concat "\n"

    let runtimeList = lazy (
        // You can see which versions of the .NET runtime are currently installed with the following command.
        let psi =
            ProcessStartInfo("dotnet", "--list-runtimes", RedirectStandardOutput = true, UseShellExecute = false)

        let proc = Process.Start(psi)
        proc.WaitForExit(1000) |> ignore

        let output =
            seq {
                while not proc.StandardOutput.EndOfStream do
                    proc.StandardOutput.ReadLine()
            }

        /// Regex for output like: Microsoft.AspNetCore.App 5.0.13 [C:\Program Files\dotnet\shared\Microsoft.AspNetCore.App]
        let listRuntimesRegex = Regex("([^\s]+) ([^\s]+) \[(.*?)\\]")

        output
        |> Seq.map (fun x ->
            let matches = listRuntimesRegex.Match(x)
            let (version: string) = matches.Groups.[2].Value

            { Name = matches.Groups.[1].Value
              Version = version
              Path = DirectoryInfo(Path.Combine(matches.Groups[3].Value, version)) }) 
        |> Seq.toList)

    let getFrameworkReference (reference: Reference) =

        let createRuntimeLoadScript blockedDlls (r: Runtime) =
            let dir = r.Path

            let isDLL (f: FileInfo) = f.Extension = ".dll"

            let tripleQuoted (s: string) = $"\"\"\"{s}\"\"\""

            let packageSource (source: string) = $"#I {tripleQuoted source}"

            let reference (ref: string) = $"#r \"{ref}\""

            let fileReferences =
                dir.GetFiles()
                |> Seq.filter isDLL
                |> Seq.filterOutAny blockedDlls
                |> Seq.map (fun f -> reference f.Name)

            seq {
                packageSource dir.FullName
                yield! fileReferences
            }
            |> String.concat "\n"

        let contains (x: string) (y: FileInfo) = y.Name.Contains x

        // List of DLLs that FSI can't load
        let blockedDlls =
            [ contains "aspnetcorev2_inprocess"
              contains "api-ms-win"
              contains "clrjit"
              contains "clrgc"
              contains "clretwrc"
              contains "coreclr"
              contains "hostpolicy"
              contains "Microsoft.DiaSymReader.Native.amd64"
              contains "mscordaccore_amd64_amd64_7"
              contains "mscordaccore"
              contains "msquic"
              contains "mscordbi"
              contains "mscorrc"
              contains "System.IO.Compression.Native" ]

        let runTimeLoadScripts =
            runtimeList.Value
            |> Seq.map (fun runtime -> runtime.Name, (runtime, createRuntimeLoadScript blockedDlls runtime))
            |> Seq.groupBy fst
            |> Seq.map (fun (name, runtimes) -> name, runtimes |> Seq.map snd |> Seq.toList)
            |> Map

        runTimeLoadScripts
        |> Map.tryFind reference.Name
        |> Option.map (
            List.filter (fun (r, _) ->
                match reference.Version with
                | Some v -> r.Version = v
                | None -> not (r.Version.Contains "preview"))
            >> List.sortByDescending (fun (r, _) -> r.Version)
        )
        |> Option.bind List.tryHead
        |> Option.map snd
        |> Option.defaultWith (fun () ->
            failwith $"Couldn't find framework reference {reference.Name} {reference.Version}. Available Runtimes: \n"
            + (runTimeLoadScripts
               |> Map.toSeq
               |> Seq.map snd
               |> Seq.collect (List.map fst)
               |> Seq.map (fun r -> $"{r.Name} {r.Version}")
               |> String.concat "\n"))


open ReferenceHelpers


type SignatureFile =
    | No
    | AutoGenerated
    | Custom of string
    member x.CustomText = match x with Custom text -> text | _ -> failwith $"Not a custom signature file (SignatureFile.%A{x})"


type SyntheticSourceFile =
    {
        Id: string
        /// This is part of the file's type name
        PublicVersion: int
        InternalVersion: int
        DependsOn: string list
        /// Changing this makes dependent files' code invalid
        FunctionName: string
        SignatureFile: SignatureFile
        HasErrors: bool
        /// If this is set to a non-empty string then it will replace the auto-generated content of the file
        Source: string
        ExtraSource: string
        EntryPoint: bool
        /// Indicates whether this is an existing F# file on disk.
        IsPhysicalFile: bool
    }

    member this.FileName =
        if this.IsPhysicalFile then $"%s{this.Id}.fs" else $"File%s{this.Id}.fs"

    member this.SignatureFileName = $"{this.FileName}i"
    member this.TypeName = $"T{this.Id}V_{this.PublicVersion}"
    member this.ModuleName = $"Module{this.Id}"

    member this.HasSignatureFile =
        match this.SignatureFile with
        | No -> false
        | _ -> true


let sourceFile fileId deps =
    { Id = fileId
      PublicVersion = 1
      InternalVersion = 1
      DependsOn = deps
      FunctionName = defaultFunctionName
      SignatureFile = No
      HasErrors = false
      Source = ""
      ExtraSource = ""
      EntryPoint = false
      IsPhysicalFile = false }


let OptionsCache = ConcurrentDictionary<_, Lazy<FSharpProjectOptions>>()

type SyntheticProject =
    { Name: string
      ProjectDir: string
      SourceFiles: SyntheticSourceFile list
      DependsOn: SyntheticProject list
      RecursiveNamespace: bool
      OtherOptions: string list
      AutoAddModules: bool
      NugetReferences: Reference list
      FrameworkReferences: Reference list
      /// If set to true this project won't cause an exception if there are errors in the initial check
      SkipInitialCheck: bool
      UseScriptResolutionRules: bool }

    static member Create(?name: string) =
        let name = defaultArg name "TestProject"
        let name = $"{name}_{Guid.NewGuid().ToString()[..7]}"
        let dir = Path.GetFullPath projectRoot

        { Name = name
          ProjectDir = dir ++ name
          SourceFiles = []
          DependsOn = []
          RecursiveNamespace = false
          OtherOptions = []
          AutoAddModules = true
          NugetReferences = []
          FrameworkReferences = []
          SkipInitialCheck = false
          UseScriptResolutionRules = false }

    static member Create([<ParamArray>] sourceFiles: SyntheticSourceFile[]) =
        { SyntheticProject.Create() with SourceFiles = sourceFiles |> List.ofArray }

    static member Create(name: string, [<ParamArray>] sourceFiles: SyntheticSourceFile[]) =
        { SyntheticProject.Create(name) with SourceFiles = sourceFiles |> List.ofArray }
    
    static member CreateForScript(scriptFile: SyntheticSourceFile) =
        { SyntheticProject.Create() with SourceFiles = [scriptFile]; UseScriptResolutionRules = true }

    member this.Find fileId =
        this.SourceFiles
        |> List.tryFind (fun f -> f.Id = fileId)
        |> Option.defaultWith (fun () -> failwith $"File with ID '{fileId}' not found in project {this.Name}.")

    member this.FindInAllProjects fileId =
        this.GetAllFiles()
        |> List.tryFind (fun (_, f) -> f.Id = fileId)
        |> Option.defaultWith (fun () -> failwith $"File with ID '{fileId}' not found in any project.")

    member this.FindByPath path =
        this.SourceFiles
        |> List.tryFind (fun f -> this.ProjectDir ++ f.FileName = path)
        |> Option.defaultWith (fun () -> failwith $"File {path} not found in project {this.Name}.")

    member this.FindInAllProjectsByPath path =
        this.GetAllFiles()
        |> List.tryFind (fun (p, f) -> p.ProjectDir ++ f.FileName = path)
        |> Option.defaultWith (fun () -> failwith $"File {path} not found in any project.")

    member this.ProjectFileName = this.ProjectDir ++ $"{this.Name}.fsproj"

    member this.OutputFilename = this.ProjectDir ++ $"{this.Name}.dll"

    member this.GetProjectOptions(checker: FSharpChecker) =

        let key =
            this.GetAllFiles()
            |> List.collect (fun (p, f) ->
                [ p.Name
                  f.Id
                  if f.HasSignatureFile then
                      "s" ]),
            this.FrameworkReferences,
            this.NugetReferences

        let factory _ =
            lazy
            use _ = Activity.start "SyntheticProject.GetProjectOptions" [ "project", this.Name ]

            let referenceScript =
                seq {
                    yield! this.FrameworkReferences |> Seq.map getFrameworkReference
                    if not this.NugetReferences.IsEmpty then
                        this.NugetReferences |> getNugetReferences (Some "https://api.nuget.org/v3/index.json")
                }
                |> String.concat "\n"

            let baseOptions, _ =
                checker.GetProjectOptionsFromScript(
                    "file.fsx",
                    SourceText.ofString referenceScript,
                    assumeDotNetFramework = false
                )
                |> Async.RunImmediate

            {
                ProjectFileName = this.ProjectFileName
                ProjectId = None
                SourceFiles =
                    [| for f in this.SourceFiles do
                            if f.HasSignatureFile then
                                this.ProjectDir ++ f.SignatureFileName

                            this.ProjectDir ++ f.FileName |]
                OtherOptions =
                    Set [
                        yield! baseOptions.OtherOptions
                        "--optimize+"
                        for p in this.DependsOn do
                            $"-r:{p.OutputFilename}"
                        yield! this.OtherOptions ]
                        |> Set.toArray
                ReferencedProjects =
                    [| for p in this.DependsOn do
                            FSharpReferencedProject.FSharpReference(p.OutputFilename, p.GetProjectOptions checker) |]
                IsIncompleteTypeCheckEnvironment = false
                UseScriptResolutionRules = this.UseScriptResolutionRules
                LoadTime = DateTime()
                UnresolvedReferences = None
                OriginalLoadReferences = []
                Stamp = None }
       
        OptionsCache.GetOrAdd(key, factory).Value



    member this.GetAllProjects() =
        [ this
          for p in this.DependsOn do
              yield! p.GetAllProjects() ]

    member this.GetAllFiles() =
        [ for f in this.SourceFiles do
              this, f
          for p in this.DependsOn do
              yield! p.GetAllFiles() ]


let getFilePath p (f: SyntheticSourceFile) = p.ProjectDir ++ f.FileName
let getSignatureFilePath p (f: SyntheticSourceFile) = p.ProjectDir ++ f.SignatureFileName


type SyntheticProject with
    member this.GetFilePath fileId = this.Find fileId |> getFilePath this
    member this.GetSignatureFilePath fileId = this.Find fileId |> getSignatureFilePath this

    member this.SourceFilePaths =
        [ for f in this.SourceFiles do
            if f.HasSignatureFile then this.GetSignatureFilePath f.Id
            this.GetFilePath f.Id ]


let private renderNamespaceModule (project: SyntheticProject) (f: SyntheticSourceFile) =
    seq {
        if project.RecursiveNamespace then
            $"namespace rec {project.Name}"
            $"module {f.ModuleName}"
        else
            $"module %s{project.Name}.{f.ModuleName}"
    } |> String.concat Environment.NewLine

let renderSourceFile (project: SyntheticProject) (f: SyntheticSourceFile) =
    seq {
        if f.Source <> "" then
            if project.AutoAddModules then
                renderNamespaceModule project f
            f.Source
        else
            renderNamespaceModule project f
            for p in project.DependsOn |> set do
                $"open {p.Name}"

            $"type {f.TypeName}<'a> = T{f.Id} of 'a"

            $"let {f.FunctionName} x ="

            for dep in f.DependsOn |> set do
                $"    Module{dep}.{defaultFunctionName} x,"

            $"    T{f.Id} x"

            $"let f2 x = x + {f.InternalVersion}"

            f.ExtraSource

            if f.HasErrors then
                "let wrong = 1 + 'a'"

        if f.EntryPoint then
            "[<EntryPoint>]"
            "let main _ ="
            "   f 1 |> ignore"
            "   printfn \"Hello World!\""
            "   0"
    }
    |> String.concat Environment.NewLine

let renderCustomSignatureFile (project: SyntheticProject) (f: SyntheticSourceFile) =
    match f.SignatureFile with
    | Custom signature ->
        if project.AutoAddModules then
            $"{renderNamespaceModule project f}\n{signature}"
        else
            signature
    | _ -> failwith $"File {f.FileName} does not have a custom signature file."

let private renderFsProj (p: SyntheticProject) =
    seq {
        """
        <Project Sdk="Microsoft.NET.Sdk">

        <PropertyGroup>
            <OutputType>Exe</OutputType>
            <TargetFramework>net9.0</TargetFramework>
        </PropertyGroup>

        <ItemGroup>
        """

        for reference in p.FrameworkReferences do
            let version = reference.Version |> Option.map (fun v -> $" Version=\"{v}\"") |> Option.defaultValue ""
            $"<FrameworkReference Include=\"{reference.Name}\"{version}/>"

        for reference in p.NugetReferences do
            let version = reference.Version |> Option.map (fun v -> $" Version=\"{v}\"") |> Option.defaultValue ""
            $"<PackageReference Include=\"{reference.Name}\"{version}/>"

        for project in p.DependsOn do
            $"<ProjectReference Include=\"{project.ProjectFileName}\" />"

        for f in p.SourceFiles do
            if f.HasSignatureFile then
                $"<Compile Include=\"{f.SignatureFileName}\" />"

            $"<Compile Include=\"{f.FileName}\" />"

        """
        </ItemGroup>
        </Project>
        """
    }
    |> String.concat Environment.NewLine

let private writeFileIfChanged path content =
    if not (File.Exists path) || File.ReadAllText(path) <> content then
        File.WriteAllText(path, content)

let private writeFile (p: SyntheticProject) (f: SyntheticSourceFile) =
    let fileName = getFilePath p f
    let content = renderSourceFile p f
    writeFileIfChanged fileName content

/// Creates a SyntheticProject from the compiler arguments found in the response file.
let mkSyntheticProjectForResponseFile (responseFile: FileInfo) : SyntheticProject =
    if not responseFile.Exists then
        failwith $"%s{responseFile.FullName} does not exist"
    
    let compilerArgs = File.ReadAllLines responseFile.FullName

    let fsharpFileExtensions = set [| ".fs" ; ".fsi" ; ".fsx" |]

    let isFSharpFile (file : string) =
        Set.exists (fun (ext : string) -> file.EndsWith (ext, StringComparison.Ordinal)) fsharpFileExtensions
          
    let fsharpFiles =
        compilerArgs
        |> Array.choose (fun (line : string) ->
            if not (isFSharpFile line) then
                None
            else

            let fullPath = Path.Combine (responseFile.DirectoryName, line)
            if not (File.Exists fullPath) then
                None
            else
                Some fullPath
        )
        |> Array.toList

    let signatureFiles, implementationFiles =
        fsharpFiles |> List.partition (fun path -> path.EndsWith ".fsi")

    let signatureFiles = set signatureFiles

    let sourceFiles =
        implementationFiles
        |> List.map (fun implPath ->
            let id =
                let fileNameWithoutExtension = Path.GetFileNameWithoutExtension implPath 
                let directoryOfFile = FileInfo(implPath).DirectoryName
                let relativeUri = Uri(responseFile.FullName).MakeRelativeUri(Uri(directoryOfFile))
                let relativeFolderPath = Uri.UnescapeDataString(relativeUri.ToString()).Replace('/', Path.DirectorySeparatorChar)
                Path.Combine(relativeFolderPath, fileNameWithoutExtension)

            {
                  Id = id
                  PublicVersion = 1
                  InternalVersion = 1
                  DependsOn = []
                  FunctionName = "f"
                  SignatureFile =
                      let sigPath = $"%s{implPath}i" in
                      if signatureFiles.Contains sigPath then Custom(File.ReadAllText sigPath) else No
                  HasErrors = false
                  Source = File.ReadAllText implPath
                  ExtraSource = ""
                  EntryPoint = false
                  IsPhysicalFile = true 
            }
        )
    
    let otherOptions =
        compilerArgs
        |> Array.filter (fun line -> not (isFSharpFile line))
        |> Array.toList

    { SyntheticProject.Create(Path.GetFileNameWithoutExtension responseFile.Name) with
        ProjectDir = responseFile.DirectoryName
        SourceFiles = sourceFiles
        OtherOptions = otherOptions
        AutoAddModules = false
    }

[<AutoOpen>]
module ProjectOperations =

    let updateFile fileId updateFunction project =
        let index = project.SourceFiles |> List.findIndex (fun file -> file.Id = fileId)

        { project with
            SourceFiles =
                project.SourceFiles
                |> List.updateAt index (updateFunction project.SourceFiles[index]) }

    let updateFileInAnyProject fileId updateFunction (rootProject: SyntheticProject) =
        let project, _ = rootProject.FindInAllProjects fileId

        if project = rootProject then
            updateFile fileId updateFunction project
        else
            let index = rootProject.DependsOn |> List.findIndex ((=) project)

            { rootProject with
                DependsOn =
                    rootProject.DependsOn
                    |> List.updateAt index (updateFile fileId updateFunction project) }

    let private counter = (Seq.initInfinite ((+) 2)).GetEnumerator()

    let updatePublicSurface f =
        counter.MoveNext() |> ignore
        { f with PublicVersion = counter.Current }

    let updateInternal f =
        counter.MoveNext() |> ignore
        { f with InternalVersion = counter.Current }

    let breakDependentFiles f = { f with FunctionName = "g" }

    let setPublicVersion n f = { f with PublicVersion = n }

    let addDependency fileId f : SyntheticSourceFile =
        { f with DependsOn = fileId :: f.DependsOn }

    let addSignatureFile f =
        { f with SignatureFile = AutoGenerated }

    let checkFileWithIncrementalBuilder fileId (project: SyntheticProject) (checker: FSharpChecker) =
        let file = project.Find fileId
        let contents = renderSourceFile project file
        let absFileName = getFilePath project file

        checker.ParseAndCheckFileInProject(
            absFileName,
            0,
            SourceText.ofString contents,
            project.GetProjectOptions checker
        )

    let getSourceText (project: SyntheticProject) (filePath: string) =
        if filePath.EndsWith(".fsi") then
            let implFilePath = filePath[..filePath.Length - 2]
            let source = project.FindByPath implFilePath
            match source.SignatureFile with
            | No -> failwith $"{implFilePath} does not have a signature file"
            | Custom _ -> renderCustomSignatureFile project source
            | AutoGenerated ->
                if File.Exists filePath then
                    // TODO: could be outdated
                    File.ReadAllText filePath
                else
                    failwith "AutoGenerated signatures not yet supported for getSource workflow"
        else
            filePath
            |> project.FindByPath
            |> renderSourceFile project
        |> SourceTextNew.ofString

    let internal getFileSnapshot (project: SyntheticProject) _options (path: string) =
        async {
            let project, filePath =
                if path.EndsWith(".fsi") then
                    let implFilePath = path[..path.Length - 2]
                    let p, f = project.FindInAllProjectsByPath implFilePath
                    p, getSignatureFilePath p f
                else
                    let p, f = project.FindInAllProjectsByPath path
                    p, getFilePath p f

            let source = getSourceText project path
            use md5 = System.Security.Cryptography.MD5.Create()
            let inputBytes = Encoding.UTF8.GetBytes(source.ToString())
            let hash = md5.ComputeHash(inputBytes) |> Array.map (fun b -> b.ToString("X2")) |> String.concat ""

            return FSharpFileSnapshot(
                FileName = filePath,
                Version = hash,
                GetSource = fun () -> source |> Task.FromResult
            )
        }

    let checkFileWithTransparentCompiler fileId (project: SyntheticProject) (checker: FSharpChecker) =
        async {
            let file = project.Find fileId
            let absFileName = getFilePath project file
            let options = project.GetProjectOptions checker
            let! projectSnapshot = FSharpProjectSnapshot.FromOptions(options, getFileSnapshot project)
            return! checker.ParseAndCheckFileInProject(absFileName, projectSnapshot)
        }

    let checkFile fileId (project: SyntheticProject) (checker: FSharpChecker) =
        (if checker.UsesTransparentCompiler then
            checkFileWithTransparentCompiler
        else
            checkFileWithIncrementalBuilder) fileId project checker

    let getTypeCheckResult (parseResults: FSharpParseFileResults, checkResults: FSharpCheckFileAnswer) =
        Assert.True(not parseResults.ParseHadErrors)

        match checkResults with
        | FSharpCheckFileAnswer.Aborted -> failwith "Type checking was aborted"
        | FSharpCheckFileAnswer.Succeeded checkResults -> checkResults

    let tryGetTypeCheckResult (parseResults: FSharpParseFileResults, checkResults: FSharpCheckFileAnswer) =
        if not parseResults.ParseHadErrors then
            match checkResults with
            | FSharpCheckFileAnswer.Aborted -> None
            | FSharpCheckFileAnswer.Succeeded checkResults -> Some checkResults
        else None

    let getSignature parseAndCheckResults =
        parseAndCheckResults
        |> tryGetTypeCheckResult
        |> Option.bind (fun r -> r.GenerateSignature())
        |> Option.map (fun s -> s.ToString())
        |> Option.defaultValue ""

    let filterErrors (diagnostics: FSharpDiagnostic array) =
        diagnostics
        |> Array.filter (fun diag ->
            match diag.Severity with
            | FSharpDiagnosticSeverity.Hidden
            | FSharpDiagnosticSeverity.Info
            | FSharpDiagnosticSeverity.Warning -> false
            | FSharpDiagnosticSeverity.Error -> true)

    let expectOk parseAndCheckResults _ =
        let checkResult = getTypeCheckResult parseAndCheckResults
        let errors = filterErrors checkResult.Diagnostics

        if errors.Length > 0 then
            failwith $"Expected no errors, but there were some: \n%A{errors}"

    let expectSingleWarningAndNoErrors (warningSubString:string) parseAndCheckResults _  =
        let checkResult = getTypeCheckResult parseAndCheckResults
        let errors = checkResult.Diagnostics|> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Error)
        if errors.Length > 0 then
            failwith $"Expected no errors, but there were some: \n%A{errors}"

        let warnings = checkResult.Diagnostics|> Array.filter (fun d -> d.Severity = FSharpDiagnosticSeverity.Warning)
        match warnings |> Array.tryExactlyOne with
        | None -> failwith $"Expected 1 warning, but got {warnings.Length} instead: \n%A{warnings}"
        | Some w ->
            if w.Message.Contains warningSubString then
                ()
            else
                failwith $"Expected 1 warning with substring '{warningSubString}' but got %A{w}"

    let expectErrors parseAndCheckResults _ =
        let (parseResult: FSharpParseFileResults), _checkResult = parseAndCheckResults
        if not parseResult.ParseHadErrors then
            let checkResult = getTypeCheckResult parseAndCheckResults
            if
                (checkResult.Diagnostics
                 |> Array.where (fun d -> d.Severity = FSharpDiagnosticSeverity.Error))
                    .Length = 0
            then
                failwith "Expected errors, but there were none"

    let expectErrorCodes codes parseAndCheckResults _ =
        let (parseResult: FSharpParseFileResults), _checkResult = parseAndCheckResults

        if not parseResult.ParseHadErrors then
            let checkResult = getTypeCheckResult parseAndCheckResults
            let actualCodes = checkResult.Diagnostics |> Seq.map (fun d -> d.ErrorNumberText) |> Set
            let codes = Set.ofSeq codes
            if actualCodes <> codes then
                failwith $"Expected error codes {codes} but got {actualCodes}. \n%A{checkResult.Diagnostics}"

        else
            failwith $"There were parse errors: %A{parseResult.Diagnostics}"

    let expectSignatureChanged result (oldSignature: string, newSignature: string) =
        expectOk result ()
        Assert.NotEqual<string>(oldSignature, newSignature)

    let expectSignatureContains (expected: string) result (_oldSignature, newSignature) =
        expectOk result ()
        Assert.Contains(expected, newSignature)

    let expectNoChanges result (oldSignature: string, newSignature: string) =
        expectOk result ()
        Assert.Equal<string>(oldSignature, newSignature)

    let expectNumberOfResults expected (results: 'a list) =
        if results.Length <> expected then
            failwith $"Found {results.Length} references but expected to find {expected}"

    let expectToFind expected (foundRanges: range seq) =
        let expected =
            expected
            |> Seq.sortBy (fun (file, _, _, _) -> file)
            |> Seq.toArray

        let actual =
            foundRanges
            |> Seq.map (fun r -> Path.GetFileName(r.FileName), r.StartLine, r.StartColumn, r.EndColumn)
            |> Seq.sort
            |> Seq.toArray

        Assert.Equal<(string * int * int * int)[]>(expected |> Seq.sort |> Seq.toArray, actual)
        
    let expectNone x =
        if Option.isSome x then failwith "expected None, but was Some"
    
    let expectSome x =
        if Option.isNone x then failwith "expected Some, but was None"
        
    let rec saveProject (p: SyntheticProject) generateSignatureFiles checker =
        async {
            Directory.CreateDirectory(p.ProjectDir) |> ignore

            for ref in p.DependsOn do
                do! saveProject ref generateSignatureFiles checker

            for i in 0 .. p.SourceFiles.Length - 1 do
                let file = p.SourceFiles[i]
                writeFile p file

                let signatureFileName = p.ProjectDir ++ file.SignatureFileName

                match file.SignatureFile with
                | AutoGenerated when generateSignatureFiles ->
                    let project = { p with SourceFiles = p.SourceFiles[0..i - 1] @ [ { file with SignatureFile = No }] }
                    let! results = checkFile file.Id project checker
                    let signature = getSignature results
                    writeFileIfChanged signatureFileName signature
                | Custom _ ->
                    let signatureContent = renderCustomSignatureFile p file
                    writeFileIfChanged signatureFileName signatureContent
                | _ -> ()

            writeFileIfChanged (p.ProjectDir ++ $"{p.Name}.fsproj") (renderFsProj p)
        }

    // Convert AutoGenerated signature files to Custom ones so they can be edited independently.
    // This will save the project to disk.
    let rec absorbAutoGeneratedSignatures checker (p: SyntheticProject) =
        async {
            do! saveProject p true checker
            let files = [ 
                for file in p.SourceFiles do
                    if file.SignatureFile = AutoGenerated then
                        let text = file |> getSignatureFilePath p |> File.ReadAllText
                        { file with SignatureFile = Custom text }
                    else file 
            ]
            let! projects = 
                p.DependsOn 
                |> Seq.map (absorbAutoGeneratedSignatures checker)
                |> Async.Sequential
            return 
                { p with 
                    SourceFiles = files
                    AutoAddModules = false
                    DependsOn = projects |> Array.toList }
        }

module Helpers =

    let internal getSymbolUse fileName (source: string) (symbolName: string) snapshot (checker: FSharpChecker) =
        async {
            let lines = source.Split '\n' |> Seq.skip 1 // module definition
            let lineNumber, fullLine, colAtEndOfNames =
                lines
                |> Seq.mapi (fun lineNumber line ->
                    let index =  line.IndexOf symbolName
                    if index >= 0 then
                        let colAtEndOfNames = line.IndexOf symbolName + symbolName.Length
                        Some (lineNumber + 2, line, colAtEndOfNames)
                    else None)
                |> Seq.tryPick id
                |> Option.defaultValue (-1, "", -1)

            let! results = checker.ParseAndCheckFileInProject(fileName, snapshot)

            let typeCheckResults = getTypeCheckResult results

            let symbolUse =
                typeCheckResults.GetSymbolUseAtLocation(lineNumber, colAtEndOfNames, fullLine, [symbolName])

            return symbolUse |> Option.defaultWith (fun () ->
                failwith $"No symbol found in {fileName} at {lineNumber}:{colAtEndOfNames}\nFile contents:\n\n{source}\n")
        }

    let internal singleFileChecker source =

        let fileName = "test.fs"

        let getSource _ fileName =
            FSharpFileSnapshot(
              FileName = fileName,
              Version = "1",
              GetSource = fun () -> source |> SourceTextNew.ofString |> Task.FromResult )
            |> async.Return

        let checker = FSharpChecker.Create(
            keepAllBackgroundSymbolUses = false,
            enableBackgroundItemKeyStoreAndSemanticClassification = true,
            enablePartialTypeChecking = true,
            captureIdentifiersWhenParsing = true,
            useTransparentCompiler = true)

        let options =
            let baseOptions, _ =
                checker.GetProjectOptionsFromScript(
                    fileName,
                    SourceText.ofString "",
                    assumeDotNetFramework = false
                )
                |> Async.RunSynchronously

            { baseOptions with
                ProjectFileName = "project"
                ProjectId = None
                SourceFiles = [|fileName|]
                IsIncompleteTypeCheckEnvironment = false
                UseScriptResolutionRules = false
                LoadTime = DateTime()
                UnresolvedReferences = None
                OriginalLoadReferences = []
                Stamp = None }

        let snapshot = FSharpProjectSnapshot.FromOptions(options, getSource) |> Async.RunSynchronously

        fileName, snapshot, checker

open Helpers


type WorkflowContext =
    { Project: SyntheticProject
      Signatures: Map<string, string>
      Cursor: FSharpSymbolUse option }

let SaveAndCheckProject project checker isExistingProject =
    async {
        use _ =
            Activity.start "SaveAndCheckProject" [ Activity.Tags.project, project.Name ]

        // Don't save the project if it is a real world project that exists on disk.
        if not isExistingProject then
            do! saveProject project true checker

        let options = project.GetProjectOptions checker
        let! snapshot = FSharpProjectSnapshot.FromOptions(options, getFileSnapshot project)

        let! results = checker.ParseAndCheckProject(snapshot)
        let errors = filterErrors results.Diagnostics

        if not (Array.isEmpty errors || project.SkipInitialCheck) then
            failwith $"Project {project.Name} failed initial check: \n%A{errors}"

        let! signatures =
            Async.Sequential
                [ for file in project.SourceFiles do
                      async {
                          let! result = checkFile file.Id project checker
                          let signature = getSignature result
                          return file.Id, signature
                      } ]

        return
            { Project = project
              Signatures = Map signatures
              Cursor = None }
    }

type MoveFileDirection = Up | Down 

type ProjectWorkflowBuilder
    (
        initialProject: SyntheticProject,
        ?initialContext,
        ?checker: FSharpChecker,
        ?useGetSource,
        ?useChangeNotifications,
        ?useTransparentCompiler,
        ?runTimeout,
        ?autoStart,
        ?isExistingProject,
        ?enablePartialTypeChecking
    ) =

    let useTransparentCompiler = defaultArg useTransparentCompiler CompilerAssertHelpers.UseTransparentCompiler
    let useGetSource = not useTransparentCompiler && defaultArg useGetSource false
    let useChangeNotifications = not useTransparentCompiler && defaultArg useChangeNotifications false
    let autoStart = defaultArg autoStart true
    let isExistingProject = defaultArg isExistingProject false

    let mutable latestProject = initialProject
    let mutable activity = None

    let getSource f = f |> getSourceText latestProject :> ISourceText |> Some |> async.Return

    let checker =
        defaultArg
            checker
            (FSharpChecker.Create(
                keepAllBackgroundSymbolUses = true,
                enableBackgroundItemKeyStoreAndSemanticClassification = true,
                enablePartialTypeChecking = defaultArg enablePartialTypeChecking true,
                captureIdentifiersWhenParsing = true,
                documentSource = (if useGetSource then DocumentSource.Custom getSource else DocumentSource.FileSystem),
                useTransparentCompiler = useTransparentCompiler
            ))

    let mapProjectAsync f workflow =
        async {
            let! ctx = workflow
            let! project = f ctx.Project
            latestProject <- project
            return { ctx with Project = project }
        }

    let mapProject f = mapProjectAsync (f >> async.Return)

    let getInitialContext() =
        match initialContext with
        | Some ctx -> async.Return ctx
        | None -> SaveAndCheckProject initialProject checker isExistingProject

    /// Creates a ProjectWorkflowBuilder which will already have the project
    /// saved and checked so time won't be spent on that.
    /// Also the project won't be deleted after the computation expression is evaluated
    member this.CreateBenchmarkBuilder() =
        let ctx = getInitialContext() |> Async.RunSynchronously

        ProjectWorkflowBuilder(
            ctx.Project,
            ctx,
            useGetSource = useGetSource,
            useChangeNotifications = useChangeNotifications,
            useTransparentCompiler = useTransparentCompiler,
            ?runTimeout = runTimeout)

    member this.Checker = checker

    member this.Yield _ = async {
        let! ctx = getInitialContext()
        activity <- Activity.start ctx.Project.Name [ Activity.Tags.project, ctx.Project.Name; "UsingTransparentCompiler", useTransparentCompiler.ToString() ] |> Some
        return ctx
    }

    member this.DeleteProjectDir() =
        if Directory.Exists initialProject.ProjectDir then
            try Directory.Delete(initialProject.ProjectDir, true) with _ -> ()

    member this.Execute(workflow: Async<WorkflowContext>) =
        try
            Async.RunSynchronously(workflow, ?timeout = runTimeout)
        finally
            if initialContext.IsNone && not isExistingProject then
                this.DeleteProjectDir()
            activity |> Option.iter (fun x -> if not (isNull x) then x.Dispose())

    member this.Run(workflow: Async<WorkflowContext>) =
        if autoStart then
            this.Execute(workflow) |> async.Return
        else
            workflow

    [<CustomOperation "withProject">]
    member this.WithProject(workflow: Async<WorkflowContext>, f) =
        workflow |> mapProjectAsync (fun project ->
            async {
                do! f project checker
                return project
            })

    [<CustomOperation "withChecker">]
    member this.WithChecker(workflow: Async<WorkflowContext>, f) =
        async {
            let! ctx = workflow
            f checker
            return ctx
        }

    [<CustomOperation "withChecker">]
    member this.WithChecker(workflow: Async<WorkflowContext>, f) =
        async {
            let! ctx = workflow
            do! f checker
            return ctx
        }

    /// Change contents of given file using `processFile` function.
    /// Does not save the file to disk.
    [<CustomOperation "updateFile">]
    member this.UpdateFile(workflow: Async<WorkflowContext>, fileId: string, processFile) =
        workflow
        |> mapProject (updateFileInAnyProject fileId processFile)
        |> mapProjectAsync (fun project ->
            async {
                use _ =
                    Activity.start "ProjectWorkflowBuilder.UpdateFile" [ Activity.Tags.project, project.Name; "fileId", fileId ]

                if useChangeNotifications then
                    let project, file = project.FindInAllProjects fileId
                    let filePath = project.ProjectDir ++ file.FileName
                    do! checker.NotifyFileChanged(filePath, project.GetProjectOptions checker)
                    if (project.Find fileId).SignatureFile <> No then
                        do! checker.NotifyFileChanged($"{filePath}i", project.GetProjectOptions checker)

                return project
            })

    member this.UpdateFile(workflow: Async<WorkflowContext>, chooseFile, processFile) =
        async {
            let! ctx = workflow
            let file = ctx.Project.SourceFiles |> chooseFile
            let fileId = file.Id
            return! this.UpdateFile(async.Return ctx, fileId, processFile)
        }

    [<CustomOperation "regenerateSignature">]
    member this.RegenerateSignature(workflow: Async<WorkflowContext>, fileId: string) =
        workflow
        |> mapProjectAsync (fun project ->
            async {
                use _ =
                    Activity.start "ProjectWorkflowBuilder.RegenerateSignature" [ Activity.Tags.project, project.Name; "fileId", fileId ]
                let project, file = project.FindInAllProjects fileId
                let! result = checkFile fileId project checker
                let signature = getSignature result
                let signatureFileName = getSignatureFilePath project file
                writeFileIfChanged signatureFileName signature
                return project
            })

    /// Add a file above given file in the project.
    [<CustomOperation "addFileAbove">]
    member this.AddFileAbove(workflow: Async<WorkflowContext>, addAboveId: string, newFile) =
        workflow
        |> mapProject (fun project ->
            let index =
                project.SourceFiles
                |> List.tryFindIndex (fun f -> f.Id = addAboveId)
                |> Option.defaultWith (fun () -> failwith $"File {addAboveId} not found")

            { project with SourceFiles = project.SourceFiles |> List.insertAt index newFile })

    /// Remove a file from the project. The file is not deleted from disk.
    [<CustomOperation "removeFile">]
    member this.RemoveFile(workflow: Async<WorkflowContext>, fileId: string) =
        workflow
        |> mapProject (fun project ->
            { project with SourceFiles = project.SourceFiles |> List.filter (fun f -> f.Id <> fileId) })

    /// Parse and type check given file and process the results using `processResults` function.
    [<CustomOperation "checkFile">]
    member this.CheckFile(workflow: Async<WorkflowContext>, fileId: string, processResults) =
        async {
            let! ctx = workflow

            let! results =
                use _ = Activity.start "ProjectWorkflowBuilder.CheckFile" [ Activity.Tags.project, initialProject.Name; "fileId", fileId ]
                checkFile fileId ctx.Project checker

            let oldSignature = ctx.Signatures[fileId]
            let newSignature = getSignature results

            processResults results (oldSignature, newSignature)

            return { ctx with Signatures = ctx.Signatures.Add(fileId, newSignature) }
        }

    member this.CheckFile(workflow: Async<WorkflowContext>, fileId: string, processResults) =
        async {
            let! ctx = workflow
            use _ =
                Activity.start "ProjectWorkflowBuilder.CheckFile" [ Activity.Tags.project, initialProject.Name; "fileId", fileId ]

            let! results = checkFile fileId ctx.Project checker
            let typeCheckResults = getTypeCheckResult results

            let newSignature = getSignature results

            processResults typeCheckResults

            return { ctx with Signatures = ctx.Signatures.Add(fileId, newSignature) }
        }

    [<CustomOperation "moveFile">]
    member this.MoveFile(workflow: Async<WorkflowContext>, fileId: string, count, direction: MoveFileDirection) =

       workflow
        |> mapProject (fun project ->
            let index =
                project.SourceFiles
                |> List.tryFindIndex (fun f -> f.Id = fileId)
                |> Option.defaultWith (fun () -> failwith $"File {fileId} not found")

            let dir = if direction = Up then -1 else 1
            let newIndex = index + count * dir

            if newIndex < 0 || newIndex > project.SourceFiles.Length - 1 then
                failwith $"Cannot move file {fileId} {count} times {direction} as it would be out of bounds"

            let file = project.SourceFiles.[index]
            let newFiles =
                project.SourceFiles
                |> List.filter (fun f -> f.Id <> fileId)
                |> List.insertAt newIndex file

            { project with SourceFiles = newFiles })

    /// Find a symbol using the provided range, mimicking placing a cursor on it in IDE scenarios
    [<CustomOperation "placeCursor">]
    member this.PlaceCursor(workflow: Async<WorkflowContext>, fileId, line, colAtEndOfNames, fullLine, symbolNames) =
        async {
            let! ctx = workflow
            let! results = checkFile fileId ctx.Project checker
            let typeCheckResults = getTypeCheckResult results

            let su =
                typeCheckResults.GetSymbolUseAtLocation(line, colAtEndOfNames, fullLine, symbolNames)

            if su.IsNone then
                let file = ctx.Project.Find fileId

                failwith
                    $"No symbol found in {file.FileName} at {line}:{colAtEndOfNames}\nFile contents:\n\n{renderSourceFile ctx.Project file}\n"

            return { ctx with Cursor = su }
        }

    member this.FindSymbolUse(ctx: WorkflowContext, fileId, symbolName: string) =
        async {
            let project, file = ctx.Project.FindInAllProjects fileId
            let fileName = project.ProjectDir ++ file.FileName
            let source = renderSourceFile project file
            let options = project.GetProjectOptions checker
            let! snapshot = FSharpProjectSnapshot.FromOptions(options, getFileSnapshot ctx.Project)
            return! getSymbolUse fileName source symbolName snapshot checker
        }

    /// Find a symbol by finding the first occurrence of the symbol name in the file
    [<CustomOperation "placeCursor">]
    member this.PlaceCursor(workflow: Async<WorkflowContext>, fileId, symbolName: string) =
        async {
            let! ctx = workflow
            let! su = this.FindSymbolUse(ctx, fileId, symbolName)
            return { ctx with Cursor = Some su }
        }

    [<CustomOperation "checkSymbolUse">]
    member this.CheckSymbolUse(workflow: Async<WorkflowContext>, fileId, symbolName: string, check) =
        async {
            let! ctx = workflow
            let! su = this.FindSymbolUse(ctx, fileId, symbolName)
            check su
            return ctx
        }

    /// Find all references within a single file, results are provided to the 'processResults' function
    [<CustomOperation "findAllReferencesInFile">]
    member this.FindAllReferencesInFile(workflow: Async<WorkflowContext>, fileId: string, processResults) =
        async {
            let! ctx = workflow
            let options = ctx.Project.GetProjectOptions checker

            let symbolUse =
                ctx.Cursor
                |> Option.defaultWith (fun () ->
                    failwith $"Please place cursor at a valid location via placeCursor first")

            let file = ctx.Project.Find fileId
            let absFileName = getFilePath ctx.Project file

            let! results =
                checker.FindBackgroundReferencesInFile(absFileName, options, symbolUse.Symbol, fastCheck = true)

            processResults (results |> Seq.toList)

            return ctx
        }

    /// Find all references within the project, results are provided to the 'processResults' function
    [<CustomOperation "findAllReferences">]
    member this.FindAllReferences(workflow: Async<WorkflowContext>, processResults) =
        async {
            let! ctx = workflow

            let symbolUse =
                ctx.Cursor
                |> Option.defaultWith (fun () ->
                    failwith $"Please place cursor at a valid location via placeCursor first")

            let! results =
                [ for p, f in ctx.Project.GetAllFiles() do
                    let options = p.GetProjectOptions checker
                    for fileName in [getFilePath p f; if f.SignatureFile <> No then getSignatureFilePath p f] do
                        async {
                            let! snapshot = FSharpProjectSnapshot.FromOptions(options, getFileSnapshot ctx.Project)
                            return! checker.FindBackgroundReferencesInFile(fileName, snapshot, symbolUse.Symbol)
                        } ]
                |> Async.Parallel

            results |> Seq.collect id |> Seq.toList |> processResults
            return ctx
        }

    /// Save given file to disk.
    [<CustomOperation "saveFile">]
    member this.SaveFile(workflow: Async<WorkflowContext>, fileId: string) =
        async {
            let! ctx = workflow
            let project, file = ctx.Project.FindInAllProjects fileId
            writeFile project file
            return ctx
        }

    /// Save all files to disk.
    [<CustomOperation "saveAll">]
    member this.SaveAll(workflow: Async<WorkflowContext>) =
        async {
            let! ctx = workflow
            do! saveProject ctx.Project false checker
            return ctx
        }

    /// Clear checker caches.
    [<CustomOperation "clearCache">]
    member this.ClearCache(workflow: Async<WorkflowContext>) =
        async {
            let! ctx = workflow
            let options = [for p in ctx.Project.GetAllProjects() -> p.GetProjectOptions checker]
            checker.ClearCache(options)
            checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
            return ctx
        }

    /// Find all references to a module defined in a given file.
    /// These should only be found in files that depend on this file.
    ///
    /// Requires `enableBackgroundItemKeyStoreAndSemanticClassification` to be true in the checker.
    [<CustomOperation "findAllReferencesToModuleFromFile">]
    member this.FindAllReferencesToModuleFromFile(workflow, fileId, fastCheck, processResults) =
        async {
            let! ctx = workflow
            let! results = checkFile fileId ctx.Project checker
            let typeCheckResult = getTypeCheckResult results
            let moduleName = (ctx.Project.Find fileId).ModuleName

            let symbolUse =
                typeCheckResult.GetSymbolUseAtLocation(
                    1,
                    moduleName.Length + ctx.Project.Name.Length + 8,
                    $"module {ctx.Project.Name}.{moduleName}",
                    [ moduleName ]
                )
                |> Option.defaultWith (fun () -> failwith "no symbol use found")

            let options = ctx.Project.GetProjectOptions checker

            let! results =
                [ for f in options.SourceFiles do
                      checker.FindBackgroundReferencesInFile(f, options, symbolUse.Symbol, fastCheck = fastCheck) ]
                |> Async.Parallel

            results |> Seq.collect id |> Seq.toList |> processResults
            return ctx
        }

    [<CustomOperation "compileWithFSC">]
    member this.Compile(workflow: Async<WorkflowContext>) =
        async {
            let! ctx = workflow
            let projectOptions = ctx.Project.GetProjectOptions(checker)
            let arguments =
                [|
                    yield "fsc.exe"
                    yield! projectOptions.OtherOptions
                    yield! projectOptions.SourceFiles
                |]
            let! _diagnostics, ex = checker.Compile(arguments)
            if ex.IsSome then raise ex.Value
            return ctx
        }
        
    [<CustomOperation "tryGetRecentCheckResults">]
    member this.TryGetRecentCheckResults(workflow: Async<WorkflowContext>, fileId: string, expected) =
        async {
            let! ctx = workflow
            let project, file = ctx.Project.FindInAllProjects fileId
            let fileName = project.ProjectDir ++ file.FileName
            let options = project.GetProjectOptions checker
            let! snapshot = FSharpProjectSnapshot.FromOptions(options, getFileSnapshot ctx.Project)
            let r = checker.TryGetRecentCheckResultsForFile(fileName, snapshot)
            expected r
            
            match r with
            | Some(parseFileResults, checkFileResults) ->
                let signature = getSignature(parseFileResults, FSharpCheckFileAnswer.Succeeded(checkFileResults)) 
                match ctx.Signatures.TryFind(fileId) with
                | Some priorSignature -> Assert.Equal(priorSignature, signature)
                | None -> ()
            | None -> ()
            
            return ctx
        }

/// Execute a set of operations on a given synthetic project.
/// The project is saved to disk and type checked at the start.
let projectWorkflow project = ProjectWorkflowBuilder project


type SyntheticProject with

    /// Execute a set of operations on this project.
    /// The project is saved to disk and type checked at the start.
    member this.Workflow = projectWorkflow this

    member this.WorkflowWith checker =
        ProjectWorkflowBuilder(this, checker = checker)

    /// Saves project to disk and checks it with default options. Returns the FSharpChecker that was created
    member this.SaveAndCheck() =
        this.Workflow.Yield() |> Async.RunSynchronously |> ignore
        this.Workflow.Checker

    static member CreateFromRealProject(projectDir) =

        let projectFile =
            projectDir
            |> Directory.GetFiles
            |> Seq.filter (fun f -> f.EndsWith ".fsproj")
            |> Seq.toList
            |> function
                | [] -> failwith $"No .fsproj file found in {projectDir}"
                | [ x ] -> x
                | files -> failwith $"Multiple .fsproj files found in {projectDir}: {files}"

        let fsproj = XmlDocument()
        do fsproj.Load projectFile

        let signatureFiles, sourceFiles =
            [ for node in fsproj.DocumentElement.SelectNodes("//Compile") ->
                  projectDir ++ node.Attributes["Include"].InnerText ]
            |> List.partition (fun path -> path.EndsWith ".fsi")
        let signatureFiles = set signatureFiles
        
        let parseReferences refType =
            [ for node in fsproj.DocumentElement.SelectNodes($"//{refType}") do
                 { Name = node.Attributes["Include"].InnerText
                   Version = node.Attributes["Version"] |> Option.ofObj |> Option.map (fun x -> x.InnerText) } ]

        let name = Path.GetFileNameWithoutExtension projectFile

        let nowarns =
            [ for node in fsproj.DocumentElement.SelectNodes("//NoWarn") do
                  yield! node.InnerText.Split(';') ]

        { SyntheticProject.Create(
              name,
              [| for f in sourceFiles do
                     { sourceFile (Path.GetFileNameWithoutExtension f) [] with
                         Source = File.ReadAllText f
                         SignatureFile = if signatureFiles.Contains $"{f}i" then Custom (File.ReadAllText $"{f}i") else No
                         } |]
          ) with
            AutoAddModules = false
            NugetReferences = parseReferences "PackageReference"
            FrameworkReferences = parseReferences "FrameworkReference"
            OtherOptions =
                [ for w in nowarns do
                      $"--nowarn:{w}" ] }
