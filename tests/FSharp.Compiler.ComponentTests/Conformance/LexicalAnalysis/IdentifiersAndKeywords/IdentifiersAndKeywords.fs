// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Conformance.LexicalAnalysis

open System.IO

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module IdentifiersAndKeywords =

    let shouldFailWithDiagnostics expectedDiagnostics compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics expectedDiagnostics

    let shouldSucceedWithDiagnostics expectedDiagnostics compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics expectedDiagnostics

    let compileAndRunAsFsxShouldSucceed compilation =
        compilation
        |> asFsx
        |> withNoWarn 988
        |> runFsi
        |> shouldSucceed

    let compileAndRunAsExeShouldSucceed compilation =
        compilation
        |> withNoWarn 988
        |> asFs
        |> compileExeAndRun
        |> shouldSucceed

    [<FileInlineData("backtickmoduleandtypenames.fsx")>]
    [<FileInlineData("ValidIdentifier01.fsx")>]
    [<FileInlineData("ValidIdentifier02.fsx")>]
    [<Theory>]
    let ``AsFsx`` compilation =
        compilation
        |> getCompilation
        |> compileAndRunAsFsxShouldSucceed

    [<FileInlineData("backtickmoduleandtypenames.fsx")>]
    [<FileInlineData("ValidIdentifier01.fsx")>]
    [<FileInlineData("ValidIdentifier02.fsx")>]
    [<Theory>]
    let ``AsExe`` compilation =
        compilation
        |> getCompilation
        |> withNoWarn 3370
        |> compileAndRunAsExeShouldSucceed

    [<Theory; FileInlineData("E_InvalidIdentifier01.fsx")>]
    let ``E_InvalidIdentifier01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 1156, Line 7, Col 5, Line 7, Col 19, "This is not a valid numeric literal. Valid numeric literals include 1, 0x1, 0o1, 0b1, 1l (int/int32), 1u (uint/uint32), 1L (int64), 1UL (uint64), 1s (int16), 1us (uint16), 1y (int8/sbyte), 1uy (uint8/byte), 1.0 (float/double), 1.0f (float32/single), 1.0m (decimal), 1I (bigint).")
        ]

    [<Theory; FileInlineData("E_KeywordIdent01.fsx")>]
    let ``E_KeywordIdent01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 10, Line 7, Col 5, Line 7, Col 9, "Unexpected keyword 'type' in binding")
            (Error 10, Line 9, Col 7, Line 9, Col 12, "Unexpected keyword 'class' in binding. Expected '=' or other token.")
        ]

    [<Theory; FileInlineData("E_MissingQualification.fsx")>]
    let ``E_MissingQualification_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 599, Line 8, Col 2, Line 8, Col 3, "Missing qualification after '.'")
        ]

    [<Theory; FileInlineData("E_NameCollision01.fsx")>]
    let ``E_NameCollision01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 37, Line 11, Col 5, Line 11, Col 6, "Duplicate definition of value 'x'")
        ]

    [<Theory; FileInlineData("E_QuotedTypeModuleNames01.fsx")>]
    let ``E_QuotedTypeModuleNames01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 883, Line 33, Col 6, Line 33, Col 23, "Invalid namespace, module, type or union case name")
            (Error 883, Line 34, Col 6, Line 34, Col 23, "Invalid namespace, module, type or union case name")
            (Error 883, Line 35, Col 6, Line 35, Col 23, "Invalid namespace, module, type or union case name")
            (Error 883, Line 36, Col 6, Line 36, Col 23, "Invalid namespace, module, type or union case name")
            (Error 883, Line 37, Col 6, Line 37, Col 23, "Invalid namespace, module, type or union case name")
            (Error 883, Line 38, Col 6, Line 38, Col 23, "Invalid namespace, module, type or union case name")
            (Error 883, Line 41, Col 6, Line 41, Col 23, "Invalid namespace, module, type or union case name")
            (Error 883, Line 42, Col 6, Line 42, Col 23, "Invalid namespace, module, type or union case name")
            (Error 883, Line 44, Col 8, Line 44, Col 26, "Invalid namespace, module, type or union case name")
            (Error 883, Line 45, Col 8, Line 45, Col 26, "Invalid namespace, module, type or union case name")
            (Error 883, Line 46, Col 8, Line 46, Col 26, "Invalid namespace, module, type or union case name")
            (Error 883, Line 47, Col 8, Line 47, Col 26, "Invalid namespace, module, type or union case name")
            (Error 883, Line 48, Col 8, Line 48, Col 26, "Invalid namespace, module, type or union case name")
            (Error 883, Line 49, Col 8, Line 49, Col 26, "Invalid namespace, module, type or union case name")
            (Error 883, Line 52, Col 8, Line 52, Col 26, "Invalid namespace, module, type or union case name")
            (Error 883, Line 53, Col 8, Line 53, Col 26, "Invalid namespace, module, type or union case name")
            (Error 883, Line 55, Col 6, Line 55, Col 23, "Invalid namespace, module, type or union case name")
            (Error 883, Line 56, Col 6, Line 56, Col 23, "Invalid namespace, module, type or union case name")
            (Error 883, Line 58, Col 8, Line 58, Col 26, "Invalid namespace, module, type or union case name")
            (Error 883, Line 59, Col 8, Line 59, Col 26, "Invalid namespace, module, type or union case name")
        ]

    [<Theory; FileInlineData("W_ReservedIdentKeywords.fsx")>]
    let ``W_ReservedIdentKeywords_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Warning 46, Line 5, Col 5, Line 5, Col 10, "The identifier 'break' is reserved for future use by F#")
            (Warning 46, Line 6, Col 5, Line 6, Col 12, "The identifier 'checked' is reserved for future use by F#")
            (Warning 46, Line 7, Col 5, Line 7, Col 14, "The identifier 'component' is reserved for future use by F#")
            (Warning 46, Line 9, Col 5, Line 9, Col 15, "The identifier 'constraint' is reserved for future use by F#")
            (Warning 46, Line 11, Col 5, Line 11, Col 13, "The identifier 'continue' is reserved for future use by F#")
            (Warning 46, Line 14, Col 5, Line 14, Col 9, "The identifier 'fori' is reserved for future use by F#")
            (Warning 46, Line 16, Col 5, Line 16, Col 12, "The identifier 'include' is reserved for future use by F#")
            (Warning 46, Line 19, Col 5, Line 19, Col 10, "The identifier 'mixin' is reserved for future use by F#")
            (Warning 46, Line 21, Col 5, Line 21, Col 13, "The identifier 'parallel' is reserved for future use by F#")
            (Warning 46, Line 22, Col 5, Line 22, Col 11, "The identifier 'params' is reserved for future use by F#")
            (Warning 46, Line 23, Col 5, Line 23, Col 12, "The identifier 'process' is reserved for future use by F#")
            (Warning 46, Line 24, Col 5, Line 24, Col 14, "The identifier 'protected' is reserved for future use by F#")
            (Warning 46, Line 25, Col 5, Line 25, Col 9, "The identifier 'pure' is reserved for future use by F#")
            (Warning 46, Line 27, Col 5, Line 27, Col 11, "The identifier 'sealed' is reserved for future use by F#")
            (Warning 46, Line 28, Col 5, Line 28, Col 13, "The identifier 'tailcall' is reserved for future use by F#")
            (Warning 46, Line 29, Col 5, Line 29, Col 10, "The identifier 'trait' is reserved for future use by F#")
            (Warning 46, Line 30, Col 5, Line 30, Col 12, "The identifier 'virtual' is reserved for future use by F#")
        ]

    [<Theory; FileInlineData("E_StructNotAllowDoKeyword.fsx")>]
    let ``E_StructNotAllowDoKeyword_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 35, Line 7, Col 3, Line 7, Col 58, "This construct is deprecated: Structs cannot contain 'do' bindings because the default constructor for structs would not execute these bindings")
        ]

    [<Theory; FileInlineData("E_ValidIdentifier03.fsx")>]
    let ``E_ValidIdentifier03_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 883, Line 10, Col 11, Line 10, Col 18, "Invalid namespace, module, type or union case name")
        ]

    [<Theory; FileInlineData("E_ValidIdentifier04.fsx")>]
    let ``E_ValidIdentifier04_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 10, Line 13, Col 6, Line 13, Col 7, "Unexpected character '±' in pattern. Expected ')' or other token.")
            (Error 583, Line 13, Col 5, Line 13, Col 6, "Unmatched '('")
            (Error 10, Line 15, Col 6, Line 15, Col 7, "Unexpected character '±' in expression")
            (Error 589, Line 15, Col 1, Line 15, Col 3, "Incomplete conditional. Expected 'if <expr> then <expr>' or 'if <expr> then <expr> else <expr>'.")
        ]

    [<Theory; FileInlineData("W_IdentContainsAtSign.fsx")>]
    let ``W_IdentContainsAtSign_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldSucceedWithDiagnostics [
            (Warning 1104, Line 5, Col 5, Line 5, Col 16, "Identifiers containing '@' are reserved for use in F# code generation")
        ]
