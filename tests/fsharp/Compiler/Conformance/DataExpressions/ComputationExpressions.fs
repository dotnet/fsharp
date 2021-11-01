// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test
open FSharp.Compiler.Diagnostics

[<TestFixture>]
module ``ComputationExpressions`` =
    let tmp = 1

    type Flags = { includeMergeSourcesOverloads: bool; includeBindReturnExtras: bool }
    let applicativeLib  (opts: Flags) = 
        """
/// Used for tracking what operations a Trace builder was asked to perform
[<RequireQualifiedAccess>]
type TraceOp =
    | ApplicativeBind
    | ApplicativeBind2
    | ApplicativeBindReturn
    | ApplicativeBind2Return
    | ApplicativeReturn
    | ApplicativeCombine
    | ApplicativeYield
    | MergeSources
    | MergeSources3
    | MergeSources4
    | MonadicBind
    | MonadicBind2
    | MonadicReturn
    | Run
    | Delay
    | Log of string

/// A pseudo identity functor
type Trace<'T>(v: 'T) =
    member x.Value = v
    override this.ToString () =
        sprintf "%+A" v

/// A builder which records what operations it is asked to perform
type TraceCore() =

    let mutable trace = ResizeArray<_>()

    member _.GetTrace () = trace.ToArray()

    member _.Trace x = trace.Add(x)

type TraceMergeSourcesCore() =
    inherit TraceCore()

    member builder.MergeSources(x1: Trace<'T1>, x2: Trace<'T2>) : Trace<'T1 * 'T2> =
        builder.Trace TraceOp.MergeSources
        Trace (x1.Value, x2.Value) 
        """ + (if opts.includeMergeSourcesOverloads then """

    // Note the struct tuple is acceptable
    member builder.MergeSources3(x1: Trace<'T1>, x2: Trace<'T2>, x3: Trace<'T3>) : Trace<struct ('T1 * 'T2 * 'T3)> =
        builder.Trace TraceOp.MergeSources3
        Trace (struct (x1.Value, x2.Value, x3.Value))

    member builder.MergeSources4(x1: Trace<'T1>, x2: Trace<'T2>, x3: Trace<'T3>, x4: Trace<'T4>) : Trace<'T1 * 'T2 * 'T3 * 'T4> =
        builder.Trace TraceOp.MergeSources4
        Trace (x1.Value, x2.Value, x3.Value, x4.Value)
        """ else "") + """

type TraceApplicative() =
    inherit TraceMergeSourcesCore()

    member builder.BindReturn(x: Trace<'T1>, f: 'T1 -> 'T2) : Trace<'T2> =
        builder.Trace TraceOp.ApplicativeBindReturn
        Trace (f x.Value)

        """ + (if opts.includeBindReturnExtras then """
    member builder.Bind2Return(x1: Trace<'T1>, x2: Trace<'T2>, f: 'T1 * 'T2 -> 'T3) : Trace<'T3> =
        builder.Trace TraceOp.ApplicativeBind2Return
        Trace (f (x1.Value, x2.Value))
        """ else "") + """

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

type TraceMultiBindingMonadic() =
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

type TraceMultiBindingMonoid() =
    inherit TraceMergeSourcesCore()

    member builder.Bind(x : Trace<'T1>, f : 'T1 -> Trace<'T2>) : Trace<'T2> =
        builder.Trace TraceOp.MonadicBind
        f x.Value

    member builder.Bind2(x1 : 'T1 Trace, x2 : 'T2 Trace, f : 'T1 * 'T2 -> Trace<'T3>) : Trace<'T3> =
        builder.Trace TraceOp.MonadicBind2
        f (x1.Value, x2.Value)

    member builder.Yield(x: 'T) : Trace<'T list> =
        builder.Trace TraceOp.ApplicativeYield
        Trace [x]

    member builder.Combine(x1: Trace<'T list>, x2: Trace<'T list>) : Trace<'T list> =
        builder.Trace TraceOp.ApplicativeCombine
        Trace (x1.Value @ x2.Value)

    member builder.Delay(thunk) =
        builder.Trace TraceOp.Delay
        thunk ()

    member builder.Zero() =
        Trace []

type TraceApplicativeNoMergeSources() =
    inherit TraceCore()

    member builder.BindReturn(x: Trace<'T1>, f: 'T1 -> 'T2) : Trace<'T2> =
        builder.Trace TraceOp.ApplicativeBind
        Trace (f x.Value)

type TraceApplicativeNoBindReturn() =
    inherit TraceCore()

    member builder.MergeSources(x1: Trace<'T1>, x2: Trace<'T2>) : Trace<'T1 * 'T2> =
        builder.Trace TraceOp.MergeSources
        Trace (x1.Value, x2.Value) 

type TraceMultiBindingMonadicCustomOp() =
    inherit TraceMultiBindingMonadic()

    [<CustomOperation("log", MaintainsVariableSpaceUsingBind = true)>]
    member builder.Log(boundValues : Trace<'T>, [<ProjectionParameter>] messageFunc: 'T -> string) =
        builder.Trace (TraceOp.Log (messageFunc boundValues.Value))
        boundValues

type TraceApplicativeCustomOp() =
    inherit TraceApplicative()

    [<CustomOperation("log", MaintainsVariableSpaceUsingBind = true)>]
    member builder.Log(boundValues : Trace<'T>, message: string) =
        builder.Trace (TraceOp.Log message)
        boundValues

let check msg actual expected = if actual <> expected then failwithf "FAILED %s, expected %A, got %A" msg expected actual
        """

    let includeAll = { includeMergeSourcesOverloads = true; includeBindReturnExtras=true }
    let includeMinimal = { includeMergeSourcesOverloads = false; includeBindReturnExtras=false }

    let ApplicativeLibTest opts source =
        CompilerAssert.CompileExeAndRunWithOptions [| "/langversion:preview" |] (applicativeLib opts + source)

    let ApplicativeLibErrorTest opts source errors =
        let lib = applicativeLib opts
        // Adjust the expected errors for the number of lines in the library
        let libLineAdjust = lib |> Seq.filter (fun c -> c = '\n') |> Seq.length
        CompilerAssert.TypeCheckWithErrorsAndOptionsAndAdjust [| "/langversion:preview" |] libLineAdjust (lib + source) errors

    let ApplicativeLibErrorTestFeatureDisabled opts source errors =
        let lib = applicativeLib opts
        // Adjust the expected errors for the number of lines in the library
        let libLineAdjust = lib |> Seq.filter (fun c -> c = '\n') |> Seq.length
        CompilerAssert.TypeCheckWithErrorsAndOptionsAndAdjust [| "/langversion:4.7" |] libLineAdjust (lib + source) errors

    [<Test>]
    let ``AndBang TraceApplicative`` () =
        ApplicativeLibTest includeAll """

let tracer = TraceApplicative()

let ceResult : Trace<int> =
    tracer {
        let! x = Trace 3
        and! y = Trace true
        return if y then x else -1
    }

check "fewljvwerjl1" ceResult.Value 3
check "fewljvwerj12" (tracer.GetTrace ()) [|TraceOp.ApplicativeBind2Return|]
            """

    [<Test>]
    let ``AndBang TraceApplicativeCustomOp`` () =
        ApplicativeLibTest includeAll """

let tracer = TraceApplicativeCustomOp()

let ceResult : Trace<int> =
    tracer {
        let! x = Trace 3
        and! y = Trace true
        log "hello!"
        return if y then x else -1
    }

check "fewljvwerjlvwe1" ceResult.Value 3
check "fewljvwerjvwe12" (tracer.GetTrace ()) [|TraceOp.ApplicativeBind2Return; TraceOp.Log "hello!";TraceOp.ApplicativeBindReturn|]
            """

    [<Test>]
    let ``AndBang TraceApplicativeCustomOp Minimal`` () =
        ApplicativeLibTest includeMinimal """

let tracer = TraceApplicativeCustomOp()

let ceResult : Trace<int> =
    tracer {
        let! x = Trace 3
        and! y = Trace true
        log "hello!"
        return if y then x else -1
    }

check "fewljvwerjlvwe1" ceResult.Value 3
check "fewljvwerjvwe12" (tracer.GetTrace ()) [|TraceOp.MergeSources; TraceOp.ApplicativeBindReturn; TraceOp.Log "hello!";TraceOp.ApplicativeBindReturn|]
            """

    [<Test>]
    let ``AndBang TraceApplicativeCustomOpTwice`` () =
        ApplicativeLibTest includeAll """

let tracer = TraceApplicativeCustomOp()

let ceResult : Trace<int> =
    tracer {
        let! x = Trace 3
        and! y = Trace true
        log "hello!"
        log "goodbye!"
        return if y then x else -1
    }

check "fewljvwerjlvwe1" ceResult.Value 3
check "fewljvwerjvwe12" (tracer.GetTrace ()) [|TraceOp.ApplicativeBind2Return; TraceOp.Log "hello!";TraceOp.Log "goodbye!";TraceOp.ApplicativeBindReturn|]
            """

    [<Test>]
    let ``AndBang TraceApplicative Disable`` () =
        ApplicativeLibErrorTestFeatureDisabled includeAll 
            """
let tracer = TraceApplicative()

let ceResult : Trace<int> =
    tracer {
        let! x = Trace 3
        and! y = Trace true
        return if y then x else -1
    }
            """
            [| FSharpDiagnosticSeverity.Error, 3344, (6, 9, 8, 35), "This feature is not supported in this version of F#. You may need to add /langversion:preview to use this feature." |]

    [<Test>]
    let ``AndBang TraceMultiBindingMonoid`` () =
        ApplicativeLibTest includeAll """

let tracer = TraceMultiBindingMonoid()

let ceResult : Trace<int list> =
    tracer {
        let! x = Trace 3
        and! y = Trace true
        yield (if y then x else -1)
        yield (if y then 5 else -1)
    }

check "fewljvwerjl5" ceResult.Value [3; 5]
check "fewljvwerj16" (tracer.GetTrace ()) [|TraceOp.Delay; TraceOp.MonadicBind2; TraceOp.ApplicativeYield; TraceOp.Delay; TraceOp.ApplicativeYield; TraceOp.ApplicativeCombine|]
            """

    [<Test>]
    let ``AndBang TraceMultiBindingMonadic`` () =
        ApplicativeLibTest includeAll """

let tracer = TraceMultiBindingMonadic()

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
    let ``AndBang TraceMultiBindingMonadicCustomOp A`` () =
        ApplicativeLibTest includeAll """

