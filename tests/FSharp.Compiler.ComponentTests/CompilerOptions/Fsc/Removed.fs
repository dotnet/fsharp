// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test.Compiler

/// Tests for removed and deprecated compiler options
module Removed =

    // ============================================================
    // Deprecated options (produce warning but still work)
    // ============================================================

    [<Fact>]
    let ``deprecated --debug-file produces warning`` () =
        Fs """module M"""
        |> withOptions ["-g"; "--debug-file:foo.pdb"]
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withWarningCode 75
        |> withDiagnosticMessageMatches "The command-line option '--debug-file' has been deprecated. Use '--pdb' instead"
        |> ignore

    [<Fact>]
    let ``deprecated --generate-filter-blocks produces warning`` () =
        Fs """module M"""
        |> withOptions ["--generate-filter-blocks"]
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withWarningCode 75
        |> withDiagnosticMessageMatches "The command-line option '--generate-filter-blocks' has been deprecated"
        |> ignore

    [<Fact>]
    let ``deprecated --gnu-style-errors produces warning`` () =
        Fs """module M"""
        |> withOptions ["--gnu-style-errors"]
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withWarningCode 75
        |> withDiagnosticMessageMatches "The command-line option '--gnu-style-errors' has been deprecated"
        |> ignore

    [<Fact>]
    let ``deprecated --max-errors with value produces warning`` () =
        Fs """module M"""
        |> withOptions ["--max-errors:1"]
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withWarningCode 75
        |> withDiagnosticMessageMatches "The command-line option '--max-errors' has been deprecated. Use '--maxerrors' instead."
        |> ignore

    [<Fact>]
    let ``deprecated --max-errors without value produces warning and error`` () =
        Fs """module M"""
        |> withOptions ["--max-errors"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 75, Line 0, Col 1, Line 0, Col 1, "The command-line option '--max-errors' has been deprecated. Use '--maxerrors' instead.")
            (Error 224, Line 0, Col 1, Line 0, Col 1, "Option requires parameter: --max-errors:<n>")
        ]
        |> ignore

    [<Fact>]
    let ``deprecated --no-string-interning produces warning`` () =
        Fs """module M"""
        |> withOptions ["--no-string-interning"]
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withWarningCode 75
        |> withDiagnosticMessageMatches "The command-line option '--no-string-interning' has been deprecated"
        |> ignore

    [<Fact>]
    let ``deprecated --statistics produces warning`` () =
        Fs """module M"""
        |> withOptions ["--statistics"]
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withWarningCode 75
        |> withDiagnosticMessageMatches "The command-line option '--statistics' has been deprecated"
        |> ignore

    // ============================================================
    // Removed options (produce unrecognized option error)
    // ============================================================

    [<Fact>]
    let ``removed -Ooff produces error`` () =
        Fs """printfn "test" """
        |> asExe
        |> withOptions ["-Ooff"]
        |> compile
        |> shouldFail
        |> withErrorCode 243
        |> withDiagnosticMessageMatches "Unrecognized option: '-Ooff'"
        |> ignore

    [<Fact>]
    let ``removed --namespace produces error`` () =
        Fs """printfn "test" """
        |> asExe
        |> withOptions ["--namespace"]
        |> compile
        |> shouldFail
        |> withErrorCode 243
        |> withDiagnosticMessageMatches "Unrecognized option: '--namespace'"
        |> ignore

    [<Fact>]
    let ``removed --namespace Foo produces error`` () =
        Fs """printfn "test" """
        |> asExe
        |> withOptions ["--namespace"; "Foo"]
        |> compile
        |> shouldFail
        |> withErrorCode 243
        |> withDiagnosticMessageMatches "Unrecognized option: '--namespace'"
        |> ignore

    [<Fact>]
    let ``removed --nopowerpack produces error`` () =
        Fs """printfn "test" """
        |> asExe
        |> withOptions ["--nopowerpack"]
        |> compile
        |> shouldFail
        |> withErrorCode 243
        |> withDiagnosticMessageMatches "Unrecognized option: '--nopowerpack'"
        |> ignore

    [<Fact>]
    let ``removed --no-power-pack produces error`` () =
        Fs """printfn "test" """
        |> asExe
        |> withOptions ["--no-power-pack"]
        |> compile
        |> shouldFail
        |> withErrorCode 243
        |> withDiagnosticMessageMatches "Unrecognized option: '--no-power-pack'"
        |> ignore

    [<Theory>]
    [<InlineData("-R")>]
    [<InlineData("--open")>]
    [<InlineData("--clr-mscorlib")>]
    [<InlineData("--quotation-data")>]
    [<InlineData("--all-tailcalls")>]
    [<InlineData("--no-tailcalls")>]
    [<InlineData("--closures-as-virtuals")>]
    [<InlineData("--multi-entrypoint-closures")>]
    [<InlineData("--generate-debug-file")>]
    [<InlineData("--no-inner-polymorphism")>]
    [<InlineData("--permit-inner-polymorphism")>]
    [<InlineData("--fast-sublanguage-only")>]
    [<InlineData("--generate-config-file")>]
    [<InlineData("--ml-keywords")>]
    [<InlineData("--no-banner")>]
    [<InlineData("--nobanner")>]
    [<InlineData("--light")>]
    [<InlineData("--indentation-syntax")>]
    [<InlineData("--no-indentation-syntax")>]
    [<InlineData("--sscli")>]
    let ``fsc - removed option produces error`` (option: string) =
        Fs """printfn "test" """
        |> asExe
        |> withOptions [option]
        |> compile
        |> shouldFail
        |> withErrorCode 243
        |> withDiagnosticMessageMatches $"Unrecognized option: '{option}'"
        |> ignore

    // ============================================================
    // FSI removed options tests (options that produce error 243)
    // Note: Some deprecated FSI options (--debug-file, --statistics, --gnu-style-errors,
    // --generate-filter-blocks, --no-string-interning) cannot be tested via the compiler
    // service as they are silently ignored - they were only checked when running FSI directly.
    // ============================================================

    [<Theory>]
    [<InlineData("--ml-keywords")>]
    [<InlineData("-R")>]
    [<InlineData("--open")>]
    [<InlineData("--clr-mscorlib")>]
    [<InlineData("--quotation-data")>]
    [<InlineData("--all-tailcalls")>]
    [<InlineData("--no-tailcalls")>]
    [<InlineData("--closures-as-virtuals")>]
    [<InlineData("--multi-entrypoint-closures")>]
    [<InlineData("--generate-debug-file")>]
    [<InlineData("--generate-config-file")>]
    [<InlineData("--no-inner-polymorphism")>]
    [<InlineData("--permit-inner-polymorphism")>]
    [<InlineData("-Ooff")>]
    [<InlineData("--no-banner")>]
    [<InlineData("--nobanner")>]
    [<InlineData("--fast-sublanguage-only")>]
    [<InlineData("--sscli")>]
    let ``fsi - removed option produces error`` (option: string) =
        Fsx """printfn "test" """
        |> withOptions [option]
        |> compile
        |> shouldFail
        |> withErrorCode 243
        |> withDiagnosticMessageMatches $"Unrecognized option: '{option}'"
        |> ignore

    [<Fact>]
    let ``fsi - deprecated --max-errors produces warning and error`` () =
        Fsx """printfn "test" """
        |> withOptions ["--max-errors"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 75, Line 0, Col 1, Line 0, Col 1, "The command-line option '--max-errors' has been deprecated. Use '--maxerrors' instead.")
            (Error 224, Line 0, Col 1, Line 0, Col 1, "Option requires parameter: --max-errors:<n>")
        ]
        |> ignore

    [<Fact>]
    let ``fsi - removed --nopowerpack produces error`` () =
        Fsx """printfn "test" """
        |> withOptions ["--nopowerpack"]
        |> compile
        |> shouldFail
        |> withErrorCode 243
        |> withDiagnosticMessageMatches "Unrecognized option: '--nopowerpack'"
        |> ignore

    [<Fact>]
    let ``fsi - removed --no-power-pack produces error`` () =
        Fsx """printfn "test" """
        |> withOptions ["--no-power-pack"]
        |> compile
        |> shouldFail
        |> withErrorCode 243
        |> withDiagnosticMessageMatches "Unrecognized option: '--no-power-pack'"
        |> ignore

    [<Fact>]
    let ``fsi - removed --namespace produces error`` () =
        Fsx """printfn "test" """
        |> withOptions ["--namespace"]
        |> compile
        |> shouldFail
        |> withErrorCode 243
        |> withDiagnosticMessageMatches "Unrecognized option: '--namespace'"
        |> ignore

    [<Fact>]
    let ``fsi - removed --namespace Foo produces error`` () =
        Fsx """printfn "test" """
        |> withOptions ["--namespace"; "Foo"]
        |> compile
        |> shouldFail
        |> withErrorCode 243
        |> withDiagnosticMessageMatches "Unrecognized option: '--namespace'"
        |> ignore
