// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace UnitTests.Tests.LanguageService

open NUnit.Framework
open System
open System.Reflection
open System.Runtime.InteropServices
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.FSharp.Compiler.SourceCodeServices
open Salsa.Salsa
open Salsa

[<Parallelizable(ParallelScope.Self)>][<TestFixture>] 
type IdealSource() = 
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public rb.MultipleSourceIsDirtyCallsChangeTimestamps() = 
        let recolorizeWholeFile() = ()
        let recolorizeLine (_line:int) = ()
        let isClosed() = false
        let source =Source.CreateDelegatingSource(recolorizeWholeFile, recolorizeLine, "dummy.fs", isClosed, VsMocks.VsFileChangeEx())
        let originalChangeCount = source.ChangeCount
        let originalDirtyTime = source.DirtyTime

        source.RecordChangeToView()
        let secondChangeCount = source.ChangeCount
        let secondDirtyTime = source.DirtyTime
        let lastTickCount =  System.Environment.TickCount
            
        Assert.AreEqual(originalChangeCount + 1, secondChangeCount)
        Assert.AreNotEqual(secondDirtyTime, originalDirtyTime)
            
        // Here's the test. NeedsVisualRefresh is true now, we call RecordChangeToView() and it should cause a new changeCount and dirty time.
        while System.Environment.TickCount = lastTickCount do 
            System.Threading.Thread.Sleep 10 // Sleep a little to avoid grabbing the same 'Now'
        source.RecordChangeToView()
        let thirdChangeCount = source.ChangeCount
        let thirdDirtyTime = source.DirtyTime
            
        Assert.AreEqual(secondChangeCount + 1, thirdChangeCount)
        Assert.AreNotEqual(thirdDirtyTime, secondDirtyTime)            




