// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open System.IO
open System.Reflection.Metadata
open Xunit
open FSharp.Test.Compiler

module GuardedMatchSemantics =

    let private fixture name = Path.Combine(__SOURCE_DIRECTORY__, "GuardedMatchSemantics", name)

    let private template name arguments =
        (File.ReadAllText(fixture name), arguments)
        ||> List.fold (fun source (hole: string, value: string) -> source.Replace(hole, value))
        |> FSharp

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

    let private contextSource attributes code action fallback =
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
        template "ContextMatch.fs" ["{{MODULE_ATTRIBUTE}}", attributes; "{{CASE}}", code; "{{MATCH}}", body]

    let private executableContext code action fallback checks =
        contextSource "" (code + "\n[<EntryPoint>]\nlet main _ =\n" + checks + "\n    0") action fallback

    [<Theory>]
    [<InlineData("Observations.fs", false, false)>]
    [<InlineData("Observations.fs", false, true)>]
    [<InlineData("Observations.fs", true, false)>]
    [<InlineData("Observations.fs", true, true)>]
    [<InlineData("AliasMutation.fs", false, false)>]
    [<InlineData("AliasMutation.fs", false, true)>]
    [<InlineData("AliasMutation.fs", true, false)>]
    [<InlineData("AliasMutation.fs", true, true)>]
    let ``Issue 20632 - preserve committed observations and projection demand`` file optimize realSignature =
        template file [
            "{{ALTERNATIVES}}", ([1..64] |> List.map string |> String.concat " | ")
            "{{DEMAND_GUARDS}}", ([for i in 1..8 -> $"    | _, true when guard {i} -> {i}"] |> String.concat "\n")
            "{{NULL_DEFAULT_GUARDS}}", ([for i in 1..7 -> $"    | _, _ :: _ when not takeFirst -> {10 + i}"] |> String.concat "\n")
            "{{NULL_HEAD_GUARDS}}", ([for i in 1..7 -> $"    | _, Second _ :: _ when not takeFirst -> {10 + i}"] |> String.concat "\n")
            "{{REFUTED_GUARDS}}", ([for i in 2..7 -> $"    | _, _ :: _ when takeFirst -> {10 + i}"] |> String.concat "\n")
            "{{GUARDS}}", ([for i in 0..7 -> $"    | _, C0 {{ Value = 1 }} :: _ when guard {i} cell -> result <- {100 + i} + cell.Value; result"] |> String.concat "\n") ]
        |> withOptimization optimize
        |> withRealInternalSignature realSignature
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Issue 20632 - context guards share code across distinct completed tails`` () =
        executableContext "let run token stack col =\n            let mutable result = -1\n{{MATCH}}"
            (fun value -> $"result <- {value}; result") "result" """
    for token in 1..16 do
        if run token [C0 0] 1000 <> token then failwith "completed tail"
    if trace.Count <> 16 then failwith ("guard count: " + string trace.Count)
    if run 5 [C15 0] 0 <> 115 then failwith "guard success"
    if run 5 [] 1000 <> -1 then failwith "empty list" """
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed
        |> emittedILExcept "Context"
        |> fun size -> Assert.True(size < 6000, $"Context match emitted {size} IL bytes")

    [<Theory>]
    [<InlineData("byref", false)>]
    [<InlineData("byref", true)>]
    [<InlineData("rethrow", false)>]
    [<InlineData("rethrow", true)>]
    [<InlineData("protected", false)>]
    [<InlineData("protected", true)>]
    [<InlineData("tail", false)>]
    [<InlineData("tail", true)>]
    [<InlineData("task", false)>]
    [<InlineData("task", true)>]
#if NETCOREAPP
    [<InlineData("runtime", false)>]
    [<InlineData("runtime", true)>]
