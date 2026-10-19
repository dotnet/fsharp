// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open System.Reflection.Metadata
open System.Reflection.PortableExecutable
open Xunit
open FSharp.Test.Compiler

module GuardedMatchSemantics =

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

    [<Theory>]
    [<InlineData(false, false)>]
    [<InlineData(false, true)>]
    [<InlineData(true, false)>]
    [<InlineData(true, true)>]
    let ``Issue 20632 - preserve committed observations and projection demand`` optimize realSignature =
        let alternatives = [ for i in 1..64 -> string i ] |> String.concat " | "
        let smallHeadGuards = [ for i in 1..7 -> $"    | _, Second _ :: _ when not takeFirst -> {10 + i}" ] |> String.concat "\n"
        FSharp $"""module Test
open System
let trace = ResizeArray<int>()
let guard n = trace.Add n; false
type Cell = {{ mutable Value: int }}
let mutate cell = cell.Value <- 2; guard 2
let record token cell =
    match token, cell with
    | 0, {{ Value = 1 }} when guard 1 -> 0
    | _, {{ Value = 1 }} when mutate cell -> 1
    | ({alternatives}), _ -> 3
    | _, {{ Value = 1 }} -> 4
    | _ -> 5
let mutable calls = 0
let (|Seen|) (input: int) =
    calls <- calls + 1
    trace.Add calls
    calls
let active token input =
    match token, input with
    | 0, Seen value when guard 10 -> value
    | _, Seen value when guard 20 -> value
    | ({alternatives}), _ -> -1
    | _, Seen value -> value
let demand (input: (int * int) * bool) =
    match input with
    | _, true when guard 1 -> 1
    | _, true when guard 2 -> 2
    | _, true when guard 3 -> 3
    | _, true when guard 4 -> 4
    | _, true when guard 5 -> 5
    | _, true when guard 6 -> 6
    | _, true when guard 7 -> 7
    | _, true when guard 8 -> 8
    | (0, _), true -> -1
    | (3, _), true -> -1
    | (4, _), true -> -1
    | ((1, _) | (2, _)), true when guard 9 -> 9
    | _ -> 10
type U = A | B
type Payload = P of int | Q
type Pair = {{ mutable Left: Payload; mutable Right: Payload }}
let reread cell =
    match cell with
    | {{ Value = value }} when (cell.Value <- 2; true) -> value
    | _ -> -1
let saved pair =
    match pair with
    | {{ Left = P x; Right = P y }} when (pair.Left <- Q; pair.Right <- Q; true) -> x + y
    | _ -> -1
[<CLIMutable>]
type OpenCell = {{ Item: Payload }}
let cliMutate (cell: OpenCell) =
    typeof<OpenCell>.GetProperty("Item").SetValue(box cell, box Q)
    true
let cli token cell =
    let mutable result = 43
    match token, cell with
    | 0, _ -> result
    | _, {{ Item = P value }} when cliMutate cell ->
        result <- value
        result
    | (1 | 2 | 3 | 4), _ when token >= 0 ->
        result <- 42
        result
    | _ -> result
type SmallToken = First of int | Second of int
let nullDefault takeFirst token (stack: int list) =
    match token, stack with
    | First _, 1 :: _ when takeFirst -> 10
    | _, _ :: _ when not takeFirst -> 11
    | _, _ :: _ when not takeFirst -> 12
    | _, _ :: _ when not takeFirst -> 13
    | _, _ :: _ when not takeFirst -> 14
    | _, _ :: _ when not takeFirst -> 15
    | _, _ :: _ when not takeFirst -> 16
    | _, _ :: _ when not takeFirst -> 17
    | Second _, _ :: _ when not takeFirst -> 20
    | Second _, _ :: _ when not takeFirst -> 21
    | Second _, _ :: _ when not takeFirst -> 22
    | (First _ | Second _), _ :: _ when not takeFirst -> 23
    | _ -> 24
type Token = T0 | T1 | T2 | T3
let nullHead takeFirst token stack =
    match token, stack with
    | T0, First _ :: _ when takeFirst -> 10
{smallHeadGuards}
    | T1, _ :: _ when not takeFirst -> 20
    | T2, _ :: _ when not takeFirst -> 21
    | T3, _ :: _ when not takeFirst -> 22
    | (T1 | T2 | T3), _ :: _ when not takeFirst -> 23
    | _ -> 24
type Head = H0 of string | H1 of int | H2 | H3
let (|Probe|) head = match head with H0 text -> text.Length | _ -> 0
let refutedActive takeFirst token stack =
    match token, stack with
    | T0, H0 _ :: _ when takeFirst -> 10
    | _, (Probe 0 & H1 _) :: _ when takeFirst -> 11
    | _, _ :: _ when takeFirst -> 12
    | _, _ :: _ when takeFirst -> 13
    | _, _ :: _ when takeFirst -> 14
    | _, _ :: _ when takeFirst -> 15
    | _, _ :: _ when takeFirst -> 16
    | _, _ :: _ when takeFirst -> 17
    | T1, _ :: _ when takeFirst -> 20
    | T2, _ :: _ when takeFirst -> 21
    | T3, _ :: _ when takeFirst -> 22
    | (T1 | T2 | T3), _ :: _ when takeFirst -> 23
    | _ -> 24
let ignored flag (u: U) n =
    match u, n with
    | _, 0 -> 0
    | _ when (trace.Add 30; flag) -> 1
    | A, 1 -> 2
    | _ -> 3
[<EntryPoint>]
let main _ =
    let cell = {{ Value = 1 }}
    if record 0 cell <> 4 || cell.Value <> 2 || List.ofSeq trace <> [1; 2] then
        failwith "record observation"
    trace.Clear()
    if active 0 42 <> 1 || calls <> 1 || List.ofSeq trace <> [1; 10; 20] then
        failwith "active observation"
    trace.Clear()
    if demand (Unchecked.defaultof<int * int>, false) <> 10 || trace.Count <> 0 then
        failwith "projection demand"
    if ignored false Unchecked.defaultof<U> 2 <> 3 || List.ofSeq trace <> [30] then
        failwith "refuted column demand"
    if reread {{ Value = 1 }} <> 2 || saved {{ Left = P 1; Right = P 2 }} <> 3 then
        failwith "success argument observation"
    if cli 2 {{ Item = P 7 }} <> 7 then failwith "CLR-settable parent"
    if nullDefault true Unchecked.defaultof<SmallToken> [1] <> 10 then failwith "exhaustive null default"
    if nullHead true T0 [Unchecked.defaultof<SmallToken>] <> 10 then failwith "exhaustive head null default"
    if refutedActive false T0 [H0 Unchecked.defaultof<string>] <> 24 then failwith "refuted active demand"
    0
"""
        |> withOptimization optimize
        |> withOptions [ if realSignature then "--realsig+" else "--realsig-" ]
        |> compileExeAndRun
        |> shouldSucceed

    let private contextPrelude =
        let contexts = [ for i in 0..15 -> $"C{i} of int" ] |> String.concat " | "
        $"""module Test
type Context = {contexts}
let trace = ResizeArray<int>()
let guard n value col = trace.Add n; col <= value + n
"""

    let private indent (source: string) =
        source.Split('\n') |> Array.map (fun line -> "    " + line) |> String.concat "\n"

    let private contextMatch action fallback =
        let guards = [ for i in 0..15 -> $"| _, C{i} value :: _ when guard {i} value col -> {action (string (100 + i))}" ] |> String.concat "\n"
        let tails = [ for i in 1..14 -> $"| {i}, _ :: _ -> {action (string i)}" ] |> String.concat "\n"
        $"""match token, stack with
| 0, C0 value :: _ when col < value -> {fallback}
{guards}
{tails}
| (15 | 16), _ :: _ when col >= 0 -> {action "token"}
| _ -> {fallback}
"""

    let private contextFunction mutableActions =
        let action value = if mutableActions then $"result <- {value}; result" else value
        let fallback = if mutableActions then "result" else "-1"
        let local = if mutableActions then "    let mutable result = -1\n" else ""
        $"let run token stack col =\n{local}{indent (contextMatch action fallback)}"

    [<Fact>]
    let ``Issue 20632 - context guards share code across distinct completed tails`` () =
        FSharp (contextPrelude + contextFunction true + """
[<EntryPoint>]
let main _ =
    for token in 1..16 do
        if run token [C0 0] 1000 <> token then failwith "completed tail"
    if trace.Count <> 16 then failwith ("guard count: " + string trace.Count)
    if run 5 [C15 0] 0 <> 115 then failwith "guard success"
    if run 5 [] 1000 <> -1 then failwith "empty list"
    0
        """)
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
        let source =
            match scope with
            | "byref" ->
                let body = contextMatch (fun value -> $"&data[({value}) %% data.Length]") "&data[0]"
                "let run (data: int[]) token stack col : byref<int> =\n" + indent body + """
[<EntryPoint>]
let main _ =
    let data = Array.zeroCreate<int> 128
    (run data 5 [C15 0] 0) <- 99
    (run data 5 [C0 0] 1000) <- 77
    if data[115] <> 99 || data[5] <> 77 then failwith "byref target"
    0
"""
            | "rethrow" ->
                let body = contextMatch (fun _ -> "reraise()") "reraise()"
                "let expected = System.Exception(\"identity\")\nlet run token stack col : int =\n    try raise expected with _ ->\n" + indent (indent body) + """
[<EntryPoint>]
let main _ =
    for stack in [ [C15 0]; [] ] do
        try
            run 5 stack 0 |> ignore
            failwith "missing rethrow"
        with error when obj.ReferenceEquals(error, expected) -> ()
    0
"""
            | "protected" ->
                let body = contextMatch (fun value -> $"this.MemberwiseClone() |> ignore; {value}") "-1"
                "type Holder() =\n    member this.Run(token, stack, col) =\n" + indent (indent body) + """
[<EntryPoint>]
let main _ =
    let holder = Holder()
    if holder.Run(5, [C15 0], 0) <> 115 || holder.Run(5, [C0 0], 1000) <> 5 then failwith "protected target"
    0
"""
            | "tail" ->
                let body = contextMatch (fun value -> $"loop (remaining - 1) ({value})") "loop (remaining - 1) -1"
                "let run token stack col =\n    let rec loop remaining result =\n        if remaining = 0 then result else\n" + indent (indent body) + """
    loop 100000 0
[<EntryPoint>]
let main _ =
    if run 5 [C15 0] 0 <> 115 || run 5 [C0 0] 1000 <> 5 then failwith "tail target"
    0
"""
            | "task" ->
                let body = contextMatch id "-1" |> indent |> indent |> indent
                "let run token stack col =\n    task {\n        do! System.Threading.Tasks.Task.Yield()\n        return (\n" + body + """
        )
    }
[<EntryPoint>]
let main _ =
    if (run 5 [C15 0] 0).Result <> 115 || (run 5 [] 0).Result <> -1 then
        failwith "task target"
    0
"""
            | "runtime" ->
                let body = contextMatch (fun value -> $"System.Runtime.CompilerServices.AsyncHelpers.Await ready; {value}") "-1"
                "open System.Threading.Tasks\nlet run (ready: Task) token stack col : Task<int> =\n    Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers.__runtimeAsyncReturn (\n" + indent (indent body) + """
    )
[<EntryPoint>]
let main _ =
    let ready = TaskCompletionSource<unit>()
    let work = run ready.Task 5 [C15 0] 0
    if work.IsCompleted then failwith "missing suspension"
    ready.SetResult(())
    if work.GetAwaiter().GetResult() <> 115 || (run Task.CompletedTask 5 [] 0).GetAwaiter().GetResult() <> -1 then
        failwith "runtime async target"
    0
"""
            | _ -> failwith $"Unknown scope {scope}"
        FSharp (contextPrelude + source)
        |> (if scope = "runtime" then withLangVersionPreview >> withFSharpCoreShippedNet else id)
        |> withOptimization optimize
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory>]
    [<InlineData(false, false)>]
    [<InlineData(false, true)>]
    [<InlineData(true, false)>]
    [<InlineData(true, true)>]
    let ``Issue 20632 - grouped guards preserve mutable observations and aliased list tails`` optimize realSignature =
        let guards = [ for i in 0..7 -> $"    | _, C0 {{ Value = 1 }} :: _ when guard {i} cell -> result <- {100 + i} + cell.Value; result" ] |> String.concat "\n"
        let tails = [ for i in 1..3 -> $"    | {i}, _ :: rest -> result <- {200 + i} + rest.Length; result" ] |> String.concat "\n"
        FSharp $"""module Test
open Microsoft.FSharp.Core.CompilerServices
type Cell = {{ mutable Value: int }}
type Context = C0 of Cell | C1 | C2 | C3
let trace = ResizeArray<int>()
let mutable accepted = -2
let mutable alias = ListCollector<Context>()
let guard n cell =
    trace.Add n
    if n <= 0 then
        cell.Value <- 2
        alias.Add C1
        alias.Close() |> ignore
    n >= 0 && n = accepted
let run token stack cell =
    let mutable result = -1
    match token, stack with
    | 0, C0 cell :: _ when guard -1 cell -> result
{guards}
{tails}
    | (4 | 5), _ :: rest when token >= 0 -> result <- rest.Length; result
    | _ -> result
let makeStack cell =
    let mutable collector = ListCollector<Context>()
    collector.Add(C0 cell)
    alias <- collector
    collector.Close()
[<EntryPoint>]
let main _ =
    for token, wanted, expected in [(2, 1, 103); (2, -2, 203); (0, -2, -1)] do
        let cell = {{ Value = 1 }}
        let stack = makeStack cell
        accepted <- wanted
        trace.Clear()
        if run token stack cell <> expected || cell.Value <> 2 || stack.Length <> 2 then
            failwith "mutation or tail recipe"
        let expectedTrace = if token = 0 then [-1] elif wanted = 1 then [0; 1] else [0..7]
        if List.ofSeq trace <> expectedTrace then failwith "guard order"
    0
"""
        |> withOptimization optimize
        |> withOptions [ if realSignature then "--realsig+" else "--realsig-" ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory>]
    [<InlineData(false, false)>]
    [<InlineData(false, true)>]
    [<InlineData(true, false)>]
    [<InlineData(true, true)>]
    let ``Issue 20632 - payload observations precede tail tests and throwing guards`` partialPattern optimize =
        let payload = if partialPattern then "Read 0 (Observe values)" else "Observe values"
        let guards =
            [ for i in 0..7 -> $"            | _, C0 ({payload}) :: (_ :: _) when guard {i} values -> {100 + i} + values[0]" ]
            |> String.concat "\n"
        FSharp $"""module Test
open System
type Context = C0 of int[] | C1 | C2 | C3
let trace = ResizeArray<string>()
let expected = InvalidOperationException("guard")
let mutable throwAt = -2
let guard n (values: int[]) =
    trace.Add($"g{{n}}:{{values[0]}}")
    values[0] <- values[0] + 1
    if n = throwAt then raise expected
    n = 6 && throwAt = -2
let (|Observe|) (values: int[]) =
    trace.Add($"a:{{values[0]}}")
    values
let (|Read|_|) n (values: int[]) =
    trace.Add($"p{{n}}:{{values[0]}}")
    if values[0] >= n then Some values else None
let run token stack =
    try
        try
            match token, stack with
            | 0, C0 values :: tail when guard -1 values -> -100 + tail.Length
{guards}
            | 1, _ :: tail -> 201 + tail.Length
            | 2, _ :: tail -> 202 + tail.Length
            | 3, _ :: tail -> 203 + tail.Length
            | (4 | 5), _ :: tail when token >= 0 -> 204 + tail.Length
            | _ -> -1
        with error when (trace.Add("filter"); obj.ReferenceEquals(error, expected)) -> -2
    finally trace.Add("finally")
let check stop length expectedResult expectedValue expectedTrace =
    let values = [|1|]
    throwAt <- stop
    trace.Clear()
    let stack = if length = 1 then [C0 values] else [C0 values; C3]
    if run 2 stack <> expectedResult || values[0] <> expectedValue || List.ofSeq trace <> expectedTrace then
        failwith (String.concat "," trace)
[<EntryPoint>]
let main _ =
    let partialPattern = {if partialPattern then "true" else "false"}
    let observations value = if partialPattern then [$"p0:{{value}}"; $"a:{{value}}"] else []
    let prefix = if partialPattern then [] else ["a:1"]
    let traceThrough last =
        [ yield! prefix
          for n in 0..last do
              yield! observations (n + 1)
              yield $"g{{n}}:{{n + 1}}" ]
    let shortTrace = if partialPattern then List.replicate 8 ["p0:1"; "a:1"] |> List.concat else ["a:1"]
    check -2 1 202 1 (shortTrace @ ["finally"])
    check -2 2 114 8 (traceThrough 6 @ ["finally"])
    check -1 2 203 9 (traceThrough 7 @ ["finally"])
    for stop in 0..7 do
        check stop 2 -2 (stop + 2) (traceThrough stop @ ["filter"; "finally"])
    0
"""
        |> withOptimization optimize
        |> withOptions [ "--nowarn:75"; "--extraoptimizationloops:1"; "--parallelcompilation+" ]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory>]
    [<InlineData(0)>]
    [<InlineData(1)>]
    [<InlineData(2)>]
    [<InlineData(3)>]
    [<InlineData(4)>]
    [<InlineData(5)>]
    [<InlineData(6)>]
    [<InlineData(7)>]
    [<InlineData(8)>]
    [<InlineData(9)>]
    let ``Issue 20632 - preserve quotation shape`` mode =
        let functionSource = contextFunction false
        let lambda = functionSource.Replace("let run token stack col =", "fun token stack col ->").TrimEnd()
        let quoted =
            match mode with
            | 0 -> $"let q =\n    <@\n{indent (indent lambda)}\n    @> :> Expr"
            | 1 -> $"type Sink = static member Capture([<ReflectedDefinition>] value: Expr<int -> Context list -> int -> int>) = value\nlet q =\n    Sink.Capture(\n{indent (indent lambda)}\n    ) :> Expr"
            | 2 -> $"[<ReflectedDefinition>]\n{functionSource}\nlet q = Expr.TryGetReflectedDefinition(typeof<Context>.DeclaringType.GetMethod(\"run\")).Value"
            | 3 ->
                let memberSource = functionSource.Replace("let run token stack col =", "static member Run(token, stack, col) =")
                $"[<ReflectedDefinition>]\ntype Holder =\n{indent memberSource}\nlet q = Expr.TryGetReflectedDefinition(typeof<Holder>.GetMethod(\"Run\")).Value"
            | 4 -> $"[<ReflectedDefinition>]\nmodule Holder =\n{indent functionSource}\nlet q = Expr.TryGetReflectedDefinition(typeof<Context>.DeclaringType.GetNestedType(\"Holder\").GetMethod(\"run\")).Value"
            | 5 -> $"type Sink = static member Capture(value: System.Linq.Expressions.Expression<System.Func<int, Context list, int, int>>) = value\nlet q =\n    Sink.Capture(\n{indent (indent lambda)}\n    )"
            | 6 -> $"type Sink =\n    static member Capture(value: System.Linq.Expressions.Expression<System.Func<int, Context list, int, int>>) = value\n    static member Capture(value: string) = value\nlet q =\n    Sink.Capture(\n{indent (indent lambda)}\n    )"
            | 7 -> $"type Builder() =\n    member _.Return(value) = value\n    member _.Delay(f) = f()\n    member _.Quote(value: Expr<'T>) = value\n    member _.Run(value: Expr<'T>) = value\nlet q =\n    (Builder() {{\n        return (\n{indent (indent (indent lambda))}\n        )\n    }}) :> Expr"
            | 8 -> $"{functionSource}\nlet q = Expr.TryGetReflectedDefinition(typeof<Context>.DeclaringType.GetMethod(\"run\")).Value"
            | _ ->
                let body = contextMatch id "-1" |> indent |> indent
                $"type Holder [<ReflectedDefinition>](token: int, stack: Context list, col: int) =\n    let result =\n{body}\n    member _.Result = result\nlet q = Expr.TryGetReflectedDefinition(typeof<Holder>.GetConstructors()[0]).Value"
        let prelude =
            if mode = 8 then contextPrelude.Replace("module Test", "[<ReflectedDefinition>]\nmodule Test")
            else contextPrelude
        let source = String.concat "\n" [ prelude; "open Microsoft.FSharp.Quotations"; quoted ]
        let verifier = """module Verify
open Test
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.ExprShape
let rec calls expression =
    match expression with
    | Patterns.Call(_, method, args) -> (if method.Name = "guard" then 1 else 0) + List.sumBy calls args
    | ShapeVar _ -> 0
    | ShapeLambda(_, body) -> calls body
    | ShapeCombination(_, args) -> List.sumBy calls args
type Counter() =
    inherit System.Linq.Expressions.ExpressionVisitor()
    let mutable count = 0
    override this.VisitMethodCall(call) =
        if call.Method.Name = "guard" then count <- count + 1
        base.VisitMethodCall(call)
    member _.Count = count
[<EntryPoint>]
let main _ =
    let count =
        match box q with
        | :? Expr as expression -> calls expression
        | :? System.Linq.Expressions.Expression as expression ->
            let visitor = Counter()
            visitor.Visit(expression) |> ignore
            visitor.Count
        | _ -> failwith "Expected quotation or LINQ expression"
    printfn "calls=%d" count
    0
"""
        FSharp source
        |> withAdditionalSourceFile (FsSourceWithFileName "Verify.fs" verifier)
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "calls=288"

    [<Fact>]
    let ``Promoted joins do not replace the incomplete match target`` () =
        let clauses =
            [ for i in 0..40 -> $"    | Some value, {i} when value < 0 -> {i}" ]
            |> String.concat "\n"
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
    try
        run (None, 200) |> ignore
        failwith "missing match failure"
    with :? MatchFailureException -> ()
    0
"""
        |> withOptions [ "--nowarn:25" ]
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed
