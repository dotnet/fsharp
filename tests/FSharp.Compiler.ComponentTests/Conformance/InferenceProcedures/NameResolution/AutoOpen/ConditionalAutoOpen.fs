// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module Conformance.InferenceProcedures.ConditionalAutoOpen

open Xunit
open FSharp.Test.Compiler

#if NETCOREAPP

let private provider condition =
    FSharp $"""
namespace Providers
module Classic = let selected = 1
module Modern = let selected = 2
[<assembly: AutoOpen("Providers.Classic")>]
[<assembly: ConditionalAutoOpen("Providers.Modern", "{condition}")>]
[<assembly: RetractExistingAutoopen("Providers.Classic", "--checked+")>]
do ()
"""
    |> asLibrary
    |> withFSharpCoreShippedNet
    |> withName "Provider"

let private consumer condition (flags: string) declarations =
    FSharp $"""
module Consumer
{declarations}
[<EntryPoint>]
let main _ = if Providers.Classic.selected + Providers.Modern.selected = 3 then 0 else 1
"""
    |> withFSharpCoreShippedNet
    |> withReferences [ provider condition ]
    |> withLangVersionPreview
    |> withOptions (flags.Split([| ' ' |], System.StringSplitOptions.RemoveEmptyEntries) |> List.ofArray)

[<Theory>]
[<InlineData("--features:mode=on", "", "if selected <> 1 then failwith \"Default\"")>]
[<InlineData("--features:mode=on", "--features:mode=on", "if selected <> 2 then failwith \"Selected\"")>]
[<InlineData("--features:mode=on", "--features:mode=off --features:mode=on", "if selected <> 2 then failwith \"Membership\"")>]
[<InlineData("--features:mode=on", "--features:mode=on", "open Providers.Classic\nif selected <> 1 then failwith \"Later open\"")>]
[<InlineData("--features:mode=on", "--features:other=1,mode=on", "if selected <> 1 then failwith \"Split\"")>]
[<InlineData("--features:mode=on", "--features:mode=ON", "if selected <> 1 then failwith \"Case\"")>]
[<InlineData("--define:MODE", "-d:MODE", "if selected <> 1 then failwith \"Alias\"")>]
[<InlineData("--checked-", "", "if selected <> 1 then failwith \"Default value\"")>]
[<InlineData("--checked-", "--checked-", "if selected <> 2 then failwith \"Other option\"")>]
[<InlineData("--features:mode=on", "--features:mode=on --langversion:11.2", "if selected <> 1 then failwith \"Language version\"")>]
let ``ConditionalAutoOpen matches one complete CLI token`` (condition: string, flags: string, declarations: string) =
    consumer condition flags declarations |> compileExeAndRun |> shouldSucceed

[<Fact>]
let ``RetractExistingAutoopen removes only the assembly AutoOpen entry`` () =
    consumer "--features:mode=on" "--checked+" "let value = selected"
    |> compile
    |> shouldFail
    |> withErrorCode 39
    |> ignore
    consumer "--features:mode=on" "--checked+" "open Providers.Classic\nif selected <> 1 then failwith \"Explicit open\""
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``ConditionalAutoOpen applies to a late FSI reference`` () =
    let path = (provider "--features:mode=on" |> withName "InteractiveProvider" |> compile |> shouldSucceed).OutputPath.Value
    Fsx $"""
let selected = 3
#r @"{path}"
if selected <> 2 then failwithf "Selected %%d" selected
"""
    |> withLangVersionPreview
    |> withOptions [ "--features:mode=on" ]
    |> runFsi
    |> shouldSucceed
#endif
