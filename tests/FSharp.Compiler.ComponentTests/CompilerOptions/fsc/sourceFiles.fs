// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module CompilerOptions.Fsc.FscSourceFilesArguments

open Xunit
open FSharp.Test
open FSharp.Test.Compiler


[<Fact>]
let ``Reports duplicate sources via warning``() =
    let file = SourceCodeFileKind.Fs({FileName="test.fs"; SourceText=Some """printfn "Hello" """ })
 
    fsFromString file
    |> FS
    |> asExe
    |> withAdditionalSourceFile file
    |> compile
    |> withWarningCodes [3551]
    |> withErrorCodes []