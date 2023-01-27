// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.ComponentTests.Language.PrintfFormatTests

open Xunit
open FSharp.Test.Compiler

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
    |> withLangVersionPreview
    |> withReferences [csLib]
    |> compile
    |> shouldSucceed

[<Fact>]
let ``Non-inline literal can be used as printf format, type matches``() =
    FSharp """
module PrintfFormatTests

[<Literal>]
let Format = "%d"

let test = sprintf Format Format.Length
    """
    |> withLangVersionPreview
    |> compile
    |> shouldSucceed

[<Fact>]
let ``Non-inline literal can be used as printf format, type does not match``() =
    FSharp """
module PrintfFormatTests

[<Literal>]
let Format = "%s"

let test = sprintf Format 42
    """
    |> withLangVersionPreview
    |> compile
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