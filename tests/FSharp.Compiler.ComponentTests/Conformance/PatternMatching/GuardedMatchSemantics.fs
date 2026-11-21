namespace Conformance.PatternMatching

open System.IO
open System.Reflection.Metadata
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module GuardedMatchSemantics =
    let private fixture name = Path.Combine(__SOURCE_DIRECTORY__, "GuardedMatchSemantics", name)
    let private template name replacements =
        (File.ReadAllText(fixture name), replacements)
        ||> List.fold (fun source (hole: string, value: string) -> source.Replace(hole, value))
        |> FSharp
    let private modes cases =
        [ for case in cases do
              for optimize in [false; true] -> [|box case; box optimize|] ]
    let private run optimize =
        withOptimization optimize >> withRealInternalSignature true >> compileExeAndRun >> shouldSucceed
    let private decision yes no = $"""
            match token, x, y with
            | 0, _, 1 -> {no}
            | (0 | 1), _, 0 when guard x -> {yes}
            | _ -> {no}"""
    let private header = """module Test
open System.Threading.Tasks
type Anchor = class end
let mutable calls = 0
let guard x = calls <- calls + 1; x >= 0
let check condition = if not condition then failwith "result"
"""
    let private program prefix yes no suffix = header + prefix + decision yes no + "\n" + suffix
    let private entry checks = "\n[<EntryPoint>]\nlet main _ =\n    " + checks + "\n    0"
    let executableCases =
        modes ["byref"; "rethrow"; "protected"; "tail"; "task"; "int"; "argument"; "function"; "delegate"; "interface"; "tuple"; "applied"
               "observations"; "total payload"; "partial payload"; "recursive self"; "recursive later"; "incomplete"]
    let tailCases = modes ["int"; "unit"; "non-tail action"; "non-tail match"]
    let sequenceCases = modes ["sequence"; "array"; "list"; "toArray"; "toList"; "yieldFrom"]
    let runtimeCases = modes ["body"; "await"; "callback"; "sequence"]

    let private scopeSource scope =
        let prefix, yes, no, suffix, checks =
            match scope with
            | "byref" ->
                "let run (data: int[]) token x y : byref<int> =", "&data[1]", "&data[0]", "",
                "let data = [|0; 0|]\n    (run data 1 0 0) <- 7\n    (run data 0 -1 0) <- 9\n    check (data = [|9; 7|])"
            | "rethrow" ->
                "let expected = System.Exception()\nlet run token x y : int =\n    try raise expected with _ ->",
                "reraise()", "reraise()", "",
                """for x in [0; -1] do
        try run 1 x 0 |> ignore; failwith "missing rethrow"
        with e when obj.ReferenceEquals(e, expected) -> ()"""
            | "protected" ->
                "type Holder() =\n    member this.Run(token, x, y) =", "this.MemberwiseClone() |> ignore; 1", "-1", "",
                "let h = Holder()\n    check (h.Run(1, 0, 0) = 1 && h.Run(0, -1, 0) = -1)"
            | "tail" ->
                "let run token x y =\n    let rec loop n result =\n        if n = 0 then result else",
                "loop (n - 1) 1", "loop (n - 1) -1", "    loop 100000 0",
                "check (run 1 0 0 = 1 && run 0 -1 0 = -1)"
            | _ ->
                "let run token x y = task {\n    do! Task.Yield()\n    return (", "1", "-1", "    ) }",
                "check ((run 1 0 0).Result = 1 && (run 0 -1 0).Result = -1)"
        FSharp (program prefix yes no (suffix + entry checks))
        |> (if scope = "tail" then withOptions ["--tailcalls-"] else id)

    [<Theory; MemberData(nameof tailCases)>]
    let ``Issue 20632 - joins retain TailCall diagnostics`` kind optimize =
        let finish = if kind = "unit" then "()" else "0"
        let action = (if kind = "non-tail action" then "1 + " else "") + "run (n - 1) token x y"
        let add = if kind = "non-tail match" then "1 + " else ""
        FSharp (program $"[<TailCall>]\nlet rec run n token x y =\n    if n = 0 then {finish} else {add}(" action finish ")")
        |> asLibrary |> withLangVersion80 |> withOptimization optimize |> compile
        |> (if kind = "non-tail action" || kind = "non-tail match" then shouldFail >> withWarningCode 3569 else shouldSucceed)
        |> ignore

    let private resultSource kind =
        let declarations, yes, no, wrap, observe, expected =
            match kind with
            | "function" -> "", "(fun n -> acc <- acc + calls; n + acc)", "(fun n -> n - 1)", "",
                            "calls <- calls + 100\n    result 10 |> ignore\n    result 10", "if accepted then 212 else 9"
            | "delegate" -> "type D = delegate of unit -> int\n", "D(fun () -> calls)", "D(fun () -> -1)", "",
                            "result", "if accepted then 1 else -1"
            | "applied" -> "", "(fun n -> n + calls)", "(fun n -> n - 1)", "", "result", "if accepted then 43 else 41"
            | "interface" -> "type I = abstract Value: int\n", "{ new I with member _.Value = calls }",
                             "{ new I with member _.Value = -1 }", "", "result.Value", "if accepted then 1 else -1"
            | "tuple" -> "", "(calls, true)", "(-1, false)", "", "result", "if accepted then (1, true) else (-1, false)"
            | _ -> "", "calls", "-1", (if kind = "argument" then "id " else ""), "result", "if accepted then 1 else -1"
        let locals = if kind = "function" then "    let mutable acc = 0\n" else ""
        let apply = if kind = "delegate" then ".Invoke()" elif kind = "applied" then " 42" else ""
        let checks = $"""for token in [0; 1; 2] do
        for x, y in [0, 0; -1, 0; 0, 1] do
            calls <- 0
            let accepted = token < 2 && x >= 0 && y = 0
            check (run token x y = ({expected}))
            check (calls = (if token < 2 && y = 0 then 1 else 0) + {if kind = "function" then "100" else "0"})"""
        FSharp (program (declarations + $"let run token x y =\n{locals}    let result =\n        {wrap}(") yes no
                    ($"        ){apply}\n    {observe}" + entry checks))

    [<Theory; MemberData(nameof executableCases)>]
    let ``Issue 20632 - shared decisions preserve semantics`` kind optimize =
        let source =
            match kind with
            | "observations" -> template "Observations.fs" []
            | "total payload" | "partial payload" ->
                template "PayloadOrder.fs" [
                    "{{PATTERN}}", (if kind = "partial payload" then "Read 0 (Observe values)" else "Observe values")
                    "{{PARTIAL}}", (if kind = "partial payload" then "true" else "false") ]
                |> withOptions ["--nowarn:75"; "--extraoptimizationloops:1"; "--parallelcompilation+"]
            | "recursive self" | "recursive later" ->
                template "RecursiveBindings.fs" [
                    "{{USE}}", (if kind = "recursive self" then "(id (fun () -> first)) ()" else "later")
                    "{{EXPECTED}}", (if kind = "recursive self" then "-1" else "(if token = 0 then 1 else 2)") ]
                |> withLangVersionPreview
            | "incomplete" ->
                FSharp """module Test
let run input =
    match input with
    | None, (0 | 1) when false -> -1
    | Some v, 0 when v < 0 -> 0
    | Some v, _ when v = 0 -> v
    | Some _, _ -> 99
[<EntryPoint>]
let main _ =
    if run (Some 0, 2) <> 0 || run (Some 42, 0) <> 99 then failwith "join result"
    try run (None, 2) |> ignore; failwith "missing match failure"
    with :? MatchFailureException -> ()
    0"""
                |> withNoWarn 25
            | "byref" | "rethrow" | "protected" | "tail" | "task" -> scopeSource kind
            | _ -> resultSource kind
        let result = source |> run optimize
        if kind = "int" && optimize then
            let size = result |> withPeReader (fun pe ->
                let md = pe.GetMetadataReader()
                md.MethodDefinitions |> Seq.sumBy (fun h ->
                    let m = md.GetMethodDefinition h
                    if m.RelativeVirtualAddress = 0 || md.GetString(md.GetTypeDefinition(m.GetDeclaringType()).Name) = "Context"
                    then 0 else pe.GetMethodBody(m.RelativeVirtualAddress).GetILContent().Length))
            Assert.True(size < 1500, $"tuple3 IL: {size}")

    [<TheoryForNETCOREAPP; MemberData(nameof runtimeCases)>]
    let ``Issue 20632 - runtime async construction and rejection`` kind optimize =
        let declaration, wrap, yes, no =
            match kind with
            | "body" -> "", "__runtimeAsyncReturn", "AsyncHelpers.Await ready; calls", "-1"
            | "await" ->
                "let inline wrap (f: int -> Task<int>) = __runtimeAsyncReturn (AsyncHelpers.Await (f 10))\n",
                "wrap", "(fun _ -> Task.FromResult calls)", "(fun _ -> AsyncHelpers.Await ready; Task.FromResult -1)"
            | "callback" ->
                "let inline wrap ([<InlineIfLambda>] f: int -> int) = __runtimeAsyncReturn (f 10)\n",
                "wrap", "(fun _ -> calls)", "(fun _ -> AsyncHelpers.Await ready; -1)"
            | _ -> "", "__runtimeAsyncSequence", "(fun () -> seq { yield calls })", "(fun () -> Seq.empty)"
        let checks = """let ready = TaskCompletionSource<unit>()
    let work = run ready.Task 1 0 0
    {{SUSPENSION}}
    ready.SetResult(())
    check (work.GetAwaiter().GetResult() = 1)
    check ((run Task.CompletedTask 0 -1 0).GetAwaiter().GetResult() = -1)"""
        let suspension = if kind = "body" then "check (not work.IsCompleted)" else "()"
        let suffix = ")" + (if kind = "sequence" then "" else entry (checks.Replace("{{SUSPENSION}}", suspension)))
        let source =
            program ("open System.Runtime.CompilerServices\nopen Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers\n"
                     + declaration + $"let run (ready: Task) token x y = {wrap}(") yes no
                suffix
            |> FSharp |> withLangVersionPreview |> withFSharpCoreShippedNet |> withOptimization optimize
        if kind = "sequence" then source |> asLibrary |> compile |> shouldFail |> withErrorCode 3922 |> ignore
        else source |> run optimize |> ignore

    [<TheoryForNETCOREAPP; MemberData(nameof sequenceCases)>]
    let ``Issue 20632 - collections retain continuations`` kind optimize =
        let opening, closing =
            match kind with
            | "array" -> "[|", "|]"
            | "list" -> "[", "]"
            | "toArray" -> "seq {", "} |> Seq.toArray"
            | "toList" -> "seq {", "} |> Seq.toList"
            | _ -> "seq {", "}"
        template "SequenceResults.fs" [
            "{{OPEN}}", opening; "{{CLOSE}}", closing
            "{{PREFIX}}", (if kind = "yieldFrom" then "yield!" else "")
            "{{EMPTY}}", (if kind = "yieldFrom" then "Seq.empty" else "()")
            "{{SUCCESS}}", (if kind = "yieldFrom" then "Seq.singleton value" else "yield value")
            "{{ALLOCATION}}", (if kind = "sequence" || kind = "yieldFrom" then "true" else "false") ]
        |> run optimize |> ignore

    [<Theory; InlineData("guard"); InlineData("action")>]
    let ``Issue 20632 - recursive initialization remains strict`` site =
        FSharp (program "let run token x y =\n    let rec value ="
                    (if site = "action" then "value + 1" else "0") "0" "    value"
                |> fun s -> if site = "guard" then s.Replace("guard x", "value = 42") else s)
        |> withLangVersionPreview |> typecheck |> shouldFail |> withErrorCode 31 |> ignore

    [<Fact>]
    let ``Issue 20632 - inline guards remain portable`` () =
        let library = FSharp (program "let inline run token x y =" "1" "-1" "") |> asLibrary |> withName "GuardedInline"
        FSharp ("module Consumer" + entry "Test.check (Test.run 1 0 0 = 1 && Test.run 0 -1 0 = -1)")
        |> withReferences [library] |> run true |> ignore

    [<Theory; InlineData("quote"); InlineData("argument"); InlineData("function"); InlineData("type"); InlineData("module")>]
    [<InlineData("LINQ"); InlineData("overload"); InlineData("builder"); InlineData("top module"); InlineData("constructor")>]
    let ``Issue 20632 - quotation shape remains unchanged`` origin =
        let fn = "let run token x y ="
        let lam = "fun token x y ->"
        let reflected m = $"let q = Microsoft.FSharp.Quotations.Expr.TryGetReflectedDefinition({m}).Value"
        let runMethod = reflected """typeof<Anchor>.DeclaringType.GetMethod("run")"""
        let linq = "System.Linq.Expressions.Expression<System.Func<int, int, int, int>>"
        let prefix, suffix =
            match origin with
            | "quote" -> $"let q = <@ {lam}", "@>"
            | "argument" ->
                "open Microsoft.FSharp.Quotations\ntype Sink =\n"
                + "    static member Capture([<ReflectedDefinition>] v: Expr<int -> int -> int -> int>) = v\n"
                + $"let q = Sink.Capture({lam}", ")"
            | "function" | "top module" -> "[<ReflectedDefinition>]\n" + fn, runMethod
            | "type" -> "[<ReflectedDefinition>]\ntype Holder =\n    static member Run(token, x, y) =",
                        reflected """typeof<Holder>.GetMethod("Run")"""
            | "module" -> "[<ReflectedDefinition>]\nmodule Holder =\n    " + fn,
                          reflected """typeof<Anchor>.DeclaringType.GetNestedType("Holder").GetMethod("run")"""
            | "LINQ" | "overload" ->
                $"type Sink =\n    static member Capture(v: {linq}) = v\n"
                + (if origin = "overload" then "    static member Capture(v: string) = v\n" else "")
                + $"let q = Sink.Capture({lam}", ")"
            | "builder" ->
                "open Microsoft.FSharp.Quotations\ntype Builder() =\n"
                + "    member _.Return(v) = v\n    member _.Delay(f) = f()\n"
                + "    member _.Quote(v: Expr<'T>) = v\n    member _.Run(v: Expr<'T>) = v\n"
                + $"let q = (Builder() {{ return ({lam}", ") })"
            | _ -> "type Holder [<ReflectedDefinition>](token: int, x: int, y: int) =\n    let result =",
                   "    member _.Result = result\n" + reflected "typeof<Holder>.GetConstructors()[0]"
        let source = program prefix "1" "-1" suffix
        FSharp (if origin = "top module" then "[<ReflectedDefinition>]\n" + source else source)
        |> withAdditionalSourceFile (SourceFromPath(fixture "VerifyQuotation.fs"))
        |> run true |> withStdOutContains "calls=2" |> ignore

    [<Theory; InlineData(false); InlineData(true)>]
    let ``Issue 20632 - guards retain caller sequence points`` optimize =
        FSharp (program "let run token x y =" "1" "-1" "")
        |> asLibrary |> withOptimization optimize |> withEmbeddedPdb |> compile |> shouldSucceed
        |> withPeReader (fun pe ->
            let entry = pe.ReadDebugDirectory() |> Seq.find (fun e ->
                e.Type = System.Reflection.PortableExecutable.DebugDirectoryEntryType.EmbeddedPortablePdb)
            use provider = pe.ReadEmbeddedPortablePdbDebugDirectoryData entry
            let md = pe.GetMetadataReader()
            let method = md.MethodDefinitions |> Seq.find (fun h -> md.GetString(md.GetMethodDefinition(h).Name) = "run")
            let points = provider.GetMetadataReader().GetMethodDebugInformation(method.ToDebugInformationHandle()).GetSequencePoints()
            Assert.Contains(points, fun p ->
                not p.IsHidden && p.StartLine = 10 && p.EndLine = 10 && p.StartColumn = 34 && p.EndColumn = 41))

    [<Fact>]
    let ``Join flags are not pickled`` () =
        let ofBits = global.FSharp.Compiler.TypedTree.ValFlags.OfPickledBits
        let empty = ofBits 0L
        let join, param = empty.WithIsDecisionTreeJoin, empty.WithIsMatchJoinParameter
        Assert.True(join.IsDecisionTreeJoin && not join.IsPinning && not empty.WithIsPinning.IsDecisionTreeJoin)
        Assert.True(param.IsMatchJoinParameter && not param.IsPinning && not param.IsDecisionTreeJoin)
        Assert.False((ofBits join.PickledBits).IsDecisionTreeJoin || (ofBits param.PickledBits).IsMatchJoinParameter)

