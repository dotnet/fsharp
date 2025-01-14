module FSharpChecker.CommonWorkflows

open System
open System.IO
open System.Diagnostics

open Xunit

open FSharp.Test.ProjectGeneration
open FSharp.Compiler.Text
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics

open OpenTelemetry
open OpenTelemetry.Resources
open OpenTelemetry.Trace

#nowarn "57"

let makeTestProject () =
    SyntheticProject.Create(
        sourceFile "First" [],
        sourceFile "Second" ["First"],
        sourceFile "Third" ["First"],
        { sourceFile "Last" ["Second"; "Third"] with EntryPoint = true })

[<Fact>]
let ``Edit file, check it, then check dependent file`` () =
    makeTestProject().Workflow {
        updateFile "First" breakDependentFiles
        checkFile "First" expectSignatureChanged
        saveFile "First"
        checkFile "Second" expectErrors
    }

[<Fact>]
let ``Edit file, don't check it, check dependent file`` () =
    makeTestProject().Workflow {
        updateFile "First" breakDependentFiles
        saveFile "First"
        checkFile "Second" expectErrors
    }

[<Fact>]
let ``Check transitive dependency`` () =
    makeTestProject().Workflow {
        updateFile "First" breakDependentFiles
        saveFile "First"
        checkFile "Last" expectSignatureChanged
    }

[<Fact>]
let ``Change multiple files at once`` () =
    makeTestProject().Workflow {
        updateFile "First" (setPublicVersion 2)
        updateFile "Second" (setPublicVersion 2)
        updateFile "Third" (setPublicVersion 2)
        saveAll
        checkFile "Last" (expectSignatureContains "val f: x: 'a -> (ModuleFirst.TFirstV_2<'a> * ModuleSecond.TSecondV_2<'a>) * (ModuleFirst.TFirstV_2<'a> * ModuleThird.TThirdV_2<'a>) * TLastV_1<'a>")
    }

[<Fact>]
let ``Files depend on signature file if present`` () =
    (makeTestProject()
    |> updateFile "First" addSignatureFile
    |> projectWorkflow) {
        updateFile "First" breakDependentFiles
        saveFile "First"
        checkFile "Second" expectNoChanges
    }

[<Fact>]
let ``Adding a file`` () =
    makeTestProject().Workflow {
        addFileAbove "Second" (sourceFile "New" [])
        updateFile "Second" (addDependency "New")
        saveAll
        checkFile "Last" (expectSignatureContains "val f: x: 'a -> (ModuleFirst.TFirstV_1<'a> * ModuleNew.TNewV_1<'a> * ModuleSecond.TSecondV_1<'a>) * (ModuleFirst.TFirstV_1<'a> * ModuleThird.TThirdV_1<'a>) * TLastV_1<'a>")
    }

[<Fact>]
let ``Removing a file`` () =
    makeTestProject().Workflow {
        removeFile "Second"
        saveAll
        checkFile "Last" expectErrors
    }

[<Fact>]
let ``Changes in a referenced project`` () =
    let library = SyntheticProject.Create("library", sourceFile "Library" [])

    let project =
        { makeTestProject() with DependsOn = [library] }
        |> updateFile "First" (addDependency "Library")

    project.Workflow {
        updateFile "Library" updatePublicSurface
        saveFile "Library"
        checkFile "Last" expectSignatureChanged
    }

[<Fact(Skip="TODO")>]
// TODO: This will probably require some special care in TransparentCompiler...
let ``Language service works if the same file is listed twice`` () =
    let file = sourceFile "First" []
    let project =  SyntheticProject.Create(file, file)
    project.Workflow {
        // checkFile "First" expectOk
        // addFileAbove "First" file
        checkFile "First" (expectSingleWarningAndNoErrors "Please verify that it is included only once in the project file.")
    }

[<Fact>]
let ``Using getSource and notifications instead of filesystem`` () =

    let size = 20

    let project =
        { SyntheticProject.Create() with
            SourceFiles = [
                sourceFile $"File%03d{0}" []
                for i in 1..size do
                    sourceFile $"File%03d{i}" [$"File%03d{i-1}"]
            ]
        }

    let first = "File001"
    let middle = $"File%03d{size / 2}"
    let last = $"File%03d{size}"

    ProjectWorkflowBuilder(project, useGetSource = true, useChangeNotifications = true) {
        updateFile first updatePublicSurface
        checkFile first expectSignatureChanged
        checkFile last expectSignatureChanged
        updateFile middle updatePublicSurface
        checkFile last expectSignatureChanged
        addFileAbove middle (sourceFile "addedFile" [first])
        updateFile middle (addDependency "addedFile")
        checkFile middle expectSignatureChanged
        checkFile last expectSignatureChanged
    }

[<Fact>]
let GetAllUsesOfAllSymbols() =

    let result =
        async {
            let project = makeTestProject()
            let checker = ProjectWorkflowBuilder(project, useGetSource=true, useChangeNotifications = true, enablePartialTypeChecking = false).Checker
            do! saveProject project false checker
            let options = project.GetProjectOptions checker
            let! checkProjectResults = checker.ParseAndCheckProject(options)
            return checkProjectResults.GetAllUsesOfAllSymbols()
        } |> Async.RunSynchronously

    if result.Length <> 79 then failwith $"Expected 81 symbolUses, got {result.Length}:\n%A{result}"

[<Fact>]
let ``We don't lose subsequent diagnostics when there's error in one file`` () =
    let project =
        { SyntheticProject.Create(
            { sourceFile "First" [] with
                Source = """module AbstractBaseClass.File1 
                
                let foo x = ()
                
                a""" },
            { sourceFile "Second" [] with
                Source = """module AbstractBaseClass.File2

                open AbstractBaseClass.File1

                let goo = foo 1

                type AbstractBaseClass() =

                    abstract P: int""" }) with
            AutoAddModules = false
            SkipInitialCheck = true }
                
    project.Workflow {
        checkFile "First" (expectErrorCodes ["FS0039"])
        checkFile "Second" (expectErrorCodes ["FS0054"; "FS0365"])
    }
