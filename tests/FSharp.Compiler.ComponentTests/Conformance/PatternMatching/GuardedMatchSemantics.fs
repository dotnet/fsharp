// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open System
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

    let private runWithSettings optimize realSignature =
        withOptimization optimize
        >> withRealInternalSignature realSignature
        >> compileExeAndRun
        >> shouldSucceed

    let private sharedResultSource kind inlineKeyword main =
        let runtimeImports =
            "open System.Collections.Generic\nopen System.Threading.Tasks\nopen System.Runtime.CompilerServices\nopen Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers\n"
        let declarations, resultType, success, tail, observe =
            match kind with
            | "int" | "argument" -> "", "int", "calls", id, "result"
            | "function" ->
                "", "int -> int", "(fun n -> acc <- acc + calls; n + acc)", (fun value -> $"(fun n -> n + ({value}))"),
                "calls <- calls + 100\n    result 10 |> ignore\n    result 10"
            | "delegate" ->
                "type GetValue = delegate of unit -> int", "int", "GetValue(fun () -> calls)",
                (fun value -> $"GetValue(fun () -> {value})"), "result"
            | "runtimeAwait" ->
                runtimeImports + "let ready = Task.CompletedTask\nlet inline wrap (f: int -> Task<int>) = __runtimeAsyncReturn (AsyncHelpers.Await (f 10))",
                "Task<int>", "(fun _ -> Task.FromResult calls)", (fun value -> $"(fun _ -> Task.FromResult({value}))"), "result.GetAwaiter().GetResult()"
            | "runtimeCallback" ->
                runtimeImports + "let ready = Task.CompletedTask\nlet inline wrap ([<InlineIfLambda>] f: int -> int) = __runtimeAsyncReturn (f 10)",
                "Task<int>", "(fun _ -> calls)", (fun value -> $"(fun _ -> {value})"), "result.GetAwaiter().GetResult()"
            | "runtimeSequence" ->
                runtimeImports,
                "IAsyncEnumerable<int>", "(fun () -> seq { yield calls })",
                (fun value -> $"(fun () -> seq {{ yield {value} }})"), "calls"
            | "interface" ->
                "type I = abstract Value: int\ntype Left(n) = interface I with member _.Value = n\ntype Right(n) = interface I with member _.Value = n",
                "I", "(Left(calls) :> I)", (fun value -> $"(Right({value}) :> I)"), "result.Value"
            | "tuple" -> "", "int * string", """(calls, "yes")""", (fun value -> $"""({value}, "no")"""), "fst result"
            | _ -> failwith $"Unknown result {kind}"
        let tails =
            ["A"; "B"; "C"; "D"; "E"; "F"; "G"; "H"]
            |> List.mapi (fun i name -> $"        | {name}, _, _ -> {tail (string (-i - 1))}")
            |> String.concat "\n"
        template "SharedResults.fs" [
            "{{DECLARATIONS}}", declarations
            "{{INLINE}}", inlineKeyword
            "{{LOCALS}}", (if kind = "function" then "let mutable acc = 0" else "")
            "{{TYPE}}", resultType
            "{{SUCCESS}}", success
            "{{PREFIX_CLAUSES}}", (match kind with
                                  | "runtimeAwait" -> "        | A, 99, 99 -> (fun _ -> AsyncHelpers.Await ready; Task.FromResult -99)"
                                  | "runtimeCallback" -> "        | A, 99, 99 -> (fun _ -> AsyncHelpers.Await ready; -99)"
                                  | _ -> "")
            "{{TAILS}}", tails
            "{{OBSERVE}}", observe
            "{{WRAP}}", (match kind with "argument" -> "id" | "runtimeAwait" | "runtimeCallback" -> "wrap" | "runtimeSequence" -> "__runtimeAsyncSequence" | _ -> "")
            "{{APPLY}}", (if kind = "delegate" then ".Invoke()" else "")
            "{{EXPECTED}}", (match kind with
                            | "function" -> "10 + (if accepted then 202 else -i - 1)"
                            | "runtimeAwait" | "runtimeCallback" -> "if i = 0 && x = 99 && y = 99 then -99 elif accepted then 1 else -i - 1"
                            | _ -> "if accepted then 1 else -i - 1")
            "{{EXTRA_CALLS}}", (if kind = "function" then "100" else "0")
            "{{MAIN}}", main ]

    let private sequenceSource kind allocationCheck =
        let opening, closing =
            match kind with
            | "sequence" | "yieldFrom" -> "seq {", "}"
            | "array" -> "[|", "|]"
            | "list" -> "[", "]"
            | "toArray" -> "seq {", "} |> Seq.toArray"
            | "toList" -> "seq {", "} |> Seq.toList"
            | _ -> failwith $"Unknown collection {kind}"
        let prefix, caseIndent, empty, success =
            if kind = "yieldFrom" then "yield! ", "        ", "Seq.empty", "Seq.singleton value"
            else "", "", "()", "yield value"
        template "SequenceResults.fs" [
            "{{OPEN}}", opening; "{{CLOSE}}", closing
            "{{MATCH_PREFIX}}", prefix; "{{CASE_INDENT}}", caseIndent; "{{EMPTY}}", empty; "{{SUCCESS}}", success
            "{{ALLOCATION_CHECK}}", allocationCheck ]

    let private optionCases values =
        seq {
            for value in values do
                for optimize in [false; true] do
                    for realSignature in [false; true] do
                        yield [| value; box optimize; box realSignature |]
        }

    let sharedResultCases = optionCases [for kind in ["int"; "argument"; "function"; "delegate"; "interface"; "tuple"] -> box kind]
    let runtimeResultCases = optionCases [box "runtimeAwait"; box "runtimeCallback"; box "runtimeSequence"]
    let recursiveBindingCases = optionCases [box false; box true]
    let observationCases = optionCases [box "Observations.fs"; box "AliasMutation.fs"; box "AppliedResults.fs"]
    let sequenceCases = optionCases [box "sequence"; box "array"; box "list"; box "toArray"; box "toList"; box "yieldFrom"]

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
        sharedResultSource kind "" "[<EntryPoint>]\nlet main _ = check ()"
        |> runWithSettings optimize realSignature
        |> (if (kind = "int" || kind = "argument") && optimize then
                emittedILExcept "Context" >> fun size -> Assert.True(size < 1500, $"Shared tuple3 match emitted {size} IL bytes")
            else ignore)

    [<Theory; InlineData(false); InlineData(true)>]
    let ``Issue 20632 - shared guards retain caller sequence points`` optimize =
        template "GuardPoints.fs" []
        |> asLibrary
        |> withOptimization optimize
        |> withOptions ["--debug:embedded"]
        |> compile
        |> shouldSucceed
        |> withPeReader (fun pe ->
            let entry =
                pe.ReadDebugDirectory()
                |> Seq.find (fun entry -> entry.Type = System.Reflection.PortableExecutable.DebugDirectoryEntryType.EmbeddedPortablePdb)
            use provider = pe.ReadEmbeddedPortablePdbDebugDirectoryData entry
            let metadata = pe.GetMetadataReader()
            let method =
                metadata.MethodDefinitions
                |> Seq.find (fun handle -> metadata.GetString(metadata.GetMethodDefinition(handle).Name) = "run")
            let points = provider.GetMetadataReader().GetMethodDebugInformation(method.ToDebugInformationHandle()).GetSequencePoints() |> Seq.toArray
            File.ReadAllLines(fixture "GuardPoints.fs")
            |> Array.iteri (fun index line ->
                if line.IndexOf(" when guard ", StringComparison.Ordinal) >= 0 then
                    let startColumn = line.IndexOf("guard ", StringComparison.Ordinal) + 1
                    let endColumn = line.IndexOf(" -> ", StringComparison.Ordinal) + 1
                    Assert.Contains(points, fun point ->
                        not point.IsHidden && point.StartLine = index + 1 && point.EndLine = index + 1
                        && point.StartColumn = startColumn && point.EndColumn = endColumn)))

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

    [<Theory>]
    [<InlineData("guard")>]
    [<InlineData("action")>]
    let ``Issue 20632 - recursive initialization remains strict`` site =
        (if site = "guard" then
            template "RecursiveBindings.fs" ["{{USE}}", "first"; "{{EXPECTED}}", "-1"]
         else template "StrictInitialization.fs" [])
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withErrorCode 31

    [<Fact>]
    let ``Decision join placement is not serialized`` () =
        let flags = global.FSharp.Compiler.TypedTree.ValFlags.OfPickledBits(0L).WithIsDecisionTreeJoin
        Assert.True(flags.IsDecisionTreeJoin)
        Assert.False(flags.IsPinning)
        Assert.False(global.FSharp.Compiler.TypedTree.ValFlags.OfPickledBits(0L).WithIsPinning.IsDecisionTreeJoin)
        Assert.False(global.FSharp.Compiler.TypedTree.ValFlags.OfPickledBits(flags.PickledBits).IsDecisionTreeJoin)
        let parameter = global.FSharp.Compiler.TypedTree.ValFlags.OfPickledBits(0L).WithIsMatchJoinParameter
        Assert.False(parameter.IsPinning)
        Assert.False(parameter.IsDecisionTreeJoin)
        Assert.False(global.FSharp.Compiler.TypedTree.ValFlags.OfPickledBits(parameter.PickledBits).IsMatchJoinParameter)

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

    [<Theory>]
    [<InlineData("mutable", true)>]
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
            | "mutable" ->
                "let run token stack col =\n            let mutable result = -1\n{{MATCH}}", """
    for token in 1..16 do
        if run token [C0 0] 1000 <> token then failwith "completed tail"
    if trace.Count <> 16 then failwith ("guard count: " + string trace.Count)
    if run 5 [C15 0] 0 <> 115 then failwith "guard success"
    if run 5 [] 1000 <> -1 then failwith "empty list" """,
                (fun value -> $"result <- {value}; result"), "result"
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
        |> (if scope = "tail" then withOptions ["--tailcalls-"] else id)
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
        |> runWithSettings true true
        |> withStdOutContains "calls=288"

    [<Fact>]
    let ``Promoted joins do not replace the incomplete match target`` () =
        let clauses = [for i in 0..40 -> $"    | Some value, {i} when value < 0 -> {i}"] |> String.concat "\n"
        template "IncompleteTarget.fs" ["{{CLAUSES}}", clauses]
        |> withOptions ["--nowarn:25"]
        |> runWithSettings true true

    [<Theory>]
    [<InlineData("int", false)>]
    [<InlineData("int", true)>]
    [<InlineData("unit", false)>]
    [<InlineData("unit", true)>]
    [<InlineData("nonTailAction", false)>]
    [<InlineData("nonTailAction", true)>]
    [<InlineData("nonTailMatch", false)>]
    [<InlineData("nonTailMatch", true)>]
    let ``Issue 20632 - promoted joins preserve attributed tail diagnostics`` kind optimize =
        let recurse = "run (remaining - 1) token stack col"
        let finish, fallback =
            if kind = "unit" then "()", "()" else "0", "-1"
        let action = if kind = "nonTailAction" then $"1 + {recurse}" else recurse
        let expression = $"""(
            match token, stack with
            | (0 | 1 | 2 | 3 | 4 | 5 | 6 | 7), C0 value :: _ when guard 0 value col -> {action}
            | _ -> {fallback})"""
        let prefix = if kind = "nonTailMatch" then "1 + " else ""
        let code = $"""[<TailCall>]
let rec run remaining token stack col =
    if remaining = 0 then {finish} else
        {prefix}{expression}"""
        let compilation =
            template "ContextMatch.fs" ["{{MODULE_ATTRIBUTE}}", ""; "{{CASE}}", code]
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
                template "ActivePatternGrowth.fs" [
                    "{{CONTEXTS}}", ([for i in 0..31 -> $"C{i} of int"] |> String.concat " | ")
                    "{{GUARDS}}", ([for i in 0..31 -> $"    | _, C{i} (Identity value) :: _ when guard {i} value col -> {100 + i}"] |> String.concat "\n")
                    "{{TAILS}}", ([for i in 1..30 -> $"    | {i}, _ :: _ -> {i}"] |> String.concat "\n") ]
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
