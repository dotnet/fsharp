// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Tests.LanguageService.Colorizer

open System
open NUnit.Framework
open Salsa.Salsa
open Salsa.VsOpsUtils
open UnitTests.TestLib.Salsa
open UnitTests.TestLib.Utils
open UnitTests.TestLib.LanguageService
open UnitTests.TestLib.ProjectSystem

// context msbuild
[<TestFixture>]
type UsingMSBuild()  = 
    inherit LanguageServiceBaseTests()

    //Marker At The End Helper Functions
    member private this.VerifyColorizerAtEndOfMarker(fileContents : string, marker : string, tokenType : TokenType) =
        let (solution, project, file) = this.CreateSingleFileProject(fileContents)
        MoveCursorToEndOfMarker(file, marker)
        AssertEqual(tokenType, GetTokenTypeAtCursor(file))

    //Marker At The Start Helper Function
    member private this.VerifyColorizerAtStartOfMarker(fileContents : string, marker : string, tokenType : TokenType) =
        let (solution, project,file) = this.CreateSingleFileProject(fileContents)
        MoveCursorToStartOfMarker(file, marker)
        AssertEqual(tokenType, GetTokenTypeAtCursor(file))

    [<Test>]
    member this.``Comment.SingleLine``() = 
        this.VerifyColorizerAtEndOfMarker(
            fileContents =  """
                let simplefunction x y = x + y // Test1SimpleComment""",
            marker = "// Test1", 
            tokenType = TokenType.Comment)
      
    [<Test>]
    member this.``Conment.SingleLine.MultiConments``() = 
        this.VerifyColorizerAtEndOfMarker(
            fileContents =  """
                let x = // Test2SimpleComment // 1""",
            marker = "// Test2", 
            tokenType = TokenType.Comment)

    [<Test>]
    member this.``Comment.MultiLine.AfterAnExpression``() = 
        this.VerifyColorizerAtEndOfMarker(
            fileContents = """
                let mutliLine x = 5(* Test1MultiLine
                     Test2MultiLine <@@asdf@@>
                Test3MultiLine*) + 1(*Test4*)""",
            marker = "Test1",
            tokenType = TokenType.Comment) 
            
    [<Test>]
    member this.``Comment.MultiLine.WithLineBreakAndATab``() = 
        this.VerifyColorizerAtEndOfMarker(
            fileContents = """
                let mutliLine x = 5(* Test1MultiLine
                     Test2MultiLine <@@asdf@@>
                Test3MultiLine*) + 1(*Test4*)
                """,
            marker = "Test2",
            tokenType = TokenType.Comment)  

    [<Test>]
    member this.``Comment.MultiLine.WithLineBreakAfterQuotExp``() = 
        this.VerifyColorizerAtEndOfMarker(
            fileContents = """
                let mutliLine x = 5(* Test1MultiLine
                     Test2MultiLine <@@asdf@@>
                Test3MultiLine*) + 1(*Test4*)
                """,
            marker = "Test3",
            tokenType = TokenType.Comment)  

    [<Test>]
    member this.``Comment.MultiLine.AfterANumber``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                let mutliLine x = 5(* Test1MultiLine
                     Test2MultiLine <@@asdf@@>
                Test3MultiLine*) + 1(*Test4*)
                """,
            marker = "1(*Test4*)",
            tokenType = TokenType.Number) 

    [<Test>]
    member this.``Comment.Nested.Nested01``() =
        this.VerifyColorizerAtEndOfMarker(
            fileContents = """
                (* L1Nesting
                    (* L2Nesting
                        (* L3 Nesting
                            let l3code = 3
                        *)
                        let l2code = 2
                    *)
                    let l1code = 1
                *)
                let l0code = 0
                """,
            marker = "let l3",
            tokenType = TokenType.Comment)

    [<Test>]
    member this.``Comment.Nested.Nested02``() = 
        this.VerifyColorizerAtEndOfMarker(
            fileContents = """
                (* L1Nesting
                    (* L2Nesting
                        (* L3 Nesting
                            let l3code = 3
                        *)
                        let l2code = 2
                    *)
                    let l1code = 1
                *)
                let l0code = 0
                """,
            marker = "let l2",
            tokenType = TokenType.Comment)
            
    [<Test>]
    member this.``Comment.Nested.Nested03``() = 
        this.VerifyColorizerAtEndOfMarker(
            fileContents = """
                (* L1Nesting
                    (* L2Nesting
                        (* L3 Nesting
                            let l3code = 3
                        *)
                        let l2code = 2
                    *)
                    let l1code = 1
                *)
                let l0code = 0
                """,
            marker = "let l1",
            tokenType = TokenType.Comment)   
            
    [<Test>]
    member this.``Comment.Nested.IdentAfterNestedComments``() = 
        this.VerifyColorizerAtEndOfMarker(
            fileContents = """
                (* L1Nesting
                    (* L2Nesting
                        (* L3 Nesting
                            let l3code = 3
                        *)
                        let l2code = 2
                    *)
                    let l1code = 1
                *)
                let l0code = 0
                """,
            marker = "let l0",
            tokenType = TokenType.Identifier)                                         

    [<Test>]
    member this.``Comment.CommentInString``() = 
        this.VerifyColorizerAtEndOfMarker(
            fileContents = """
                let commentsInString = "...(*test1_comment_in_string_literal*)..."
                )""",
            marker = "test1",
            tokenType = TokenType.String) 
  
    [<Test>]
    member this.``Comment.StringInComment``() = 
        this.VerifyColorizerAtEndOfMarker(
            fileContents = """
                (*
                let commentsInString2 = "...*)test2_stringliteral_in_comment(*..."
                *)""",
            marker = "test2",
            tokenType = TokenType.Comment)

    [<Test>]
    member this.``Comment.Unterminated.KeywordBeforeComment``() = 
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                type IPeekPoke = interface(*ML Comment Start
                  abstract member Peek: unit -> int
                  abstract member Poke: int -> unit
                end
                """,
            marker = "face(*ML Comment Start",
            tokenType = TokenType.Keyword) 
            
    [<Test>]
    member this.``Comment.Unterminated.KeywordInComment``() = 
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                type IPeekPoke = interface(*ML Comment Start
                  abstract member Peek: unit -> int
                  abstract member Poke: int -> unit
                end

                type wodget = class
                  val mutable state: int 
                  interface IPeekPoke with(*Few Lines Later2*)
                    member x.Poke(n) = x.state <- x.state + n
                    member x.Peek() = x.state 
                  end
                end(*Few Lines Later3*)""",
            marker = "with(*Few Lines Later2*)",
            tokenType = TokenType.Comment)  
            
    [<Test>]
    member this.``Comment.Unterminated.NestedComments``() = 
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                type IPeekPoke = interface(*ML Comment Start
                  abstract member Peek: unit -> int
                  abstract member Poke: int -> unit
                end

                type wodget = class
                  val mutable state: int 
                  interface IPeekPoke with(*Few Lines Later2*)
                    member x.Poke(n) = x.state <- x.state + n
                    member x.Peek() = x.state 
                  end
                  member x.HasBeenPoked = (x.state <> 0)
                  new() = { state = 0 }
                end(*Few Lines Later3*)""",
            marker = "nd(*Few Lines Later3*)",
            tokenType = TokenType.Comment) 

    [<Test>]
    member this.``String.AtEnd``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                let stringone = "simple string test"(*Simple String*) """, 
            marker = """est"(*Simple String*)""", tokenType = TokenType.String)

    [<Test>]
    member this.``String.MultiLines``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                let stringtwo = "simple test(*MultiLine - First*)
                                string test"(*MultiLine - Second*)""",
            marker = "st(*MultiLine - First*)",
            tokenType = TokenType.String) 
            
    [<Test>]
    member this.``String.MultiLines.LineBreak``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                let stringtwo = "simple test(*MultiLine - First*)
                                string test"(*MultiLine - Second*) """,
            marker =  "\"(*MultiLine - Second*) ",
            tokenType = TokenType.String)
            
    [<Test>]
    member this.``String.Literal``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """               
                let stringthree = @"literal test"(*Literal String*)""",
                            marker = """st"(*Literal String*)""",
            tokenType = TokenType.String)                                   
            
    [<Test>]
    member this.``ByteString.AtEnd``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                let bytestringone = "abcdefg"B(*Byte String*)""", 
            marker = "B(*Byte String*)", tokenType = TokenType.String)

    [<Test>]
    member this.``ByteString.MultiLines``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                let bytestringtwo = "simple(*MultiLineB - First*)
                                    string"B(*MultiLineB - Second*)""",
            marker =  """ing"B(*MultiLineB - Second*)""",
            tokenType = TokenType.String)
             
    [<Test>]
    member this.``ByteString.Literal``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """                                  
                let bytestringthree = @"literal"B(*Literal Byte*)""",
            marker = """al"B(*Literal Byte*)""",
            tokenType = TokenType.String)             

    [<Test>]
    member this.``EscapedIdentifier.word``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """let ``this is an escaped identifier 123ASDF@#$"`` = 4""",
            marker = "`this",
            tokenType = TokenType.Identifier)

    [<Test>]
    member this.``EscapedIdentifier.SpecialChar``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """let ``this is an escaped identifier 123ASDF@#$"`` = 4""",
            marker = "3ASDF@#",
            tokenType = TokenType.Identifier)

    [<Test>]
    member this.``EscapedIdentifier.EscapeChar``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """let ``this is an escaped identifier 123ASDF@#$"`` = 4""",
            marker = "\"``",
            tokenType = TokenType.Identifier)

    /// Regression for 3609 - Colorizer: __SOURCE__ and others colorized as a string
    [<Test>]
    member this.``PredefinedIdentifier.SOURCE_DIRECTORY``() = 
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                let x = __SOURCE_DIRECTORY__(*Test1*)""",
            marker = "__(*Test1*)",
            tokenType = TokenType.Keyword)

    [<Test>]
    member this.``PredefinedIdentifier.SOURCE_FILE``() = 
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                let y = __SOURCE_FILE__(*Test2*))""",
            marker = "__(*Test2*)",
            tokenType = TokenType.Keyword)

    [<Test>]
    member this.``PredefinedIdentifier.LINE``() = 
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                let z = __LINE__(*Test3*)""",
            marker = "__(*Test3*)",
            tokenType = TokenType.Keyword)

    // Regression Test for FSB 3566, F# colorizer does not respect numbers 
    [<Test>]
    member this.``Number.InAnExpression``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """let f x = x + 9""",
            marker = "9",
            tokenType = TokenType.Number)
           
    // Regression Test for FSB 1778 - Colorization seems to be confused after parsing a comment that contains a verbatim string that contains a \
    [<Test>]
    member this.``Number.AfterCommentWithBackSlash``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """let f (* @"\\" *)x = x + 19(*Marker1*)""",
            marker = "9(*Marker1*)",
            tokenType = TokenType.Number)
    
    // Regression Test for FSharp1.0:2539 -- lexing @"" strings inside (* *) comments?
    [<Test>]
    member this.``Keyword.AfterCommentWithLexingStrings``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                (*
                let x = @"\\"
                *)

                let(*Marker1*) y = 1
                """,
            marker = "t(*Marker1*)",
            tokenType = TokenType.Keyword)
    
    // Regression Test for FSB 1380 - Language Service colorizes anything followed by a bang as a keyword
    [<Test>]
    member this.``Keyword.LetBang``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                let seqExpr = 
                    seq {
                        let! x = [1 .. 10](*Marker1*)
                        yield! x(*Marker2*)
                        do! - = ()(*Marker3*)
                    }""",
            marker = "! x = [1 .. 10](*Marker1*)",
            tokenType = TokenType.Keyword)

    [<Test>]
    member this.``Keyword.Yield``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                let seqExpr = 
                    seq {
                        let! x = [1 .. 10](*Marker1*)
                        yield! x(*Marker2*)
                        do! - = ()(*Marker3*)
                    }""",
            marker = "! x(*Marker2*)",
            tokenType = TokenType.Keyword)

    [<Test>]
    member this.``Keyword.Do``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                let seqExpr = 
                    seq {
                        let! x = [1 .. 10](*Marker1*)
                        yield! x(*Marker2*)
                        do! - = ()(*Marker3*)
                    }""",
            marker = "! - = ()(*Marker3*)",
            tokenType = TokenType.Keyword)
    
    [<Test>]
    member this.``Keyword.Invalid.Bang``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                let seqExpr = 
                    seq {
                        foo! = true(*Marker1*)
                    }""",
            marker = "! = true(*Marker1*)",
            tokenType = TokenType.Identifier)
    
    [<Test>]
    [<Category("TypeProvider")>]
    [<Category("TypeProvider.StaticParameters")>]
    //This test case Verify that the color of const is the keyword color
    member this.``TypeProvider.StaticParameters.Keyword.const``() =                 
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                     type foo = N1.T< const(*Marker1*) "Hello World",2>""",
            marker = "t(*Marker1*)",
            tokenType = TokenType.Keyword)

    // Regression test for FSB 3696 - Colorization doesn't treat #if/else/endif correctly when embedded in a string literal
    [<Test>]
    member this.``PreProcessor.InStringLiteral01``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                #if UNDEFINED
                let x = "#elseMarker1"
                let y = "#endifMarker2"
                #else//Marker3
                let x = "#elseMarker4"
                let y = "#endifMarker5"
                #endif""",
            marker = "eMarker1",
            tokenType = TokenType.InactiveCode)

    [<Test>]
    member this.``PreProcessor.InStringLiteral02``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                #if UNDEFINED
                let x = "#elseMarker1"
                let y = "#endifMarker2"
                #else//Marker3
                let x = "#elseMarker4"
                let y = "#endifMarker5"
                #endif""",
            marker = "fMarker2",
            tokenType = TokenType.InactiveCode)
  
    [<Test>]
    member this.``PreProcessor.ElseKeyword``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                #if UNDEFINED
                let x = "#elseMarker1"
                let y = "#endifMarker2"
                #else//Marker3
                let x = "#elseMarker4"
                let y = "#endifMarker5"
                #endif""",
            marker = "e//Marker3",
            tokenType = TokenType.PreprocessorKeyword)    
    
    [<Test>]
    member this.``PreProcessor.InStringLiteral03``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                #if UNDEFINED
                let x = "#elseMarker1"
                let y = "#endifMarker2"
                #else//Marker3
                let x = "#elseMarker4"
                let y = "#endifMarker5"
                #endif""",
            marker = "eMarker4",
            tokenType = TokenType.String)   

    [<Test>]
    member this.``PreProcessor.InStringLiteral04``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                #if UNDEFINED
                let x = "#elseMarker1"
                let y = "#endifMarker2"
                #else//Marker3
                let x = "#elseMarker4"
                let y = "#endifMarker5"
                #endif""",
            marker = "fMarker5",
            tokenType = TokenType.String)   
       
    // Regression test for FSHARP1.0:4279
    [<Test>]
    member this.``Keyword.OCaml.asr``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                let foo a =
                  match a with
                  | Some(asr, b)  -> ()
                  |_ -> ()""",
            marker = "asr",
            tokenType = TokenType.Keyword)

    [<Test>]
    member this.``Keyword.OCaml.land``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                let foo a =
                  match a with
                  | Some(land, b)  -> ()
                  |_ -> ()""",
            marker = "land",
            tokenType = TokenType.Keyword)

    [<Test>]
    member this.``Keyword.OCaml.lor``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                let foo a =
                  match a with
                  | Some(lor, b)  -> ()
                  |_ -> ()""",
            marker = "lor",
            tokenType = TokenType.Keyword)

    [<Test>]
    member this.``Keyword.OCaml.lsl``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                let foo a =
                  match a with
                  | Some(lsl, b)  -> ()
                  |_ -> ()""",
            marker = "lsl",
            tokenType = TokenType.Keyword)

    [<Test>]
    member this.``Keyword.OCaml.lsr``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                let foo a =
                  match a with
                  | Some(lsr, b)  -> ()
                  |_ -> ()""",
            marker = "lsr",
            tokenType = TokenType.Keyword)

    [<Test>]
    member this.``Keyword.OCaml.lxor``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                let foo a =
                  match a with
                  | Some(lxor, b)  -> ()
                  |_ -> ()""",
            marker = "lxor",
            tokenType = TokenType.Keyword)

    [<Test>]
    member this.``Keyword.OCaml.mod``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                let foo a =
                  match a with
                  | Some(mod, b)  -> ()
                  |_ -> ()""",
            marker = "mod",
            tokenType = TokenType.Keyword)

    [<Test>]
    member this.``Keyword.OCaml.sig``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                let foo a =
                  match a with
                  | Some(sig, b)  -> ()
                  |_ -> ()""",
            marker = "sig",
            tokenType = TokenType.Keyword)

    [<Test>]
    member this.InactiveCode() =
        let fileContents = """
                #if UNDEFINED
                                    (*Inactive Code1*)let notLegit1 x = x
                #else
                    #if UNDEFINED
                                    (*Inactive Code2*)let notLegit2 x = x
                    #else
                        #if UNDEFINED
                                    (*Inactive Code3*)let notLegit3 x = x
                        #else
                            #if UNDEFINED
                                    (*Inactive Code4*)let notLegit4 x = x            
                            #else
                                #if UNDEFINED
                                    (*Inactive Code5*)let notLegit5 x = x
                                #else
                                      (*Active Code5*)let legitCode5 x = x
                                #endif
                                      (*Active Code4*)let legitCode4 x = x
                            #endif
                                      (*Active Code3*)let legitCode3 x = x
                        #endif

                                      (*Active Code2*)let legitCode2 x = x
                    #endif
                                      (*Active Code1*)let legitCode1 x = x
                #endif

                #if DEFINED
                                      (*Active Code6*)let legitCode6 x = x
                    #if DEFINED
                                      (*Active Code7*)let legitCode7 x = x
                    #else
                                    (*Inactive Code7*)let notLegit7 x = x
                    #endif
                #else
                                    (*Inactive Code6*)let notLegit6 x = x
                #endif
                """

        let (_solution, _project, file) = this.CreateSingleFileProject(fileContents, defines = ["DEFINED"])
        MoveCursorToEndOfMarker(file, "Active Code1*)le"); AssertEqual(TokenType.Keyword, GetTokenTypeAtCursor(file))
        MoveCursorToEndOfMarker(file, "Active Code2*)le"); AssertEqual(TokenType.Keyword, GetTokenTypeAtCursor(file))
        MoveCursorToEndOfMarker(file, "Active Code3*)le"); AssertEqual(TokenType.Keyword, GetTokenTypeAtCursor(file))
        MoveCursorToEndOfMarker(file, "Active Code4*)le"); AssertEqual(TokenType.Keyword, GetTokenTypeAtCursor(file))
        MoveCursorToEndOfMarker(file, "Active Code5*)le"); AssertEqual(TokenType.Keyword, GetTokenTypeAtCursor(file))
        MoveCursorToEndOfMarker(file, "Active Code6*)le"); AssertEqual(TokenType.Keyword, GetTokenTypeAtCursor(file))
        MoveCursorToEndOfMarker(file, "Active Code7*)le"); AssertEqual(TokenType.Keyword, GetTokenTypeAtCursor(file))

        MoveCursorToEndOfMarker(file, "Inactive Code1*)le"); AssertEqual(TokenType.InactiveCode, GetTokenTypeAtCursor(file))
        MoveCursorToEndOfMarker(file, "Inactive Code2*)le"); AssertEqual(TokenType.InactiveCode, GetTokenTypeAtCursor(file))
        MoveCursorToEndOfMarker(file, "Inactive Code3*)le"); AssertEqual(TokenType.InactiveCode, GetTokenTypeAtCursor(file))
        MoveCursorToEndOfMarker(file, "Inactive Code4*)le"); AssertEqual(TokenType.InactiveCode, GetTokenTypeAtCursor(file))
        MoveCursorToEndOfMarker(file, "Inactive Code5*)le"); AssertEqual(TokenType.InactiveCode, GetTokenTypeAtCursor(file))
        MoveCursorToEndOfMarker(file, "Inactive Code6*)le"); AssertEqual(TokenType.InactiveCode, GetTokenTypeAtCursor(file))
        MoveCursorToEndOfMarker(file, "Inactive Code7*)le"); AssertEqual(TokenType.InactiveCode, GetTokenTypeAtCursor(file))



        //ColorizerTest start
    [<Test>]
    member public this.``Regression.Bug2986``() =
        let code = 
                                    [
                                        "#light"
                                        "let x = 12"
                                        "(*"
                                        "blaa"
                                        "blaa"
                                        "blaa"
                                        "blaa"
                                        "blaa"
                                        "blaa"
                                        "blaa"
                                        "*)"
                                        "open System"
                                        "open System.IO"
                                        "open System.Text"
                                        "open System.Security.Cryptography"
                                        "let fold = List.fold"
                                        "let linkMap f xs = List.concat (List.map f xs)"
                                        "let argv = System.Environment.GetCommandLineArgs()"
                                        "let buildConfiguration = argv.[argv.Length - 1]"
                                    ]
        let (_solution, _project, file) = this.CreateSingleFileProject(code)
        
        // Make sure things are colored right to start
        MoveCursorToEndOfMarker(file,"open Sys")
        AssertEqual(TokenType.Identifier,GetTokenTypeAtCursor(file))   
        MoveCursorToEndOfMarker(file,"open System.Security.Crypto")
        AssertEqual(TokenType.Identifier,GetTokenTypeAtCursor(file))   
    
        // Delete the chunk of comments.        
        ReplaceFileInMemory file
                                    [
                                        "#light"
                                        "let x = 12"
                                        "open System"
                                        "open System.IO"
                                        "open System.Text"
                                        "open System.Security.Cryptography"
                                        "let fold = List.fold"
                                        "let linkMap f xs = List.concat (List.map f xs)"
                                        "let argv = System.Environment.GetCommandLineArgs()"
                                        "let buildConfiguration = argv.[argv.Length - 1]"
                                    ]

        // Reconfirm
        MoveCursorToEndOfMarker(file,"open Sys")
        AssertEqual(TokenType.Identifier,GetTokenTypeAtCursor(file))   
        MoveCursorToEndOfMarker(file,"open System.Security.Crypto")
        AssertEqual(TokenType.Identifier,GetTokenTypeAtCursor(file))   

    [<Test>]
    member public this.``Colorizer.AtString``() =
        let (_solution, _project, file) = this.CreateSingleFileProject("let s = @\"Bob\"")        
        // Check Bob
        MoveCursorToEndOfMarker(file,"let s = @\"B")
        AssertEqual(TokenType.String,GetTokenTypeAtCursor(file))   
        
    [<Test>]
    member public this.``Regression.Bug4860``() =        
        let fileContents = "
