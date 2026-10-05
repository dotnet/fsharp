// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Signatures

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open TestFramework

/// Tests for Signature conformance - migrated from tests/fsharpqa/Source/Conformance/Signatures/SignatureConformance/
module SignatureConformance =

    let private resourcePath = __SOURCE_DIRECTORY__ ++ ".." ++ ".." ++ "resources" ++ "tests" ++ "Conformance" ++ "Signatures" ++ "SignatureConformance"

    // Regression test for DevDiv:266717 - "Unable to compile .fs/.fsi with literal values"
    // SOURCE="Literal01.fsi Literal01.fs" SCFLAGS=-a
    [<Fact>]
    let ``Literal01 - literal values in signature`` () =
        FsFromPath (resourcePath ++ "Literal01.fsi")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "Literal01.fs"))
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // Verify ability to use FSI files in conjunction with internal types
    // SOURCE="InternalAccessibility01.fsi InternalAccessibility01.fs"
    [<Fact>]
    let ``InternalAccessibility01 - internal types with signature`` () =
        FsFromPath (resourcePath ++ "InternalAccessibility01.fsi")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "InternalAccessibility01.fs"))
        |> compile
        |> shouldSucceed
        |> ignore

    // Regression test for FSHARP1.0:4155 - combined accessibilities internal --> internal give "private"
    // SOURCE="InternalAccessibility02.fsi InternalAccessibility02.fs" SCFLAGS="--warnaserror+"
    // Note: Original test uses PRECMD to generate .fsi from --sig. This test compiles standalone.
    // When compiled as part of a multi-file compilation, this code works - so we use asExe to test it
    [<Fact>]
    let ``InternalAccessibility02 - combined internal accessibilities`` () =
        FsFromPath (resourcePath ++ "InternalAccessibility02.fs")
        |> withOptions ["--warnaserror+"]
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    // Verify ability to use FSI files in conjunction with internal types (interface case)
    // SOURCE="ImplementsComparable.fsi ImplementsComparable.fs"
    [<Fact>]
    let ``ImplementsComparable - interface hidden by signature`` () =
        FsFromPath (resourcePath ++ "ImplementsComparable.fsi")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "ImplementsComparable.fs"))
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> ignore

    // Regression for FSHARP1.0:5852 - Overly strict checks when a type is implicitly hidden by signature
    // SOURCE="InternalAccessibility03.fsi InternalAccessibility03.fs"
    [<Fact>]
    let ``InternalAccessibility03 - implicit internal types`` () =
        FsFromPath (resourcePath ++ "InternalAccessibility03.fsi")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "InternalAccessibility03.fs"))
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> ignore

    // Regression for 5618 - bad error message when LSS doesn't implement member Bar
    // SOURCE="MissingMethodInImplementation01.fsi MissingMethodInImplementation01.fs" SCFLAGS="--test:ErrorRanges"
    // <Expects status="error" span="(7,8-7,13)" id="FS0193">Module 'MyNS\.File2' requires a value 'member File2\.LSS\.Bar: string -> int'$</Expects>
    [<Fact>]
    let ``MissingMethodInImplementation01 - missing member in implementation`` () =
        FsFromPath (resourcePath ++ "MissingMethodInImplementation01.fsi")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "MissingMethodInImplementation01.fs"))
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0193
        |> withDiagnosticMessageMatches "requires a value 'member File2.LSS.Bar"
        |> ignore

    // Regression for 6446 - verifying spec matches implementation when fs/fsi files attributes differ
    // SOURCE="AttributeMatching01.fsi AttributeMatching01.fs" SCFLAGS="--test:ErrorRanges --warnaserror"
    // <Expects status="error" id="FS1200" span="(17,7-17,23)">The attribute 'ObsoleteAttribute' appears in both the implementation and the signature, but the attribute arguments differ</Expects>
    [<Fact>]
    let ``AttributeMatching01 - attribute mismatch between signature and implementation`` () =
        FsFromPath (resourcePath ++ "AttributeMatching01.fsi")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "AttributeMatching01.fs"))
        |> withOptions ["--test:ErrorRanges"; "--warnaserror"]
        |> compile
        |> shouldFail
        |> withErrorCode 1200
        |> withDiagnosticMessageMatches "ObsoleteAttribute"
        |> ignore

    // Regression for Dev11:137930 - structs used to not give errors on unimplemented constructors
    // SOURCE="E_StructConstructor01.fsi E_StructConstructor01.fs" SCFLAGS="--test:ErrorRanges"
    // <Expects status="error" id="FS0193" span="(4,8-4,9)">Module 'M' requires a value 'new: unit -> Foo<'T>'</Expects>
    [<Fact>]
    let ``E_StructConstructor01 - struct missing constructor`` () =
        FsFromPath (resourcePath ++ "E_StructConstructor01.fsi")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "E_StructConstructor01.fs"))
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0193
        |> withDiagnosticMessageMatches "requires a value"
        |> ignore

    // Regression for Dev11:137942 - structs used to not give errors when member names conflicted with interface members
    // SOURCE="E_StructWithNameConflict01.fsi E_StructWithNameConflict01.fs" SCFLAGS="--test:ErrorRanges --flaterrors"
    // <Expects status="error" span="(14,21-14,34)" id="FS0034">...</Expects>
    // <Expects status="notin" span="(18,13-18,26)" id="FS0039">The field, constructor or member 'GetEnumerator' is not defined$</Expects>
    [<Fact>]
    let ``E_StructWithNameConflict01 - struct interface member name conflict`` () =
        FsFromPath (resourcePath ++ "E_StructWithNameConflict01.fsi")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "E_StructWithNameConflict01.fs"))
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0034
        |> withDiagnosticMessageDoesntMatch "The field, constructor or member 'GetEnumerator' is not defined"
        |> ignore

    // Regression for Dev11:137942 - structs used to not give errors when member names conflicted with interface members
    // SOURCE="E_StructWithNameConflict02.fsi E_StructWithNameConflict02.fs" SCFLAGS="--test:ErrorRanges --flaterrors"
    // <Expects status="error" span="(18,13-18,26)" id="FS0039">The type 'Foo<_>' does not define a field, constructor, or member named 'GetEnumerator'</Expects>
    // <Expects status="notin" span="(14,21-14,34)" id="FS0034">...</Expects>
    [<Fact>]
    let ``E_StructWithNameConflict02 - struct undefined member from signature`` () =
        FsFromPath (resourcePath ++ "E_StructWithNameConflict02.fsi")
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "E_StructWithNameConflict02.fs"))
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> compile
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "GetEnumerator"
        |> withDiagnosticMessageDoesntMatch "The compiled names differ"
        |> ignore

    // https://github.com/dotnet/fsharp/issues/11331
    [<Fact>]
    let ``Issue 11331 - Public constructor taking internal type should report FS0410 in signature`` () =
        Fsi
            """
module WrongAccessibility.Library

type internal A = class end

type B =
    new: a: A -> B
            """
        |> withAdditionalSourceFile (
            FsSource
                """
module WrongAccessibility.Library

type internal A = class end

type B(a: A) = class end
            """
        )
        |> compile
        |> shouldFail
        |> withErrorCode 0410

    let private recordSignaturePair signature implementation =
        Fsi ("module M\n" + signature)
        |> withAdditionalSourceFile (FsSource ("module M\n" + implementation))
        |> asLibrary

    let private assertRecordDiagnostics expected (result: CompilationResult) =
        // Raw diagnostic columns are zero-based; keep repeated warnings and their order.
        let actual =
            result.Output.Diagnostics
            |> List.map (fun d ->
                d.Error, System.IO.Path.GetFileName d.NativeRange.FileName,
                (d.Range.StartLine, d.Range.StartColumn, d.Range.EndLine, d.Range.EndColumn),
                d.Message.Replace("\r\n", "\n"))
        Assert.True((expected = actual), sprintf "Expected:\n%A\nActual:\n%A" expected actual)

    let private recordOrderDiagnostic typeName line column =
        ErrorType.Error 312, "test.fs", (line, column, line, column + String.length typeName),
        $"The type definitions for type '{typeName}' in the signature and implementation are not compatible because the order of the fields is different in the signature and implementation"

    let private fieldMismatch column (implementation: string) (signature: string) (reason: string) =
        ErrorType.Error 193, "test.fs", (2, column, 2, column + 1),
        $"The module contains the field\n    {implementation}    \nbut its signature specifies\n    {signature}    \n{reason}"

    [<Theory>]
    [<InlineData(
        """type FormatConfig = FormatConfig of int
type ResolvedSetting = ResolvedSetting of int
type EditorConfigProblem = EditorConfigProblem of int
type ResolvedConfig =
    {
        Config: FormatConfig
        Settings: ResolvedSetting list
        EditorConfigFiles: string list
        Problems: EditorConfigProblem list
    }""",
        """type FormatConfig = FormatConfig of int
type ResolvedSetting = ResolvedSetting of int
type EditorConfigProblem = EditorConfigProblem of int
type ResolvedConfig =
    {
        Config: FormatConfig
        EditorConfigFiles: string list
        Problems: EditorConfigProblem list
        Settings: ResolvedSetting list
    }""", "ResolvedConfig", 5, false)>]
    [<InlineData("type R = { A: int; B: string }", "type R = { B: string; A: int }", "R", 2, false)>]
    [<InlineData("type R = { Prefix: bool; A: int; B: string; C: decimal }", "type R = { Prefix: bool; B: string; C: decimal; A: int }", "R", 2, false)>]
    [<InlineData("type R = { A: int; B: int }", "type R = { B: int; A: int }", "R", 2, false)>]
    [<InlineData("[<Struct>]\ntype R<'T> = { A: 'T; B: 'T list }", "[<Struct>]\ntype R<'T> = { B: 'T list; A: 'T }", "R", 3, false)>]
    [<InlineData("type R = { [<System.Obsolete(\"sig\")>] A: int; B: string }", "type R = { B: string; [<System.Obsolete(\"impl\")>] A: int }", "R", 2, true)>]
    let ``Issue 20410 - record field permutations retain only genuine diagnostics`` signature implementation typeName line attributeConflict =
        let result = recordSignaturePair signature implementation |> compile |> shouldFail
        Assert.Contains(result.Output.Diagnostics, fun d -> d.Error = ErrorType.Error 312)
        result |> assertRecordDiagnostics [
            if attributeConflict then
                Warning 1200, "test.fs", (2, 24, 2, 47),
                "The attribute 'ObsoleteAttribute' appears in both the implementation and the signature, but the attribute arguments differ. Only the attribute from the signature will be included in the compiled code."
            recordOrderDiagnostic typeName line 5
        ]

    [<Fact>]
    let ``Issue 20410 - aligned record fields compile`` () =
        let declaration = "type R = { A: int; B: string }"
        recordSignaturePair declaration declaration
        |> compile
        |> shouldSucceed
        |> assertRecordDiagnostics []

    [<Theory>]
    [<InlineData("type R = { A: string; B: string }", "A: string", 11, "The types differ")>]
    [<InlineData("type R = { mutable A: int; B: string }", "mutable A: int", 19, "The 'mutable' modifiers differ")>]
    [<InlineData("type R = internal { A: int; B: string }", "A: int", 20, "the accessibility specified in the signature is more than that specified in the implementation")>]
    let ``Issue 20410 - same-name field mismatches are preserved`` implementation field column reason =
        recordSignaturePair "type R = { A: int; B: string }" implementation
        |> compile
        |> shouldFail
        |> assertRecordDiagnostics [
            if field = "A: int" then
                fieldMismatch 28 "B: string" "B: string" reason
            fieldMismatch column field "A: int" reason
        ]

    [<Theory>]
    [<InlineData("type R = { A: int }", false)>]
    [<InlineData("type R = { A: int; B: string; C: bool }", true)>]
    [<InlineData("type R = { A: int; C: string }", false)>]
    let ``Issue 20410 - different record field name sets are preserved`` implementation extra =
        let code, reason =
            if extra then 311, "C was present in the implementation but not in the signature"
            else 313, "B was required by the signature but was not specified by the implementation"
        recordSignaturePair "type R = { A: int; B: string }" implementation
        |> compile
        |> shouldFail
        |> assertRecordDiagnostics [
            ErrorType.Error code, "test.fs", (2, 5, 2, 6),
            $"The type definitions for type 'R' in the signature and implementation are not compatible because the field {reason}"
        ]

    [<Theory>]
    [<InlineData(false)>]
    [<InlineData(true)>]
    let ``Issue 20410 - matching prefix retains repeated nullness warnings`` swapped =
        let fields = if swapped then "B: bool; A: int" else "A: int; B: bool"
        let result =
            recordSignaturePair
                "type R = { Prefix: string; A: int; B: bool }"
                $"type R = {{ Prefix: string | null; {fields} }}"
            |> withLangVersionPreview
            |> withCheckNulls
            |> withWarnOn 3261
            |> compile
        if swapped then
            result |> shouldFail |> ignore
            Assert.Contains(result.Output.Diagnostics, fun d -> d.Error = ErrorType.Error 312)
        result |> assertRecordDiagnostics [
            for _ in 1..3 do
                Warning 3261, "test.fs", (2, 11, 2, 17),
                "Nullness warning: The module contains the field\n    Prefix: string | null    \nbut its signature specifies\n    Prefix: string    \nThe types differ in their nullness annotations"
            if swapped then
                recordOrderDiagnostic "R" 2 5
        ]

    [<Theory>]
    [<InlineData("type U = Case of A: int * B: string", "type U = Case of B: string * A: int", 36)>]
    [<InlineData("exception E of A: int * B: string", "exception E of B: string * A: int", 63)>]
    [<InlineData("type C =\n    val A: int\n    val B: string", "type C =\n    val B: string\n    val A: int", 0)>]
    [<InlineData("type R = { A: int; B: string }", "type R = { A: int; A: int }", 37)>]
    let ``Issue 20410 - non-record and duplicate-field behavior is preserved`` signature implementation code =
        let expected =
            match code with
            | 36 -> [
                fieldMismatch 17 "B: string" "A: int" "The names differ"
                ErrorType.Error 36, "test.fs", (2, 9, 2, 13),
                "The module contains the constructor\n    | Case of B: string * A: int    \nbut its signature specifies\n    | Case of A: int * B: string    \nThe types of the fields differ"
              ]
            | 63 -> [
                fieldMismatch 15 "B: string" "A: int" "The names differ"
                ErrorType.Error 63, "test.fs", (2, 10, 2, 11),
                "The exception definitions are not compatible because the order of the fields is different in the signature and implementation. The module contains the exception definition\n    exception E of B: string * A: int    \nbut its signature specifies\n\texception E of A: int * B: string."
              ]
            | 37 -> [ErrorType.Error 37, "test.fs", (2, 19, 2, 20), "Duplicate definition of field 'A'"]
            | 0 -> []
            | _ -> failwithf "Unexpected control diagnostic: %d" code
        let result = recordSignaturePair signature implementation |> compile
        result |> (if code = 0 then shouldSucceed else shouldFail) |> assertRecordDiagnostics expected
