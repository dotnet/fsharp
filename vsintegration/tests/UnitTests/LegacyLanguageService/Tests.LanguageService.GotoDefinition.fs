// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Tests.LanguageService.GotoDefinition

open System
open System.IO
open NUnit.Framework
open Salsa.Salsa
open Salsa.VsOpsUtils
open UnitTests.TestLib.Salsa
open UnitTests.TestLib.Utils
open System.Collections.Generic
open System.Text.RegularExpressions
open UnitTests.TestLib.LanguageService
open UnitTests.TestLib.ProjectSystem
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices

[<TestFixture>]
[<Category "LanguageService">] 
type UsingMSBuild()  = 
    inherit LanguageServiceBaseTests()

    //GoToDefinitionSuccess Helper Function
    member private this.VerifyGoToDefnSuccessAtStartOfMarker(fileContents : string, marker : string,  definitionCode : string,?addtlRefAssy : string list) =
        let (sln, proj, file) = this.CreateSingleFileProject(fileContents, ?references = addtlRefAssy)

        MoveCursorToStartOfMarker (file, marker)
        let identifier = (GetIdentifierAtCursor file).Value |> fst //use marker to get the identifier
        let result = GotoDefinitionAtCursor file
        CheckGotoDefnResult
            (GotoDefnSuccess identifier definitionCode) 
            file
            result 
    
    member private this.VerifyGotoDefnSuccessForNonIdentifierAtStartOfMarker(fileContents : string, marker: string, pos : int * int, ?extraRefs) =
        let (_, _, file) = this.CreateSingleFileProject(fileContents, ?references = extraRefs)
        MoveCursorToStartOfMarker (file, marker)
        let result = GotoDefinitionAtCursor file
        Assert.IsTrue(result.Success, "result.Success")
        let actualPos = (result.Span.iStartLine, result.Span.iStartIndex)
        let line = GetLineNumber file (result.Span.iStartLine + 1)
        printfn "Actual line:%s, actual pos:%A" line actualPos
        Assert.AreEqual(pos, actualPos, "pos")
                    
    //GoToDefinitionFail Helper Function
    member private this.VerifyGoToDefnFailAtStartOfMarker(fileContents : string,  marker :string,?addtlRefAssy : string list) =
        
        this.VerifyGoToDefnFailAtStartOfMarker(
            fileContents = fileContents,
            marker = marker,
            f = (fun (file,result) -> CheckGotoDefnResult GotoDefnFailure file result),
            ?addtlRefAssy = addtlRefAssy
            )


    //GoToDefinitionFail Helper Function
    member private this.VerifyGoToDefnFailAtStartOfMarker(fileContents : string,  marker :string, f : OpenFile * GotoDefnResult -> unit, ?addtlRefAssy : string list) =
        let (sln, proj, file) = this.CreateSingleFileProject(fileContents, ?references = addtlRefAssy)

        MoveCursorToStartOfMarker (file, marker)
        let result = GotoDefinitionAtCursor file
        f (file, result)


    //GoToDefinition verify no Error dialog
    //The verification result should be:
    //  Fail at automation lab
    //  Succeed on dev machine with enlistment installed.
    member private this.VerifyGoToDefnNoErrorDialogAtStartOfMarker(fileContents : string,  marker :string, definitionCode : string, ?addtlRefAssy : string list) =
        let (sln, proj, file) = this.CreateSingleFileProject(fileContents, ?references = addtlRefAssy)

        MoveCursorToStartOfMarker (file, marker)
        let identifier = (GetIdentifierAtCursor file).Value |> fst //use marker to get the identifier
        let result = GotoDefinitionAtCursor file
        if not result.Success then
            CheckGotoDefnResult
                GotoDefnFailure 
                file
                result
        else
            CheckGotoDefnResult
                (GotoDefnSuccess identifier definitionCode) 
                file 
                result 
    
    [<Test>]                                  
    member this.``Operators.TopLevel``() = 
        this.VerifyGotoDefnSuccessForNonIdentifierAtStartOfMarker(
            fileContents = """
                let (===) a b = a = b
                let _ = 1 === 2
                """,
            marker = "=== 2",
            pos=(1,21)
            )

    [<Test>]                                  
    member this.``Operators.Member``() = 
        this.VerifyGotoDefnSuccessForNonIdentifierAtStartOfMarker(
            fileContents = """
                type U = U
                    with
                    static member (+++) (U, U) = U
                let _ = U +++ U
                """,
            marker = "++ U",
            pos=(3,35)
            )

    [<Test>]
    member public this.``Value``() = 
        this.VerifyGoToDefnSuccessAtStartOfMarker(
            fileContents = """
                type DiscUnion =
                    | Alpha of string
                    | Beta of decimal * unit
                    | Gamma

                let valueX = Beta(1.0M, ())(*GotoTypeDef*)
                let valueY = valueX (*GotoValDef*)
                """,
            marker = "valueX (*GotoValDef*)",
            definitionCode = "let valueX = Beta(1.0M, ())(*GotoTypeDef*)")
    
    [<Test>]
    member public this.``DisUnionMember``() =
        this.VerifyGoToDefnSuccessAtStartOfMarker(
                            fileContents = """
                type DiscUnion =
                    | Alpha of string
                    | Beta of decimal * unit
                    | Gamma

                let valueX = Beta(1.0M, ())(*GotoTypeDef*)
                let valueY = valueX (*GotoValDef*)
                """,
            marker = "Beta(1.0M, ())(*GotoTypeDef*)",
            definitionCode = "| Beta of decimal * unit")        

    [<Test>]
    member public this.``PrimitiveType``() =
        this.VerifyGoToDefnFailAtStartOfMarker(
            fileContents = """
                // Can't goto def on an int literal
                let bi = 123456I""",
            marker = "123456I")
           
    [<Test>]
    member public this.``OnTypeDefintion``() =
        this.VerifyGoToDefnSuccessAtStartOfMarker(
            fileContents = """
                //regression test for bug 2516
                type One (*Marker1*) = One
                let f (x : One (*Marker2*)) = 2
                """,
            marker = "One (*Marker1*)",
            definitionCode = "type One (*Marker1*) = One")

    [<Test>]
    member public this.``Parameter``() = 
        this.VerifyGoToDefnSuccessAtStartOfMarker(
            fileContents = """
                //regression test for bug 2516
                type One (*Marker1*) = One
                let f (x : One (*Marker2*)) = 2
                """,
            marker = "One (*Marker2*)",
            definitionCode = "type One (*Marker1*) = One")    

    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.DefinitionLocationAttribute")>]
    // This test case check the GotoDefinition (i.e. the TypeProviderDefinitionLocation Attribute)
    // We expect the correct FilePath, Line and Column on provided: Type, Event, Method, and Property
    // TODO: add a case for a provided Field
    member public this.``GotoDefinition.TypeProvider.DefinitionLocationAttribute``() =
        use _guard = this.UsingNewVS()
        // Note that the verification helped method is custom because we *do* care about the column as well, 
        // which is something that the general purpose method in this file (surprisingly!) does not do.
        let VerifyGoToDefnSuccessAtStartOfMarkerColumn(fileContents : string, marker : string,  definitionCode : string, typeProviderAssembly : string, columnMarker : string) =
            let (sln, proj, file) = GlobalFunctions.CreateNamedSingleFileProject (this.VS, (fileContents, "File.fs"))  

            // Add reference to the type provider
            this.AddAssemblyReference(proj,typeProviderAssembly)

            // Identify (line,col) of the destination, i.e. where we expect to land after hitting F12
            // We do this to avoid hardcoding absolute numbers in the code.
            MoveCursorToStartOfMarker (file,columnMarker)
            let _,column = GetCursorLocation(file)

            // Put curson at start of marker and then hit F12
            MoveCursorToStartOfMarker (file, marker)
            let identifier = (GetIdentifierAtCursor file).Value |> fst
            let result = GotoDefinitionAtCursor file

            // Execute validation (on file name and line)                   
            CheckGotoDefnResult
                (GotoDefnSuccess identifier definitionCode) 
                file 
                result 
            
            // Reminder: coordinates in the F# compiler are 1-based for lines, and 0-based for columns
            //           coordinates from type providers are 1-based for both lines and columns
            //           GetCursorLocation() seems to return something even more off by 1...
            let column' = column - 2

            match result.ToOption() with 
            | Some(span,_) -> Assert.AreEqual(column',span.iStartIndex, "The cursor landed on the incorrect column!") 
            | None ->  Assert.Fail <| sprintf "Expected to find the definition at column '%d' but GotoDefn failed." column'

        // Basic scenario on a provided Type
        let ``Type.BasicScenario``() = 
            VerifyGoToDefnSuccessAtStartOfMarkerColumn("""
                let a = typeof<N.T(*GotoValDef*)>
                // A0(*ColumnMarker*)1234567890
                // B01234567890
                // C01234567890 """,
            "T(*GotoValDef*)",
             "// A0(*ColumnMarker*)1234567890",            
            PathRelativeToTestAssembly(@"DefinitionLocationAttribute.dll"),
            "(*ColumnMarker*)")

        // This test case checks the type with space in between like N.``T T`` for GotoDefinition
        let ``Type.SpaceInTheType``() = 
             VerifyGoToDefnSuccessAtStartOfMarkerColumn("""
                let a = typeof<N.``T T``>
                // A0(*ColumnMarker*)1234567890
                // B01234567890
                // C01234567890 """,
            "T``",
            "// A0(*ColumnMarker*)1234567890",
            PathRelativeToTestAssembly(@"DefinitionLocationAttributeWithSpaceInTheType.dll"),
            "(*ColumnMarker*)") 
        
        // Basic scenario on a provided Constructor
        let ``Constructor.BasicScenario``() = 
            
            VerifyGoToDefnSuccessAtStartOfMarkerColumn(""" 
                let foo = new N.T(*GotoValDef*)()
                // A0(*ColumnMarker*)1234567890
                // B01234567890
                // C01234567890 """,
            "T(*GotoValDef*)",
             "// A0(*ColumnMarker*)1234567890",            
            PathRelativeToTestAssembly(@"DefinitionLocationAttribute.dll"),
            "(*ColumnMarker*)")
          
        // Basic scenario on a provided Method
        let ``Method.BasicScenario``() = 
            VerifyGoToDefnSuccessAtStartOfMarkerColumn("""
                let t = new N.T.M(*GotoValDef*)()
                // A0(*ColumnMarker*)1234567890
                // B01234567890
                // C01234567890 """,
            "M(*GotoValDef*)",
             "// A0(*ColumnMarker*)1234567890",            
            PathRelativeToTestAssembly(@"DefinitionLocationAttribute.dll"),
            "(*ColumnMarker*)")
        
        // Basic scenario on a provided Property
        let ``Property.BasicScenario``() = 
            VerifyGoToDefnSuccessAtStartOfMarkerColumn(""" 
                let p = N.T.StaticProp(*GotoValDef*)
                // A0(*ColumnMarker*)1234567890
                // B01234567890
                // C01234567890 """,
            "StaticProp(*GotoValDef*)",
             "// A0(*ColumnMarker*)1234567890",            
            PathRelativeToTestAssembly(@"DefinitionLocationAttribute.dll"),
            "(*ColumnMarker*)")
        
        // Basic scenario on a provided Event
        let ``Event.BasicScenario``() = 
            VerifyGoToDefnSuccessAtStartOfMarkerColumn(""" 
                let t = new N.T()
                t.Event1(*GotoValDef*)
                // A0(*ColumnMarker*)1234567890
                // B01234567890
                // C01234567890 """,
            "Event1(*GotoValDef*)",
             "// A0(*ColumnMarker*)1234567890",            
            PathRelativeToTestAssembly(@"DefinitionLocationAttribute.dll"),
            "(*ColumnMarker*)")
        
        // Actually execute all the scenarios...      
        ``Type.BasicScenario``()
        ``Type.SpaceInTheType``()
        ``Constructor.BasicScenario``()
        ``Method.BasicScenario``()
        ``Property.BasicScenario``()
        ``Event.BasicScenario``()

    
    [<Test>]
    member public this.``GotoDefinition.NoSourceCodeAvailable``() = 
        this.VerifyGoToDefnFailAtStartOfMarker
            (
                fileContents = "System.String.Format(\"\")",
                marker = "ormat",
                f = (fun (_, result) ->
                    Assert.IsFalse(result.Success)
                    Assert.IsTrue(result.ErrorDescription.Contains("Source code is not available"))
                    )
            )
    
    [<Test>]
    member public this.``GotoDefinition.NoIdentifierAtLocation``() = 
        let useCases = 
            [
                "let x = 1", "1"
                "let x = 1.2", ".2"
                "let x = \"123\"", "2"
            ]
        for (source, marker) in useCases do
            this.VerifyGoToDefnFailAtStartOfMarker
                (
                    fileContents = source,
                    marker = marker,
                    f = (fun (_, result) ->
                        Assert.IsFalse(result.Success)
                        Assert.IsTrue(result.ErrorDescription.Contains("Cursor is not on identifier"))
                        )
                )

    [<Test>]
    member public this.``GotoDefinition.ProvidedTypeNoDefinitionLocationAttribute``() =  

        this.VerifyGoToDefnFailAtStartOfMarker
            (
                fileContents = """
                type T = N1.T<"", 1>
                """,
                marker = "T<",
                f = (fun (_, result) -> Assert.IsFalse(result.Success) ),
                addtlRefAssy = [PathRelativeToTestAssembly(@"DummyProviderForLanguageServiceTesting.dll")]
            )
        
    [<Test>]
    member public this.``GotoDefinition.ProvidedMemberNoDefinitionLocationAttribute``() = 
        let useCases = 
            [
                """
                type T = N1.T<"", 1>
                T.Param1
                """, "ram1", "Param1"

                """
                type T = N1.T1
                T.M1(1)
                """, "1(", "M1"
            ]

        for (source, marker, name) in useCases do
            this.VerifyGoToDefnFailAtStartOfMarker
                (
                    fileContents = source,
                    marker = marker,
                    f = (fun (_, result) ->
                        Assert.IsFalse(result.Success)
                        let expectedText = sprintf "provided member '%s'" name
                        Assert.IsTrue(result.ErrorDescription.Contains(expectedText))
                        ),
                    addtlRefAssy = [PathRelativeToTestAssembly(@"DummyProviderForLanguageServiceTesting.dll")]
                )
    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.DefinitionLocationAttribute.Negative")>]
    // This test case is when the TypeProviderDefinitionLocationAttribute filepath doesn't exist  for TypeProvider Type
    member public this.``GotoDefinition.TypeProvider.DefinitionLocationAttribute.Type.FileDoesnotExist``() =
        this.VerifyGoToDefnFailAtStartOfMarker(
            fileContents = """
                let a = typeof<N.T(*GotoValDef*)>
                // A0(*Marker*)1234567890
                // B01234567890
                // C01234567890 """,
            marker = "T(*GotoValDef*)",
            addtlRefAssy = [PathRelativeToTestAssembly(@"DefinitionLocationAttributeFileDoesnotExist.dll")])

    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.DefinitionLocationAttribute.Negative")>]
    [<Ignore("Need some work to detect the line doesnot exist.")>]
    //This test case is when the TypeProviderDefinitionLocationAttribute Line doesn't exist  for TypeProvider Type
    member public this.``GotoDefinition.TypeProvider.DefinitionLocationAttribute.Type.LineDoesnotExist``() =
        this.VerifyGoToDefnFailAtStartOfMarker(
            fileContents = """
                let a = typeof<N.T(*GotoValDef*)>
                // A0(*Marker*)1234567890
                // B01234567890
                // C01234567890 """,
            marker = "T(*GotoValDef*)",
            addtlRefAssy = [PathRelativeToTestAssembly(@"DefinitionLocationAttributeLineDoesnotExist.dll")])
     
    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.DefinitionLocationAttribute.Negative")>]
    // This test case is when the TypeProviderDefinitionLocationAttribute filepath doesn't exist  for TypeProvider Constructor
    member public this.``GotoDefinition.TypeProvider.DefinitionLocationAttribute.Constructor.FileDoesnotExist``() =
        this.VerifyGoToDefnFailAtStartOfMarker(
            fileContents = """
                let foo = new N.T(*GotoValDef*)()
                // A0(*Marker*)1234567890
                // B01234567890
                // C01234567890 """,
            marker = "T(*GotoValDef*)",
            addtlRefAssy = [PathRelativeToTestAssembly(@"DefinitionLocationAttributeFileDoesnotExist.dll")])


         
    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.DefinitionLocationAttribute.Negative")>]
    //This test case is when the TypeProviderDefinitionLocationAttribute filepath doesn't exist  for TypeProvider Method
    member public this.``GotoDefinition.TypeProvider.DefinitionLocationAttribute.Method.FileDoesnotExist``() =
        this.VerifyGoToDefnFailAtStartOfMarker(
            fileContents = """ 
                let t = new N.T.M(*GotoValDef*)()
                // A0(*Marker*)1234567890
                // B01234567890
                // C01234567890  """,
            marker = "M(*GotoValDef*)",
            addtlRefAssy = [PathRelativeToTestAssembly(@"DefinitionLocationAttributeFileDoesnotExist.dll")])

    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.DefinitionLocationAttribute.Negative")>]
    // This test case is when the TypeProviderDefinitionLocationAttribute filepath doesn't exist  for TypeProvider Property
    member public this.``GotoDefinition.TypeProvider.DefinitionLocationAttribute.Property.FileDoesnotExist``() =
        this.VerifyGoToDefnFailAtStartOfMarker(
            fileContents = """ 
                let p = N.T.StaticProp(*GotoValDef*)
                // A0(*Marker*)1234567890
                // B01234567890
                // C01234567890 """,
            marker = "StaticProp(*GotoValDef*)",
            addtlRefAssy = [PathRelativeToTestAssembly(@"DefinitionLocationAttributeFileDoesnotExist.dll")])
    
    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.DefinitionLocationAttribute.Negative")>]
    //This test case is when the TypeProviderDefinitionLocationAttribute filepath doesn't exist  for TypeProvider Event
    member public this.``GotoDefinition.TypeProvider.DefinitionLocationAttribute.Event.FileDoesnotExist``() =
        this.VerifyGoToDefnFailAtStartOfMarker(
            fileContents = """ 
                let t = new N.T()
                t.Event1(*GotoValDef*)
                // A0(*Marker*)1234567890
                // B01234567890
                // C01234567890 """,
            marker = "Event1(*GotoValDef*)",
            addtlRefAssy = [PathRelativeToTestAssembly(@"DefinitionLocationAttributeFileDoesnotExist.dll")])

    [<Test>]
    member public this.``ModuleDefintion``() =
        this.VerifyGoToDefnSuccessAtStartOfMarker(
            fileContents = """
                //regretion test for bug 2517
                module Foo (*MarkerModuleDefinition*) =
                  let x = ()
                """,
            marker = "Foo (*MarkerModuleDefinition*)",
            definitionCode = "module Foo (*MarkerModuleDefinition*) =")

    [<Test>]
    member public this.``Record.Field.Defintion``() = 
        this.VerifyGoToDefnSuccessAtStartOfMarker(
            fileContents = """
                //regretion test for bug 2518
                type MyRec =
                  { myX (*MarkerXFieldDefinition*) : int
                    myY (*MarkerYFieldDefinition*) : int
                  }
                let rDefault =
                  { myX (*MarkerXField*) = 2
                    myY (*MarkerYField*) = 3
                  }
                """,      
            marker = "myX (*MarkerXFieldDefinition*)",   
            definitionCode = "{ myX (*MarkerXFieldDefinition*) : int")

    [<Test>]
    member public this.``Record.Field.Usage``() = 
        this.VerifyGoToDefnSuccessAtStartOfMarker(
            fileContents = """
                //regretion test for bug 2518
                type MyRec =
                  { myX (*MarkerXFieldDefinition*) : int
                    myY (*MarkerYFieldDefinition*) : int
                  }
                let rDefault =
                  { myX (*MarkerXField*) = 2
                    myY (*MarkerYField*) = 3
                  }
                """,
            marker = "myY (*MarkerYField*)",
            definitionCode = " myY (*MarkerYFieldDefinition*) : int")

    /// run a GotoDefinition test where the expected result is a file that we
    /// have an `OpenFile` handle for (this won't work, e.g., if this file is a
    /// generated .fsi that (potentially) doesn't yet exist)

    /// exp  = (<identifier where cursor ought to be>, <whole line where cursor ought to be>, <name of file that's expected>) option
    /// file = <file to gotodefinition in>
    /// act  = <the result of the GotoDefinition call>
    member internal this.GotoDefinitionCheckResultAgainst (exp : (string * string * string) option)(file : OpenFile)(act : GotoDefnResult) : unit =
      match (exp, act.ToOption()) with
      | (Some (toFind, expLine, expFile), Some (span, actFile)) -> printfn "%s" "Result received, as expected; checking."
                                                                   Assert.AreEqual (expFile, actFile)
                                                                   printfn "%s" "Filename matches expected."
                                                                   MoveCursorTo(file, span.iStartLine + 1, span.iStartIndex + 1) // adjust & move to the identifier
                                                                   match GetIdentifierAtCursor file with // REVIEW: actually check that we're on the leftmost character of the identifier
                                                                   | None         -> Assert.Fail("No identifier at cursor!")
                                                                   | Some (id, _) -> Assert.AreEqual (toFind, id) // are we on the identifier we expect?
                                                                                     printfn "%s" "Identifier at cursor matches expected."
                                                                                     Assert.AreEqual (expLine.Trim (), (span.iStartLine |> (+) 1 |> GetLineNumber file).Trim ()) // ignore initial- / final-whitespace-introduced noise; adjust for difference in index numbers
                                                                                     printfn "%s" "Line at cursor matches expected."
      | (None,                            None)                 -> printfn "%s" "No result received, as expected." // sometimes we may expect GotoDefinition to fail, e.g., when the cursor isn't placed on a valid position (i.e., over an identifier, and, maybe, a constant if we decide to support that)
      | (Some _,                          None)                 -> Assert.Fail("No result received, but one was expected!") // distinguish this and the following case to give insight in case of failure
      | (None,                            Some _)               -> Assert.Fail("Result received, but none was expected!")

    /// this can be used when we don't have the expected file open; we still
    /// need its name

    /// exp = (<identifier where cursor ought to be>, <name of file that's expected>) option
    member internal this.GotoDefinitionCheckResultAgainstAnotherFile (proj : OpenProject)(exp : (string * string) option)(act : GotoDefnResult) : unit =
      match (exp, act.ToOption()) with
      | (Some (toFind, expFile), Some (span, actFile)) -> printfn "%s" "Result received, as expected; checking."
                                                          Assert.AreEqual (expFile, Path.GetFileName actFile)
                                                          printfn "%s" "Filename matches expected."
                                                          let file = OpenFile (proj, actFile)
                                                          let line = span.iStartLine |> ((+) 1) |> GetLineNumber file // need to adjust line number here
                                                          Assert.AreEqual (toFind, line.Substring (span.iStartIndex, toFind.Length))
                                                          printfn "%s" "Identifier at cursor matches expected."
      | (None,                   None)                 -> printfn "%s" "No result received, as expected." // sometimes we may expect GotoDefinition to fail, e.g., when the cursor isn't placed on a valid position (i.e., over an identifier, and, maybe, a constant if we decide to support that)
      | (Some _,                 None)                 -> Assert.Fail("No result received, but one was expected!") // distinguish this and the following case to give insight in case of failure
      | (None,                   Some _)               -> Assert.Fail("Result received, but none was expected!")

    /// exp = (<expected line>, <expected identifier>) option
    member this.GotoDefinitionTestWithSimpleFile (startLoc : string)(exp : (string * string) option) : unit =
        this.SolutionGotoDefinitionTestWithSimpleFile startLoc exp 

    [<Test>]
    member this.``GotoDefinition.OverloadResolution``() =
        let lines =
          [ "type D() ="
            "   override this.#3#ToString() = System.String.Empty"
            "   member this.#4#ToString(s : string) = ()"
            ""
            "   member this.#1#Foo() = ()"
            "   member this.#2#Foo(x) = ()"
            ""
            "let d = new D()"
            "d.Foo$1$()"
            "d.Foo$2$(1)"
            "d.ToString$3$()"
            "d.ToString$4$(\"aaa\") "
          ]
        this.GotoDefinitionTestWithMarkup lines
    [<Test>]
    member this.``GotoDefinition.OverloadResolutionForProperties``() =
        let lines = [ "type D() ="
                      "  member this.#1##2#Foo"
                      "    with get(i:int) = 1"
                      "    and set (i:int) v = ()"
                      ""
                      "  member this.#3##4#Foo"
                      "    with get (s:string) = 1"
                      "    and  set (s:string) v = ()"
                      ""
                      "D().$1$Foo 1"
                      "D().$2$Foo 1 <- 2"
                      "D().$3$Foo \"abc\""
                      "D().$4$Foo \"abc\" <- 2"
            ]
        this.GotoDefinitionTestWithMarkup lines

    [<Test>]
    member this.``GotoDefinition.OverloadResolutionWithOverrides``() =
        let lines =
          [ "[<AbstractClass>]"
            "type Base<'T>() ="
            "   member this.#2#Method() = ()"
            "   abstract Method : 'T -> unit"
            ""
            "type Derived() ="
            "   inherit Base<int>()"
            ""
            "   override this.#1#Method (i:int) = ()"
            ""
            "let d = new Derived()"
            "d.$1$Method 12"
            "d.$2$Method()"
          ]
        this.GotoDefinitionTestWithMarkup lines

    [<Test>]
    member this.``GotoDefinition.OverloadResolutionStatics``() =
        let lines =
          [   "type T ="
              "   static member #1#Foo(i : int) = ()"
              "   static member #2#Foo(s : string) = ()"
              ""
              "T.$1$Foo 1"
              "T.$2$Foo \"abc\""
          ]
        this.GotoDefinitionTestWithMarkup lines

    [<Test>]
    member this.``GotoDefinition.Constructors``() =
        let lines =
          [   "type #1a##1b##1c##1d#B() ="
              "  #2a##2b##2c##2d#new(i : int) = B()"
              "  #3a##3b##3c##3d#new(s : string) = B()"
              ""
              "B()"
              "B(1)"
              "B(\"abc\")"
              ""
              "new $1b$B()"
              "new $2b$B(1)"
              "new $3b$B(\"abc\")"
              ""
              "type D1() ="
              "    inherit $1c$B()"
              ""
              "type D2() ="
              "    inherit $2c$B(1)"

              "type D3() ="
              "    inherit $3c$B(\"abc\")"
              ""
              "let o1 = { new $1d$B() with"
              "             override this.ToString() = \"\""
              "         }"
              "let o2 = { new $2d$B(1) with"
              "             override this.ToString() = \"\""
              "        }"
              "let o2 = { new $3d$B(\"aaa\") with"
              "             override this.ToString() = \"\""
              "         }"

          ]
        this.GotoDefinitionTestWithMarkup lines

    member internal this.GotoDefinitionTestWithMarkup (lines : string list) =      
      let origins = Dictionary<string, int*int>()
      let targets = Dictionary<string, int*int>()
      let lines =
        [   let mutable lineNo = 0
            for l in lines do
                let builder = new System.Text.StringBuilder(l)
                let mutable cont = true
                while cont do
                    let s = builder.ToString()
                    let index = s.IndexOfAny([|'$';'#'|])
                    if index < 0 then
                        cont <- false
                    else
                        let c = s.[index]
                        let nextIndex = s.IndexOf(c, index+1) 
                        let marker = s.Substring(index+1, nextIndex - (index+1))
                        if c = '$' then 
                            origins.Add(marker, (lineNo+1,index+1)) // caret positions are 1-based, but...
                        else 
                            targets.Add(marker, (lineNo,index)) // ...spans are 0-based. Argh. Thank you, Salsa!
                        builder.Remove(index, nextIndex - index + 1) |> ignore
                yield builder.ToString()
                lineNo <- lineNo + 1
        ]

      let (_, _, file) = this.CreateSingleFileProject(lines)

      for KeyValue(marker,(line,col)) in origins do
          MoveCursorTo(file, line, col)
          let res = GotoDefinitionAtCursor file
          match res.ToOption() with
          |   None -> Assert.IsFalse(targets.ContainsKey(marker), sprintf "%s: definition not found " marker)
          |   Some (span,text) ->
                  match targets.TryGetValue(marker) with
                  |   false, _ ->  Assert.Fail(sprintf "%s: unexpected definition found" marker)
                  |   true, (line1, col1) -> 
                          Assert.IsTrue(span.iStartIndex = col1 && span.iStartLine = line1, 
                                sprintf "%s: wrong definition found expected %d %d but found %d %d %s" marker line1 col1 span.iStartLine span.iStartIndex text )

    
    /// exp = (<expected line>, <expected identifier>) option
    member internal this.SolutionGotoDefinitionTestWithSimpleFile (startLoc : string)(exp : (string * string) option) : unit =
      let lines = 
        [ "#light"
          "let _ = 3"
          "let _ = \"hi\""
          "let _ = 2 + 3"
          "let _ = []"
          "let _ = ()"
          "let _ = null"
          "let _ ="
          "  let x = () (*loc-2*)"
          "  x (*loc-1*)"
          "let _ ="
          "  let x = () (*loc-5*)"
          "  let x = () (*loc-3*)"
          "  x (*loc-4*)"
          "let _ ="
          "  let x = () (*loc-7*)"
          "  let x ="
          "    x (*loc-6*)"
          "  ()"
          "let _ ="
          "  let     x = ()"
          "  let rec x = (*loc-9*)"
          "    fun y -> (*loc-10*)"
          "      x y (*loc-8*)"
          "  ()"
          "let _ ="
          "  let (+) x _ = x (*loc-12*)"
          "  2 + 3 (*loc-11*)"
          "type Zero = (*loc-13*)"
          "let foo (_ : Zero) : 'a = failwith \"hi\" (*loc-14*)"
          "type One = (*loc-16*)"
          "  One (*loc-15*)"
          "let f (x : One) = (*loc-17*)"
          "  One (*loc-18*)"
          "type Nat = (*loc-19*)"
          "  | Suc of Nat (*loc-20*)"
          "  | Zro (*loc-21*)"
          "let rec plus m n = (*loc-23*)"
          "  match m with (*loc-22*)"
          "  | Zro   -> (*loc-24*)"
          "      n"
          "  | Suc m -> (*loc-25*)"
          "      Suc (plus m n) (*loc-26*)"
          "type MyRec = (*loc-27*)"
          "  { myX : int (*loc-28*)"
          "    myY : int (*loc-29*)"
          "  }"
          "let rDefault ="
          "  { myX = 2 (*loc-30*)"
          "    myY = 3 (*loc-31*)"
          "  }"
          "let _ = { rDefault with myX = 7 } (*loc-32*)"
          "let _ ="
          "  let a = 2"
          "  let id (x : 'a) (*loc-33*)"
          "    : 'a = x (*loc-34*)"
          "  ()"
          "let _ ="
          "  let foo          = ()"
          "  let f (_ as foo) = (*loc-35*)"
          "    foo (*loc-36*)"
          "  ()"
          "let _ ="
          "  let foo          = ()"
          "  let f (x as foo) = foo"
          "  ()"
          "let _ ="
          "  fun x (*loc-37*)"
          "      x -> (*loc-38*)"
          "    x (*loc-39*)"
          "let _ ="
          "  let f = () (*loc-40*)"
          "  let f = (*loc-41*)"
          "    function f -> (*loc-42*)"
          "      f (*loc-43*)"
          "  ()"
          "let _ ="
          "  let f x ="
          "    match x with"
          "    | Suc x (*loc-44*)"
          "    | x (*loc-45*) -> "
          "        x"
          "  ()"
          "let _ ="
          "  let f x ="
          "    match x with"
          "    | Suc y & z -> (*loc-47*)"
          "        y (*loc-46*)"
          "  ()"
          "let _ ="
          "  let f xs ="
          "    match xs with"
          "    | x :: xs -> (*loc-49*)"
          "        x (*loc-48*)"
          "    | _       -> []"
          "  ()"
          "let _ ="
          "  let f x ="
          "    match x with"
          "    | (y : int, z) -> (*loc-51*)"
          "         y (*loc-50*)"
          "  ()"
          "let _ ="
          "  let f xs ="
          "    match xs with"
          "    | x :: xs (*loc-54*)"
          "      when xs <> [] -> (*loc-52*)"
          "        x :: xs (*loc-53*)"
          "  ()"
          "module Too = (*loc-55*)"
          "  let foo = 0 (*loc-56*)"
          "module Bar ="
          "  open Too (*loc-57*)"
          "let _ = Too.foo (*loc-58*)"
          "module Overlap ="
          "  type Parity = Even | Odd"
          "  let (|Even|Odd|) x = (*loc-59*)"
          "    if x % 0 = 0"
          "       then Even (*loc-60*)"
          "       else Odd"
          "  let foo (x : int) ="
          "    match x with"
          "    | Even -> 1 (*loc-61*)"
          "    | Odd  -> 0"
          "  let patval = (|Even|Odd|) (*loc-61b*)"
          "let _ ="
          "  op_Addition 2 2"
          "type Class () = (*loc-62*)"
          "  member c.Method () = () (*loc-63*)"
          "  static member Foo () = () (*loc-64*)"
          "let _ ="
          "  let c = Class () (*loc-65*)"
          "  c.Method () (*loc-66*)"
          "  Class.Foo () (*loc-67*)"
          "type Class' () ="
          "  member c.Method  () = c.Method () (*loc-68*)"
          "  member c.Method1 () = c.Method2 () (*loc-69*)"
          "  member c.Method2 () = c.Method1 () (*loc-70*)"
          "  member c.Method3  () ="
          "    let c = Class ()"
          "    c.Method () (*loc-71*)"
          "type Colors = Red   = 1"
          "            | White = 2"
          "            | Blue  = 3"
          "let _ = Colors.Red"
          "let _ ="
          "  let x = 2"
          "  \"x(*loc-72*)\""
          "let _ ="
          "  let x = 2"
          "  \"this is a string"
          "    x(*loc-73*)"
          "  \""
          "let _ ="
          "  let rec ``let`` = (*loc-74*)"
          "    function 0 -> 1"
          "           | n -> n * ``let`` (n - 1) (*loc-75*)"
          "let id77 = 0"  
          "type C ="
          "  val id77 (*loc-77*) : int"
        ]
      this.SolutionGotoDefinitionTestWithLines lines startLoc exp




    member internal this.SolutionGotoDefinitionTestWithLines lines (startLoc : string)(exp : (string * string) option) : unit =        
      // The test itself
      let (_, _, file) = this.CreateSingleFileProject(lines)
      let fnm = 
        GetNameOfOpenFile file
        |> Path.GetFileName
      MoveCursorToStartOfMarker (file, startLoc)
      let res = GotoDefinitionAtCursor file |> this.GotoDefinitionFixupFilename
      match exp with
      | None              -> this.GotoDefinitionCheckResultAgainst None                     file res
      | Some (endLoc, id) -> this.GotoDefinitionCheckResultAgainst (Some (id, endLoc, fnm)) file res

    /// exp = (<identifier where cursor ought to be>, <name of file that's expected>) option
    member this.GotoDefinitionTestWithLib (startLoc : string)(exp : (string * string) option) : unit =
      let lines = [ "let _ = List.map (*loc-1*)" ]
      let (_, proj, file) = this.CreateSingleFileProject(lines)

      MoveCursorToStartOfMarker (file, startLoc)
      let res = GotoDefinitionAtCursor file
      this.GotoDefinitionCheckResultAgainstAnotherFile proj exp res

    member this.GotoDefinitionFixupFilename (x : GotoDefnResult) : GotoDefnResult =
      if x.Success then 
        GotoDefnResult.MakeSuccess(Path.GetFileName x.Url, x.Span) 
      else 
        GotoDefnResult.MakeError(x.ErrorDescription)

    // the format of the comments for each test displays the desired behaviour,
    // where `$` and `#` indicate the initial and final cursor positions,
    // respectively; if the two are the same then the `#` is omitted

    // the `loc-<number>` comments in the source used for the tests are to
    // ensure that we've found the correct position (i.e., these must be unique
    // in any given test source file)

    [<Test>]
    member this.``GotoDefinition.InheritedMembers``() =
        let lines =
            [ "[<AbstractClass>]"
              "type Foo() ="
              "   abstract Method : unit -> unit"
              "   abstract Property : int"
              "type Bar() ="
              "   inherit Foo()"
              "   override this.Method () = ()"
              "   override this.Property = 1"
              "let b = Bar()"
              "b.Method(*loc-1*)()"
              "b.Property(*loc-2*)"
            ]
        this.SolutionGotoDefinitionTestWithLines lines  "Method(*loc-1*)" (Some("override this.Method () = ()","this.Method"))
        this.SolutionGotoDefinitionTestWithLines lines  "Property(*loc-2*)" (Some("override this.Property = 1","this.Property"))


    /// let #x = () in $x
    [<Test>]
    member public this.``GotoDefinition.InsideClass.Bug3176`` () =
      this.GotoDefinitionTestWithSimpleFile "id77 (*loc-77*)" (Some("val id77 (*loc-77*) : int", "id77"))

    /// let #x = () in $x
    [<Test>]
    member public this.``GotoDefinition.Simple.Binding.TrivialLetRHS`` () =
      this.GotoDefinitionTestWithSimpleFile "x (*loc-1*)" (Some("let x = () (*loc-2*)", "x"))

    /// let #x = () in x$
    [<Test>]
    member public this.``GotoDefinition.Simple.Binding.TrivialLetRHSToRight`` () =
      this.GotoDefinitionTestWithSimpleFile " (*loc-1*)" (Some("let x = () (*loc-2*)", "x"))

    /// let $x = () in x
    [<Test>]
    member public this.``GotoDefinition.Simple.Binding.TrivialLetLHS`` () =
      this.GotoDefinitionTestWithSimpleFile "x = () (*loc-2*)" (Some("let x = () (*loc-2*)", "x"))

    /// let x = () in let #x = () in $x
    [<Test>]
    member public this.``GotoDefinition.Simple.Binding.NestedLetWithSameNameRHS`` () =
      this.GotoDefinitionTestWithSimpleFile "x (*loc-4*)" (Some("let x = () (*loc-3*)", "x"))

    /// let x = () in let $x = () in x
    [<Test>]
    member public this.``GotoDefinition.Simple.Binding.NestedLetWithSameNameLHSInner`` () =
      this.GotoDefinitionTestWithSimpleFile "x = () (*loc-3*)" (Some("let x = () (*loc-3*)", "x"))

    /// let $x = () in let x = () in x
    [<Test>]
    member public this.``GotoDefinition.Simple.Binding.NestedLetWithSameNameLHSOuter`` () =
      this.GotoDefinitionTestWithSimpleFile "x = () (*loc-5*)" (Some("let x = () (*loc-5*)", "x"))

    /// let #x = () in let x = $x in ()
    [<Test>]
    member public this.``GotoDefinition.Simple.Binding.NestedLetWithXIsX`` () =
      this.GotoDefinitionTestWithSimpleFile "x (*loc-6*)" (Some("let x = () (*loc-7*)", "x"))

    /// let x = () in let rec #x = fun y -> $x y in ()
    [<Test>]
    member public this.``GotoDefinition.Simple.Binding.NestedLetWithXRec`` () =
      this.GotoDefinitionTestWithSimpleFile "x y (*loc-8*)" (Some("let rec x = (*loc-9*)", "x"))

    /// let x = () in let rec x = fun #y -> x $y in ()
    [<Test>]
    member public this.``GotoDefinition.Simple.Binding.NestedLetWithXRecParam`` () =
      this.GotoDefinitionTestWithSimpleFile "y (*loc-8*)" (Some("fun y -> (*loc-10*)", "y"))

    /// let #(+) x _ = x in 2 $+ 3
    [<Test>]
    [<Ignore "Bug 2514 filed.">]
    member public this.``GotoDefinition.Simple.Binding.Operator`` () =
      this.GotoDefinitionTestWithSimpleFile "+ 3 (*loc-11*)" (Some("let (+) x _ = x (*loc-2*)", "+"))

    /// type #Zero =
    /// let f (_ : $Zero) = 0
    [<Test>]
    member public this.``GotoDefinition.Simple.Datatype.NullType`` () =
      this.GotoDefinitionTestWithSimpleFile "Zero) : 'a = failwith \"hi\" (*loc-14*)" (Some("type Zero = (*loc-13*)", "Zero"))

    /// type One = $One
    [<Test>]
    member public this.``GotoDefinition.Simple.Datatype.UnitTypeConsDef`` () =
      this.GotoDefinitionTestWithSimpleFile "One (*loc-15*)" (Some("One (*loc-15*)", "One"))

    /// type $One = One
    [<Test>]
    member public this.``GotoDefinition.Simple.Datatype.UnitTypeTypenameDef`` () =
      this.GotoDefinitionTestWithSimpleFile "One = (*loc-16*)" (Some("type One = (*loc-16*)", "One"))

    /// type One = #One
    /// let f (_ : One) = $One
    [<Test>]
    member public this.``GotoDefinition.Simple.Datatype.UnitTypeCons`` () =
      this.GotoDefinitionTestWithSimpleFile "One (*loc-18*)" (Some("One (*loc-15*)", "One"))

    /// type #One = One
    /// let f (_ : $One) = One
    [<Test>]
    member public this.``GotoDefinition.Simple.Datatype.UnitTypeTypename`` () =
      this.GotoDefinitionTestWithSimpleFile "One) = (*loc-17*)" (Some("type One = (*loc-16*)", "One"))

    /// type $Nat = Suc of Nat | Zro
    [<Test>]
    member public this.``GotoDefinition.Simple.Datatype.NatTypeTypenameDef`` () =
      this.GotoDefinitionTestWithSimpleFile "Nat = (*loc-19*)" (Some("type Nat = (*loc-19*)", "Nat"))

    /// type #Nat = Suc of $Nat | Zro
    [<Test>]
    member public this.``GotoDefinition.Simple.Datatype.NatTypeConsArg`` () =
      this.GotoDefinitionTestWithSimpleFile "Nat (*loc-20*)" (Some("type Nat = (*loc-19*)", "Nat"))

    /// type Nat = Suc of Nat | #Zro
    /// fun m -> match m with | $Zro -> () | _ -> ()
    [<Test>]
    member public this.``GotoDefinition.Simple.Datatype.NatPatZro`` () =
      this.GotoDefinitionTestWithSimpleFile "Zro   -> (*loc-24*)" (Some("| Zro (*loc-21*)", "Zro"))

    /// type Nat = $Suc of Nat | Zro
    /// fun m -> match m with | Zro -> () | $Suc _ -> ()
    [<Test>]
    member public this.``GotoDefinition.Simple.Datatype.NatPatSuc`` () =
      this.GotoDefinitionTestWithSimpleFile "Suc m -> (*loc-25*)" (Some("| Suc of Nat (*loc-20*)", "Suc"))

    /// let rec plus m n = match m with | Zro -> n | Suc #m -> Suc (plus $m n)
    [<Test>]
    member public this.``GotoDefinition.Simple.Datatype.NatPatSucVarUse`` () =
      this.GotoDefinitionTestWithSimpleFile "m n) (*loc-26*)" (Some("| Suc m -> (*loc-25*)", "m"))

    /// let rec plus m n = match m with | Zro -> n | Suc #m -> Suc (plus $m n)
    [<Test>]
    member public this.``GotoDefinition.Simple.Datatype.NatPatSucOuterVarUse`` () =
      this.GotoDefinitionTestWithSimpleFile "n) (*loc-26*)" (Some("let rec plus m n = (*loc-23*)", "n"))

    /// type $MyRec = { myX : int ; myY : int }
    [<Test>]
    member public this.``GotoDefinition.Simple.Datatype.RecordTypenameDef`` () =
      this.GotoDefinitionTestWithSimpleFile "MyRec = (*loc-27*)" (Some("type MyRec = (*loc-27*)", "MyRec"))

    /// type MyRec = { $myX : int ; myY : int }
    [<Test>]
    member public this.``GotoDefinition.Simple.Datatype.RecordField1Def`` () =
      this.GotoDefinitionTestWithSimpleFile "myX : int (*loc-28*)" (Some("{ myX : int (*loc-28*)", "myX"))

    /// type MyRec = { myX : int ; $myY : int }
    [<Test>]
    member public this.``GotoDefinition.Simple.Datatype.RecordField2Def`` () =
      this.GotoDefinitionTestWithSimpleFile "myY : int (*loc-29*)" (Some("myY : int (*loc-29*)", "myY"))

    /// type MyRec = { #myX : int ; myY : int }
    /// let rDefault = { $myX = 2 ; myY = 3 }
    [<Test>]
    member public this.``GotoDefinition.Simple.Datatype.RecordField1Use`` () =
      this.GotoDefinitionTestWithSimpleFile "myX = 2 (*loc-30*)" (Some("{ myX : int (*loc-28*)", "myX"))

    /// type MyRec = { myX : int ; #myY : int }
    /// let rDefault = { myX = 2 ; $myY = 3 }
    [<Test>]
    member public this.``GotoDefinition.Simple.Datatype.RecordField2Use`` () =
      this.GotoDefinitionTestWithSimpleFile "myY = 3 (*loc-31*)" (Some("myY : int (*loc-29*)", "myY"))

    /// type MyRec = { #myX : int ; myY : int }
    /// let rDefault = { myX = 2 ; myY = 3 }
    /// let _ = { rDefault with $myX = 7 }
    [<Test>]
    member public this.``GotoDefinition.Simple.Datatype.RecordField1UseInWith`` () =
      this.GotoDefinitionTestWithSimpleFile "myX = 7 } (*loc-32*)" (Some("{ myX : int (*loc-28*)", "myX"))


    /// let a = () in let id (x : '$a) : 'a = x
    [<Test>]
    member public this.``GotoDefinition.Simple.Polymorph.Leftmost`` () =
      this.GotoDefinitionTestWithSimpleFile "a) (*loc-33*)" (Some("let id (x : 'a) (*loc-33*)", "'a"))

    /// let a = () in let id (x : 'a) : '$a = x
    [<Test>]
    member public this.``GotoDefinition.Simple.Polymorph.NotLeftmost`` () =
      this.GotoDefinitionTestWithSimpleFile "a = x (*loc-34*)" (Some("let id (x : 'a) (*loc-33*)", "'a"))

    /// let foo = () in let f (_ as $foo) = foo in ()
    [<Test>]
    member public this.``GotoDefinition.Simple.Tricky.AsPatLHS`` () =
      this.GotoDefinitionTestWithSimpleFile "foo) = (*loc-35*)" (Some("let f (_ as foo) = (*loc-35*)", "foo"))

    /// let foo = () in let f (_ as #foo) = $foo in ()
    [<Test>]
    member public this.``GotoDefinition.Simple.Tricky.AsPatRHS`` () =
      this.GotoDefinitionTestWithSimpleFile "foo (*loc-36*)" (Some("let f (_ as foo) = (*loc-35*)", "foo"))

    /// fun $x x -> x
    [<Test>]
    member public this.``GotoDefinition.Simple.Tricky.LambdaMultBind1`` () =
      this.GotoDefinitionTestWithSimpleFile "x (*loc-37*)" (Some("fun x (*loc-37*)", "x"))

    /// fun x $x -> x
    [<Test>]
    member public this.``GotoDefinition.Simple.Tricky.LambdaMultBind2`` () =
      this.GotoDefinitionTestWithSimpleFile "x -> (*loc-38*)" (Some("x -> (*loc-38*)", "x"))

    /// fun x $x -> x
    [<Test>]
    member public this.``GotoDefinition.Simple.Tricky.LambdaMultBindBody`` () =
      this.GotoDefinitionTestWithSimpleFile "x (*loc-39*)" (Some("x -> (*loc-38*)", "x"))

    /// let f = () in let $f = function f -> f in ()
    [<Test>]
    member public this.``GotoDefinition.Simple.Tricky.LotsOfFsFunc`` () =
      this.GotoDefinitionTestWithSimpleFile "f = (*loc-41*)" (Some("let f = (*loc-41*)", "f"))

    /// let f = () in let f = function $f -> f in ()
    [<Test>]
    member public this.``GotoDefinition.Simple.Tricky.LotsOfFsPat`` () =
      this.GotoDefinitionTestWithSimpleFile "f -> (*loc-42*)" (Some("function f -> (*loc-42*)", "f"))

    /// let f = () in let f = function #f -> $f in ()
    [<Test>]
    member public this.``GotoDefinition.Simple.Tricky.LotsOfFsUse`` () =
      this.GotoDefinitionTestWithSimpleFile "f (*loc-43*)" (Some("function f -> (*loc-42*)", "f"))

    /// let f x = match x with | Suc $x | x -> x
    [<Test>]
    member public this.``GotoDefinition.Simple.Tricky.OrPatLeft`` () =
      this.GotoDefinitionTestWithSimpleFile "x (*loc-44*)" (Some("| Suc x (*loc-44*)", "x"))

    /// let f x = match x with | Suc x | $x -> x
    [<Test >]
    member public this.``GotoDefinition.Simple.Tricky.OrPatRight`` () =
      this.GotoDefinitionTestWithSimpleFile "x (*loc-45*)" (Some("| Suc x (*loc-44*)", "x"))  // NOTE: or-patterns bind at first occurrence of the variable

    /// let f x = match x with | Suc #y & z -> $y
    [<Test>]
    member public this.``GotoDefinition.Simple.Tricky.AndPat`` () =
      this.GotoDefinitionTestWithSimpleFile "y (*loc-46*)" (Some("| Suc y & z -> (*loc-47*)", "y"))

    /// let f xs = match xs with | #x :: xs -> $x
    [<Test>]
    member public this.``GotoDefinition.Simple.Tricky.ConsPat`` () =
      this.GotoDefinitionTestWithSimpleFile "x (*loc-48*)" (Some("| x :: xs -> (*loc-49*)", "x"))

    /// let f p = match p with (#y, z) -> $y
    [<Test>]
    member public this.``GotoDefinition.Simple.Tricky.PairPat`` () =
      this.GotoDefinitionTestWithSimpleFile "y (*loc-50*)" (Some("| (y : int, z) -> (*loc-51*)", "y"))

    /// fun xs -> match xs with x :: #xs when $xs <> [] -> x :: xs
    [<Test>]
    member public this.``GotoDefinition.Simple.Tricky.ConsPatWhenClauseInWhen`` () =
      this.GotoDefinitionTestWithSimpleFile "xs <> [] -> (*loc-52*)" (Some("| x :: xs (*loc-54*)", "xs"))

    /// fun xs -> match xs with #x :: xs when xs <> [] -> $x :: xs
    [<Test>]
    member public this.``GotoDefinition.Simple.Tricky.ConsPatWhenClauseInWhenRhsX`` () =
      this.GotoDefinitionTestWithSimpleFile "x :: xs (*loc-53*)" (Some("| x :: xs (*loc-54*)", "x"))

    /// fun xs -> match xs with x :: #xs when xs <> [] -> x :: $xs
    [<Test>]
    member public this.``GotoDefinition.Simple.Tricky.ConsPatWhenClauseInWhenRhsXs`` () =
      this.GotoDefinitionTestWithSimpleFile "xs (*loc-53*)" (Some("| x :: xs (*loc-54*)", "xs"))

    /// let x = "$x"
    [<Test>]
    member public this.``GotoDefinition.Simple.Tricky.InStringFails`` () =
      this.GotoDefinitionTestWithSimpleFile "x(*loc-72*)" None

    /// let x = "hello
    ///          $x
    ///         "
    [<Test>]
    member public this.``GotoDefinition.Simple.Tricky.InMultiLineStringFails`` () =
      this.GotoDefinitionTestWithSimpleFile "x(*loc-73*)" None

    [<Test>]
    member public this.``GotoDefinition.Simple.Tricky.QuotedKeyword`` () =
      this.GotoDefinitionTestWithSimpleFile "let`` = (*loc-74*)" (Some("let rec ``let`` = (*loc-74*)", "``let``"))

    /// module $Too = let foo = ()
    [<Test>]
    member public this.``GotoDefinition.Simple.Module.DefModname`` () =
      this.GotoDefinitionTestWithSimpleFile "Too = (*loc-55*)" (Some("module Too = (*loc-55*)", "Too"))

    /// module Too = $foo = ()
    [<Test>]
    member public this.``GotoDefinition.Simple.Module.DefMember`` () =
      this.GotoDefinitionTestWithSimpleFile "foo = 0 (*loc-56*)" (Some("let foo = 0 (*loc-56*)", "foo"))

    /// module #Too = foo = ()
    /// module Bar = open $Too
    [<Test>]
    member public this.``GotoDefinition.Simple.Module.Open`` () =
      this.GotoDefinitionTestWithSimpleFile "Too (*loc-57*)" (Some("module Too = (*loc-55*)", "Too"))

    /// module #Too = foo = ()
    /// $Too.foo
    [<Test>]
    member public this.``GotoDefinition.Simple.Module.QualifiedModule`` () =
      this.GotoDefinitionTestWithSimpleFile "Too.foo (*loc-58*)" (Some("module Too = (*loc-55*)", "Too"))

    /// module Too = #foo = ()
    /// Too.$foo
    [<Test>]
    member public this.``GotoDefinition.Simple.Module.QualifiedMember`` () =
      this.GotoDefinitionTestWithSimpleFile "foo (*loc-58*)" (Some("let foo = 0 (*loc-56*)", "foo"))

    /// type Parity = Even | Odd
    /// let (|$Even|Odd|) x = if x % 0 = 0 then Even else Odd
    [<Test>]
    member public this.``GotoDefinition.Simple.ActivePat.ConsDefLHS`` () =
      this.GotoDefinitionTestWithSimpleFile "Even|Odd|) x = (*loc-59*)" (Some("let (|Even|Odd|) x = (*loc-59*)", "|Even|Odd|"))

    /// type Parity = Even | Odd
    /// let (|#Even|Odd|) x = if x % 0 = 0 then $Even else Odd
    [<Test>]
    member public this.``GotoDefinition.Simple.ActivePat.ConsDefRhs`` () =
      this.GotoDefinitionTestWithSimpleFile "Even (*loc-60*)" (Some("let (|Even|Odd|) x = (*loc-59*)", "|Even|Odd|"))

    /// type Parity = Even | Odd
    /// let (|#Even|Odd|) x = if x % 0 = 0 then Even else Odd
    /// let foo x =
    ///   match x with
    ///   | $Even -> 1
    ///   | Odd  -> 0
    [<Test>]
    member public this.``GotoDefinition.Simple.ActivePat.PatUse`` () =
      this.GotoDefinitionTestWithSimpleFile "Even -> 1 (*loc-61*)" (Some("let (|Even|Odd|) x = (*loc-59*)", "|Even|Odd|"))

    /// let patval = (|Even|Odd|) (*loc-61b*)
    [<Test>]
    member public this.``GotoDefinition.Simple.ActivePat.PatUseValue`` () =
      this.GotoDefinitionTestWithSimpleFile "en|Odd|) (*loc-61b*)" (Some("let (|Even|Odd|) x = (*loc-59*)", "|Even|Odd|"))

    [<Test>]
    [<Ignore("This is currently broken for dev enlistments.")>]
    member public this.``GotoDefinition.Library.InitialTest`` () =
      this.GotoDefinitionTestWithLib "map (*loc-1*)" (Some("map", "lis.fs"))

    // ********** Tests of OO Stuff **********

    /// type #Class$ () =
    ///   member c.Method () = ()
    [<Test>]
    member public this.``GotoDefinition.ObjectOriented.ClassNameDef`` () =
      this.GotoDefinitionTestWithSimpleFile " () = (*loc-62*)" (Some("type Class () = (*loc-62*)", "Class"))

    /// type Class () =
    ///   member c.#Method$ () = ()
    [<Test>]
    member public this.``GotoDefinition.ObjectOriented.ILMethodDef`` () =
      this.GotoDefinitionTestWithSimpleFile " () = () (*loc-63*)" (Some("member c.Method () = () (*loc-63*)", "c.Method"))

    /// type Class () =
    ///   member #c$.Method () = ()
    [<Test>]
    member public this.``GotoDefinition.ObjectOriented.ThisDef`` () =
      this.GotoDefinitionTestWithSimpleFile ".Method () = () (*loc-63*)" (Some("member c.Method () = () (*loc-63*)", "c"))

    /// type Class () =
    ///   static member #Foo$ () = ()
    [<Test>]
    member public this.``GotoDefinition.ObjectOriented.StaticMethodDef`` () =
      this.GotoDefinitionTestWithSimpleFile " () = () (*loc-64*)" (Some("static member Foo () = () (*loc-64*)", "Foo"))

    /// type #Class () =
    ///   member Method () = ()
    /// let c = Class$ ()
    [<Test>]
    member public this.``GotoDefinition.ObjectOriented.ConstructorUse`` () =
      this.GotoDefinitionTestWithSimpleFile " () (*loc-65*)" (Some("type Class () = (*loc-62*)", "Class"))

    /// type Class () =
    ///   member #Method () = ()
    /// let c = Class ()
    /// c.Method$ ()
    [<Test>]
    member public this.``GotoDefinition.ObjectOriented.MethodInvocation`` () =
      this.GotoDefinitionTestWithSimpleFile " () (*loc-66*)" (Some("member c.Method () = () (*loc-63*)", "c.Method"))

    /// type Class () =
    ///   static member #Foo () = ()
    /// Class.Foo$ ()
    [<Test>]
    member public this.``GotoDefinition.ObjectOriented.StaticMethodInvocation`` () =
      this.GotoDefinitionTestWithSimpleFile " () (*loc-67*)" (Some("static member Foo () = () (*loc-64*)", "Foo"))

    /// type Class () =
    ///   member c.Method# () = c.Method$ ()
    [<Test>]
    member public this.``GotoDefinition.ObjectOriented.MethodSelfInvocation`` () =
      this.GotoDefinitionTestWithSimpleFile " () (*loc-68*)" (Some("member c.Method  () = c.Method () (*loc-68*)", "c.Method"))

    /// type Class () =
    ///   member c.Method1 ()  = c.Method2$ ()
    ///   member #c.Method2 () = c.Method1 ()
    [<Test>]
    member public this.``GotoDefinition.ObjectOriented.MethodToMethodForward`` () =
      this.GotoDefinitionTestWithSimpleFile " () (*loc-69*)" (Some("member c.Method2 () = c.Method1 () (*loc-70*)", "c.Method2"))

    /// type Class () =
    ///   member c.Method () = ()
    /// type Class' () =
    ///   member c.Method () =
    ///     let #c = Class ()
    ///     c$.Method ()
    [<Test>]
    member public this.``GotoDefinition.ObjectOriented.ShadowThis`` () =
      this.GotoDefinitionTestWithSimpleFile ".Method () (*loc-71*)" (Some("let c = Class ()", "c"))

    /// type Class () =
    ///   member #c.Method () = ()
    /// type Class' () =
    ///   member c.Method () =
    ///     let c = Class ()
    ///     c.Method$ ()
    [<Test>]
    member public this.``GotoDefinition.ObjectOriented.ShadowThisMethodInvocation`` () =
      this.GotoDefinitionTestWithSimpleFile " () (*loc-71*)" (Some("member c.Method () = () (*loc-63*)", "c.Method"))

    [<Test>]
    member this.``GotoDefinition.ObjectOriented.StructConstructor`` () =
      let lines = 
        [ "#light"
          "[<Struct>]"
          "type Astruct(x:int, y:int) ="
          "  [<DefaultValue()>]"
          "  val mutable a : int"
          "  new(a) = Astruct(a, a)" 
          "type AS = Astruct"
          "let a1 = Astruct(0)"
          "let b1 = Astruct(0, 1)"
          "let c1 = Astruct()"
          "let a2 = AS(0)"
          "let b2 = AS(0, 1)"
          "let c2 = AS()"
        ]
      
      let (_,_, file) = this.CreateSingleFileProject(lines)
      let checkGTD marker (line, col) =        
          MoveCursorToStartOfMarker (file, marker)
          let res = GotoDefinitionAtCursor file |> fun x -> x.ToOption() |>  Option.map (fun (res, _) -> res.iStartLine + 1, res.iStartIndex + 1)
          AssertEqual(Some(line, col), res)
      
      checkGTD "Astruct(0)" (6, 3)
      checkGTD "Astruct(0, 1)" (3, 6)
      checkGTD "Astruct()" (3, 6)
      checkGTD "AS(0)" (6, 3)
      checkGTD "AS(0, 1)" (3, 6)
      checkGTD "AS()" (3, 6)

    // ********** GetCompleteIdentifierIsland tests **********

    /// takes a string with a `$` representing the cursor position, gets the
    /// GotoDefinition identifier at that position and compares against expected
    /// result

    member this.GetCompleteIdTest tolerate (s : string)(exp : string option) : unit =
      let n = s.IndexOf '$'
      let s = s.Remove (n, 1)
      match (QuickParse.GetCompleteIdentifierIsland tolerate s n, exp) with
      | (Some (s1, _, _), Some s2) -> 
        printfn "%s" "Received result, as expected."
        Assert.AreEqual (s1, s2)
      | (None,         None)    -> 
        printfn "%s" "Received no result, as expected."
      | (Some _,       None)    -> 
        Assert.Fail("Received result, but none was expected!")
      | (None,         Some _)  -> 
        Assert.Fail("Expected result, but didn't receive one!")

    [<Test>]
    member public this.``GetCompleteIdTest.TrivialBefore`` () =
      for tolerate in [true;false] do
          this.GetCompleteIdTest tolerate "let $ThisIsAnIdentifier = ()" (Some "ThisIsAnIdentifier")

    [<Test>]
    member public this.``GetCompleteIdTest.TrivialMiddle`` () =
      for tolerate in [true;false] do
          this.GetCompleteIdTest tolerate "let This$IsAnIdentifier = ()" (Some "ThisIsAnIdentifier")

    [<Test>]
    member public this.``GetCompleteIdTest.TrivialEnd`` () =
      this.GetCompleteIdTest true "let ThisIsAnIdentifier$ = ()" (Some "ThisIsAnIdentifier")
      this.GetCompleteIdTest false "let ThisIsAnIdentifier$ = ()" None

    [<Test>]
    member public this.``GetCompleteIdTest.GetsUpToDot1`` () =
      for tolerate in [true;false] do
          this.GetCompleteIdTest tolerate "let ThisIsAnIdentifier = Te$st.Moo.Foo.bar" (Some "Test")

    [<Test>]
    member public this.``GetCompleteIdTest.GetsUpToDot2`` () =
      for tolerate in [true;false] do
          this.GetCompleteIdTest tolerate "let ThisIsAnIdentifier = Test.Mo$o.Foo.bar" (Some "Test.Moo")

    [<Test>]
    member public this.``GetCompleteIdTest.GetsUpToDot3`` () =
      for tolerate in [true;false] do
          this.GetCompleteIdTest tolerate "let ThisIsAnIdentifier = Test.Moo.Fo$o.bar" (Some "Test.Moo.Foo")

    [<Test>]
    member public this.``GetCompleteIdTest.GetsUpToDot4`` () =
      for tolerate in [true;false] do
          this.GetCompleteIdTest tolerate "let ThisIsAnIdentifier = Test.Moo.Foo.ba$r" (Some "Test.Moo.Foo.bar")

    [<Test>]
    member public this.``GetCompleteIdTest.GetsUpToDot5`` () =
      this.GetCompleteIdTest true "let ThisIsAnIdentifier = Test.Moo.Foo.bar$" (Some "Test.Moo.Foo.bar")
      this.GetCompleteIdTest false "let ThisIsAnIdentifier = Test.Moo.Foo.bar$" None

    [<Test>]
    member public this.``GetCompleteIdTest.GetOperator`` () =
      for tolerate in [true;false] do
          this.GetCompleteIdTest tolerate "let ThisIsAnIdentifier = 3 +$ 4" None


    [<Test>]
    member public this.``Identifier.IsConstructor.Bug2516``() =
        let fileContents = """
            module GotoDefinition
            type One(*Mark1*) = One
            let f (x : One(*Mark2*)) = 2"""
        let definitionCode = "type One(*Mark1*) = One"
        this.VerifyGoToDefnSuccessAtStartOfMarker(fileContents,"(*Mark1*)",definitionCode) 
        
    [<Test>]
    member public this.``Identifier.IsTypeName.Bug2516``() =
        let fileContents = """
            module GotoDefinition
            type One(*Mark1*) = One
            let f (x : One(*Mark2*)) = 2"""
        let definitionCode = "type One(*Mark1*) = One"
        this.VerifyGoToDefnSuccessAtStartOfMarker(fileContents,"(*Mark2*)",definitionCode)       
       
    [<Test>]
    member public this.``ModuleName.OnDefinitionSite.Bug2517``() =
        let fileContents = """
            namespace GotoDefinition
            module Foo(*Mark*) =
            let x = ()"""
        let definitionCode = "module Foo(*Mark*) ="
        this.VerifyGoToDefnSuccessAtStartOfMarker(fileContents,"(*Mark*)",definitionCode) 

    /// GotoDef on abbreviation
    [<Test>]
    member public this.``GotoDefinition.Abbreviation.Bug193064``() =
        let fileContents = """
            type X = int
            let f (x:X) = x(*Marker*) """
        let definitionCode = "let f (x:X) = x(*Marker*)"
        this.VerifyGoToDefnSuccessAtStartOfMarker(fileContents,"x(*Marker*)",definitionCode) 

    /// Verify the GotoDefinition on UoM yield does NOT jump out error dialog, 
    /// will do nothing in automation lab machine or GTD SI.fs on dev machine with enlistment.
    [<Test>]
    member public this.``GotoDefinition.UnitOfMeasure.Bug193064``() =
        let fileContents = """
            open Microsoft.FSharp.Data.UnitSystems.SI
            UnitSymbols.A(*Marker*)"""
        this.VerifyGoToDefnNoErrorDialogAtStartOfMarker(fileContents,"A(*Marker*)", "type A = ampere")


// Context project system
[<TestFixture>]
type UsingProjectSystem() = 
    inherit UsingMSBuild(VsOpts = LanguageServiceExtension.ProjectSystemTestFlavour)
