// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Tiebreakers

open FSharp.Test
open FSharp.Test.Compiler
open Xunit
open Conformance.SharedTestHelpers

/// Source fixtures shared by the three feature-gating submodules below.
module TiebreakerFixtures =

    let case (desc: string) (source: string) : obj[] = [| box desc; box source |]

    let concretenessWarningSource =
        """
module Test

type Example =
    static member Invoke<'t>(value: Option<'t>) = "generic"
    static member Invoke<'t>(value: Option<'t list>) = "more concrete"

let result = Example.Invoke(Some([1]))
        """

    // Shared by the preview/non-preview pair below: two Compare overloads that are each more
    // concrete at a different position, so the call is incomparable (FS0041) at every langversion.
    let incomparableConcretenessSource =
        """
module Test

type Example =
    static member Compare(value: Result<int, 'error>) = "int ok"
    static member Compare(value: Result<'ok, string>) = "string error"

let result = Example.Compare(Ok 42 : Result<int, string>)
        """

    // Two 3-parameter overloads that disagree per parameter: the first is more concrete at
    // parameter 1 (int vs 'a), the second at parameter 2 (string vs 'y), and parameter 3
    // (Result<_,_>) is internally incomparable, so it favours neither. The call is therefore an
    // incomparable FS0041. This exercises the multi-parameter breakdown: positions must be reported
    // per formal parameter (1 and 2), never as flattened, duplicated type-argument indices.
    let multiParamIncomparableSource =
        """
module Test

type Example =
    static member Compare(a: int, b: 'y, c: Result<int, 'error>) = "one"
    static member Compare(a: 'a, b: string, c: Result<'ok, string>) = "two"

let result = Example.Compare(5, "hi", Ok 7)
        """

    let moreConcretDisabledAmbiguousCases: obj[] seq =
        [
            case
                "fully generic vs wrapped generic"
                "module Test\ntype Example =\n    static member Process(value: 't) = \"fully generic\"\n    static member Process(value: Option<'t>) = \"wrapped\"\nlet result = Example.Process(Some 42)\nif result <> \"wrapped\" then failwithf \"Expected 'wrapped' but got '%s'\" result"

            case
                "array generic vs bare generic"
                "module Test\ntype Example =\n    static member Handle(value: 't) = \"bare\"\n    static member Handle(value: 't array) = \"array\"\nlet result = Example.Handle([|1; 2; 3|])\nif result <> \"array\" then failwithf \"Expected 'array' but got '%s'\" result"
        ]

    let moreConcreteTestCases: obj[] seq =
        [
            case "Option<'T> vs Option<'T list> - nested list more concrete"
                 "module Test\ntype Resolver =\n    static member Resolve<'t>(x: Option<'t>) = \"generic\"\n    static member Resolve<'t>(x: Option<'t list>) = \"list\"\nlet result = Resolver.Resolve(Some [1;2;3])\nif result <> \"list\" then failwithf \"Expected 'list' but got '%s'\" result"

            case "Result<'T,'E> vs Result<'T, string> - partial concreteness"
                 "module Test\ntype Handler =\n    static member Handle<'t,'e>(x: Result<'t,'e>) = \"generic\"\n    static member Handle<'t>(x: Result<'t, string>) = \"string err\"\nlet result = Handler.Handle(Ok 42 : Result<int, string>)\nif result <> \"string err\" then failwithf \"Expected 'string err' but got '%s'\" result"

            case "'T vs Option<'T> - wrapped more concrete than bare"
                 "module Test\ntype Picker =\n    static member Pick<'t>(x: 't) = \"bare\"\n    static member Pick<'t>(x: Option<'t>) = \"option\"\nlet result = Picker.Pick(Some 1)\nif result <> \"option\" then failwithf \"Expected 'option' but got '%s'\" result"

            case "Option<'T> vs Option<Option<'T>> - double wrap more concrete"
                 "module Test\ntype Deep =\n    static member Go<'t>(x: Option<'t>) = \"single\"\n    static member Go<'t>(x: Option<Option<'t>>) = \"double\"\nlet result = Deep.Go(Some(Some 1))\nif result <> \"double\" then failwithf \"Expected 'double' but got '%s'\" result"

            case "list<'T> vs list<int * 'T> - tuple element more concrete"
                 "module Test\ntype Proc =\n    static member Run<'t>(x: list<'t>) = \"generic\"\n    static member Run<'t>(x: list<int * 't>) = \"paired\"\nlet result = Proc.Run([(1, \"a\")])\nif result <> \"paired\" then failwithf \"Expected 'paired' but got '%s'\" result"

            case "'a -> 'b vs 'a -> string - concrete range in function type"
                 "module Test\ntype Dispatcher =\n    static member Dispatch<'a, 'b>(handler: 'a -> 'b) = \"fully generic\"\n    static member Dispatch<'a>(handler: 'a -> string) = \"concrete range\"\nlet result = Dispatcher.Dispatch(fun (x: int) -> \"hello\")\nif result <> \"concrete range\" then failwithf \"Expected 'concrete range' but got '%s'\" result"

            case "'a * 'b vs 'a * int - concrete element in tuple type"
                 "module Test\ntype Handler =\n    static member Handle<'a, 'b>(pair: 'a * 'b) = \"fully generic tuple\"\n    static member Handle<'a>(pair: 'a * int) = \"concrete second\"\nlet result = Handler.Handle((\"hello\", 42))\nif result <> \"concrete second\" then failwithf \"Expected 'concrete second' but got '%s'\" result"

            case "'a -> 'b vs int -> 'b - concrete domain in function type"
                 "module Test\ntype Mapper =\n    static member Map<'a, 'b>(f: 'a -> 'b, items: 'a list) = \"generic\"\n    static member Map<'b>(f: int -> 'b, items: int list) = \"int domain\"\nlet result = Mapper.Map((fun x -> string x), [1; 2; 3])\nif result <> \"int domain\" then failwithf \"Expected 'int domain' but got '%s'\" result"

            case "'a * 'b vs int * 'b - concrete first element in tuple"
                 "module Test\ntype Tupler =\n    static member Pack<'a, 'b>(x: 'a * 'b) = \"generic\"\n    static member Pack<'b>(x: int * 'b) = \"int first\"\nlet result = Tupler.Pack((42, \"hello\"))\nif result <> \"int first\" then failwithf \"Expected 'int first' but got '%s'\" result"
        ]

    // Edge-case matrix: every row is FS0041 at --langversion:default and resolves to the concrete
    // overload "c" at preview, covering the member kinds the feature must serve: naked generics,
    // constructors, extension methods, static and instance methods on a generic type, optionals,
    // and paramarray.
    let mostConcreteEdgeCases: obj[] seq =
        let case (name: string) (source: string) =
            let checkedSource =
                source + sprintf "\nif r <> \"c\" then failwith (\"%s: got \" + r)" name

            [| box name; box checkedSource |]

        [
            case "naked-generic-ctor" """
module T
type W<'T>(t: string) =
    new(x: 'T)        = W<'T>("g")
    new(x: 'T option) = W<'T>("c")
    member _.T = t
let r = (W(Some 5)).T"""

            case "generic-extension" """
module T
open System.Runtime.CompilerServices
type W() = class end
[<Extension>]
type E =
    [<Extension>] static member M(w: W, x: 'T)        = "g"
    [<Extension>] static member M(w: W, x: 'T option) = "c"
let r = (W()).M(Some 5)"""

            case "static-method-generic-type" """
module T
type Factory<'T>() =
    static member Make(x: 'T)        = "g"
    static member Make(x: 'T option) = "c"
let r = Factory<_>.Make(Some 5)"""

            case "instance-method-generic-type" """
module T
type Factory<'T>() =
    member _.Make(x: 'T)        = "g"
    member _.Make(x: 'T option) = "c"
let r = Factory<_>().Make(Some 5)"""

            case "optional-tail" """
module T
type F() =
    static member M(x: 'T, ?y: int)        = "g"
    static member M(x: 'T option, ?y: int) = "c"
let r = F.M(Some 5)"""

            case "paramarray-tail" """
module T
type F() =
    static member M(x: 'T, [<System.ParamArray>] rest: int[])        = "g"
    static member M(x: 'T option, [<System.ParamArray>] rest: int[]) = "c"
let r = F.M(Some 5)"""
        ]

    // Result/partial-concreteness across multiple type parameters: one generic overload and one
    // whose type argument is partially pinned. Built for both the With (preview) and Without (10.0) pair.
    let example5PartialSource (methodName: string) (concreteParam: string) (concreteDesc: string) (callExpr: string) =
        $"""
module Test

type Example =
    static member {methodName}(value: Result<'ok, 'error>) = "fully generic"
    static member {methodName}(value: {concreteParam}) = "{concreteDesc}"

let result = Example.{methodName}({callExpr})
if result <> "{concreteDesc}" then
    failwithf "Expected '{concreteDesc}' but got '%%s' - wrong overload selected" result
        """

    // Task<'T> vs 'T factory overloads (the RFC's ValueTask-shaped motivation).
    let example7Source =
        """
module Test

open System.Threading.Tasks

[<NoComparison>]
type ValueTaskSimulator<'T> =
    | FromResult of 'T
    | FromTask of Task<'T>

type ValueTaskFactory =
    static member Create(result: 'T) = ValueTaskSimulator<'T>.FromResult result
    static member Create(task: Task<'T>) = ValueTaskSimulator<'T>.FromTask task

let createFromTask () =
    let task = Task.FromResult(42)
    let result = ValueTaskFactory.Create(task)
    result

// Discriminator: the concrete Task overload yields FromTask; picking the generic 'T overload
// would yield FromResult (with 'T = Task<int>) and fail here.
match createFromTask () with
| FromTask _ -> ()
| FromResult _ -> failwith "picked the generic 'T overload instead of the concrete Task<'T> one"
        """

    // Computation-expression Source overloads (FsToolkit AsyncResult pattern).
    let example8Source =
        """
module Test

open System

type AsyncResultBuilder() =
    member _.Return(x) = async { return Ok x }
    member _.ReturnFrom(x) = x
    
    member _.Source(result: Async<Result<'ok, 'error>>) : Async<Result<'ok, 'error>> = result
    member _.Source(result: Result<'ok, 'error>) : Async<Result<'ok, 'error>> = async { return result }
    member _.Source(asyncValue: Async<'t>) : Async<Result<'t, exn>> = 
        async { 
            let! v = asyncValue 
            return Ok v 
        }
    
    member _.Bind(computation: Async<Result<'ok, 'error>>, f: 'ok -> Async<Result<'ok2, 'error>>) =
        async {
            let! result = computation
            match result with
            | Ok value -> return! f value
            | Error e -> return Error e
        }

let asyncResult = AsyncResultBuilder()

let example () =
    let source : Async<Result<int, string>> = async { return Ok 42 }
    asyncResult.Source(source)

// Discriminator: the concrete Async<Result<_,_>> Source overload makes example() an
// Async<Result<int,string>>. The generic Async<'t> overload would make it
// Async<Result<Result<int,string>,exn>>, so matching Ok 42 (an int) would fail to type-check -
// proving the concrete overload was selected.
match Async.RunSynchronously(example ()) with
| Ok 42 -> ()
| _ -> failwith "wrong Source overload selected"
        """

    // Builder.Source with a Result overload vs a fully generic one.
    let realWorldSource =
        """
module Test

type Builder() =
    member _.Source(x: Result<'a, 'e>) = "result"
    member _.Source(x: 't) = "generic"

let b = Builder()

let result = b.Source(Ok 42 : Result<int, string>)
if result <> "result" then failwithf "Expected 'result' but got '%s' - wrong Source overload selected" result
        """

    // Same-module extension Source overloads resolved by concreteness.
    let fsToolkitSource =
        """
module Test

open System

type AsyncResultBuilder() =
    member _.Return(x) = async { return Ok x }

[<AutoOpen>]
module AsyncResultCEExtensions =
    type AsyncResultBuilder with
        member inline _.Source(result: Async<'t>) : Async<Result<'t, exn>> =
            async { 
                let! v = result 
                return Ok v 
            }
            
        member inline _.Source(result: Async<Result<'ok, 'error>>) : Async<Result<'ok, 'error>> =
            result

let asyncResult = AsyncResultBuilder()

let example () =
    let source : Async<Result<int, string>> = async { return Ok 42 }
    asyncResult.Source(source)

// Discriminator: the concrete Async<Result<_,_>> Source overload makes example() an
// Async<Result<int,string>>. The generic Async<'t> overload would make it
// Async<Result<Result<int,string>,exn>>, so matching Ok 42 (an int) would fail to type-check -
// proving the concrete overload was selected.
match Async.RunSynchronously(example ()) with
| Ok 42 -> ()
| _ -> failwith "wrong Source overload selected"
        """

    // Cross-feature: a high-priority (ORPA) less-concrete overload beats a low-priority more-concrete
    // one under preview, but with both features off the call is ambiguous.
    let orpWinsSource =
        """
module Test
open System.Runtime.CompilerServices
type C<'T>() =
    [<OverloadResolutionPriority(1)>] static member M(x: 'T)        = "high-generic"
    static member M(x: 'T option) = "low-concrete"
let r = C<_>.M(Some 5)
if r <> "high-generic" then failwithf "expected high-generic, got %s" r
        """

    // Three overloads where two generic ones are bypassed in favour of the most concrete (FS3576).
    let multipleBypassedSource =
        """
module Test

type Example =
    static member Process<'t>(value: 't) = "fully generic"
    static member Process<'t>(value: Option<'t>) = "option generic"
    static member Process<'t>(value: Option<'t list>) = "most concrete"

let result = Example.Process(Some([1]))
        """

