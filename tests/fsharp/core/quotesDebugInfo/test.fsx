type E = Microsoft.FSharp.Quotations.Expr
type V = Microsoft.FSharp.Quotations.Var
type FST = Microsoft.FSharp.Reflection.FSharpType

module P = Microsoft.FSharp.Quotations.Patterns
module DP = Microsoft.FSharp.Quotations.DerivedPatterns
module Shape = Microsoft.FSharp.Quotations.ExprShape

open System.CodeDom.Compiler

let toString (e : E) = 
    let sw = new System.IO.StringWriter()
    let w = new IndentedTextWriter(sw)
    
    let tab() = w.Indent <- w.Indent + 1
    let newline() = w.WriteLine()
    let untab(newLine) = 
        w.Indent <- w.Indent - 1
        if newLine then newline()

    let print fmt = Printf.fprintf w fmt
    let println fmt = Printf.fprintfn w fmt

    let rec go (e : E, newLineAfterAttr) = 
        let isWrapped =
            match e.CustomAttributes with
            | [P.NewTuple([_; P.NewTuple([file; P.Value(sl, _); P.Value(sc, _); P.Value(el, _); P.Value(ec, _)])])] -> 
                print "[DebugRange(%O:%O - %O:%O)] <{ " sl sc el ec
                if newLineAfterAttr then newline()
                true                
            | _ -> false

        match e with
        | P.Var(v) -> print "%s" v.Name
        | P.Value(v, _) -> print "%A" v
        | P.NewRecord(ty, args) ->
            let fields = FST.GetRecordFields(ty)
            print "new %s {" ty.Name
            tab()
            (fields, args) 
            ||> Seq.zip 
            |> Seq.iteri (fun i (f, v) ->
                if i <> 0 then
                    println "," 
                print "%s = ( " f.Name
                go(v, false)
                print ")"            )            
            untab(true)
            println "}"
        | DP.SpecificCall <@ (=) @>(_, _, [a; b]) ->
            go(a, false)
            print " = "
            go(b, false)
        | DP.SpecificCall <@ (-) @> (_, _, [a; b]) ->
            go(a, false)
            print " - "
            go(b, false)
        | DP.SpecificCall <@ (*) @> (_, _, [a; b]) ->
            go(a, false)
            print " * "
            go(b, false)
        | DP.SpecificCall <@ (+) @> (_, _, [a; b]) ->
            go(a, false)
            print " + "
            go(b, false)
        | DP.SpecificCall <@ ignore @>(_, _, [a]) -> 
            print "ignore"
            go(a, false)
            print ")"
        | P.Let(var, value, body) -> 
            println "let %s : %s = " var.Name var.Type.Name
            tab()
            go(value, true)
            newline()
            println "in"
            go(body, true)
            untab(false)
        | P.LetRecursive([var, value], body) -> 
            println "let rec %s : %s = " var.Name var.Type.Name
            tab()
            go(value, true)
            newline()
            println "in"
            go(body, true)
            untab(false)
        | P.PropertyGet(Some inst, pi, _) -> 
            go(inst, false)
            print ".%s" pi.Name
        | P.NewObject(ci, args) ->
            print "new %s (" ci.DeclaringType.Name
            if List.length args > 0 then
                tab()
                newline()
                args
                |> Seq.iteri (fun i v ->
                    if i <> 0 then
                        println "," 
                    go(v, false)
                )            
                untab(true)
            println ")"
        | P.NewArray(ty, args) ->
            println "new %s [" ty.Name
            if List.length args > 0 then
                tab()
                args |> Seq.iteri (fun i arg ->
                    if i <> 0 then
                        println "," 
                    go(arg, true)
                )
                untab(true)
            println "]"
        | P.Call(_, mi, args) -> 
            println "%s (" mi.Name
            if List.length args > 0 then
                tab()
                args |> Seq.iteri (fun i arg ->
                    if i <> 0 then
                        println "," 
                    go(arg, true)
                )
                untab(true)
            println ")"
        | P.Application(app, arg) ->
            go(app, false)
            println "("
            tab()
            go(arg, false)
            untab(true)
            println ")"
        | P.WhileLoop(cond, body) ->
            println "while("
            go(cond, false)
            println ") {"
            tab()
            go(body, true)
            untab(true)
            println "}"
        | P.ForIntegerRangeLoop(var, s, e, body) -> 
            print "for("
            go (E.Var var, false)
            print " in "
            go(s, false)
            print ".."
            go(e, false)
            println " {"
            tab()
            go(body, true)
            untab(true)
            println "}"
        | P.Lambda(var, body) ->
            println "(fun %s : %s -> " var.Name var.Type.Name
            tab()
            go(body, true)
            untab(true)
            println ")"
        | P.IfThenElse(cond, ifTrue, ifFalse) ->
            println "if ("
            go(cond, false)
            println ") {"
            tab()
            go(ifTrue, true)
            untab(true)
            println "} else {"
            tab()
            go(ifFalse, true)
            untab(true)
            println "}"
        | P.UnionCaseTest(e, ucase) -> 
            print "UnionCaseTest ("
            go(e, false)
            print ") is %s" ucase.Name
        | P.NewUnionCase(ucase, args) ->
            print "%s" ucase.Name
            if List.length args > 0 then
                tab()
                newline()
                args
                |> Seq.iteri (fun i v ->
                    if i <> 0 then
                        println "," 
                    go(v, false)
                )            
                untab(true)
            println ")"
        | P.Sequential(a, b) ->
            go(a, false)
            newline()
            go(b, false)
        | P.PropertySet(Some inst, pi, _, value) ->
            go(inst, false)
            print ".%s <-" pi.Name
            go(value, false)
        | P.FieldGet(Some inst, fi) ->
            go(inst, false)
            print ".%s" fi.Name
        | P.FieldSet(Some inst, fi, value) ->
            go(inst, false)
            print ".%s <-" fi.Name
            go(value, false)
        | x -> failwithf "unexpected %A" x
        if isWrapped then print " }>"
        
    go(e, true)
    sw.ToString()

