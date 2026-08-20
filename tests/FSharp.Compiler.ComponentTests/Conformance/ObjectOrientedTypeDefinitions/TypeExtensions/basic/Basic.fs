// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ObjectOrientedTypeDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module TypeExtensionsBasic =

    // Error tests

    [<Theory; FileInlineData("E_ProtectedMemberInExtensionMember01.fs")>]
    let ``E_ProtectedMemberInExtensionMember01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 491

    [<Theory; FileInlineData("E_CantExtendTypeAbbrev.fs")>]
    let ``E_CantExtendTypeAbbrev_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 964

    [<Theory; FileInlineData("E_ConflictingMembers.fs")>]
    let ``E_ConflictingMembers_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 438

    [<Theory; FileInlineData("E_InvalidExtensions01.fs")>]
    let ``E_InvalidExtensions01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 912

    [<Theory; FileInlineData("E_InvalidExtensions02.fs")>]
    let ``E_InvalidExtensions02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 912

    [<Theory; FileInlineData("E_InvalidExtensions03.fs")>]
    let ``E_InvalidExtensions03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 925

    [<Theory; FileInlineData("E_InvalidExtensions04.fs")>]
    let ``E_InvalidExtensions04_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 39

    [<Theory; FileInlineData("E_ExtensionInNamespace01.fs")>]
    let ``E_ExtensionInNamespace01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 644

    [<Theory; FileInlineData("E_ExtendVirtualMethods01.fs")>]
    let ``E_ExtendVirtualMethods01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 854

    [<Theory; FileInlineData("E_InvalidForwardRef01.fs")>]
    let ``E_InvalidForwardRef01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 430

    [<Theory; FileInlineData("E_ExtensionOperator01.fs")>]
    let ``E_ExtensionOperator01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1215

    // Success tests

    [<Theory; FileInlineData("BasicExtensions.fs")>]
    let ``BasicExtensions_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("MultipleExtensions.fs")>]
    let ``MultipleExtensions_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("UnqualifiedName.fs")>]
    let ``UnqualifiedName_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // Tests temporarily skipped due to 'allows ref struct' constraint mismatch on IEnumerable
    // [<Theory; FileInlineData("ExtendHierarchy01.fs")>]
    // let ``ExtendHierarchy01_fs`` compilation =
    //     compilation
    //     |> getCompilation
    //     |> asExe
    //     |> withLangVersionPreview
    //     |> ignoreWarnings
    //     |> compile
    //     |> shouldSucceed

    [<Theory; FileInlineData("ExtendHierarchy02.fs")>]
    let ``ExtendHierarchy02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ExtensionInNamespace02.fs")>]
    let ``ExtensionInNamespace02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ExtendWithOperator01.fs")>]
    let ``ExtendWithOperator01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("NonConflictingIntrinsicMembers.fs")>]
    let ``NonConflictingIntrinsicMembers_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ExtendViaOverloading01.fs")>]
    let ``ExtendViaOverloading01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // Tests temporarily skipped due to 'allows ref struct' constraint mismatch on IEnumerable
    // [<Theory; FileInlineData("ExtendViaOverloading02.fs")>]
    // let ``ExtendViaOverloading02_fs`` compilation =
    //     compilation
    //     |> getCompilation
    //     |> asExe
    //     |> withLangVersionPreview
    //     |> ignoreWarnings
    //     |> compile
    //     |> shouldSucceed

    [<Theory; FileInlineData("fslib.fs")>]
    let ``fslib_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("TupleTypeExtension01.fs")>]
    let ``TupleTypeExtension01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("TupleTypeExtension02.fs")>]
    let ``TupleTypeExtension02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("TupleTypeExtension03.fs")>]
    let ``TupleTypeExtension03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("TupleTypeExtension04.fs")>]
    let ``TupleTypeExtension04_fs`` compilation =
        // A tuple type extension of arity 8 (> 7) cannot be desugared to a flat System.Tuple<T1..T8>
        // that unifies with a real 8-tuple (which uses a nested TRest slot), so it is rejected with FS3915.
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3915, Line 9, Col 6, Line 9, Col 53, "Tuple type extensions are supported only for tuples of up to 7 elements, but this tuple type has 8 elements. Extensions of larger tuples are not supported.")
        ]

    [<Fact>]
    let ``Tuple type extension of arity 7 compiles`` () =
        // The maximum supported arity is 7 (goodTupleFields); a 7-tuple desugars to a flat
        // System.Tuple<T1..T7> that matches a real 7-tuple, so the extension is valid.
        FSharp """
module Test
type ('T1 * 'T2 * 'T3 * 'T4 * 'T5 * 'T6 * 'T7) with
    static member Seven ((a, _, _, _, _, _, _)) = a

let r = System.Tuple<int, int, int, int, int, int, int>.Seven((1, 2, 3, 4, 5, 6, 7))
if r <> 1 then failwith "expected 1"
        """
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Tuple type extension requires preview language version`` () =
        // Tuple-type extensions are gated behind the extension feature flag. Below preview the
        // syntax parses (it is not a parse error) but the checker rejects it with a clean
        // feature-availability diagnostic (FS3350) rather than a confusing downstream error.
        FSharp """
module Test
type ('T1 * 'T2) with
    static member PairFirst ((a, _b)) = a
        """
        |> withLangVersion80
        |> typecheck
        |> shouldFail
        |> withErrorCode 3350
        |> withDiagnosticMessageMatches "is not available in F#"

    [<Fact>]
    let ``Instance members on a tuple type extension are not resolvable`` () =
        // Tuple type extensions support static members and operators (see TupleTypeExtension01-03),
        // but NOT instance members. The instance-member definition is accepted silently, yet it
        // cannot be invoked through dot-notation on a tuple value: resolution fails with a plain
        // 'not defined' error. This pins that known limitation as a conscious, tested boundary.
        FSharp """
module Test
type (int * int) with
    member x.Foo = 42

let v = (1, 2).Foo
        """
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withErrorCode 39
        |> withDiagnosticMessageMatches "The field, constructor or member 'Foo' is not defined"

    [<Fact>]
    let ``Struct tuple type extension rewrites to ValueTuple`` () =
        // A struct tuple (type struct ('T1 * 'T2) with ...) is rewritten to System.ValueTuple.
        FSharp """
module Test
type struct ('T1 * 'T2) with
    static member Fst (struct (a, _b)) = a

let r = System.ValueTuple<int, string>.Fst(struct (42, "x"))
if r <> 42 then failwith "expected 42"
        """
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Tuple type extension in a recursive module resolves`` () =
        // The tuple-to-System.Tuple desugaring must also run in the mutually-recursive module path.
        // Without it the checker reaches type-checking with a bare tuple SynType, yielding an empty
        // long-ident and an internal error (rangeOfLid) rather than compiling the extension.
        FSharp """
module rec Test
type ('T1 * 'T2) with
    static member PairFirst ((a, _b)) = a

let r = System.Tuple<int, string>.PairFirst((42, "x"))
if r <> 42 then failwith "expected 42"
        """
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Tuple type extension in a recursive module requires preview language version`` () =
        // The feature gate must fire in the recursive path too: below preview this is a clean FS3350,
        // never an internal error that escapes the language-version check.
        FSharp """
module rec Test
type ('T1 * 'T2) with
    static member PairFirst ((a, _b)) = a
        """
        |> withLangVersion80
        |> typecheck
        |> shouldFail
        |> withErrorCode 3350
        |> withDiagnosticMessageMatches "is not available in F#"

    [<Fact>]
    let ``Tuple type extension declared in a signature file resolves`` () =
        // The desugaring must also run when the extension is declared in an explicit signature file.
        let impl = """
module Test
type ('T1 * 'T2) with
    static member PairFirst ((a, _b): 'T1 * 'T2) : 'T1 = a
"""
        Fsi """
module Test
type ('T1 * 'T2) with
    static member PairFirst : ('T1 * 'T2) -> 'T1
        """
        |> withFileName "Test.fsi"
        |> withAdditionalSourceFiles [ FsSourceWithFileName "Test.fs" impl ]
        |> asLibrary
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Tuple type extension in a signature file requires preview language version`` () =
        // The feature gate must fire on the signature-file declaration too, as a clean FS3350
        // rather than an internal error.
        let impl = """
module Test
type ('T1 * 'T2) with
    static member PairFirst ((a, _b)) = a
"""
        Fsi """
module Test
type ('T1 * 'T2) with
    static member PairFirst : ('T1 * 'T2) -> 'T1
        """
        |> withFileName "Test.fsi"
        |> withAdditionalSourceFiles [ FsSourceWithFileName "Test.fs" impl ]
        |> asLibrary
        |> withLangVersion80
        |> compile
        |> shouldFail
        |> withErrorCode 3350
        |> withDiagnosticMessageMatches "is not available in F#"

    [<Fact>]
    let ``Tuple type extension in a recursive module with a signature file resolves`` () =
        // A recursive module WITH a signature file routes through TcSignatureElementsMutRec, a
        // fourth declaration path. Without the desugaring there the bare tuple SynType reaches
        // type-checking and yields an internal error (rangeOfLid) rather than compiling.
        let impl = """
module rec Test
type ('T1 * 'T2) with
    static member PairFirst ((a, _b): 'T1 * 'T2) : 'T1 = a
"""
        Fsi """
module rec Test
type ('T1 * 'T2) with
    static member PairFirst : ('T1 * 'T2) -> 'T1
        """
        |> withFileName "Test.fsi"
        |> withAdditionalSourceFiles [ FsSourceWithFileName "Test.fs" impl ]
        |> asLibrary
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
