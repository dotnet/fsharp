// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open System.IO
open System.Reflection.Metadata
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module GuardedMatchSemantics =

    let private fixture name = Path.Combine(__SOURCE_DIRECTORY__, "GuardedMatchSemantics", name)

    let private template name arguments =
        (File.ReadAllText(fixture name), arguments)
        ||> List.fold (fun source (hole: string, value: string) -> source.Replace(hole, value))
        |> FSharp

    let private contextSourceWith last pattern attributes code action fallback =
        let body =
            [ "            match token, stack with"
              $"            | 0, C0 value :: _ when col < value -> {fallback}"
              for i in 0..last do
                  $"            | _, C{i} {pattern} :: _ when guard {i} value col -> {action (string (100 + i))}"
              for i in 1 .. last - 1 do
                  $"            | {i}, _ :: _ -> {action (string i)}"
              $"""            | ({last} | {last + 1}), _ :: _ when col >= 0 -> {action "token"}"""
              $"            | _ -> {fallback}" ]
            |> String.concat "\n"
        let contexts = [ for i in 0..last -> $"C{i} of int" ] |> String.concat " | "
        template "ContextMatch.fs" [ "{{MODULE_ATTRIBUTE}}", attributes; "{{CONTEXTS}}", contexts; "{{CASE}}", code; "{{MATCH}}", body ]

    let private contextSource attributes code action fallback =
        contextSourceWith 15 "value" attributes code action fallback

    let private runWithSettings optimize realSignature =
        withOptimization optimize
        >> withRealInternalSignature realSignature
        >> compileExeAndRun
        >> shouldSucceed

    let private sharedResultSource kind inlineKeyword main =
        let runtimeImports =
            "open System.Collections.Generic\nopen System.Threading.Tasks\nopen System.Runtime.CompilerServices\nopen Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers\n"
        let wrapAsync sigAndBody = $"{runtimeImports}let ready = Task.CompletedTask\nlet inline wrap {sigAndBody}"
        let declarations, resultType, wrap, prefix, success, tail, observe =
            match kind with
            | "int" -> "", "int", "", "", "calls", id, "result"
            | "argument" -> "", "int", "id", "", "calls", id, "result"
            | "function" ->
                "", "int -> int", "", "", "(fun n -> acc <- acc + calls; n + acc)", (fun v -> $"(fun n -> n + ({v}))"),
                "calls <- calls + 100\n    result 10 |> ignore\n    result 10"
            | "delegate" ->
                "type GetValue = delegate of unit -> int", "int", "", "", "GetValue(fun () -> calls)",
                (fun v -> $"GetValue(fun () -> {v})"), "result"
            | "runtimeAwait" ->
                wrapAsync "(f: int -> Task<int>) = __runtimeAsyncReturn (AsyncHelpers.Await (f 10))", "Task<int>", "wrap",
                "        | A, 99, 99 -> (fun _ -> AsyncHelpers.Await ready; Task.FromResult -99)",
                "(fun _ -> Task.FromResult calls)", (fun v -> $"(fun _ -> Task.FromResult({v}))"), "result.GetAwaiter().GetResult()"
            | "runtimeCallback" ->
                wrapAsync "([<InlineIfLambda>] f: int -> int) = __runtimeAsyncReturn (f 10)", "Task<int>", "wrap",
                "        | A, 99, 99 -> (fun _ -> AsyncHelpers.Await ready; -99)",
                "(fun _ -> calls)", (fun v -> $"(fun _ -> {v})"), "result.GetAwaiter().GetResult()"
            | "runtimeSequence" ->
                runtimeImports, "IAsyncEnumerable<int>", "__runtimeAsyncSequence", "",
                "(fun () -> seq { yield calls })", (fun v -> $"(fun () -> seq {{ yield {v} }})"), "calls"
            | "interface" ->
                "type I = abstract Value: int\ntype Left(n) = interface I with member _.Value = n\ntype Right(n) = interface I with member _.Value = n",
                "I", "", "", "(Left(calls) :> I)", (fun v -> $"(Right({v}) :> I)"), "result.Value"
            | "tuple" -> "", "int * string", "", "", """(calls, "yes")""", (fun v -> $"""({v}, "no")"""), "fst result"
            | _ -> failwith $"Unknown result {kind}"
        let tails =
            [ "A"; "B"; "C"; "D"; "E"; "F"; "G"; "H" ]
            |> List.mapi (fun i name -> $"        | {name}, _, _ -> {tail (string (-i - 1))}")
            |> String.concat "\n"
        let expected =
            if kind = "function" then "10 + (if accepted then 202 else -i - 1)"
            elif kind = "runtimeAwait" || kind = "runtimeCallback" then "if i = 0 && x = 99 && y = 99 then -99 elif accepted then 1 else -i - 1"
            else "if accepted then 1 else -i - 1"
        template "SharedResults.fs" [
            "{{DECLARATIONS}}", declarations; "{{INLINE}}", inlineKeyword; "{{LOCALS}}", (if kind = "function" then "let mutable acc = 0" else "")
            "{{TYPE}}", resultType; "{{SUCCESS}}", success; "{{PREFIX_CLAUSES}}", prefix; "{{TAILS}}", tails
            "{{OBSERVE}}", observe; "{{WRAP}}", wrap; "{{APPLY}}", (if kind = "delegate" then ".Invoke()" else "")
            "{{EXPECTED}}", expected; "{{EXTRA_CALLS}}", (if kind = "function" then "100" else "0"); "{{MAIN}}", main ]

    let private sequenceSource kind allocationCheck =
        let opening, closing =
            match kind with
            | "sequence" | "yieldFrom" -> "seq {", "}"
            | "array" -> "[|", "|]"
            | "list" -> "[", "]"
            | "toArray" -> "seq {", "} |> Seq.toArray"
            | "toList" -> "seq {", "} |> Seq.toList"
            | _ -> failwith $"Unknown collection {kind}"
        let prefix, empty, success =
            if kind = "yieldFrom" then "yield!", "Seq.empty", "Seq.singleton value"
            else "", "()", "yield value"
        template "SequenceResults.fs" [ "{{OPEN}}", opening; "{{CLOSE}}", closing; "{{MATCH_PREFIX}}", prefix; "{{EMPTY}}", empty; "{{SUCCESS}}", success; "{{ALLOCATION_CHECK}}", allocationCheck ]

    let private optimizeCases values =
        [ for value in values do
              for optimize in [ false; true ] -> [| box value; box optimize |] ]

    let private optionCases values =
        [ for value in values do
              for optimize in [ false; true ] do
                  for realSignature in [ false; true ] -> [| box value; box optimize; box realSignature |] ]

    let sharedResultCases = optionCases [ "int"; "argument"; "function"; "delegate"; "interface"; "tuple" ]
    let runtimeResultCases = optionCases [ "runtimeAwait"; "runtimeCallback"; "runtimeSequence" ]
    let recursiveBindingCases = optionCases [ false; true ]
    let observationCases = optionCases [ "Observations.fs"; "AliasMutation.fs"; "AppliedResults.fs" ]
    let sequenceCases = optionCases [ "sequence"; "array"; "list"; "toArray"; "toList"; "yieldFrom" ]
    let payloadOrderCases = optimizeCases [ false; true ]
    let tailDiagnosticCases = optimizeCases [ "int"; "unit"; "nonTailAction"; "nonTailMatch" ]
    let scopeCases =
        optimizeCases [
            "mutable"; "byref"; "rethrow"; "protected"; "tail"; "task"
#if NETCOREAPP
            "runtime"
#endif
        ]

    [<Theory; MemberData(nameof sequenceCases)>]
    let ``Issue 20632 - shared decisions retain collection continuations`` kind optimize realSignature =
        sequenceSource kind ""
        |> runWithSettings optimize realSignature

    [<TheoryForNETCOREAPP; InlineData("sequence"); InlineData("yieldFrom")>]
    let ``Issue 20632 - shared decisions do not allocate a sequence per yield`` kind =
        sequenceSource kind """
    let input = [|0..999|]
    let consume () =
        for _ in 1..100 do
            for _ in choose input do ()
    consume ()
    let before = System.GC.GetAllocatedBytesForCurrentThread()
    consume ()
    let allocated = System.GC.GetAllocatedBytesForCurrentThread() - before
    if allocated > 5_000_000L then failwithf "sequence allocation: %d bytes" allocated"""
        |> runWithSettings true false

    [<Theory; MemberData(nameof sharedResultCases)>]
    let ``Issue 20632 - shared tuple3 decisions preserve result continuations`` kind optimize realSignature =
        let result =
            sharedResultSource kind "" "[<EntryPoint>]\nlet main _ = check ()"
            |> runWithSettings optimize realSignature
        if (kind = "int" || kind = "argument") && optimize then
            result
            |> withPeReader (fun pe ->
                let md = pe.GetMetadataReader()
                md.MethodDefinitions
                |> Seq.sumBy (fun h ->
                    let m = md.GetMethodDefinition h
                    if m.RelativeVirtualAddress = 0 || md.GetString(md.GetTypeDefinition(m.GetDeclaringType()).Name) = "Context" then 0
                    else pe.GetMethodBody(m.RelativeVirtualAddress).GetILContent().Length))
            |> fun size -> Assert.True(size < 1500, $"Shared tuple3 match emitted {size} IL bytes")

    [<Theory; InlineData(false); InlineData(true)>]
    let ``Issue 20632 - shared guards retain caller sequence points`` optimize =
        FsFromPath(fixture "GuardPoints.fs")
        |> asLibrary
        |> withOptimization optimize
        |> withEmbeddedPdb
        |> compile
        |> shouldSucceed
        |> withPeReader (fun pe ->
            let entry =
                pe.ReadDebugDirectory()
                |> Seq.find (fun entry -> entry.Type = System.Reflection.PortableExecutable.DebugDirectoryEntryType.EmbeddedPortablePdb)
            use provider = pe.ReadEmbeddedPortablePdbDebugDirectoryData entry
            let md = pe.GetMetadataReader()
            let method = md.MethodDefinitions |> Seq.find (fun h -> md.GetString(md.GetMethodDefinition(h).Name) = "run")
            let points = provider.GetMetadataReader().GetMethodDebugInformation(method.ToDebugInformationHandle()).GetSequencePoints() |> Seq.toArray
            for line, startCol, endCol in [ 9, 33, 48; 10, 47, 61; 11, 47, 61 ] do
                Assert.Contains(points, fun p ->
                    not p.IsHidden && p.StartLine = line && p.EndLine = line && p.StartColumn = startCol && p.EndColumn = endCol))

    [<TheoryForNETCOREAPP; MemberData(nameof runtimeResultCases)>]
    let ``Issue 20632 - runtime construction keeps returned functions and diagnostics`` kind optimize realSignature =
        let compilation =
            sharedResultSource kind "" (if kind <> "runtimeSequence" then "[<EntryPoint>]\nlet main _ = check ()" else "")
            |> withLangVersionPreview
            |> withFSharpCoreShippedNet
            |> withOptimization optimize
            |> withRealInternalSignature realSignature
        if kind <> "runtimeSequence" then compilation |> compileExeAndRun |> shouldSucceed
        else compilation |> asLibrary |> compile |> shouldFail |> withErrorCode 3922

    [<Theory; MemberData(nameof recursiveBindingCases)>]
    let ``Issue 20632 - shared guards retain recursive binding fixups`` selfReference optimize realSignature =
        template "RecursiveBindings.fs" [
            "{{USE}}", (if selfReference then "(id (fun () -> first)) ()" else "later")
            "{{EXPECTED}}", (if selfReference then "-1" else "(if token = 0 then 1 else 2)") ]
        |> withLangVersionPreview
        |> runWithSettings optimize realSignature

    [<Fact>]
    let ``Issue 20632 - inline guard definitions remain portable`` () =
        let library = sharedResultSource "int" "inline " "" |> asLibrary |> withName "GuardedInline"
        FSharp """
module Consumer
[<EntryPoint>]
let main _ =
    if Test.run (Test.A, 0, 0) <> 1 || Test.run (Test.H, -8, 0) <> -8 then failwith "inline body"
    0"""
        |> withReferences [library]
        |> runWithSettings true true

    [<Theory; InlineData("guard"); InlineData("action")>]
    let ``Issue 20632 - recursive initialization remains strict`` site =
        (if site = "guard" then
            template "RecursiveBindings.fs" ["{{USE}}", "first"; "{{EXPECTED}}", "-1"]
         else
            FSharp """module Test
let run gate token flag =
    let rec value: int =
        match token, flag with
        | (0 | 1), true when gate -> 0
        | _ -> value + 1
    value""")
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withErrorCode 31

    [<Fact>]
    let ``Decision join placement is not serialized`` () =
        let ofBits = global.FSharp.Compiler.TypedTree.ValFlags.OfPickledBits
        let empty = ofBits 0L
        let join, param = empty.WithIsDecisionTreeJoin, empty.WithIsMatchJoinParameter
        Assert.True(join.IsDecisionTreeJoin && not join.IsPinning && not empty.WithIsPinning.IsDecisionTreeJoin)
        Assert.False((ofBits join.PickledBits).IsDecisionTreeJoin)
        Assert.True(param.IsMatchJoinParameter && not param.IsPinning && not param.IsDecisionTreeJoin)
        Assert.False((ofBits param.PickledBits).IsMatchJoinParameter)

    [<Theory; MemberData(nameof observationCases)>]
    let ``Issue 20632 - preserve committed observations and projection demand`` file optimize realSignature =
        template file [
            "{{ALTERNATIVES}}", ([1..64] |> List.map string |> String.concat " | ")
            "{{DEMAND_GUARDS}}", ([for i in 1..8 -> $"    | _, true when guard {i} -> {i}"] |> String.concat "\n")
            "{{NULL_DEFAULT_GUARDS}}", ([for i in 1..7 -> $"    | _, _ :: _ when not takeFirst -> {10 + i}"] |> String.concat "\n")
            "{{NULL_HEAD_GUARDS}}", ([for i in 1..7 -> $"    | _, Second _ :: _ when not takeFirst -> {10 + i}"] |> String.concat "\n")
            "{{REFUTED_GUARDS}}", ([for i in 2..7 -> $"    | _, _ :: _ when takeFirst -> {10 + i}"] |> String.concat "\n")
            "{{GUARDS}}", ([for i in 0..7 -> $"    | _, C0 {{ Value = 1 }} :: _ when guard {i} cell -> result <- {100 + i} + cell.Value; result"] |> String.concat "\n") ]
        |> runWithSettings optimize realSignature

    [<Theory; MemberData(nameof scopeCases)>]
    let ``Issue 20632 - grouped actions keep their original scope`` scope optimize =
        let code, checks, action, fallback =
            match scope with
            | "mutable" ->
                "let run token stack col =\n            let mutable result = -1\n{{MATCH}}",
                "for token in 1..16 do if run token [C0 0] 1000 <> token then failwith \"completed tail\"\n    if trace.Count <> 16 || run 5 [C15 0] 0 <> 115 || run 5 [] 1000 <> -1 then failwith \"mutable target\"",
                (fun v -> $"result <- {v}; result"), "result"
            | "byref" ->
                "let run (data: int[]) token stack col : byref<int> =\n{{MATCH}}",
                "let data = Array.zeroCreate<int> 128\n    (run data 5 [C15 0] 0) <- 99\n    (run data 5 [C0 0] 1000) <- 77\n    if data[115] <> 99 || data[5] <> 77 then failwith \"byref target\"",
                (fun v -> $"&data[({v}) %% data.Length]"), "&data[0]"
            | "rethrow" ->
                "let expected = System.Exception(\"identity\")\nlet run token stack col : int =\n    try raise expected with _ ->\n{{MATCH}}",
                "for stack in [ [C15 0]; [] ] do\n        try run 5 stack 0 |> ignore; failwith \"missing rethrow\"\n        with error when obj.ReferenceEquals(error, expected) -> ()",
                (fun _ -> "reraise()"), "reraise()"
            | "protected" ->
                "type Holder() =\n    member this.Run(token, stack, col) =\n{{MATCH}}",
                "let holder = Holder()\n    if holder.Run(5, [C15 0], 0) <> 115 || holder.Run(5, [C0 0], 1000) <> 5 then failwith \"protected target\"",
                (fun v -> $"this.MemberwiseClone() |> ignore; {v}"), "-1"
            | "tail" ->
                "let run token stack col =\n    let rec loop remaining result =\n        if remaining = 0 then result else\n{{MATCH}}\n    loop 100000 0",
                "if run 5 [C15 0] 0 <> 115 || run 5 [C0 0] 1000 <> 5 then failwith \"tail target\"",
                (fun v -> $"loop (remaining - 1) ({v})"), "loop (remaining - 1) -1"
            | "task" ->
                "let run token stack col =\n    task {\n        do! Task.Yield()\n        return (\n{{MATCH}}\n        )\n    }",
                "if (run 5 [C15 0] 0).Result <> 115 || (run 5 [] 0).Result <> -1 then failwith \"task target\"",
                id, "-1"
            | "runtime" ->
                "let run (ready: Task) token stack col : Task<int> =\n    Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers.__runtimeAsyncReturn (\n{{MATCH}}\n    )",
                "let ready = TaskCompletionSource<unit>()\n    let work = run ready.Task 5 [C15 0] 0\n    if work.IsCompleted then failwith \"missing suspension\"\n    ready.SetResult(())\n    if work.GetAwaiter().GetResult() <> 115 || (run Task.CompletedTask 5 [] 0).GetAwaiter().GetResult() <> -1 then failwith \"runtime async target\"",
                (fun v -> $"System.Runtime.CompilerServices.AsyncHelpers.Await ready; {v}"), "-1"
            | _ -> failwith $"Unknown scope {scope}"
        contextSource "" $"{code}\n[<EntryPoint>]\nlet main _ =\n    {checks}\n    0" action fallback
        |> (if scope = "runtime" then withLangVersionPreview >> withFSharpCoreShippedNet else id)
        |> withOptimization optimize
        |> (if scope = "tail" then withOptions ["--tailcalls-"] else id)
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; MemberData(nameof payloadOrderCases)>]
    let ``Issue 20632 - payload observations precede tail tests and throwing guards`` partialPattern optimize =
        template "PayloadOrder.fs" [
            "{{GUARDS}}", ([for i in 0..7 -> $"            | _, C0 {{{{PAYLOAD}}}} :: (_ :: _) when guard {i} values -> {100 + i} + values[0]"] |> String.concat "\n")
            "{{PAYLOAD}}", (if partialPattern then "(Read 0 (Observe values))" else "(Observe values)")
            "{{PARTIAL}}", (if partialPattern then "true" else "false") ]
        |> withOptimization optimize
        |> withOptions ["--nowarn:75"; "--extraoptimizationloops:1"; "--parallelcompilation+"]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; InlineData("explicit quote"); InlineData("reflected argument"); InlineData("reflected function"); InlineData("reflected type"); InlineData("reflected module")>]
    [<InlineData("LINQ"); InlineData("LINQ overload"); InlineData("auto quote"); InlineData("reflected top module"); InlineData("reflected constructor")>]
    let ``Issue 20632 - preserve quotation shape`` origin =
        let fn = "let run token stack col =\n{{MATCH}}"
        let lam = "fun token stack col ->\n{{MATCH}}"
        let reflected m = $"let q = Expr.TryGetReflectedDefinition({m}).Value"
        let runMethod = reflected """typeof<Context>.DeclaringType.GetMethod("run")"""
        let linqTy = "System.Linq.Expressions.Expression<System.Func<int, Context list, int, int>>"
        let code =
            match origin with
            | "explicit quote" -> $"let q = <@ {lam} @> :> Expr"
            | "reflected argument" -> $"type Sink = static member Capture([<ReflectedDefinition>] v: Expr<int -> Context list -> int -> int>) = v\nlet q = Sink.Capture({lam}) :> Expr"
            | "reflected function" -> $"[<ReflectedDefinition>]\n{fn}\n{runMethod}"
            | "reflected type" -> "[<ReflectedDefinition>]\ntype Holder =\n    static member Run(token, stack, col) =\n{{MATCH}}\n" + reflected """typeof<Holder>.GetMethod("Run")"""
            | "reflected module" -> $"[<ReflectedDefinition>]\nmodule Holder =\n    {fn}\n" + reflected """typeof<Context>.DeclaringType.GetNestedType("Holder").GetMethod("run")"""
            | "LINQ" -> $"type Sink = static member Capture(v: {linqTy}) = v\nlet q = Sink.Capture({lam})"
            | "LINQ overload" -> $"type Sink =\n    static member Capture(v: {linqTy}) = v\n    static member Capture(v: string) = v\nlet q = Sink.Capture({lam})"
            | "auto quote" -> $"type Builder() =\n    member _.Return(v) = v\n    member _.Delay(f) = f()\n    member _.Quote(v: Expr<'T>) = v\n    member _.Run(v: Expr<'T>) = v\nlet q = (Builder() {{ return ({lam}) }}) :> Expr"
            | "reflected top module" -> $"{fn}\n{runMethod}"
            | "reflected constructor" -> "type Holder [<ReflectedDefinition>](token: int, stack: Context list, col: int) =\n    let result =\n{{MATCH}}\n    member _.Result = result\n" + reflected "typeof<Holder>.GetConstructors()[0]"
            | _ -> failwith $"Unknown quotation origin {origin}"
        contextSource (if origin = "reflected top module" then "[<ReflectedDefinition>]" else "") code id "-1"
        |> withAdditionalSourceFile (SourceFromPath (fixture "VerifyQuotation.fs"))
        |> runWithSettings true true
        |> withStdOutContains "calls=288"

    [<Fact>]
    let ``Promoted joins do not replace the incomplete match target`` () =
        let clauses = [for i in 0..40 -> $"    | Some value, {i} when value < 0 -> {i}"] |> String.concat "\n"
        FSharp $"""module Test
let run input =
    match input with
    | None, (100 | 101) when false -> -1
{clauses}
    | Some value, _ when value = 0 -> value
    | Some _, _ -> 99
[<EntryPoint>]
let main _ =
    if run (Some 0, 200) <> 0 || run (Some 42, 0) <> 99 then failwith "join result"
    try run (None, 200) |> ignore; failwith "missing match failure"
    with :? MatchFailureException -> ()
    0"""
        |> withNoWarn 25
        |> runWithSettings true true

    [<Theory; MemberData(nameof tailDiagnosticCases)>]
    let ``Issue 20632 - promoted joins preserve attributed tail diagnostics`` kind optimize =
        let finish, fallback = if kind = "unit" then "()", "()" else "0", "-1"
        let action = (if kind = "nonTailAction" then "1 + " else "") + "run (remaining - 1) token stack col"
        let prefix = if kind = "nonTailMatch" then "1 + " else ""
        let code = $"""[<TailCall>]
let rec run remaining token stack col =
    if remaining = 0 then {finish} else
        {prefix}(
            match token, stack with
            | (0 | 1 | 2 | 3 | 4 | 5 | 6 | 7), C0 value :: _ when guard 0 value col -> {action}
            | _ -> {fallback})"""
        let compilation =
            contextSource "" code id fallback
            |> asLibrary
            |> withLangVersion80
            |> withOptimization optimize
            |> withRealInternalSignature true
            |> compile
        if kind = "nonTailAction" || kind = "nonTailMatch" then
            compilation |> shouldFail |> withWarningCode 3569 |> ignore
        else
            compilation |> shouldSucceed |> ignore

#if NETCOREAPP
    [<Collection(nameof NotThreadSafeResourceCollection)>]
    type CompilerAllocation() =
        [<Fact>]
        member _.``Issue 20632 - guarded total active patterns retain bounded compilation allocation`` () =
            let source =
                contextSourceWith 31 "(Identity value)" "" "let (|Identity|) value = value\nlet run token stack col =\n{{MATCH}}" id "-1"
                |> asLibrary
                |> withLangVersionPreview
                |> withOptimize
                |> withRealInternalSignature false
                |> withOptions ["--nowarn:75"; "--extraoptimizationloops:0"; "--parallelcompilation-"]
            let compileSource () = source |> compile |> shouldSucceed |> ignore
            compileSource ()
            let before = System.GC.GetTotalAllocatedBytes true
            compileSource ()
            let allocated = System.GC.GetTotalAllocatedBytes true - before
            Assert.True(allocated < 1_000_000_000L, $"Compiler allocated {allocated} bytes")
#endif
