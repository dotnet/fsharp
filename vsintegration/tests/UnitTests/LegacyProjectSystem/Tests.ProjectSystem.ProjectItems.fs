// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.ProjectSystem

open System
open System.IO
open Xunit
open UnitTests.TestLib.Utils.Asserts
open UnitTests.TestLib.ProjectSystem
open Microsoft.VisualStudio.FSharp.ProjectSystem


module private DebuggingHelpers = 
    open System.Diagnostics
    open Microsoft.Build.Utilities
    let private envFlag () = Environment.GetEnvironmentVariable("FSHARP_DIAG_FRAMEWORK") = "1" || true

    let private fwDiagBasicsOnce =
        lazy (
            if envFlag() then
                let pfX86 = Environment.GetEnvironmentVariable "ProgramFiles(x86)"
                let pf    = Environment.GetEnvironmentVariable "ProgramFiles"
                let dirs =
                    [ if not (String.IsNullOrWhiteSpace pfX86) then
                          Path.Combine(pfX86,"Reference Assemblies","Microsoft","Framework",".NETFramework","v4.7.2")
                      if not (String.IsNullOrWhiteSpace pf) then
                          Path.Combine(pf,"Reference Assemblies","Microsoft","Framework",".NETFramework","v4.7.2") ]
                for d in dirs do
                    let exists = Directory.Exists d
                    printfn "FWDiag: RefDir='%s' Exists=%b" d exists
                    if exists then
                        try
                            let dlls = Directory.EnumerateFiles(d, "*.dll") |> Seq.length
                            printfn "FWDiag:   DLLCount=%d" dlls
                        with e ->
                            printfn "FWDiag:   DLLCount=ERR(%s)" e.Message
                        let sn = Path.Combine(d,"System.Configuration.dll")
                        if File.Exists sn then
                            let fi = FileInfo(sn)
                            let ver = FileVersionInfo.GetVersionInfo(sn).FileVersion
                            printfn "FWDiag:   System.Configuration.dll Size=%d Version=%s" fi.Length ver
                        else
                            printfn "FWDiag:   System.Configuration.dll MISSING"
                // ToolLocationHelper probe
                try
                    let p = ToolLocationHelper.GetPathToDotNetFrameworkFile("System.Configuration.dll", TargetDotNetFrameworkVersion.Version472, DotNetFrameworkArchitecture.Current)
                    printfn "FWDiag: ToolLocationHelper(System.Configuration)='%s'" (if String.IsNullOrWhiteSpace p then "<null>" else p)
                with e ->
                    printfn "FWDiag: ToolLocationHelper exception: %s" e.Message
                printfn "FWDiag: Is64BitProcess=%b PROC_ARCH=%s" Environment.Is64BitProcess (Environment.GetEnvironmentVariable "PROCESSOR_ARCHITECTURE")
                for v in [ "VisualStudioVersion"; "FrameworkPathOverride"; "TargetFrameworkRootPath" ] do
                    printfn "FWDiag: Env %s='%s'" v (Environment.GetEnvironmentVariable v)
        )

    let fwDiagProject (project: UnitTestingFSharpProjectNode) =
        if envFlag() then
            fwDiagBasicsOnce.Force() |> ignore
            try
                // BuildProject is Microsoft.Build.Evaluation.Project
                let p = project.BuildProject
                let get name = p.GetPropertyValue name
                let tfd = get "TargetFrameworkDirectories"
                let vs  = get "VisualStudioVersion"
                let tv  = get "TargetFrameworkVersion"
                let ti  = get "TargetFrameworkIdentifier"
                let tp  = get "TargetFrameworkProfile"
                let mtp = get "MSBuildToolsPath"
                let mrt = get "MSBuildRuntimeType"
                let mrv = get "MSBuildRuntimeVersion"
                printfn "FWDiag: ProjectProps TFV='%s' TFI='%s' TFP='%s' VS='%s'" tv ti tp vs
                printfn "FWDiag: Project MSBuildToolsPath='%s' RuntimeType='%s' RuntimeVersion='%s'" mtp mrt mrv
                printfn "FWDiag: Project TargetFrameworkDirectories='%s'" (if String.IsNullOrWhiteSpace tfd then "<empty>" else tfd)
            with e ->
                printfn "FWDiag: Unable to read project properties (%s)" e.Message

    let dumpOnFailure (project: UnitTestingFSharpProjectNode) =
        // Called only when an assertion about System.Configuration is about to fail
        let opts =
            try project.CompilationOptions with _ -> [||]
        printfn "FWDiag-FAIL: CompilationOptionsCount=%d" opts.Length
        opts |> Array.iter (fun o -> printfn "FWDiag-FAIL:   Opt=%s" o)
        try
            let refContainer = project.GetReferenceContainer()
            let refs = refContainer.EnumReferences() |> Seq.toArray
            printfn "FWDiag-FAIL: ReferencesCount=%d" refs.Length
            for r in refs do
                let name = r.SimpleName
                // Not all mock reference objects expose path; guard
                let path =
                    try
                        let rp = r.GetType().GetProperty("FullPath")
                        if rp = null then "<no FullPath prop>"
                        else
                            match rp.GetValue(r) with
                            | :? string as s when not (String.IsNullOrWhiteSpace s) -> s
                            | _ -> "<empty>"
                    with _ -> "<err>"
                printfn "FWDiag-FAIL:   Ref Name=%s Path=%s" name path
                if name.Equals("System.Configuration", StringComparison.OrdinalIgnoreCase) then
                    // Try pull 'ResolvedPath' metadata if available
                    let rpMeta =
                        try
                            let mp = r.GetType().GetProperty("ResolvedPath")
                            if mp = null then "<no ResolvedPath prop>"
                            else
                                match mp.GetValue(r) with
                                | :? string as s when s <> "" -> s
                                | _ -> "<empty>"
                        with _ -> "<err>"
                    printfn "FWDiag-FAIL:   System.Configuration.ResolvedPath=%s" rpMeta
        with e ->
            printfn "FWDiag-FAIL: Error dumping references (%s)" e.Message