let normalize (s : string) = 
    s.Replace("\r", "")
     .Split([|'\n'|], System.StringSplitOptions.RemoveEmptyEntries) 
     |> Array.map (fun s -> s.Trim()) |> String.concat "\n"

let foo a b c = ignore(); fun d -> a + b + c + d
type A = {x : int}
type B(x : int) = 
    member this.X = x

let mutable failures = ref 0
let test caption (baseLine : string) e = 
    let got = toString e
    let expected = normalize baseLine
    if expected <> (normalize got) then
        printf "%s failed, normalized baseline %s, normalized result %s" caption expected got
        // save non-normalized result with tabs for better readability
        System.IO.File.WriteAllText(System.IO.Path.Combine(__SOURCE_DIRECTORY__, caption) + ".actual", got)
        incr failures
do
    let q = 
        <@ 
            1 + 2
        @>
    let baseLine = """
[DebugRange(229:12 - 229:17)] <{ 
[DebugRange(229:12 - 229:13)] <{ 1 }> + [DebugRange(229:16 - 229:17)] <{ 2 }> }>"""    
    test "test1" baseLine q

do
    let q = 
        <@ 
            let y = fun a -> a + 1
            for x in 1..10 do
                ignore (y x)
        @>
    let baseLine = """
[DebugRange(239:16 - 239:17)] <{ 
let y : FSharpFunc`2 = 
    [DebugRange(239:20 - 239:34)] <{ 
    (fun a : Int32 -> 
        [DebugRange(239:29 - 239:34)] <{ 
        [DebugRange(239:29 - 239:30)] <{ a }> + [DebugRange(239:33 - 239:34)] <{ 1 }> }>
    )
     }>
    in
    [DebugRange(240:12 - 241:28)] <{ 
    for(x in [DebugRange(240:21 - 240:22)] <{ 1 }>..[DebugRange(240:24 - 240:26)] <{ 10 }> {
        [DebugRange(241:16 - 241:28)] <{ 
        ignore[DebugRange(241:24 - 241:27)] <{ [DebugRange(241:24 - 241:25)] <{ y }>(
            [DebugRange(241:26 - 241:27)] <{ x }>
        )
         }>) }>
    }
     }> }>"""
    test "test2" baseLine q

do
    let q =
         <@
            [| {x = 5}; {x = 1} |]
         @>

    let baseLine = """
[DebugRange(267:12 - 267:34)] <{ 
new A [
    [DebugRange(267:15 - 267:22)] <{ 
    new A {x = ( [DebugRange(267:20 - 267:21)] <{ 5 }>)
    }
     }>,
    [DebugRange(267:24 - 267:31)] <{ 
    new A {x = ( [DebugRange(267:29 - 267:30)] <{ 1 }>)
    }
     }>
]
 }>"""    
    test "test3" baseLine q