/// Tests whose outcome is identical whether or not the most-concrete tiebreaker is enabled.
/// Some are pinned to a feature-off version (no-pin/10.0/latest) and resolve via an earlier rule
/// (exact match, PreferNonGeneric/NonExtension, TDC, adhoc, SRTP). Others are pinned to preview to
/// prove the feature does NOT change their outcome - either an earlier rule still decides them, or
/// they stay ambiguous (FS0041) even with the tiebreaker on (incomparable / SRTP-only differences).
module AgnosticOfTieBreakerFeature =

    open TiebreakerFixtures

    let genericVsConcreteNestingCases: obj[] seq =
        [
            case
                "Basic - Option<'t> vs Option<int>"
                "module Test\ntype Example =\n    static member Invoke(value: Option<'t>) = \"generic\"\n    static member Invoke(value: Option<int>) = \"int\"\nlet result = Example.Invoke(Some 42)\nif result <> \"int\" then failwithf \"Expected 'int' but got '%s' - wrong overload selected\" result"

            case
                "Nested - Option<Option<'t>> vs Option<Option<int>>"
                "module Test\ntype Example =\n    static member Handle(value: Option<Option<'t>>) = \"nested generic\"\n    static member Handle(value: Option<Option<int>>) = \"nested int\"\nlet result = Example.Handle(Some(Some 42))\nif result <> \"nested int\" then failwithf \"Expected 'nested int' but got '%s' - wrong overload selected\" result"

            case
                "Triple nesting - list<Option<Result<'t, exn>>> vs list<Option<Result<int, exn>>>"
                "module Test\ntype Example =\n    static member Deep(value: list<Option<Result<'t, exn>>>) = \"generic\"\n    static member Deep(value: list<Option<Result<int, exn>>>) = \"int\"\nlet result = Example.Deep([Some(Ok 42)])\nif result <> \"int\" then failwithf \"Expected 'int' but got '%s' - wrong overload selected\" result"
        ]

    [<Theory>]
    [<MemberData(nameof genericVsConcreteNestingCases)>]
    let ``Generic vs concrete at varying nesting depths`` (_description: string) (source: string) =
        FSharp source
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Example 5 - Multiple Type Parameters - Result fully concrete wins`` () =
        FSharp """
module Test

type Example =
    static member Transform(value: Result<'ok, 'error>) = "fully generic"
    static member Transform(value: Result<int, 'error>) = "int ok"
    static member Transform(value: Result<'ok, string>) = "string error"
    static member Transform(value: Result<int, string>) = "both concrete"

let result = Example.Transform(Ok 42 : Result<int, string>)
if result <> "both concrete" then
    failwithf "Expected 'both concrete' but got '%s' - wrong overload selected" result
        """
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Example 6 - Incomparable Concreteness - Result int e vs Result t string - ambiguous with helpful message`` () =
        // Preview is required: the "strictly more concrete" detail is gated behind MoreConcreteTiebreaker.
        FSharp incomparableConcretenessSource
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withErrorCode 41 // FS0041: A unique overload could not be determined
        |> withDiagnosticMessageMatches "Neither candidate is strictly more concrete"
        // The detail names each distinct candidate signature (not the bare "Compare" name) and
        // attributes concreteness to the correct type-argument position: Result<int,'error> has the
        // concrete 'int' at position 1, Result<'ok,string> has the concrete 'string' at position 2.
        |> withDiagnosticMessageMatches "Result<int,'error> -> string is more concrete at position 1"
        |> withDiagnosticMessageMatches "Result<'ok,string> -> string is more concrete at position 2"
        |> ignore

    [<Fact>]
    let ``Multi-parameter incomparable concreteness reports clean per-parameter positions`` () =
        // Regression: with multiple parameters the breakdown must report the formal-parameter index,
        // not flattened per-type-argument indices. A same-constructor parameter (Result<_,_>) is
        // internally incomparable and must simply drop out, so each method is more concrete at exactly
        // one parameter position - never the conflated, duplicated "positions 1, 1" / "positions 2, 2".
        FSharp multiParamIncomparableSource
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withErrorCode 41
        |> withDiagnosticMessageMatches "Neither candidate is strictly more concrete"
        |> withDiagnosticMessageMatches @"b: 'y \* c: Result<int,'error> -> string is more concrete at position 1"
        |> withDiagnosticMessageMatches @"b: string \* c: Result<'ok,string> -> string is more concrete at position 2"
        |> withDiagnosticMessageDoesntMatch "positions"
        |> ignore

    [<Fact>]
    let ``Incomparable Concreteness detail is absent under non-preview langversion`` () =
        // Pairs with Example 6: same source and harness (typecheck), only the langversion differs.
        // With MoreConcreteTiebreaker off the call is still ambiguous (FS0041) but the plain message
        // must not carry the "strictly more concrete" detail.
        FSharp incomparableConcretenessSource
        |> withLangVersion "9.0"
        |> typecheck
        |> shouldFail
        |> withErrorCode 41
        |> withDiagnosticMessageDoesntMatch "strictly more concrete"
        |> ignore

    [<Fact>]
    let ``Multiple Type Parameters - Three way comparison with clear winner`` () =
        FSharp """
module Test

type Example =
    static member Check(a: 't, b: 'u) = "both generic"
    static member Check(a: int, b: 'u) = "first concrete"
    static member Check(a: int, b: string) = "both concrete"

let result = Example.Check(42, "hello")
if result <> "both concrete" then
    failwithf "Expected 'both concrete' but got '%s' - wrong overload selected" result
        """
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Multiple Type Parameters - Tuple-like scenario`` () =
        FSharp """
module Test

type Example =
    static member Pair(fst: 't, snd: 'u) = "both generic"
    static member Pair(fst: int, snd: int) = "both int"

let result = Example.Pair(1, 2)
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Example 7 - ValueTask constructor - bare int resolves to result overload`` () =
        FSharp """