type ProjectItems() = 
    inherit TheTests()
    
    //TODO: look for a way to remove the helper functions
    static let ANYTREE = Tree("",Nil,Nil)

    [<Fact>]
    member public this.``RemoveAssemblyReference.NoIVsTrackProjectDocuments2Events``() =
        let systemAssName = "System.Configuration"

        this.MakeProjectAndDo(["file.fs"], [systemAssName],"", (fun project ->
            let listener = project.Site.GetService(typeof<Salsa.VsMocks.IVsTrackProjectDocuments2Listener>) :?> Salsa.VsMocks.IVsTrackProjectDocuments2Listener
            project.ComputeSourcesAndFlags()

            let containsSystemNumerics () =
                project.CompilationOptions
                |> Array.exists (fun f -> f.IndexOf(systemAssName, StringComparison.OrdinalIgnoreCase) >= 0)

            DebuggingHelpers.fwDiagProject project

            if not (containsSystemNumerics()) then
                DebuggingHelpers.dumpOnFailure project
                Assert.True(false, $"Project should contain reference to {systemAssName} (pre-remove)")

            let refContainer = project.GetReferenceContainer()
            let reference =
                refContainer.EnumReferences()
                |> Seq.find(fun r -> r.SimpleName = systemAssName)

            let mutable wasCalled = false
            (
                use _guard = listener.OnAfterRemoveFiles.Subscribe(fun _ -> wasCalled <- true)
                reference.Remove(false)
            )

            if containsSystemNumerics() then
                DebuggingHelpers.dumpOnFailure project
                Assert.True(false, $"Project should not contain reference to {systemAssName} (post-remove)")

            Assert.False(wasCalled, "No events from IVsTrackProjectDocuments2 are expected")
        ))

    [<Fact>]
    member public this.``AddNewItem.ItemAppearsAtBottomOfFsprojFile``() =
        this.MakeProjectAndDo(["orig.fs"], [], "", (fun project ->
            let absFilePath = Path.Combine(project.ProjectFolder, "a.fs")
            try
                File.AppendAllText(absFilePath, "#light")
                // Note: this is not the same code path as the UI, but it is close
                project.MoveNewlyAddedFileToBottomOfGroup (fun () ->
                    project.AddNewFileNodeToHierarchy(project,absFilePath) |> ignore)
                let msbuildInfo = TheTests.MsBuildCompileItems(project.BuildProject)
                AssertEqual ["orig.fs"; "a.fs"] msbuildInfo
            finally
                File.Delete(absFilePath)
            ))

    [<Fact>]
    member public this.``AddNewItem.ToAFolder.ItemAppearsAtBottomOfFolder``() =
        this.MakeProjectAndDo(["orig.fs"; "Folder\\f1.fs"; "Folder\\f2.fs"; "final.fs"], [], "", (fun project ->
            let dir = Path.Combine(project.ProjectFolder, "Folder")
            Directory.CreateDirectory(dir) |> ignore
            let absFilePath = Path.Combine(dir, "a.fs")
            try
                File.AppendAllText(absFilePath, "#light")
                // Note: this is not the same code path as the UI, but it is close
                project.MoveNewlyAddedFileToBottomOfGroup (fun () ->
                    project.AddNewFileNodeToHierarchy(project.FindChild("Folder"),absFilePath) |> ignore)
                let msbuildInfo = TheTests.MsBuildCompileItems(project.BuildProject)
                AssertEqual ["orig.fs"; "Folder\\f1.fs"; "Folder\\f2.fs"; "Folder\\a.fs"; "final.fs"] msbuildInfo
            finally
                File.Delete(absFilePath)
            ))
    
    [<Fact>]
    member public this.``AddNewItemBelow.ItemAppearsInRightSpot``() =
        this.MakeProjectAndDo(["orig1.fs"; "orig2.fs"], [], "", (fun project ->
            let absFilePath = Path.Combine(project.ProjectFolder, "new.fs")
            try
                File.AppendAllText(absFilePath, "#light")
                // Note: this is not the same code path as the UI, but it is close
                let orig1 = TheTests.FindNodeWithCaption(project, "orig1.fs")
                project.MoveNewlyAddedFileBelow (orig1, fun () ->
                    project.AddNewFileNodeToHierarchy(project,absFilePath) |> ignore)
                // ensure right in .fsproj
                let msbuildInfo = TheTests.MsBuildCompileItems(project.BuildProject)
                AssertEqual ["orig1.fs"; "new.fs"; "orig2.fs"] msbuildInfo
                // ensure right in solution explorer
                let expect = Tree("References", ANYTREE,
                             Tree("orig1.fs", Nil,
                             Tree("new.fs", Nil,
                             Tree("orig2.fs", Nil, Nil))))
                TheTests.AssertSameTree(expect, project.FirstChild)
            finally
                File.Delete(absFilePath)
            ))
    
    [<Fact>]
    member public this.``AddNewItemAbove.ItemAppearsInRightSpot.Case1``() =
        this.MakeProjectAndDo(["orig1.fs"; "orig2.fs"], [], "", (fun project ->
            let absFilePath = Path.Combine(project.ProjectFolder, "new.fs")
            try
                File.AppendAllText(absFilePath, "#light")
                // Note: this is not the same code path as the UI, but it is close
                let orig2 = TheTests.FindNodeWithCaption(project, "orig2.fs")
                project.MoveNewlyAddedFileAbove (orig2, fun () ->
                    project.AddNewFileNodeToHierarchy(project,absFilePath) |> ignore)
                // ensure right in .fsproj
                let msbuildInfo = TheTests.MsBuildCompileItems(project.BuildProject)
                AssertEqual ["orig1.fs"; "new.fs"; "orig2.fs"] msbuildInfo
                // ensure right in solution explorer
                let expect = Tree("References", ANYTREE,
                             Tree("orig1.fs", Nil,
                             Tree("new.fs", Nil,
                             Tree("orig2.fs", Nil, Nil))))
                TheTests.AssertSameTree(expect, project.FirstChild)
            finally
                File.Delete(absFilePath)
            ))
    
    [<Fact>]
    member public this.``AddNewItemAbove.ItemAppearsInRightSpot.Case2``() =
        this.MakeProjectAndDo(["orig1.fs"; "orig2.fs"], [], "", (fun project ->
            let absFilePath = Path.Combine(project.ProjectFolder, "new.fs")
            try
                File.AppendAllText(absFilePath, "#light")
                // Note: this is not the same code path as the UI, but it is close
                let orig1 = TheTests.FindNodeWithCaption(project, "orig1.fs")
                project.MoveNewlyAddedFileAbove (orig1, fun () ->
                    project.AddNewFileNodeToHierarchy(project,absFilePath) |> ignore)
                // ensure right in .fsproj
                let msbuildInfo = TheTests.MsBuildCompileItems(project.BuildProject)
                AssertEqual ["new.fs"; "orig1.fs"; "orig2.fs"] msbuildInfo
                // ensure right in solution explorer
                let expect = Tree("References", ANYTREE,
                             Tree("new.fs", Nil,
                             Tree("orig1.fs", Nil,
                             Tree("orig2.fs", Nil, Nil))))
                TheTests.AssertSameTree(expect, project.FirstChild)
            finally
                File.Delete(absFilePath)
            ))
