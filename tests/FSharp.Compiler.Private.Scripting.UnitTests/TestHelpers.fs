// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Scripting.UnitTests

open System
open FSharp.Compiler.Interactive.Shell
open FSharp.Compiler.SourceCodeServices

[<AutoOpen>]
module TestHelpers =

    let getValue ((value: Result<FsiValue option, exn>), (errors: FSharpErrorInfo[])) =
        if errors.Length > 0 then
            failwith <| sprintf "Evaluation returned %d errors:\r\n\t%s" errors.Length (String.Join("\r\n\t", errors))
        match value with
        | Ok(value) -> value
        | Error ex -> raise ex

    let ignoreValue = getValue >> ignore