let x = __SOURCE_DIRECTORY__(*Test1*)
let y = __SOURCE_FILE__(*Test2*)
let z = __LINE__(*Test3*)
"

        let (solution, project, file) = this.CreateSingleFileProject(fileContents)
        
        MoveCursorToStartOfMarker(file, "__(*Test1*)")
        AssertEqual(TokenType.Keyword, GetTokenTypeAtCursor(file))
        
        MoveCursorToStartOfMarker(file, "__(*Test2*)")
        AssertEqual(TokenType.Keyword, GetTokenTypeAtCursor(file))
        
        MoveCursorToStartOfMarker(file, "__(*Test3*)")
        AssertEqual(TokenType.Keyword, GetTokenTypeAtCursor(file))        

    [<Test>]
    member public this.``Number.Regression.Bug3566``() =
        let otherNumbers = "let other = 0x4, 0b0100, 4L, 4UL, 4u, 4s, 4us, 4y, 4uy, 4.0, 4.0f, 4N, 4I, 1M, 123"
        let code = 
            [ "let n = 123";
                "let l = [12..15]";
                "let l2 = [12 .. 15]";
                "let l3 = [ 12 .. 15 ]";
                "// comment1: 1234";
                "(* comment2: 1234 *)";
                otherNumbers;
            ]
        let (_solution, _project, file) = this.CreateSingleFileProject(code)
        
        // Check integer number
        MoveCursorToEndOfMarker(file,"let n = 1")
        AssertEqual(TokenType.Number,GetTokenTypeAtCursor(file))   

        // Check numbers in a range expression
        MoveCursorToEndOfMarker(file,"let l = [1")
        AssertEqual(TokenType.Number,GetTokenTypeAtCursor(file))   
        MoveCursorToEndOfMarker(file,"let l = [12..1")
        AssertEqual(TokenType.Number,GetTokenTypeAtCursor(file))   

        MoveCursorToEndOfMarker(file,"let l2 = [1")
        AssertEqual(TokenType.Number,GetTokenTypeAtCursor(file))   
        MoveCursorToEndOfMarker(file,"let l2 = [12 .. 1")
        AssertEqual(TokenType.Number,GetTokenTypeAtCursor(file))   

        MoveCursorToEndOfMarker(file,"let l3 = [ 1")
        AssertEqual(TokenType.Number,GetTokenTypeAtCursor(file))   
        MoveCursorToEndOfMarker(file,"let l3 = [ 12 .. 1")
        AssertEqual(TokenType.Number,GetTokenTypeAtCursor(file))   
        
        // Check other numeric formats
        let mutable index = otherNumbers.IndexOf(",")
        while(index <> -1) do
            let substr = otherNumbers.Substring(0, index - 1) // -1 to move into the number
            MoveCursorToEndOfMarker(file, substr)
            AssertEqual(TokenType.Number, GetTokenTypeAtCursor(file))   
            index <- otherNumbers.IndexOf(",", index + 1)
            
        // Check that numbers in comments are not colored as numbers
        MoveCursorToEndOfMarker(file,"// comment1: 12")
        AssertEqual(TokenType.Comment,GetTokenTypeAtCursor(file))   

        MoveCursorToEndOfMarker(file,"(* comment2: 12")
        AssertEqual(TokenType.Comment,GetTokenTypeAtCursor(file))   
            
       
    /// FEATURE: Hash commands in .fsx files are colorized in PreprocessorKeyword color        
    [<Test>]
    member public this.``Preprocessor.InFsxFile``() =
        let code = 
            [
                "#reference @\"\""
                "#load @\"\""
                "#I <--hash I"
                "#time @\"\""
                "    #reference @\"\""
                "    #load @\"\""
                "    #I <--spaces then hash I"
                "    #time @\"\""                                     
            ]
        let (_, _, file) = this.CreateSingleFileProject(code, fileKind = SourceFileKind.FSX)
        
        MoveCursorToEndOfMarker(file,"#ref")
        AssertEqual(TokenType.PreprocessorKeyword ,GetTokenTypeAtCursor(file))                           
        MoveCursorToEndOfMarker(file,"#loa")
        AssertEqual(TokenType.PreprocessorKeyword ,GetTokenTypeAtCursor(file))                           
        MoveCursorToStartOfMarker(file,"I <--hash I")
        AssertEqual(TokenType.PreprocessorKeyword ,GetTokenTypeAtCursor(file))                           
        MoveCursorToEndOfMarker(file,"#ti")
        AssertEqual(TokenType.PreprocessorKeyword ,GetTokenTypeAtCursor(file))  
        MoveCursorToEndOfMarker(file,"    #ref")
        AssertEqual(TokenType.PreprocessorKeyword ,GetTokenTypeAtCursor(file))                           
        MoveCursorToEndOfMarker(file,"    #loa")
        AssertEqual(TokenType.PreprocessorKeyword ,GetTokenTypeAtCursor(file))                           
        MoveCursorToStartOfMarker(file,"I <--spaces then hash I")
        AssertEqual(TokenType.PreprocessorKeyword ,GetTokenTypeAtCursor(file))                           
        MoveCursorToEndOfMarker(file,"    #ti")
        AssertEqual(TokenType.PreprocessorKeyword ,GetTokenTypeAtCursor(file))     
                             
        
    /// FEATURE: Script-specific hash commands do not show up in blue in .fs files.
    [<Test>]
    member public this.``Preprocessor.InFsFile``() =
        let code = 
            [
                "#reference @\"\""
                "#load @\"\""
                "#I <--hash I"
                "#time @\"\""
                "    #reference @\"\""
                "    #load @\"\""
                "    #I <--spaces then hash I"
                "    #time @\"\""                                     
            ]
        let (_, _, file) = this.CreateSingleFileProject(code)
        
        MoveCursorToEndOfMarker(file,"#ref")
        AssertEqual(TokenType.Text,GetTokenTypeAtCursor(file))                           
        MoveCursorToEndOfMarker(file,"#loa")
        AssertEqual(TokenType.Text ,GetTokenTypeAtCursor(file))                           
        MoveCursorToStartOfMarker(file,"I <--hash I")
        AssertEqual(TokenType.Text ,GetTokenTypeAtCursor(file))                           
        MoveCursorToEndOfMarker(file,"#ti")
        AssertEqual(TokenType.Text ,GetTokenTypeAtCursor(file))  
        MoveCursorToEndOfMarker(file,"    #ref")
        AssertEqual(TokenType.Text ,GetTokenTypeAtCursor(file))                           
        MoveCursorToEndOfMarker(file,"    #loa")
        AssertEqual(TokenType.Text ,GetTokenTypeAtCursor(file))                           
        MoveCursorToStartOfMarker(file,"I <--spaces then hash I")
        AssertEqual(TokenType.Text ,GetTokenTypeAtCursor(file))                           
        MoveCursorToEndOfMarker(file,"    #ti")
        AssertEqual(TokenType.Text ,GetTokenTypeAtCursor(file))                                    

    /// FEATURE: Nested (* *) comments are allowed and will be colorized with CommentColor. Only the final *) causes the comment to close.
    [<Test>]
    member public this.``Comment.AfterCommentBlock``() =
        let code = 
            ["(*Bob*)type Bob() = class end"
             "(*"
             "(*"
             "(*Alice*)type Alice() = class end"
             "*)"
             "*)"
             "(*Charles*)type Charles() = class end"]
        let (_, _, file) = this.CreateSingleFileProject(code)
        // Check Bob
        MoveCursorToEndOfMarker(file,"(*Bob*)t")
        AssertEqual(TokenType.Keyword,GetTokenTypeAtCursor(file))
        
        // Check Alice
        MoveCursorToEndOfMarker(file,"(*Alice*)t")
        AssertEqual(TokenType.Comment,GetTokenTypeAtCursor(file))
        
        // Check Charles
        MoveCursorToEndOfMarker(file,"(*Charles*)t")
        AssertEqual(TokenType.Keyword,GetTokenTypeAtCursor(file))
        
    /// BUG: The comment used to be colored in black.
    [<Test>]
    member public this.``Regression.Bug1596``() =
        let code = [" let 2d (* Identifiers cannot start with numbers *)"]
        let (_, _, file) = this.CreateSingleFileProject(code)
        
        // Check Bob
        MoveCursorToEndOfMarker(file,"let 2d (* Ide")
        AssertEqual(TokenType.Comment,GetTokenTypeAtCursor(file))
  
        
    /// FEATURE: Code inside #if\#else\#endif blocks is colored with InactiveCodeColor depending on defines. This works for nested #if blocks as well.
    [<Test>]
    member public this.``Preprocessor.AfterPreprocessorBlock``() =
        let code = 
               ["(*Bob*)type Bob() = class end"
                "#if UNDEFINED"
                "    #if UNDEFINED"
                "    (*Alice*)type Alice() = class end"
                "    #else"
                "    (*Tom*)type Tom() = class end"
                "    #endif"
                "#else"
                "    #if UNDEFINED"
                "    (*Maurice*)type Maurice() = class end"
                "    #else"
                "    (*Larry*)type Larry() = class end"
                "    #endif"
                "#endif"
                "(*Charles*)type Charles() = class end"]
        let (_, _, file) = this.CreateSingleFileProject(code)
        
        let check marker token = 
            MoveCursorToEndOfMarker(file,marker)
            AssertEqual(token,GetTokenTypeAtCursor(file))
        
        check "(*Bob*)t" TokenType.Keyword 
        check "(*Alice*)t" TokenType.InactiveCode
        check "(*Tom*)t" TokenType.InactiveCode 
        check "(*Maurice*)t" TokenType.InactiveCode 
        check "(*Larry*)t" TokenType.Keyword 
        check "(*Charles*)t" TokenType.Keyword 

    // Wrong "#else" in "#if" should be ignored
    [<Test>]
    member public this.``Preprocessor.InvalidElseDirectiveIgnored``() =
        let code = 
                                    ["#if UNDEFINED"
                                     "    (*Alice*)type Alice() = class end"
                                     "(**) #else"
                                     "    (*Larry*)type Larry() = class end"
                                     "#endif"]
        let (_, _, file) = this.CreateSingleFileProject(code)
        let check marker token = 
            MoveCursorToEndOfMarker(file,marker)
            AssertEqual(token,GetTokenTypeAtCursor(file))
        
        check "(*Alice*)t" TokenType.InactiveCode
        check "(*Larry*)t" TokenType.InactiveCode
        
    /// FEATURE: Code inside #if\#else\#endif blocks is colored with InactiveCodeColor depending on defines. This works for nested #if blocks as well.
    [<Test>]
    member public this.``Preprocessor.AfterPreprocessorBlockWithDefines``() =
        let code = 
                                    ["(*Bob*)type Bob() = class end"
                                     "#if UNDEFINED"
                                     "    #if UNDEFINED"
                                     "    (*Alice*)type Alice() = class end"
                                     "    #else"
                                     "    (*Tom*)type Tom() = class end"
                                     "    #endif"
                                     "#else"
                                     "    #if UNDEFINED"
                                     "    (*Maurice*)type Maurice() = class end"
                                     "    #else"
                                     "    (*Larry*)type Larry() = class end"
                                     "    #endif"
                                     "#endif"
                                     "(*Charles*)type Charles() = class end"]
        let (_, _, file) = this.CreateSingleFileProject(code, defines = ["FOO";"UNDEFINED"])
        
        let check marker token = 
            MoveCursorToEndOfMarker(file,marker)
            AssertEqual(token,GetTokenTypeAtCursor(file))

        check "(*Bob*)t" TokenType.Keyword 
        check "(*Alice*)t" TokenType.Keyword
        check "(*Tom*)t" TokenType.InactiveCode 
        check "(*Maurice*)t" TokenType.InactiveCode 
        check "(*Larry*)t" TokenType.InactiveCode
        check "(*Charles*)t" TokenType.Keyword 
        
    /// FEATURE: Preprocessor keywords #light\#if\#else\#endif are colored with the PreprocessorKeyword color.
    /// FEATURE: All code in the inactive side of #if\#else\#endif is colored with with InactiveCode color.
    [<Test>]
    member public this.``Preprocessor.Keywords``() =
        let code = 
                                    ["#light (*Light*)"
                                     "  #if UNDEFINED //(*If*)"
                                     "    let x = 1(*Inactive*)"
                                     "  #else //(*Else*)"
                                     "    let(*Active*) x = 1"
                                     "  #endif //(*Endif*)"]
        let (_, _, file) = this.CreateSingleFileProject(code)
        
        let check marker token = 
            MoveCursorToStartOfMarker(file,marker)
            AssertEqual(token,GetTokenTypeAtCursor(file))
        
        check "light (*Light*)" TokenType.PreprocessorKeyword
        check "(*Inactive*)" TokenType.InactiveCode
        check "if UNDEFINED //(*If*)" TokenType.PreprocessorKeyword
        check "FINED //(*If*)" TokenType.Identifier
        check "(*If*)" TokenType.Comment
        check "else //(*Else*)" TokenType.PreprocessorKeyword
        check "t(*Active*)" TokenType.Keyword
        check "endif //(*Endif*)" TokenType.PreprocessorKeyword
        check "(*If*)" TokenType.Comment
        check "(*Else*)" TokenType.Comment
        check "(*Endif*)" TokenType.Comment

    /// FEATURE: Preprocessor extended grammar basic check.
    /// FEATURE:  More extensive grammar test is done in compiler unit tests
    [<Test>]
    member public this.``Preprocessor.ExtendedIfGrammar.Basic01``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                #if UNDEFINED || !UNDEFINED // Extended #if
                let x = "activeCode"
                #else
                let x = "inactiveCode"
                #endif
                """,
            marker = "activeCode",
            tokenType = TokenType.String)

    [<Test>]
    member public this.``Preprocessor.ExtendedIfGrammar.Basic02``() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """
                #if UNDEFINED || !UNDEFINED // Extended #if
                let x = "activeCode"
                #else
                let x = "inactiveCode"
                #endif
                """,
            marker = "inactiveCode",
            tokenType = TokenType.InactiveCode)

    /// #else / #endif in multiline strings is ignored
    [<Test>]
    member public this.``Preprocessor.DirectivesInString``() =
        let code = 
                                    ["#light"
                                     ""
                                     "#if DEFINED"
                                     "let s = \""
                                     "#else"
                                     "\""
                                     "let testme = 1"
                                     "#endif"]
        let (_, _, file) = this.CreateSingleFileProject(code, defines = ["DEFINED"])
        MoveCursorToStartOfMarker(file,"let testme")
        AssertEqual(TokenType.Keyword,GetTokenTypeAtCursor(file))
        
    /// Bug 2076 - String literals were causing the endif stack information to be discarded
    [<Test>]
    member public this.``Preprocessor.KeywordsWithStrings``() =
        let code = 
                                    ["#light (*Light*)"
                                     "let x1 = \"string1\""
                                     "#if UNDEFINED //(*If*)"
                                     "let x2 = \"string2\""
                                     "#else //(*Else*)"
                                     "let x3 = \"string3\""
                                     "#endif //(*Endif*)"
                                     "let x4 = \"string4\""]
        let (_, _, file) = this.CreateSingleFileProject(code)
        
        let check marker token = 
            MoveCursorToStartOfMarker(file,marker)
            AssertEqual(token,GetTokenTypeAtCursor(file))
        check "if UNDEFINED //(*If*)" TokenType.PreprocessorKeyword
        check "else //(*Else*)" TokenType.PreprocessorKeyword
        check "endif //(*Endif*)" TokenType.PreprocessorKeyword

    [<Test>]
    member public this.``Comment.VerbatimStringInComment.Bug1778``() =
        let code = 
                                    ["#light"
                                     "(* @\"\\\" *) let a = 0"]
        let (_, _, file) = this.CreateSingleFileProject(code)
      
        MoveCursorToStartOfMarker(file, "le")
        AssertEqual(TokenType.Keyword ,GetTokenTypeAtCursor(file))

    [<Test>]
    member public this.``Preprocessor.KeywordsWrongIf.Bug1577``() =
        let code = 
                                    ["#if !!!!!!!!!!!!!!!COMPILED"
                                     "#endif"]
        let (_, _, file) = this.CreateSingleFileProject(code)
        MoveCursorToStartOfMarker(file, "!!COMPILED")
        AssertEqual(TokenType.Identifier, GetTokenTypeAtCursor(file))


    // This was an off-by-one bug in the replacement Colorizer
    [<Test>]
    member public this.``Keyword.LastCharacterOfKeyword``() =
        let code = ["(*Bob*)type Bob() = int"]
        let (_, _, file) = this.CreateSingleFileProject(code)
        
        // Check Bob
        MoveCursorToEndOfMarker(file,"(*Bob*)typ")
        AssertEqual(TokenType.Keyword,GetTokenTypeAtCursor(file))

// Context project system
[<TestFixture>]
type UsingProjectSystem() = 
    inherit UsingMSBuild(VsOpts = LanguageServiceExtension.ProjectSystemTestFlavour)
    
  