let tracer = TraceMultiBindingMonadicCustomOp()
let ceResult : Trace<int> =
    tracer {
        let! x = Trace 3
        log (sprintf "%A" x)
        return x
    }

check "gwrhjkrwpoiwer1t4" ceResult.Value 3
            """

    [<Test>]
    let ``AndBang TraceMultiBindingMonadicCustomOp B`` () =
        ApplicativeLibTest includeAll """
let tracer = TraceMultiBindingMonadicCustomOp()
let ceResult : Trace<int> =
    tracer {
        let! x = Trace 3
        and! y = Trace true
        log (sprintf "%A" (x,y))
        return (ignore y; x)
    }

check "gwrhjkrwpoiwer1t45" ceResult.Value 3
check "gwrhjkrwpoiwer2t36" (tracer.GetTrace ())  [|TraceOp.MonadicBind2; TraceOp.MonadicReturn; TraceOp.Log "(3, true)"; TraceOp.MonadicBind; TraceOp.MonadicReturn |]
            """

    [<Test>]
    let ``AndBang TraceMultiBindingMonadic TwoBind`` () =
        ApplicativeLibTest includeAll """

let tracer = TraceMultiBindingMonadic()

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
        ApplicativeLibTest includeAll """

let tracer = TraceApplicativeWithDelayAndRun()

let ceResult : Trace<int> =
    tracer {
        let! x = Trace 3
        and! y = Trace true
        return if y then x else -1
    }

check "vlkjrrlwevlk23" ceResult.Value 3
check "vlkjrrlwevlk24" (tracer.GetTrace ())  [|TraceOp.Delay; TraceOp.ApplicativeBind2Return; TraceOp.Run|]
        """

    [<Test>]
    let ``AndBang TraceApplicativeWithDelay`` () =
        ApplicativeLibTest includeAll """

let tracer = TraceApplicativeWithDelay()

let ceResult : int Trace =
    tracer {
        let! x = Trace 3
        and! y = Trace true
        return if y then x else -1
    }

check "vlkjrrlwevlk23" ceResult.Value 3
check "vlkjrrlwevlk24" (tracer.GetTrace ())  [|TraceOp.Delay; TraceOp.ApplicativeBind2Return|]
        """

    [<Test>]
    let ``AndBang TraceApplicativeWithDelay Minimal`` () =
        ApplicativeLibTest includeMinimal """

let tracer = TraceApplicativeWithDelay()

let ceResult : int Trace =
    tracer {
        let! x = Trace 3
        and! y = Trace true
        return if y then x else -1
    }

check "vlkjrrlwevlk23" ceResult.Value 3
check "vlkjrrlwevlk24" (tracer.GetTrace ())  [|TraceOp.Delay; TraceOp.MergeSources; TraceOp.ApplicativeBindReturn|]
        """

    [<Test>]
    let ``AndBang TraceApplicativeWithRun`` () =
        ApplicativeLibTest includeAll """

let tracer = TraceApplicativeWithRun()

let ceResult : int Trace =
    tracer {
        let! x = Trace 3
        and! y = Trace true
        return if y then x else -1
    }

check "vwerweberlk3" ceResult.Value 3
check "vwerweberlk4" (tracer.GetTrace ())  [|TraceOp.ApplicativeBind2Return; TraceOp.Run |]
        """


    [<Test>]
    let ``AndBang TraceApplicative Size 3`` () =
        ApplicativeLibTest includeAll """

let tracer = TraceApplicative()

let ceResult =
    tracer {
        let! x = Trace 3
        and! y = Trace true
        and! z = Trace 5
        return if y then x else z
    }

check "fewljvwerjl7" ceResult.Value 3
check "fewljvwerj18" (tracer.GetTrace ()) [|TraceOp.MergeSources3; TraceOp.ApplicativeBindReturn|]
        """

    [<Test>]
    let ``AndBang TraceApplicative Size 3 minimal`` () =
        ApplicativeLibTest includeMinimal """

let tracer = TraceApplicative()

let ceResult =
    tracer {
        let! x = Trace 3
        and! y = Trace true
        and! z = Trace 5
        return if y then x else z
    }

check "fewljvwerjl7" ceResult.Value 3
check "fewljvwerj18" (tracer.GetTrace ()) [|TraceOp.MergeSources; TraceOp.MergeSources; TraceOp.ApplicativeBindReturn|]
        """
    [<Test>]
    let ``AndBang TraceApplicative Size 4`` () =
        ApplicativeLibTest includeAll """

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
check "fewljvwerj1192" (tracer.GetTrace ()) [|TraceOp.MergeSources4; TraceOp.ApplicativeBindReturn|]
        """

    [<Test>]
    let ``AndBang TraceApplicative Size 5`` () =
        ApplicativeLibTest includeAll """

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
check "fewljvwerj1194" (tracer.GetTrace ()) [|TraceOp.MergeSources; TraceOp.MergeSources4; TraceOp.ApplicativeBindReturn|]
        """

    [<Test>]
    let ``AndBang TraceApplicative Size 6`` () =
        ApplicativeLibTest includeAll """

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
check "fewljvwerj1196" (tracer.GetTrace ()) [|TraceOp.MergeSources3; TraceOp.MergeSources4; TraceOp.ApplicativeBindReturn|]
        """

    [<Test>]
    let ``AndBang TraceApplicative Size 10`` () =
        ApplicativeLibTest includeAll """

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
check "fewljvwerj1198" (tracer.GetTrace ()) [|TraceOp.MergeSources4; TraceOp.MergeSources4; TraceOp.MergeSources4; TraceOp.ApplicativeBindReturn|]
    """


    [<Test>]
    let ``AndBang Negative TraceApplicative missing MergeSources`` () =
        ApplicativeLibErrorTest includeAll """
let tracer = TraceApplicativeNoMergeSources()

let _ = 
    tracer {
        let! x = Trace 1
        and! y = Trace 2
        return x + y
    }
    """
            [|(FSharpDiagnosticSeverity.Error, 3343, (6, 9, 6, 25), "The 'let! ... and! ...' construct may only be used if the computation expression builder defines either a 'Bind2' method or appropriate 'MergeSource' and 'Bind' methods")|]

    [<Test>]
    let ``AndBang Negative TraceApplicative missing Bind and BindReturn`` () =
        ApplicativeLibErrorTest includeAll """
let tracer = TraceApplicativeNoBindReturn()

let _ = 
    tracer {
        let! x = Trace 1
        and! y = Trace 2
        return x + y
    }
    """
            [|(FSharpDiagnosticSeverity.Error, 708, (6, 9, 6, 25), "This control construct may only be used if the computation expression builder defines a 'Bind' method")|]


    [<Test>]
    let ``AndBang Negative TraceApplicative with bad construct`` () =
        ApplicativeLibErrorTest includeAll """

let tracer = TraceApplicativeNoBindReturn()

let _ = 
    tracer {
        let! x = Trace 1 // this is a true bind, check the error message here
        let! x2 = Trace 1
        return x + y
    }
    """
            [| FSharpDiagnosticSeverity.Error, 708, (7, 9, 7, 25), "This control construct may only be used if the computation expression builder defines a 'Bind' method" |]

    [<Test>]
    let ``AndBang TraceApplicative with do-bang`` () =
        ApplicativeLibErrorTest includeAll """
let tracer = TraceApplicative()

let _ = 
    tracer {
        do! Trace() 
        and! x = Trace 1
        and! y = Trace 2
        return x + y
    }
    """
            [|(FSharpDiagnosticSeverity.Error, 10, (7, 9, 7, 13),"Unexpected keyword 'and!' in expression. Expected '}' or other token.");
              (FSharpDiagnosticSeverity.Error, 604, (5, 12, 5, 13), "Unmatched '{'");
              (FSharpDiagnosticSeverity.Error, 10, (8, 9, 8, 13), "Unexpected keyword 'and!' in implementation file")|]

    [<Test>]
    let ``AndBang Negative TraceApplicative let betweeen let! and and!`` () =
        ApplicativeLibErrorTest includeAll """
let tracer = TraceApplicative()

let _ = 
    tracer {
        let! x = Trace 1
        let _ = 42
        and! y = Trace 2
        return x + y
    }
    """
            [| (FSharpDiagnosticSeverity.Error, 10, (8, 9, 8, 13), "Unexpected keyword 'and!' in expression") |]


    [<Test>]
    let ``AndBang Negative TraceApplicative no return`` () =
        ApplicativeLibErrorTest includeAll """
let tracer = TraceApplicative()

let _ = 
    tracer {
        let! x = Trace 1
        and! y = Trace 2
    }
    """
            [|(FSharpDiagnosticSeverity.Error, 10, (8, 5, 8, 6), "Unexpected symbol '}' in expression")|]

    [<Test>]
    let ``AndBang TraceApplicative conditional return`` () =
        ApplicativeLibTest includeAll """
let tracer = TraceApplicative()

let ceResult = 
    tracer {
        let! x = Trace 1
        and! y = Trace 2
        if x = 1 then 
            return y
        else 
            return 4
    }
check "grwerjkrwejgk" ceResult.Value 2
    """

    [<Test>]
    let ``AndBang TraceApplicative match return`` () =
        ApplicativeLibTest includeAll """
let tracer = TraceApplicative()

let ceResult = 
    tracer {
        let! x = Trace 1
        and! y = Trace 2
        match x with 
        | 1 -> return y
        | _ -> return 4
    }
check "grwerjkrwejgk42" ceResult.Value 2
    """

    [<Test>]
    let ``AndBang TraceApplicative incomplete match return`` () =
        ApplicativeLibTest includeAll """
#nowarn "25"

let tracer = TraceApplicative()

let ceResult = 
    tracer {
        let! x = Trace 1
        and! y = Trace 2
        match x with 
        | 1 -> return y
    }
check "grwerjkrwejgk42" ceResult.Value 2
    """

    let overloadLib includeInternalExtensions includeExternalExtensions = 
        """
open System

type Content = ArraySegment<byte> list

type ContentBuilder() =
    member this.Run(c: Content) =
        let crlf = "\r\n"B
        [|for part in List.rev c do
            yield! part.Array[part.Offset..(part.Count+part.Offset-1)]
            yield! crlf |]

    member this.Yield(_) = []

    [<CustomOperation("body")>]
    member this.Body(c: Content, segment: ArraySegment<byte>) =
        segment::c
        """ + (if includeInternalExtensions then """

type ContentBuilder with
    // unattributed internal type extension with same arity
    member this.Body(c: Content, bytes: byte[]) =
        ArraySegment<byte>(bytes, 0, bytes.Length)::c

    // internal type extension with different arity
    [<CustomOperation("body")>]
    member this.Body(c: Content, bytes: byte[], offset, count) =
        ArraySegment<byte>(bytes, offset, count)::c
        """ else """

    // unattributed type member with same arity
    member this.Body(c: Content, bytes: byte[]) =
        ArraySegment<byte>(bytes, 0, bytes.Length)::c

    // type member with different arity
    [<CustomOperation("body")>]
    member this.Body(c: Content, bytes: byte[], offset, count) =
        ArraySegment<byte>(bytes, offset, count)::c
        """) + (if includeExternalExtensions then """

module Extensions =
    type ContentBuilder with
        // unattributed external type extension with same arity
        member this.Body(c: Content, content: System.IO.Stream) =
            let mem = new System.IO.MemoryStream()
            content.CopyTo(mem)
            let bytes = mem.ToArray()
            ArraySegment<byte>(bytes, 0, bytes.Length)::c

        // external type extensions as ParamArray
        [<CustomOperation("body")>]
        member this.Body(c: Content, [<ParamArray>] contents: string[]) =
            List.rev [for c in contents -> let b = Text.Encoding.ASCII.GetBytes c in ArraySegment<_>(b,0,b.Length)] @ c
open Extensions
        """ else """

    // unattributed type member with same arity
    member this.Body(c: Content, content: System.IO.Stream) =
        let mem = new System.IO.MemoryStream()
        content.CopyTo(mem)
        let bytes = mem.ToArray()
        ArraySegment<byte>(bytes, 0, bytes.Length)::c

    // type members
    [<CustomOperation("body")>]
    member this.Body(c: Content, [<ParamArray>] contents: string[]) =
        List.rev [for c in contents -> let b = Text.Encoding.ASCII.GetBytes c in ArraySegment<_>(b,0,b.Length)] @ c
        """) + """

let check msg actual expected = if actual <> expected then failwithf "FAILED %s, expected %A, got %A" msg expected actual
        """

    let OverloadLibTest inclInternalExt inclExternalExt source =
        CompilerAssert.CompileExeAndRunWithOptions [| "/langversion:preview" |] (overloadLib inclInternalExt inclExternalExt + source)

    [<Test>]
    let ``OverloadLib accepts overloaded methods`` () =
        OverloadLibTest false false """
let mem = new System.IO.MemoryStream("Stream"B)
let content = ContentBuilder()
let ceResult =
    content {
        body "Name"
        body (ArraySegment<_>("Email"B, 0, 5))
        body "Password"B 2 4
        body "BYTES"B
        body mem
        body "Description" "of" "content"
    }
check "TmFtZVxyXG5FbWF1" ceResult "Name\r\nEmail\r\nsswo\r\nBYTES\r\nStream\r\nDescription\r\nof\r\ncontent\r\n"B
    """

    [<Test>]
    let ``OverloadLib accepts overloaded internal extension methods`` () =
        OverloadLibTest true false """
let mem = new System.IO.MemoryStream("Stream"B)
let content = ContentBuilder()
let ceResult =
    content {
        body "Name"
        body (ArraySegment<_>("Email"B, 0, 5))
        body "Password"B 2 4
        body "BYTES"B
        body mem
        body "Description" "of" "content"
    }
check "TmFtZVxyXG5FbWF2" ceResult "Name\r\nEmail\r\nsswo\r\nBYTES\r\nStream\r\nDescription\r\nof\r\ncontent\r\n"B
    """

    [<Test>]
    let ``OverloadLib accepts overloaded internal and external extensions`` () =
        OverloadLibTest true true """
let mem = new System.IO.MemoryStream("Stream"B)
let content = ContentBuilder()
let ceResult =
    content {
        body "Name"
        body (ArraySegment<_>("Email"B, 0, 5))
        body "Password"B 2 4
        body "BYTES"B
        body mem
        body "Description" "of" "content"
    }
check "TmFtZVxyXG5FbWF3" ceResult "Name\r\nEmail\r\nsswo\r\nBYTES\r\nStream\r\nDescription\r\nof\r\ncontent\r\n"B
    """
