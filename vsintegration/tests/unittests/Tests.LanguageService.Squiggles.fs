// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Tests.LanguageService.Squiggles

open System
open System.IO
open NUnit.Framework
open Salsa.Salsa
open Salsa.VsOpsUtils
open UnitTests.TestLib.Salsa
open UnitTests.TestLib.Utils
open UnitTests.TestLib.LanguageService
open UnitTests.TestLib.ProjectSystem

[<TestFixture>] 
type UsingMSBuild() as this= 
    inherit LanguageServiceBaseTests()

    #if FX_ATLEAST_45
    let AA l = Some(System.IO.Path.Combine(System.IO.Path.GetTempPath(), ".NETFramework,Version=v4.0.AssemblyAttributes.fs")), l
    let notAA l = None,l
    #else
    let AA l = None, l
    let notAA l = None,l
    #endif

    let CheckSquiggles (fileContents : string) markers f = 
        let (sln, proj, file) = this.CreateSingleFileProject(fileContents)        
        TakeCoffeeBreak(this.VS)// Wait for the background compiler to catch up.

        for marker in markers do
            MoveCursorToStartOfMarker(file, marker)
            let squiggles = GetSquigglesAtCursor(file)
            f marker squiggles


    /// Assert that there is no squiggle.
    let AssertNoSquiggle(squiggleOption) = 
        match squiggleOption with 
        | None -> ()
        | Some(severity,message) ->
            Assert.Fail(sprintf "Expected no squiggle but got '%A' with message: %s" severity message)

    /// Assert that a given squiggle is an Error (or warning) containing the given text        
    let AssertSquiggleIsErrorContaining,AssertSquiggleIsWarningContaining, AssertSquiggleIsErrorNotContaining,AssertSquiggleIsWarningNotContaining =         
        let AssertSquiggle expectedSeverity nameOfExpected nameOfNotExpected assertf (squiggleOption,containing) = 
            match squiggleOption with
            | None -> Assert.Fail("Expected a squiggle but none was seen.")
            | Some(severity,message) ->
                Assert.IsTrue((severity=expectedSeverity), sprintf "Expected %s but saw %s: %s" nameOfExpected nameOfNotExpected message)
                assertf(message,containing)        
        AssertSquiggle Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error    "Error"    "Warning" AssertContains,
        AssertSquiggle Microsoft.VisualStudio.FSharp.LanguageService.Severity.Warning  "Warning"  "Error"   AssertContains,
        AssertSquiggle Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error    "Error"    "Warning" AssertNotContains,
        AssertSquiggle Microsoft.VisualStudio.FSharp.LanguageService.Severity.Warning  "Warning"  "Error"   AssertNotContains 

    member private this.VerifyNoSquiggleAtStartOfMarker(fileContents : string, marker : string, ?addtlRefAssy : list<string>) = 
        let (sln, proj, file) = this.CreateSingleFileProject(fileContents, ?references=addtlRefAssy)

        MoveCursorToStartOfMarker(file, marker)        
        TakeCoffeeBreak(this.VS)// Wait for the background compiler to catch up.
        let squiggle = GetSquiggleAtCursor(file)
        AssertEqual(None,squiggle)

    member private this.VerifySquiggleAtStartOfMarker(fileContents : string, marker : string, expectedSquiggle : (Microsoft.VisualStudio.FSharp.LanguageService.Severity * string), ?addtlRefAssy : list<string>, ?thereShouldBeNoOtherSquigglesHere : bool) = 
        let (sln, proj, file) = this.CreateSingleFileProject(fileContents, ?references=addtlRefAssy)
        
        TakeCoffeeBreak(this.VS)// Wait for the background compiler to catch up.

        MoveCursorToStartOfMarker(file, marker)
        let squiggles = GetSquigglesAtCursor(file)
        if squiggles |> List.exists ((=) expectedSquiggle) then
            ()  // ok, found it
            match thereShouldBeNoOtherSquigglesHere with
            | Some(true) ->
                if squiggles.Length <> 1 then
                    Assert.Fail(sprintf "Multiple squiggles when only one expected; expected only\r\n%A\r\ngot\r\n%A" expectedSquiggle squiggles)
            | _ -> ()
        else
            Assert.Fail(sprintf "Expected %A but got %A" expectedSquiggle squiggles)
  
    member private this.VerifySquiggleContainedAtStartOfMarker(fileContents : string, marker : string, expectedSquiggle : (Microsoft.VisualStudio.FSharp.LanguageService.Severity * string)) = 
        let (sln, proj, file) = this.CreateSingleFileProject(fileContents)
        MoveCursorToStartOfMarker(file, marker)
        let squiggles = GetSquigglesAtCursor(file)
        if squiggles |> List.exists (fun (sev,msg) -> fst expectedSquiggle = sev && msg.Contains(snd expectedSquiggle)) then
            ()  // ok, found it
        else
            Assert.Fail(sprintf "Expected %A but got %A" expectedSquiggle squiggles)

    [<Test>]
    member public this.``Error.Expression.IllegalIntegerLiteral``() = 
        this.VerifySquiggleAtStartOfMarker(
            fileContents = """
                let _ = 1
                let a = 0.1.(*MError1*)0
                """,
            marker = "(*MError1*)",
            expectedSquiggle = (Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error,
                                 "Missing qualification after '.'")) 
                               
                     
    [<Test>]
    member public this.``Error.Expression.IncompleteDefine``() = 
        this.VerifySquiggleAtStartOfMarker(
            fileContents = """
                let a = ;(*MError3*)""",
            marker = "(*MError3*)",
            expectedSquiggle= (Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error,
                              "Unexpected symbol ';' in binding")) 

    [<Test>]
    member public this.``Error.Expression.KeywordAsValue``() = 
        this.VerifySquiggleAtStartOfMarker(
            fileContents = """
                let b =
                type(*MError4*) 
                """,
            marker = "(*MError4*)",
            expectedSquiggle= (Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error,
                              "Incomplete structured construct at or before this point in binding"))
                              
    [<Test>]
    member public this.``Error.Type.WithoutName``() = 
        this.VerifySquiggleAtStartOfMarker(
            fileContents = """
                type =(*MError5*)
                ;;""",
            marker = "(*MError5*)",
            expectedSquiggle= (Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error,
                              "Unexpected symbol '=' in type name"))         
    
    [<Test>]                          
    member public this.``AbstractClasses.Constructors.PositiveTests``() = 

        let testCases = 
            [
            """
[<AbstractClass>]
type C(a : int) = 
    new(a : string) = C(int a)
    new(b) = match b with Some _ -> C(1) | _ -> C("")

            """, ["C(1)"; "C(\"\")"; "C(int a)"]

            """
[<AbstractClass>]
type C(a : int) = 
    new(a : string) = new C(int a)
    new(b) = match b with Some _ -> new C(1) | _ -> new C("")

            """, ["C(1)"; "C(\"\")"; "C(int a)"]


            """
[<AbstractClass>]
type O(o : int) = 
    new() = O(1)
            """, ["O(1)"]

            """
[<AbstractClass>]
type O(o : int) = 
    new() = new O(1)
            """, ["O(1)"]


            """
[<AbstractClass>]
type O(o : int) = 
    new() = O() then printfn "A"
            """, ["O()"]

            """
[<AbstractClass>]
type O(o : int) = 
    new() = new O(1) then printfn "A"
            """, ["O(1)"]


            """
[<AbstractClass>]
type D() = class end
[<AbstractClass>]
type E = 
    inherit D
    new() = { inherit D(); }
            """, ["D();"]

            ]

        for (source, markers) in testCases do
            printfn "AbstractClasses.Constructors.PositiveTests: testing %s, markers: %A" source markers
            CheckSquiggles source markers <|
                fun _ -> 
                    function
                    | [] -> () // OK : no squiggles expected
                    | errs -> sprintf "Unexpected squiggles %A" errs |> Assert.Fail

    [<Test>]                          
    member public this.``AbstractClasses.Constructors.NegativeTests``() = 
        let testCases = 
            [
            """
[<AbstractClass>]
type D = 
    val d : D
    new() = {d = D()}
            """, ["D()", true]

            """
[<AbstractClass>]
type D = 
    val d : D
    new() = {d = new D()}
            """, ["D()", true]

            """
[<AbstractClass>]
type Z() = 
    new(a : int) = 
        Z(10) then ignore(Z())
            """, ["Z())", true]

            """
[<AbstractClass>]
type Z() = 
    new(a : int) = 
        new Z(10) then ignore(new Z())
            """, ["Z())", true]

            """
[<AbstractClass>]
type X() = 
    member val V : bool = true
    new(_ : bool) = 
            if X().V 
            then X(true) 
            else X()//1
            then ignore(X())
            """, ["X().V", true; "X())", true; "X()//1", false; "X(true) ", false]

            """
[<AbstractClass>]
type X() = 
    member val V : bool = true
    new(_ : bool) = 
            if (new X()).V 
            then new X(true) 
            else new X()//1 
            then ignore(new X())
            """, ["X()).V", true; "X())", true; "new X(true)", false; "X()//1", false ]

            """
[<AbstractClass>]
type X() = 
    member val V : bool = true
    new(_ : bool) = 
        let _ = 
            if X().V 
            then X(true) 
            else X()//M
        X()//1
            then ignore(X())
            """, ["X().V", true; "X(true) ", true; "X()//M", true; "X())", true; "X()//1", false]


            """
[<AbstractClass>]
type X() = 
    member val V : bool = true
    new(_ : bool) = 
        let _ = 
            if (new X()).V 
            then new X(true) 
            else new X()//M
        new X() //2
            then ignore(new X())
            """, ["X()).V", true; "X(true)", true; "X()//M", true; "X())", true; "new X() //2", false]
            ]
        for (source, markers) in testCases do
            printfn "AbstractClasses.Constructors.NegativeTests: testing %s, markers: %A" source markers
            let map = dict markers
            let markers = List.map fst markers
            CheckSquiggles source markers <|
                fun marker -> 
                    function
                    | [] -> if map.[marker] then Assert.Fail("Squiggles expected") else ()
                    | errs ->
                        if not (map.[marker]) then Assert.Fail("Squiggles not expected")
                        else
                        for (sev, text) in errs do
                            printfn "Actual squiggles at %s: Severity %A, text %s" marker sev text
                            Assert.AreEqual(Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error, sev, "Error severity expected")
                            Assert.AreEqual("Instances of this type cannot be created since it has been marked abstract or not all methods have been given implementations. Consider using an object expression '{ new ... with ... }' instead.", text, "Unexpected error text")



    [<Test>]
    [<Category("TypeProvider")>]
    member this.``TypeProvider.Error.VerbatimStringAccident.GoodErrorMessage``() = 
        let r = [PathRelativeToTestAssembly(@"UnitTestsResources\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")]
        let (sln, proj, file) = this.CreateSingleFileProject("""type foo = N1.T<@"foo">""", references = r)
        TakeCoffeeBreak(this.VS)// Wait for the background compiler to catch up.
        MoveCursorToStartOfMarker(file, "1")
        AssertEqual( [], GetSquigglesAtCursor(file) )  // no error messages here
        MoveCursorToStartOfMarker(file, "T")
        AssertEqual( [], GetSquigglesAtCursor(file) )  // no error messages here either
        MoveCursorToStartOfMarker(file, "<@")
        let squiggles = GetSquigglesAtCursor(file)
        let expected = Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error,
                          """Unexpected quotation operator '<@' in type definition. If you intend to pass a verbatim string as a static argument to a type provider, put a space between the '<' and '@' characters."""
        AssertEqual( [expected], squiggles )

    [<Test>]
    [<Category("TypeProvider")>]
    member public this.``TypeProvider.WarningAboutEmptyAssembly`` () =
        let emptyLoc = PathRelativeToTestAssembly(@"UnitTestsResources\MockTypeProviders\EmptyAssembly.dll")
        this.VerifySquiggleAtStartOfMarker(
            fileContents = "type foo = N1.T<\"foo\"",
            marker = "t",
            expectedSquiggle= (Microsoft.VisualStudio.FSharp.LanguageService.Severity.Warning,
                                "Referenced assembly '"+emptyLoc+"' has assembly level attribute 'Microsoft.FSharp.Core.CompilerServices.TypeProviderAssemblyAttribute' but no public type provider classes were found"),
            // ensure that if you referenced two TP assemblies, one of which contained TPs, and the other did not, then you get the warning about a TP assembly with no TPs
            addtlRefAssy = [emptyLoc; PathRelativeToTestAssembly(@"UnitTestsResources\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")],
            thereShouldBeNoOtherSquigglesHere=true)      

    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    member public this.``TypeProvider.Error.CodePrefix1.GoodErrorMessage`` () =
        this.VerifySquiggleAtStartOfMarker(
            fileContents = "type foo = N1.T<",
            marker = "N1",
            expectedSquiggle= (Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error,
                              "The static parameter 'Param1' of the provided type or method 'T' requires a value. Static parameters to type providers may be optionally specified using named arguments, e.g. 'T<Param1=...>'."),
            addtlRefAssy = [PathRelativeToTestAssembly(@"UnitTestsResources\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])      

    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    member public this.``TypeProvider.Error.CodePrefix2.GoodErrorMessage`` () =
        this.VerifySquiggleAtStartOfMarker(
            fileContents = "type foo = N1.T<\"foo\",",
            marker = "N1",
            expectedSquiggle= (Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error,
                              "The static parameter 'ParamIgnored' of the provided type or method 'T' requires a value. Static parameters to type providers may be optionally specified using named arguments, e.g. 'T<ParamIgnored=...>'."),
            addtlRefAssy = [PathRelativeToTestAssembly(@"UnitTestsResources\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])      

    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    member public this.``TypeProvider.Error.CodePrefix3.GoodErrorMessage`` () =
        this.VerifySquiggleAtStartOfMarker(
            fileContents = "
                type foo = N1.T<\"foo\",
                let z = 42",
            marker = "let",
            expectedSquiggle= (Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error,
                              "Expected type argument or static argument"),
            addtlRefAssy = [PathRelativeToTestAssembly(@"UnitTestsResources\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])      

    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    member public this.``TypeProvider.Error.CodePrefix4.GoodErrorMessage`` () =
        this.VerifySquiggleAtStartOfMarker(
            fileContents = "
                type foo = N1.T<\"foo\",42
                let z = 42",
            marker = "let",
            expectedSquiggle= (Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error,
                              "Incomplete structured construct at or before this point in type arguments. Expected ',', '>' or other token."),
            addtlRefAssy = [PathRelativeToTestAssembly(@"UnitTestsResources\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])      

    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    //This test case verify the error squiggle shows up when TypeProvider StaticParameter is Invalid 
    //Dummy Type Provider exposes a parametric type (N1.T) that takes 2 static params (string * int) 
    member public this.``TypeProvider.Error.InvalidStaticParameter`` () =
        this.VerifySquiggleAtStartOfMarker(
            fileContents = "
                           type foo = N1.T< const 100(*Marker*),2>",
            marker = "(*Marker*)",
            expectedSquiggle= (Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error,
                               "This expression was expected to have type\n"+
                               "    'string'    \n"+
                               "but here has type\n"+
                               "    'int'    "),
            addtlRefAssy = [PathRelativeToTestAssembly(@"UnitTestsResources\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])   

    
    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    //This test case verify the No squiggle doesn't show up in the file content marker where TypeProvider line is
    //Dummy Type Provider exposes a parametric type (N1.T) that takes 2 static params (string * int)
    member public this.``TypeProvider.WithNoSquiggle`` () =
        this.VerifyNoSquiggleAtStartOfMarker(
            fileContents = """
                           type foo = N1(*Marker*).T< const "Hello World",2>""",
            marker = "(*Marker*)",
            addtlRefAssy = [PathRelativeToTestAssembly(@"UnitTestsResources\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])     
    
   
    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    //This test case verify the Warning squiggle does show up in the file content marker where TypeProvider line is
    //Dummy Type Provider exposes a parametric type (N1.T) that takes 2 static params (string * int)
    member public this.``TypeProvider.WarningSquiggle`` () =
        this.VerifySquiggleAtStartOfMarker(
            fileContents = """
                           type foo = N1.T< 
                               const(*Marker*) "Hello World",2>""",
            marker = "(*Marker*)",
            expectedSquiggle= (Microsoft.VisualStudio.FSharp.LanguageService.Severity.Warning,
                                "Possible incorrect indentation: this token is "+
                                "offside of context started at position (2:39). "+
                                "Try indenting this token further or using "+
                                "standard formatting conventions."),
            addtlRefAssy = [PathRelativeToTestAssembly(@"UnitTestsResources\MockTypeProviders\DummyProviderForLanguageServiceTesting.dll")])     


    [<Test>]
    member public this.``Waring.Construct.TypeMatchWithoutAnnotation``() = 
        this.VerifySquiggleAtStartOfMarker(
            fileContents = """
                let f () = 
                    let g1 (x:'a) = x
                    let g2 (y:'a) = (y(*MWarning3*):string)
                    g1 3, g1 "3", g2 "4" """,
            marker = "(*MWarning3*)",
            expectedSquiggle= (Microsoft.VisualStudio.FSharp.LanguageService.Severity.Warning,
                              "This construct causes code to be less generic " +
                              "than indicated by the type annotations. The " +
                              "type variable 'a has been constrained to be " +
                              "type 'string'."))  
                                                      
    [<Test>] 
    member public this.``Warning.Expression.IncorrectFormat``() = 
        this.VerifySquiggleAtStartOfMarker(
            fileContents = """
                let nConstant = 
                100(*MWarning4*) """,
            marker = "(*MWarning4*)",
            expectedSquiggle= (Microsoft.VisualStudio.FSharp.LanguageService.Severity.Warning,
                              "Possible incorrect indentation: this token is "+
                              "offside of context started at position (2:17). "+
                              "Try indenting this token further or using "+
                              "standard formatting conventions."))                                    

    [<Test>]
    member public this.``Error.Method.ParameterNumDoseNotMatch``() =
        this.VerifySquiggleAtStartOfMarker(
            fileContents = """
                open System
                open System.IO

                if File.Exists(aFile) then
                    File.(aFile,"")(*M1*)""",
            marker = "(*M1*)",
            expectedSquiggle= (Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error,
                              "This construct is deprecated: This indexer notation has been removed from the F# language"))

    (*    Now that the language service reports typecheck errors even if there are parse errors, this location
    now has two errors.  And Salsa just picks the 'first' squiggle (based on some arbitrary implementation
    artifacts of what order errors are put in the collection), so 'GetSquiggleAtCursor' is unreliable if there 
    is more that on squiggle there. Workaround here is to look through the whole error list for what we're looking for.  *) 
    [<Test>]
    [<Ignore("Salsa limitation")>]                          
    member public this.``Error.Identifer.IllegalFloatPointLiteral``() = 
        this.VerifySquiggleAtStartOfMarker(
            fileContents = """
                let someFunction x.0.0(*MError2*) = 1""",
            marker = "(*MError2*)",
            expectedSquiggle= (Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error,
                                "Unexpected floating point literal in pattern. Expected identifier, '(', '(*)' or other token."))      
    
    
    [<Test>]
    member public this.ErrorSquiggleSpan() =
        let fileContents = """
            #light
            let _ = 1
            let a = 0.1.0(*MError1*)
            ;;
            let someFunction x.0.0(*MError2*) = 1
            ;;
            let a = ;(*MError3*)
            ;;
            let b =
            type(*MError4*) 
            ;;
            type =(*MError5*)
            ;;
            """     
        let (sln, proj, file) = this.CreateSingleFileProject(fileContents)  
        let errors = GetErrors(proj)
        let desiredError = errors |> List.tryFind (fun e -> e.Message = "Unexpected floating point literal in pattern. Expected identifier, '(', '(*)' or other token.")
        match desiredError with
        | None -> Assert.Fail("did not find expected error")
        | Some(e) -> 
            Assert.IsTrue(e.Context = TextSpan(iStartLine=5, iStartIndex=31, iEndLine=5, iEndIndex=34), "error had wrong location")   
  
    [<Test>]
    member public this.``Error.TypeCheck.ParseError.Bug67133``() =
        // Note: no 'open System', so DateTime is unknown
        this.VerifySquiggleAtStartOfMarker(
            fileContents = """
                let gDateTime (arr: DateTime(*Mark*)[]) =
                     arr.[0].""",
            marker = "(*Mark*)",
            expectedSquiggle= (Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error,"The type 'DateTime' is not defined"))
 
    [<Test>]
    member public this.``Error.CyclicalDeclarationDoesNotCrash``() =
        // Note: no 'open System', so DateTime is unknown
        this.VerifySquiggleAtStartOfMarker(
            fileContents = """type A(*Mark*) = int * A""",
            marker = "(*Mark*)",
            expectedSquiggle= (Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error,"This type definition involves an immediate cyclic reference through an abbreviation"))       

    /// FEATURE: Flags from the MSBuild compile target are respected.
    [<Test>]
    member public this.``Warning.FlagsAndSettings.TargetOptionsRespected``() =  
        let fileContent = """
            [<System.Obsolete("x")>]
            let fn x = 0
            let y = fn(*Mark*) 1"""
        /// Make sure the warning would be there.
        this.VerifySquiggleAtStartOfMarker(fileContent,"(*Mark*)",(Microsoft.VisualStudio.FSharp.LanguageService.Severity.Warning, "This construct is deprecated. x"))       

    /// When a .fs file is opened with no project context we do show squiggles
    /// for missing types etc.
    [<Test>]
    member public this.``OrphanFs.MissingTypesShouldNotShowErrors``() =
        let fileContent = """open Unknown(*Mark*)"""
        this.VerifySquiggleContainedAtStartOfMarker(fileContent,"(*Mark*)",(Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error, "Unknown"))  
        
    /// When a .fs file is opened with no project context we still do want to show squiggles
    /// for parse errors which could not have been caused by a missing reference or prior source file.
    [<Test>]
    member public this.``OrphanFs.ParseErrorsStillShow``() =  
        let fileContent = """let foo = let(*Mark*)"""
        this.VerifySquiggleContainedAtStartOfMarker(fileContent,"(*Mark*)",(Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error, "Every code block "))

    /// FEATURE: If a .fs file has a BuildAction other than "Compile", it behaves like a
    /// single-file-project with regards to intellisense.
    [<Test>]
    member public this.``Project.FsFileWithBuildActionOtherThanCompileBehavesLikeSingleFileProject``() =
        use _guard = this.UsingNewVS()
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        let file1 = AddFileFromText(project,"File1.fs",
                                    ["let bob = 42"])

        let file2 = AddFileFromTextEx(project, "File2.fs", "File2.fs", BuildAction.None,
                                    ["let i = 4"
                                     "let r = i.ToString()"
                                     "let x = File1.bob"])
        let file1 = OpenFile(project,"File1.fs")
        let file2 = OpenFile(project,"File2.fs")
        // file2 should not be able to 'see' file1
        MoveCursorToEndOfMarker(file2,"File1")
        let squiggle = GetSquiggleAtCursor(file2)
        Assert.IsTrue(snd squiggle.Value |> fun str -> str.Contains("The namespace"))// The namespace or module 'File1' is not defined

    /// FEATURE: Errors in the code are underlined with red squiggles and a clickable description of the error appears in the Error List.
    [<Test>]
    member public this.``Basic.Case1``() =
        let fileContent = """
            let x = 3
            let y = x(*Mark*) 4
            let arr = [| 1; 2; 3 |]"""
        this.VerifySquiggleContainedAtStartOfMarker(fileContent,"(*Mark*)",(Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error, "This value is not a function and cannot be applied")) 

    [<Test>]
    member public this.``Basic.Case2``() =
        let fileContent = """
            let x(*Mark*) = 3
            let y = x 4
            let arr = [| 1; 2; 3 |]"""            
        // test an error-free location
        this.VerifyNoSquiggleAtStartOfMarker(fileContent,"(*Mark*)")


    [<Test>]
    member public this.``Multiline.Bug5449.Case1``() =
        let fileContent = """
            let f x = 1
            let r = f(*Mark*)
                           234
                           567
            """
        this.VerifySquiggleContainedAtStartOfMarker(fileContent,"(*Mark*)",(Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error, "This value is not a function and cannot be applied"))  

    [<Test>]
    member public this.``Multiline.Bug5449.Case2``() =
        let fileContent = """
            let f x = 1
            let r = f
                           234(*Mark*)
                           567
            """
        this.VerifySquiggleContainedAtStartOfMarker(fileContent,"(*Mark*)",(Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error, "This value is not a function and cannot be applied"))  
        
    [<Test>]
    member public this.``ErrorAtTheEndOfFile``() =
        let fileContent = """3 + """
        let (sln, proj, file) = this.CreateSingleFileProject(fileContent)
        
        MoveCursorToEndOfMarker(file,"3 + ") 
        let squiggles = GetSquigglesAtCursor(file)
        let expectedSquiggle = (Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error,"Unexpected end")
        if squiggles |> List.exists (fun (sev,msg) -> fst expectedSquiggle = sev && msg.Contains(snd expectedSquiggle)) then
            ()  // ok, found it
        else
            Assert.Fail(sprintf "Expected %A but got %A" expectedSquiggle squiggles)

    [<Test>]
    member public this.``InComputationExpression.6095_a``() =
        let code = 
                                    ["let a = async {"
                                     "   let! [| r1; r2 |] = Async.Parallel [| async.Return(1); async.Return(2) |]"
                                     "   let yyyy = 4"
                                     "   return r1,r2 }"]
        let (_,_, file) = this.CreateSingleFileProject(code)
        
        // in the bug, the squiggle did not appear at the actual problem
        MoveCursorToEndOfMarker(file,"r1;") 
        let ans = GetSquiggleAtCursor(file)
        match ans with
        | Some(sev,msg) -> AssertEqual(Microsoft.VisualStudio.FSharp.LanguageService.Severity.Warning,sev)
                           AssertContains(msg,"Incomplete pattern matches on this expression. For example, the value '[|_; _; _|]' may indicate a case not covered by the pattern(s).")
        | _ -> Assert.Fail("Expected squiggle in computation expression")

        // in the bug, the squiggle was on the 'whole rest of the computation'
        MoveCursorToEndOfMarker(file,"yyy") 
        let ans = GetSquiggleAtCursor(file)
        AssertNoSquiggle(ans)

        MoveCursorToEndOfMarker(file,"retur") 
        let ans = GetSquiggleAtCursor(file)
        AssertNoSquiggle(ans)

    [<Test>]
    member public this.``InComputationExpression.6095_b``() =
        let code = 
                                    ["let a = async {"
                                     "    for [|x;y|] in [| [|42|] |] do () }"]
        let (_, _, file) = this.CreateSingleFileProject(code)
        
        // in the bug, the squiggle covered whole for..do
        MoveCursorToEndOfMarker(file,"fo") 
        let ans = GetSquiggleAtCursor(file)
        AssertNoSquiggle(ans)

        MoveCursorToEndOfMarker(file,"x") 
        let ans = GetSquiggleAtCursor(file)
        AssertNoSquiggle(ans)

        MoveCursorToEndOfMarker(file,"in") 
        let ans = GetSquiggleAtCursor(file)
        AssertNoSquiggle(ans)

        MoveCursorToEndOfMarker(file,"do") 
        let ans = GetSquiggleAtCursor(file)
        AssertNoSquiggle(ans)

        MoveCursorToEndOfMarker(file,"42") 
        let ans = GetSquiggleAtCursor(file)
        match ans with
        | Some(sev,msg) -> AssertEqual(Microsoft.VisualStudio.FSharp.LanguageService.Severity.Warning,sev)
                           AssertContains(msg,"Incomplete pattern matches on this expression. For example, the value '[|_; _; _|]' may indicate a case not covered by the pattern(s).")
        | _ -> Assert.Fail("Expected squiggle in computation expression")

    [<Test>]
    member public this.``InComputationExpression.6095_c``() =  // this one is not in a computation expression actually
        let code = ["let f = function | [| a;b |] -> ()"]
        let (_, _, file) = this.CreateSingleFileProject(code)
        
        // in the bug, the squiggle covered whole function... end of line
        MoveCursorToEndOfMarker(file,"a") 
        let ans = GetSquiggleAtCursor(file)
        AssertNoSquiggle(ans)

        MoveCursorToEndOfMarker(file,"(") 
        let ans = GetSquiggleAtCursor(file)
        AssertNoSquiggle(ans)

        MoveCursorToEndOfMarker(file,"func") // we just want the squiggle under the word 'function', which is shorthand for 'fun x -> match x when' (squiggle would go under 'x')
        let ans = GetSquiggleAtCursor(file)
        match ans with
        | Some(sev,msg) -> AssertEqual(Microsoft.VisualStudio.FSharp.LanguageService.Severity.Warning,sev)
                           AssertContains(msg,"Incomplete pattern matches on this expression. For example, the value '[|_; _; _|]' may indicate a case not covered by the pattern(s).")
        | _ -> Assert.Fail("Expected squiggle in computation expression")

    [<Test>]
    member public this.``InComputationExpression.6095_d``() =  // this one is not in a computation expression actually
        let code = ["for [|a;b|] in [| [|42|] |] do ()"]
        let (_, _, file) = this.CreateSingleFileProject(code)
        
        // in the bug, the squiggle covered whole for..()
        MoveCursorToEndOfMarker(file,"fo") 
        let ans = GetSquiggleAtCursor(file)
        AssertNoSquiggle(ans)

        MoveCursorToEndOfMarker(file,"a") 
        let ans = GetSquiggleAtCursor(file)
        match ans with
        | Some(sev,msg) -> AssertEqual(Microsoft.VisualStudio.FSharp.LanguageService.Severity.Warning,sev)
                           AssertContains(msg,"Incomplete pattern matches on this expression. For example, the value '[|_; _; _|]' may indicate a case not covered by the pattern(s).")
        | _ -> Assert.Fail("Expected squiggle in computation expression")

        MoveCursorToEndOfMarker(file,"in") 
        let ans = GetSquiggleAtCursor(file)
        AssertNoSquiggle(ans)

        MoveCursorToEndOfMarker(file,"do") 
        let ans = GetSquiggleAtCursor(file)
        AssertNoSquiggle(ans)

        MoveCursorToEndOfMarker(file,"42")
        let ans = GetSquiggleAtCursor(file)
        AssertNoSquiggle(ans)


    [<Test>]
    member public this.``GloballyScoped.6284``() =
        use _guard = this.UsingNewVS()
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        let bfile = AddFileFromText(project,"Bar.fs",
                                    ["namespace global
                                        type Bar() = 
                                          member this.Y = 1"])
        let programFile = AddFileFromText(project,"Program.fs", ["let b = global.Bar()"])
        let programFile = OpenFile(project,"Program.fs")       
        // test an error
        MoveCursorToEndOfMarker(programFile,"let b = glo") 
        let ans = GetSquiggleAtCursor(programFile)
        AssertNoSquiggle(ans)
                
    [<Test>]
    member public this.``InComputationExpression.914685``() =
        let code = ["async { if true then return 1 } |> ignore"]
        let (_, _, file) = this.CreateSingleFileProject(code)
        // test an error
        MoveCursorToEndOfMarker(file,"async { if tr") 
        let ans = GetSquiggleAtCursor(file)
        match ans with
            | Some(sev,msg) -> AssertEqual(Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error,sev)
                               AssertContains(msg,"The type 'int'")
            | _ -> Assert.Fail("Expected squiggle in computation expression")

    [<Test>]
    member public this.``InComputationExpression.214740``() =
        this.VerifySquiggleAtStartOfMarker(
            fileContents = """
                let x = 1 + ""    // type error, but no squiggle if parse error below
                let l = [1;2]
                seq {             // (seq or async or whatever, hits same path through the parser)
                    for x in l.   // delete this dot and squiggle above appears
                }""",
            marker = "\"\"",
            expectedSquiggle = (Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error,
                                 "The type 'string' does not match the type 'int'")) 

    /// Extra endif
    [<Test>]
    member public this.``ExtraEndif``() =
        let fileContent = """
            #if UNDEFINED //(*If*)
                let x = 1(*Inactive*)
            #else //(*Else*)
                let(*Active*) x = 1
            #endif //(*Endif*)
            #endif(*Mark*) //(*Extra endif*)"""
        this.VerifySquiggleContainedAtStartOfMarker(fileContent,"(*Mark*)",(Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error, "#endif has no matching #if in implementation file")) 
         
    [<Test>]
    member public this.``DirectivesInString``() =
        let fileContent = """
            #if UNDEFINED
            #else(*Mark*)
            let s = "
            #endif
            "
            let testme = 1"""
        this.VerifySquiggleContainedAtStartOfMarker(fileContent,"(*Mark*)",(Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error, "No #endif found"))           

    (* Various #if/#else/#endif errors -------------------------------------------------- *)
    
    member private this.TestSquiggle error lines marker expected =
        let code = "#light"::""::lines
        let (_, _, file) = this.CreateSingleFileProject(code, defines = ["FOO"])
        MoveCursorToStartOfMarker(file, marker)
        let squiggle = GetSquiggleAtCursor(file)
        match squiggle with
        | Some(sev,msg) -> AssertEqual((if error then Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error else Microsoft.VisualStudio.FSharp.LanguageService.Severity.Warning),sev)
                           AssertContains(msg, expected)
        | _ -> Assert.Fail(sprintf "No squiggle seen. Expected: '%s'" expected)   
        
    [<Test>]
    member public this.``Squiggles.HashNotFirstSymbolA``() =
        this.TestSquiggle true [ "(**) #if IDENT"; "#endif" ] "if" "#if directive must appear as the first non-whitespace"

    [<Test>]
    member public this.``Squiggles.HashNotFirstSymbolB``() =
        this.TestSquiggle true [ "#if FOO"; "(**) #else"; "#endif" ] "else" "#else directive must appear as the first non-whitespace"

    [<Test>]
    member public this.``Squiggles.HashNotFirstSymbolC``() =
        this.TestSquiggle true [ "#if IDENT"; "#else"; "(**) #endif" ] "endif" "#endif directive must appear as the first non-whitespace"
        
    [<Test>]
    member public this.``Squiggles.HashIfWithoutIdent``() =
        this.TestSquiggle true [ "#if"; "#endif" ] "if" "#if directive should be immediately followed by an identifier"

    [<Test>]
    member public this.``Squiggles.HashIfWithMultilineComment``() =
        this.TestSquiggle true [ "#if IDENT (* aaa *)"; "#endif" ] "(* aaa" "Expected single line comment or end of line"

    [<Test>]
    member public this.``Squiggles.HashIfWithUnexpected``() =
        this.TestSquiggle true [ "#if IDENT whatever"; "#endif" ] "whatever" "Incomplete preprocessor expression"

     // FEATURE: Touching a depended-upon file will cause a intellisense to update in the currently edited file.
    [<Test>]
    member public this.``Refresh.RefreshOfDependentOpenFiles.Bug2166.CaseA``() =    
        use _guard = this.UsingNewVS()
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        printfn "Adding file1"
        let file1 = AddFileFromText(project,"File1.fs", 
                                    ["#light"
                                     "module Module1"
                                    ])
        let file1 = OpenFile(project,"File1.fs")
        gpatcc.AssertExactly(AA[file1],AA[file1], true (* deleted, because a new file was added to the project *))

        printfn "Adding file2"
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        let file2 = AddFileFromText(project,"File2.fs",
                                    ["#light"
                                     "open Module2"
                                     ])
        let file2 = OpenFile(project,"File2.fs")
        gpatcc.AssertExactly(AA[file1;file2],AA[file1;file2], true (* deleted, because a new file was added to the project *))
        
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        // Expect an error here because of reference to nonexistent Module2
        MoveCursorToEndOfMarker(file2,"open Modu")
        let ans = GetSquiggleAtCursor(file2)
        AssertSquiggleIsErrorContaining(ans, "Module2")                    
        gpatcc.AssertExactly(notAA[],notAA[])
        
        // Fix File1.fs so that it contains Module2
        printfn "Fixing file1 in memory"
        ReplaceFileInMemory file1 
                                ["#light"
                                 "module Module2"]
                                 
        gpatcc.AssertExactly(notAA[], notAA[])
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        printfn "Fixing file1 on disk"
        SaveFileToDisk file1      
        gpatcc.AssertExactly(notAA[file1],notAA[file1;file2])
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        SwitchToFile this.VS file2
        TakeCoffeeBreak(this.VS)

        MoveCursorToEndOfMarker(file2,"open Modu") // This switches focus back to file2
        TakeCoffeeBreak(this.VS)
        let ans = GetSquiggleAtCursor(file2)
        AssertNoSquiggle(ans)
        gpatcc.AssertExactly(notAA[],notAA[])

     // FEATURE: Touching a depended-upon file will cause a intellisense to update in the currently edited file.
    [<Test>]
    member public this.``Refresh.RefreshOfDependentOpenFiles.Bug2166.CaseB``() =  
        use _guard = this.UsingNewVS()  
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        let solution = this.CreateSolution()
        let project = CreateProject(solution,"testproject")
        printfn "Adding file1"
        let file1 = AddFileFromText(project,"File1.fs", 
                                    ["#light"
                                     "module Module1"
                                    ])
        let file1 = OpenFile(project,"File1.fs")
        gpatcc.AssertExactly(AA[file1],AA[file1], true (* deleted, because a new file was added to the project *))

        printfn "Adding file2"
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        let file2 = AddFileFromText(project,"File2.fs",
                                    ["#light"
                                     "open Module2"
                                     ])
        let file2 = OpenFile(project,"File2.fs")
        // See long comment in (Case A), above.
        // The first time we glance at file1 since the project file changed (file2 was added) will delete.
        MoveCursorToEndOfMarker(file1,"#light")  // focus file1, so it gets idled
        gpatcc.AssertExactly(AA[file1;file2],AA[file1;file2], true (* deleted, because a new file was added to the project *))
        
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        // Expect an error here because of reference to nonexistent Module2
        MoveCursorToEndOfMarker(file2,"open Modu")
        let ans = GetSquiggleAtCursor(file2)
        AssertSquiggleIsErrorContaining(ans, "Module2")                    
        gpatcc.AssertExactly(notAA[],notAA[])
        
        // Fix File1.fs so that it contains Module2
        printfn "Fixing file1 in memory"
        ReplaceFileInMemory file1 
                                ["#light"
                                 "module Module2"]
        gpatcc.AssertExactly(notAA[],notAA[])  // incremental builder does not see in-memory changes
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        printfn "Fixing file1 on disk"
        SaveFileToDisk file1      
        gpatcc.AssertExactly(notAA[file1],notAA[file1;file2])
        let gpatcc = GlobalParseAndTypeCheckCounter.StartNew(this.VS)
        MoveCursorToEndOfMarker(file2,"open Modu") // This switches focus back to file2
        TakeCoffeeBreak(this.VS)
        let ans = GetSquiggleAtCursor(file2)
        AssertNoSquiggle(ans)
        gpatcc.AssertExactly(notAA[],notAA[])
        
    // FEATURE: Give a nice error message when a type in a unreferenced dependant assembly is used
    [<Test>]
    member public this.``MissingDependencyReferences.MissingAssemblyErrorMessage``() = 
        let code = 
                                    ["#light"
                                     "let myForm = new System.Windows.Forms.Form()"
                                     "let bounds = myForm.Bounds"
                                    ]
        let (_,_, file) = this.CreateSingleFileProject(code, references = ["System"; "System.Windows.Forms"])
        MoveCursorToEndOfMarker(file,"let bounds = myFo")
        let ans = GetSquiggleAtCursor(file)
        match ans with
        | Some(sev,msg) -> AssertEqual(Microsoft.VisualStudio.FSharp.LanguageService.Severity.Error,sev)
                           AssertContains(msg,"System.Drawing")
                           AssertContains(msg,"You must add a reference")
        | _ -> Assert.Fail("No squiggle seen")  



// Context project system
[<TestFixture>] 
type UsingProjectSystem() = 
    inherit UsingMSBuild(VsOpts = LanguageServiceExtension.ProjectSystemTestFlavour)