do
    let q =
         <@
            [| B(5); B(10) |]
         @>
    let baseLine = """
[DebugRange(288:12 - 288:29)] <{ 
new B [
    [DebugRange(288:15 - 288:19)] <{ 
    new B (
        [DebugRange(288:17 - 288:18)] <{ 5 }>
    )
     }>,
    [DebugRange(288:21 - 288:26)] <{ 
    new B (
        [DebugRange(288:23 - 288:25)] <{ 10 }>
    )
     }>
]
 }>"""
    test "test4" baseLine q
do
    let q =
        <@
            let x = 5
            match x with
            | 10 -> "a"
            | 15 -> "b"
            | _ -> "c"
        @>
    let baseLine = """
[DebugRange(309:16 - 309:17)] <{ 
let x : Int32 = 
    [DebugRange(309:20 - 309:21)] <{ 
    5 }>
    in
    [DebugRange(310:18 - 310:19)] <{ 
    if (
    [DebugRange(310:18 - 310:19)] <{ [DebugRange(310:18 - 310:19)] <{ x }> = [DebugRange(310:18 - 310:19)] <{ 10 }> }>) {
        [DebugRange(311:20 - 311:23)] <{ 
        "a" }>
    } else {
        [DebugRange(310:18 - 310:19)] <{ 
        if (
        [DebugRange(310:18 - 310:19)] <{ [DebugRange(310:18 - 310:19)] <{ x }> = [DebugRange(310:18 - 310:19)] <{ 15 }> }>) {
            [DebugRange(312:20 - 312:23)] <{ 
            "b" }>
        } else {
            [DebugRange(313:19 - 313:22)] <{ 
            "c" }>
        }
         }>
    }
     }> }>"""    
    test "test5" baseLine q

do
    let q =
        <@
            let foo x = 
                match x with
                | Some v -> v + 10
                | None -> 100
            foo (Some 5)
        @>
    let baseLine = """
[DebugRange(344:16 - 344:19)] <{ 
let foo : FSharpFunc`2 = 
    [DebugRange(345:16 - 347:29)] <{ 
    (fun x : FSharpOption`1 -> 
        [DebugRange(345:22 - 345:23)] <{ 
        if (
        UnionCaseTest ([DebugRange(345:22 - 345:23)] <{ x }>) is None) {
            [DebugRange(347:26 - 347:29)] <{ 
            100 }>
        } else {
            [DebugRange(346:28 - 346:34)] <{ 
            let v : Int32 = 
                [DebugRange(346:28 - 346:34)] <{ 
                [DebugRange(345:22 - 345:23)] <{ x }>.Value }>
                in
                [DebugRange(346:28 - 346:34)] <{ 
                [DebugRange(346:28 - 346:29)] <{ v }> + [DebugRange(346:32 - 346:34)] <{ 10 }> }> }>
        }
         }>
    )
     }>
    in
    [DebugRange(348:12 - 348:24)] <{ 
    [DebugRange(348:12 - 348:15)] <{ foo }>(
        [DebugRange(348:17 - 348:23)] <{ Some
            [DebugRange(348:22 - 348:23)] <{ 5 }>
        )
         }>
    )
     }> }>"""
    
    test "test6" baseLine q


do
    let q =
        <@
            let a = 5
            for x in a..10 do
                ignore (a + 5)
                ignore ("v")
        @>
    let baseLine = """
[DebugRange(388:16 - 388:17)] <{ 
let a : Int32 = 
    [DebugRange(388:20 - 388:21)] <{ 
    5 }>
    in
    [DebugRange(389:12 - 391:28)] <{ 
    for(x in [DebugRange(389:21 - 389:22)] <{ a }>..[DebugRange(389:24 - 389:26)] <{ 10 }> {
        [DebugRange(390:16 - 391:28)] <{ 
        [DebugRange(390:16 - 390:30)] <{ ignore[DebugRange(390:24 - 390:29)] <{ [DebugRange(390:24 - 390:25)] <{ a }> + [DebugRange(390:28 - 390:29)] <{ 5 }> }>) }>
        [DebugRange(391:16 - 391:28)] <{ ignore[DebugRange(391:24 - 391:27)] <{ "v" }>) }> }>
    }
     }> }>"""    
    test "test7" baseLine q

