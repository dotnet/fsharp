// #Conformance #LetBindings #Recursion #TypeInference #ObjectConstructors #Classes #Records 

module rec Core_letrec_mutrec

let failures = ref false
let report_failure s = 
  stderr.WriteLine ("FAIL: "+s); failures := true

let test t s1 s2 = 
  if s1 <> s2 then 
    (stderr.WriteLine ("test "+t+" failed");
     failures := true)
  else
    stdout.WriteLine ("test "+t+" succeeded")   


let f = 
  let x = ref 0 in 
  fun () ->
    x := !x + 1;
    let rec g n = if (n = 0) then 1 else h(n-1)
    and h n = if (n = 0) then 2 else g(n-1) in 
    g !x

do if f() <> 2 then report_failure "ewiucew" 
do if f() <> 1 then report_failure "ewiew8w" 


let nestedInnerRec2 = 
  let x = ref 0 in 
  fun () ->
    x := !x + 1;
    let rec g n = if (n = 0) then !x + 100 else h(n-1)
    and h n = if (n = 0) then !x + 200 else g(n-1) in 
    g !x

do if nestedInnerRec2() <> 201 then report_failure "ewiucew"
do if nestedInnerRec2() <> 102 then report_failure "ewiew8w"



(* --------------------------------------------------------------------
 * Recursion through constructors
 * -------------------------------------------------------------------- *)


type myRecType = { f1: int; f2: myRecType }
let x = { f1=3; f2=x }
let y1 = { f1=3; f2=y2 }
let y2 = { f1=3; f2=y1 }
let f2 = 
  let x = ref 0 in 
  fun () ->
    x := !x + 1;
    let rec y = { f1=3; f2=y } in
    y

do if (f2()).f1 <> 3 then report_failure "ewi3dw8w"
do if (f2()).f2.f1 <> 3 then report_failure "dwc8w"


(* TYPEERROR: let rec a = 1 :: a *)

type myRecType2 = { g1: int; g2: myRecType2 ref }
let z1 = { g1=3; g2= { contents = z2 } }
let z2 = { g1=4; g2={ contents = z1 } }

do if z2.g1 <> 4 then report_failure "ewieds32w8w"
do if z2.g2.contents.g1 <> 3 then report_failure "ceewieds32w8w"


(* --------------------------------------------------------------------
 * Recursion through constructors
 * -------------------------------------------------------------------- *)

(* --------------------------------------------------------------------
 * Inner recursion where some items go TLR and others do not
 * -------------------------------------------------------------------- *)

(* TLR letrec, with only some functions going TLR.
   Required optimizations off to hit bug.
   Fix: use SELF-CARRYING env values.
*)

let apply f x = f x
let dec n = (n:int) (* -1 *)
  
let inner () =
  let rec
      odd  n = if n=1 then true else not (even (dec n))
  and even n = if n=0 then true else not (apply odd (dec n))
  in
  even 99

(* --------------------------------------------------------------------
 * Polymorphic letrec where not all bindings get qualified by all type 
 * variables.  Surprisingly hard to get right in a type-preserving
 * compiler.
 * -------------------------------------------------------------------- *)

module PartiallyPolymorphicLetRecTest = 

  let f x = g (fun y -> ())
  let g h = ()


  let f2 x = g2 (fun y -> ());  g2 (fun z -> ())
  let g2 h = ()
 
  let f3 x = g3 (fun y -> ());  g3 (fun z -> ())
  let g3 h = h3 (fun z -> ())
  let h3 h = ()


module GeneralizeObjectExpressions = 

  type IPattern<'a,'b> = 
    abstract Query : ('b -> 'a option)
    abstract Make : ('a -> 'b)

  let ApCons = { new IPattern<'a * 'a list, 'a list> with 
                           member __.Query = fun xs -> match xs with y::ys -> Some (y,ys) | _ -> None 
                           member __.Make = fun (x,xs) -> x::xs }
  let ApNil = { new IPattern<unit, 'a list>  with 
                           member __.Query = fun xs -> match xs with [] -> Some () | _ -> None 
                           member __.Make = fun () -> [] }

  let x1 = ApCons : IPattern<int * int list, int list>
  let x2 = ApCons : IPattern<string * string list, string list> 
  let y1 = ApNil : IPattern<unit, int list>
  let y2 = ApNil : IPattern<unit, string list>
 
module GeneralizeObjectExpressions2 = 

  type  IPattern<'a,'b> = 
    abstract Query : ('b -> 'a option)
    abstract Make : ('a -> 'b)

  let rec ApCons = { new IPattern<'a * 'a list, 'a list> with 
                          member __.Query = fun xs -> match xs with y::ys -> Some (y,ys) | _ -> None 
                          member __.Make = fun (x,xs) -> x::xs }
  and ApNil = { new IPattern<unit, 'a list>  with 
                          member __.Query = fun xs -> match xs with [] -> Some () | _ -> None 
                          member __.Make = fun () -> [] }

  let x1 = ApCons : IPattern<int * int list, int list>
  let x2 = ApCons : IPattern<string * string list, string list>
  let y1 = ApNil : IPattern<unit, int list>
  let y2 = ApNil : IPattern<unit, string list>
 

module RecursiveInterfaceObjectExpressions = 
 
  type Expr = App of IOp * Expr | Const of float
  type IOp = 
    abstract Name : string;
    abstract Deriv : Expr -> Expr 
  

  let NegOp = { new IOp with member __.Name = "neg" 
                             member __.Deriv(e) = Const (-1.0) }
  let Neg x = App(NegOp,x)
  let CosOp = { new IOp with 
                             member __.Name = "cos" 
                             member __.Deriv(e) = Neg(Sin(e)) }
  let     Cos x = App(CosOp,x)
  let     Sin x = App(SinOp,x)
  let     SinOp = { new IOp with 
                             member __.Name = "sin" 
                             member __.Deriv(e) = Cos(e) }
 

  let f nm = 
    let NegOp = { new IOp with member __.Name = nm 
                               member __.Deriv(e) = Const (-1.0) } in
    let Neg x = App(NegOp,x) in
    let rec CosOp = { new IOp with 
                               member __.Name = nm 
                               member __.Deriv(e) = Neg(Sin(e)) } 
    and     Cos x = App(CosOp,x)
    and     Sin x = App(SinOp,x)
    and     SinOp = { new IOp with member __.Name = nm 
                                   member __.Deriv(e) = Cos(e) } in 
    CosOp,Cos,Sin,SinOp,Neg,NegOp

  let ops = f "abc"
  let CosOp2 = (let CosOp2,Cos2,Sin2,SinOp2,Neg2,NegOp2 = ops in CosOp2)
  let Cos2 = (let CosOp2,Cos2,Sin2,SinOp2,Neg2,NegOp2 = ops in Cos2)
  let One = Const 1.0
  let x = Cos(One)
  let Two  = Const 2.0
  let y = Cos2(Two)

  do if CosOp.Name <> "cos" then report_failure "RecursiveInterfaceObjectExpressions: test 1"
  do if CosOp2.Name <> "abc" then report_failure "RecursiveInterfaceObjectExpressions: test 2"

#if TESTS_AS_APP
let aa = 
    if !failures then (stdout.WriteLine "Test Failed"; exit 1) 
    else (stdout.WriteLine "Test Passed"; exit 0)
#else
do 
  if !failures then (stdout.WriteLine "Test Failed"; exit 1) 

do (stdout.WriteLine "Test Passed"; 
    printf "TEST PASSED OK"; 
    exit 0)
#endif
