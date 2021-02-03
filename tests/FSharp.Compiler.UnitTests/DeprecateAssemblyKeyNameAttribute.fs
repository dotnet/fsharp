// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.CompilerUnitTests.AssemblySigningAttributes

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module warn =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/warn)
    //<Expects status="warning" span="(11,14)" id="FS0052">The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/warn", Includes=[|"warn5_level5w.fs"|])>]
    let ``warn - warn5_level5w.fs - --warn:5`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warn:5"]
        |> compile
        |> shouldFail
        |> withWarningCode 0052
        |> withDiagnosticMessageMatches "The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed$"
        |> ignore







// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp
module MyOne

open System
open System.Reflection

[<assembly:AssemblyKeyNameAttribute("myContainer")>]
do ()

// Define a function to construct a message to print
let from whom =
    sprintf "from %s" whom

[<EntryPoint>]
let main argv =
    let message = from "F#" // Call the function
    printfn "Hello world %s" message
    0 // return an integer exit code