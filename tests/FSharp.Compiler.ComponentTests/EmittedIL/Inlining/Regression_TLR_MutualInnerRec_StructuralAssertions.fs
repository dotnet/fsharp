namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Regression_TLR_MutualInnerRec_StructuralAssertions =

    let internal compileOptimized realsig source =
        FSharp source
        |> withRealInternalSignature realsig
        |> asExe
        |> withOptimize
        |> ignoreWarnings

    let private compileOptimizedAndRun realsig source =
        source |> compileOptimized realsig |> compileExeAndRun |> shouldSucceed |> ignore

    /// Shared "PEVerify + run" tail used by tests that assert both metadata validity and runtime success.
    let internal verifyPEAndRun (compiled: CompilationResult) =
        compiled |> verifyPEFileWithSystemDlls |> shouldSucceed |> ignore
        compiled |> run |> shouldSucceed |> ignore

    let private compileAndAssertNoClosures realsig expectedIL source =
        let result = source |> compileOptimized realsig |> compile |> shouldSucceed
        result |> verifyILPresent expectedIL
        result |> verifyILNotPresent [ "extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc" ]
        result

    let private compileAssertNoClosuresAndRun realsig expectedIL source =
        source |> compileAndAssertNoClosures realsig expectedIL |> run |> shouldSucceed |> ignore

    // Inline constrained call inside a memoize closure — the inline expands into the closure body,
    // attaching constraints to the closure class typars; EraseClosures must strip them.
    let internal closureWithConstraint constraintClause inlineBody typeAnnotation callValue =
        $"""module Test
open System
let inline worker<'a when {constraintClause}> (a: 'a) (b: 'a) : bool = {inlineBody}
let inline tee f x = f x; x
let memoize (f: 'a -> 'b) =
    let cell = ref None
    let f' (x: 'a) =
        match cell.Value with
        | Some (x', value) when worker x' x -> value
        | _ -> f x |> tee (fun y -> cell.Value <- Some (x, y))
    f'
[<EntryPoint>]
let main _argv =
    let f: {typeAnnotation} -> {typeAnnotation} = memoize id
    if f {callValue} = {callValue} then 0 else 1
"""

    // --- TLR tests (#17607) ---

    [<Theory; InlineData(true); InlineData(false)>]
    let ``Single inner let rec fires TLR`` (realsig: bool) =
        """module Sample
let wrapper() =
    let rec countdown(n) =
        if n = 0 then 100
        else countdown(n - 1)
    countdown(1000000)
[<EntryPoint>]
let main _argv = wrapper()
"""
        |> compileAndAssertNoClosures realsig [ "static int32  countdown@" ]
        |> ignore

    [<Theory; InlineData(true); InlineData(false)>]
    let ``TLR inside generic class member`` (realsig: bool) =
        """module Sample
type Container<'T>(initial: 'T) =
    member _.Run() =
        let rec a(n, v: 'T) =
            if n = 0 then v
            elif n % 2 = 0 then b(n - 1, v)
            else a(n - 1, v)
        and b(n, v: 'T) =
            if n = 0 then v
            elif n % 2 = 0 then b(n - 1, v)
            else a(n - 1, v)
        a(100, initial)
[<EntryPoint>]
let main _argv =
    if Container(42).Run() = 42 then 0 else 1
"""
        |> compileAssertNoClosuresAndRun realsig [ "static !!T  a@"; "static !!T  b@" ]

    [<Theory; InlineData(true); InlineData(false)>]
    let ``TLR with generic method on generic type`` (realsig: bool) =
        """module Sample
type Processor<'T when 'T : struct>(value: 'T) =
    member _.Transform<'U when 'U : equality>(input: 'U, expected: 'U) =
        let rec apply(n, acc: 'U) =
            if n <= 0 then acc = expected
            else step(n, acc)
        and step(n, acc: 'U) =
            apply(n - 1, acc)
        apply(100, input)
[<EntryPoint>]
let main _argv =
    if Processor(1).Transform("hello", "hello") then 0 else 1
"""
        |> compileAssertNoClosuresAndRun realsig [ "static bool  apply@"; "static bool  step@"; "!!U" ]

    [<Theory; InlineData(true); InlineData(false)>]
    let ``Three-way mutual recursion`` (realsig: bool) =
        """module Sample
let run() =
    let rec a(n) =
        if n = 0 then 100
        elif n % 3 = 0 then b(n - 1)
        elif n % 3 = 1 then c(n - 1)
        else a(n - 1)
    and b(n) =
        if n = 0 then 101 else a(n - 1)
    and c(n) =
        if n = 0 then 102 else b(n - 1)
    a(1000)
[<EntryPoint>]
let main _argv = run()
"""
        |> compileAssertNoClosuresAndRun realsig [ "static int32  a@"; "static int32  b@"; "static int32  c@" ]

    [<Theory; InlineData(true); InlineData(false)>]
    let ``TLR in nested module`` (realsig: bool) =
        """module Outer
module Inner =
    let run() =
        let rec a(n) =
            if n = 0 then 100
            elif n % 2 = 0 then b(n - 1)
            else a(n - 1)
        and b(n) =
            if n = 0 then 101 else a(n - 1)
        a(1000)
[<EntryPoint>]
let main _argv = Inner.run()
"""
        |> compileAssertNoClosuresAndRun realsig [ "static int32  a@"; "static int32  b@" ]

    // Regression for FS2014 duplicate generated method names across namespace-only files (see IlxGen.AllocValReprWithinExpr).
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Namespace-level inner-rec in multiple files does not collide`` (realsig: bool) =
        let src1 = """namespace Mine
type A() =
    static member Run() =
        let rec capture x = if x = 0 then 0 else capture (x - 1)
        capture 10

type B() =
    static member Run() =
        let rec capture y = if y = 0 then 1 else capture (y - 1)
        capture 20
"""
        let src2 = """namespace Mine
type C() =
    static member Run() =
        let rec capture z = if z = 0 then 2 else capture (z - 1)
        capture 30
"""
        FSharp src1
        |> withAdditionalSourceFile (SourceCodeFileKind.Create("B.fs", src2))
        |> withRealInternalSignature realsig
        |> asLibrary
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> verifyILNotPresent [ "PrivateImplementationDetails" ]

    [<Theory; InlineData(true); InlineData(false)>]
    let ``Value recursion is not broken by TLR`` (realsig: bool) =
        """module Sample
let run() =
    let rec values = [| 1; 2; 3 |]
    and sum() = Array.sum values
    sum()
[<EntryPoint>]
let main _argv = if run() = 6 then 0 else 1
"""
        |> compileOptimizedAndRun realsig

    [<Theory; InlineData(true); InlineData(false)>]
    let ``Quotation body is not affected by TLR`` (realsig: bool) =
        """module Sample
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
let q : Expr<int> =
    <@
        let rec a(n) =
            if n = 0 then 1
            elif n % 2 = 0 then b(n - 1)
            else a(n - 1)
        and b(n) =
            if n = 0 then 2
            elif n % 2 = 0 then b(n - 1)
            else a(n - 1)
        a(100)
    @>
[<EntryPoint>]
let main _argv =
    match q.Raw with LetRecursive _ -> 0 | _ -> 1
"""
        |> compileOptimizedAndRun realsig

    // --- Constraint stripping tests (#14492) ---

    [<Theory; InlineData(true); InlineData(false)>]
    let ``Issue 14492: Specialize override and T-suffix class have no constraints in IL`` (realsig: bool) =
        let result =
            closureWithConstraint "'a : not struct and 'a : equality" "obj.ReferenceEquals(a, b)" "string" "\"ok\""
            |> compileOptimized realsig
            |> compile
            |> shouldSucceed

        result |> verifyILPresent [ "object Specialize<" ]
        result |> verifyILNotPresent [ "Specialize<class "; "Specialize<valuetype " ]
        result |> verifyILNotPresent [ "T<class "; "T<valuetype " ]

    // >5 curried params forces EraseClosures CASE 2a term-splitting, producing a D-suffixed nested type.
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Issue 14492: >5 params chain produces D-suffix nested type (CASE 2a)`` (realsig: bool) =
        let source =
            """module Sample
open System
let inline worker<'a when 'a : not struct> (a: 'a) (b: 'a) : bool = obj.ReferenceEquals(a, b)
let inline tee f x = f x; x
let memoize (f: 'a -> 'b) =
    let cell = ref None
    let f' (x: 'a) (a2: 'a) (a3: 'a) (a4: 'a) (a5: 'a) (a6: 'a) =
        match cell.Value with
        | Some (k, v) when worker k x && worker a2 a3 -> v
        | _ -> f (if worker a4 a5 then x else a6) |> tee (fun y -> cell.Value <- Some (x, y))
    f'
[<EntryPoint>]
let main _argv =
    let g: string -> string -> string -> string -> string -> string -> string = memoize id
    if g "ok" "ok" "ok" "ok" "ok" "ok" = "ok" then 0 else 1
"""

        let result = source |> compileOptimized realsig |> compile |> shouldSucceed

        result |> verifyILPresent [ "memoize@8D<" ]
        verifyPEAndRun result

    // --- Combined test (exercises both #17607 and #14492 together) ---

    // TLR inside generic class (#17607) + constrained inline inside closure (#14492).
    [<Theory; InlineData(true); InlineData(false)>]
    let ``Combined: TLR in generic class with constrained inline closure`` (realsig: bool) =
        let result =
            """module Sample
open System
let inline worker<'a when 'a : not struct> (a: 'a) (b: 'a) : bool = obj.ReferenceEquals(a, b)
type Cache<'T when 'T : not struct and 'T : equality>(initial: 'T) =
    member _.Lookup(key: 'T) =
        let memoize (f: 'a -> 'b) =
            let cell = ref None
            let f' (x: 'a) =
                match cell.Value with
                | Some (k, v) when worker k x -> v
                | _ -> let v = f x in cell.Value <- Some (x, v); v
            f'
        let rec search n =
            if n <= 0 then false
            elif (memoize id) key = key then true
            else check (n - 1)
        and check n = search (n - 1)
        search 10
[<EntryPoint>]
let main _argv =
    if Cache("test").Lookup("test") then 0 else 1
"""
            |> compileOptimized realsig |> compile |> shouldSucceed

        result |> verifyILPresent [ "static bool  search@" ]
        result |> verifyILNotPresent [ "Specialize<class "; "Specialize<valuetype " ]
        verifyPEAndRun result
