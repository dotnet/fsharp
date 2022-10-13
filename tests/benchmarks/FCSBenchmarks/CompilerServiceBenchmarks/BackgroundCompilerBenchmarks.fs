module FSharp.Benchmarks.BackgroundCompilerBenchmarks

open System
open System.IO
open BenchmarkDotNet.Attributes
open FSharp.Compiler.Text

let projectRoot = __SOURCE_DIRECTORY__


type SyntheticSourceFile = {
    Id: string
    PublicVersion: int
    InternalVersion: int
    PublicUnusedVersion: int
    DependsOn: string list
    EntryPoint: bool
} with
    member this.FileName = $"File{this.Id}.fs"

let sourceFile id deps = {
    Id = id
    PublicVersion = 1
    InternalVersion = 1
    PublicUnusedVersion = 1
    DependsOn = deps
    EntryPoint = false
}

type SyntheticProject = {
    Name: string
    SourceFiles: SyntheticSourceFile list
} with
    member this.Find id = this.SourceFiles |> List.find (fun f -> f.Id = id)

let somethingToCompile = File.ReadAllText (projectRoot ++ "SomethingToCompile.fs")

let renderSourceFile projectName (f: SyntheticSourceFile) =
    seq {
        $"module %s{projectName}.Module{f.Id}"

        $"type T{f.Id}v{f.PublicVersion}<'a> = T{f.Id} of 'a"

        "let f x ="
        for dep in f.DependsOn do
            $"    Module{dep}.f x,"
        $"    T{f.Id} x"

        $"let NobodyUsesThis_v{f.PublicUnusedVersion} = ()"

        $"let private PrivateFunc_v{f.InternalVersion} = ()"

        somethingToCompile

        if f.EntryPoint then
            "[<EntryPoint>]"
            "let main _ ="
            "   f 1 |> ignore"
            "   printfn \"Hello World!\""
            "   0"
    }
    |> String.concat Environment.NewLine

let renderFsProj (p: SyntheticProject) =
    seq {
        """
        <Project Sdk="Microsoft.NET.Sdk">

        <PropertyGroup>
            <OutputType>Exe</OutputType>
            <TargetFramework>net7.0</TargetFramework>
        </PropertyGroup>
        """

        "<ItemGroup>"

        for f in p.SourceFiles do
            $"<Compile Include=\"{f.FileName}\" />"

        "</ItemGroup>"

        "</Project>"
    }
    |> String.concat Environment.NewLine

let writeFileIfChanged path content =
    if not (File.Exists path) || File.ReadAllText(path) <> content then
        File.WriteAllText(path, content)

let saveFile projectDir (p: SyntheticProject) (f: SyntheticSourceFile) =
    let fileName = projectDir ++ f.FileName
    let content = renderSourceFile p.Name f
    writeFileIfChanged fileName content

let writeProject dir (p: SyntheticProject) =
    let projectDir = dir ++ p.Name
    Directory.CreateDirectory(projectDir) |> ignore
    for f in p.SourceFiles do
        saveFile projectDir p f
    writeFileIfChanged (projectDir ++ $"{p.Name}.fsproj") (renderFsProj p)

let createProject name size =
    {
        Name = name
        SourceFiles = [
            sourceFile "First" []
            sourceFile "1" [ "First" ]
            for n in 2..(size / 2) do
                sourceFile $"{n}" [ "First"; $"{n-1}" ]
            sourceFile "Middle" [ $"{size / 4}" ]
            for n in ((size / 2) + 1)..size do
                sourceFile $"{n}" [ "First"; "Middle" ]
            { sourceFile "Last" [ for n in (size / 2)..size -> $"{n}" ] with EntryPoint = true }
        ]
    }

let updateFile id f project =
    let index = project.SourceFiles |> List.findIndex (fun f -> f.Id = id)
    { project with SourceFiles = project.SourceFiles |> List.updateAt index (f project.SourceFiles[index]) }

let counter = (Seq.initInfinite id).GetEnumerator()

let updatePublicSurface f =
    counter.MoveNext() |> ignore
    { f with PublicVersion = counter.Current }

let updateUnused f =
    counter.MoveNext() |> ignore
    { f with PublicUnusedVersion = counter.Current }

let updateInternal f =
    counter.MoveNext() |> ignore
    { f with InternalVersion = counter.Current }


