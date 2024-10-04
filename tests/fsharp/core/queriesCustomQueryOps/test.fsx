// #Quotations #Query
#if TESTS_AS_APP
module Core_queriesCustomQueryOps
#endif


#nowarn "57"

open System
open System.Linq
open System.Collections
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.ExprShape
open Microsoft.FSharp.Linq.RuntimeHelpers

[<AutoOpen>]
module Infrastructure =
    let failures = ref []

    let report_failure (s : string) = 
        stderr.Write" NO: "
        stderr.WriteLine s
        failures := !failures @ [s]



    let check  s v1 v2 = 
       if v1 = v2 then 
           printfn "test %s...passed " s 
       else 
           report_failure (sprintf "test %s...failed, expected %A got %A" s v2 v1)

    let test s b = check s b true


module ShapesOfQueryQuotations =
    open Microsoft.FSharp.Quotations
    open Microsoft.FSharp.Quotations.Patterns
    open Microsoft.FSharp.Quotations.DerivedPatterns
    open Microsoft.FSharp.Linq
    /// Detect a call to query.Run
    let (|CallQueryRunAsEnumerable|_|) = function SpecificCall <@ (fun (query:QueryBuilder,x:Expr<QuerySource<_,IEnumerable>>) -> query.Run(x)) @>(None, [_],[_;e]) -> Some e | _ -> None
    let (|CallQueryRunAsValue|_|) = function SpecificCall <@ (fun (query:QueryBuilder,x:Expr<int>) -> query.Run(x)) @>(None, [_],[_;e]) -> Some e | _ -> None
    let (|CallQueryRunAsQueryable|_|) = function SpecificCall <@ (fun (query:QueryBuilder,x:Expr<QuerySource<_,IQueryable>>) -> query.Run(x)) @>(Some _, [_],[e]) -> Some e | _ -> None
    /// Detect a use of query { ... }
    let (|QuerySyntaxForEnumerable|_|) = function Application(Lambda(_,CallQueryRunAsEnumerable e),_) -> Some e | _ -> None
    let (|QuerySyntaxForQueryable|_|) = function Application(Lambda(_,CallQueryRunAsQueryable e),_) -> Some e | _ -> None
    let (|QuerySyntaxForValue|_|) = function Application(Lambda(_,CallQueryRunAsValue e),_) -> Some e | _ -> None
    /// Detect a call to query.For
    let (|CallQueryFor|_|) = function SpecificCall <@ (fun (query:QueryBuilder) -> query.For) @>(_, [_;_;_;_],[sq;b]) -> Some (sq,b) | _ -> None


    /// REVIEW: there is an extra 'let' inserted here. This feels unfortunate. I presume it's
    /// there in F# 2.0 quotations for 'async' etc.
    let (|OuterFor|_|) = function CallQueryFor(sq,Lambda(v,Let(v2,Var ev,e))) when v = ev -> Some (v2,sq,e) | _ -> None

    /// Detect a use of 'yield' inside a query
    let (|SourceEnumerable|_|) = function SpecificCall <@ (fun (query:QueryBuilder,x:seq<int>) -> query.Source x) @>(_, [_],[e]) -> Some e | _ -> None
    let (|Yield|_|) = function SpecificCall <@ (fun (query:QueryBuilder) -> query.Yield) @>(_, [_;_],[e]) -> Some e | _ -> None

    /// Detect a use of 'for .. in do yield x' inside a query
    let (|FinalFor|_|) = function OuterFor(v,sq,Yield e) -> Some (v,sq,e) | _ -> None

    let (|Empty|_|) = function SpecificCall <@ (fun (query:QueryBuilder) -> query.Zero) @>(_,_,_) -> Some () | _ -> None
    let (|Pipe|_|) = function SpecificCall <@ (|>) @>(_,_,[x;f]) -> Some (x,f) | _ -> None
    let (|CallDistinct|_|) = function SpecificCall <@ (fun (query:QueryBuilder) -> query.Distinct)@>(_,_,[x]) -> Some x | _ -> None
    let (|CallSelect|_|) = function SpecificCall <@ (fun (query:QueryBuilder) -> query.Select) @>(_,_,[x;y]) -> Some (x,y) | _ -> None

        // TODO: check nested .Max(), .Min(), .Distinct() etc. on IQueryable sources execute on the database. I think this will _not_
        // be the case as the type-based resolution of these will be as IEnumerable calls.

    test "vrenjkr90kj1" 
       (match <@ query { for x in [1] -> x } @> with 
        | QuerySyntaxForEnumerable(Quote(FinalFor(v,SourceEnumerable (Coerce(sq,_)),res))) when sq = <@@ [1] @@> && res = Expr.Var(v) -> true
        // The following are for debugging this test in case something goes wrong....
        | QuerySyntaxForEnumerable(Quote(FinalFor(v,SourceEnumerable sq,res))) -> printfn "SourceEnumerable found, sq = %A" sq; false
        | QuerySyntaxForEnumerable(Quote(FinalFor(v,sq,res))) -> printfn "FinalFor found, sq = %A" sq; false
        | QuerySyntaxForEnumerable(Quote(OuterFor(v,sq,res))) -> printfn "OuterFor found, v = %A, res = %A, sq = %A" v res sq; false
        | QuerySyntaxForEnumerable(Quote(CallQueryFor(sq,b))) -> printfn "CallQueryFor found, sq = %A, b = %A" sq b; false
        | QuerySyntaxForEnumerable(Quote(sq)) -> printfn "QuerySyntaxForEnumerable(_), sq = %A" sq; false 
        | sq -> printfn "tm = %A" sq; false) 

    test "vrenjkr90kj2" 
       (match <@ query { for x in [1] do yield x } @> with 
        | QuerySyntaxForEnumerable(Quote(FinalFor(v,SourceEnumerable(Coerce(sq,_)),res))) when sq = <@@ [1] @@> && res = Expr.Var(v) -> true
        | sq -> printfn "tm = %A" sq; false) 

    test "vrenjkr90kj3" 
       (match <@ query { for x in [1] do for y in [2] do yield x } @> with 
        | QuerySyntaxForEnumerable(Quote(OuterFor(v1,SourceEnumerable(Coerce(sq1,_)),FinalFor(v2,SourceEnumerable(Coerce(sq2,_)),res)))) when sq1 = <@@ [1] @@> && sq2 = <@@ [2] @@> && res = Expr.Var(v1) -> true
        | sq -> printfn "tm = %A" sq; false) 

    test "vrenjkr90kj5" 
       (match <@ query { if true then yield 1 } @> with 
        | QuerySyntaxForQueryable(Quote(IfThenElse(Bool true,Yield(Int32 1),Empty))) -> true
        | sq -> printfn "tm = %A" sq; false) 

    test "vrenjkr90kj7" 
       (match <@ query { for x in [1] do distinct } @> with 
        | QuerySyntaxForEnumerable(Quote(CallDistinct(OuterFor(vx,SourceEnumerable(Coerce(sq,_)),Yield (Var res))))) when vx = res && sq = <@@ [1] @@>  && res = vx -> true
        | sq -> printfn "tm = %A" sq; false) 

    test "vrenjkr90kj8" 
       (match <@ query { for x in [1] do select x } @> with 
        //| QuerySyntaxForEnumerable(Quote(OuterFor(vx,SourceEnumerable(Coerce(sq,_)),Yield (Var res)))) when vx = res && sq = <@@ [1] @@>  && res = vx -> true
        | QuerySyntaxForEnumerable(Quote(CallSelect(OuterFor(vx,SourceEnumerable(Coerce(sq,_)),Yield (Var res)), Lambda(v4,Var v4b)))) when v4 = v4b && vx = res && sq = <@@ [1] @@>  && res = vx -> true
        | sq -> printfn "tm = %A" sq; false) 