do
    let q =
        <@
            let rec fact i = if i < 2 then 1 else i * fact(i - 1)
            fact 5
        @>
    let baseLine = """
[DebugRange(411:12 - 412:18)] <{ 
let rec fact : FSharpFunc`2 = 
    [DebugRange(411:29 - 411:65)] <{ 
    (fun i : Int32 -> 
        [DebugRange(411:29 - 411:65)] <{ 
        if (
        [DebugRange(411:32 - 411:37)] <{ op_LessThan (
            [DebugRange(411:32 - 411:33)] <{ 
            i }>,
            [DebugRange(411:36 - 411:37)] <{ 
            2 }>
        )
         }>) {
            [DebugRange(411:43 - 411:44)] <{ 
            1 }>
        } else {
            [DebugRange(411:50 - 411:65)] <{ 
            [DebugRange(411:50 - 411:51)] <{ i }> * [DebugRange(411:54 - 411:65)] <{ [DebugRange(411:54 - 411:58)] <{ fact }>(
                [DebugRange(411:59 - 411:64)] <{ [DebugRange(411:59 - 411:60)] <{ i }> - [DebugRange(411:63 - 411:64)] <{ 1 }> }>
            )
             }> }>
        }
         }>
    )
     }>
    in
    [DebugRange(412:12 - 412:18)] <{ 
    [DebugRange(412:12 - 412:16)] <{ fact }>(
        [DebugRange(412:17 - 412:18)] <{ 5 }>
    )
     }> }>"""
    test "test8" baseLine q

do
    let q =
        <@
            let f (x : A) = x.x
            f Unchecked.defaultof<_>
        @>
    let baseLine = """
[DebugRange(451:16 - 451:17)] <{ 
let f : FSharpFunc`2 = 
    [DebugRange(451:28 - 451:31)] <{ 
    (fun x : A -> 
        [DebugRange(451:28 - 451:31)] <{ 
        [DebugRange(451:28 - 451:29)] <{ x }>.x }>
    )
     }>
    in
    [DebugRange(452:12 - 452:36)] <{ 
    [DebugRange(452:12 - 452:13)] <{ f }>(
        [DebugRange(452:14 - 452:33)] <{ DefaultOf (
        )
         }>
    )
     }> }>"""
    test "test9" baseLine q

do    
    let q =
        <@
            let x = ref 5
            x.contents
        @>
    let baseLine = """
[DebugRange(476:16 - 476:17)] <{ 
let x : FSharpRef`1 = 
    [DebugRange(476:20 - 476:25)] <{ 
    Ref (
        [DebugRange(476:24 - 476:25)] <{ 
        5 }>
    )
     }>
    in
    [DebugRange(477:12 - 477:22)] <{ 
    [DebugRange(477:12 - 477:13)] <{ x }>.contents }> }>"""
    test "test10" baseLine q

module M =    
    [<ReflectedDefinition>]
    let Func (x : B) = x.X + 1

do
    let q =
        let(P.Call(_, mi, _)) = <@ M.Func(Unchecked.defaultof<_>) @>
        Option.get (E.TryGetReflectedDefinition mi)
        
    let baseLine = """
[DebugRange(495:23 - 495:30)] <{ 
(fun x : B -> 
    [DebugRange(495:23 - 495:30)] <{ 
    [DebugRange(495:23 - 495:26)] <{ [DebugRange(495:23 - 495:24)] <{ x }>.X }> + [DebugRange(495:29 - 495:30)] <{ 1 }> }>
)
 }>"""
    test "test11" baseLine q


type R1 = { mutable f : int}
do
    let q = 
        <@
            let x = {f = 5}
            x.f <- 10
        @>
    let baseLine = """
[DebugRange(516:16 - 516:17)] <{ 
let x : R1 = 
    [DebugRange(516:20 - 516:27)] <{ 
    new R1 {f = ( [DebugRange(516:25 - 516:26)] <{ 5 }>)
    }
     }>
    in
    [DebugRange(517:12 - 517:21)] <{ 
    [DebugRange(517:12 - 517:13)] <{ x }>.f <-[DebugRange(517:19 - 517:21)] <{ 10 }> }> }>"""
    test "test12" baseLine q

type T1 = 
    val mutable x : int
    new(x) = {x = x}

