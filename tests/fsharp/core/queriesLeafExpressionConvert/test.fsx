// #Query
#if TESTS_AS_APP
module Core_queriesLeafExpressionConvert
#endif

#nowarn "57"

open System
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

module LeafExpressionEvaluationTests = 

    // The following hopefully is an identity function on quotations:
    let Id (x: Expr<'T>) : Expr<'T> = 
        let rec conv x = 
            match x with
            | ShapeVar _ -> 
                x
            | ShapeLambda (head, body) -> 
                Expr.Lambda (head, conv body)    
            | ShapeCombination (head, tail) -> 
                RebuildShapeCombination (head, List.map conv tail)
        conv x |> Expr.Cast

    let Eval (q: Expr<'T>) : 'T = 
        LeafExpressionConverter.QuotationToExpression q |> ignore 
        LeafExpressionConverter.EvaluateQuotation q  |> unbox

    let checkText nm q s = check nm ((LeafExpressionConverter.QuotationToExpression q).ToString()) s

    let checkEval nm (q : Expr<'T>) expected = 
        check nm (Eval q) expected
        check (nm + "(after applying Id)") (Eval (Id q)) expected
        check (nm + "(after applying Id^2)")  (Eval (Id (Id q))) expected

    checkEval "2ver9ewv2" (<@ if true then 1 else 0  @>) 1
    checkEval "2ver9ewv3" (<@ if false then 1 else 0  @>) 0
    checkEval "2ver9ewv4" (<@ true && true @>) true
    checkEval "2ver9ewv5" (<@ true && false @>) false
    check "2ver9ewv6" (try Eval <@ failwith "fail" : int @> with Failure "fail" -> 1 | _ -> 0) 1 
    check "2ver9ewv7" (try Eval <@ true && (failwith "fail") @> with Failure "fail" -> true | _ -> false) true
    checkEval "2ver9ewv8" (<@ 0x001 &&& 0x100 @>) (0x001 &&& 0x100)
    checkEval "2ver9ewv9" (<@ 0x001 ||| 0x100 @>) (0x001 ||| 0x100)
    checkEval "2ver9ewvq" (<@ 0x011 ^^^ 0x110 @>) (0x011 ^^^ 0x110)
    checkEval "2ver9ewvw" (<@ ~~~0x011 @>) (~~~0x011)

    let _ = 1
    checkEval "2ver9ewve" (<@ () @>) ()
    check "2ver9ewvr1" (Eval <@ (fun x -> NonStructuralComparison.(>) x 1) @> 3) true
    check "2ver9ewvr1" (Eval <@ (fun x -> NonStructuralComparison.(>) x 4) @> 3) false
    check "2ver9ewvr1" (Eval <@ (fun x -> NonStructuralComparison.(>) x 3) @> 3) false

    check "2ver9ewvr2" (Eval <@ (fun x -> NonStructuralComparison.(<) x 1) @> 3) false
    check "2ver9ewvr2" (Eval <@ (fun x -> NonStructuralComparison.(<) x 3) @> 3) false
    check "2ver9ewvr2" (Eval <@ (fun x -> NonStructuralComparison.(<) x 4) @> 3) true

    check "2ver9ewvr2" (Eval <@ (fun x -> NonStructuralComparison.(>=) x 1) @> 3) true
    check "2ver9ewvr2" (Eval <@ (fun x -> NonStructuralComparison.(>=) x 3) @> 3) true
    check "2ver9ewvr2" (Eval <@ (fun x -> NonStructuralComparison.(>=) x 4) @> 3) false

    check "2ver9ewvr2" (Eval <@ (fun x -> NonStructuralComparison.(<=) x 1) @> 3) false
    check "2ver9ewvr2" (Eval <@ (fun x -> NonStructuralComparison.(<=) x 3) @> 3) true
    check "2ver9ewvr2" (Eval <@ (fun x -> NonStructuralComparison.(<=) x 4) @> 3) true

    check "2ver9ewvr2" (Eval <@ (fun x -> NonStructuralComparison.(=) x 3) @> 3) true
    check "2ver9ewvr2" (Eval <@ (fun x -> NonStructuralComparison.(=) x 4) @> 3) false
    check "2ver9ewvr2" (Eval <@ (fun x -> NonStructuralComparison.(=) x 1) @> 3) false

    check "2ver9ewvr2" (Eval <@ (fun x -> NonStructuralComparison.(<>) x 4) @> 3) true
    check "2ver9ewvr2" (Eval <@ (fun x -> NonStructuralComparison.(<>) x 3) @> 3) false
    check "2ver9ewvr2" (Eval <@ (fun x -> NonStructuralComparison.(<>) x 1) @> 3) true

    check "2ver9ewvr" (Eval <@ (fun x -> x + 1) @> (3)) 4
    check "2ver9ewvr" (Eval <@ (fun x -> x + 1) @> (3)) 4
    check "2ver9ewvr" (Eval <@ (fun x -> x + 1) @> (3)) 4
    check "2ver9ewvr" (Eval <@ (fun x -> x + 1) @> (3)) 4
    check "2ver9ewvt" (Eval <@ (fun (x,y) -> x + 1) @> (3,4)) 4
    check "2ver9ewvy" (Eval <@ (fun (x1,x2,x3) -> x1 + x2 + x3) @> (3,4,5)) (3+4+5)
    check "2ver9ewvu" (Eval <@ (fun (x1,x2,x3,x4) -> x1 + x2 + x3 + x4) @> (3,4,5,6)) (3+4+5+6)
    check "2ver9ewvi" (Eval <@ (fun (x1,x2,x3,x4,x5) -> x1 + x2 + x3 + x4 + x5) @> (3,4,5,6,7)) (3+4+5+6+7)
    check "2ver9ewvo" (Eval <@ (fun (x1,x2,x3,x4,x5,x6) -> x1 + x2 + x3 + x4 + x5 + x6) @> (3,4,5,6,7,8)) (3+4+5+6+7+8)
    check "2ver9ewvp" (Eval <@ (fun (x1,x2,x3,x4,x5,x6,x7) -> x1 + x2 + x3 + x4 + x5 + x6 + x7) @> (3,4,5,6,7,8,9)) (3+4+5+6+7+8+9)
    check "2ver9ewva" (Eval <@ (fun (x1,x2,x3,x4,x5,x6,x7,x8) -> x1 + x2 + x3 + x4 + x5 + x6 + x7 + x8) @> (3,4,5,6,7,8,9,10)) (3+4+5+6+7+8+9+10)
    check "2ver9ewvs" (Eval <@ (fun (x1,x2,x3,x4,x5,x6,x7,x8,x9) -> x1 + x2 + x3 + x4 + x5 + x6 + x7 + x8 + x9) @> (3,4,5,6,7,8,9,10,11)) (3+4+5+6+7+8+9+10+11)
    check "2ver9ewvd" (Eval <@ (fun (x1,x2,x3,x4,x5,x6,x7,x8,x9,x10) -> x1 + x2 + x3 + x4 + x5 + x6 + x7 + x8 + x9 + x10) @> (3,4,5,6,7,8,9,10,11,12)) (3+4+5+6+7+8+9+10+11+12)
    check "2ver9ewvf" (Eval <@ (fun (x1,x2,x3,x4,x5,x6,x7,x8,x9,x10,x11) -> x1 + x2 + x3 + x4 + x5 + x6 + x7 + x8 + x9 + x10 + x11) @> (3,4,5,6,7,8,9,10,11,12,13)) (3+4+5+6+7+8+9+10+11+12+13)
    check "2ver9ewvg" (Eval <@ (fun (x1,x2,x3,x4,x5,x6,x7,x8,x9,x10,x11,x12) -> x1 + x2 + x3 + x4 + x5 + x6 + x7 + x8 + x9 + x10 + x11 + x12) @> (3,4,5,6,7,8,9,10,11,12,13,14)) (3+4+5+6+7+8+9+10+11+12+13+14)

    checkEval "vlwjvrwe90" (<@ let f (x:int) (y:int) = x + y in f 1 2  @>) 3

    checkEval "2ver9ewvk" (<@ 1 + 1 @>) 2
    checkEval "2ver9ewvl" (<@ 1 > 1 @>) false
    checkEval "2ver9ewvz" (<@ 1 < 1 @>) false
    checkEval "2ver9ewvx" (<@ 1 <= 1 @>) true
    checkEval "2ver9ewvc" (<@ 1 >= 1 @>) true
    Eval <@ System.DateTime.Now @>
    checkEval "2ver9ewvv" (<@ System.Int32.MaxValue @>) System.Int32.MaxValue  // literal field!
    checkEval "2ver9ewvb" (<@ None  : int option @>) None
    checkEval "2ver9ewvn" (<@ Some(1)  @>) (Some(1))
    checkEval "2ver9ewvm" (<@ [] : int list @>) []
    checkEval "2ver9ewqq" (<@ [1] @>) [1]
    checkEval "2ver9ewqq" (<@ ["1"] @>) ["1"]
    checkEval "2ver9ewqq" (<@ ["1";"2"] @>) ["1";"2"]
    check "2ver9ewww" (Eval <@ (fun x -> x + 1) @> 3) 4

    let v = (1,2)
    checkEval "2ver9ewer" (<@ v @>) (1,2)
    checkEval "2ver9ewet" (<@ let x = 1 in x @>) 1
    checkEval "2ver9ewed" (<@ let x = 1+1 in x+x @>) 4

    let raise x = Operators.raise x
    check "2ver9ewveq" (try Eval <@ raise (new System.Exception("hello")) : bool @> with :? System.Exception -> true | _ -> false) true

    
    check "2ver9ewrf" (let v2 = (3,4) in Eval <@ v2 @>) (3,4)
    
    check "2ver9ewrg" (let v2 = (3,4) in Eval <@ v2,v2 @>) ((3,4),(3,4))

    checkEval "2ver9ewrt" (<@ (1,2) @>) (1,2)
    checkEval "2ver9ewvk" (<@ (1,2,3) @>) (1,2,3)
    checkEval "2ver9ewrh" (<@ (1,2,3,4) @>) (1,2,3,4)
    checkEval "2ver9ewrj" (<@ (1,2,3,4,5) @>) (1,2,3,4,5)
    checkEval "2ver9ewrk" (<@ (1,2,3,4,5,6) @>) (1,2,3,4,5,6)
    checkEval "2ver9ewrl" (<@ (1,2,3,4,5,6,7) @>) (1,2,3,4,5,6,7)
    checkEval "2ver9ewra" (<@ (1,2,3,4,5,6,7,8) @>) (1,2,3,4,5,6,7,8)
    checkEval "2ver9ewrs" (<@ (1,2,3,4,5,6,7,8,9) @>) (1,2,3,4,5,6,7,8,9)
    checkEval "2ver9ewrx" (<@ (1,2,3,4,5,6,7,8,9,10) @>) (1,2,3,4,5,6,7,8,9,10)
    checkEval "2ver9ewrc" (<@ (1,2,3,4,5,6,7,8,9,10,11) @>) (1,2,3,4,5,6,7,8,9,10,11)
    checkEval "2ver9ewrv" (<@ (1,2,3,4,5,6,7,8,9,10,11,12) @>) (1,2,3,4,5,6,7,8,9,10,11,12)


    check "2ver9ewrsf" (let v2 = struct(3,4) in Eval <@ v2 @>) struct(3,4)
    
    check "2ver9ewrsg" (let v2 = struct(3,4) in Eval <@ struct(v2,v2) @>) struct(struct(3,4),struct(3,4))

    checkEval "2ver9ewrst" (<@ struct(1,2) @>) struct(1,2)
    checkEval "2ver9ewvsk" (<@ struct(1,2,3) @>) struct(1,2,3)
    checkEval "2ver9ewrsh" (<@ struct(1,2,3,4) @>) struct(1,2,3,4)
    checkEval "2ver9ewrsj" (<@ struct(1,2,3,4,5) @>) struct(1,2,3,4,5)
    checkEval "2ver9ewrsk" (<@ struct(1,2,3,4,5,6) @>) struct(1,2,3,4,5,6)
    checkEval "2ver9ewrsl" (<@ struct(1,2,3,4,5,6,7) @>) struct(1,2,3,4,5,6,7)
    checkEval "2ver9ewrsa" (<@ struct(1,2,3,4,5,6,7,8) @>) struct(1,2,3,4,5,6,7,8)
    checkEval "2ver9ewrss" (<@ struct(1,2,3,4,5,6,7,8,9) @>) struct(1,2,3,4,5,6,7,8,9)
    checkEval "2ver9ewrsx" (<@ struct(1,2,3,4,5,6,7,8,9,10) @>) struct(1,2,3,4,5,6,7,8,9,10)
    checkEval "2ver9ewrsc" (<@ struct(1,2,3,4,5,6,7,8,9,10,11) @>) struct(1,2,3,4,5,6,7,8,9,10,11)
    checkEval "2ver9ewrsv" (<@ struct(1,2,3,4,5,6,7,8,9,10,11,12) @>) struct(1,2,3,4,5,6,7,8,9,10,11,12)

    checkEval "2ver9ewrb" (<@ System.DateTime.Now.DayOfWeek @>) System.DateTime.Now.DayOfWeek
    checkEval "2ver9ewrn" (<@ Checked.(+) 1 1 @>) 2
    checkEval "2ver9ewrm" (<@ Checked.(-) 1 1 @>) 0
    checkEval "2ver9ewrw" (<@ Checked.( * ) 1 1 @>) 1
    checkEval "2ver9ewrre" (let v2 = (3,4) in <@ match v2 with (x,y) -> x + y @>) 7
    checkEval "2ver9ewrn" (<@ "1" = "2" @>) false
    
    module Bug617196 =
        type T = static member Run (e : System.Linq.Expressions.Expression<System.Action>) = ()
        let f = ResizeArray<int>()
        test "Bug617196" (try T.Run (fun () -> f.Clear()); true with _ -> false)

    module Bug445828 =
        open System.Linq.Expressions
        type C = 
            static member M(x : Expression<System.Func<obj,float>>) = x
        test "Bug445828_1"
            (
                try
                    let expr = C.M(fun x -> x :?> float)
                    let body = expr.Body :?> UnaryExpression // Expression.Convert
                    body.NodeType = ExpressionType.Convert && body.Type = typeof<float>
                with _ -> false
            )

        module WeirdCases =
            open System
            open Microsoft.FSharp.Quotations
            
            type A = class end
            type B = class inherit A end
            // F# compiler will throw away conversion in scenarios like
            // fun (x : B) -> x :?> B
            // fun (x : B) -> x :?> A
            // but you can always create such cases by hand - so we'll test them as well
            
            test "Bug445828_WeirdCases_1"
                (
                    try
                        let expr = 
                            let v = Var("x", typeof<B>)
                            let q = Expr.NewDelegate(typeof<Func<B, B>>, [v], Expr.Coerce(Expr.Var(v), typeof<B>)) // fun (x : B) -> x :?> B
                            (Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.QuotationToExpression q) :?> Expression<Func<B, B>>
                        expr.Body :? ParameterExpression // no Convert node
                    with _ -> false
                )
                
            test "Bug445828_WeirdCases_2"
                (
                    try
                        let expr = 
                            let v = Var("x", typeof<B>)
                            let q = Expr.NewDelegate(typeof<Func<B, A>>, [v], Expr.Coerce(Expr.Var(v), typeof<A>)) // fun (x : B) -> x :?> A
                            (Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter.QuotationToExpression q) :?> Expression<Func<B, A>>
                        expr.Body :? ParameterExpression // no Convert node
                    with _ -> false
                )                
        
    module NonGenericRecdTests = 
        type Customer = { mutable Name:string; Data: int }
        let c1 = { Name="Don"; Data=6 }
        let c2 = { Name="Peter"; Data=7 }
        let c3 = { Name="Freddy"; Data=8 }
        checkEval "2ver9e1rw1" (<@ c1.Name @>) "Don"
        checkEval "2ver9e2rw2" (<@ c2.Name @>) "Peter"
        checkEval "2ver9e3rw3" (<@ c2.Data @>) 7
        checkEval "2ver9e7rw4" (<@ { Name = "Don"; Data = 6 } @>) { Name="Don"; Data=6 }
        checkEval "2ver9e7rw5" (<@ { Name = "Don"; Data = 6 } @>) { Name="Don"; Data=6 }

    module GenericRecdTests = 
        type CustomerG<'a> = { mutable Name:string; Data: 'a }
        let c1 : CustomerG<int> = { Name="Don"; Data=6 }
        let c2 : CustomerG<int> = { Name="Peter"; Data=7 }
        let c3 : CustomerG<string> = { Name="Freddy"; Data="8" }
        checkEval "2ver9e4rw6" (<@ c1.Name @>) "Don"
        checkEval "2ver9e5rw7" (<@ c2.Name @>) "Peter"
        checkEval "2ver9e6rw8" (<@ c2.Data @>) 7
        checkEval "2ver9e7rw9" (<@ c3.Data @>) "8"
        checkEval "2ver9e7rwQ" (<@ { Name = "Don"; Data = 6 } @>) { Name="Don"; Data=6 }
        checkEval "2ver9e7rwE" (<@ c1.Name  @>) "Don"

    module ArrayTests = 
        checkEval "2ver9e8rwR1" (<@ [| |]  @>) ([| |] : int array)
        checkEval "2ver9e8rwR2" (<@ [| 0 |]  @>) ([| 0 |] : int array)
        checkEval "2ver9e8rwR3" (<@ [| 0  |].[0]  @>) 0
        checkEval "2ver9e8rwR4" (<@ [| 1; 2  |].[0]  @>) 1
        checkEval "2ver9e8rwR5" (<@ [| 1; 2  |].[1]  @>) 2

    module Array2DTests = 
        checkEval "2ver9e8rwR6" (<@ (Array2D.init 3 4 (fun i j -> i + j)).[0,0] @>) 0
        checkEval "2ver9e8rwR7" (<@ (Array2D.init 3 4 (fun i j -> i + j)).[1,2] @>) 3
        checkEval "2ver9e8rwR8" (<@ (Array2D.init 3 4 (fun i j -> i + j)) |> Array2D.base1 @>) 0
        checkEval "2ver9e8rwR9" (<@ (Array2D.init 3 4 (fun i j -> i + j)) |> Array2D.base2 @>) 0
        checkEval "2ver9e8rwRQ" (<@ (Array2D.init 3 4 (fun i j -> i + j)) |> Array2D.length1 @>) 3
        checkEval "2ver9e8rwRW" (<@ (Array2D.init 3 4 (fun i j -> i + j)) |> Array2D.length2 @>) 4


    module Array3DTests = 
        checkEval "2ver9e8rwRE" (<@ (Array3D.init 3 4 5 (fun i j k -> i + j)).[0,0,0] @>) 0
        checkEval "2ver9e8rwRR" (<@ (Array3D.init 3 4 5 (fun i j k -> i + j + k)).[1,2,3] @>) 6
        checkEval "2ver9e8rwRT" (<@ (Array3D.init 3 4 5 (fun i j k -> i + j)) |> Array3D.length1 @>) 3
        checkEval "2ver9e8rwRY" (<@ (Array3D.init 3 4 5 (fun i j k -> i + j)) |> Array3D.length2 @>) 4
        checkEval "2ver9e8rwRU" (<@ (Array3D.init 3 4 5 (fun i j k -> i + j)) |> Array3D.length3 @>) 5

    module ExceptionTests = 
        exception E0
        exception E1 of string
        let c1 = E0
        let c2 = E1 "1"
        let c3 = E1 "2"
        checkEval "2ver9e8rwR" (<@ E0  @>) E0
        checkEval "2ver9e8rwT" (<@ E1 "1"  @>) (E1 "1")
        checkEval "2ver9eQrwY" (<@ match c1 with E0 -> 1 | _ -> 2  @>) 1
        checkEval "2ver9eQrwU" (<@ match c2 with E0 -> 1 | _ -> 2  @>) 2
        checkEval "2ver9eQrwI" (<@ match c2 with E0 -> 1 | E1 _  -> 2 | _ -> 3  @>) 2
        checkEval "2ver9eQrwO" (<@ match c2 with E1 _  -> 2 | E0 -> 1 | _ -> 3  @>) 2
        checkEval "2ver9eQrwP" (<@ match c2 with E1 "1"  -> 2 | E0 -> 1 | _ -> 3  @>) 2
        checkEval "2ver9eQrwA" (<@ match c2 with E1 "2"  -> 2 | E0 -> 1 | _ -> 3  @>) 3
        checkEval "2ver9eQrwS" (<@ match c3 with E1 "2"  -> 2 | E0 -> 1 | _ -> 3  @>) 2

    module TypeTests = 
        type C0() = 
            member x.P = 1
        type C1(s:string) = 
            member x.P = s
        let c1 = C0()
        let c2 = C1 "1"
        let c3 = C1 "2"
        checkEval "2ver9e8rwJ" (<@ C0().P  @>) 1
        checkEval "2ver9e8rwK" (<@ C1("1").P  @>)  "1"
        checkEval "2ver9eQrwL" (<@ match box c1 with :? C0 -> 1 | _ -> 2  @>) 1
        checkEval "2ver9eQrwZ" (<@ match box c2 with :? C0 -> 1 | _ -> 2  @>) 2
        checkEval "2ver9eQrwX" (<@ match box c2 with :? C0 -> 1 | :? C1   -> 2 | _ -> 3  @>) 2
        checkEval "2ver9eQrwC" (<@ match box c2 with :? C1   -> 2 | :? C0 -> 1 | _ -> 3  @>) 2
        checkEval "2ver9eQrwV" (<@ match box c2 with :? C1  -> 2 | :? C0 -> 1 | _ -> 3  @>) 2
        checkEval "2ver9eQrwN" (<@ match box c3 with :? C1  as c1 when c1.P = "2"  -> 2 | :? C0 -> 1 | _ -> 3  @>) 2

    module NonGenericUnionTests0 = 
        type Animal = Cat of string | Dog
        let c1 = Cat "meow"
        let c2 = Dog
        checkEval "2ver9e8rw11" (<@ Cat "sss" @>) (Cat "sss")
        checkEval "2ver9e9rw12" (<@ Dog @>) Dog
        checkEval "2ver9eQrw13" (<@ match c1 with Cat _ -> 2 | Dog -> 1 @>) 2
        checkEval "2ver9eWrw14" (<@ match c1 with Cat s -> s | Dog -> "woof" @>) "meow"
        checkEval "2ver9eErw15" (<@ match c2 with Cat s -> s | Dog -> "woof" @>) "woof"

    module NonGenericUnionTests1 = 
        type Animal = Cat of string 
        let c1 = Cat "meow"
        checkEval "2ver9e8rw16" (<@ Cat "sss" @>) (Cat "sss")
        checkEval "2ver9eQrw17" (<@ match c1 with Cat _ -> 2  @>) 2
        checkEval "2ver9eWrw18" (<@ match c1 with Cat s -> s  @>) "meow"

    module NonGenericUnionTests2 = 
        type Animal = 
           | Cat of string 
           | Dog of string
        let c1 = Cat "meow"
        let c2 = Dog "woof"
        checkEval "2ver9e8rw19" (<@ Cat "sss" @>) (Cat "sss")
        checkEval "2ver9e9rw20" (<@ Dog "bowwow" @>) (Dog "bowwow")
        checkEval "2ver9eQrw21" (<@ match c1 with Cat _ -> 2 | Dog  _ -> 1 @>) 2
        checkEval "2ver9eWrw22" (<@ match c1 with Cat s -> s | Dog s -> s @>) "meow"
        checkEval "2ver9eErw23" (<@ match c2 with Cat s -> s | Dog s -> s @>) "woof"

    module NonGenericUnionTests3 = 
        type Animal = 
           | Cat of string 
           | Dog of string
           | Dog1 of string
           | Dog2 of string
           | Dog3 of string
           | Dog4 of string
           | Dog5 of string
           | Dog6 of string
           | Dog7 of string
           | Dog8 of string
           | Dog9 of string
           | DogQ of string
           | DogW of string
           | DogE of string
           | DogR of string
           | DogT of string
           | DogY of string
           | DogU of string
           | DogI of string
        let c1 = Cat "meow"
        let c2 = Dog "woof"
        checkEval "2ver9e8rw24" (<@ Cat "sss" @>) (Cat "sss")
        checkEval "2ver9e9rw25" (<@ Dog "bowwow" @>) (Dog "bowwow")
        checkEval "2ver9eQrw26" (<@ match c1 with Cat _ -> 2 | _ -> 1 @>) 2
        checkEval "2ver9eWrw27" (<@ match c1 with Cat s -> s | _ -> "woof" @>) "meow"
        checkEval "2ver9eErw28" (<@ match c2 with Cat s -> s | Dog s -> s | _ -> "bark" @>) "woof"


    module GenericUnionTests = 
        type Animal<'a> = Cat of 'a | Dog
        let c1 = Cat "meow"
        let c2 = Dog
        checkEval "2ver9e8rw29" (<@ Cat "sss" @>) (Cat "sss")
        checkEval "2ver9e9rw30" (<@ Dog @>) Dog
        checkEval "2ver9eQrw31" (<@ match c1 with Cat _ -> 2 | Dog -> 1 @>) 2
        checkEval "2ver9eWrw32" (<@ match c1 with Cat s -> s | Dog -> "woof" @>) "meow"
        checkEval "2ver9eErw33" (<@ match c2 with Cat s -> s | Dog -> "woof" @>) "woof"

    module InlinedOperationsStillDynamicallyAvailableTests = 

        checkEval "vroievr093" (<@ LanguagePrimitives.GenericZero<sbyte> @>)  0y
        checkEval "vroievr091" (<@ LanguagePrimitives.GenericZero<int16> @>)  0s
        checkEval "vroievr091" (<@ LanguagePrimitives.GenericZero<int32> @>)  0
        checkEval "vroievr091" (<@ LanguagePrimitives.GenericZero<int64> @>)  0L
        checkEval "vroievr091" (<@ LanguagePrimitives.GenericZero<nativeint> @>)  0n
        checkEval "vroievr093" (<@ LanguagePrimitives.GenericZero<byte> @>)  0uy
        checkEval "vroievr091" (<@ LanguagePrimitives.GenericZero<uint16> @>)  0us
        checkEval "vroievr091" (<@ LanguagePrimitives.GenericZero<uint32> @>)  0u
        checkEval "vroievr091" (<@ LanguagePrimitives.GenericZero<uint64> @>)  0UL
        checkEval "vroievr091" (<@ LanguagePrimitives.GenericZero<unativeint> @>)  0un
        checkEval "vroievr091" (<@ LanguagePrimitives.GenericZero<float> @>)  0.0
        checkEval "vroievr091" (<@ LanguagePrimitives.GenericZero<float32> @>)  0.0f
        checkEval "vroievr092" (<@ LanguagePrimitives.GenericZero<decimal> @>)  0M



        checkEval "vroievr093" (<@ LanguagePrimitives.GenericOne<sbyte> @>)  1y
        checkEval "vroievr191" (<@ LanguagePrimitives.GenericOne<int16> @>)  1s
        checkEval "vroievr191" (<@ LanguagePrimitives.GenericOne<int32> @>)  1
        checkEval "vroievr191" (<@ LanguagePrimitives.GenericOne<int64> @>)  1L
        checkEval "vroievr191" (<@ LanguagePrimitives.GenericOne<nativeint> @>)  1n
        checkEval "vroievr193" (<@ LanguagePrimitives.GenericOne<byte> @>)  1uy
        checkEval "vroievr191" (<@ LanguagePrimitives.GenericOne<uint16> @>)  1us
        checkEval "vroievr191" (<@ LanguagePrimitives.GenericOne<uint32> @>)  1u
        checkEval "vroievr191" (<@ LanguagePrimitives.GenericOne<uint64> @>)  1UL
        checkEval "vroievr191" (<@ LanguagePrimitives.GenericOne<unativeint> @>)  1un
        checkEval "vroievr191" (<@ LanguagePrimitives.GenericOne<float> @>)  1.0
        checkEval "vroievr191" (<@ LanguagePrimitives.GenericOne<float32> @>)  1.0f
        checkEval "vroievr192" (<@ LanguagePrimitives.GenericOne<decimal> @>)  1M

        check "vroievr0971" (LanguagePrimitives.AdditionDynamic 3y 4y) 7y
        check "vroievr0972" (LanguagePrimitives.AdditionDynamic 3s 4s) 7s
        check "vroievr0973" (LanguagePrimitives.AdditionDynamic 3 4) 7
        check "vroievr0974" (LanguagePrimitives.AdditionDynamic 3L 4L) 7L
        check "vroievr0975" (LanguagePrimitives.AdditionDynamic 3n 4n) 7n
        check "vroievr0976" (LanguagePrimitives.AdditionDynamic 3uy 4uy) 7uy
        check "vroievr0977" (LanguagePrimitives.AdditionDynamic 3us 4us) 7us
        check "vroievr0978" (LanguagePrimitives.AdditionDynamic 3u 4u) 7u
        check "vroievr0979" (LanguagePrimitives.AdditionDynamic 3UL 4UL) 7UL
        check "vroievr0970" (LanguagePrimitives.AdditionDynamic 3un 4un) 7un
        check "vroievr097q" (LanguagePrimitives.AdditionDynamic 3.0 4.0) 7.0
        check "vroievr097w" (LanguagePrimitives.AdditionDynamic 3.0f 4.0f) 7.0f
        check "vroievr097e" (LanguagePrimitives.AdditionDynamic 3.0M 4.0M) 7.0M

        check "vroievr097r" (LanguagePrimitives.CheckedAdditionDynamic 3y 4y) 7y
        check "vroievr097t" (LanguagePrimitives.CheckedAdditionDynamic 3s 4s) 7s
        check "vroievr097y" (LanguagePrimitives.CheckedAdditionDynamic 3 4) 7
        check "vroievr097u" (LanguagePrimitives.CheckedAdditionDynamic 3L 4L) 7L
        check "vroievr097i" (LanguagePrimitives.CheckedAdditionDynamic 3n 4n) 7n
        check "vroievr097o" (LanguagePrimitives.CheckedAdditionDynamic 3uy 4uy) 7uy
        check "vroievr097p" (LanguagePrimitives.CheckedAdditionDynamic 3us 4us) 7us
        check "vroievr097a" (LanguagePrimitives.CheckedAdditionDynamic 3u 4u) 7u
        check "vroievr097s" (LanguagePrimitives.CheckedAdditionDynamic 3UL 4UL) 7UL
        check "vroievr097d" (LanguagePrimitives.CheckedAdditionDynamic 3un 4un) 7un
        check "vroievr097f" (LanguagePrimitives.CheckedAdditionDynamic 3.0 4.0) 7.0
        check "vroievr097g" (LanguagePrimitives.CheckedAdditionDynamic 3.0f 4.0f) 7.0f
        check "vroievr097h" (LanguagePrimitives.CheckedAdditionDynamic 3.0M 4.0M) 7.0M

        check "vroievr0912q" (LanguagePrimitives.MultiplyDynamic 3y 4y) 12y
        check "vroievr0912w" (LanguagePrimitives.MultiplyDynamic 3s 4s) 12s
        check "vroievr0912e" (LanguagePrimitives.MultiplyDynamic 3 4) 12
        check "vroievr0912r" (LanguagePrimitives.MultiplyDynamic 3L 4L) 12L
        check "vroievr0912t" (LanguagePrimitives.MultiplyDynamic 3n 4n) 12n
        check "vroievr0912y" (LanguagePrimitives.MultiplyDynamic 3uy 4uy) 12uy
        check "vroievr0912u" (LanguagePrimitives.MultiplyDynamic 3us 4us) 12us
        check "vroievr0912i" (LanguagePrimitives.MultiplyDynamic 3u 4u) 12u
        check "vroievr0912o" (LanguagePrimitives.MultiplyDynamic 3UL 4UL) 12UL
        check "vroievr0912p" (LanguagePrimitives.MultiplyDynamic 3un 4un) 12un
        check "vroievr0912a" (LanguagePrimitives.MultiplyDynamic 3.0 4.0) 12.0
        check "vroievr0912s" (LanguagePrimitives.MultiplyDynamic 3.0f 4.0f) 12.0f
        check "vroievr0912d" (LanguagePrimitives.MultiplyDynamic 3.0M 4.0M) 12.0M


        check "vroievr0912f" (LanguagePrimitives.CheckedMultiplyDynamic 3y 4y) 12y
        check "vroievr0912g" (LanguagePrimitives.CheckedMultiplyDynamic 3s 4s) 12s
        check "vroievr0912h" (LanguagePrimitives.CheckedMultiplyDynamic 3 4) 12
        check "vroievr0912j" (LanguagePrimitives.CheckedMultiplyDynamic 3L 4L) 12L
        check "vroievr0912k" (LanguagePrimitives.CheckedMultiplyDynamic 3n 4n) 12n
        check "vroievr0912l" (LanguagePrimitives.CheckedMultiplyDynamic 3uy 4uy) 12uy
        check "vroievr0912z" (LanguagePrimitives.CheckedMultiplyDynamic 3us 4us) 12us
        check "vroievr0912x" (LanguagePrimitives.CheckedMultiplyDynamic 3u 4u) 12u
        check "vroievr0912c" (LanguagePrimitives.CheckedMultiplyDynamic 3UL 4UL) 12UL
        check "vroievr0912v" (LanguagePrimitives.CheckedMultiplyDynamic 3un 4un) 12un
        check "vroievr0912b" (LanguagePrimitives.CheckedMultiplyDynamic 3.0 4.0) 12.0
        check "vroievr0912n" (LanguagePrimitives.CheckedMultiplyDynamic 3.0f 4.0f) 12.0f
        check "vroievr0912m" (LanguagePrimitives.CheckedMultiplyDynamic 3.0M 4.0M) 12.0M


        // TODO: check subtraction over user defined types.
        // TODO: check subtraction and be complete w.r.t. "inline" functions in prim-types.fsi

        let iarr = [| 0..1000 |]
        let ilist = [ 0..1000 ]

        let farr = [| 0.0 .. 1.0 .. 100.0 |]
        let flist = [ 0.0 .. 1.0 .. 100.0 ]


        checkEval "vrewoinrv091" (<@ farr.[0] @>) 0.0
        checkEval "vrewoinrv092" (<@ flist.[0] @>) 0.0
        checkEval "vrewoinrv093" (<@ iarr.[0] @>) 0
        checkEval "vrewoinrv094" (<@ ilist.[0] @>) 0


        checkEval "vrewoinrv09r" (<@ Array.average farr @>) (Array.average farr)
        checkEval "vrewoinrv09t" (<@ Array.sum farr @>) (Array.sum farr)
        checkEval "vrewoinrv09y" (<@ Seq.sum farr @>) (Seq.sum farr)
        checkEval "vrewoinrv09u" (<@ Seq.average farr @>) (Seq.average farr) 
        checkEval "vrewoinrv09i" (<@ Seq.average flist @>) (Seq.average flist)
        checkEval "vrewoinrv09o" (<@ Seq.averageBy (fun x -> x) farr @> ) (Seq.averageBy (fun x -> x) farr )
        checkEval "vrewoinrv09p" (<@ Seq.averageBy (fun x -> x) flist @>) (Seq.averageBy (fun x -> x) flist )
        checkEval "vrewoinrv09a" (<@ Seq.averageBy float ilist @>) (Seq.averageBy float ilist)
        checkEval "vrewoinrv09s" (<@ List.sum flist @>) (List.sum flist)
        checkEval "vrewoinrv09d" (<@ List.average flist @>) (List.average flist)
        checkEval "vrewoinrv09f" (<@ List.averageBy float ilist @>) (List.averageBy float ilist)

        checkEval "vrewoinrv09g1" (<@ compare 0 0 = 0 @>) true
        checkEval "vrewoinrv09g2" (<@ compare 0 1 < 0 @>) true
        checkEval "vrewoinrv09g3" (<@ compare 1 0 > 0 @>) true
        checkEval "vrewoinrv09g4" (<@ 0 < 1 @>) true
        checkEval "vrewoinrv09g5" (<@ 0 <= 1 @>) true
        checkEval "vrewoinrv09g6" (<@ 1 <= 1 @>) true
        checkEval "vrewoinrv09g7" (<@ 2 <= 1 @>) false
        checkEval "vrewoinrv09g8" (<@ 0 > 1 @>) false
        checkEval "vrewoinrv09g9" (<@ 0 >= 1 @>) false
        checkEval "vrewoinrv09g0" (<@ 1 >= 1 @>) true
        checkEval "vrewoinrv09gQ" (<@ 2 >= 1 @>) true

        checkEval "vrewoinrv09gw" (<@ compare 0.0 0.0 = 0 @>) true
        checkEval "vrewoinrv09ge" (<@ compare 0.0 1.0 < 0 @>) true
        checkEval "vrewoinrv09gr" (<@ compare 1.0 0.0 > 0 @>) true
        checkEval "vrewoinrv09gt" (<@ 0.0 < 1.0 @>) true
        checkEval "vrewoinrv09gy" (<@ 0.0 <= 1.0 @>) true
        checkEval "vrewoinrv09gu" (<@ 1.0 <= 1.0 @>) true
        checkEval "vrewoinrv09gi" (<@ 2.0 <= 1.0 @>) false
        checkEval "vrewoinrv09go" (<@ 0.0 > 1.0 @>) false
        checkEval "vrewoinrv09gp" (<@ 0.0 >= 1.0 @>) false
        checkEval "vrewoinrv09ga" (<@ 1.0 >= 1.0 @>) true
        checkEval "vrewoinrv09gs" (<@ 2.0 >= 1.0 @>) true

        checkEval "vrewoinrv09gd" (<@ compare 0.0f 0.0f = 0 @>) true
        checkEval "vrewoinrv09gf" (<@ compare 0.0f 1.0f < 0 @>) true
        checkEval "vrewoinrv09gg" (<@ compare 1.0f 0.0f > 0 @>) true
        checkEval "vrewoinrv09gh" (<@ 0.0f < 1.0f @>) true
        checkEval "vrewoinrv09gk" (<@ 0.0f <= 1.0f @>) true
        checkEval "vrewoinrv09gl" (<@ 1.0f <= 1.0f @>) true
        checkEval "vrewoinrv09gz" (<@ 2.0f <= 1.0f @>) false
        checkEval "vrewoinrv09gx" (<@ 0.0f > 1.0f @>) false
        checkEval "vrewoinrv09gc" (<@ 0.0f >= 1.0f @>) false
        checkEval "vrewoinrv09gv" (<@ 1.0f >= 1.0f @>) true
        checkEval "vrewoinrv09gb" (<@ 2.0f >= 1.0f @>) true

        checkEval "vrewoinrv09gn" (<@ compare 0L 0L = 0 @>) true
        checkEval "vrewoinrv09gm" (<@ compare 0L 1L < 0 @>) true
        checkEval "vrewoinrv09g11" (<@ compare 1L 0L > 0 @>) true
        checkEval "vrewoinrv09g12" (<@ 0L < 1L @>) true
        checkEval "vrewoinrv09g13" (<@ 0L <= 1L @>) true
        checkEval "vrewoinrv09g14" (<@ 1L <= 1L @>) true
        checkEval "vrewoinrv09g15" (<@ 2L <= 1L @>) false
        checkEval "vrewoinrv09g16" (<@ 0L > 1L @>) false
        checkEval "vrewoinrv09g17" (<@ 0L >= 1L @>) false
        checkEval "vrewoinrv09g18" (<@ 1L >= 1L @>) true
        checkEval "vrewoinrv09g19" (<@ 2L >= 1L @>) true

        checkEval "vrewoinrv09g21" (<@ compare 0y 0y = 0 @>) true
        checkEval "vrewoinrv09g22" (<@ compare 0y 1y < 0 @>) true
        checkEval "vrewoinrv09g23" (<@ compare 1y 0y > 0 @>) true
        checkEval "vrewoinrv09g24" (<@ 0y < 1y @>) true
        checkEval "vrewoinrv09g25" (<@ 0y <= 1y @>) true
        checkEval "vrewoinrv09g26" (<@ 1y <= 1y @>) true
        checkEval "vrewoinrv09g27" (<@ 2y <= 1y @>) false
        checkEval "vrewoinrv09g28" (<@ 0y > 1y @>) false
        checkEval "vrewoinrv09g29" (<@ 0y >= 1y @>) false
        checkEval "vrewoinrv09g30" (<@ 1y >= 1y @>) true
        checkEval "vrewoinrv09g31" (<@ 2y >= 1y @>) true

        checkEval "vrewoinrv09g32" (<@ compare 0M 0M = 0 @>) true
        checkEval "vrewoinrv09g33" (<@ compare 0M 1M < 0 @>) true
        checkEval "vrewoinrv09g34" (<@ compare 1M 0M > 0 @>) true
        checkEval "vrewoinrv09g35" (<@ 0M < 1M @>) true
        checkEval "vrewoinrv09g36" (<@ 0M <= 1M @>) true
        checkEval "vrewoinrv09g37" (<@ 1M <= 1M @>) true
        checkEval "vrewoinrv09g38" (<@ 2M <= 1M @>) false
        checkEval "vrewoinrv09g39" (<@ 0M > 1M @>) false
        checkEval "vrewoinrv09g40" (<@ 0M >= 1M @>) false
        checkEval "vrewoinrv09g41" (<@ 1M >= 1M @>) true
        checkEval "vrewoinrv09g42" (<@ 2M >= 1M @>) true

        checkEval "vrewoinrv09g43" (<@ compare 0I 0I = 0 @>) true
        checkEval "vrewoinrv09g44" (<@ compare 0I 1I < 0 @>) true
        checkEval "vrewoinrv09g45" (<@ compare 1I 0I > 0 @>) true
        checkEval "vrewoinrv09g46" (<@ 0I < 1I @>) true
        checkEval "vrewoinrv09g47" (<@ 0I <= 1I @>) true
        checkEval "vrewoinrv09g48" (<@ 1I <= 1I @>) true
        checkEval "vrewoinrv09g49" (<@ 2I <= 1I @>) false
        checkEval "vrewoinrv09g50" (<@ 0I > 1I @>) false
        checkEval "vrewoinrv09g51" (<@ 0I >= 1I @>) false
        checkEval "vrewoinrv09g52" (<@ 1I >= 1I @>) true
        checkEval "vrewoinrv09g53" (<@ 2I >= 1I @>) true


        checkEval "vrewoinrv09g" (<@ sin 0.0 @>) (sin 0.0)
        checkEval "vrewoinrv09h" (<@ sinh 0.0 @>) (sinh 0.0)
        checkEval "vrewoinrv09j" (<@ cos 0.0 @>) (cos 0.0)
        checkEval "vrewoinrv09k" (<@ cosh 0.0 @>) (cosh 0.0)
        checkEval "vrewoinrv09l" (<@ tan 1.0 @>) (tan 1.0)
        checkEval "vrewoinrv09z" (<@ tanh 1.0 @>) (tanh 1.0)
        checkEval "vrewoinrv09x" (<@ abs -2.0 @>) (abs -2.0)
        checkEval "vrewoinrv09c" (<@ ceil 2.0 @>) (ceil 2.0)
        checkEval "vrewoinrv09v" (<@ sqrt 2.0 @>) (sqrt 2.0)
        checkEval "vrewoinrv09b" (<@ sign 2.0 @>) (sign 2.0)
#if !NETCOREAPP
        checkEval "vrewoinrv09n" (<@ truncate 2.3 @>) (truncate 2.3)
#endif
        checkEval "vrewoinrv09m" (<@ floor 2.3 @>) (floor 2.3)
        checkEval "vrewoinrv09Q" (<@ round 2.3 @>) (round 2.3)
        checkEval "vrewoinrv09W" (<@ log 2.3 @>) (log 2.3)
        checkEval "vrewoinrv09E" (<@ log10 2.3 @>) (log10 2.3)
        checkEval "vrewoinrv09R" (<@ exp 2.3 @>) (exp 2.3)
        checkEval "vrewoinrv09T" (<@ 2.3 ** 2.4 @>) (2.3 ** 2.4)

        checkEval "vrewoinrv09Y" (<@ sin 0.0f @>) (sin 0.0f)
        checkEval "vrewoinrv09U" (<@ sinh 0.0f @>) (sinh 0.0f)
        checkEval "vrewoinrv09I" (<@ cos 0.0f @>) (cos 0.0f)
        checkEval "vrewoinrv09O" (<@ cosh 0.0f @>) (cosh 0.0f)
        checkEval "vrewoinrv09P" (<@ tan 1.0f @>) (tan 1.0f)
        checkEval "vrewoinrv09A" (<@ tanh 1.0f @>) (tanh 1.0f)
        checkEval "vrewoinrv09S" (<@ abs -2.0f @>) (abs -2.0f)
        checkEval "vrewoinrv09D" (<@ ceil 2.0f @>) (ceil 2.0f)
        checkEval "vrewoinrv09F" (<@ sqrt 2.0f @>) (sqrt 2.0f)
        checkEval "vrewoinrv09G" (<@ sign 2.0f @>) (sign 2.0f)
#if !NETCOREAPP
        checkEval "vrewoinrv09H" (<@ truncate 2.3f @>) (truncate 2.3f)
#endif
        checkEval "vrewoinrv09J" (<@ floor 2.3f @>) (floor 2.3f)
        checkEval "vrewoinrv09K" (<@ round 2.3f @>) (round 2.3f)
        checkEval "vrewoinrv09L" (<@ log 2.3f @>) (log 2.3f)
        checkEval "vrewoinrv09Z" (<@ log10 2.3f @>) (log10 2.3f)
        checkEval "vrewoinrv09X" (<@ exp 2.3f @>) (exp 2.3f)
        checkEval "vrewoinrv09C" (<@ 2.3f ** 2.4f @>) (2.3f ** 2.4f)

        checkEval "vrewoinrv09V" (<@ ceil 2.0M @>) (ceil 2.0M)
        checkEval "vrewoinrv09B" (<@ sign 2.0M @>) (sign 2.0M)
#if !NETCOREAPP
        checkEval "vrewoinrv09N" (<@ truncate 2.3M @>) (truncate 2.3M)
#endif
        checkEval "vrewoinrv09M" (<@ floor 2.3M @>) (floor 2.3M)

        checkEval "vrewoinrv09QQ" (<@ sign -2 @>) (sign -2)
        checkEval "vrewoinrv09WW" (<@ sign -2y @>) (sign -2y)
        checkEval "vrewoinrv09EE" (<@ sign -2s @>) (sign -2s)
        checkEval "vrewoinrv09RR" (<@ sign -2L @>) (sign -2L)

        checkEval "vrewoinrv09TT" (<@ [ 0 .. 10 ] @>) [ 0 .. 10 ]
        checkEval "vrewoinrv09YY" (<@ [ 0y .. 10y ] @>) [ 0y .. 10y ]
        checkEval "vrewoinrv09UU" (<@ [ 0s .. 10s ] @>) [ 0s .. 10s ]
        checkEval "vrewoinrv09II" (<@ [ 0L .. 10L ] @>) [ 0L .. 10L ]
        checkEval "vrewoinrv09OO" (<@ [ 0u .. 10u ] @>) [ 0u .. 10u ]
        checkEval "vrewoinrv09PP" (<@ [ 0uy .. 10uy ] @>) [ 0uy .. 10uy ]
        checkEval "vrewoinrv09AA" (<@ [ 0us .. 10us ] @>) [ 0us .. 10us ]
        checkEval "vrewoinrv09SS" (<@ [ 0UL .. 10UL ] @>) [ 0UL .. 10UL ]
        
        //Comment this testcase under portable due to bug 500323:[FSharp] portable library can't run "round" function
#if !NETCOREAPP
        // Round dynamic dispatch on Decimal
        checkEval "vrewoinrv09FF" (<@ round 2.3M @>) (round 2.3M)
#endif

        // Measure stuff:
        checkEval "vrewoinrv09GG" (<@ atan2 3.0 4.0 @>) (atan2 3.0 4.0 )
        
        [<Measure>]
        type kg
        checkEval "vrewoinrv09HH" (<@ 1.0<kg> @>) (1.0<kg>)

        // Measure stuff:
        checkEval "vrewoinrv09JJ" (<@ 1.0<kg> + 2.0<kg> @>) (3.0<kg>)


        Eval <@ Array.average [| 0.0 .. 1.0 .. 10000.0 |] @> 

    module LanguagePrimitiveCastingUnitsOfMeasure = 
        [<Measure>]
        type m

        checkEval "castingunits1" (<@ 2.5 |> LanguagePrimitives.FloatWithMeasure<m> |> float @>) 2.5
        checkEval "castingunits2" (<@ 2.5f |> LanguagePrimitives.Float32WithMeasure<m> |> float32 @>) 2.5f
        checkEval "castingunits3" (<@ 2.0m |> LanguagePrimitives.DecimalWithMeasure<m> |> decimal @>) 2.0M
        checkEval "castingunits4" (<@ 2 |> LanguagePrimitives.Int32WithMeasure<m> |> int @>) 2
        checkEval "castingunits5" (<@ 2L |> LanguagePrimitives.Int64WithMeasure<m> |> int64 @>) 2L
        checkEval "castingunits6" (<@ 2s |> LanguagePrimitives.Int16WithMeasure<m> |> int16 @>) 2s
        checkEval "castingunits7" (<@ 2y |> LanguagePrimitives.SByteWithMeasure<m> |> sbyte @>) 2y
        checkEval "castingunits8" (<@ 2ul |> LanguagePrimitives.UInt32WithMeasure<m> |> uint @>) 2ul
        checkEval "castingunits9" (<@ 2UL |> LanguagePrimitives.UInt64WithMeasure<m> |> uint64 @>) 2UL
        checkEval "castingunits10" (<@ 2us |> LanguagePrimitives.UInt16WithMeasure<m> |> uint16 @>) 2us
        checkEval "castingunits11" (<@ 2uy |> LanguagePrimitives.ByteWithMeasure<m> |> byte @>) 2uy
        
        //NOTE quotations currently *DO NOT* support native integers
        //TODO revisit when the test scaffolding is changed/migrated!
        // checkEval "castingunits12" (<@ 2n |> LanguagePrimitives.IntPtrWithMeasure<m> |> nativeint @>) 2n
        // checkEval "castingunits13" (<@ 2un |> LanguagePrimitives.UIntPtrWithMeasure<m> |> unativeint @>) 2un


    module LargerAutomaticDifferentiationTest_FSharp_1_0_Bug_3498 = 

        let q = 
            <@ (fun (x1:double) -> 
                   let fwd6 = 
                       let y3 = x1 * x1
                       (y3, (fun yb4 -> yb4 * 2.0 * x1))
                   let rev5 = snd fwd6
                   let w0 = fst fwd6

                   let fwd14 = 
                       let y11 = w0 + 1.0
                       (y11, (fun yb12 -> yb12 * 1.0))
                   let rev13 = snd fwd14
                   let y8 = fst fwd14
                   (y8, (fun y8b10 -> 
                              let w0b2 = 0.0 
                              let x1b1 = 0.0 
                              let dxs15 = rev13 y8b10 
                              let w0b2 = w0b2 + dxs15 
                              let dxs7 = rev5 w0b2 
                              let x1b1 = x1b1 + dxs7 
                              x1b1))) @>

        checkEval "!cwjkwwecew0" <@ fst ((%q) 4.0) @> 17.0
        checkEval "!cwjkwwecew0" <@ snd ((%q) 4.0) 0.1 @> 0.8

    module FunkyMethodRepresentations = 
        // The IsSome and IsNone properties are represented as static methods because
        // option uses 'null' as a representation
        checkEval "clkedw0" (<@ let x : int option = None in x.IsSome @>) false
        checkEval "clkedw1" (<@ let x : int option = None in x.IsNone @>) true
        checkEval "clkedw2" (<@ let x : int option = Some 1 in x.Value @>) 1
        checkEval "clkedw3" (<@ let x : int option = Some 1 in x.ToString() @>) "Some(1)"

    module NestedQuotes = 

        open Microsoft.FSharp.Linq.NullableOperators

        checkEval "feoewjewjlejf1" <@ <@@ 1 @@> @> <@@ 1 @@> 
        checkEval "feoewjewjlejf2" <@ <@ 1 @> @> <@ 1 @> 
        checkEval "feoewjewjlejf3" <@ <@@ 1 @@>, <@ 2 @> @> (<@@ 1 @@> , <@ 2 @>)

    module Extensions = 
        type System.Object with 
            member x.ExtensionMethod0()  = 3
            member x.ExtensionMethod1()  = ()
            member x.ExtensionMethod2(y:int)  = y
            member x.ExtensionMethod3(y:int)  = ()
            member x.ExtensionMethod4(y:int,z:int)  = y + z
            member x.ExtensionMethod5(y:(int*int))  = y 
            member x.ExtensionProperty1 = 3
            member x.ExtensionProperty2 with get() = 3
            member x.ExtensionProperty3 with set(v:int) = ()
            member x.ExtensionIndexer1 with get(idx:int) = idx
            member x.ExtensionIndexer2 with set(idx:int) (v:int) = ()

        type System.Int32 with 
            member x.Int32ExtensionMethod0()  = 3
            member x.Int32ExtensionMethod1()  = ()
            member x.Int32ExtensionMethod2(y:int)  = y
            member x.Int32ExtensionMethod3(y:int)  = ()
            member x.Int32ExtensionMethod4(y:int,z:int)  = y + z
            member x.Int32ExtensionMethod5(y:(int*int))  = y 
            member x.Int32ExtensionProperty1 = 3
            member x.Int32ExtensionProperty2 with get() = 3
            member x.Int32ExtensionProperty3 with set(v:int) = ()
            member x.Int32ExtensionIndexer1 with get(idx:int) = idx
            member x.Int32ExtensionIndexer2 with set(idx:int) (v:int) = ()

        let v = new obj()
        checkEval "ecnowe0" (<@ v.ExtensionMethod0() @>)  3
        checkEval "ecnowe2" (<@ v.ExtensionMethod2(3) @>) 3
        checkEval "ecnowe4" (<@ v.ExtensionMethod4(3,4) @>)  7
        checkEval "ecnowe5" (<@ v.ExtensionMethod5(3,4) @>)  (3,4)
        checkEval "ecnowe6" (<@ v.ExtensionProperty1 @>) 3
        checkEval "ecnowe7" (<@ v.ExtensionProperty2 @>) 3
        checkEval "ecnowe9" (<@ v.ExtensionIndexer1(3) @>) 3

        check "ecnoweb" (Eval (<@ v.ExtensionMethod0 @>) ()) 3 
        check "ecnowed" (Eval (<@ v.ExtensionMethod2 @>) 3) 3
        check "ecnowef" (Eval (<@ v.ExtensionMethod4 @>) (3,4)) 7
        check "ecnoweg" (Eval (<@ v.ExtensionMethod5 @>) (3,4)) (3,4)

        let v2 = 3
        let mutable v2b = 3
        checkEval "ecnweh0" (<@ v2.ExtensionMethod0() @>) 3
        checkEval "ecnweh2" (<@ v2.ExtensionMethod2(3) @>) 3
        checkEval "ecnweh4" (<@ v2.ExtensionMethod4(3,4) @>) 7
        checkEval "ecnweh5" (<@ v2.ExtensionMethod5(3,4) @>) (3,4)
        checkEval "ecnweh6" (<@ v2.ExtensionProperty1 @>) 3
        checkEval "ecnweh7" (<@ v2.ExtensionProperty2 @>) 3
        checkEval "ecnweh9" (<@ v2.ExtensionIndexer1(3) @>) 3



    module NullableAddInt = 
        open Microsoft.FSharp.Linq.NullableOperators

        checkEval "addip2oin209v304" <@ 2 +? Nullable 3 @> (Nullable 5)
        checkEval "addip2oin209v315" <@ 3 +? Nullable 3 @> (Nullable 6)
        checkEval "addip2oin209v316" <@ 4 +? Nullable 3 @> (Nullable 7)
        checkEval "addip2oin209v337" <@ 3 +? Nullable() @> (Nullable ())

        checkEval "addip2oin209v304" <@ Nullable 2 ?+ 3 @> (Nullable 5)
        checkEval "addip2oin209v315" <@ Nullable 3 ?+ 3 @> (Nullable 6)
        checkEval "addip2oin209v316" <@ Nullable 4 ?+ 3 @> (Nullable 7)
        checkEval "addip2oin209v337" <@ Nullable () ?+ 3 @> (Nullable ())

        checkEval "addip2oin209v30a" <@ Nullable 2 ?+? Nullable 3 @> (Nullable 5)
        checkEval "addip2oin209v31s" <@ Nullable 3 ?+? Nullable 3 @> (Nullable 6)
        checkEval "addip2oin209v31d" <@ Nullable 4 ?+? Nullable 3 @> (Nullable 7)
        checkEval "addip2oin209v33f" <@ Nullable () ?+? Nullable 3 @> (Nullable ())

        checkEval "addip2oin209v30k" <@ Nullable 2 ?+? Nullable () @> (Nullable ())
        checkEval "addip2oin209v31l" <@ Nullable 3 ?+? Nullable () @> (Nullable ())
        checkEval "addip2oin209v31z" <@ Nullable 4 ?+? Nullable () @> (Nullable ())
        checkEval "addip2oin209v33x" <@ Nullable () ?+? Nullable () @> (Nullable ())

        // Some tests to checkEval the type inference when the left and right types are not identical
        let now = System.DateTime.Now
        checkEval "addip2oin209v304dt" <@ Nullable now ?+ System.TimeSpan.Zero @> (Nullable now)
        checkEval "addip2oin209v304dt" <@ now +? Nullable System.TimeSpan.Zero @> (Nullable now)
        checkEval "addip2oin209v304dt" <@ now +? Nullable () @> (Nullable ())
        checkEval "addip2oin209v30adt" <@ Nullable now ?+? Nullable System.TimeSpan.Zero @> (Nullable now)
        checkEval "addip2oin209v30kdt" <@ Nullable now ?+? Nullable () @> (Nullable ())

    module NullableAddIntMeasure = 
        open Microsoft.FSharp.Linq.NullableOperators
        open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

        checkEval "addip2oin209v304" <@ 2<kg> +? Nullable 3<kg> @> (Nullable 5<kg>)
        checkEval "addip2oin209v304" <@ Nullable 2<m> ?+ 3<m> @> (Nullable 5<m>)

    module NullableAddDouble = 
        open Microsoft.FSharp.Linq.NullableOperators

        checkEval "adddp2oin209v304" <@ 2.0 +? Nullable 3.0 @> (Nullable 5.0)
        checkEval "adddp2oin209v315" <@ 3.0 +? Nullable 3.0 @> (Nullable 6.0)
        checkEval "adddp2oin209v316" <@ 4.0 +? Nullable 3.0 @> (Nullable 7.0)
        checkEval "adddp2oin209v337" <@ 3.0 +? Nullable() @> (Nullable ())

        checkEval "adddp2oin209v304" <@ Nullable 2.0 ?+ 3.0 @> (Nullable 5.0)
        checkEval "adddp2oin209v315" <@ Nullable 3.0 ?+ 3.0 @> (Nullable 6.0)
        checkEval "adddp2oin209v316" <@ Nullable 4.0 ?+ 3.0 @> (Nullable 7.0)
        checkEval "adddp2oin209v337" <@ Nullable () ?+ 3.0 @> (Nullable ())

        checkEval "adddp2oin209v30a" <@ Nullable 2.0 ?+? Nullable 3.0 @> (Nullable 5.0)
        checkEval "adddp2oin209v31s" <@ Nullable 3.0 ?+? Nullable 3.0 @> (Nullable 6.0)
        checkEval "adddp2oin209v31d" <@ Nullable 4.0 ?+? Nullable 3.0 @> (Nullable 7.0)
        checkEval "adddp2oin209v33f" <@ Nullable () ?+? Nullable 3.0 @> (Nullable ())

        checkEval "adddp2oin209v30k" <@ Nullable 2.0 ?+? Nullable () @> (Nullable ())
        checkEval "adddp2oin209v31l" <@ Nullable 3.0 ?+? Nullable () @> (Nullable ())
        checkEval "adddp2oin209v31z" <@ Nullable 4.0 ?+? Nullable () @> (Nullable ())
        checkEval "adddp2oin209v33x" <@ Nullable () ?+? Nullable () @> (Nullable ())

        checkText "p2oin209v33x" <@ Nullable 2 ?+? Nullable () @> "(Convert(2) + new Nullable`1())"
        checkText "p2oin209v33x" <@ Nullable 2 ?+ 3 @> "(Convert(2) + Convert(3))"
        checkText "p2oin209v33x" <@ 2 +? Nullable 3 @> "(Convert(2) + Convert(3))"

    module NullableAddDoubleMeasure = 
        open Microsoft.FSharp.Linq.NullableOperators
        open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

        checkEval "adddp2oin209v304" <@ 2.0<kg> +? Nullable 3.0<kg> @> (Nullable 5.0<kg>)
        checkEval "adddp2oin209v304" <@ Nullable 2.0<m> ?+ 3.0<m> @> (Nullable 5.0<m>)
        checkEval "adddp2oin209v31z" <@ Nullable 4.0<s> ?+? Nullable () @> (Nullable ())

    module NullableConversions = 
        open Microsoft.FSharp.Linq
        open Microsoft.FSharp.Linq.NullableOperators
        open Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols

        checkEval "opp2oin209v3041" <@ Nullable.byte (Nullable 2) @> (Nullable 2uy)
        checkEval "opp2oin209v3041" <@ Nullable.uint8  (Nullable 2) @> (Nullable 2uy)
        checkEval "opp2oin209v3042" <@ Nullable.sbyte (Nullable 2) @> (Nullable 2y)
        checkEval "opp2oin209v3042" <@ Nullable.int8 (Nullable 2) @> (Nullable 2y)
        checkEval "opp2oin209v3043" <@ Nullable.uint16(Nullable 2 )@> (Nullable 2us)
        checkEval "opp2oin209v3044" <@ Nullable.int16(Nullable 2 )@> (Nullable 2s)
        checkEval "opp2oin209v3045" <@ Nullable.uint32 (Nullable 2s) @> (Nullable 2u)
        checkEval "opp2oin209v3046" <@ Nullable.int32 (Nullable 2s) @> (Nullable 2)
        checkEval "opp2oin209v3047" <@ Nullable.uint64(Nullable 2 )@> (Nullable 2UL)
        checkEval "opp2oin209v3048" <@ Nullable.int64(Nullable 2 )@> (Nullable 2L)
        checkEval "opp2oin209v3049" <@ Nullable.decimal(Nullable 2 )@> (Nullable 2M)
        checkEval "opp2oin209v304q" <@ Nullable.char(Nullable (int '2') )@> (Nullable '2')
        checkEval "opp2oin209v304w" <@ Nullable.enum(Nullable 2 ): System.Nullable<System.DayOfWeek> @> (Nullable System.DayOfWeek.Tuesday )

        checkEval "opp2oin209v304e" <@ Nullable.sbyte (Nullable 2<kg>) @> (Nullable 2y)
        checkEval "opp2oin209v304e" <@ Nullable.int8 (Nullable 2<kg>) @> (Nullable 2y)
        checkEval "opp2oin209v304r" <@ Nullable.int16 (Nullable 2<kg>) @> (Nullable 2s)
        checkEval "opp2oin209v304t" <@ Nullable.int32 (Nullable 2s<kg>) @> (Nullable 2)
        checkEval "opp2oin209v304y" <@ Nullable.int64 (Nullable 2<kg>) @> (Nullable 2L)
        checkEval "opp2oin209v304u" <@ Nullable.float (Nullable 2<kg>) @> (Nullable 2.0)
        checkEval "opp2oin209v304u" <@ Nullable.double (Nullable 2<kg>) @> (Nullable 2.0)
        checkEval "opp2oin209v304i" <@ Nullable.float32 (Nullable 2<kg>) @> (Nullable 2.0f)
        checkEval "opp2oin209v304i" <@ Nullable.single (Nullable 2<kg>) @> (Nullable 2.0f)



    module NullableMinus = 

        open Microsoft.FSharp.Linq.NullableOperators

        checkEval "minusp2oin209v304" <@ 2 -? Nullable 3 @> (Nullable -1)
        checkEval "minusp2oin209v315" <@ 3 -? Nullable 3 @> (Nullable 0)
        checkEval "minusp2oin209v316" <@ 4 -? Nullable 3 @> (Nullable 1)
        checkEval "minusp2oin209v337" <@ 3 -? Nullable() @> (Nullable ())

        checkEval "minusp2oin209v304" <@ Nullable 2 ?- 3 @> (Nullable -1)
        checkEval "minusp2oin209v315" <@ Nullable 3 ?- 3 @> (Nullable 0)
        checkEval "minusp2oin209v316" <@ Nullable 4 ?- 3 @> (Nullable 1)
        checkEval "minusp2oin209v337" <@ Nullable () ?- 3 @> (Nullable ())

        checkEval "minusp2oin209v30a" <@ Nullable 2 ?-? Nullable 3 @> (Nullable -1)
        checkEval "minusp2oin209v31s" <@ Nullable 3 ?-? Nullable 3 @> (Nullable 0)
        checkEval "minusp2oin209v31d" <@ Nullable 4 ?-? Nullable 3 @> (Nullable 1)
        checkEval "minusp2oin209v33f" <@ Nullable () ?-? Nullable 3 @> (Nullable ())

        checkEval "minusp2oin209v30k" <@ Nullable 2 ?-? Nullable () @> (Nullable ())
        checkEval "minusp2oin209v31l" <@ Nullable 3 ?-? Nullable () @> (Nullable ())
        checkEval "minusp2oin209v31z" <@ Nullable 4 ?-? Nullable () @> (Nullable ())
        checkEval "minusp2oin209v33x" <@ Nullable () ?-? Nullable () @> (Nullable ())

        // Some tests to checkEval the type inference when the left and right types are not identical
        let now = System.DateTime.Now
        checkEval "minusp2oin209v304dt" <@ Nullable now ?- System.TimeSpan.Zero @> (Nullable now)
        checkEval "minusp2oin209v304dt" <@ now -? Nullable System.TimeSpan.Zero @> (Nullable now)
        checkEval "minusp2oin209v304dt" <@ now -? Nullable<System.TimeSpan> () @> (Nullable ())
        checkEval "minusp2oin209v30adt" <@ Nullable now ?-? Nullable System.TimeSpan.Zero @> (Nullable now)
        checkEval "minusp2oin209v30kdt" <@ Nullable now ?-? Nullable<System.TimeSpan> () @> (Nullable ())

        checkText "p2oin209v33x" <@ Nullable 2 ?-? Nullable () @> "(Convert(2) - new Nullable`1())"
        checkText "p2oin209v33x" <@ Nullable 2 ?- 3 @> "(Convert(2) - Convert(3))"
        checkText "p2oin209v33x" <@ 2 -? Nullable 3 @> "(Convert(2) - Convert(3))"

    module NullableMultiply = 

        open Microsoft.FSharp.Linq.NullableOperators

        checkEval "multp2oin209v304" <@ 2 *? Nullable 3 @> (Nullable 6)
        checkEval "multp2oin209v315" <@ 3 *? Nullable 3 @> (Nullable 9)
        checkEval "multp2oin209v316" <@ 4 *? Nullable 3 @> (Nullable 12)
        checkEval "multp2oin209v337" <@ 3 *? Nullable() @> (Nullable ())

        checkEval "multp2oin209v304" <@ Nullable 2 ?* 3 @> (Nullable 6)
        checkEval "multp2oin209v315" <@ Nullable 3 ?* 3 @> (Nullable 9)
        checkEval "multp2oin209v316" <@ Nullable 4 ?* 3 @> (Nullable 12)
        checkEval "multp2oin209v337" <@ Nullable () ?* 3 @> (Nullable ())

        checkEval "multp2oin209v30a" <@ Nullable 2 ?*? Nullable 3 @> (Nullable 6)
        checkEval "multp2oin209v31s" <@ Nullable 3 ?*? Nullable 3 @> (Nullable 9)
        checkEval "multp2oin209v31d" <@ Nullable 4 ?*? Nullable 3 @> (Nullable 12)
        checkEval "multp2oin209v33f" <@ Nullable () ?*? Nullable 3 @> (Nullable ())

        checkEval "multp2oin209v30k" <@ Nullable 2 ?*? Nullable () @> (Nullable ())
        checkEval "multp2oin209v31l" <@ Nullable 3 ?*? Nullable () @> (Nullable ())
        checkEval "multp2oin209v31z" <@ Nullable 4 ?*? Nullable () @> (Nullable ())
        checkEval "multp2oin209v33x" <@ Nullable () ?*? Nullable () @> (Nullable ())

        checkText "p2oin209v33x" <@ Nullable 2 ?*? Nullable () @> "(Convert(2) * new Nullable`1())"
        checkText "p2oin209v33x" <@ Nullable 2 ?* 3 @> "(Convert(2) * Convert(3))"
        checkText "p2oin209v33x" <@ 2 *? Nullable 3 @> "(Convert(2) * Convert(3))"


    module NullableDivide = 

        open Microsoft.FSharp.Linq.NullableOperators

        checkEval "divp2oin209v304" <@ 2 /? Nullable 3 @> (Nullable 0)
        checkEval "divp2oin209v315" <@ 3 /? Nullable 3 @> (Nullable 1)
        checkEval "divp2oin209v316" <@ 4 /? Nullable 3 @> (Nullable 1)
        checkEval "divp2oin209v337" <@ 3 /? Nullable() @> (Nullable ())

        checkEval "divp2oin209v304" <@ Nullable 2 ?/ 3 @> (Nullable 0)
        checkEval "divp2oin209v315" <@ Nullable 3 ?/ 3 @> (Nullable 1)
        checkEval "divp2oin209v316" <@ Nullable 4 ?/ 3 @> (Nullable 1)
        checkEval "divp2oin209v337" <@ Nullable () ?/ 3 @> (Nullable ())

        checkEval "divp2oin209v30a" <@ Nullable 2 ?/? Nullable 3 @> (Nullable 0)
        checkEval "divp2oin209v31s" <@ Nullable 3 ?/? Nullable 3 @> (Nullable 1)
        checkEval "divp2oin209v31d" <@ Nullable 4 ?/? Nullable 3 @> (Nullable 1)
        checkEval "divp2oin209v33f" <@ Nullable () ?/? Nullable 3 @> (Nullable ())

        checkEval "divp2oin209v30k" <@ Nullable 2 ?/? Nullable () @> (Nullable ())
        checkEval "divp2oin209v31l" <@ Nullable 3 ?/? Nullable () @> (Nullable ())
        checkEval "divp2oin209v31z" <@ Nullable 4 ?/? Nullable () @> (Nullable ())
        checkEval "divp2oin209v33x" <@ Nullable () ?/? Nullable () @> (Nullable ())

        checkText "p2oin209v33x" <@ Nullable 2 ?/? Nullable () @> "(Convert(2) / new Nullable`1())"
        checkText "p2oin209v33x" <@ Nullable 2 ?/ 3 @> "(Convert(2) / Convert(3))"
        checkText "p2oin209v33x" <@ 2 /? Nullable 3 @> "(Convert(2) / Convert(3))"

    module NullableModulo = 

        open Microsoft.FSharp.Linq.NullableOperators

        checkEval "modp2oin209v304" <@ 2 %? Nullable 3 @> (Nullable 2)
        checkEval "modp2oin209v315" <@ 3 %? Nullable 3 @> (Nullable 0)
        checkEval "modp2oin209v316" <@ 4 %? Nullable 3 @> (Nullable 1)
        checkEval "modp2oin209v337" <@ 3 %? Nullable() @> (Nullable ())

        checkEval "modp2oin209v304" <@ Nullable 2 ?% 3 @> (Nullable 2)
        checkEval "modp2oin209v315" <@ Nullable 3 ?% 3 @> (Nullable 0)
        checkEval "modp2oin209v316" <@ Nullable 4 ?% 3 @> (Nullable 1)
        checkEval "modp2oin209v337" <@ Nullable () ?% 3 @> (Nullable ())

        checkEval "modp2oin209v30a" <@ Nullable 2 ?%? Nullable 3 @> (Nullable 2)
        checkEval "modp2oin209v31s" <@ Nullable 3 ?%? Nullable 3 @> (Nullable 0)
        checkEval "modp2oin209v31d" <@ Nullable 4 ?%? Nullable 3 @> (Nullable 1)
        checkEval "modp2oin209v33f" <@ Nullable () ?%? Nullable 3 @> (Nullable ())

        checkEval "modp2oin209v30k" <@ Nullable 2 ?%? Nullable () @> (Nullable ())
        checkEval "modp2oin209v31l" <@ Nullable 3 ?%? Nullable () @> (Nullable ())
        checkEval "modp2oin209v31z" <@ Nullable 4 ?%? Nullable () @> (Nullable ())
        checkEval "modp2oin209v33x" <@ Nullable () ?%? Nullable () @> (Nullable ())


        checkText "p2oin209v33x" <@ Nullable 2 ?%? Nullable () @> "(Convert(2) % new Nullable`1())"
        checkText "p2oin209v33x" <@ Nullable 2 ?% 3 @> "(Convert(2) % Convert(3))"


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