module QuerySyntaxForEnumerableForAnFSharpType = 


    type EventBuilder() = 
        member __.For(ev:IObservable<'T>, loop:('T -> #IObservable<'U>)) : IObservable<'U> = failwith ""
        member __.Yield(v:'T) : IObservable<'T> = failwith ""
        member __.Quote(v:Quotations.Expr<'T>) : Expr<'T> = v
        member __.Run(x:Expr<'T>) = Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.EvaluateQuotation x :?> 'T
         
        [<CustomOperation("where",MaintainsVariableSpace=true)>]
        member __.Where (x, [<ProjectionParameter>] f) = Observable.filter f x
         
        [<CustomOperation("select")>]
        member __.Select (x, [<ProjectionParameter>] f) = Observable.map f x

        [<CustomOperation("choose")>]
        member __.Choose (x, [<ProjectionParameter>] f) = Observable.choose f x

        [<CustomOperation("joinLikeOp",IsLikeJoin=true,JoinConditionWord="squeeze")>]
        member __.JoinLikeOp (outerSource:IObservable<'Outer>,  innerSource:IObservable<'Inner>,  outerKeySelector:('Outer -> 'Key),  innerKeySelector:('Inner -> 'Key),  resultSelector:('Outer -> 'Inner -> 'Result)) : IObservable<'Result>
            = failwith ""

        [<CustomOperation("groupJoinLikeOp",IsLikeGroupJoin=true)>]
        member __.GroupJoinLikeOp (outerSource:IObservable<'Outer>,  innerSource:IObservable<'Inner>,  outerKeySelector:('Outer -> 'Key),  innerKeySelector:('Inner -> 'Key),  resultSelector:('Outer -> IObservable<'Inner> -> 'Result)) : IObservable<'Result>
            = failwith ""

        [<CustomOperation("zipLikeOp",IsLikeZip=true)>]
        member __.ZipLikeOp (outerSource:IObservable<'Outer>,  innerSource:IObservable<'Inner>,  resultSelector:('Outer -> 'Inner -> 'Result)) : IObservable<'Result>
            = failwith ""

    // intrinsic extension member
    type EventBuilder with 
        [<CustomOperation("action",MaintainsVariableSpace=true)>]
        member __.Action (source:IObservable<_>, [<ProjectionParameter>] f)  = Observable.add f source; source

    // extrinsic extension member
    /// Check that a custom operator defined by an extension member works
    [<AutoOpen>]
    module ExtensionMembers = 
        type EventBuilder with 
            [<CustomOperation("scanSumBy")>]
            member inline __.ScanSumBy (source, [<ProjectionParameter>] f : 'T -> 'U) : IObservable<'U> = Observable.scan (fun a b -> a + f b) LanguagePrimitives.GenericZero<'U> source

            [<CustomOperation("scanSumByWithInto",AllowIntoPattern=true) >]
            member inline __.ScanSumByWithInto (source, [<ProjectionParameter>] f : 'T -> 'U) : IObservable<'U> = Observable.scan (fun a b -> a + f b) LanguagePrimitives.GenericZero<'U> source

            [<CustomOperation("customOpWithTwoArgs")>]
            member inline __.CustomOpWithTwoArgs (source, [<ProjectionParameter>] f1 : 'T -> 'U, [<ProjectionParameter>] f2 : 'T -> 'U) : IObservable<'U> = Observable.scan (fun a b -> a + f1 b + f2 b) LanguagePrimitives.GenericZero<'U> source

            [<CustomOperation("customOpWithThreeArgs")>]
            member inline __.CustomOpWithThreeArgs (source, [<ProjectionParameter>] f1 : 'T -> 'U, [<ProjectionParameter>] f2 : 'T -> 'U, [<ProjectionParameter>] f3 : 'T -> 'U) : IObservable<'U> = Observable.scan (fun a b -> a + f1 b + f2 b + f3 b) LanguagePrimitives.GenericZero<'U> source

            [<CustomOperation("customOpWithFourArgs")>]
            member inline __.CustomOpWithFourArgs (source, [<ProjectionParameter>] f1 : 'T -> 'U, [<ProjectionParameter>] f2 : 'T -> 'U, [<ProjectionParameter>] f3 : 'T -> 'U, [<ProjectionParameter>] f4 : 'T -> 'U) : IObservable<'U> = Observable.scan (fun a b -> a + f1 b + f2 b + f3 b + f4 b) LanguagePrimitives.GenericZero<'U> source

            [<CustomOperation("customOpWithTwoArgsWithInto",AllowIntoPattern=true)>]
            member inline __.CustomOpWithTwoArgsWithInto (source, [<ProjectionParameter>] f1 : 'T -> 'U, [<ProjectionParameter>] f2 : 'T -> 'U) : IObservable<'U> = Observable.scan (fun a b -> a + f1 b + f2 b) LanguagePrimitives.GenericZero<'U> source

            [<CustomOperation("customOpWithThreeArgsWithInto",AllowIntoPattern=true)>]
            member inline __.CustomOpWithThreeArgsWithInto (source, [<ProjectionParameter>] f1 : 'T -> 'U, [<ProjectionParameter>] f2 : 'T -> 'U, [<ProjectionParameter>] f3 : 'T -> 'U) : IObservable<'U> = Observable.scan (fun a b -> a + f1 b + f2 b + f3 b) LanguagePrimitives.GenericZero<'U> source

            [<CustomOperation("customOpWithFourArgsWithInto",AllowIntoPattern=true)>]
            member inline __.CustomOpWithFourArgsWithInto (source, [<ProjectionParameter>] f1 : 'T -> 'U, [<ProjectionParameter>] f2 : 'T -> 'U, [<ProjectionParameter>] f3 : 'T -> 'U, [<ProjectionParameter>] f4 : 'T -> 'U) : IObservable<'U> = Observable.scan (fun a b -> a + f1 b + f2 b + f3 b + f4 b) LanguagePrimitives.GenericZero<'U> source

    let myquery = EventBuilder()
    let mybuilder = EventBuilder()

    //---------------------------------------------------------------

    type PretendForm() = 
    
       let mouseClickEvent = new Event<int * int >()
       let beepEvent = new Event<string>()
       member x.TriggerMouseClick = mouseClickEvent.Trigger(10,10)
       member x.MouseClick = mouseClickEvent.Publish

       member x.TriggerBeep = beepEvent.Trigger "beep"
       member x.Beep = beepEvent.Publish

    //let f = new System.Windows.Forms.Form()
    let testWhichWeDontActuallyRun() = 
        let f = new PretendForm()

        let e1 =     
          myquery { for x in f.MouseClick do 
                      where (fst x < 100)
                      scanSumBy (snd x) }

        let e2 =     
            myquery { for x in f.MouseClick do 
                      where (fst x < 100)
                      scanSumByWithInto (snd x) into clickCount
                      action (printfn "%d clicks" clickCount)}

        let e3 =     
            myquery { for x in f.MouseClick do 
                      where (fst x < 100)
                      customOpWithTwoArgs (snd x) (snd x) }

        let e3into =     
            myquery { for x in f.MouseClick do 
                      where (fst x < 100)
                      customOpWithTwoArgsWithInto (snd x) (snd x) into clickCount
                      action (printfn "%d clicks" clickCount)}

        let e4 =     
            myquery { for x in f.MouseClick do 
                      where (fst x < 100)
                      customOpWithThreeArgs (snd x) (snd x) (snd x) }
        let e4into =     
            myquery { for x in f.MouseClick do 
                      where (fst x < 100)
                      customOpWithThreeArgsWithInto (snd x) (snd x) (snd x) into clickCount
                      action (printfn "%d clicks" clickCount)}

        let e5 =     
            myquery { for x in f.MouseClick do 
                      where (fst x < 100)
                      customOpWithFourArgs (snd x) (snd x) (snd x) (snd x) }

        let e5into =     
            myquery { for x in f.MouseClick do 
                      where (fst x < 100)
                      customOpWithFourArgsWithInto (snd x) (snd x) (snd x) (snd x) into clickCount
                      action (printfn "%d clicks" clickCount)}


        let join1 =     
            myquery { for x in f.MouseClick do 
                      joinLikeOp y in f.Beep squeeze (string (fst x) = y) 
                      select (x,y) }

        let join2 =     
            myquery { for x in f.MouseClick do 
                      groupJoinLikeOp y in f.Beep on (string (fst x) = y) into group
                      select (x,group) }
        let join3 =     
            myquery { for x in f.MouseClick do 
                      zipLikeOp beep in f.Beep 
                      select (x,beep) }
        ()

module LessRestrictionsForCustomOperators_Bug475766 =
    type DoNothingBuilder() = 
        member this.Yield(v) = [v]
        member this.Delay f = f
        member this.Run(f) = f()
        member this.Zero() = []
        member this.For(e, b) = List.collect b e
        member this.TryWith(b, h) = 
            try b()
            with e -> h e
        member this.TryFinally(b, c) = try b() finally c()
        [<CustomOperation("select")>]
        member this.Select(v, [<ProjectionParameter>]f) = List.map f v 
         
    let builder = DoNothingBuilder()

    // allow if\then\else if 'then' and 'else' branches doesn't contain custom operators
    let q1 = 
        builder {
            for c in [1;2;3] do
                if true then
                    yield (c,1)
                else
                    yield (c,0)
            }
    check "LessRestrictionsForCustomOperators_Bug475766_1" q1 [1,1; 2,1; 3,1]

    // allows try\with if nor body of 'try' neither handler contain custom operators
    let q2 = 
        builder {
            try failwith "error"
            with Failure e -> yield e
        }
    check "LessRestrictionsForCustomOperators_Bug475766_2" q2 ["error"]

    // allows try\finally if nor body of 'try' neither compensation action contain custom operators
    let compensation = ref false
    let q3 = 
        builder {
            try
                yield 1
            finally
                compensation := true
        }
    check "LessRestrictionsForCustomOperators_Bug475766_3" (q3, !compensation) ([1], true)

module ProbabilityWorkflow = 
    
    type Distribution<'T when 'T : comparison> =
        abstract Sample : 'T
        abstract Support : Set<'T>
        abstract Expectation: ('T -> float) -> float
    
    let always x =
        { new Distribution<'T> with
             member d.Sample = x
             member d.Support = Set.singleton x
             member d.Expectation(H) = H(x) }
    
    let rnd = System.Random()
    
    let coinFlip (p:float) (d1:Distribution<'T>) (d2:Distribution<'T>) =
        if p < 0.0 || p > 1.0 then failwith "invalid probability in coinFlip"
        { new Distribution<'T> with
             member d.Sample =
                 if rnd.NextDouble() < p then d1.Sample else d2.Sample
             member d.Support = Set.union d1.Support d2.Support
             member d.Expectation(H) =
                 p * d1.Expectation(H) + (1.0-p) * d2.Expectation(H) }
    
    // ----------------------------
    // Listing 9-8.
    
    let bind (dist:Distribution<'T>) (k: 'T -> Distribution<'b>) =
        { new Distribution<'b> with
             member d.Sample = (k(dist.Sample)).Sample
             member d.Support = dist.Support |> Set.map (fun d -> (k d).Support) |> Set.unionMany
             member d.Expectation(H) = dist.Expectation(fun x -> (k x).Expectation(H)) }
    
    type DistributionBuilder() =
        member builder.Delay(f) = bind (always ()) f
        member builder.Let(v,f) = bind (always v) f
        member builder.Bind(d,f) = bind d f
        member builder.Return(x) = always x
        member builder.ReturnFrom(x) = x
        [<CustomOperation("condition",MaintainsVariableSpaceUsingBind=true)>]
        member builder.Condition(x:Distribution<_>,[<ProjectionParameter>] p) = 
              { new Distribution<_> with
                   member d.Sample = let rec loop() = let v = x.Sample in if p v then v else loop() in loop() 
                   member d.Support = x.Support
                   member d.Expectation(H) = failwith "can't compute expectation of condition" }
    
    let dist = new DistributionBuilder()
    
    // ----------------------------
    // Listing 9-9.
    
    let weightedCases (inp: ('a * float) list) =
        let rec coinFlips w l =
            match l with
            | []          -> failwith "no coinFlips"
            | [(d,_)]     -> always d
            | (d,p)::rest -> coinFlip (p/(1.0-w)) (always d) (coinFlips (w+p) rest)
        coinFlips 0.0 inp
    
    let countedCases inp =
        let total = List.sumBy (fun (_,v) -> v) inp
        weightedCases (inp |> List.map (fun (x,v) -> (x,(float v/float total))))
    
    type Outcome = Even | Odd | Zero
    let roulette = countedCases [ Even,18; Odd,18; Zero,1]
    
    
    // ----------------------------
    
    roulette.Sample
    
    roulette.Sample
    
    roulette.Expectation (function Even -> 10.0 | Odd -> 0.0 | Zero -> 0.0)
    
    // ----------------------------
    
    type Light =
        | Red
        | Green
        | Yellow
    
    let trafficLightD = weightedCases [ Red,0.50; Yellow,0.10; Green, 0.40 ]
    
    type Action = Stop | Drive
    
    let cautiousDriver light =
        dist { match light with
               | Red -> return Stop
               | Yellow -> return! weightedCases [ Stop, 0.9; Drive, 0.1 ]
               | Green -> return Drive }
    
    let aggressiveDriver light =
        dist { match light with
               | Red    -> return! weightedCases [ Stop, 0.9; Drive, 0.1 ]
               | Yellow -> return! weightedCases [ Stop, 0.1; Drive, 0.9 ]
               | Green  -> return Drive }
    
    let otherLight light =
        match light with
        | Red -> Green
        | Yellow -> Red
        | Green -> Red
    
    type CrashResult = Crash | NoCrash
    
    let crash(driverOneD,driverTwoD,lightD) =
        dist { // Sample from the traffic light
               let! light = lightD
    
               // Sample the first driver's behavior given the traffic light
               let! driverOne = driverOneD light
    
               // Sample the second driver's behavior given the traffic light
               let! driverTwo = driverTwoD (otherLight light)
    
               // Condition that the drivers make the same choice
               condition (driverOne = driverTwo)
               
               return (driverOne, driverTwo) }
    
    let model = crash(cautiousDriver,aggressiveDriver,trafficLightD)
    
    // ----------------------------
    
    for i = 0 to 100 do 
       let (v1,v2) = model.Sample
       printfn "sample #%d = %A" i (v1,v2)
       // Every sample should be identical on left and right because of the condition
       check "cwnew0" v1 v2


    let crash2(driverOneD,driverTwoD,lightD) =
        dist { // Sample from the traffic light
               let! light = lightD
    
               // Sample the first driver's behavior given the traffic light
               let! driverOne = driverOneD light
    
               // Sample the second driver's behavior given the traffic light
               let! driverTwo = driverTwoD (otherLight light)
    
               // Condition that the drivers make the same choice, twice
               condition (driverOne = driverTwo)
               condition (driverOne = driverTwo)
               
               return (driverOne, driverTwo) }
    
    let model2 = crash2(cautiousDriver,aggressiveDriver,trafficLightD)
    
    // ----------------------------
    
    for i = 0 to 100 do 
       let (v1,v2) = model2.Sample
       printfn "sample #%d = %A" i (v1,v2)
       // Every sample should be identical on left and right because of the condition
       check "cwnew0" v1 v2
       
#if TESTS_AS_APP
let RUN() = !failures
#else
let aa =
  match !failures with 
  | [] -> 
      stdout.WriteLine "Test Passed"
      printf "TEST PASSED OK" ;
      exit 0
  | _ -> 
      stdout.WriteLine "Test Failed"
      exit 1
#endif