module Test

open System.Threading.Tasks

type ValueTaskFactory =
    static member Create(result: 'T) = "result"
    static member Create(task: Task<'T>) = "task"

let createFromInt () =
    let result = ValueTaskFactory.Create(42)
    result
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Example 8 - CE Source overloads - Async of plain value uses generic`` () =
        FSharp """
module Test

type SimpleBuilder() =
    member _.Source(asyncResult: Async<Result<'ok, 'error>>) = "async result"
    member _.Source(asyncValue: Async<'t>) = "async generic"

let builder = SimpleBuilder()

let result = builder.Source(async { return 42 })
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Example 9 - CE Bind with Task types - TaskBuilder pattern`` () =
        FSharp """
module Test

open System.Threading.Tasks

type TaskBuilder() =
    member _.Return(x: 'a) : Task<'a> = Task.FromResult(x)
    
    member _.Bind(taskLike: 't, continuation: 't -> Task<'b>) : Task<'b> = 
        continuation taskLike
        
    member _.Bind(task: Task<'a>, continuation: 'a -> Task<'b>) : Task<'b> = 
        task.ContinueWith(fun (t: Task<'a>) -> continuation(t.Result)).Unwrap()

let taskBuilder = TaskBuilder()

let example () =
    let task = Task.FromResult(42)
    taskBuilder.Bind(task, fun x -> Task.FromResult(x + 1))
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Example 9 - CE Bind with Task - non-task value uses generic overload`` () =
        FSharp """
module Test

open System.Threading.Tasks

type SimpleTaskBuilder() =
    member _.Bind(taskLike: 't, continuation: 't -> Task<'b>) = continuation taskLike
    member _.Bind(task: Task<'a>, continuation: 'a -> Task<'b>) = 
        task.ContinueWith(fun (t: Task<'a>) -> continuation(t.Result)).Unwrap()

let builder = SimpleTaskBuilder()

let result = builder.Bind(42, fun x -> Task.FromResult(x + 1))
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Real-world pattern - Nested task result types`` () =
        FSharp """
module Test

open System.Threading.Tasks

type AsyncBuilder() =
    member _.Bind(x: Task<Result<'a, 'e>>, f: 'a -> Task<Result<'b, 'e>>) = 
        x.ContinueWith(fun (t: Task<Result<'a, 'e>>) ->
            match t.Result with
            | Ok v -> f(v)
            | Error e -> Task.FromResult(Error e)
        ).Unwrap()
        
    member _.Bind(x: Task<'t>, f: 't -> Task<Result<'b, 'e>>) = 
        x.ContinueWith(fun (t: Task<'t>) -> f(t.Result)).Unwrap()

let ab = AsyncBuilder()

let example () =
    let taskResult : Task<Result<int, string>> = Task.FromResult(Ok 42)
    ab.Bind(taskResult, fun x -> Task.FromResult(Ok (x + 1)))
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Example 10 - Mixed Optional and Generic - existing optional rule has priority`` () =
        FSharp """
module Test

type Example =
    static member Configure(value: Option<'t>) = "generic, required"
    static member Configure(value: Option<int>, ?timeout: int) = "int, optional timeout"

let result = Example.Configure(Some 42)
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Example 10 - Mixed Optional - verify priority order does not change`` () =
        FSharp """
module Test

type Example =
    static member Process(value: Option<Option<'t>>) = "nested generic, no optional"
    static member Process(value: Option<Option<int>>, ?retries: int) = "nested int, with optional"

let result = Example.Process(Some(Some 42))
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    let bothHaveOptionalTestCases: obj[] seq =
        [
            case "Same optional types"
                 "module Test\ntype Example =\n    static member Format(value: Option<'t>, ?prefix: string) = \"generic\"\n    static member Format(value: Option<int>, ?prefix: string) = \"int\"\nlet result = Example.Format(Some 42)"

            case "Different optional types"
                 "module Test\ntype Example =\n    static member Transform(value: Option<'t>, ?prefix: string) = \"generic\"\n    static member Transform(value: Option<int>, ?timeout: int) = \"int\"\nlet result = Example.Transform(Some 42)"

            case "Multiple optional params"
                 "module Test\ntype Example =\n    static member Config(value: Option<'t>, ?prefix: string, ?suffix: string) = \"generic\"\n    static member Config(value: Option<int>, ?min: int, ?max: int) = \"int\"\nlet result = Example.Config(Some 42)"

            case "Nested generics"
                 "module Test\ntype Example =\n    static member Handle(value: Option<Option<'t>>, ?tag: string) = \"nested generic\"\n    static member Handle(value: Option<Option<int>>, ?tag: string) = \"nested int\"\nlet result = Example.Handle(Some(Some 42))"
        ]

    [<Theory>]
    [<MemberData(nameof bothHaveOptionalTestCases)>]
    let ``Both have optional params - concreteness breaks tie`` (_description: string) (source: string) =
        FSharp source
        |> typecheck
        |> shouldSucceed
        |> ignore

    let paramArrayTestCases: obj[] seq =
        [
            case "Option elements"
                 "module Test\ntype Example =\n    static member Log([<System.ParamArray>] items: Option<'t>[]) = \"generic options\"\n    static member Log([<System.ParamArray>] items: Option<int>[]) = \"int options\"\nlet result = Example.Log(Some 1, Some 2, Some 3)"

            case "Nested Option elements"
                 "module Test\ntype Example =\n    static member Combine([<System.ParamArray>] values: Option<Option<'t>>[]) = \"nested generic\"\n    static member Combine([<System.ParamArray>] values: Option<Option<int>>[]) = \"nested int\"\nlet result = Example.Combine(Some(Some 1), Some(Some 2))"

            case "Result elements"
                 "module Test\ntype Example =\n    static member Process([<System.ParamArray>] results: Result<int, 'e>[]) = \"generic error\"\n    static member Process([<System.ParamArray>] results: Result<int, string>[]) = \"string error\"\nlet r1 : Result<int, string> = Ok 1\nlet r2 : Result<int, string> = Ok 2\nlet result = Example.Process(r1, r2)"
        ]

    [<Theory>]
    [<MemberData(nameof paramArrayTestCases)>]
    let ``ParamArray with generic elements - concreteness breaks tie`` (_description: string) (source: string) =
        FSharp source
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``ParamArray vs explicit array - identical types remain ambiguous`` () =
        FSharp """
module Test

type Example =
    static member Write(messages: string[]) = "explicit array"
    static member Write([<System.ParamArray>] messages: string[]) = "param array"

let messages = [| "a"; "b"; "c" |]
let result = Example.Write(messages)
        """
        |> typecheck
        |> shouldFail
        |> withErrorCode 41 // FS0041 - ambiguous when both have same types
        |> ignore

    [<Fact>]
    let ``Combined Optional and ParamArray - complex scenario`` () =
        FSharp """
module Test

type Example =
    static member Send(target: string, [<System.ParamArray>] data: Option<'t>[]) = "generic"
    static member Send(target: string, [<System.ParamArray>] data: Option<int>[]) = "int"

let result = Example.Send("dest", Some 1, Some 2, Some 3)
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    // An intrinsic member is preferred over an extension member (the PreferNonExtension tiebreaker),
    // even when the extension is the more concrete candidate. Because the most-concrete tiebreaker
    // runs last, enabling the preview feature must not let it override that earlier choice. Observed
    // at runtime: the intrinsic (generic) overload is the one that executes.
    [<Fact>]
    let ``Intrinsic member is preferred over a more concrete extension member`` () =
        FSharp """
module Test

type Wrapper<'t>() =
    member this.Process(value: 't) = "intrinsic"

[<AutoOpen>]
module WrapperExtensions =
    type Wrapper<'t> with
        member this.Process(value: int) = "extension"

let w = Wrapper<int>()
let result = w.Process(42)
if result <> "intrinsic" then failwithf "Expected 'intrinsic' but got '%s'" result
"""
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Extension methods in same module - concreteness breaks tie`` () =
        FSharp """
module Test

type Data = { Value: int }

module DataExtensions =
    type Data with
        member this.Map(f: 'a -> 'b) = "generic map"
        member this.Map(f: int -> int) = "int map"

open DataExtensions

let d = { Value = 1 }
let result = d.Map(fun x -> x + 1)
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    let sameModuleExtensionTestCases: obj[] seq =
        [
            case "Result types"
                 "module Test\ntype Wrapper = class end\nmodule WrapperExtensions =\n    type Wrapper with\n        static member Process(value: Result<'ok, 'err>) = \"generic result\"\n        static member Process(value: Result<int, string>) = \"concrete result\"\nopen WrapperExtensions\nlet result = Wrapper.Process(Ok 42 : Result<int, string>)"

            case "Option type"
                 "module Test\ntype Processor = class end\nmodule ProcessorExtensions =\n    type Processor with\n        static member Handle(value: Option<'t>) = \"generic option\"\n        static member Handle(value: Option<int>) = \"int option\"\nopen ProcessorExtensions\nlet result = Processor.Handle(Some 42)"

            case "Nested generic"
                 "module Test\ntype Builder = class end\nmodule BuilderExtensions =\n    type Builder with\n        static member Create(value: Option<Option<'t>>) = \"nested generic\"\n        static member Create(value: Option<Option<int>>) = \"nested int\"\nopen BuilderExtensions\nlet result = Builder.Create(Some(Some 42))"
        ]

    [<Theory>]
    [<MemberData(nameof sameModuleExtensionTestCases)>]
    let ``Extension methods in same module - concreteness resolves`` (_description: string) (source: string) =
        FSharp source
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``SRTP resolution - intrinsic method preferred over extension`` () =
        FSharp """
module Test

type Processor() =
    member this.Handle(x: obj) = "intrinsic obj"

module ProcessorExtensions =
    type Processor with
        member this.HandleExt(x: int) = "extension int"

open ProcessorExtensions

let inline handle (p: ^T when ^T : (member Handle : 'a -> string)) (arg: 'a) =
    (^T : (member Handle : 'a -> string) (p, arg))

let p = Processor()

let directResult = p.Handle(42)

let srtpResult = handle p 42
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``SRTP resolution - extension-only overloads resolved by concreteness`` () =
        FSharp """
module Test

type Data = { Value: int }

module DataExtensions =
    type Data with
        member this.Format(x: 't) = sprintf "generic: %A" x
        member this.Format(x: string) = sprintf "string: %s" x

open DataExtensions

let d = { Value = 1 }

let directResult = d.Format("hello")
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``SRTP resolution - generic SRTP constraint with concrete extension`` () =
        FSharp """
module Test

type Container<'t> = { Item: 't }

module ContainerExtensions =
    type Container<'t> with
        member this.Extract() = this.Item
        member this.Extract() = 0 // Specialized for int return - but this creates ambiguity
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``C# style extension methods consumed in F# - concreteness applies`` () =
        FSharp """
module Test

type System.String with
    member this.Transform(arg: 't) = sprintf "generic %A" arg
    member this.Transform(arg: int) = sprintf "int %d" arg

let result = "hello".Transform(42)
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Extension priority - later opened module takes precedence over concreteness`` () =
        FSharp """
module Test

module GenericExtensions =
    type System.Int32 with
        member this.Describe() = "generic extension"

module ConcreteExtensions =
    type System.Int32 with
        member this.Describe() = "concrete extension"

open ConcreteExtensions
open GenericExtensions

let result = (42).Describe()
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Extension methods - incomparable concreteness remains ambiguous`` () =
        FSharp """
module Test

type Pair = class end

module PairExtensions =
    type Pair with
        static member Compare(a: Result<int, 'e>) = "int ok"
        static member Compare(a: Result<'t, string>) = "string error"

open PairExtensions

let result = Pair.Compare(Ok 42 : Result<int, string>)
        """
        |> typecheck
        |> shouldFail
        |> withErrorCode 41 // FS0041: incomparable concreteness
        |> ignore

    [<Fact>]
    let ``Adhoc rule - T is always better than inref of T`` () =
        FSharp """
module Test

type Example =
    static member Process(x: int) = "by value"
    static member Process(x: inref<int>) = "by ref"

let value = 42
let result = Example.Process(value)
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Adhoc rule priority - T over inref T takes precedence over concreteness`` () =
        FSharp """
module Test

type Example =
    static member Process<'a>(x: 'a) = "generic by value"
    static member Process(x: inref<int>) = "concrete by ref"

let value = 42
let result = Example.Process(value)
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Constrained type variable - different wrapper types with constraints allowed`` () =
        FSharp """
module Test

open System

type Example =
    static member Compare(value: 't) = "generic"
    static member Compare(value: IComparable) = "interface"

let result = Example.Compare(42)
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``TDC priority - No TDC preferred over TDC even when TDC target is more concrete`` () =
        FSharp """
module Test

type Example =
    static member Process(x: int) = "int"           // No TDC needed
    static member Process(x: int64) = "int64"       // Would need TDC: int→int64

let result = Example.Process(42)
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``TDC priority - Concreteness applies only when TDC is equal`` () =
        FSharp """
module Test

type Example =
    static member Invoke(value: Option<'t>) = "generic"
    static member Invoke(value: Option<int list>) = "concrete"

let result = Example.Invoke(Some([1]))
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``TDC priority - Combined TDC and generic resolution`` () =
        FSharp """
module Test

type Example =
    static member Handle(x: int64, y: Option<'t>) = "generic"
    static member Handle(x: int64, y: Option<string>) = "concrete"

let result = Example.Handle(42L, Some("hello"))
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``TDC priority - Nullable TDC preferred over op_Implicit TDC`` () =
        FSharp """
module Test

type Example =
    static member Method(x: System.Nullable<int>) = "nullable"    // TDC: int → Nullable<int>
    static member Method(x: int) = "direct"                       // No TDC

let result = Example.Method(42)
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Adhoc rule - Func is preferred over other delegate types`` () =
        FSharp """
module Test

open System

type CustomDelegate = delegate of int -> string

type Example =
    static member Process(f: Func<int, string>) = "func"
    static member Process(f: CustomDelegate) = "custom"

let result = Example.Process(fun x -> string x)
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Adhoc rule - Func concreteness applies when both are Func`` () =
        FSharp """
module Test

open System

type Example =
    static member Invoke(f: Func<int, string>) = "concrete func"
    static member Invoke(f: Func<'a, 'b>) = "generic func"

let result = Example.Invoke(fun x -> string x)
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Adhoc rule - Nullable concreteness applies when both are Nullable`` () =
        FSharp """
module Test

type Example =
    static member Convert(value: System.Nullable<int>) = "nullable int"
    static member Convert(value: System.Nullable<'t>) = "nullable generic"

let result = Example.Convert(System.Nullable<int>(42))
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Adhoc rule - Nullable and concreteness combined`` () =
        FSharp """
module Test

type Example =
    static member Convert(value: int) = "int"
    static member Convert(value: System.Nullable<int>) = "nullable int"
    static member Convert(value: System.Nullable<'t>) = "nullable generic"

let result1 = Example.Convert(42)

let result2 = Example.Convert(System.Nullable<int>(42))
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``SRTP - Generic SRTP vs concrete type instantiation`` () =
        FSharp """
module Test

type Handler =
    static member inline Process< ^T when ^T : (static member Parse : string -> ^T)>(s: string) : Option< ^T> =
        Some (( ^T) : (static member Parse : string -> ^T) s)
    static member inline Process(s: string) : Option<int> =
        Some(System.Int32.Parse s)

let result : Option<int> = Handler.Process("42")
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``SRTP - Inline function with concrete specialization`` () =
        FSharp """
module Test

type Converter =
    static member inline Convert< ^T when ^T : (member Value : int)>(x: ^T) = (^T : (member Value : int) x)
    static member Convert(x: System.Nullable<int>) = x.GetValueOrDefault()

let result = Converter.Convert(System.Nullable<int>(42))
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``SRTP - Member constraint with nested type arguments`` () =
        FSharp """
module Test

type Builder =
    static member inline Build< ^T when ^T : (static member Create : unit -> Option< ^T>)>() : Option< ^T> =
        (^T : (static member Create : unit -> Option< ^T>) ())
    static member Build() : Option<int> = Some 0

let result : Option<int> = Builder.Build()
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``SRTP skip - Both generic with SRTP produces ambiguity`` () =
        FSharp """
module Test

type Resolver =
    static member inline Resolve< ^T>(input: Option< ^T>) = "srtp option"
    static member inline Resolve< ^T>(input: Option< ^T list>) = "srtp option list"

let result : string = Resolver.Resolve(Some([1]))
        """
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withErrorCode 41
        |> ignore

    let concreteWrapperTestCases: obj[] seq =
        [
            case "Async<int> vs Async<'T>"
                 "module Test\ntype AsyncRunner =\n    static member Run(comp: Async<int>) = \"int async\"\n    static member Run(comp: Async<'T>) = \"generic async\"\nlet computation = async { return 42 }\nlet result = AsyncRunner.Run(computation)"

            case "Async<Result<int, exn>> vs Async<Result<'T, exn>>"
                 "module Test\ntype AsyncHandler =\n    static member Handle(comp: Async<Result<int, exn>>) = \"int result async\"\n    static member Handle(comp: Async<Result<'T, exn>>) = \"generic result async\"\nlet computation : Async<Result<int, exn>> = async { return Ok 42 }\nlet result = AsyncHandler.Handle(computation)"

            case "MailboxProcessor<int> vs MailboxProcessor<'T>"
                 "module Test\ntype Message = Start | Stop\ntype Dispatcher =\n    static member Dispatch(mb: MailboxProcessor<int>) = \"int mailbox\"\n    static member Dispatch(mb: MailboxProcessor<'T>) = \"generic mailbox\"\nlet mb = MailboxProcessor.Start(fun inbox -> async { return () })\nlet result = Dispatcher.Dispatch(mb)"

            case "Lazy<int list> vs Lazy<'T>"
                 "module Test\ntype LazyLoader =\n    static member Load(value: Lazy<int list>) = \"int list lazy\"\n    static member Load(value: Lazy<'T>) = \"generic lazy\"\nlet lazyValue = lazy [1; 2; 3]\nlet result = LazyLoader.Load(lazyValue)"

            case "Choice<int, string> vs Choice<'T1, 'T2>"
                 "module Test\ntype Router =\n    static member Route(choice: Choice<int, string>) = \"int or string\"\n    static member Route(choice: Choice<'T1, 'T2>) = \"generic choice\"\nlet c = Choice1Of2 42\nlet result = Router.Route(c)"

            case "ValueOption<int> vs ValueOption<'T>"
                 "module Test\ntype ValueProcessor =\n    static member Process(v: ValueOption<int>) = \"voption int\"\n    static member Process(v: ValueOption<'T>) = \"voption generic\"\nlet vopt = ValueSome 42\nlet result = ValueProcessor.Process(vopt)"

            case "seq<int> vs seq<'T>"
                 "module Test\ntype SeqHandler =\n    static member Handle(s: seq<int>) = \"int seq\"\n    static member Handle(s: seq<'T>) = \"generic seq\"\nlet numbers = seq { 1; 2; 3 }\nlet result = SeqHandler.Handle(numbers)"

            case "Option<int> list vs Option<'T> list"
                 "module Test\ntype ListHandler =\n    static member Handle(lst: Option<int> list) = \"option int list\"\n    static member Handle(lst: Option<'T> list) = \"option generic list\"\nlet items = [ Some 1; Some 2; None ]\nlet result = ListHandler.Handle(items)"

            case "Async<int * string> vs Async<'T>"
                 "module Test\ntype AsyncBuilder =\n    static member Wrap(comp: Async<int * string>) = \"tuple async\"\n    static member Wrap(comp: Async<'T>) = \"generic async\"\nlet work = async { return (42, \"hello\") }\nlet result = AsyncBuilder.Wrap(work)"

            case "Result<int, string> vs Result<int, 'E>"
                 "module Test\ntype ErrorHandler =\n    static member Handle(r: Result<int, string>) = \"int result string error\"\n    static member Handle(r: Result<int, 'E>) = \"int result generic error\"\nlet ok : Result<int, string> = Ok 42\nlet result = ErrorHandler.Handle(ok)"

            case "Tree<int> vs Tree<'T>"
                 "module Test\ntype Tree<'T> =\n    | Leaf of 'T\n    | Node of Tree<'T> * Tree<'T>\ntype TreeProcessor =\n    static member Process(t: Tree<int>) = \"int tree\"\n    static member Process(t: Tree<'T>) = \"generic tree\"\nlet tree = Node(Leaf 1, Leaf 2)\nlet result = TreeProcessor.Process(tree)"

            case "inref<Result<int, exn>> vs inref<Result<'T, exn>>"
                 "module Test\ntype RefProcessor =\n    static member Transform(ref: inref<Result<'T, exn>>) = \"generic result\"\n    static member Transform(ref: inref<Result<int, exn>>) = \"int result\"\nlet runTest () =\n    let mutable result: Result<int, exn> = Ok 42\n    RefProcessor.Transform(&result)"

            case "outref<int> vs outref<'T>"
                 "module Test\ntype Writer =\n    static member Write(dest: outref<int>, value: int) = dest <- value\n    static member Write(dest: outref<'T>, value: 'T) = dest <- value\nlet mutable x = 0\nWriter.Write(&x, 42)"

            case "inref<int>/outref<int> vs inref<'T>/outref<'T>"
                 "module Test\ntype Transformer =\n    static member Transform(src: inref<int>, dest: outref<int>) = dest <- src\n    static member Transform(src: inref<'T>, dest: outref<'T>) = dest <- src\nlet mutable value = 42\nlet mutable result = 0\nTransformer.Transform(&value, &result)"

            case "byref<Option<int>> vs byref<Option<'T>>"
                 "module Test\ntype RefProcessor =\n    static member Process(r: byref<Option<int>>) = r <- Some 42\n    static member Process(r: byref<Option<'T>>) = r <- None\nlet mutable opt : Option<int> = None\nRefProcessor.Process(&opt)"

            case "nativeptr<int> vs nativeptr<'T>"
                 "module Test\nopen Microsoft.FSharp.NativeInterop\ntype PtrHandler =\n    static member Handle(p: nativeptr<int>) = 1\n    static member Handle(p: nativeptr<'T>) = 2\nlet inline handlePtr (p: nativeptr<int>) = PtrHandler.Handle(p)"

            case "{| Value: int |} vs {| Value: 'T |}"
                 "module Test\ntype Processor =\n    static member Process(r: {| Value: int |}) = \"int\"\n    static member Process(r: {| Value: 'T |}) = \"generic\"\nlet result = Processor.Process({| Value = 42 |})"

            case "nested {| Inner: {| X: int |} |} vs {| Inner: {| X: 'T |} |}"
                 "module Test\ntype Handler =\n    static member Handle(r: {| Inner: {| X: int |} |}) = \"concrete\"\n    static member Handle(r: {| Inner: {| X: 'T |} |}) = \"generic\"\nlet result = Handler.Handle({| Inner = {| X = 42 |} |})"

            case "Option<{| Id: int; Name: string |}> vs Option<{| Id: 'T; Name: string |}>"
                 "module Test\ntype Builder =\n    static member Build(x: Option<{| Id: int; Name: string |}>) = \"concrete\"\n    static member Build(x: Option<{| Id: 'T; Name: string |}>) = \"generic id\"\nlet result = Builder.Build(Some {| Id = 1; Name = \"test\" |})"

            case "float<m> vs float<'u>"
                 "module Test\n[<Measure>] type m\n[<Measure>] type s\ntype Calculator =\n    static member Calculate(x: float<m>) = \"meters\"\n    static member Calculate(x: float<'u>) = \"generic unit\"\nlet distance : float<m> = 5.0<m>\nlet result = Calculator.Calculate(distance)"

            case "float<m/s> vs float<'u>"
                 "module Test\n[<Measure>] type m\n[<Measure>] type s\ntype Physics =\n    static member Velocity(x: float<m/s>) = \"velocity\"\n    static member Velocity(x: float<'u>) = \"generic\"\nlet speed : float<m/s> = 10.0<m/s>\nlet result = Physics.Velocity(speed)"

            case "Option<float<kg>> vs Option<float<'u>>"
                 "module Test\n[<Measure>] type kg\ntype Scale =\n    static member Weigh(x: Option<float<kg>>) = \"kg\"\n    static member Weigh(x: Option<float<'u>>) = \"generic\"\nlet result = Scale.Weigh(Some 75.0<kg>)"

            case "float<Hz>[] vs float<'u>[]"
                 "module Test\n[<Measure>] type Hz\ntype SignalProcessor =\n    static member Process(samples: float<Hz>[]) = \"Hz array\"\n    static member Process(samples: float<'u>[]) = \"generic array\"\nlet frequencies : float<Hz>[] = [| 440.0<Hz>; 880.0<Hz> |]\nlet result = SignalProcessor.Process(frequencies)"
        ]

    let concreteWrapperNetCoreTestCases: obj[] seq =
        [
            case "Span<byte> vs Span<'T>"
                 "module Test\nopen System\ntype Parser =\n    static member Parse(data: Span<'T>) = \"generic\"\n    static member Parse(data: Span<byte>) = \"bytes\"\nlet runTest () =\n    let buffer: byte[] = [| 1uy; 2uy; 3uy |]\n    let span = Span(buffer)\n    Parser.Parse(span)"

            case "ReadOnlySpan<byte> vs ReadOnlySpan<'T>"
                 "module Test\nopen System\ntype Parser =\n    static member Parse(data: ReadOnlySpan<'T>) = \"generic\"\n    static member Parse(data: ReadOnlySpan<byte>) = \"bytes\"\nlet runTest () =\n    let bytes: byte[] = [| 1uy; 2uy; 3uy |]\n    let roSpan = ReadOnlySpan(bytes)\n    Parser.Parse(roSpan)"

            case "Span<Option<int>> vs Span<Option<'T>>"
                 "module Test\nopen System\ntype DataHandler =\n    static member Handle(data: Span<Option<'T>>) = \"generic option\"\n    static member Handle(data: Span<Option<int>>) = \"int option\"\nlet runTest () =\n    let options: Option<int>[] = [| Some 1; Some 2 |]\n    let span = Span(options)\n    DataHandler.Handle(span)"

            case "ValueTask<int> vs ValueTask<'T>"
                 "module Test\nopen System.Threading.Tasks\ntype TaskRunner =\n    static member Run(t: ValueTask<int>) = \"int valuetask\"\n    static member Run(t: ValueTask<'T>) = \"generic valuetask\"\nlet vt = ValueTask<int>(42)\nlet result = TaskRunner.Run(vt)"
        ]

    [<Theory>]
    [<MemberData(nameof concreteWrapperTestCases)>]
    let ``Concrete wrapper type resolves over generic`` (_description: string) (source: string) =
        FSharp source
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<TheoryForNETCOREAPP>]
    [<MemberData(nameof concreteWrapperNetCoreTestCases)>]
    let ``Concrete wrapper type resolves over generic (NETCOREAPP)`` (_description: string) (source: string) =
        FSharp source
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``SRTP - member constraint with overloaded static member`` () =
        FSharp """
module Test

type Converter =
    static member Convert<'t>(x: 't) = box x
    static member Convert(x: int) = box (x * 2)

let result = Converter.Convert 21
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``SRTP - inline function calling overloaded method`` () =
        FSharp """
module Test

type Handler =
    static member Handle<'t>(x: 't) = x
    static member Handle(x: int) = x * 2

let inline handle x = Handler.Handle x

let result : int = handle 21
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``SRTP - layered inline with deferred overload resolution`` () =
        FSharp """
module Test

type Processor =
    static member Process<'t>(x: Option<'t>) = x
    static member Process(x: Option<int>) = x |> Option.map ((*) 2)

let inline layer3 x = Processor.Process(Some x)
let inline layer2 x = layer3 x
let inline layer1 x = layer2 x

let result = layer1 42
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``SRTP - explicit member constraint with Parse`` () =
        FSharp """
module Test

type MyParser =
    static member Parse(s: string) = 42
    static member Parse<'t>(s: string) = Unchecked.defaultof<'t>

let inline parse< ^T when ^T : (static member Parse : string -> ^T)> (s: string) : ^T =
    (^T : (static member Parse : string -> ^T) s)

let result : int = parse "42"
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``SRTP - witness passing with explicit type`` () =
        FSharp """
module Test

type IMonoid<'T> =
    abstract Zero : 'T
    abstract Plus : 'T -> 'T -> 'T

type IntMonoid() =
    interface IMonoid<int> with
        member _.Zero = 0
        member _.Plus a b = a + b

type Folder =
    static member Fold<'t>(xs: 't list, m: IMonoid<'t>) = 
        List.fold (fun acc x -> m.Plus acc x) m.Zero xs
    static member Fold(xs: int list, m: IMonoid<int>) = 
        List.fold (fun acc x -> m.Plus acc x) m.Zero xs

let sum = Folder.Fold([1;2;3], IntMonoid() :> IMonoid<int>)
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``SRTP - nested generic in inline with concrete specialization`` () =
        FSharp """
module Test

type Wrapper =
    static member Wrap<'t>(x: Option<'t>) = Some x
    static member Wrap(x: Option<int>) = Some (x |> Option.map ((*) 2))

let inline wrap x = Wrapper.Wrap(Some x)
let inline wrapTwice x = wrap x |> Option.bind id

let result = wrapTwice 21
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``LangVersion Latest - Non-generic overload preferred over generic - existing behavior`` () =
        FSharp """
module Test

type Example =
    static member Process(value: 't) = "generic"
    static member Process(value: int) = "int"

let result = Example.Process(42)
        """
        |> withLangVersion "latest"
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``LangVersion Latest - Non-extension method preferred over extension - existing behavior`` () =
        FSharp """
module Test

type MyType() =
    member this.Invoke(x: int) = "instance"

module Extensions =
    type MyType with
        member this.Invoke(x: obj) = "extension"

open Extensions

let t = MyType()
let result = t.Invoke(42)
        """
        |> withLangVersion "latest"
        |> typecheck
        |> shouldSucceed
        |> ignore

    let orpIgnoredTestCases: obj[] seq =
        [
            [| "higher priority does not win"; "BasicPriority.Invoke(\"test\")"; "priority-1-string" |]
            [| "negative priority has no effect"; "NegativePriority.Legacy(\"test\")"; "current" |]
            [| "priority does not override concreteness"; "PriorityVsConcreteness.Process(42)"; "int-low-priority" |]
        ]

    [<TheoryForNETCOREAPP>]
    [<MemberData(nameof orpIgnoredTestCases)>]
    let ``LangVersion Latest - ORP attribute ignored`` (_description: string) (callExpr: string) (expected: string) =
        FSharp $"""
module Test
open PriorityTests

let result = {callExpr}
if result <> "{expected}" then
    failwithf "Expected '{expected}' but got '%%s' - ORP should be ignored" result
        """
        |> withReferences [csharpPriorityLib]
        |> withLangVersion "latest"
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    // Constructors and generic-type members whose instantiation is inferred from the arguments carry
    // no method type args, so the rule ranks them via their enclosing-type type parameters. The
    // "picks the concrete overload" behaviour across member kinds is covered by the parametrized
    // matrix below; the standalone facts here assert the distinct guard behaviours (single-applicable,
    // incomparable, and that a later shipped rule still decides successful resolutions).

    [<Fact>]
    let ``MoreConcrete - explicit type arg leaves a single applicable ctor`` () =
        // 'T pinned = int; only new(x:int) applies -> no tiebreak needed.
        FSharp """
module Test

type Wrapper<'T>(tag: string) =
    new(x: 'T)        = Wrapper<'T>("value")
    new(x: 'T option) = Wrapper<'T>("option")
    member _.Tag = tag

let w = Wrapper<int>(5)
if w.Tag <> "value" then failwithf "expected value, got %s" w.Tag
        """
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``MoreConcrete - constructor with incomparable concreteness stays ambiguous`` () =
        // c1 more concrete at pos1, c2 more concrete at pos2 -> incomparable -> must remain FS0041.
        FSharp """
module Test

type Pair<'A, 'B>(tag: string) =
    new(x: 'A option, y: 'B)        = Pair<'A, 'B>("first")
    new(x: 'A, y: 'B option)        = Pair<'A, 'B>("second")
    member _.Tag = tag

let p = Pair(Some 1, Some 2)
        """
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCode 41
        |> ignore

    [<Fact>]
    let ``MoreConcrete - a later shipped rule still decides a successful resolution (static)`` () =
        // Safety property: the most-concrete tiebreak is a last resort. It must not preempt a
        // resolution that a rule enabled at default langversion already settles. Here the named
        // arg z (string vs obj) makes the nullable/optional-interop rule decisive at default; the
        // preview feature must select the SAME overload, not silently flip to the concrete one.
        FSharp """
module Test
type Box<'T>() =
    static member Make(x: 'T,        z: string) = "A_naked"
    static member Make(x: 'T option, z: obj)    = "B_concrete"
let r = Box.Make(Some 5, z = "k")
if r <> "A_naked" then failwithf "expected A_naked, got %s" r
        """
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``MoreConcrete - a later shipped rule still decides a successful resolution (ctor)`` () =
        // Same safety property for constructors (empty method type args): the named arg z makes an
        // earlier-at-default rule decisive; preview must run the same constructor body.
        FSharp """
module Test
type W<'T> =
    val tag: string
    new (x: 'T,        z: string) = { tag = "A_naked:" + z }
    new (x: 'T option, z: obj)    = { tag = "B_concrete:" + string z }
let w = W(Some 5, z = "hi")
if w.tag <> "A_naked:hi" then failwithf "expected A_naked:hi, got %s" w.tag
        """
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``MoreConcrete - overloads differing only by SRTP concreteness stay ambiguous`` () =
        // The tiebreak short-circuits on statically-resolved type parameters (they resolve via a
        // different mechanism), so an SRTP-only concreteness difference must remain FS0041.
        FSharp """
module Test
type F() =
    static member inline M(x: ^T)        = "g"
    static member inline M(x: ^T option) = "c"
let r = F.M(Some 5)
        """
        |> withLangVersionPreview
        |> compile
        |> shouldFail
        |> withErrorCode 41
        |> ignore

/// The most-concrete tiebreaker turns a previously ambiguous call into a successful
/// resolution. Each test here has an exact mirror in WithoutTieBreakerFeature that pins the
/// same source to a feature-off language version and asserts FS0041 instead.
module WithTieBreakerFeature =

    open TiebreakerFixtures

    // Both-generic overloads that differ only by concreteness: the tiebreaker resolves them.
    let moreConcreteFlipCases = moreConcreteTestCases

    [<Theory>]
    [<MemberData(nameof moreConcreteFlipCases)>]
    let ``Both-generic overloads resolve to the more concrete one`` (_description: string) (source: string) =
        FSharp source
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    // Generic vs wrapped-generic (e.g. 't vs Option<'t>, 't vs 't array): concreteness resolves them.
    let disabledFlipCases = moreConcretDisabledAmbiguousCases

    [<Theory>]
    [<MemberData(nameof disabledFlipCases)>]
    let ``Generic-vs-wrapped overloads resolve to the concrete one`` (_description: string) (source: string) =
        FSharp source
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    // Member-kind matrix: ctor / extension / static / instance / optional-tail / paramarray-tail.
    let edgeCaseFlipCases = mostConcreteEdgeCases

    [<Theory>]
    [<MemberData(nameof edgeCaseFlipCases)>]
    let ``Edge-case matrix picks the concrete overload`` (_name: string) (source: string) =
        FSharp source
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<Theory>]
    [<InlineData("Process", "Result<int, 'error>", "int ok", "Ok 42 : Result<int, exn>")>]
    [<InlineData("Handle", "Result<'ok, string>", "string error", "Ok \"test\" : Result<string, string>")>]
    let ``Partial concreteness resolves`` (methodName: string) (concreteParam: string) (concreteDesc: string) (callExpr: string) =
        FSharp(example5PartialSource methodName concreteParam concreteDesc callExpr)
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Task<T> vs T factory resolves to the concrete Task overload`` () =
        FSharp example7Source |> withLangVersionPreview |> asExe |> compileAndRun |> shouldSucceed |> ignore

    [<Fact>]
    let ``CE Source overloads resolve (FsToolkit AsyncResult pattern)`` () =
        FSharp example8Source |> withLangVersionPreview |> asExe |> compileAndRun |> shouldSucceed |> ignore

    [<Fact>]
    let ``Builder Source with Result vs generic resolves`` () =
        FSharp realWorldSource |> withLangVersionPreview |> asExe |> compileAndRun |> shouldSucceed |> ignore

    [<Fact>]
    let ``Same-module extension Source overloads resolve by concreteness`` () =
        FSharp fsToolkitSource |> withLangVersionPreview |> asExe |> compileAndRun |> shouldSucceed |> ignore

    [<FactForNETCOREAPP>]
    let ``Overload resolution priority still wins over concreteness`` () =
        FSharp orpWinsSource |> withLangVersionPreview |> asExe |> compileAndRun |> shouldSucceed |> ignore

    // The tiebreaker emits advisory diagnostics (FS3575 selected / FS3576 bypassed) when opted into.

    [<Fact>]
    let ``Warning 3575 - Not emitted by default when concreteness tiebreaker used`` () =
        FSharp concretenessWarningSource
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Warning 3575 - Emitted when enabled and concreteness tiebreaker is used`` () =
        FSharp concretenessWarningSource
        |> withLangVersionPreview
        |> withOptions ["--warnon:3575"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 3575
        |> withDiagnosticMessageMatches "concreteness"
        // FS3575 names the two distinct signatures (concrete winner, generic loser), not "Invoke"/"Invoke".
        |> withDiagnosticMessageMatches "Option<'t list>"
        |> withDiagnosticMessageMatches "Invoke: value: Option<'t> ->"
        |> ignore

    [<Fact>]
    let ``Warning 3576 - Emitted when enabled and generic overload is bypassed`` () =
        FSharp concretenessWarningSource
        |> withLangVersionPreview
        |> withOptions ["--warnon:3576"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 3576
        |> withDiagnosticMessageMatches "bypassed"
        // FS3576 names the two distinct signatures (generic loser, concrete winner), not "Invoke"/"Invoke".
        |> withDiagnosticMessageMatches "Option<'t list>"
        |> withDiagnosticMessageMatches "Invoke: value: Option<'t> ->"
        |> ignore

    [<Fact>]
    let ``Warning 3576 - Multiple bypassed overloads`` () =
        FSharp multipleBypassedSource
        |> withLangVersionPreview
        |> withOptions ["--warnon:3576"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 3576
        // "Multiple" must name BOTH bypassed generic overloads, not just report a single warning.
        |> withDiagnosticMessageMatches "Process: value: 't ->"
        |> withDiagnosticMessageMatches "Process: value: Option<'t> ->"
        |> ignore

/// Exact mirror of WithTieBreakerFeature pinned to langversion 10.0 (feature off): every source
/// that resolves under preview instead stays ambiguous (FS0041) without the tiebreaker.
module WithoutTieBreakerFeature =

    open TiebreakerFixtures

    // Both-generic overloads that differ only by concreteness: ambiguous without the tiebreaker.
    let moreConcreteFlipCases = moreConcreteTestCases

    [<Theory>]
    [<MemberData(nameof moreConcreteFlipCases)>]
    let ``Both-generic overloads stay ambiguous`` (_description: string) (source: string) =
        FSharp source
        |> withLangVersion "10.0"
        |> compile
        |> shouldFail
        |> withErrorCode 41
        |> ignore

    let disabledFlipCases = moreConcretDisabledAmbiguousCases

    [<Theory>]
    [<MemberData(nameof disabledFlipCases)>]
    let ``Generic-vs-wrapped overloads stay ambiguous`` (_description: string) (source: string) =
        FSharp source
        |> withLangVersion "10.0"
        |> typecheck
        |> shouldFail
        |> withErrorCode 41
        |> ignore

    let edgeCaseFlipCases = mostConcreteEdgeCases

    [<Theory>]
    [<MemberData(nameof edgeCaseFlipCases)>]
    let ``Edge-case matrix stays ambiguous`` (_name: string) (source: string) =
        FSharp source
        |> withLangVersion "10.0"
        |> compile
        |> shouldFail
        |> withErrorCode 41
        |> ignore

    [<Theory>]
    [<InlineData("Process", "Result<int, 'error>", "int ok", "Ok 42 : Result<int, exn>")>]
    [<InlineData("Handle", "Result<'ok, string>", "string error", "Ok \"test\" : Result<string, string>")>]
    let ``Partial concreteness stays ambiguous`` (methodName: string) (concreteParam: string) (concreteDesc: string) (callExpr: string) =
        FSharp(example5PartialSource methodName concreteParam concreteDesc callExpr)
        |> withLangVersion "10.0"
        |> compile
        |> shouldFail
        |> withErrorCode 41
        |> ignore

    [<Fact>]
    let ``Task<T> vs T factory stays ambiguous`` () =
        FSharp example7Source |> withLangVersion "10.0" |> typecheck |> shouldFail |> withErrorCode 41 |> ignore

    [<Fact>]
    let ``CE Source overloads stay ambiguous`` () =
        FSharp example8Source |> withLangVersion "10.0" |> typecheck |> shouldFail |> withErrorCode 41 |> ignore

    [<Fact>]
    let ``Builder Source with Result vs generic stays ambiguous`` () =
        FSharp realWorldSource |> withLangVersion "10.0" |> typecheck |> shouldFail |> withErrorCode 41 |> ignore

    [<Fact>]
    let ``Same-module extension Source overloads stay ambiguous`` () =
        FSharp fsToolkitSource |> withLangVersion "10.0" |> typecheck |> shouldFail |> withErrorCode 41 |> ignore

    [<FactForNETCOREAPP>]
    let ``Overload resolution priority disabled leaves the call ambiguous`` () =
        FSharp orpWinsSource |> withLangVersion "10.0" |> compile |> shouldFail |> withErrorCode 41 |> ignore

    [<Fact>]
    let ``Advisory warning source stays ambiguous`` () =
        FSharp concretenessWarningSource |> withLangVersion "10.0" |> typecheck |> shouldFail |> withErrorCode 41 |> ignore

    [<Fact>]
    let ``Multiple bypassed source stays ambiguous`` () =
        FSharp multipleBypassedSource |> withLangVersion "10.0" |> typecheck |> shouldFail |> withErrorCode 41 |> ignore
