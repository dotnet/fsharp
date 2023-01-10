// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System.Threading
open Xunit
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor

type SyntacticClassificationServiceTests() =

    member private this.ExtractMarkerData(fileContents: string, marker: string, defines: string list, isScriptFile: Option<bool>) =
        let textSpan = TextSpan(0, fileContents.Length)

        let fileName =
            if isScriptFile.IsSome && isScriptFile.Value then
                "test.fsx"
            else
                "test.fs"

        let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())

        let tokens =
            Tokenizer.getClassifiedSpans (
                documentId,
                SourceText.From(fileContents),
                textSpan,
                Some(fileName),
                defines,
                CancellationToken.None
            )

        let markerPosition = fileContents.IndexOf(marker)
        Assert.True(markerPosition >= 0, $"Cannot find marker '{marker}' in file contents")
        (tokens, markerPosition)

    member private this.VerifyColorizerAtStartOfMarker
        (
            fileContents: string,
            marker: string,
            defines: string list,
            classificationType: string,
            ?isScriptFile: bool
        ) =
        let (tokens, markerPosition) =
            this.ExtractMarkerData(fileContents, marker, defines, isScriptFile)

        match tokens |> Seq.tryFind (fun token -> token.TextSpan.Contains(markerPosition)) with
        | None -> failwith "Cannot find colorization data for start of marker"
        | Some (classifiedSpan) ->
            let result = classificationType = classifiedSpan.ClassificationType 
            Assert.True(result, "Classification data doesn't match for start of marker")

    member private this.VerifyColorizerAtEndOfMarker
        (
            fileContents: string,
            marker: string,
            defines: string list,
            classificationType: string,
            ?isScriptFile: bool
        ) =
        let (tokens, markerPosition) =
            this.ExtractMarkerData(fileContents, marker, defines, isScriptFile)

        match
            tokens
            |> Seq.tryFind (fun token -> token.TextSpan.Contains(markerPosition + marker.Length - 1))
        with
        | None -> failwith "Cannot find colorization data for end of marker"
        | Some (classifiedSpan) ->
            let result = classificationType = classifiedSpan.ClassificationType 
            Assert.True(result, "Classification data doesn't match for end of marker")

    [<Fact>]
    member this.Comment_SingleLine() =
        this.VerifyColorizerAtEndOfMarker(
            fileContents =
                """
                let simplefunction x y = x + y // Test1SimpleComment""",
            marker = "// Test1",
            defines = [],
            classificationType = ClassificationTypeNames.Comment
        )

    [<Fact>]
    member this.Conment_SingleLine_MultiConments() =
        this.VerifyColorizerAtEndOfMarker(
            fileContents =
                """
                let x = // Test2SimpleComment // 1""",
            marker = "// Test2",
            defines = [],
            classificationType = ClassificationTypeNames.Comment
        )

    [<Fact>]
    member this.Comment_MultiLine_AfterAnExpression() =
        this.VerifyColorizerAtEndOfMarker(
            fileContents =
                """
                let mutliLine x = 5(* Test1MultiLine
                     Test2MultiLine <@@asdf@@>
                Test3MultiLine*) + 1(*Test4*)""",
            marker = "Test1",
            defines = [],
            classificationType = ClassificationTypeNames.Comment
        )

    [<Fact>]
    member this.Comment_MultiLine_WithLineBreakAndATab() =
        this.VerifyColorizerAtEndOfMarker(
            fileContents =
                """
                let mutliLine x = 5(* Test1MultiLine
                     Test2MultiLine <@@asdf@@>
                Test3MultiLine*) + 1(*Test4*)
                """,
            marker = "Test2",
            defines = [],
            classificationType = ClassificationTypeNames.Comment
        )

    [<Fact>]
    member this.Comment_MultiLine_WithLineBreakAfterQuotExp() =
        this.VerifyColorizerAtEndOfMarker(
            fileContents =
                """
                let mutliLine x = 5(* Test1MultiLine
                     Test2MultiLine <@@asdf@@>
                Test3MultiLine*) + 1(*Test4*)
                """,
            marker = "Test3",
            defines = [],
            classificationType = ClassificationTypeNames.Comment
        )

    [<Fact>]
    member this.Comment_MultiLine_AfterANumber() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                let mutliLine x = 5(* Test1MultiLine
                     Test2MultiLine <@@asdf@@>
                Test3MultiLine*) + 1(*Test4*)
                """,
            marker = "1(*Test4*)",
            defines = [],
            classificationType = ClassificationTypeNames.NumericLiteral
        )

    [<Fact>]
    member this.Comment_Nested_Nested01() =
        this.VerifyColorizerAtEndOfMarker(
            fileContents =
                """
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
            defines = [],
            classificationType = ClassificationTypeNames.Comment
        )

    [<Fact>]
    member this.Comment_Nested_Nested02() =
        this.VerifyColorizerAtEndOfMarker(
            fileContents =
                """
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
            defines = [],
            classificationType = ClassificationTypeNames.Comment
        )

    [<Fact>]
    member this.Comment_Nested_Nested03() =
        this.VerifyColorizerAtEndOfMarker(
            fileContents =
                """
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
            defines = [],
            classificationType = ClassificationTypeNames.Comment
        )

    [<Fact>]
    member this.Comment_Nested_IdentAfterNestedComments() =
        this.VerifyColorizerAtEndOfMarker(
            fileContents =
                """
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
            defines = [],
            classificationType = ClassificationTypeNames.Identifier
        )

    [<Fact>]
    member this.Comment_CommentInString() =
        this.VerifyColorizerAtEndOfMarker(
            fileContents =
                """
                let commentsInString = "...(*test1_comment_in_string_literal*)..."
                )""",
            marker = "test1",
            defines = [],
            classificationType = ClassificationTypeNames.StringLiteral
        )

    [<Fact>]
    member this.Comment_StringInComment() =
        this.VerifyColorizerAtEndOfMarker(
            fileContents =
                """
                (*
                let commentsInString2 = "...*)test2_stringliteral_in_comment(*..."
                *)""",
            marker = "test2",
            defines = [],
            classificationType = ClassificationTypeNames.Comment
        )

    [<Fact>]
    member this.Comment_Unterminated_KeywordBeforeComment() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                type IPeekPoke = interface(*ML Comment Start
                  abstract member Peek: unit -> int
                  abstract member Poke: int -> unit
                end
                """,
            marker = "face(*ML Comment Start",
            defines = [],
            classificationType = ClassificationTypeNames.Keyword
        )

    [<Fact>]
    member this.Comment_Unterminated_KeywordInComment() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
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
            defines = [],
            classificationType = ClassificationTypeNames.Comment
        )

    [<Fact>]
    member this.Comment_Unterminated_NestedComments() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
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
            defines = [],
            classificationType = ClassificationTypeNames.Comment
        )

    [<Fact>]
    member this.String_AtEnd() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                let stringone = "simple string test"(*Simple String*) """,
            marker = """est"(*Simple String*)""",
            defines = [],
            classificationType = ClassificationTypeNames.StringLiteral
        )

    [<Fact>]
    member this.String_MultiLines() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                let stringtwo = "simple test(*MultiLine - First*)
                                string test"(*MultiLine - Second*)""",
            marker = "st(*MultiLine - First*)",
            defines = [],
            classificationType = ClassificationTypeNames.StringLiteral
        )

    [<Fact>]
    member this.String_MultiLines_LineBreak() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                let stringtwo = "simple test(*MultiLine - First*)
                                string test"(*MultiLine - Second*) """,
            marker = "\"(*MultiLine - Second*) ",
            defines = [],
            classificationType = ClassificationTypeNames.StringLiteral
        )

    [<Fact>]
    member this.String_Literal() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """               
                let stringthree = @"literal test"(*Literal String*)""",
            marker = """st"(*Literal String*)""",
            defines = [],
            classificationType = ClassificationTypeNames.StringLiteral
        )

    [<Fact>]
    member this.ByteString_AtEnd() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                let bytestringone = "abcdefg"B(*Byte String*)""",
            marker = "B(*Byte String*)",
            defines = [],
            classificationType = ClassificationTypeNames.StringLiteral
        )

    [<Fact>]
    member this.ByteString_MultiLines() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                let bytestringtwo = "simple(*MultiLineB - First*)
                                    string"B(*MultiLineB - Second*)""",
            marker = """ing"B(*MultiLineB - Second*)""",
            defines = [],
            classificationType = ClassificationTypeNames.StringLiteral
        )

    [<Fact>]
    member this.ByteString_Literal() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """                                  
                let bytestringthree = @"literal"B(*Literal Byte*)""",
            marker = """al"B(*Literal Byte*)""",
            defines = [],
            classificationType = ClassificationTypeNames.StringLiteral
        )

    [<Fact>]
    member this.EscapedIdentifier_word() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """let ``this is an escaped identifier 123ASDF@#$"`` = 4""",
            marker = "this",
            defines = [],
            classificationType = ClassificationTypeNames.Identifier
        )

    [<Fact>]
    member this.EscapedIdentifier_SpecialChar() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """let ``this is an escaped identifier 123ASDF@#$"`` = 4""",
            marker = "3ASDF@#",
            defines = [],
            classificationType = ClassificationTypeNames.Identifier
        )

    [<Fact>]
    member this.EscapedIdentifier_EscapeChar() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """let ``this is an escaped identifier 123ASDF@#$"`` = 4""",
            marker = "\"``",
            defines = [],
            classificationType = ClassificationTypeNames.Identifier
        )

    /// Regression for 3609 - Colorizer: __SOURCE__ and others colorized as a string
    [<Fact>]
    member this.PredefinedIdentifier_SOURCE_DIRECTORY() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                let x = __SOURCE_DIRECTORY__(*Test1*)""",
            marker = "__(*Test1*)",
            defines = [],
            classificationType = ClassificationTypeNames.Keyword
        )

    [<Fact>]
    member this.PredefinedIdentifier_SOURCE_FILE() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                let y = __SOURCE_FILE__(*Test2*))""",
            marker = "__(*Test2*)",
            defines = [],
            classificationType = ClassificationTypeNames.Keyword
        )

    [<Fact>]
    member this.PredefinedIdentifier_LINE() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                let z = __LINE__(*Test3*)""",
            marker = "__(*Test3*)",
            defines = [],
            classificationType = ClassificationTypeNames.Keyword
        )

    // Regression Test for FSB 3566, F# colorizer does not respect numbers
    [<Fact>]
    member this.Number_InAnExpression() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """let f x = x + 9""",
            marker = "9",
            defines = [],
            classificationType = ClassificationTypeNames.NumericLiteral
        )

    // Regression Test for FSB 1778 - Colorization seems to be confused after parsing a comment that contains a verbatim string that contains a \
    [<Fact>]
    member this.Number_AfterCommentWithBackSlash() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents = """let f (* @"\\" *)x = x + 19(*Marker1*)""",
            marker = "9(*Marker1*)",
            defines = [],
            classificationType = ClassificationTypeNames.NumericLiteral
        )

    // Regression Test for FSharp1.0:2539 -- lexing @"" strings inside (* *) comments?
    [<Fact>]
    member this.Keyword_AfterCommentWithLexingStrings() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                (*
                let x = @"\\"
                *)

                let(*Marker1*) y = 1
                """,
            marker = "t(*Marker1*)",
            defines = [],
            classificationType = ClassificationTypeNames.Keyword
        )

    // Regression Test for FSB 1380 - Language Service colorizes anything followed by a bang as a keyword
    [<Fact>]
    member this.Keyword_LetBang() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                let seqExpr = 
                    seq {
                        let! x = [1 .. 10](*Marker1*)
                        yield! x(*Marker2*)
                        do! - = ()(*Marker3*)
                    }""",
            marker = "! x = [1 .. 10](*Marker1*)",
            defines = [],
            classificationType = ClassificationTypeNames.Keyword
        )

    [<Fact>]
    member this.Keyword_Yield() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                let seqExpr = 
                    seq {
                        let! x = [1 .. 10](*Marker1*)
                        yield! x(*Marker2*)
                        do! - = ()(*Marker3*)
                    }""",
            marker = "! x(*Marker2*)",
            defines = [],
            classificationType = ClassificationTypeNames.Keyword
        )

    [<Fact>]
    member this.Keyword_Do() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                let seqExpr = 
                    seq {
                        let! x = [1 .. 10](*Marker1*)
                        yield! x(*Marker2*)
                        do! - = ()(*Marker3*)
                    }""",
            marker = "! - = ()(*Marker3*)",
            defines = [],
            classificationType = ClassificationTypeNames.Keyword
        )

    [<Fact>]
    member this.Keyword_Invalid_Bang() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                let seqExpr = 
                    seq {
                        foo! = true(*Marker1*)
                    }""",
            marker = "! = true(*Marker1*)",
            defines = [],
            classificationType = ClassificationTypeNames.Identifier
        )

    [<Fact>]
    //This test case Verify that the color of const is the keyword color
    member this.TypeProvider_StaticParameters_Keyword_const() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                     type foo = N1.T< const(*Marker1*) "Hello World",2>""",
            marker = "t(*Marker1*)",
            defines = [],
            classificationType = ClassificationTypeNames.Keyword
        )

    // Regression test for FSB 3696 - Colorization doesn't treat #if/else/endif correctly when embedded in a string literal
    [<Fact>]
    member this.PreProcessor_InStringLiteral01() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                #if UNDEFINED
                let x = "#elseMarker1"
                let y = "#endifMarker2"
                #else//Marker3
                let x = "#elseMarker4"
                let y = "#endifMarker5"
                #endif""",
            marker = "eMarker1",
            defines = [],
            classificationType = ClassificationTypeNames.ExcludedCode
        )

    [<Fact>]
    member this.PreProcessor_InStringLiteral02() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                #if UNDEFINED
                let x = "#elseMarker1"
                let y = "#endifMarker2"
                #else//Marker3
                let x = "#elseMarker4"
                let y = "#endifMarker5"
                #endif""",
            marker = "fMarker2",
            defines = [],
            classificationType = ClassificationTypeNames.ExcludedCode
        )

    [<Fact>]
    member this.PreProcessor_ElseKeyword() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                #if UNDEFINED
                let x = "#elseMarker1"
                let y = "#endifMarker2"
                #else//Marker3
                let x = "#elseMarker4"
                let y = "#endifMarker5"
                #endif""",
            marker = "e//Marker3",
            defines = [],
            classificationType = ClassificationTypeNames.PreprocessorKeyword
        )

    [<Fact>]
    member this.PreProcessor_InStringLiteral03() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                #if UNDEFINED
                let x = "#elseMarker1"
                let y = "#endifMarker2"
                #else//Marker3
                let x = "#elseMarker4"
                let y = "#endifMarker5"
                #endif""",
            marker = "eMarker4",
            defines = [],
            classificationType = ClassificationTypeNames.StringLiteral
        )

    [<Fact>]
    member this.PreProcessor_InStringLiteral04() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                #if UNDEFINED
                let x = "#elseMarker1"
                let y = "#endifMarker2"
                #else//Marker3
                let x = "#elseMarker4"
                let y = "#endifMarker5"
                #endif""",
            marker = "fMarker5",
            defines = [],
            classificationType = ClassificationTypeNames.StringLiteral
        )

    // Regression test for FSHARP1.0:4279
    [<Fact>]
    member this.Keyword_OCaml_asr() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                let foo a =
                  match a with
                  | Some(asr, b)  -> ()
                  |_ -> ()""",
            marker = "asr",
            defines = [],
            classificationType = ClassificationTypeNames.Keyword
        )

    [<Fact>]
    member this.Keyword_OCaml_land() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                let foo a =
                  match a with
                  | Some(land, b)  -> ()
                  |_ -> ()""",
            marker = "land",
            defines = [],
            classificationType = ClassificationTypeNames.Keyword
        )

    [<Fact>]
    member this.Keyword_OCaml_lor() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                let foo a =
                  match a with
                  | Some(lor, b)  -> ()
                  |_ -> ()""",
            marker = "lor",
            defines = [],
            classificationType = ClassificationTypeNames.Keyword
        )

    [<Fact>]
    member this.Keyword_OCaml_lsl() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                let foo a =
                  match a with
                  | Some(lsl, b)  -> ()
                  |_ -> ()""",
            marker = "lsl",
            defines = [],
            classificationType = ClassificationTypeNames.Keyword
        )

    [<Fact>]
    member this.Keyword_OCaml_lsr() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                let foo a =
                  match a with
                  | Some(lsr, b)  -> ()
                  |_ -> ()""",
            marker = "lsr",
            defines = [],
            classificationType = ClassificationTypeNames.Keyword
        )

    [<Fact>]
    member this.Keyword_OCaml_lxor() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                let foo a =
                  match a with
                  | Some(lxor, b)  -> ()
                  |_ -> ()""",
            marker = "lxor",
            defines = [],
            classificationType = ClassificationTypeNames.Keyword
        )

    [<Fact>]
    member this.Keyword_OCaml_mod() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                let foo a =
                  match a with
                  | Some(mod, b)  -> ()
                  |_ -> ()""",
            marker = "mod",
            defines = [],
            classificationType = ClassificationTypeNames.Keyword
        )

    [<Fact>]
    member this.Keyword_OCaml_sig() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                let foo a =
                  match a with
                  | Some(sig, b)  -> ()
                  |_ -> ()""",
            marker = "sig",
            defines = [],
            classificationType = ClassificationTypeNames.Keyword
        )

    [<Theory>]
    [<InlineData("Active Code1*)le", ClassificationTypeNames.Keyword)>]
    [<InlineData("Active Code2*)le", ClassificationTypeNames.Keyword)>]
    [<InlineData("Active Code3*)le", ClassificationTypeNames.Keyword)>]
    [<InlineData("Active Code4*)le", ClassificationTypeNames.Keyword)>]
    [<InlineData("Active Code5*)le", ClassificationTypeNames.Keyword)>]
    [<InlineData("Active Code6*)le", ClassificationTypeNames.Keyword)>]
    [<InlineData("Active Code7*)le", ClassificationTypeNames.Keyword)>]
    [<InlineData("Inactive Code1*)le", ClassificationTypeNames.ExcludedCode)>]
    [<InlineData("Inactive Code2*)le", ClassificationTypeNames.ExcludedCode)>]
    [<InlineData("Inactive Code3*)le", ClassificationTypeNames.ExcludedCode)>]
    [<InlineData("Inactive Code4*)le", ClassificationTypeNames.ExcludedCode)>]
    [<InlineData("Inactive Code5*)le", ClassificationTypeNames.ExcludedCode)>]
    [<InlineData("Inactive Code6*)le", ClassificationTypeNames.ExcludedCode)>]
    [<InlineData("Inactive Code7*)le", ClassificationTypeNames.ExcludedCode)>]
    member this.InactiveCode(marker: string, classificationType: string) =
        let fileContents =
            """
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

        this.VerifyColorizerAtEndOfMarker(fileContents, marker, [ "DEFINED" ], classificationType)

    [<Fact>]
    member public this.Colorizer_AtString() =
        this.VerifyColorizerAtEndOfMarker(
            "let s = @\"Bob\"",
            marker = "let s = @\"B",
            defines = [],
            classificationType = ClassificationTypeNames.StringLiteral
        )

    [<Theory>]
    [<InlineData("__(*Test1*)", ClassificationTypeNames.Keyword)>]
    [<InlineData("__(*Test2*)", ClassificationTypeNames.Keyword)>]
    [<InlineData("__(*Test3*)", ClassificationTypeNames.Keyword)>]
    member public this.Regression_Bug4860(marker: string, classificationType: string) =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                "
                let x = __SOURCE_DIRECTORY__(*Test1*)
                let y = __SOURCE_FILE__(*Test2*)
                let z = __LINE__(*Test3*)",
            marker = marker,
            defines = [],
            classificationType = classificationType
        )

    [<Theory>]
    [<InlineData("let n = 1", ClassificationTypeNames.NumericLiteral)>]
    [<InlineData("let l = [1", ClassificationTypeNames.NumericLiteral)>]
    [<InlineData("let l = [12..1", ClassificationTypeNames.NumericLiteral)>]
    [<InlineData("let l2 = [1", ClassificationTypeNames.NumericLiteral)>]
    [<InlineData("let l2 = [12 .. 1", ClassificationTypeNames.NumericLiteral)>]
    [<InlineData("let l3 = [ 1", ClassificationTypeNames.NumericLiteral)>]
    [<InlineData("let l3 = [ 12 .. 1", ClassificationTypeNames.NumericLiteral)>]
    [<InlineData("0x4", ClassificationTypeNames.NumericLiteral)>]
    [<InlineData("0b0100", ClassificationTypeNames.NumericLiteral)>]
    [<InlineData("4L", ClassificationTypeNames.NumericLiteral)>]
    [<InlineData("4UL", ClassificationTypeNames.NumericLiteral)>]
    [<InlineData("4u", ClassificationTypeNames.NumericLiteral)>]
    [<InlineData("4s", ClassificationTypeNames.NumericLiteral)>]
    [<InlineData("4us", ClassificationTypeNames.NumericLiteral)>]
    [<InlineData("4y", ClassificationTypeNames.NumericLiteral)>]
    [<InlineData("4uy", ClassificationTypeNames.NumericLiteral)>]
    [<InlineData("4.0", ClassificationTypeNames.NumericLiteral)>]
    [<InlineData("4.0f", ClassificationTypeNames.NumericLiteral)>]
    [<InlineData("4N", ClassificationTypeNames.NumericLiteral)>]
    [<InlineData("4I", ClassificationTypeNames.NumericLiteral)>]
    [<InlineData("1M", ClassificationTypeNames.NumericLiteral)>]
    [<InlineData("123", ClassificationTypeNames.NumericLiteral)>]
    [<InlineData("// comment1: 12", ClassificationTypeNames.Comment)>]
    [<InlineData("(* comment2: 12", ClassificationTypeNames.Comment)>]
    member public this.Number_Regression_Bug3566(marker: string, classificationType: string) =
        this.VerifyColorizerAtEndOfMarker(
            fileContents =
                "let n = 123
                let l = [12..15]
                let l2 = [12 .. 15]
                let l3 = [ 12 .. 15 ]
                // comment1: 1234
                (* comment2: 1234 *)
                let other = 0x4, 0b0100, 4L, 4UL, 4u, 4s, 4us, 4y, 4uy, 4.0, 4.0f, 4N, 4I, 1M, 123",
            marker = marker,
            defines = [],
            classificationType = classificationType
        )

    /// FEATURE: Hash commands in .fsx files are colorized in PreprocessorKeyword color
    [<Theory>]
    [<InlineData("I <--hash I", ClassificationTypeNames.PreprocessorKeyword)>]
    member public this.Preprocessor_InFsxFile_StartOfMarker(marker: string, classificationType: string) =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                "#reference @\"\"
                #load @\"\"
                #I <--hash I
                #time @\"\"",
            marker = marker,
            defines = [],
            classificationType = classificationType,
            isScriptFile = true
        )

    /// FEATURE: Hash commands in .fsx files are colorized in PreprocessorKeyword color
    [<Theory>]
    [<InlineData("#ref", ClassificationTypeNames.PreprocessorKeyword)>]
    [<InlineData("#loa", ClassificationTypeNames.PreprocessorKeyword)>]
    [<InlineData("#ti", ClassificationTypeNames.PreprocessorKeyword)>]
    member public this.Preprocessor_InFsxFile_EndOfMarker(marker: string, classificationType: string) =
        this.VerifyColorizerAtEndOfMarker(
            fileContents =
                "#reference @\"\"
                #load @\"\"
                #I <--hash I
                #time @\"\"",
            marker = marker,
            defines = [],
            classificationType = classificationType,
            isScriptFile = true
        )

    /// FEATURE: Script-specific hash commands do not show up in blue in .fs files.
    [<Theory>]
    [<InlineData(" <--hash I", ClassificationTypeNames.Text)>]
    member public this.Preprocessor_InFsFile_StartOfMarker(marker: string, classificationType: string) =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                "#reference @\"\"
                #load @\"\"
                #I <--hash I
                #time @\"\"",
            marker = marker,
            defines = [],
            classificationType = classificationType
        )

    /// FEATURE: Script-specific hash commands do not show up in blue in .fs files.
    [<Theory>]
    [<InlineData("#ref", ClassificationTypeNames.Text)>]
    [<InlineData("#loa", ClassificationTypeNames.Text)>]
    [<InlineData("#ti", ClassificationTypeNames.Text)>]
    member public this.Preprocessor_InFsFile_EndOfMarker(marker: string, classificationType: string) =
        this.VerifyColorizerAtEndOfMarker(
            fileContents =
                "#reference @\"\"
                #load @\"\"
                #I <--hash I
                #time @\"\"",
            marker = marker,
            defines = [],
            classificationType = classificationType
        )

    /// FEATURE: Nested (* *) comments are allowed and will be colorized with CommentColor. Only the final *) causes the comment to close.
    [<Theory>]
    [<InlineData("(*Bob*)t", ClassificationTypeNames.Keyword)>]
    [<InlineData("(*Alice*)t", ClassificationTypeNames.Comment)>]
    [<InlineData("(*Charles*)t", ClassificationTypeNames.Keyword)>]
    member public this.Comment_AfterCommentBlock(marker: string, classificationType: string) =
        this.VerifyColorizerAtEndOfMarker(
            fileContents =
                "(*Bob*)type Bob() = class end
                (*
                (*
                (*Alice*)type Alice() = class end
                *)
                *)
                (*Charles*)type Charles() = class end",
            marker = marker,
            defines = [],
            classificationType = classificationType
        )

    /// BUG: The comment used to be colored in black.
    [<Fact>]
    member public this.Regression_Bug1596() =
        this.VerifyColorizerAtEndOfMarker(
            fileContents = " let 2d (* Identifiers cannot start with numbers *)",
            marker = "let 2d (* Ide",
            defines = [],
            classificationType = ClassificationTypeNames.Comment
        )

    /// FEATURE: Code inside #if\#else\#endif blocks is colored with InactiveCodeColor depending on defines. This works for nested #if blocks as well.
    [<Theory>]
    [<InlineData("(*Bob*)t", ClassificationTypeNames.Keyword)>]
    [<InlineData("(*Alice*)t", ClassificationTypeNames.ExcludedCode)>]
    [<InlineData("(*Tom*)t", ClassificationTypeNames.ExcludedCode)>]
    [<InlineData("(*Maurice*)t", ClassificationTypeNames.ExcludedCode)>]
    [<InlineData("(*Larry*)t", ClassificationTypeNames.Keyword)>]
    [<InlineData("(*Charles*)t", ClassificationTypeNames.Keyword)>]
    member public this.Preprocessor_AfterPreprocessorBlock(marker: string, classificationType: string) =
        this.VerifyColorizerAtEndOfMarker(
            fileContents =
                "(*Bob*)type Bob() = class end
                #if UNDEFINED
                    #if UNDEFINED
                    (*Alice*)type Alice() = class end
                    #else
                    (*Tom*)type Tom() = class end
                    #endif
                #else
                    #if UNDEFINED
                    (*Maurice*)type Maurice() = class end
                    #else
                    (*Larry*)type Larry() = class end
                    #endif
                #endif
                (*Charles*)type Charles() = class end",
            marker = marker,
            defines = [],
            classificationType = classificationType
        )

    // Wrong "#else" in "#if" should be ignored
    [<Theory>]
    [<InlineData("(*Alice*)t", ClassificationTypeNames.ExcludedCode)>]
    [<InlineData("(*Larry*)t", ClassificationTypeNames.ExcludedCode)>]
    member public this.Preprocessor_InvalidElseDirectiveIgnored(marker: string, classificationType: string) =
        this.VerifyColorizerAtEndOfMarker(
            fileContents =
                "#if UNDEFINED
                    (*Alice*)type Alice() = class end
                (**) #else
                    (*Larry*)type Larry() = class end
                #endif",
            marker = marker,
            defines = [],
            classificationType = classificationType
        )

    /// FEATURE: Code inside #if\#else\#endif blocks is colored with InactiveCodeColor depending on defines. This works for nested #if blocks as well.
    [<Theory>]
    [<InlineData("(*Bob*)t", ClassificationTypeNames.Keyword)>]
    [<InlineData("(*Alice*)t", ClassificationTypeNames.ExcludedCode)>]
    [<InlineData("(*Tom*)t", ClassificationTypeNames.ExcludedCode)>]
    [<InlineData("(*Maurice*)t", ClassificationTypeNames.ExcludedCode)>]
    [<InlineData("(*Larry*)t", ClassificationTypeNames.Keyword)>]
    [<InlineData("(*Charles*)t", ClassificationTypeNames.Keyword)>]
    member public this.Preprocessor_AfterPreprocessorBlockWithDefines(marker: string, classificationType: string) =
        this.VerifyColorizerAtEndOfMarker(
            fileContents =
                "(*Bob*)type Bob() = class end
                #if UNDEFINED
                    #if UNDEFINED
                    (*Alice*)type Alice() = class end
                    #else
                    (*Tom*)type Tom() = class end
                    #endif
                #else
                    #if UNDEFINED
                    (*Maurice*)type Maurice() = class end
                    #else
                    (*Larry*)type Larry() = class end
                    #endif
                #endif
                (*Charles*)type Charles() = class end",
            marker = marker,
            defines = [],
            classificationType = classificationType
        )

    /// FEATURE: Preprocessor keywords #light\#if\#else\#endif are colored with the PreprocessorKeyword color.
    /// FEATURE: All code in the inactive side of #if\#else\#endif is colored with with InactiveCode color.
    [<Theory>]
    [<InlineData("light (*Light*)", ClassificationTypeNames.PreprocessorKeyword)>]
    [<InlineData("(*Inactive*)", ClassificationTypeNames.ExcludedCode)>]
    [<InlineData("if UNDEFINED //(*If*)", ClassificationTypeNames.PreprocessorKeyword)>]
    [<InlineData("FINED //(*If*)", ClassificationTypeNames.Identifier)>]
    [<InlineData("(*If*)", ClassificationTypeNames.Comment)>]
    [<InlineData("else //(*Else*)", ClassificationTypeNames.PreprocessorKeyword)>]
    [<InlineData("t(*Active*)", ClassificationTypeNames.Keyword)>]
    [<InlineData("endif //(*Endif*)", ClassificationTypeNames.PreprocessorKeyword)>]
    [<InlineData("(*Else*)", ClassificationTypeNames.Comment)>]
    [<InlineData("(*Endif*)", ClassificationTypeNames.Comment)>]
    member public this.Preprocessor_Keywords(marker: string, classificationType: string) =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                "#light (*Light*)
                  #if UNDEFINED //(*If*)
                    let x = 1(*Inactive*)
                  #else //(*Else*)
                    let(*Active*) x = 1
                  #endif //(*Endif*)",
            marker = marker,
            defines = [],
            classificationType = classificationType
        )

    /// FEATURE: Preprocessor extended grammar basic check.
    /// FEATURE:  More extensive grammar test is done in compiler unit tests
    [<Fact>]
    member public this.Preprocesso_ExtendedIfGrammar_Basic01() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                #if UNDEFINED || !UNDEFINED // Extended #if
                let x = "activeCode"
                #else
                let x = "inactiveCode"
                #endif
                """,
            marker = "activeCode",
            defines = [],
            classificationType = ClassificationTypeNames.StringLiteral
        )

    [<Fact>]
    member public this.Preprocessor_ExtendedIfGrammar_Basic02() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                """
                #if UNDEFINED || !UNDEFINED // Extended #if
                let x = "activeCode"
                #else
                let x = "inactiveCode"
                #endif
                """,
            marker = "inactiveCode",
            defines = [],
            classificationType = ClassificationTypeNames.ExcludedCode
        )

    /// #else / #endif in multiline strings is ignored
    [<Fact>]
    member public this.Preprocessor_DirectivesInString() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                "#light
                
                #if DEFINED
                let s = \"
                #else
                \"
                let testme = 1
                #endif",
            marker = "let testme",
            defines = [ "DEFINED" ],
            classificationType = ClassificationTypeNames.Keyword
        )

    /// Bug 2076 - String literals were causing the endif stack information to be discarded
    [<Theory>]
    [<InlineData("if UNDEFINED //(*If*)", ClassificationTypeNames.PreprocessorKeyword)>]
    [<InlineData("else //(*Else*)", ClassificationTypeNames.PreprocessorKeyword)>]
    [<InlineData("endif //(*Endif*)", ClassificationTypeNames.PreprocessorKeyword)>]
    member public this.Preprocessor_KeywordsWithStrings(marker: string, classificationType: string) =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                "#light (*Light*)
                let x1 = \"string1\"
                #if UNDEFINED //(*If*)
                let x2 = \"string2\"
                #else //(*Else*)
                let x3 = \"string3\"
                #endif //(*Endif*)
                let x4 = \"string4\"",
            marker = marker,
            defines = [],
            classificationType = classificationType
        )

    [<Fact>]
    member public this.Comment_VerbatimStringInComment_Bug1778() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                "#light
                (* @\"\\\" *) let a = 0",
            marker = "le",
            defines = [],
            classificationType = ClassificationTypeNames.Keyword
        )

    [<Fact>]
    member public this.Preprocessor_KeywordsWrongIf_Bug1577() =
        this.VerifyColorizerAtStartOfMarker(
            fileContents =
                "#if !!!!!!!!!!!!!!!COMPILED
                #endif",
            marker = "!!COMPILED",
            defines = [],
            classificationType = ClassificationTypeNames.Identifier
        )

    // This was an off-by-one bug in the replacement Colorizer
    [<Fact>]
    member public this.Keyword_LastCharacterOfKeyword() =
        this.VerifyColorizerAtEndOfMarker(
            fileContents = "(*Bob*)type Bob() = int",
            marker = "(*Bob*)typ",
            defines = [],
            classificationType = ClassificationTypeNames.Keyword
        )
