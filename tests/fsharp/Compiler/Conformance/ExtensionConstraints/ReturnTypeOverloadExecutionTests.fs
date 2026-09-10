// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// RFC FS-1043 (@gusty): return-type-directed overload resolution via [<AllowOverloadOnReturnType>].
//
// WHY THIS TEST LIVES IN FSharpSuite (and not ComponentTests):
//   The ComponentTests test host runs FSharp.Core 10.0.0.0, which does NOT contain the new
//   AllowOverloadOnReturnTypeAttribute. There, the feature can only be checked for graceful
//   degradation (FS0039) via `compileAndRunOrExpectMissingAttribute`, so the feature is never
//   actually executed. FSharpSuite.Tests, in contrast, ProjectReferences FSharp.Core.fsproj and
//   compiles with --langversion:preview, so its process hosts the freshly BUILT FSharp.Core
//   11.0.0.0 which DOES contain the attribute. A snippet compiled+run from this process therefore
//   references 11.0.0.0 and genuinely exercises return-type-directed overloading at runtime.
//   This is the one executing proof of the feature; the runtime `if ... then failwith` assertions
//   verify the produced values, so a silent regression fails loudly here.

namespace FSharp.Compiler.UnitTests

open Xunit
open FSharp.Test.Compiler

module ReturnTypeOverloadExecutionTests =

    [<Fact>]
    let ``Return-type-directed overloading runs through SRTP and extension members`` () =
        FSharp """
module ReturnTypeOverloadExecution

// ---- AllowOverloadOnReturnType through SRTP ----
// Resolution goes through an inline SRTP constraint; the return type selects the overload.

type Converter2 =
    [<AllowOverloadOnReturnType>]
    static member Convert(x: int) : float = float x
    [<AllowOverloadOnReturnType>]
    static member Convert(x: int) : string = string x

let inline convert (x: int) : ^U =
    ((^U or Converter2) : (static member Convert: int -> ^U) x)

let r4: float = convert 42
if r4 <> 42.0 then failwith $"Expected 42.0, got {r4}"

let r5: string = convert 42
if r5 <> "42" then failwith $"Expected '42', got '{r5}'"

// ---- AllowOverloadOnReturnType on an EXTENSION member through SRTP ----
// The return-type-overloaded candidates are optional extension members in a separate module.

module Domain =
    type Meters = { V: float }

module Extensions =
    open Domain
    type Meters with
        [<AllowOverloadOnReturnType>]
        static member inline Of (x: int) : Meters = { V = float x }
        [<AllowOverloadOnReturnType>]
        static member inline Of (x: int) : string = $"{x}m"

open Domain
open Extensions

let inline meters (x: int) : ^U = ((^U or Domain.Meters) : (static member Of: int -> ^U) x)

let r6: Meters = meters 7
if r6 <> { V = 7.0 } then failwith $"Expected 7.0, got {r6.V}"

let r7: string = meters 7
if r7 <> "7m" then failwith $"Expected '7m', got '{r7}'"

[<EntryPoint>]
let main _ =
    printfn "return-type-directed overloading executed"
    0
"""
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed
        |> ignore