#if NETCOREAPP
    [<Collection(nameof NotThreadSafeResourceCollection)>]
    type CompilerAllocation() =
        [<Fact>]
        member _.``Issue 20632 - 32 guarded total active patterns compile within 1GB`` () =
            let cases = [for i in 0..31 -> $"C{i} of int"] |> String.concat " | "
            let guards = [for i in 0..31 -> $"    | _, C{i} (Identity v) :: _ when v >= {i} -> {i}"]
            let tails = [for i in 1..30 -> $"    | {i}, _ :: _ -> {i}"]
            let source =
                FSharp ($"module Test\ntype C = {cases}\nlet (|Identity|) v = v\nlet run token stack =\n"
                        + "    match token, stack with\n    | 0, C0 v :: _ when v < 0 -> -1\n"
                        + String.concat "\n" (guards @ tails) + "\n    | (31 | 32), _ :: _ when token >= 0 -> token\n    | _ -> -1")
                |> asLibrary |> withLangVersionPreview |> withOptimize |> withRealInternalSignature false
                |> withOptions ["--nowarn:75"; "--extraoptimizationloops:0"; "--parallelcompilation-"]
            let compileSource () = source |> compile |> shouldSucceed |> ignore
            compileSource ()
            let before = System.GC.GetTotalAllocatedBytes true
            compileSource ()
            Assert.True(System.GC.GetTotalAllocatedBytes true - before < 1_000_000_000L)
#endif
