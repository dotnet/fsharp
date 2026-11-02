// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module Conformance.InferenceProcedures.ConditionalAutoOpen

open Xunit
open FSharp.Test.Compiler

let private provider name metadata =
    FSharp $"""
namespace Providers
type Classic() = class end
[<AutoOpen>]
module Classic = let selected = 1
module Modern = let selected = 2
[<assembly: AutoOpen("Providers.ClassicModule")>]
{metadata}
do ()
"""
    |> asLibrary
    |> withFSharpCoreShippedNet
    |> withName name

let private conditionalProvider =
    provider "ConditionalProvider" """[<assembly: ConditionalAutoOpen("Providers.Modern", "--reflectionfree")>]"""

let private consumer references options source =
    FSharp source
    |> withFSharpCoreShippedNet
    |> withReferences references
    |> withLangVersionPreview
    |> withOptions options

[<Theory>]
[<InlineData(false, "", 1)>]
[<InlineData(true, "", 2)>]
[<InlineData(true, "open Providers.Classic", 1)>]
[<InlineData(true, "let selected = 3", 3)>]
[<InlineData(true, "let selected = 3\nopen Providers.Classic", 1)>]
[<InlineData(true, "let nested () = let selected = 3 in selected\nif nested() <> 3 then failwith \"Local scope\"", 2)>]
let ``ConditionalAutoOpen uses ordinary shadowing`` (enabled: bool, declarations: string, expected: int) =
    consumer [conditionalProvider] (if enabled then ["--reflectionfree"] else []) $"""
module Consumer
{declarations}
[<EntryPoint>]
let main _ =
    if selected <> {expected} then failwith "Shadowing"
    if Providers.Classic.selected <> 1 || Providers.Modern.selected <> 2 then failwith "Qualified access"
    0
"""
    |> compileExeAndRun
    |> shouldSucceed

[<Theory>]
[<InlineData(false, false)>]
[<InlineData(true, false)>]
[<InlineData(false, true)>]
[<InlineData(true, true)>]
let ``ConditionalAutoOpen keeps ordinary assembly reference order`` (reverse, interactive) =
    let ordinary =
        FSharp """
namespace Ordinary
module Values = let selected = 9
[<assembly: AutoOpen("Ordinary.Values")>]
do ()
"""
        |> asLibrary
        |> withFSharpCoreShippedNet
        |> withName "OrdinaryProvider"
    let references = if reverse then [ordinary; conditionalProvider] else [conditionalProvider; ordinary]
    let expected = if reverse then 2 else 9
    if interactive then
        let references =
            references
            |> List.map (fun library -> $"#r @\"{(library |> compile |> shouldSucceed).OutputPath.Value}\"")
            |> String.concat "\n"
        Fsx $"{references}\nif selected <> {expected} then failwithf \"Selected %%d\" selected"
        |> withLangVersionPreview
        |> withOptions ["--reflectionfree"]
        |> runFsi
        |> shouldSucceed
    else
        consumer references ["--reflectionfree"] $"module Consumer\n[<EntryPoint>]\nlet main _ = if selected = {expected} then 0 else failwithf \"Selected %%d\" selected"
        |> compileExeAndRun
        |> shouldSucceed

[<Fact>]
let ``Retraction removes only the assembly AutoOpen entry`` () =
    let retracted = provider "RetractedProvider" """[<assembly: RetractExistingAutoopen("Providers.ClassicModule", "--reflectionfree")>]"""
    consumer [retracted] ["--reflectionfree"] "module Consumer\nlet value = selected"
    |> asLibrary
    |> compile
    |> shouldFail
    |> withErrorCode 39
    |> ignore
    for opened in ["Providers"; "Providers.Classic"] do
        consumer [retracted] ["--reflectionfree"] $"module Consumer\nopen {opened}\nlet value = selected + Providers.Classic.selected"
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

