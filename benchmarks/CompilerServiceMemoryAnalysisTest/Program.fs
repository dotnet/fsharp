open System
open System.IO
open System.Text
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Text
open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader

[<AutoOpen>]
module Helpers =

    // Hack: Currently a hack to get the runtime assemblies for netcore in order to compile.
    let assemblies =
        typeof<obj>.Assembly.Location
        |> Path.GetDirectoryName
        |> Directory.EnumerateFiles
        |> Seq.toArray
        |> Array.filter (fun x -> x.ToLowerInvariant().Contains("system."))
        |> Array.map (fun x -> sprintf "-r:%s" x)
    let netcoreArgs = Array.append [|"--targetprofile:netcore"; "--noframework"|] assemblies

    let createProject name referencedProjects =
        let tmpPath = Path.GetTempPath()
        let file = Path.Combine(tmpPath, Path.ChangeExtension(name, ".fs"))
        {
            ProjectFileName = Path.Combine(tmpPath, Path.ChangeExtension(name, ".dll"))
            ProjectId = None
            SourceFiles = [|file|]
            OtherOptions = 
                Array.append [|"--optimize+"; "--target:library" |] (referencedProjects |> Array.ofList |> Array.map (fun x -> "-r:" + x.ProjectFileName))
                |> Array.append netcoreArgs
            ReferencedProjects =
                referencedProjects
                |> List.map (fun x -> (x.ProjectFileName, x))
                |> Array.ofList
            IsIncompleteTypeCheckEnvironment = false
            UseScriptResolutionRules = false
            LoadTime = DateTime()
            UnresolvedReferences = None
            OriginalLoadReferences = []
            ExtraProjectInfo = None
            Stamp = Some 0L (* set the stamp to 0L on each run so we don't evaluate the whole project again *)
        }

    let generateSourceCode moduleName =
        sprintf """
module Benchmark.%s

type %s =

    val X : int

    val Y : int

    val Z : int

let function%s (x: %s) =
    let x = 1
    let y = 2
    let z = x + y
    z""" moduleName moduleName moduleName moduleName

type Test() =

    let checker = FSharpChecker.Create(projectCacheSize = 200)

    let mainProjectOptions =
        createProject "MainProject"
            [ for i = 1 to 100 do
                yield 
                    createProject ("ReferencedProject" + string i) []
            ]

    member this.Run() =
        mainProjectOptions.SourceFiles
        |> Seq.iter (fun file ->
            File.WriteAllText(file, generateSourceCode (Path.GetFileNameWithoutExtension(file)))
        )

        mainProjectOptions.ReferencedProjects
        |> Seq.iter (fun (_, referencedProjectOptions) ->
            referencedProjectOptions.SourceFiles
            |> Seq.iter (fun file ->
                File.WriteAllText(file, generateSourceCode (Path.GetFileNameWithoutExtension(file)))
            )
        )

        let file = mainProjectOptions.SourceFiles.[0]

        let parseResult, checkResult =                                                                
            checker.ParseAndCheckFileInProject(file, 0, SourceText.ofString (File.ReadAllText(file)), mainProjectOptions)
            |> Async.RunSynchronously

        if parseResult.Errors.Length > 0 then
            failwithf "%A" parseResult.Errors

        match checkResult with
        | FSharpCheckFileAnswer.Aborted -> failwith "aborted"
        | FSharpCheckFileAnswer.Succeeded checkFileResult ->

            if checkFileResult.Errors.Length > 0 then
                failwithf "%A" checkFileResult.Errors

[<EntryPoint>]
let main _ =
    let test = Test()
    test.Run()
    GC.Collect(2, GCCollectionMode.Forced)
    printfn "CompilerServiceMemoryAnalysisTest-Finished"
    Console.ReadLine() |> ignore
    0