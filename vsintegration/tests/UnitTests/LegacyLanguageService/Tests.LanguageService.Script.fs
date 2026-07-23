// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.LanguageService.Script

open System
open System.IO
open System.Reflection
open Xunit
open Salsa.Salsa
open Salsa.VsOpsUtils
open UnitTests.TestLib.Salsa
open UnitTests.TestLib.Utils
open UnitTests.TestLib.LanguageService
open UnitTests.TestLib.ProjectSystem

type UsingMSBuild() as this = 
    inherit LanguageServiceBaseTests() 

    let notAA l = None,l

    let createSingleFileFsx (code : string) = 
        let (_, p, f) = this.CreateSingleFileProject(code, fileKind = SourceFileKind.FSX)
        (p, f)

    let createSingleFileFsxFromLines (code : string list) = 
        let (_, p, f) = this.CreateSingleFileProject(code, fileKind = SourceFileKind.FSX)
        (p, f)

    (* Timings ----------------------------------------------------------------------------- *)
    let stopWatch = new System.Diagnostics.Stopwatch()
    let ResetStopWatch() = stopWatch.Reset(); stopWatch.Start()
    let time1 op a message = 
        ResetStopWatch()
        let result = op a
        printf "%s %d ms\n" message stopWatch.ElapsedMilliseconds
        result

    let ShowErrors(project:OpenProject) =     
        for error in (GetErrors(project)) do
            printf "%s\n" (error.ToString())  

    let AssertListContainsInOrder(s:string list,cs:string list) =
        let s : string array = Array.ofList s
        let s : string = String.Join("\n",s)
        AssertContainsInOrder(s,cs)

    /// Assert that there is no squiggle.
    let AssertNoSquiggle(squiggleOption) = 
        match squiggleOption with 
        | None -> ()
        | Some(severity,message) ->
            Assert.Fail(sprintf "Expected no squiggle but got '%A' with message: %s" severity message)

    let VerifyErrorListContainedExpectedStr(expectedStr:string,project : OpenProject) = 
        let errorList = GetErrors(project)
        let GetErrorMessages(errorList : Error list) =
            [ for i = 0 to errorList.Length - 1 do
                yield errorList.[i].Message]
            
        Assert.True(errorList
                          |> GetErrorMessages
                          |> Seq.exists (fun errorMessage ->
                                errorMessage.Contains(expectedStr)))

    let AssertNoErrorsOrWarnings(project:OpenProject) = 
        let count = List.length (GetErrors(project))
        if count<>0 then
            printf "Saw %d errors and expected none.\n" count
            printf "Errors are: \n" 
            for e in GetErrors project do 
                printf "  path = <<<%s>>>\n" e.Path
                printf "  message = <<<%s> \n" e.Message 
            AssertEqual(0,count)

    let AssertExactlyCountErrorSeenContaining(project:OpenProject,text,expectedCount) =
        let nMatching = (GetErrors(project)) |> List.filter (fun e ->e.ToString().Contains(text)) |> List.length
        match nMatching with
        | 0 -> 
            failwith (sprintf "No errors containing \"%s\"" text)
        | x when x = expectedCount -> ()
        | _ -> 
            failwith (sprintf "Multiple errors containing \"%s\"" text)

    let AssertExactlyOneErrorSeenContaining(project:OpenProject,text) =
        AssertExactlyCountErrorSeenContaining(project,text,1)

    /// Assert that a given squiggle is an Error (or warning) containing the given text        
    let AssertSquiggleIsErrorContaining,AssertSquiggleIsWarningContaining, AssertSquiggleIsErrorNotContaining,AssertSquiggleIsWarningNotContaining =         
        let AssertSquiggle expectedSeverity nameOfExpected nameOfNotExpected assertf (squiggleOption,containing) = 
            match squiggleOption with
            | None -> Assert.Fail("Expected a squiggle but none was seen.")
            | Some(severity,message) ->
                Assert.True((severity=expectedSeverity), sprintf "Expected %s but saw %s: %s" nameOfExpected nameOfNotExpected message)
                assertf(message,containing)        
        AssertSquiggle Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error    "Error"    "Warning" AssertContains,
        AssertSquiggle Microsoft.VisualStudio.FSharp.LanguageService.Severity.Warning  "Warning"  "Error"   AssertContains,
        AssertSquiggle Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error    "Error"    "Warning" AssertNotContains,
        AssertSquiggle Microsoft.VisualStudio.FSharp.LanguageService.Severity.Warning  "Warning"  "Error"   AssertNotContains 


    //Verify the error list in fsx file contained the expected string
    member private this.VerifyFSXErrorListContainedExpectedString(fileContents : string, expectedStr : string) =
        let (_, project, file) = this.CreateSingleFileProject(fileContents, fileKind = SourceFileKind.FSX)
        VerifyErrorListContainedExpectedStr(expectedStr,project)    

    //Verify no error list in fsx file 
    member private this.VerifyFSXNoErrorList(fileContents : string) =
        let (_, project, file) = this.CreateSingleFileProject(fileContents, fileKind = SourceFileKind.FSX)
        AssertNoErrorsOrWarnings(project)  
    //Verify QuickInfo Contained In Fsx file
    member public this.AssertQuickInfoContainsAtEndOfMarkerInFsxFile (code : string) marker expected =

        let (_, _, file) = this.CreateSingleFileProject(code, fileKind = SourceFileKind.FSX)

        MoveCursorToEndOfMarker(file, marker)
        let tooltip = GetQuickInfoAtCursor file
        AssertContains(tooltip, expected)
    //Verify QuickInfo Contained In Fsx file
    member public this.AssertQuickInfoContainsAtStartOfMarkerInFsxFile (code : string) marker expected =
        let (_, _, file) = this.CreateSingleFileProject((code : string), fileKind = SourceFileKind.FSX)

        MoveCursorToStartOfMarker(file, marker)
        let tooltip = GetQuickInfoAtCursor file
        AssertContains(tooltip, expected)
    //Verify QuickInfo Not Contained In Fsx file     
    member public this.AssertQuickInfoNotContainsAtEndOfMarkerInFsxFile code marker notexpected =
        let (_, _, file) = this.CreateSingleFileProject((code : string), fileKind = SourceFileKind.FSX)

        MoveCursorToEndOfMarker(file, marker)
        let tooltip = GetQuickInfoAtCursor file
        AssertNotContains(tooltip, notexpected)

    /// FEATURE: Hovering over a resolved #r file will show a data tip with the fully qualified path to that file.
    [<Fact>]
    member public this.``Fsx.HashR_QuickInfo.ShowFilenameOfResolvedAssembly``() =
        this.AssertQuickInfoContainsAtEndOfMarkerInFsxFile
            """#r "System.Transactions" """ // Pick anything that isn't in the standard set of assemblies.
            "#r \"System.Tra" "System.Transactions.dll"

    /// FEATURE: .fsx files have INTERACTIVE #defined
    [<Fact>]
    member public this.``Fsx.INTERACTIVEIsDefinedInFsxFiles``() =
        let code =
                                    [
                                     "#if INTERACTIVE"
                                     "let xyz = 1"
                                     "#endif"
                                    ]
        let (_, file) = createSingleFileFsxFromLines code
        MoveCursorToEndOfMarker(file,"let xy")
        AssertEqual(TokenType.Identifier ,GetTokenTypeAtCursor(file))  

    // Ensure that basic compile of an .fsx works        
    [<Fact>]
    member public this.``Fsx.CompileFsx_1``() =
        use _guard = this.UsingNewVS()
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        
        let file1 = AddFileFromTextEx(project,"Script.fsx","Script.fsx",BuildAction.Compile,
                                      ["printfn \"Hello world\""])
        let build = time1 Build project "Time to build project"
        Assert.True(build.BuildSucceeded, "Expected build to succeed")
        ShowErrors(project)
        

    // Compile a script which #loads a source file. The build can't succeed without the loaded file.      
    [<Fact>]
    member public this.``Fsx.CompileFsx_2``() =
        use _guard = this.UsingNewVS()
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        let fs = AddFileFromTextEx(project,"File.fs","File.fs",BuildAction.Compile,
                                      ["namespace Namespace"
                                       "module Module ="
                                       "  let Value = 1"
                                      ])
        let fsx = AddFileFromTextEx(project,"Script.fsx","Script.fsx",BuildAction.Compile,
                                      ["#load \"File.fs\""
                                       "printfn \"%d\" Namespace.Module.Value"])
        let build = time1 Build project "Time to build project"
        if SupportsOutputWindowPane(this.VS) then 
            let lines = GetOutputWindowPaneLines(this.VS)
            for line in lines do printfn "%s" line
            ()
        Assert.True(build.BuildSucceeded, "Expected build to succeed")
        
    // Compile a script which #loads a source file. The build can't succeed without 
    [<Fact>]
    member public this.``Fsx.CompileFsx_3``() =
        use _guard = this.UsingNewVS()
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        let fs = AddFileFromTextEx(project,"File.fs","File.fs",BuildAction.None,
                                      ["namespace Namespace"
                                       "module Module ="
                                       "  let Value = 1"
                                      ])
        let fsx = AddFileFromTextEx(project,"Script.fsx","Script.fsx",BuildAction.Compile,
                                      ["#load \"File.fs\""
                                       "printfn \"%d\" Namespace.Module.Value"])
        let build = time1 Build project "Time to build project" 
        if SupportsOutputWindowPane(this.VS) then 
            let lines = GetOutputWindowPaneLines(this.VS)
            for line in lines do printfn "%s" line
            ()
        Assert.True(build.BuildSucceeded, "Expected build to succeed")        
        
    // Must be explicitly referenced by compile.
    [<Fact>]
    member public this.``Fsx.CompileFsx_Bug5416_1``() =
        use _guard = this.UsingNewVS()
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        let fsx = AddFileFromTextEx(project,"Script.fsx","Script.fsx",BuildAction.Compile,
                                      ["let x = fsi.CommandLineArgs"])
        let build = time1 Build project "Time to build project" 
        if SupportsOutputWindowPane(this.VS) then 
            let lines = GetOutputWindowPaneLines(this.VS)
            for line in lines do printfn "%s" line
            ()
        Assert.True(not(build.BuildSucceeded), "Expected build to fail")    
        
    // Must be explicitly referenced by compile.
    [<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
    member public this.``Fsx.CompileFsx_Bug5416_2``() =
        use _guard = this.UsingNewVS()
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        let binariesFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        PlaceIntoProjectFileBeforeImport
            (project, sprintf @"
                <ItemGroup>
                    <!-- Subtle: You need this reference to compile but not to get language service -->
                    <Reference Include=""FSharp.Compiler.Interactive.Settings"">
                        <HintPath>%s\\FSharp.Compiler.Interactive.Settings.dll</HintPath>
                    </Reference>
                    <Reference Include=""FSharp.Compiler.Service"">
                        <HintPath>%s\\FSharp.Compiler.Service.dll</HintPath>
                    </Reference>
                </ItemGroup>" binariesFolder binariesFolder)

        let fsx = AddFileFromTextEx(project,"Script.fsx","Script.fsx",BuildAction.Compile,
                                      ["let x = fsi.CommandLineArgs"])
        let build = time1 Build project "Time to build project" 
        if SupportsOutputWindowPane(this.VS) then 
            let lines = GetOutputWindowPaneLines(this.VS)
            for line in lines do printfn "%s" line
            ()
        if not(SupportsOutputWindowPane(this.VS)) then  
            Assert.True(build.BuildSucceeded, "Expected build to succeed")                
        
        
    // Ensure that #load order is preserved when #loading multiple files. 
    [<Fact>]
    member public this.``Fsx.CompileFsx_5``() =
        use _guard = this.UsingNewVS()
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        let fs1 = AddFileFromTextEx(project,"File1.fs","File1.fs",BuildAction.None,
                                      ["namespace Namespace"
                                       "module Module1 ="
                                       "  let Value = 1"
                                      ])
        let fs2 = AddFileFromTextEx(project,"File2.fs","File2.fs",BuildAction.None,
                                      ["namespace Namespace"
                                       "module Module2 ="
                                       "  let Value = Module1.Value"
                                      ])                                      
        let fsx = AddFileFromTextEx(project,"Script.fsx","Script.fsx",BuildAction.Compile,
                                      [
                                       "#load \"File1.fs\""
                                       "#load \"File2.fs\""
                                       "printfn \"%d\" Namespace.Module2.Value"])
        let build = time1 Build project "Time to build project" 
        if SupportsOutputWindowPane(this.VS) then 
            let lines = GetOutputWindowPaneLines(this.VS)
            for line in lines do printfn "%s" line
            ()
        Assert.True(build.BuildSucceeded, "Expected build to succeed")          
        
    // If an fs file is explicitly passed in to the compiler and also #loaded then 
    // the command-line order is respected rather than the #load order
    [<Fact>]
    member public this.``Fsx.CompileFsx_6``() =
        use _guard = this.UsingNewVS()
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        let fs1 = AddFileFromTextEx(project,"File1.fs","File1.fs",BuildAction.Compile,
                                      ["namespace Namespace"
                                       "module Module1 ="
                                       "  let Value = 1"
                                      ])
        let fs2 = AddFileFromTextEx(project,"File2.fs","File2.fs",BuildAction.Compile,
                                      ["namespace Namespace"
                                       "module Module2 ="
                                       "  let Value = Module1.Value"
                                      ])                                      
        let fsx = AddFileFromTextEx(project,"Script.fsx","Script.fsx",BuildAction.Compile,
                                      [
                                       "#load \"File2.fs\"" // Wrong order
                                       "#load \"File1.fs\""
                                       "printfn \"%d\" Namespace.Module2.Value"])
        let build = time1 Build project "Time to build project" 
        if SupportsOutputWindowPane(this.VS) then 
            let lines = GetOutputWindowPaneLines(this.VS)
            for line in lines do printfn "%s" line
            ()
        Assert.True(build.BuildSucceeded, "Expected build to succeed") 

        
        
    // If a #loaded file does not exist, there should be an error
    [<Fact>]
    member public this.``Fsx.CompileFsx_7``() =
        use _guard = this.UsingNewVS()
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        let fsx = AddFileFromTextEx(project,"Script.fsx","Script.fsx",BuildAction.Compile,
                                      [
                                       "#load \"NonexistentFile.fs\""
                                       ])
        let build = time1 Build project "Time to build project" 
        if SupportsOutputWindowPane(this.VS) then 
            let lines = GetOutputWindowPaneLines(this.VS)
            for line in lines do printfn "%s" line
            AssertListContainsInOrder(lines, ["error FS0079: Could not load file"; "NonexistentFile.fs"; "because it does not exist or is inaccessible"])            
            
        Assert.True(not(build.BuildSucceeded), "Expected build to fail")       
        
        
    // #r references should be respected.
    [<Fact>]
    member public this.``Fsx.CompileFsx_8``() =
        use _guard = this.UsingNewVS()
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        let fsx = AddFileFromTextEx(project,"Script.fsx","Script.fsx",BuildAction.Compile,
                                      [
                                       "#r \"System.Messaging\""
                                       "let a = new System.Messaging.AccessControlEntry()"
                                       ])
        let build = time1 Build project "Time to build project" 
        if SupportsOutputWindowPane(this.VS) then 
            let lines = GetOutputWindowPaneLines(this.VS)
            for line in lines do printfn "%s" line
            
        Assert.True(build.BuildSucceeded, "Expected build to succeed")          
        
        
    // Missing script file should be a reasonable failure, not a callstack.
    [<Fact>]
    member public this.``Fsx.CompileFsx_Bug5414``() =
        use _guard = this.UsingNewVS()
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        let fsx = AddFileFromTextEx(project,"Script.fsx","Script.fsx",BuildAction.Compile,[])
        DeleteFileFromDisk(this.VS, fsx) 
        
        let build = Build project
        if SupportsOutputWindowPane(this.VS) then 
            let lines = GetOutputWindowPaneLines(this.VS)
            AssertListContainsInOrder(lines, 
                                      ["Could not find file "
                                       "Script.fsx"])           
            for line in lines do 
                printfn "%s" line
                AssertNotContains(line,"error MSB") // Microsoft.FSharp.Targets(135,9): error MSB6006: "fsc.exe" exited with code -532462766.

        Assert.True(not(build.BuildSucceeded), "Expected build to fail")                                  
        
        
    member public this.TypeProviderDisposalSmokeTest(clearing) =
        use _guard = this.UsingNewVS()
        let providerAssemblyName = PathRelativeToTestAssembly(@"DummyProviderForLanguageServiceTesting.dll")
        let providerAssembly = System.Reflection.Assembly.LoadFrom providerAssemblyName
        Assert.NotNull(providerAssembly)
        let providerCounters = providerAssembly.GetType("DummyProviderForLanguageServiceTesting.GlobalCounters")
        Assert.NotNull(providerCounters)
        let totalCreationsMeth = providerCounters.GetMethod("GetTotalCreations")
        Assert.NotNull(totalCreationsMeth)
        let totalDisposalsMeth = providerCounters.GetMethod("GetTotalDisposals")
        Assert.NotNull(totalDisposalsMeth)
        let checkConfigsMeth = providerCounters.GetMethod("CheckAllConfigsDisposed")
        Assert.NotNull(checkConfigsMeth)

        let providerCounters2 = providerAssembly.GetType("ProviderImplementation.ProvidedTypes.GlobalCountersForInvalidation")
        Assert.NotNull(providerCounters2)
        let totalInvalidationHandlersAddedMeth = providerCounters2.GetMethod("GetInvalidationHandlersAdded")
        Assert.NotNull(totalInvalidationHandlersAddedMeth)
        let totalInvalidationHandlersRemovedMeth = providerCounters2.GetMethod("GetInvalidationHandlersRemoved")
        Assert.NotNull(totalInvalidationHandlersRemovedMeth)

        let totalCreations() = totalCreationsMeth.Invoke(null, [| |]) :?> int
        let totalDisposals() = totalDisposalsMeth.Invoke(null, [| |]) :?> int
        let checkConfigsDisposed() = checkConfigsMeth.Invoke(null, [| |]) |> ignore
        let totalInvalidationHandlersAdded() = totalInvalidationHandlersAddedMeth.Invoke(null, [| |]) :?> int
        let totalInvalidationHandlersRemoved() = totalInvalidationHandlersRemovedMeth.Invoke(null, [| |]) :?> int

         
        let startCreations = totalCreations()
        let startDisposals = totalDisposals()
        let startInvalidationHandlersAdded = totalInvalidationHandlersAdded()
        let startInvalidationHandlersRemoved =  totalInvalidationHandlersRemoved()
        let countCreations() = totalCreations() - startCreations
        let countDisposals() = totalDisposals() - startDisposals
        let countInvalidationHandlersAdded() = totalInvalidationHandlersAdded() - startInvalidationHandlersAdded
        let countInvalidationHandlersRemoved() = totalInvalidationHandlersRemoved() - startInvalidationHandlersRemoved

        Assert.True(startCreations >= startDisposals, "Check0")
        Assert.True(startInvalidationHandlersAdded >= startInvalidationHandlersRemoved, "Check0")
        for i in 1 .. 50 do 
            let solution = this.CreateSolution()
            let project = CreateProject(solution,"testproject" + string (i % 20))    
            this.AddAssemblyReference(project, PathRelativeToTestAssembly(@"DummyProviderForLanguageServiceTesting.dll"))
            let fileName = sprintf "File%d.fs" i
            let file1 = AddFileFromText(project,fileName, ["let x" + string i + " = N1.T1()" ])    
            let file = OpenFile(project,fileName)
            TakeCoffeeBreak(this.VS)
            AssertNoErrorsOrWarnings project   // ...and not an error on the first line.
            MoveCursorToEndOfMarker(file, "N1.T1")

            // do some stuff to get declarations etc.
            let tooltip = GetQuickInfoAtCursor file
            AssertContains(tooltip, "T1")
            ignore (GetF1KeywordAtCursor file)
            let parmInfo = GetParameterInfoAtCursor file

            let file1 = OpenFile(project,fileName)   

            // The disposals should be at least one less 
            let c = countCreations()
            let d = countDisposals()

            // Creations should always be greater or equal to disposals
            Assert.True(c >= d, "Check2, countCreations() >= countDisposals(), iteration " + string i + ", countCreations() = " + string c + ", countDisposals() = " + string d)

            // Creations can run ahead of iterations if the background checker resurrects the builder for a project
            // even after we've moved on from it.
            Assert.True((c >= i), "Check3, countCreations() >= i, iteration " + string i + ", countCreations() = " + string c)

            if not clearing then 
                // By default we hold 3 build incrementalBuilderCache entries and 5 typeCheckInfo entries, so if we're not clearing
                // there should be some roots to project builds still present
                if i >= 3 then 
                    Assert.True(c >= d + 3, "Check4a, c >= countDisposals() + 3, iteration " + string i + ", i = " + string i + ", countDisposals() = " + string d)
                    printfn "Check4a2, i = %d, countInvalidationHandlersRemoved() = %d" i (countInvalidationHandlersRemoved())

            // If we forcefully clear out caches and force a collection, then we can say much stronger things...
            if clearing then 
                ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients(this.VS)
                let c = countCreations()
                let d = countDisposals()

                // Creations should be equal to disposals after a `ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients`
                Assert.True((c = d), "Check4b, countCreations() = countDisposals(), iteration " + string i)
                Assert.True((countInvalidationHandlersAdded() = countInvalidationHandlersRemoved()), "Check4b2, all invalidation handlers removed, iteration " + string i)
        
        let c = countCreations()
        let d = countDisposals()
        Assert.True(c >= 50, "Check5, at end, countCreations() >= 50")

        ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients(this.VS)

        let c = countCreations()
        let d = countDisposals()
        // Creations should be equal to disposals after a `ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients`
        Assert.True((c = d), "Check6b, at end, countCreations() = countDisposals() after explicit clearing")
        Assert.True((countInvalidationHandlersAdded() = countInvalidationHandlersRemoved()), "Check6b2, at end, all invalidation handlers removed after explicit clearing")
        checkConfigsDisposed()

    [<Fact(Skip = "Flaky test, unclear if it is valuable")>]
    member public this.``TypeProvider.Disposal.SmokeTest1``() = this.TypeProviderDisposalSmokeTest(true)

    [<Fact(Skip ="Flaky test, unclear if it is valuable")>]
    member public this.``TypeProvider.Disposal.SmokeTest2``() = this.TypeProviderDisposalSmokeTest(false)


// Context project system
type UsingProjectSystem() = 
    inherit UsingMSBuild(VsOpts = LanguageServiceExtension.ProjectSystemTestFlavour)

