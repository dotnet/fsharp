// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open Xunit
open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Editor.Tests.Helpers
open FSharp.Test

type DocumentDiagnosticAnalyzerTests() =
    let startMarker = "(*start*)"
    let endMarker = "(*end*)"

    let getDiagnostics (fileContents: string) =
        async {
            let document =
                RoslynTestHelpers.CreateSolution(fileContents)
                |> RoslynTestHelpers.GetSingleDocument

            let! syntacticDiagnostics = FSharpDocumentDiagnosticAnalyzer.GetDiagnostics(document, DiagnosticsType.Syntax)
            let! semanticDiagnostics = FSharpDocumentDiagnosticAnalyzer.GetDiagnostics(document, DiagnosticsType.Semantic)
            return syntacticDiagnostics.AddRange(semanticDiagnostics)
        }
        |> Async.RunSynchronously

    member private this.VerifyNoErrors(fileContents: string, ?additionalFlags: string[]) =
        let errors = getDiagnostics fileContents

        if not errors.IsEmpty then
            failwith $"There should be no errors generated: {errors}"

    member private this.VerifyErrorAtMarker(fileContents: string, expectedMarker: string, ?expectedMessage: string) =
        let errors =
            getDiagnostics fileContents
            |> Seq.filter (fun e -> e.Severity = DiagnosticSeverity.Error)
            |> Seq.toArray

        Assert.True(1 = errors.Length, "There should be exactly one error generated")
        let actualError = errors.[0]

        if expectedMessage.IsSome then
            Assert.True(expectedMessage.Value = actualError.GetMessage(), "Error messages should match")

        Assert.Equal(DiagnosticSeverity.Error, actualError.Severity)
        let expectedStart = fileContents.IndexOf(expectedMarker)
        actualError.Location.SourceSpan.Start |> Assert.shouldBeEqualWith expectedStart "Error start positions should match"
        let expectedEnd = expectedStart + expectedMarker.Length
        actualError.Location.SourceSpan.End |> Assert.shouldBeEqualWith expectedEnd "Error end positions should match"

    member private this.VerifyDiagnosticBetweenMarkers
        (
            fileContents: string,
            expectedMessage: string,
            expectedSeverity: DiagnosticSeverity
        ) =
        let errors =
            getDiagnostics fileContents
            |> Seq.filter (fun e -> e.Severity = expectedSeverity)
            |> Seq.toArray

        Assert.True(1 = errors.Length, "There should be exactly one error generated")
        let actualError = errors.[0]
        Assert.Equal(expectedSeverity, actualError.Severity)
        actualError.GetMessage() |> Assert.shouldBeEqualWith expectedMessage "Error messages should match"
        let expectedStart = fileContents.IndexOf(startMarker) + startMarker.Length
        actualError.Location.SourceSpan.Start |> Assert.shouldBeEqualWith expectedStart "Error start positions should match"
        let expectedEnd = fileContents.IndexOf(endMarker)
        actualError.Location.SourceSpan.End |> Assert.shouldBeEqualWith expectedEnd "Error end positions should match"

    member private this.VerifyErrorBetweenMarkers(fileContents: string, expectedMessage: string) =
        this.VerifyDiagnosticBetweenMarkers(fileContents, expectedMessage, DiagnosticSeverity.Error)

    member private this.VerifyWarningBetweenMarkers(fileContents: string, expectedMessage: string) =
        this.VerifyDiagnosticBetweenMarkers(fileContents, expectedMessage, DiagnosticSeverity.Warning)

    [<Fact>]
    member public this.Error_Expression_IllegalIntegerLiteral() =
        this.VerifyErrorBetweenMarkers(
            fileContents =
                """
let _ = 1
let a = 0.1(*start*).(*end*)0
                """,
            expectedMessage = "Missing qualification after '.'"
        )

    [<Fact>]
    member public this.Error_Expression_IncompleteDefine() =
        this.VerifyErrorBetweenMarkers(
            fileContents =
                """
let a = (*start*);(*end*)
                """,
            expectedMessage = "Unexpected symbol ';' in binding"
        )

    [<Fact>]
    member public this.Error_Expression_KeywordAsValue() =
        this.VerifyErrorBetweenMarkers(
            fileContents =
                """
let b =
(*start*)type(*end*)
                """,
            expectedMessage = "Incomplete structured construct at or before this point in binding"
        )

    [<Fact>]
    member public this.Error_Type_WithoutName() =
        this.VerifyErrorBetweenMarkers(
            fileContents =
                """
type (*start*)=(*end*)
                """,
            expectedMessage = "Unexpected symbol '=' in type name"
        )

    [<Fact>]
    member public this.AbstractClasses_Constructors_PositiveTests_1() =
        this.VerifyNoErrors(
            """
[<AbstractClass>]
type C(a : int) = 
    new(a : string) = C(int a)
    new(b) = match b with Some _ -> C(1) | _ -> C("")
            """
        )

    [<Fact>]
    member public this.AbstractClasses_Constructors_PositiveTests_2() =
        this.VerifyNoErrors(
            """
[<AbstractClass>]
type C(a : int) = 
    new(a : string) = new C(int a)
    new(b) = match b with Some _ -> new C(1) | _ -> new C("")
            """
        )

    [<Fact>]
    member public this.AbstractClasses_Constructors_PositiveTests_3() =
        this.VerifyNoErrors(
            """
[<AbstractClass>]
type O(o : int) = 
    new() = O(1)
            """
        )

    [<Fact>]
    member public this.AbstractClasses_Constructors_PositiveTests_4() =
        this.VerifyNoErrors(
            """
[<AbstractClass>]
type O(o : int) = 
    new() = O() then printfn "A"
            """
        )

    [<Fact>]
    member public this.AbstractClasses_Constructors_PositiveTests_5() =
        this.VerifyNoErrors(
            """
[<AbstractClass>]
type O(o : int) = 
    new() = new O(1) then printfn "A"
            """
        )

    [<Fact>]
    member public this.AbstractClasses_Constructors_PositiveTests_6() =
        this.VerifyNoErrors(
            """
[<AbstractClass>]
type D() = class end
[<AbstractClass>]
type E = 
    inherit D
    new() = { inherit D(); }
            """
        )

    [<Fact>]
    member public this.AbstractClasses_Constructors_NegativeTests_1() =
        this.VerifyErrorAtMarker(
            fileContents =
                """
[<AbstractClass>]
type D = 
    val d : D
    new() = {d = D()}
            """,
            expectedMarker = "D()"
        )

    [<Fact>]
    member public this.AbstractClasses_Constructors_NegativeTests_2() =
        this.VerifyErrorAtMarker(
            fileContents =
                """
[<AbstractClass>]
type Z () = 
    new(a : int) = 
        Z(10) then ignore(Z())
            """,
            expectedMarker = "Z()"
        )

    [<Fact>]
    member public this.AbstractClasses_Constructors_NegativeTests_3() =
        this.VerifyErrorAtMarker(
            fileContents =
                """
[<AbstractClass>]
type X() = 
    member val V : bool = true
    abstract member Test: int -> unit

type Y() =
    member val M : bool = (new X()).V
            """,
            expectedMarker = "new X()"
        )

    [<Fact>]
    member public this.Warning_Construct_TypeMatchWithoutAnnotation() =
        this.VerifyWarningBetweenMarkers(
            fileContents =
                """
let f () = 
    let g1 (x:'a) = x
    let g2 (y:'a) = ((*start*)y(*end*):string)
    g1 3, g1 "3", g2 "4" 
                """,
            expectedMessage =
                "This construct causes code to be less generic "
                + "than indicated by the type annotations. The "
                + "type variable 'a has been constrained to be "
                + "type 'string'."
        )

    [<Fact>]
    member public this.Error_Identifer_IllegalFloatPointLiteral() =
        this.VerifyErrorBetweenMarkers(
            fileContents =
                """
let x: float = 1.2(*start*).(*end*)3
            """,
            expectedMessage = "Missing qualification after '.'"
        )

    [<Fact>]
    member public this.Error_TypeCheck_ParseError_Bug67133() =
        this.VerifyErrorBetweenMarkers(
            fileContents =
                """
let gDateTime (arr: (*start*)DateTime(*end*)[]) =
    arr.[0]
            """,
            expectedMessage = "The type 'DateTime' is not defined."
        )

    [<Fact>]
    member public this.Error_CyclicalDeclarationDoesNotCrash() =
        this.VerifyErrorBetweenMarkers(
            fileContents =
                """
type (*start*)A(*end*) = int * A
            """,
            expectedMessage = "This type definition involves an immediate cyclic reference through an abbreviation"
        )

    [<Fact>]
    member public this.Warning_FlagsAndSettings_TargetOptionsRespected() =
        this.VerifyWarningBetweenMarkers(
            fileContents =
                """
[<System.Obsolete("x")>]
let fn x = 0
let y = (*start*)fn(*end*) 1
            """,
            expectedMessage = "This construct is deprecated. x"
        )

    [<Fact>]
    member public this.Basic_Case() =
        this.VerifyErrorBetweenMarkers(
            fileContents =
                """
let x = 3
let y = (*start*)x(*end*) 4
let arr = [| 1; 2; 3 |]
            """,
            expectedMessage = "This value is not a function and cannot be applied."
        )

    [<Fact>]
    member public this.Multiline_Bug5449() =
        this.VerifyErrorBetweenMarkers(
            fileContents =
                """
let f x = x + 1
let r = (*start*)f 3(*end*) 4
            """,
            expectedMessage = "This value is not a function and cannot be applied."
        )

    [<Fact>]
    member public this.InComputationExpression_Bug6095_A() =
        this.VerifyWarningBetweenMarkers(
            fileContents =
                """
let a = async {
    let! (*start*)[| r1; r2 |](*end*) = Async.Parallel [| async.Return(1); async.Return(2) |]
    let yyyy = 4
    return r1,r2
}
            """,
            expectedMessage =
                "Incomplete pattern matches on this expression. For example, the value '[|_; _; _|]' may indicate a case not covered by the pattern(s)."
        )

    [<Fact>]
    member public this.InComputationExpression_Bug6095_B() =
        this.VerifyWarningBetweenMarkers(
            fileContents =
                """
let f = (*start*)function(*end*) | [| a;b |] -> ()
            """,
            expectedMessage =
                "Incomplete pattern matches on this expression. For example, the value '[|_; _; _|]' may indicate a case not covered by the pattern(s)."
        )

    [<Fact>]
    member public this.InComputationExpression_Bug6095_C() =
        this.VerifyWarningBetweenMarkers(
            fileContents =
                """
for (*start*)[|a;b|](*end*) in [| [|42|] |] do ()
            """,
            expectedMessage =
                "Incomplete pattern matches on this expression. For example, the value '[|_; _; _|]' may indicate a case not covered by the pattern(s). Unmatched elements will be ignored."
        )

    [<Fact>]
    member public this.InComputationExpression_Bug914685() =
        this.VerifyErrorAtMarker(
            fileContents =
                """
async { if true then return 1 } |> ignore
            """,
            expectedMarker = "if true then"
        )

    [<Fact>]
    member public this.ExtraEndif() =
        this.VerifyErrorBetweenMarkers(
            fileContents =
                """
#if UNDEFINED
1
#else
0
#endif
(*start*)#endif(*end*)
            """,
            expectedMessage = "#endif has no matching #if in implementation file"
        )

    [<Fact>]
    member public this.Squiggles_HashNotFirstSymbol_If() =
        this.VerifyErrorAtMarker(
            fileContents =
                """
(*comment*) #if UNDEFINED
1
#else
0
#endif
            """,
            expectedMarker = " #if UNDEFINED",
            expectedMessage = "#if directive must appear as the first non-whitespace character on a line"
        )

    [<Fact>]
    member public this.Squiggles_HashNotFirstSymbol_Endif() =
        this.VerifyErrorAtMarker(
            fileContents =
                """
#if DEBUG
1
#else
0
(*comment*) #endif
            """,
            expectedMarker = " #endif",
            expectedMessage = "#endif directive must appear as the first non-whitespace character on a line"
        )

    [<Fact>]
    member public this.Squiggles_HashIfWithMultilineComment() =
        this.VerifyErrorAtMarker(
            fileContents =
                """
#if DEBUG (*comment*)
#endif
            """,
            expectedMarker = "(*comment*)",
            expectedMessage = "Expected single line comment or end of line"
        )

    [<Fact>]
    member public this.Squiggles_HashIfWithUnexpected() =
        this.VerifyErrorAtMarker(
            fileContents =
                """
#if DEBUG TEST
#endif
            """,
            expectedMarker = "TEST",
            expectedMessage = "Incomplete preprocessor expression"
        )

    [<Fact>]
    member public this.OverloadsAndExtensionMethodsForGenericTypes() =
        this.VerifyNoErrors(
            fileContents =
                """
open System.Linq

type T = 
    abstract Count : int -> bool
    default this.Count(_ : int) = true

    interface System.Collections.Generic.IEnumerable<int> with
        member this.GetEnumerator() : System.Collections.Generic.IEnumerator<int> = failwith "not implemented"
    interface System.Collections.IEnumerable with
        member this.GetEnumerator() : System.Collections.IEnumerator = failwith "not implemented"

let g (t : T) = t.Count()
            """
        )

    [<Fact>]
    member public this.DocumentDiagnosticsDontReportProjectErrors_Bug1596() =
        // https://github.com/dotnet/fsharp/issues/1596
        this.VerifyNoErrors(
            fileContents =
                """
let x = 3
printf "%d" x
            """,
            additionalFlags = [| "--times" |]
        )