[<Theory>]
[<InlineData("--reflectionfree")>]
[<InlineData("--quiet")>]
[<InlineData("--shadowcopyreferences-")>]
[<InlineData("--multiemit+")>]
[<InlineData("--quotations-debug+")>]
let ``ConditionalAutoOpen applies to a late FSI reference`` condition =
    let library = provider ("InteractiveProvider" + condition) $"""[<assembly: ConditionalAutoOpen("Providers.Modern", "{condition}")>]"""
    let path = (library |> compile |> shouldSucceed).OutputPath.Value
    Fsx $"""
let selected = 3
#r @"{path}"
if selected <> 2 then failwithf "Selected %%d" selected
open Providers.Classic
if selected <> 1 then failwithf "Selected %%d" selected
"""
    |> withLangVersionPreview
    |> withOptions [condition]
    |> runFsi
    |> shouldSucceed

[<Theory>]
[<InlineData("--lib")>]
[<InlineData("--reference")>]
let ``Script directives do not supply ConditionalAutoOpen CLI flags`` optionName =
    let dependency =
        FSharp "module ScriptDependency\nlet value = 0"
        |> asLibrary
        |> withFSharpCoreShippedNet
        |> compile
        |> shouldSucceed
    let operand = if optionName = "--lib" then __SOURCE_DIRECTORY__ else dependency.OutputPath.Value
    let library =
        provider ("ScriptOptions" + optionName)
            $"""[<assembly: ConditionalAutoOpen("Providers.Modern", @"{optionName}:{operand}")>]"""
        |> compile
        |> shouldSucceed
    Fsx $"""
#I @"{__SOURCE_DIRECTORY__}"
#r @"{dependency.OutputPath.Value}"
#r @"{library.OutputPath.Value}"
[<EntryPoint>]
let main _ =
    if selected <> 1 then failwith "Script directives are not CLI flags"
    0
"""
    |> withLangVersionPreview
    |> withFSharpCoreShippedNet
    |> compileExeAndRun
    |> shouldSucceed

[<Fact>]
let ``ConditionalAutoOpen merges namespace fragments`` () =
    let fragment name child =
        FSharp $"""
namespace Exports.Shared
module {child} = let selected = 1
[<assembly: ConditionalAutoOpen("Exports", "--reflectionfree")>]
do ()
"""
        |> asLibrary
        |> withFSharpCoreShippedNet
        |> withName name
    consumer [fragment "NamespaceLeft" "Left"; fragment "NamespaceRight" "Right"] ["--reflectionfree"]
        "module Consumer\nlet value = Shared.Left.selected + Shared.Right.selected"
    |> asLibrary
    |> compile
    |> shouldSucceed

[<Fact>]
let ``ConditionalAutoOpen accepts overlapping imports of the same property`` () =
    let library =
        FSharp """
namespace Overlapping
module Root =
    [<AutoOpen>]
    module Shared =
        type System.String with member _.Marker = 42
[<assembly: ConditionalAutoOpen("Overlapping.Root", "--reflectionfree")>]
[<assembly: ConditionalAutoOpen("Overlapping.Root.Shared", "--reflectionfree")>]
do ()
"""
        |> asLibrary
        |> withFSharpCoreShippedNet
    consumer [library] ["--reflectionfree"]
        "module Consumer\n[<EntryPoint>]\nlet main _ = if \"\".Marker = 42 then 0 else failwith \"Property\""
    |> compileExeAndRun
    |> shouldSucceed