open System
open System.IO
open NUnit.Framework
open Salsa.Salsa
open Salsa.VsOpsUtils
open UnitTests.TestLib.Salsa
open UnitTests.TestLib.Utils
open Microsoft.FSharp.Compiler
open UnitTests.TestLib.LanguageService
type GeneralTests() =
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

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public this.``PendingRequests``() =
        let makeRequest (reason : BackgroundRequestReason) = new BackgroundRequest(false, Reason = reason)

        let requests = Microsoft.VisualStudio.FSharp.LanguageService.PendingRequests()
        
        let verify r = 
            let dequeued = requests.Dequeue()
            Assert.AreEqual(r, dequeued.Reason)

        // Ui1 + Ui2 = Ui2
        // should have only last
        requests.Enqueue(makeRequest BackgroundRequestReason.MemberSelect)
        requests.Enqueue(makeRequest BackgroundRequestReason.Goto)
        verify BackgroundRequestReason.Goto
        Assert.AreEqual(0, requests.Count)

        // n-Ui1 + Ui2 = Ui2
        // should have only last
        requests.Enqueue(makeRequest BackgroundRequestReason.FullTypeCheck)
        requests.Enqueue(makeRequest BackgroundRequestReason.MemberSelect)
        verify BackgroundRequestReason.MemberSelect
        Assert.AreEqual(0, requests.Count)

        // n-Ui1 + n-Ui2 = n-Ui2
        requests.Enqueue(makeRequest BackgroundRequestReason.FullTypeCheck)
        requests.Enqueue(makeRequest BackgroundRequestReason.UntypedParse)
        verify BackgroundRequestReason.UntypedParse
        Assert.AreEqual(0, requests.Count)

        // Ui1 + n-Ui2 = Ui1 + n-Ui2
        requests.Enqueue(makeRequest BackgroundRequestReason.MemberSelect)
        requests.Enqueue(makeRequest BackgroundRequestReason.UntypedParse)
        verify BackgroundRequestReason.MemberSelect
        Assert.AreEqual(1, requests.Count)
        verify BackgroundRequestReason.UntypedParse
        Assert.AreEqual(0, requests.Count)

        // (Ui1 + n-Ui2) + Ui3 = Ui3
        requests.Enqueue(makeRequest BackgroundRequestReason.MemberSelect)
        requests.Enqueue(makeRequest BackgroundRequestReason.UntypedParse)
        requests.Enqueue(makeRequest BackgroundRequestReason.MemberSelect)
        verify BackgroundRequestReason.MemberSelect
        Assert.AreEqual(0, requests.Count)

        // (Ui1 + n-Ui2) + n-Ui3 = Ui1 + n-Ui3
        requests.Enqueue(makeRequest BackgroundRequestReason.MemberSelect)
        requests.Enqueue(makeRequest BackgroundRequestReason.UntypedParse)
        requests.Enqueue(makeRequest BackgroundRequestReason.FullTypeCheck)
        verify BackgroundRequestReason.MemberSelect
        Assert.AreEqual(1, requests.Count)
        verify BackgroundRequestReason.FullTypeCheck
        Assert.AreEqual(0, requests.Count)
        

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public this.``PublicSurfaceArea.DotNetReflection``() =
        let ps = publicTypesInAsm @"fsharp.projectsystem.fsharp.dll"
        Assert.AreEqual(1, ps)  // BuildPropertyDescriptor
        let ls = publicTypesInAsm @"fsharp.languageservice.dll"
        Assert.AreEqual(0, ls)
        let comp = publicTypesInAsm @"fsharp.compiler.dll"
        Assert.AreEqual(0, comp)
        let compis = publicTypesInAsm @"FSharp.Compiler.Interactive.Settings.dll"
        Assert.AreEqual(5, compis)
        let compserver = publicTypesInAsm @"FSharp.Compiler.Server.Shared.dll"
        Assert.AreEqual(0, compserver)
        let lsbase = publicTypesInAsm @"FSharp.LanguageService.Base.dll"
        Assert.AreEqual(0, lsbase)
        let psbase = publicTypesInAsm @"FSharp.ProjectSystem.Base.dll"
        Assert.AreEqual(17, psbase)
        let fsi = publicTypesInAsm @"FSharp.VS.FSI.dll"
        Assert.AreEqual(1, fsi)

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public this.``PublicSurfaceArea.DotNetReflectionAndTypeProviders``() =
        let tp = publicTypesInAsm @"FSharp.Data.TypeProviders.dll"
        Assert.AreEqual(1, tp)  // the 'DataProviders' type that is decorated with [<TypeProvider>] must be public\
        let curDir = (new Uri(Assembly.GetExecutingAssembly().EscapedCodeBase)).LocalPath |> Path.GetDirectoryName
        let script = """
open Microsoft.FSharp.Core.CompilerServices

let cfg = new TypeProviderConfig(fun _ -> true)
cfg.IsInvalidationSupported <- false
cfg.IsHostedExecution <- false
cfg.ReferencedAssemblies <- Array.create 0 ""
cfg.ResolutionFolder <- @"c:\"
cfg.RuntimeAssembly <- ""
cfg.TemporaryFolder <- ""
 
let tp = new Microsoft.FSharp.Data.TypeProviders.DesignTime.DataProviders(cfg)
let ipn = tp :> IProvidedNamespace
let itp = tp :> ITypeProvider

let types = 
    [   "ODataService"
        "WsdlService"
        "SqlDataConnection"
        "SqlEntityConnection"
        "DbmlFile"
        "EdmxFile"
        ]

for p in types do
    printfn "%s" p
    let pType = ipn.ResolveTypeName(p)
    let odataStaticArgs = itp.GetStaticParameters(pType)
    for sa in odataStaticArgs do
        printfn "    %s:%s" sa.Name sa.ParameterType.Name
        """
        let expected = """ODataService
    ServiceUri:String
    LocalSchemaFile:String
    ForceUpdate:Boolean
    ResolutionFolder:String
    DataServiceCollection:Boolean
WsdlService
    ServiceUri:String
    LocalSchemaFile:String
    ForceUpdate:Boolean
    ResolutionFolder:String
    MessageContract:Boolean
    EnableDataBinding:Boolean
    Serializable:Boolean
    Async:Boolean
    CollectionType:String
SqlDataConnection
    ConnectionString:String
    ConnectionStringName:String
    LocalSchemaFile:String
    ForceUpdate:Boolean
    Pluralize:Boolean
    Views:Boolean
    Functions:Boolean
    ConfigFile:String
    DataDirectory:String
    ResolutionFolder:String
    StoredProcedures:Boolean
    Timeout:Int32
    ContextTypeName:String
    Serializable:Boolean
SqlEntityConnection
    ConnectionString:String
    ConnectionStringName:String
    LocalSchemaFile:String
    Provider:String
    EntityContainer:String
    ConfigFile:String
    DataDirectory:String
    ResolutionFolder:String
    ForceUpdate:Boolean
    Pluralize:Boolean
    SuppressForeignKeyProperties:Boolean
DbmlFile
    File:String
    ResolutionFolder:String
    ContextTypeName:String
    Serializable:Boolean
EdmxFile
    File:String
    ResolutionFolder:String
"""
        File.WriteAllText(Path.Combine(curDir, "tmp.fsx"), script)
        let psi = System.Diagnostics.ProcessStartInfo("fsi.exe", "-r:FSharp.Data.TypeProviders.dll tmp.fsx")
        psi.WorkingDirectory <- curDir
        psi.RedirectStandardOutput <- true
        psi.UseShellExecute <- false
        let p = System.Diagnostics.Process.Start(psi)
        let out = p.StandardOutput.ReadToEnd()
        p.WaitForExit()
        let out = out.Replace("\r\n", "\n")
        let expected = expected.Replace("\r\n", "\n")
        Assert.AreEqual(expected, out)

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public this.``ReconcileErrors.Test1``() = 
        let (_solution, project, file) = this.CreateSingleFileProject(["erroneous"])
        Build project |> ignore
        TakeCoffeeBreak(this.VS)  // Error list is populated on idle
        ()
 
    /// FEATURE: (Project System only) Adding a file outside the project directory creates a link
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    [<Category("PerfCheck")>]
    member public this.``ProjectSystem.FilesOutsideProjectDirectoryBecomeLinkedFiles``() =
        use _guard = this.UsingNewVS()
        if OutOfConeFilesAreAddedAsLinks(this.VS) then
            let solution = this.CreateSolution()
            let project = CreateProject(solution,"testproject")
            let file1 = AddFileFromTextEx(project, @"..\LINK.FS", @"..\link.fs", BuildAction.Compile,
                                        ["#light"
                                         "type Bob() = "
                                         "    let x = 1"])
            let file1 = OpenFile(project, @"..\link.fs")
            Save(project)
            let projFileText = System.IO.File.ReadAllText(ProjectFile(project))
            AssertMatchesRegex '<' @"<ItemGroup>\s*<Compile Include=""..\\link.fs"">\s*<Link>link.fs</Link>" projFileText
                                  
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    [<Category("PerfCheck")>]
    member public this.``Lexer.CommentsLexing.Bug1548``() =
        let scan = new FSharpScanner(fun source -> 
                        let filename = "test.fs"
                        let defines = [ "COMPILED"; "EDITING" ]
            
                        SourceTokenizer(defines,filename).CreateLineTokenizer(source))
        
        let cm = Microsoft.VisualStudio.FSharp.LanguageService.TokenColor.Comment
        let kw = Microsoft.VisualStudio.FSharp.LanguageService.TokenColor.Keyword
        
        // This specifies the source code to test and a collection of tokens that 
        // we want to find in the result (note: it doesn't have to contain every token, because 
        // behavior for some of them is undefined - e.g. "(* "\"*)" - what is token here?
        let sources = 
          [ "// some comment", 
                [ (0, 1), cm; (2, 2), cm; (3, 6), cm; (7, 7), cm; (8, 14), cm ]
            "// (* hello // 12345\nlet",
                [ (6, 10), cm; (15, 19), cm; (0, 2), kw  ] // checks 'hello', '12345' and keyword 'let'
            "//- test",
                [ (0, 2), cm; (4, 7), cm ] // checks whether '//-' isn't treated as an operator

            /// same thing for XML comments - these are treated in a different lexer branch
            "/// some comment", 
                [ (0, 2), cm; (3, 3), cm; (4, 7), cm; (8, 8), cm; (9, 15), cm ]
            "/// (* hello // 12345\nmember",
                [ (7, 11), cm; (16, 20), cm; (0, 5), kw  ] 
            "///- test",
                [ (0, 3), cm; (5, 8), cm ]
            
            //// same thing for "////" - these are treated in a different lexer branch
            "//// some comment", 
                [ (0, 3), cm; (4, 4), cm; (5, 8), cm; (9, 9), cm; (10, 16), cm ]
            "//// (* hello // 12345\nlet",
                [ (8, 12), cm; (17, 21), cm; (0, 2), kw  ] 
            "////- test",
                [ (0, 4), cm; (6, 9), cm ]
                
            "(* test 123 (* 456 nested *) comments *)",
                [ (3, 6), cm; (8, 10), cm; (15, 17), cm; (19, 24), cm; (29, 36), cm ] // checks 'test', '123', '456', 'nested', 'comments'
            "(* \"with 123 \\\" *)\" string *)",    
                [ (4, 7), cm; (9, 11), cm; (20, 25), cm ]  // checks 'with', '123', 'string'
            "(* @\"with 123 \"\" *)\" string *)",
                [ (5, 8), cm; (10, 12), cm; (21, 26), cm ]  // checks 'with', '123', 'string'
          ]
                        
        for lineText, expected in sources do
            scan.SetLineText lineText
            
            let currentTokenInfo = new Microsoft.VisualStudio.FSharp.LanguageService.TokenInfo()
            let lastColorState = 0 // First line of code, so no previous state
            currentTokenInfo.EndIndex <- -1
            let refState = ref (ColorStateLookup.LexStateOfColorState lastColorState)
            
            // Lex the line and add all lexed tokens to a dictionary
            let lexed = new System.Collections.Generic.Dictionary<_, _>()
            while scan.ScanTokenAndProvideInfoAboutIt(1, currentTokenInfo, refState) do
                lexed.Add( (currentTokenInfo.StartIndex, currentTokenInfo.EndIndex), currentTokenInfo.Color )
                
            // Verify that all tokens in the specified list occur in the lexed result
            for pos, clr in expected do
                let (succ, v) = lexed.TryGetValue(pos)
                let found = lexed |> Seq.map (fun kvp -> kvp.Key, kvp.Value) |> Seq.toList 
                AssertEqualWithMessage(true, succ, sprintf "Cannot find token %A at %A in %A\nFound: %A" clr pos lineText found)
                AssertEqualWithMessage(clr, v, sprintf "Wrong color of token %A at %A in %A\nFound: %A" clr pos lineText found)
           
        
    // This was a bug in ReplaceAllText (subsequent calls to SetMarker would fail)
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public this.``Salsa.ReplaceAllText``() =
        let code = 
                ["#light"; 
                 "let x = \"A String Literal\""]
        let (_solution, _project, file) = this.CreateSingleFileProject(code)
        
        // Sanity check
        MoveCursorToStartOfMarker(file,"#light")
        AssertEqual(TokenType.PreprocessorKeyword, GetTokenTypeAtCursor(file))
        MoveCursorToEndOfMarker(file,"let x = ")
        AssertEqual(TokenType.String, GetTokenTypeAtCursor(file))
        
        // Replace file contents
        ReplaceFileInMemory file
                            ["#light";
                              "let x = 42 // comment!";
                              "let y = \"A String Literal\""]
        
        // Verify able to move cursor and get correct results
        MoveCursorToEndOfMarker(file, "comment")
        AssertEqual(TokenType.Comment, GetTokenTypeAtCursor(file))   // Not a string, as was origionally
        MoveCursorToEndOfMarker(file, "let y = ")
        AssertEqual(TokenType.String, GetTokenTypeAtCursor(file))   // Able to find new marker
        MoveCursorToStartOfMarker(file, "let y = ")
        AssertEqual(TokenType.Keyword, GetTokenTypeAtCursor(file))  // Check MoveCursorToStartOfMarker
        
    

    // Make sure that possible overloads (and other related errors) are shown in the error list
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    [<Category("PerfCheck")>]
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
        
        Assert.IsTrue(not build.BuildSucceeded, "Expected build to fail")              
        
        if SupportsOutputWindowPane(this.VS) then 
            Helper.AssertListContainsInOrder(GetOutputWindowPaneLines(this.VS), 
                                      ["error FS0041: A unique overload for method 'Plot' could not be determined based on type information prior to this program point. A type annotation may be needed. Candidates: member N.M.LineChart.Plot : f:(float -> float) * xmin:float * xmax:float -> unit, member N.M.LineChart.Plot : f:System.Func<double,double> * xmin:float * xmax:float -> unit"])

    [<Category("TakesMoreThanFifteenSeconds")>]
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public this.``ExhaustivelyScrutinize.ThisOnceAsserted``() =     
        Helper.ExhaustivelyScrutinize(
          this.TestRunner,
          [ """let F() =                 """
            """    if true then [],      """
            """    elif true then [],""  """
            """    else [],""            """ ]
            )

    [<Category("TakesMoreThanFifteenSeconds")>]
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public this.``ExhaustivelyScrutinize.ThisOnceAssertedToo``() =     
        Helper.ExhaustivelyScrutinize(
            this.TestRunner,
            [ "type C() = "
              "    member this.F() = ()"
              "    interface System.IComparable with "
              "        member __.CompareTo(v:obj) = 1" ]
            )
    [<Category("TakesMoreThanFifteenSeconds")>]
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public this.``ExhaustivelyScrutinize.ThisOnceAssertedThree``() =     
        Helper.ExhaustivelyScrutinize(
            this.TestRunner,
            [ "type Foo =" 
              "    { mutable Data: string }"
              "    member x.XmlDocSig "
              "        with get() = x.Data"
              "        and set(v) = x.Data <- v" ]
              )
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public this.``ExhaustivelyScrutinize.ThisOnceAssertedFour``() =     
        Helper.ExhaustivelyScrutinize(
            this.TestRunner,
            [ "let y=new"
              "let z=4" ]
              )

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public this.``ExhaustivelyScrutinize.ThisOnceAssertedFive``() =     
        Helper.ExhaustivelyScrutinize(this.TestRunner, [ """CSV.File<@"File1.txt">.[0].""" ])  // <@ is one token, wanted < @"...

    [<Category("TakesMoreThanFifteenSeconds")>]
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public this.``ExhaustivelyScrutinize.Bug2277``() =     
        Helper.ExhaustivelyScrutinize(
            this.TestRunner,
              ["#light"
               "open Microsoft.FSharp.Plot.Excel"
               "open Microsoft.FSharp.Plot.Interactive"
               "let ps = [| (1.,\"c\"); (-2.,\"p\") |]"
               "plot (Bars(ps))"
               "let xs = [| 1.0 .. 20.0 |]"
               "let ys = [| 2.0 .. 21.0 |]"
               "let pp= plot(Area(xs,ys))" ]
                )
                                     
    [<Category("TakesMoreThanFifteenSeconds")>]
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public this.``ExhaustivelyScrutinize.Bug2283``() =     
        Helper.ExhaustivelyScrutinize(
            this.TestRunner,
              ["#light"
               "#r \"NestedClasses.dll\"" // Scenario requires this assembly not exist.
               "//753 atomType -> atomType DOT path typeArgs"
               "let specificIdent (x : RootNamespace.ClassOfT<int>.NestedClassOfU<string>) = x"
               "let x = new RootNamespace.ClassOfT<int>.NestedClassOfU<string>()"
               "if specificIdent x <> x then exit 1"
               "exit 0"] 
                )


   /// Verifies that token info returns correct trigger classes 
    /// - this is used in MPF for triggering various intellisense features
    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public this.``TokenInfo.TriggerClasses``() =      
      let important = 
        [ // Member select for dot completions
          Parser.DOT, (TokenColorKind.Operator,TokenCharKind.Delimiter,TriggerClass.MemberSelect)
          // for parameter info
          Parser.LPAREN, (TokenColorKind.Text,TokenCharKind.Delimiter, TriggerClass.ParamStart ||| TriggerClass.MatchBraces)
          Parser.COMMA,  (TokenColorKind.Text,TokenCharKind.Delimiter, TriggerClass.ParamNext)
          Parser.RPAREN, (TokenColorKind.Text,TokenCharKind.Delimiter, TriggerClass.ParamEnd ||| TriggerClass.MatchBraces) ]
      let matching =           
        [ // Other cases where we expect MatchBraces
          Parser.LQUOTE("", false); Parser.LBRACK; Parser.LBRACE; Parser.LBRACK_BAR;
          Parser.RQUOTE("", false); Parser.RBRACK; Parser.RBRACE; Parser.BAR_RBRACK ]
        |> List.map (fun n -> n, (TokenColorKind.Text,TokenCharKind.Delimiter, TriggerClass.MatchBraces))
      for tok, expected in List.concat [ important; matching ] do
        let info = TestExpose.TokenInfo tok
        AssertEqual(expected, info)

    [<Parallelizable(ParallelScope.Self)>][<Test>]
    member public this.``MatchingBraces.VerifyMatches``() = 
        let content = 
            [|
            "
                let x = (1, 2)//1
                let y =    (  3 + 1  ) * 2
                let z =
                   async {
                       return 10
                   }
                let lst = 
                    [// list_start
                        1;2;3
                    ]//list_end
                let arr = 
                    [|
                        1
                        2
                    |]
                let quote = <@(* S0 *) 1 @>(* E0 *)
                let quoteWithNestedList = <@(* S1 *) ['x';'y';'z'](* E_L*) @>(* E1 *)
                [< System.Serializable() >]
                type T = class end
            "
            |]
        let (_solution, _project, file) =  this.CreateSingleFileProject(String.concat Environment.NewLine content)

        let getPos marker = 
            // fix 1-based positions to 0-based
            MoveCursorToStartOfMarker(file, marker)
            let (row, col) = GetCursorLocation(file)
            (row - 1), (col - 1)

        let setPos row col = 
            // fix 0-based positions to 1-based
            MoveCursorTo(file, row + 1, col + 1)            

        let checkBraces startMarker endMarker expectedSpanLen = 
            let (startRow, startCol) = getPos startMarker
            let (endRow, endCol) = getPos endMarker

            let checkTextSpan (actual : TextSpan) expectedRow expectedCol = 
                Assert.IsTrue(actual.iStartLine = actual.iEndLine, "Start and end of the span should be on the same line")
                Assert.AreEqual(expectedRow, actual.iStartLine, "Unexpected row")
                Assert.AreEqual(expectedCol, actual.iStartIndex, "Unexpected column")
                Assert.IsTrue(actual.iEndIndex = (actual.iStartIndex + expectedSpanLen), sprintf "Span should have length == %d" expectedSpanLen)

            let checkBracesForPosition row col = 
                setPos row col
                let braces = GetMatchingBracesForPositionAtCursor(file)
                Assert.AreEqual(1, braces.Length, "One result expected")

                let (lbrace, rbrace) = braces.[0]
                checkTextSpan lbrace startRow startCol
                checkTextSpan rbrace endRow endCol

            checkBracesForPosition startRow startCol
            checkBracesForPosition endRow endCol           
            
        checkBraces "(1" ")//1" 1
        checkBraces "( " ") *" 1
        checkBraces "{" "}" 1
        checkBraces "[// list_start" "]//list_end" 1
        checkBraces "[|" "|]" 2
        checkBraces "<@(* S0 *)" "@>(* E0 *)" 2
        checkBraces "<@(* S1 *)" "@>(* E1 *)" 2
        checkBraces "['x'" "](* E_L*)" 1
        checkBraces "[<" ">]" 2

//Allow the TimeStampTests run under different context
namespace UnitTests.Tests.LanguageService.General
open UnitTests.Tests.LanguageService
open UnitTests.TestLib.LanguageService
open UnitTests.TestLib.ProjectSystem
open NUnit.Framework
open Salsa.Salsa

// context msbuild
[<Parallelizable(ParallelScope.Self)>][<TestFixture>]
[<Category("LanguageService.MSBuild")>]
type ``MSBuild`` = 
   inherit GeneralTests
   new() = { inherit GeneralTests(VsOpts = fst (Models.MSBuild())); }

// Context project system
[<Parallelizable(ParallelScope.Self)>][<TestFixture>]
[<Category("LanguageService.ProjectSystem")>]
type ``ProjectSystem`` = 
    inherit GeneralTests
    new() = { inherit GeneralTests(VsOpts = LanguageServiceExtension.ProjectSystem); } 