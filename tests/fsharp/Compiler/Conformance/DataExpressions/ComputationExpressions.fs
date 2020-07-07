// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices

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
            [| FSharpErrorSeverity.Error, 3344, (6, 9, 8, 35), "This feature is not supported in this version of F#. You may need to add /langversion:preview to use this feature." |]

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
            [|(FSharpErrorSeverity.Error, 3343, (6, 9, 6, 25), "The 'let! ... and! ...' construct may only be used if the computation expression builder defines either a 'Bind2' method or appropriate 'MergeSource' and 'Bind' methods")|]

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
            [|(FSharpErrorSeverity.Error, 708, (6, 9, 6, 25), "This control construct may only be used if the computation expression builder defines a 'Bind' method")|]


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
            [| FSharpErrorSeverity.Error, 708, (7, 9, 7, 25), "This control construct may only be used if the computation expression builder defines a 'Bind' method" |]

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
            [|(FSharpErrorSeverity.Error, 10, (7, 9, 7, 13),"Unexpected keyword 'and!' in expression. Expected '}' or other token.");
              (FSharpErrorSeverity.Error, 604, (5, 12, 5, 13), "Unmatched '{'");
              (FSharpErrorSeverity.Error, 10, (8, 9, 8, 13), "Unexpected keyword 'and!' in implementation file")|]

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
            [| (FSharpErrorSeverity.Error, 10, (8, 9, 8, 13), "Unexpected keyword 'and!' in expression") |]


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
            [|(FSharpErrorSeverity.Error, 10, (8, 5, 8, 6), "Unexpected symbol '}' in expression")|]

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

    [<Test>]
    let ``Applicative MergeSourcesN`` () =
        let source = """
open System

let inline ivk f x =
    let inline call (_: ^I, x:'TT) = ((^I or ^TT) : (static member Invoke : _-> _) x)
    call ( f, x)  

let inline loop f x =
    let inline call (_: ^I, x:'TT) = ((^I or ^TT) : (static member Loop : _-> _) x)
    call ( f, x)

type Uncons = Uncons with
    static member inline Invoke tuple = (Unchecked.defaultof<Uncons> => tuple)

    static member inline (=>) (_:obj, t : 't when 't : not struct) =
        let rest = (Uncons.Invoke (^t : (member Rest : _) t))
        (^t : (member Item1 : _) t) , 
            System.Tuple<_,_,_,_,_,_,_,_>(
                (^t : (member Item2 : _) t) ,
                (^t : (member Item3 : _) t) ,
                (^t : (member Item4 : _) t) ,
                (^t : (member Item5 : _) t) ,
                (^t : (member Item6 : _) t) ,
                (^t : (member Item7 : _) t) ,
                fst rest, snd rest)

    static member inline (=>) (Uncons, t : 't when 't : not struct) =
        let rest = (Uncons.Invoke (^t : (member Rest : _) t)) : _ * unit
        (^t : (member Item1 : _) t) , 
            System.Tuple<_,_,_,_,_,_,_>(
                (^t : (member Item2 : _) t) ,
                (^t : (member Item3 : _) t) ,
                (^t : (member Item4 : _) t) ,
                (^t : (member Item5 : _) t) ,
                (^t : (member Item6 : _) t) ,
                (^t : (member Item7 : _) t) ,
                fst rest)

    static member (=>) (Uncons, x1:Tuple<_>) = (x1.Item1, ())
    static member (=>) (Uncons, (a, b)) = a, System.Tuple<_>(b)
    static member (=>) (Uncons, (a, b, c)) = a, (b, c)
    static member (=>) (Uncons, (a, b, c, d)) = a, (b, c, d)
    static member (=>) (Uncons, (a, b, c, d, e)) = a, (b, c, d, e)
    static member (=>) (Uncons, (a, b, c, d, e, f)) = a, (b, c, d, e, f)
    static member (=>) (Uncons, (a, b, c, d, e, f, g)) = a, (b, c, d, e, f, g)
 

type Cons = Cons with
    static member inline Invoke tuple = let inline f (_: 'M, t: 'T) = ((^M or ^T) : (static member (!) : _ -> _) t) in f (Cons, tuple)
    static member inline (!) (t:'t) = fun x -> 
        let (x1,x2,x3,x4,x5,x6,x7,xr) = 
            (
                (^t : (member Item1 : 't1) t),
                (^t : (member Item2 : 't2) t),
                (^t : (member Item3 : 't3) t),
                (^t : (member Item4 : 't4) t),
                (^t : (member Item5 : 't5) t),
                (^t : (member Item6 : 't6) t),
                (^t : (member Item7 : 't7) t),
                (^t : (member Rest  : 'tr) t)
           )
        System.Tuple<_,_,_,_,_,_,_,_>(x, x1, x2, x3, x4, x5, x6, Cons.Invoke xr x7)
    static member (!) (()                    ) = fun x -> Tuple x
    static member (!) (x1:Tuple<_>           ) = fun x -> (x,x1.Item1)
    static member (!) ((x1,x2)               ) = fun x -> (x,x1,x2)
    static member (!) ((x1,x2,x3)            ) = fun x -> (x,x1,x2,x3)
    static member (!) ((x1,x2,x3,x4)         ) = fun x -> (x,x1,x2,x3,x4)
    static member (!) ((x1,x2,x3,x4,x5)      ) = fun x -> (x,x1,x2,x3,x4,x5)
    static member (!) ((x1,x2,x3,x4,x5,x6)   ) = fun x -> (x,x1,x2,x3,x4,x5,x6)
    static member (!) ((x1,x2,x3,x4,x5,x6,x7)) = fun x -> (x,x1,x2,x3,x4,x5,x6,x7)

let inline (|Cons|) tuple   = Uncons.Invoke tuple

type Rev = Rev with
    static member inline Invoke tuple = ($) Rev tuple ()
    static member inline ($) (Rev, Cons(h,t)) = fun ac -> ($) Rev t (Cons.Invoke ac h)
    static member        ($) (Rev, ()       ) = id

type ZipOption = ZipOption with
    static member inline Loop (tup: ^a when ^a : not struct) =
        fun acc ->
            let tHead, tRest = Uncons.Invoke tup
            let nextAcc =
                match acc, tHead with
                | Some t, Some x -> Some (Cons.Invoke t x)
                | _, _ -> None
            loop ZipOption tRest nextAcc
    static member inline Loop (()) = fun acc -> acc
    static member inline Invoke tuple = 
        match loop ZipOption tuple (Some ()) with
        | Some x -> Some (Rev.Invoke x)
        | None -> None
        
let inline zipN_Srtp tuple = ZipOption.Invoke tuple

let zipN_Reflection tuple =
    let read a =
        let ty = typedefof<option<_>>
        if obj.ReferenceEquals(a, null) then None
        else
            let aty = a.GetType()
            let v = aty.GetProperty("Value")
            if aty.IsGenericType && aty.GetGenericTypeDefinition() = ty then
              if a = null then None
              else Some(v.GetValue(a, [| |]))
            else None
    let arrayToTuple a =
        let types = a |> Array.map (fun o -> o.GetType())
        let tupleType = Microsoft.FSharp.Reflection.FSharpType.MakeTupleType types
        Microsoft.FSharp.Reflection.FSharpValue.MakeTuple (a, tupleType)

    let a = Microsoft.FSharp.Reflection.FSharpValue.GetTupleFields tuple
    let res = a |> Array.choose read
    if Array.length res = Array.length a then Some (arrayToTuple res) else None
    |> Option.map unbox



// computation expressions

type ResultBuilder_srtp() = 
    member inline _.MergeSourcesN tupleOfOptions = zipN_Srtp tupleOfOptions
    member        _.BindReturn (x: 'T option, f) = Option.map f x

type ResultBuilder_reflection() = 
    member        _.MergeSourcesN tupleOfOptions = zipN_Reflection tupleOfOptions
    member        _.BindReturn (x: 'T option, f) = Option.map f x

let option_srtp = ResultBuilder_srtp()
let option_reflection = ResultBuilder_reflection()


let testSrtp r1 r2 r3 r4 r5 r6 r7 r8 r9 r10 =
    let res1: _ option =
        option_srtp { 
            let! a = r1
            and! b = r2
            and! c = r3
            and! d = r4
            and! e = r5
            and! f = r6
            and! g = r7
            and! h = r8
            and! i = r9
            and! j = r10
            return a + b - c + d - e - f - g - h - i + j
        }
    
    match res1 with
    | Some x -> sprintf "Result is: %d" x
    | None   -> "No result"


let testReflection r1 r2 r3 r4 r5 r6 r7 r8 r9 r10 =
    let res1: _ option =
        option_reflection { 
            let! a = r1
            and! b = r2
            and! c = r3
            and! d = r4
            and! e = r5
            and! f = r6
            and! g = r7
            and! h = r8
            and! i = r9
            and! j = r10
            return a + b - c + d - e - f - g - h - i + j
        }

    match res1 with
    | Some x -> sprintf "Result is: %d" x
    | None   -> "No result"

let a1, a2, a3, a4, a5, a6, a7, a8, a9, a10 = (Some 1, Some 2, Some 3, Some 4, Some 5, Some 6, Some 7, Some 8, Some 9, Some 10)

let res1 = testSrtp a1 a2 a3 a4 a5 a6 a7 a8 a9 a10
let res2 = testReflection a1 a2 a3 a4 a5 a6 a7 a8 a9 a10

if res1 <> "Result is: -21" then failwithf "Unexpected result for testSrtp. Expected was -21 but actual was %s" res1
if res2 <> "Result is: -21" then failwithf "Unexpected result for testSrtp. Expected was -21 but actual was %s" res2

()

        """
        CompilerAssert.CompileExeAndRunWithOptions [| "/langversion:preview" |] source