[<MemoryDiagnoser>]
[<BenchmarkCategory(FSharpCategory)>]
type BackgroundCompilerBenchmarks () =

    let size = 20

    let initialProjectModel = createProject "SyntheticProject" size
    let projDir = projectRoot ++ initialProjectModel.Name
    do if Directory.Exists projDir then DirectoryInfo(projDir).Delete(true)

    do initialProjectModel |> writeProject projectRoot

    let projectDir, projectOptions, checker = prepareProject initialProjectModel.Name |> parseAndTypeCheckProject

    let checkFile fileId (project: SyntheticProject) =
        let file = project.Find fileId
        let contents = renderSourceFile project.Name file
        let absFileName = projectDir ++ file.FileName
        checker.ParseAndCheckFileInProject(absFileName, 0, SourceText.ofString contents, projectOptions)
        |> Async.RunSynchronously
        |> validateCheckResult
        |> ignore
        project

    let editFile fileId f = updateFile fileId f >> checkFile fileId

    let save fileId (project: SyntheticProject) =
        let f = project.Find fileId
        saveFile projectDir project f
        project

    let editAndSave fileId f = editFile fileId f >> save fileId

    [<IterationSetup(Targets = [| "EditFirstFile_CheckLastFile"; "EditFirstFile_CheckMiddleFile" |])>]
    member _.EditFirstFile() =
        initialProjectModel
        |> updateFile "First" updatePublicSurface
        |> writeProject projectRoot

    [<Benchmark>]
    member _.EditFirstFile_CheckLastFile() =
        initialProjectModel |> checkFile "Last"

    [<Benchmark>]
    member _.EditFirstFile_CheckMiddleFile() =
        initialProjectModel |> checkFile "Middle"

    [<IterationSetup(Targets = [| "EditFirstFile_OnlyInternalChange_CheckLastFile" |])>]
    member _.EditFirstFile_OnlyInternalChange() =
        initialProjectModel
        |> updateFile "First" updateInternal
        |> writeProject projectRoot

    [<Benchmark>]
    member _.EditFirstFile_OnlyInternalChange_CheckLastFile() =
        initialProjectModel |> checkFile "Last"

    [<IterationSetup(Targets = [| "EditFirstFile_ChangeUnusedFunction_CheckLastFile" |])>]
    member _.EditFirstFile_ChangeUnusedFunction() =
        initialProjectModel
        |> updateFile "First" updateUnused
        |> writeProject projectRoot

    [<Benchmark>]
    member _.EditFirstFile_ChangeUnusedFunction_CheckLastFile() =
        initialProjectModel |> checkFile "Last"

    [<IterationSetup(Targets = [| "EditMiddleFile_CheckMiddleFile"
                                  "EditMiddleFile_CheckLastFile"
                                  "EditMiddleFile_CheckSecondToLastFile" |])>]
    member _.EditMiddleFile() =
        initialProjectModel
        |> updateFile "Middle" updatePublicSurface
        |> writeProject projectRoot

    [<Benchmark>]
    member _.EditMiddleFile_CheckMiddleFile() =
        initialProjectModel |> checkFile "Middle"

    [<Benchmark>]
    member _.EditMiddleFile_CheckLastFile() =
        initialProjectModel |> checkFile "Last"

    [<Benchmark>]
    // The checked file depends only on first and middle file, not on the ones in between
    member _.EditMiddleFile_CheckSecondToLastFile() =
        initialProjectModel |> checkFile $"{size}"

    [<Benchmark>]
    member _.EditCycle_WithoutSaving() =
        initialProjectModel
        |> editFile "First" updatePublicSurface
        |> editFile "Middle" updatePublicSurface
        |> editFile "Last" updatePublicSurface
        |> editFile "First" updateInternal
        |> editFile "Middle" updateInternal
        |> editFile "Last" updateInternal

    [<Benchmark>]
    member _.EditCycle_WithSaving() =
        initialProjectModel
        |> editAndSave "First" updatePublicSurface
        |> editAndSave "Middle" updatePublicSurface
        |> editAndSave "Last" updatePublicSurface
        |> editAndSave "First" updateInternal
        |> editAndSave "Middle" updateInternal
        |> editAndSave "Last" updateInternal
