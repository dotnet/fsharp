// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.LanguageService.General

open Xunit
open System
open System.IO
open System.Reflection
open System.Runtime.InteropServices
open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Syntax
open FSharp.Compiler.Tokenization
open Microsoft.VisualStudio.FSharp.LanguageService
open Salsa.Salsa
open Salsa
open Salsa.VsOpsUtils
open UnitTests.TestLib.Salsa
open UnitTests.TestLib.Utils
open UnitTests.TestLib.LanguageService
open UnitTests.TestLib.ProjectSystem

module IFSharpSource_DEPRECATED = 

    [<Fact>]
    let MultipleSourceIsDirtyCallsChangeTimestamps() = 
        let recolorizeWholeFile() = ()
        let recolorizeLine (_line:int) = ()
        let isClosed() = false
        let depFileChangeNotify = 
            { new IDependencyFileChangeNotify_DEPRECATED with
                member this.DependencyFileCreated _projectSite = ()
                member this.DependencyFileChanged _filename = () }
        let source = Source.CreateSourceTestable_DEPRECATED(recolorizeWholeFile, recolorizeLine, (fun () -> "dummy.fs"), isClosed, VsMocks.VsFileChangeEx(),depFileChangeNotify)
        let originalChangeCount = source.ChangeCount
        let originalDirtyTime = source.DirtyTime

        source.RecordChangeToView()
        let secondChangeCount = source.ChangeCount
        let secondDirtyTime = source.DirtyTime
        let lastTickCount =  System.Environment.TickCount
            
        Assert.Equal(originalChangeCount + 1, secondChangeCount)
        Assert.NotEqual(secondDirtyTime, originalDirtyTime)
            
        // Here's the test. NeedsVisualRefresh is true now, we call RecordChangeToView() and it should cause a new changeCount and dirty time.
        while System.Environment.TickCount = lastTickCount do 
            System.Threading.Thread.Sleep 10 // Sleep a little to avoid grabbing the same 'Now'
        source.RecordChangeToView()
        let thirdChangeCount = source.ChangeCount
        let thirdDirtyTime = source.DirtyTime
            
        Assert.Equal(secondChangeCount + 1, thirdChangeCount)
        Assert.NotEqual(thirdDirtyTime, secondDirtyTime)            




