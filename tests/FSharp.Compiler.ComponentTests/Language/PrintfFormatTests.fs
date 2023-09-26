﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module Language.PrintfFormatTests

open Xunit
open FSharp.Test.Compiler
open StructuredResultsAsserts

[<Fact>]
let ``Constant defined in C# can be used as printf format``() =
    let csLib = 
        CSharp """
public static class Library
{
    public const string Version = "1.0.0";
}"""    |> withName "CsLib"

    FSharp """
module PrintfFormatTests

let printLibraryVersion () = printfn Library.Version
    """
    |> withLangVersion80
    |> withReferences [csLib]
    |> compile
    |> shouldSucceed

[<Fact>]
let ``Non-inline literal can be used as printf format, type matches``() =
    FSharp """
module PrintfFormatTests

[<Literal>]
let Format = "%d"

if sprintf Format (Format.Length) <> "2" then
    failwith "failed"
    """
    |> withLangVersion80
    |> asExe
    |> compileAndRun
    |> shouldSucceed

[<Fact>]
let ``Non-inline literal can be used as printf format, type does not match``() =
    FSharp """
module PrintfFormatTests

[<Literal>]
let Format = "%s"

let test = sprintf Format 42
    """
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withResult {
        Error = Error 1
        Range = { StartLine = 7
                  StartColumn = 27
                  EndLine = 7
                  EndColumn = 29 }
        Message = "This expression was expected to have type
    'string'    
but here has type
    'int'    "
        }

[<Fact>]
let ``Non-inline literals cannot be used as printf format in lang version70``() =
    let csLib = 
        CSharp """
public static class Library
{
    public const string Version = "1.0.0";
}"""    |> withName "CsLib"

    FSharp """
module PrintfFormatTests

[<Literal>]
let Format = "%s%d%s"

let bad1 = sprintf Format "yup" Format.Length (string Format.Length)
let ok1 = sprintf "%s" Format
let bad2 = sprintf Library.Version
    """
    |> withLangVersion70
    |> withReferences [csLib]
    |> compile
    |> shouldFail
    |> withDiagnostics [
        (Error 3350, Line 7, Col 20, Line 7, Col 26, "Feature 'String values marked as literals and IL constants as printf format' is not available in F# 7.0. Please use language version 8.0 or greater.")
        (Error 3350, Line 9, Col 20, Line 9, Col 35, "Feature 'String values marked as literals and IL constants as printf format' is not available in F# 7.0. Please use language version 8.0 or greater.")
    ]