do 
    let q = 
        <@
            let x = T1(100)
            x.x <- x.x - 50
        @>
    let baseLine = """
[DebugRange(538:16 - 538:17)] <{ 
let x : T1 = 
    [DebugRange(538:20 - 538:27)] <{ 
    new T1 (
        [DebugRange(538:23 - 538:26)] <{ 100 }>
    )
     }>
    in
    [DebugRange(539:12 - 539:27)] <{ 
    [DebugRange(539:12 - 539:13)] <{ x }>.x <-[DebugRange(539:19 - 539:27)] <{ [DebugRange(539:19 - 539:22)] <{ [DebugRange(539:19 - 539:20)] <{ x }>.x }> - [DebugRange(539:25 - 539:27)] <{ 50 }> }> }> }>"""
    test "test13" baseLine q


[<ReflectedDefinition>]
module ModuleWithReflectedDefinitions =
    let f1 a b = a.x + b
    
    let f2 (a : T1) b c = a.x - b + c

do
    let q = 
        let(P.Lambda(_, P.Lambda(_, P.Call(_, mi, _)))) = <@ ModuleWithReflectedDefinitions.f1 @>
        Option.get (E.TryGetReflectedDefinition mi)
    let baseLine = """
[DebugRange(557:17 - 557:24)] <{ 
(fun a : A -> 
    [DebugRange(557:17 - 557:24)] <{ 
    (fun b : Int32 -> 
        [DebugRange(557:17 - 557:24)] <{ 
        [DebugRange(557:17 - 557:20)] <{ [DebugRange(557:17 - 557:18)] <{ a }>.x }> + [DebugRange(557:23 - 557:24)] <{ b }> }>
    )
     }>
)
 }>"""
    test "test14" baseLine q

do
    let q = 
        let(P.Lambda(_, P.Lambda(_, P.Lambda(_, P.Call(_, mi, _))))) = <@ ModuleWithReflectedDefinitions.f2 @>
        Option.get (E.TryGetReflectedDefinition mi)
    let baseLine = """
[DebugRange(559:26 - 559:37)] <{ 
(fun a : T1 -> 
    [DebugRange(559:26 - 559:37)] <{ 
    (fun b : Int32 -> 
        [DebugRange(559:26 - 559:37)] <{ 
        (fun c : Int32 -> 
            [DebugRange(559:26 - 559:37)] <{ 
            [DebugRange(559:26 - 559:33)] <{ [DebugRange(559:26 - 559:29)] <{ [DebugRange(559:26 - 559:27)] <{ a }>.x }> - [DebugRange(559:32 - 559:33)] <{ b }> }> + [DebugRange(559:36 - 559:37)] <{ c }> }>
        )
         }>
    )
     }>
)
 }>"""
    test "test15" baseLine q

[<ReflectedDefinition>]
type TypeWithReflectedDefinitions (a : int) =
    let x = string a
    override this.ToString() = x

do
    let q = 
        let ci = typeof<TypeWithReflectedDefinitions>.GetConstructors() |> Seq.exactlyOne
        Option.get (E.TryGetReflectedDefinition ci)
    let baseLine = """
[DebugRange(600:5 - 600:33)] <{ 
(fun a : Int32 -> 
    [DebugRange(600:5 - 600:33)] <{ 
    [DebugRange(600:5 - 600:33)] <{ new Object ()
     }>
    [DebugRange(601:4 - 601:20)] <{ [DebugRange(601:4 - 601:20)] <{ [DebugRange(601:4 - 601:20)] <{ this }>.x <-[DebugRange(601:12 - 601:20)] <{ ToString (
        [DebugRange(601:19 - 601:20)] <{ 
        a }>
    )
     }> }>
    [DebugRange(600:5 - 600:33)] <{ <null> }> }> }>
)
 }>"""
    test "test16" baseLine q
        
do
    let q = 
        let mi = typeof<TypeWithReflectedDefinitions>.GetMethod("ToString")
        Option.get (E.TryGetReflectedDefinition mi)
    let baseLine = """
[DebugRange(602:31 - 602:32)] <{ 
(fun this : TypeWithReflectedDefinitions -> 
    [DebugRange(602:31 - 602:32)] <{ 
    (fun unitVar1 : Unit -> 
        [DebugRange(602:31 - 602:32)] <{ 
        [DebugRange(602:31 - 602:32)] <{ this }>.x }>
    )
     }>
)
 }>"""
    test "test17" baseLine q
    

#if TESTS_AS_APP
let RUN() = !failures
#else
let aa =
  match !failures with 
  | 0 -> 
      stdout.WriteLine "Test Passed"
      printf "TEST PASSED OK" ;
      exit 0
  | _ -> 
      stdout.WriteLine "Test Failed"
      exit 1
#endif