type UsingMSBuild() =
    inherit LanguageServiceBaseTests()

    let stopWatch = new System.Diagnostics.Stopwatch()
    let ResetStopWatch() = stopWatch.Reset(); stopWatch.Start()
    let time1 op a message = 
        ResetStopWatch()
        let result = op a
        printf "%s %d ms\n" message stopWatch.ElapsedMilliseconds
        result

    let publicTypesInAsm(asmfile : string) =
        printfn "Validating assembly '%s'" asmfile
        let codeBase = (new Uri(Assembly.GetExecutingAssembly().EscapedCodeBase)).LocalPath |> Path.GetDirectoryName
        let asm = Assembly.LoadFrom(Path.Combine(codeBase, asmfile))

        // For public types that have ComVisible, validate that the constructor is internal
        asm.GetTypes()
        |> Seq.fold(fun n t ->
                        if t.IsPublic then
                            if Array.length (t.GetCustomAttributes(typeof<ComVisibleAttribute>, false)) > 0 then
                                t.GetConstructors()
                                |> Seq.fold(fun m c ->
                                                if c.IsPublic then
                                                    printfn "    Public type (ComVisible, public Constructor),%s" t.FullName
                                                    m + 1
                                                else m
                                            ) n
                            else
                                printfn "    Type: %s" t.FullName
                                n + 1
                        else
                            let CVAs = t.GetCustomAttributes(typeof<ComVisibleAttribute>, false)
                            let CVAs = CVAs |> Array.map (fun o -> o :?> ComVisibleAttribute)
                            for cva in CVAs do
                                if cva.Value then
                                    Assert.Fail(sprintf "Type %s is internal, but also ComVisible(true)" t.FullName)
                            let CIAs = t.GetCustomAttributes(typeof<ClassInterfaceAttribute>, false)
                            let CIAs = CIAs |> Array.map (fun o -> o :?> ClassInterfaceAttribute)
                            for cia in CIAs do
                                if cia.Value <> ClassInterfaceType.None then
                                    Assert.Fail(sprintf "Type %s is internal, but also ClassInterface(<something-other-than-none>)" t.FullName)
                            n
                   ) 0

    [<Fact>]
    member public this.``ReconcileErrors.Test1``() = 
        let (_solution, project, file) = this.CreateSingleFileProject(["erroneous"])
        Build project |> ignore
        TakeCoffeeBreak(this.VS)  // Error list is populated on idle
        ()
 
    /// FEATURE: (Project System only) Adding a file outside the project directory creates a link
    [<Fact>]
    member public this.``ProjectSystem.FilesOutsideProjectDirectoryBecomeLinkedFiles``() =
        use _guard = this.UsingNewVS()
        if OutOfConeFilesAreAddedAsLinks(this.VS) then
            let solution = this.CreateSolution()
            let project = CreateProject(solution,"testproject")
            let file1 = AddFileFromTextEx(project, @"..\LINK.FS", @"..\link.fs", BuildAction.Compile,
                                        [
                                         "type Bob() = "
                                         "    let x = 1"])
            let file1 = OpenFile(project, @"..\link.fs")
            Save(project)
            let projFileText = System.IO.File.ReadAllText(ProjectFile(project))
            AssertMatchesRegex '<' @"<ItemGroup>\s*<Compile Include=""..\\link.fs"">\s*<Link>link.fs</Link>" projFileText
                                  
    // This was a bug in ReplaceAllText (subsequent calls to SetMarker would fail)
    [<Fact>]
    member public this.``Salsa.ReplaceAllText``() =
        let code = 
                ["//"; 
                 "let x = \"A String Literal\""]
        let (_solution, _project, file) = this.CreateSingleFileProject(code)
        
        // Sanity check
        MoveCursorToStartOfMarker(file,"//")
        AssertEqual(TokenType.Comment, GetTokenTypeAtCursor(file))
        MoveCursorToEndOfMarker(file,"let x = ")
        AssertEqual(TokenType.String, GetTokenTypeAtCursor(file))
        
        // Replace file contents
        ReplaceFileInMemory file
                            [
                              "let x = 42 // comment!";
                              "let y = \"A String Literal\""]
        
        // Verify able to move cursor and get correct results
        MoveCursorToEndOfMarker(file, "comment")
        AssertEqual(TokenType.Comment, GetTokenTypeAtCursor(file))   // Not a string, as was originally
        MoveCursorToEndOfMarker(file, "let y = ")
        AssertEqual(TokenType.String, GetTokenTypeAtCursor(file))   // Able to find new marker
        MoveCursorToStartOfMarker(file, "let y = ")
        AssertEqual(TokenType.Keyword, GetTokenTypeAtCursor(file))  // Check MoveCursorToStartOfMarker
        
    

    // Make sure that possible overloads (and other related errors) are shown in the error list
    [<Fact>]
    member public this.``ErrorLogging.Bug5144``() =
        use _guard = this.UsingNewVS()
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        let fs1 = AddFileFromText(project,"File1.fs",
                                      ["namespace N"
                                       "module M = "
                                       "    type LineChart() ="
                                       "        member x.Plot(f : float->float, xmin:float, xmax:float) = ()"
                                       "        member x.Plot(f : System.Func<double, double>, xmin:float, xmax:float) = ()"
                                      ])
        let fs2 = AddFileFromText(project,"File2.fs",
                                      ["let p = new N.M.LineChart()"
                                       "p.Plot(sin, 0., 0.)"])
        let build = time1 Build project "Time to build project"
        
        Assert.True(not build.BuildSucceeded, "Expected build to fail")              
        
        if SupportsOutputWindowPane(this.VS) then 
            Helper.AssertListContainsInOrder(GetOutputWindowPaneLines(this.VS), 
                                      ["error FS0041: A unique overload for method 'Plot' could not be determined based on type information prior to this program point. A type annotation may be needed. Candidates: member N.M.LineChart.Plot : f:(float -> float) * xmin:float * xmax:float -> unit, member N.M.LineChart.Plot : f:System.Func<double,double> * xmin:float * xmax:float -> unit"])

// Context project system
type UsingProjectSystem() = 
    inherit UsingMSBuild(VsOpts = LanguageServiceExtension.ProjectSystemTestFlavour)