#endif
    let ``Issue 20632 - grouped actions keep their original scope`` scope optimize =
        let code, checks, action, fallback =
            match scope with
            | "byref" ->
                "let run (data: int[]) token stack col : byref<int> =\n{{MATCH}}", """
    let data = Array.zeroCreate<int> 128
    (run data 5 [C15 0] 0) <- 99
    (run data 5 [C0 0] 1000) <- 77
    if data[115] <> 99 || data[5] <> 77 then failwith "byref target" """,
                (fun value -> $"&data[({value}) %% data.Length]"), "&data[0]"
            | "rethrow" ->
                "let expected = System.Exception(\"identity\")\nlet run token stack col : int =\n    try raise expected with _ ->\n{{MATCH}}", """
    for stack in [ [C15 0]; [] ] do
        try
            run 5 stack 0 |> ignore
            failwith "missing rethrow"
        with error when obj.ReferenceEquals(error, expected) -> () """, (fun _ -> "reraise()"), "reraise()"
            | "protected" ->
                "type Holder() =\n    member this.Run(token, stack, col) =\n{{MATCH}}", """
    let holder = Holder()
    if holder.Run(5, [C15 0], 0) <> 115 || holder.Run(5, [C0 0], 1000) <> 5 then failwith "protected target" """,
                (fun value -> $"this.MemberwiseClone() |> ignore; {value}"), "-1"
            | "tail" ->
                "let run token stack col =\n    let rec loop remaining result =\n        if remaining = 0 then result else\n{{MATCH}}\n    loop 100000 0",
                """    if run 5 [C15 0] 0 <> 115 || run 5 [C0 0] 1000 <> 5 then failwith "tail target" """,
                (fun value -> $"loop (remaining - 1) ({value})"), "loop (remaining - 1) -1"
            | "task" ->
                "let run token stack col =\n    task {\n        do! Task.Yield()\n        return (\n{{MATCH}}\n        )\n    }",
                """    if (run 5 [C15 0] 0).Result <> 115 || (run 5 [] 0).Result <> -1 then failwith "task target" """, id, "-1"
            | "runtime" ->
                "let run (ready: Task) token stack col : Task<int> =\n    Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers.__runtimeAsyncReturn (\n{{MATCH}}\n    )", """
    let ready = TaskCompletionSource<unit>()
    let work = run ready.Task 5 [C15 0] 0
    if work.IsCompleted then failwith "missing suspension"
    ready.SetResult(())
    if work.GetAwaiter().GetResult() <> 115 || (run Task.CompletedTask 5 [] 0).GetAwaiter().GetResult() <> -1 then failwith "runtime async target" """,
                (fun value -> $"System.Runtime.CompilerServices.AsyncHelpers.Await ready; {value}"), "-1"
            | _ -> failwith $"Unknown scope {scope}"
        executableContext code action fallback checks
        |> (if scope = "runtime" then withLangVersionPreview >> withFSharpCoreShippedNet else id)
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory>]
    [<InlineData(false, false)>]
    [<InlineData(false, true)>]
    [<InlineData(true, false)>]
    [<InlineData(true, true)>]
    let ``Issue 20632 - payload observations precede tail tests and throwing guards`` partialPattern optimize =
        template "PayloadOrder.fs" [
            "{{GUARDS}}", ([for i in 0..7 -> $"            | _, C0 {{{{PAYLOAD}}}} :: (_ :: _) when guard {i} values -> {100 + i} + values[0]"] |> String.concat "\n")
            "{{PAYLOAD}}", (if partialPattern then "(Read 0 (Observe values))" else "(Observe values)")
            "{{PARTIAL}}", (if partialPattern then "true" else "false") ]
        |> withOptimization optimize
        |> withOptions ["--nowarn:75"; "--extraoptimizationloops:1"; "--parallelcompilation+"]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory>]
    [<InlineData("explicit quote")>]
    [<InlineData("reflected argument")>]
    [<InlineData("reflected function")>]
    [<InlineData("reflected type")>]
    [<InlineData("reflected module")>]
    [<InlineData("LINQ")>]
    [<InlineData("LINQ overload")>]
    [<InlineData("auto quote")>]
    [<InlineData("reflected top module")>]
    [<InlineData("reflected constructor")>]
    let ``Issue 20632 - preserve quotation shape`` origin =
        let functionCode = "let run token stack col =\n{{MATCH}}"
        let lambda = "fun token stack col ->\n{{MATCH}}"
        let reflectedFunction = "[<ReflectedDefinition>]\n" + functionCode
        let reflectedLookup = "let q = Expr.TryGetReflectedDefinition(typeof<Context>.DeclaringType.GetMethod(\"run\")).Value"
        let expressionTree = "System.Linq.Expressions.Expression<System.Func<int, Context list, int, int>>"
        let code =
            match origin with
            | "explicit quote" -> $"let q = <@ {lambda} @> :> Expr"
            | "reflected argument" -> $"type Sink = static member Capture([<ReflectedDefinition>] value: Expr<int -> Context list -> int -> int>) = value\nlet q = Sink.Capture({lambda}) :> Expr"
            | "reflected function" -> reflectedFunction + "\n" + reflectedLookup
            | "reflected type" -> "[<ReflectedDefinition>]\ntype Holder =\n    static member Run(token, stack, col) =\n{{MATCH}}\nlet q = Expr.TryGetReflectedDefinition(typeof<Holder>.GetMethod(\"Run\")).Value"
            | "reflected module" -> "[<ReflectedDefinition>]\nmodule Holder =\n    let run token stack col =\n{{MATCH}}\nlet q = Expr.TryGetReflectedDefinition(typeof<Context>.DeclaringType.GetNestedType(\"Holder\").GetMethod(\"run\")).Value"
            | "LINQ" -> $"type Sink = static member Capture(value: {expressionTree}) = value\nlet q = Sink.Capture({lambda})"
            | "LINQ overload" -> $"type Sink =\n    static member Capture(value: {expressionTree}) = value\n    static member Capture(value: string) = value\nlet q = Sink.Capture({lambda})"
            | "auto quote" -> $"type Builder() =\n    member _.Return(value) = value\n    member _.Delay(f) = f()\n    member _.Quote(value: Expr<'T>) = value\n    member _.Run(value: Expr<'T>) = value\nlet q = (Builder() {{ return ({lambda}) }}) :> Expr"
            | "reflected top module" -> functionCode + "\n" + reflectedLookup
            | "reflected constructor" -> "type Holder [<ReflectedDefinition>](token: int, stack: Context list, col: int) =\n    let result =\n{{MATCH}}\n    member _.Result = result\nlet q = Expr.TryGetReflectedDefinition(typeof<Holder>.GetConstructors()[0]).Value"
            | _ -> failwith $"Unknown quotation origin {origin}"
        let attributes = if origin = "reflected top module" then "[<ReflectedDefinition>]" else ""
        contextSource attributes code id "-1"
        |> withAdditionalSourceFile (SourceFromPath (fixture "VerifyQuotation.fs"))
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "calls=288"

    [<Fact>]
    let ``Promoted joins do not replace the incomplete match target`` () =
        let clauses = [for i in 0..40 -> $"    | Some value, {i} when value < 0 -> {i}"] |> String.concat "\n"
        template "IncompleteTarget.fs" ["{{CLAUSES}}", clauses]
        |> withOptions ["--nowarn:25"]
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed
