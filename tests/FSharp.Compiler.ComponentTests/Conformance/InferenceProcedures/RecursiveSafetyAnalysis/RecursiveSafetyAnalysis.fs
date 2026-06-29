// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.InferenceProcedures

open System
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module RecursiveSafetyAnalysis =

    let verifyCompileAndRun compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--nowarn:988"]
        |> compileExeAndRun

    // SOURCE=E_DuplicateRecursiveRecords.fs SCFLAGS="--test:ErrorRanges"          # E_DuplicateRecursiveRecords.fs
    [<Theory; FileInlineData("E_DuplicateRecursiveRecords.fs")>]
    let``E_DuplicateRecursiveRecords_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 37, Line 10, Col 5, Line 10, Col 8, "Duplicate definition of type, exception or module 'Foo'")
        ]

    [<Theory; FileInlineData("E_RecursiveInline.fs")>]
    let ``E_RecursiveInline_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3888, Line 7, Col 16, Line 7, Col 20, "The value or member 'test' has been marked 'inline' but is part of a recursive binding group. F# does not support recursive 'inline' values. Either remove the 'inline' modifier or refactor the recursion.")
        ]

    // SOURCE=E_TypeDeclaration01.fs SCFLAGS="--langversion:8.0 --test:ErrorRanges" COMPILE_ONLY=1	# E_TypeDeclaration01.fs
    [<Theory; FileInlineData("E_TypeDeclaration01.fs")>]
    let ``E_TypeDeclaration01`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 193, Line 15, Col 14, Line 15, Col 20, "Type constraint mismatch. The type \n    'int * 'a'    \nis not compatible with type\n    'string'    \n")
        ]

    //<Expects status="error" id="FS0193" span="(21,27-21,28)">Type constraint mismatch</Expects>
    [<Theory; FileInlineData("E_TypeDeclaration02.fs")>]
    let ``E_TypeDeclaration02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 20, Col 48, Line 20, Col 49, "This expression was expected to have type\n    'myint<'a>'    \nbut here has type\n    'SfsIntTerm<'c>'    ")
            (Error 1, Line 21, Col 27, Line 21, Col 28, "The type 'myint<'d>' is not compatible with the type 'SfsIntTerm<'a>'")
            (Error 1, Line 21, Col 29, Line 21, Col 30, "The type 'myint<'d>' is not compatible with the type 'SfsIntTerm<'b>'")
            (Error 193, Line 21, Col 27, Line 21, Col 28, "Type constraint mismatch. The type \n    'myint<'d>'    \nis not compatible with type\n    'SfsIntTerm<'a>'    \n")
        ]

    // SOURCE=E_VariationsOnRecursiveStruct.fs SCFLAGS="--test:ErrorRanges"     # E_VariationsOnRecursiveStruct.fs
    [<Theory; FileInlineData("E_VariationsOnRecursiveStruct.fs")>]
    let ``E_VariationsOnRecursiveStruct_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 954, Line 6, Col 6, Line 6, Col 8, "This type definition involves an immediate cyclic reference through a struct field or inheritance relation")
            (Error 912, Line 6, Col 6, Line 6, Col 8, "This declaration element is not permitted in an augmentation")
        ]

    [<Theory; FileInlineData("InfiniteRecursiveExplicitConstructor.fs")>]
    let ``InfiniteRecursiveExplicitConstructor_fs`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> compile
        |> shouldSucceed

    // SOURCE=RecursiveTypeDeclarations01.fs                        # RecursiveTypeDeclarations01.fs
    [<Theory; FileInlineData("RecursiveTypeDeclarations01.fs")>]
    let ``RecursiveTypeDeclarations01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // SOURCE=RecursiveValueDeclarations01.fs                        # RecursiveValueDeclarations01.fs
    [<Theory; FileInlineData("RecursiveValueDeclarations01.fs")>]
    let ``RecursiveValueDeclarations01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // SOURCE=RecursiveTypeDeclarations02.fs                        # RecursiveTypeDeclarations02.fs
    [<Theory; FileInlineData("RecursiveTypeDeclarations02.fs")>]
    let ``RecursiveTypeDeclarations02_fs`` compilation =
        compilation
        |> getCompilation
        |> withOptions ["--nowarn:3370"]
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Recursive inline member emits single clear diagnostic`` () =
        let source = "module M\ntype C() as self = member inline _.X = self.X"
        let diags =
            FSharp source
            |> compile
            |> fun r -> r.Output.Diagnostics
        Assert.Equal(1, diags.Length)
        Assert.Contains("recursive", diags.[0].Message, StringComparison.OrdinalIgnoreCase)

    [<Fact>]
    let ``Mutual recursive inline members emit clear diagnostic`` () =
        let source = """module M
type C() as self =
    member inline _.X = self.Y
    member inline _.Y = self.X"""
        let diags =
            FSharp source
            |> compile
            |> fun r -> r.Output.Diagnostics
        let recursiveErrs =
            diags
            |> List.filter (fun d -> d.Error = Error 3888)
        Assert.Equal(2, recursiveErrs.Length)

    [<Fact>]
    let ``Module-level recursive inline emits clear diagnostic`` () =
        let source = "module M\nlet rec inline f x = f x"
        let diags =
            FSharp source
            |> compile
            |> fun r -> r.Output.Diagnostics
        Assert.Equal(1, diags.Length)
        Assert.Contains("recursive", diags.[0].Message, StringComparison.OrdinalIgnoreCase)

    [<Fact>]
    let ``Non-recursive inline in rec group compiles cleanly`` () =
        let source = "module M\nlet rec inline f x = x + 1"
        FSharp source
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Inline CE builder members do not trigger recursive diagnostic`` () =
        let source = """module M
type B() =
    member inline _.Bind(x, f) = f x
    member inline _.Return(x) = x"""
        let diags =
            FSharp source
            |> compile
            |> fun r -> r.Output.Diagnostics
        let recursiveErrs =
            diags
            |> List.filter (fun d -> d.Message.IndexOf("recursive", StringComparison.OrdinalIgnoreCase) >= 0)
        Assert.Empty(recursiveErrs)

    [<Fact>]
    let ``Inline binding cycling through a non-inline sibling compiles cleanly`` () =
        // 'f' is inline but the cycle f -> g -> f passes through non-inline 'g'; inlining
        // terminates at 'g', so this is valid and must not be flagged as recursive inline.
        let source = "module M\nlet rec inline f x = g x\nand g x = f x"
        FSharp source
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Terminating recursion through a non-inline sibling compiles cleanly`` () =
        let source = "module M\nlet rec inline f n = if n <= 0 then 0 else g (n - 1)\nand g n = f n\nlet result = f 10"
        FSharp source
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Transitive inline cycle through inline siblings emits diagnostic`` () =
        let source = "module M\nlet rec inline f x = g x\nand inline g x = h x\nand inline h x = f x"
        let diags =
            FSharp source
            |> compile
            |> fun r -> r.Output.Diagnostics
        let recursiveErrs = diags |> List.filter (fun d -> d.Error = Error 3888)
        Assert.NotEmpty(recursiveErrs)

    [<Fact>]
    let ``Member cycling through a non-inline member compiles cleanly`` () =
        let source = """module M
type C() as self =
    member inline _.X = self.Y
    member _.Y = self.X"""
        let diags =
            FSharp source
            |> compile
            |> fun r -> r.Output.Diagnostics
        let recursiveErrs = diags |> List.filter (fun d -> d.Error = Error 3888)
        Assert.Empty(recursiveErrs)

    [<Fact>]
    let ``Nested recursive inline inside member is diagnosed once`` () =
        let source = """module M
type C() =
    member _.M (x: bool) =
        let rec inline loop y = loop y
        loop x"""
        let diags =
            FSharp source
            |> compile
            |> fun r -> r.Output.Diagnostics
        let recursiveErrs = diags |> List.filter (fun d -> d.Error = Error 3888)
        Assert.Equal(1, recursiveErrs.Length)

