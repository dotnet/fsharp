// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module ``ComputationExpressions`` =
    let tmp = 1

    let applicativeLib = """
/// Used for tracking what operations a Trace builder was asked to perform
[<RequireQualifiedAccess>]
type TraceOp =
    | ApplicativeBind
    | ApplicativeBind2
    | ApplicativeReturn
    | ApplicativeCombine
    | ApplicativeYield
    | MergeSources of int
    | MonadicBind
    | MonadicBind2
    | MonadicReturn
    | Run
    | Delay

/// A pseudo identity functor
type Trace<'T>(v: 'T) =
    member x.Value = v
    override this.ToString () =
        sprintf "%+A" v

/// A builder which records what operations it is asked to perform
type TraceCore() =

    let mutable trace = ResizeArray<_>()

    member builder.GetTrace () = trace.ToArray()

    member builder.Trace x = trace.Add(x)

type TraceMergeSourcesCore() =
    inherit TraceCore()

    member builder.MergeSources(x1: Trace<'T1>, x2: Trace<'T2>) : Trace<'T1 * 'T2> =
        builder.Trace (TraceOp.MergeSources 2)
        Trace (x1.Value, x2.Value) 

    member builder.MergeSources3(x1: Trace<'T1>, x2: Trace<'T2>, x3: Trace<'T3>) : Trace<struct ('T1 * 'T2 * 'T3)> =
        builder.Trace (TraceOp.MergeSources 3)
        Trace (struct (x1.Value, x2.Value, x3.Value))

    member builder.MergeSources4(x1: Trace<'T1>, x2: Trace<'T2>, x3: Trace<'T3>, x4: Trace<'T4>) : Trace<'T1 * 'T2 * 'T3 * 'T4> =
        builder.Trace (TraceOp.MergeSources 4)
        Trace (x1.Value, x2.Value, x3.Value, x4.Value)


type TraceApplicativeCore() =
    inherit TraceMergeSourcesCore()

    // Note that per the RFC in true applicatives the 'Bind' and 'Return' have non-standard types
    member builder.Bind(x: Trace<'T1>, f: 'T1 -> 'T2) : Trace<'T2> =
        builder.Trace TraceOp.ApplicativeBind
        Trace (f x.Value)

    // Note that per the RFC in true applicatives the 'Bind' and 'Return' have non-standard types
    member builder.Bind2(x1: Trace<'T1>, x2: Trace<'T2>, f: 'T1 * 'T2 -> 'T3) : Trace<'T3> =
        builder.Trace TraceOp.ApplicativeBind2
        Trace (f (x1.Value, x2.Value))

type TraceApplicative() =
    inherit TraceApplicativeCore()

    // Note that per the RFC in true applicatives the 'Return' has non-standard types 'T -> 'T
    member builder.Return(x: 'T) : 'T =
        builder.Trace TraceOp.ApplicativeReturn
        x

type TraceApplicativeMonoid() =
    inherit TraceApplicativeCore()

    // Note that per the RFC in true applicatives the 'Yield' has non-standard type 'T -> 'T list
    member builder.Yield(x: 'T) : 'T list =
        builder.Trace TraceOp.ApplicativeYield
        [x]

    // Note that per the RFC in true applicatives the 'Combine' has non-standard type 'T list * 'T list-> 'T list
    member builder.Combine(x1: 'T list, x2: 'T list) =
        builder.Trace TraceOp.ApplicativeCombine
        x1 @ x2

    member builder.Delay(thunk) =
        builder.Trace TraceOp.Delay
        thunk ()

type TraceApplicativeWithDelayAndRun() =
    inherit TraceApplicative()

    member builder.Run(x) =
        builder.Trace TraceOp.Run
        x

    member builder.Delay(thunk) =
        builder.Trace TraceOp.Delay
        thunk ()

type TraceApplicativeWithDelay() =
    inherit TraceApplicative()

    member builder.Delay(thunk) =
        builder.Trace TraceOp.Delay
        thunk ()

type TraceApplicativeWithRun() =
    inherit TraceApplicative()

    member builder.Run(x) =
        builder.Trace TraceOp.Run
        x

type TraceMonadic() =
    inherit TraceMergeSourcesCore()

    member builder.Bind(x : Trace<'T1>, f : 'T1 -> Trace<'T2>) : Trace<'T2> =
        builder.Trace TraceOp.MonadicBind
        f x.Value

    member builder.Bind2(x1 : 'T1 Trace, x2 : 'T2 Trace, f : 'T1 * 'T2 -> Trace<'T3>) : Trace<'T3> =
        builder.Trace TraceOp.MonadicBind2
        f (x1.Value, x2.Value)

    member builder.Return(x: 'T) : Trace<'T> =
        builder.Trace TraceOp.MonadicReturn
        Trace x

let check msg actual expected = if actual <> expected then failwithf "FAILED %s, expected %A, got %A" msg expected actual
"""

    let ApplicativeLibTest source =
        CompilerAssert.CompileExeAndRunWithOptions [| "/langversion:preview" |]
            (applicativeLib + source)


    [<Test>]
    let ``AndBang TraceApplicative`` () =
        ApplicativeLibTest """

let tracer = TraceApplicative()

let ceResult : Trace<int> =
    tracer {
        let! x = Trace 3
        and! y = Trace true
        return if y then x else -1
    }

check "fewljvwerjl1" ceResult.Value 3
check "fewljvwerj12" (tracer.GetTrace ()) [|TraceOp.ApplicativeBind2; TraceOp.ApplicativeReturn|]
            """

    [<Test>]
    let ``AndBang TraceApplicativeMonoid`` () =
        ApplicativeLibTest """

let tracer = TraceApplicativeMonoid()

let ceResult : Trace<int list> =
    tracer {
        let! x = Trace 3
        and! y = Trace true
        yield (if y then x else -1)
        yield (if y then 5 else -1)
    }

check "fewljvwerjl5" ceResult.Value [3; 5]
check "fewljvwerj16" (tracer.GetTrace ()) [|TraceOp.Delay; TraceOp.ApplicativeBind2; TraceOp.ApplicativeYield; TraceOp.Delay; TraceOp.ApplicativeYield; TraceOp.ApplicativeCombine|]
            """

    [<Test>]
    let ``AndBang TraceMonadic`` () =
        ApplicativeLibTest """

let tracer = TraceMonadic()

let ceResult : Trace<int> =
    tracer {
        let fb = Trace "foobar"
        match! fb with
        | "bar" ->
            let! bar = fb
            return String.length bar
        | _ ->
            let! x = Trace 3
            and! y = Trace true
            return if y then x else -1
    }

check "gwrhjkrwpoiwer1" ceResult.Value 3
check "gwrhjkrwpoiwer2" (tracer.GetTrace ())  [|TraceOp.MonadicBind; TraceOp.MonadicBind2; TraceOp.MonadicReturn|]
            """


    [<Test>]
    let ``AndBang TraceMonadic TwoBind`` () =
        ApplicativeLibTest """

let tracer = TraceMonadic()

let ceResult : Trace<int> =
    tracer {
        let fb = Trace "foobar"
        match! fb with
        | "bar" ->
            let! bar = fb
            return String.length bar
        | _ ->
            let! x = Trace 3
            and! y = Trace true
            let! x2 = Trace x
            and! y2 = Trace y
            if y2 then return x2 else return  -1
    }

check "gwrhjkrwpoiwer38" ceResult.Value 3
check "gwrhjkrwpoiwer39" (tracer.GetTrace ())  [|TraceOp.MonadicBind; TraceOp.MonadicBind2; TraceOp.MonadicBind2; TraceOp.MonadicReturn|]
            """

    [<Test>]
    let ``AndBang TraceApplicativeWithDelayAndRun`` () =
        ApplicativeLibTest """

let tracer = TraceApplicativeWithDelayAndRun()

let ceResult : Trace<int> =
    tracer {
        let! x = Trace 3
        and! y = Trace true
        return if y then x else -1
    }

check "vlkjrrlwevlk23" ceResult.Value 3
check "vlkjrrlwevlk24" (tracer.GetTrace ())  [|TraceOp.Delay; TraceOp.ApplicativeBind2; TraceOp.ApplicativeReturn; TraceOp.Run|]
        """

    [<Test>]
    let ``AndBang TraceApplicativeWithDelay`` () =
        ApplicativeLibTest """

let tracer = TraceApplicativeWithDelay()

let ceResult : int Trace =
    tracer {
        let! x = Trace 3
        and! y = Trace true
        return if y then x else -1
    }

check "vlkjrrlwevlk23" ceResult.Value 3
check "vlkjrrlwevlk24" (tracer.GetTrace ())  [|TraceOp.Delay; TraceOp.ApplicativeBind2; TraceOp.ApplicativeReturn|]
        """

    [<Test>]
    let ``AndBang TraceApplicativeWithRun`` () =
        ApplicativeLibTest """

let tracer = TraceApplicativeWithRun()

let ceResult : int Trace =
    tracer {
        let! x = Trace 3
        and! y = Trace true
        return if y then x else -1
    }

check "vwerweberlk3" ceResult.Value 3
check "vwerweberlk4" (tracer.GetTrace ())  [|TraceOp.ApplicativeBind2; TraceOp.ApplicativeReturn; TraceOp.Run |]
        """


    [<Test>]
    let ``AndBang TraceApplicative Size 3`` () =
        ApplicativeLibTest """

let tracer = TraceApplicative()

let ceResult =
    tracer {
        let! x = Trace 3
        and! y = Trace true
        and! z = Trace 5
        return if y then x else z
    }

check "fewljvwerjl7" ceResult.Value 3
check "fewljvwerj18" (tracer.GetTrace ()) [|TraceOp.MergeSources 3; TraceOp.ApplicativeBind; TraceOp.ApplicativeReturn|]
        """

    [<Test>]
    let ``AndBang TraceApplicative Size 4`` () =
        ApplicativeLibTest """

let tracer = TraceApplicative()

let ceResult =
    tracer {
        let! x1 = Trace 3
        and! x2 = Trace true
        and! x3 = Trace 5
        and! x4 = Trace 5
        return if x2 then x1 else x3+x4
    }

check "fewljvwerjl191" ceResult.Value 3
check "fewljvwerj1192" (tracer.GetTrace ()) [|TraceOp.MergeSources 4; TraceOp.ApplicativeBind; TraceOp.ApplicativeReturn|]
        """

    [<Test>]
    let ``AndBang TraceApplicative Size 5`` () =
        ApplicativeLibTest """

let tracer = TraceApplicative()

let ceResult : Trace<int> =
    tracer {
        let! x1 = Trace 3
        and! x2 = Trace true
        and! x3 = Trace 5
        and! x4 = Trace 5
        and! x5 = Trace 8
        return if x2 then x1+x4+x5 else x3
    }

check "fewljvwerjl193" ceResult.Value 16
check "fewljvwerj1194" (tracer.GetTrace ()) [|TraceOp.MergeSources 2; TraceOp.MergeSources 4; TraceOp.ApplicativeBind; TraceOp.ApplicativeReturn|]
        """

    [<Test>]
    let ``AndBang TraceApplicative Size 6`` () =
        ApplicativeLibTest """

let tracer = TraceApplicative()

let ceResult : Trace<int> =
    tracer {
        let! x1 = Trace 3
        and! x2 = Trace true
        and! x3 = Trace 5
        and! x4 = Trace 5
        and! x5 = Trace 8
        and! x6 = Trace 9
        return if x2 then x1+x4+x5+x6 else x3
    }

check "fewljvwerjl195" ceResult.Value 25
check "fewljvwerj1196" (tracer.GetTrace ()) [|TraceOp.MergeSources 3; TraceOp.MergeSources 4; TraceOp.ApplicativeBind; TraceOp.ApplicativeReturn|]
        """

    [<Test>]
    let ``AndBang TraceApplicative Size 10`` () =
        ApplicativeLibTest """

let tracer = TraceApplicative()

let ceResult : Trace<int> =
    tracer {
        let! x1 = Trace 3
        and! x2 = Trace true
        and! x3 = Trace 5
        and! x4 = Trace 5
        and! x5 = Trace 8
        and! x6 = Trace 9
        and! x7 = Trace 1
        and! x8 = Trace 2
        and! x9 = Trace 3
        and! x10 = Trace 4
        return if x2 then x1+x4+x5+x6+x7+x8+x9+x10 else x3
    }

check "fewljvwerjl197" ceResult.Value 35
check "fewljvwerj1198" (tracer.GetTrace ()) [|TraceOp.MergeSources 4; TraceOp.MergeSources 4; TraceOp.MergeSources 4; TraceOp.ApplicativeBind; TraceOp.ApplicativeReturn|]
    """
