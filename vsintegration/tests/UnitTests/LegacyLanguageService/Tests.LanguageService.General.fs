// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.LanguageService.General

open NUnit.Framework
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

[<TestFixture>][<Category "LanguageService">] 
module IFSharpSource_DEPRECATED = 

    [<Test>]
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




[<TestFixture>][<Category "LanguageService">]  
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

    [<Test>]
    member public this.``ReconcileErrors.Test1``() = 
        let (_solution, project, file) = this.CreateSingleFileProject(["erroneous"])
        Build project |> ignore
        TakeCoffeeBreak(this.VS)  // Error list is populated on idle
        ()
 
    /// FEATURE: (Project System only) Adding a file outside the project directory creates a link
    [<Test>]
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
                                  
    [<Test>]
    member public this.``Lexer.CommentsLexing.Bug1548``() =
        let scan = new FSharpScanner_DEPRECATED(fun source -> 
                        let filename = "test.fs"
                        let defines = [ "COMPILED"; "EDITING" ]
            
                        FSharpSourceTokenizer(defines,Some(filename)).CreateLineTokenizer(source))
        
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
            let refState = ref (ColorStateLookup_DEPRECATED.LexStateOfColorState lastColorState)
            
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
    [<Test>]
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
    [<Test>]
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

    [<Test; Category("Expensive")>]
    member public this.``ExhaustivelyScrutinize.ThisOnceAsserted``() =     
        Helper.ExhaustivelyScrutinize(
          this.TestRunner,
          [ """let F() =                 """
            """    if true then [],      """
            """    elif true then [],""  """
            """    else [],""            """ ]
            )

    [<Test; Category("Expensive")>]
    member public this.``ExhaustivelyScrutinize.ThisOnceAssertedToo``() =     
        Helper.ExhaustivelyScrutinize(
            this.TestRunner,
            [ "type C() = "
              "    member this.F() = ()"
              "    interface System.IComparable with "
              "        member _.CompareTo(v:obj) = 1" ]
            )

    [<Test; Category("Expensive")>]
    member public this.``ExhaustivelyScrutinize.ThisOnceAssertedThree``() =     
        Helper.ExhaustivelyScrutinize(
            this.TestRunner,
            [ "type Foo =" 
              "    { mutable Data: string }"
              "    member x.XmlDocSig "
              "        with get() = x.Data"
              "        and set(v) = x.Data <- v" ]
              )
    [<Test>]
    member public this.``ExhaustivelyScrutinize.ThisOnceAssertedFour``() =     
        Helper.ExhaustivelyScrutinize(
            this.TestRunner,
            [ "let y=new"
              "let z=4" ]
              )

    [<Test>]
    member public this.``ExhaustivelyScrutinize.ThisOnceAssertedFive``() =     
        Helper.ExhaustivelyScrutinize(this.TestRunner, [ """CSV.File<@"File1.txt">.[0].""" ])  // <@ is one token, wanted < @"...

    [<Category("Expensive")>]
    [<Test>]
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
                                     
    [<Category("Expensive")>]
    [<Test>]
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
    [<Test>]
    member public this.``TokenInfo.TriggerClasses``() =      
      let important = 
        [ // Member select for dot completions
          Parser.DOT, (FSharpTokenColorKind.Punctuation,FSharpTokenCharKind.Delimiter,FSharpTokenTriggerClass.MemberSelect)
          // for parameter info
          Parser.LPAREN, (FSharpTokenColorKind.Punctuation,FSharpTokenCharKind.Delimiter, FSharpTokenTriggerClass.ParamStart ||| FSharpTokenTriggerClass.MatchBraces)
          Parser.COMMA,  (FSharpTokenColorKind.Punctuation,FSharpTokenCharKind.Delimiter, FSharpTokenTriggerClass.ParamNext)
          Parser.RPAREN, (FSharpTokenColorKind.Punctuation,FSharpTokenCharKind.Delimiter, FSharpTokenTriggerClass.ParamEnd ||| FSharpTokenTriggerClass.MatchBraces) ]
      let matching =           
        [ // Other cases where we expect MatchBraces
          Parser.LQUOTE("", false); Parser.LBRACK; Parser.LBRACE (Unchecked.defaultof<_>); Parser.LBRACK_BAR;
          Parser.RQUOTE("", false); Parser.RBRACK; Parser.RBRACE (Unchecked.defaultof<_>); Parser.BAR_RBRACK ]
        |> List.map (fun n -> n, (FSharpTokenColorKind.Punctuation,FSharpTokenCharKind.Delimiter, FSharpTokenTriggerClass.MatchBraces))
      for tok, expected in List.concat [ important; matching ] do
        let info = TestExpose.TokenInfo tok
        AssertEqual(expected, info)

    [<Test>]
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


// Context project system
[<TestFixture>]
type UsingProjectSystem() = 
    inherit UsingMSBuild(VsOpts = LanguageServiceExtension.ProjectSystemTestFlavour)

