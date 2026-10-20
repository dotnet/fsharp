// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open System.IO
open System.Reflection.Metadata
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module GuardedMatchSemantics =

    let private fixture name = Path.Combine(__SOURCE_DIRECTORY__, "GuardedMatchSemantics", name)

    let private emittedILExcept excludedType =
        withPeReader (fun pe ->
            let metadata = pe.GetMetadataReader()
            let rec included (handle: TypeDefinitionHandle) =
                if handle.IsNil then true else
                let definition = metadata.GetTypeDefinition handle
                metadata.GetString definition.Name <> excludedType && included (definition.GetDeclaringType())
            metadata.MethodDefinitions
            |> Seq.sumBy (fun handle ->
                let method = metadata.GetMethodDefinition handle
                if method.RelativeVirtualAddress = 0 || not (included (method.GetDeclaringType())) then 0
                else pe.GetMethodBody(method.RelativeVirtualAddress).GetILContent().Length))

    let private contextSource define action fallback =
        let body =
            [ "            match token, stack with"
              $"            | 0, C0 value :: _ when col < value -> {fallback}"
              for i in 0..15 do
                  $"            | _, C{i} value :: _ when guard {i} value col -> {action (string (100 + i))}"
              for i in 1..14 do
                  $"            | {i}, _ :: _ -> {action (string i)}"
              $"""            | (15 | 16), _ :: _ when col >= 0 -> {action "token"}"""
              $"            | _ -> {fallback}" ]
            |> String.concat "\n"
        File.ReadAllText(fixture "ContextMatch.fs").Replace("            // MATCH", body)
        |> FSharp
        |> withDefines [define]

    [<Theory;
      FileInlineData("GuardedMatchSemantics/Observations.fs", Realsig = BooleanOptions.Both, Optimize = BooleanOptions.Both);
      FileInlineData("GuardedMatchSemantics/AliasMutation.fs", Realsig = BooleanOptions.Both, Optimize = BooleanOptions.Both)>]
    let ``Issue 20632 - preserve committed observations and projection demand`` compilation =
        compilation |> getCompilation |> withName "GuardedMatchObservations" |> compileExeAndRun |> shouldSucceed

    [<Fact>]
    let ``Issue 20632 - context guards share code across distinct completed tails`` () =
        contextSource "IL_BOUND" (fun value -> $"result <- {value}; result") "result"
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed
        |> emittedILExcept "Context"
        |> fun size -> Assert.True(size < 6000, $"Context match emitted {size} IL bytes")

    [<Theory>]
    [<InlineData("BYREF", false)>]
    [<InlineData("BYREF", true)>]
    [<InlineData("RETHROW", false)>]
    [<InlineData("RETHROW", true)>]
    [<InlineData("PROTECTED", false)>]
    [<InlineData("PROTECTED", true)>]
    [<InlineData("TAIL", false)>]
    [<InlineData("TAIL", true)>]
    [<InlineData("TASK", false)>]
    [<InlineData("TASK", true)>]
#if NETCOREAPP
    [<InlineData("RUNTIME", false)>]
    [<InlineData("RUNTIME", true)>]
#endif
    let ``Issue 20632 - grouped actions keep their original scope`` scope optimize =
        let action, fallback =
            match scope with
            | "BYREF" -> (fun value -> $"&data[({value}) %% data.Length]"), "&data[0]"
            | "RETHROW" -> (fun _ -> "reraise()"), "reraise()"
            | "PROTECTED" -> (fun value -> $"this.MemberwiseClone() |> ignore; {value}"), "-1"
            | "TAIL" -> (fun value -> $"loop (remaining - 1) ({value})"), "loop (remaining - 1) -1"
            | "TASK" -> id, "-1"
            | "RUNTIME" -> (fun value -> $"System.Runtime.CompilerServices.AsyncHelpers.Await ready; {value}"), "-1"
            | _ -> failwith $"Unknown scope {scope}"
        contextSource scope action fallback
        |> (if scope = "RUNTIME" then withLangVersionPreview >> withFSharpCoreShippedNet else id)
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory>]
    [<InlineData(false, false)>]
    [<InlineData(false, true)>]
    [<InlineData(true, false)>]
    [<InlineData(true, true)>]
    let ``Issue 20632 - payload observations precede tail tests and throwing guards`` partialPattern optimize =
        let source = File.ReadAllText(fixture "PayloadOrder.fs")
        let payload = if partialPattern then "(Read 0 (Observe values))" else "(Observe values)"
        FSharp (source.Replace("__PAYLOAD__", payload))
        |> withDefines [if partialPattern then "PARTIAL"]
        |> withOptimization optimize
        |> withOptions ["--nowarn:75"; "--extraoptimizationloops:1"; "--parallelcompilation+"]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory>]
    [<InlineData("EXPLICIT_QUOTE")>]
    [<InlineData("REFLECTED_ARGUMENT")>]
    [<InlineData("REFLECTED_FUNCTION")>]
    [<InlineData("REFLECTED_TYPE")>]
    [<InlineData("REFLECTED_MODULE")>]
    [<InlineData("LINQ")>]
    [<InlineData("LINQ_OVERLOAD")>]
    [<InlineData("AUTO_QUOTE")>]
    [<InlineData("REFLECTED_TOP_MODULE")>]
    [<InlineData("REFLECTED_CONSTRUCTOR")>]
    let ``Issue 20632 - preserve quotation shape`` origin =
        contextSource origin id "-1"
        |> withAdditionalSourceFile (SourceFromPath (fixture "VerifyQuotation.fs"))
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "calls=288"

    [<Fact>]
    let ``Promoted joins do not replace the incomplete match target`` () =
        let clauses = [for i in 0..40 -> $"    | Some value, {i} when value < 0 -> {i}"] |> String.concat "\n"
        FSharp (File.ReadAllText(fixture "IncompleteTarget.fs").Replace("    // CLAUSES", clauses))
        |> withOptions ["--nowarn:25"]
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed
