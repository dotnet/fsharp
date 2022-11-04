﻿module FSharp.Compiler.ComponentTests.FSharpChecker.CommonWorkflows

open System
open System.IO

open Xunit

open FSharp.Test.ProjectGeneration

let projectDir = "test-projects"

let makeTestProject () =
    let name = $"testProject{Guid.NewGuid().ToString()[..7]}"
    let dir = Path.GetFullPath projectDir
    {
        Name = name
        ProjectDir = dir ++ name
        SourceFiles = [
            sourceFile "First" []
            sourceFile "Second" ["First"]
            sourceFile "Third" ["First"]
            { sourceFile "Last" ["Second"; "Third"] with EntryPoint = true }
        ]
        DependsOn = []
    }

[<Fact>]
let ``Edit file, check it, then check dependent file`` () =
    projectWorkflow (makeTestProject()) {
        updateFile "First" breakDependentFiles
        checkFile "First" expectSignatureChanged
        saveFile "First"
        checkFile "Second" expectErrors
    }

[<Fact>]
let ``Edit file, don't check it, check dependent file`` () =
    projectWorkflow (makeTestProject()) {
        updateFile "First" breakDependentFiles
        saveFile "First"
        checkFile "Second" expectErrors
    }

[<Fact>]
let ``Check transitive dependency`` () =
    projectWorkflow (makeTestProject()) {
        updateFile "First" breakDependentFiles
        saveFile "First"
        checkFile "Last" expectSignatureChanged
    }

[<Fact>]
let ``Change multiple files at once`` () =
    projectWorkflow (makeTestProject()) {
        updateFile "First" (setPublicVersion 2)
        updateFile "Second" (setPublicVersion 2)
        updateFile "Third" (setPublicVersion 2)
        saveAll
        checkFile "Last" (expectSignatureContains "val f: x: 'a -> (ModuleFirst.TFirstV_2<'a> * ModuleSecond.TSecondV_2<'a>) * (ModuleFirst.TFirstV_2<'a> * ModuleThird.TThirdV_2<'a>) * TLastV_1<'a>")
    }

[<Fact>]
let ``Files depend on signature file if present`` () =
    (makeTestProject()
    |> updateFile "First" (fun f -> { f with HasSignatureFile = true })
    |> projectWorkflow) {
        updateFile "First" breakDependentFiles
        saveFile "First"
        checkFile "Second" expectNoChanges
    }

[<Fact>]
let ``Adding a file`` () =
    projectWorkflow (makeTestProject()) {
        addFileAbove "Second" (sourceFile "New" [])
        updateFile "Second" (addDependency "New")
        saveAll
        checkFile "Last" (expectSignatureContains "val f: x: 'a -> (ModuleNew.TNewV_1<'a> * ModuleFirst.TFirstV_1<'a> * ModuleSecond.TSecondV_1<'a>) * (ModuleFirst.TFirstV_1<'a> * ModuleThird.TThirdV_1<'a>) * TLastV_1<'a>")
    }

[<Fact>]
let ``Removing a file`` () =
    projectWorkflow (makeTestProject()) {
        removeFile "Second"
        saveAll
        checkFile "Last" expectErrors
    }

[<Fact>]
let ``Changes in a referenced project`` () =
    let name = $"library{Guid.NewGuid().ToString()[..7]}"
    let dir = Path.GetFullPath projectDir
    let library = {
        Name = name
        ProjectDir = dir ++ name
        SourceFiles = [ sourceFile "Library" [] ]
        DependsOn = []
    }

    let project =
        { makeTestProject() with DependsOn = [library] }
        |> updateFile "First" (addDependency "Library")

    projectWorkflow project {
        updateFile "Library" updatePublicSurface
        saveFile "Library"
        checkFile "Last" expectSignatureChanged
    }