[<Theory>]
[<InlineData("--features:runtime-async=on", "--features:runtime-async=off --features:runtime-async=on", 2)>]
[<InlineData("--features:runtime-async=on", "--features:runtime-async=on --features:runtime-async=off", 2)>]
[<InlineData("--features:runtime-async=on", "--features:other=1,runtime-async=on", 1)>]
[<InlineData("--features:other=1,runtime-async=on", "--features:other=1,runtime-async=on", 2)>]
[<InlineData("--features:runtime-async=on", "--features:runtime-async=ON", 1)>]
[<InlineData("--features:Mode=on", "--features:mode=on", 1)>]
[<InlineData("--features:mode", "--features:mode", 2)>]
[<InlineData("--features:mode=true", "--features:mode", 1)>]
[<InlineData("--features:runtime-async=on", "--features:runtime-async", 1)>]
[<InlineData("--features:mode=", "--features:mode=", 2)>]
[<InlineData("--features:mode=quoted", "--features:mode=\"quoted\"", 1)>]
[<InlineData("--features:mode=one=two", "--features:mode=one=two", 2)>]
[<InlineData("--define:NET11_OR_GREATER", "-d:NET11_OR_GREATER", 1)>]
[<InlineData("-d:NET11_OR_GREATER", "-d:NET11_OR_GREATER", 2)>]
[<InlineData("/define:NET11_OR_GREATER", "--define:NET11_OR_GREATER", 1)>]
[<InlineData("-reflectionfree", "--reflectionfree", 1)>]
[<InlineData("-features:mode=on", "--features:mode=on", 1)>]
[<InlineData("--checknulls-", "", 1)>]
[<InlineData("--checknulls+", "--checknulls+ --checknulls-", 2)>]
[<InlineData("--checknulls-", "--checknulls+ --checknulls-", 2)>]
[<InlineData("--checked+", "--checked- --checked+", 2)>]
[<InlineData("--checked-", "--checked+ --checked-", 2)>]
[<InlineData("--warn:4", "--warn:3 --warn:4", 2)>]
[<InlineData("--checked-", "", 1)>]
[<InlineData("--langversion:preview", "", 2)>]
[<InlineData("--always-inline+", "--optimize+", 1)>]
[<InlineData("--always-inline-", "--optimize+", 1)>]
[<InlineData("--optimize+", "--optimize- --optimize+", 2)>]
[<InlineData("--optimize+", "-O", 1)>]
[<InlineData("--tailcalls-", "--tailcalls+ --tailcalls-", 2)>]
[<InlineData("--realsig-", "--nowarn:75 --realsig+ --realsig-", 2)>]
[<InlineData("--debug:portable", "--debug:portable", 2)>]
[<InlineData("--debug+", "-g", 1)>]
[<InlineData("--warnaserror:39", "--warnaserror:39", 2)>]
[<InlineData("--help", "--reflectionfree", 1)>]
[<InlineData("--warn:not-an-int", "--warn:4", 1)>]
[<InlineData("--nowarn:not-a-warning", "", 1)>]
[<InlineData("--warnaserror:not-a-warning", "", 1)>]
[<InlineData("--nullable", "--checknulls+", 1)>]
[<InlineData("--unknown", "--reflectionfree", 1)>]
[<InlineData("--reflectionfree", "--reflectionfree --langversion:11.2", 1)>]
let ``ConditionalAutoOpen matches complete CLI tokens`` (condition: string, flags: string, expected: int) =
    let library = provider "OptionProvider" $"""[<assembly: ConditionalAutoOpen("Providers.Modern", "{condition}")>]"""
    consumer [library] (flags.Split([|' '|], System.StringSplitOptions.RemoveEmptyEntries) |> List.ofArray) $"""
module Consumer
[<EntryPoint>]
let main _ =
    if selected <> {expected} then failwithf "Selected %%d" selected
    0
"""
    |> compileExeAndRun
    |> shouldSucceed

[<Theory>]
[<InlineData("Providers.Missing", 970)>]
[<InlineData(null, 3926)>]
let ``ConditionalAutoOpen reports invalid metadata without removing declarations`` (path: string, warningCode: int) =
    let path = if isNull path then "null" else $"\"{path}\""
    let library = provider "InvalidProvider" $"""[<assembly: ConditionalAutoOpen({path}, "--reflectionfree")>]"""
    let result =
        consumer [library] ["--reflectionfree"] "module Consumer\nlet value = Providers.Classic.selected"
        |> asLibrary
        |> compile
        |> shouldFail
        |> withWarningCode warningCode
    Assert.Single(result.Output.Diagnostics) |> ignore
