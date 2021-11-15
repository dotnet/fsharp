// #Conformance #LetBindings #Recursion #TypeInference #ObjectConstructors #Classes #Records 
#if TESTS_AS_APP
module Core_letrec
#endif

let failures = ref []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures := !failures @ [s]


let test t s1 s2 = 
  if s1 <> s2 then 
    report_failure ("test "+t+" failed")
  else
    stdout.WriteLine ("test "+t+" succeeded")   




(* --------------------------------------------------------------------
 * Nested letrecs
 * -------------------------------------------------------------------- *)

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
 * Recursion through constuctors
 * -------------------------------------------------------------------- *)


type myRecType = { f1: int; f2: myRecType }
let rec x = { f1=3; f2=x }
let rec y1 = { f1=3; f2=y2 }
and y2 = { f1=3; f2=y1 }
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
let rec z1 = { g1=3; g2= { contents = z2 } }
and z2 = { g1=4; g2={ contents = z1 } }

do if z2.g1 <> 4 then report_failure "ewieds32w8w"
do if z2.g2.contents.g1 <> 3 then report_failure "ceewieds32w8w"


(* --------------------------------------------------------------------
 * Recursion through constuctors
 * -------------------------------------------------------------------- *)

(* TYPEERROR: let rec a = a *)

let rec a1 = 1 
and b = a1
do if a1 <> 1 then report_failure "celkewieds32w8w"
do if b <> a1 then report_failure "cel3f98u8w"



let rec a2 = test "grekjre" (b2 + 1 ) 3
and b2  = 2

(* TYPEERROR: let rec a3 = b3 and b3 = c3 and c3 = a3 *)


let nonRecursiveImmediate () = 
  stdout.WriteLine "Testing nonRecursiveImmediate";
  let x = ref 1 in 
  let rec a1 = (x := 3; !x) 
  and b = a1 in 
  if a1 <> 3 then report_failure "dqwij";
  if b <> 3 then report_failure "dqwecqwij"

do nonRecursiveImmediate()
do nonRecursiveImmediate()

let rec recObj = {new System.Object() with member __.GetHashCode() = (recObj.ToString()).Length}

do Printf.printf "recObj.GetHashCode() = %d\n" (recObj.GetHashCode())
do Printf.printf "recObj.ToString() = %s\n" (recObj.ToString())
do if recObj.GetHashCode() <> (recObj.ToString()).Length then report_failure "dqwij"


let WouldFailAtRuntimeTest () = 
  let rec a2 = (fun x -> stdout.WriteLine "a2app"; stderr.Flush();  a2 + 2) (stdout.WriteLine "a2arg"; stderr.Flush(); 1) in 
  a2

do try WouldFailAtRuntimeTest (); report_failure "fwoi-03" with _ -> stdout.WriteLine "caught ok!"

let WouldFailAtRuntimeTest2 () = 
  let rec a2 = (fun x -> a3 + 2) 1 
  and a3 = (fun x -> a2 + 2) 1 in 
  a2 + a3

#if !TESTS_AS_APP && !NETCOREAPP
open System
open System.Windows.Forms

let rec mnuiSayHello : MenuItem = 
  new MenuItem("&Say Hello", 
               new EventHandler(fun sender e -> Printf.printf "Hello! Text = %s\n" mnuiSayHello.Text), 
               Shortcut.CtrlH)

(* Check that type annotations are proagated outer-to-inner *)
let testTypeAnnotationForRecursiveBinding () = 
   let rec x = 1
   and _a = form.Menu <- new MainMenu();
   and form : Form = new Form() 
   and _b = form.Text <- "Hello" in 
   form
#endif

(* --------------------------------------------------------------------
 * Inner recursion where some items go TLR and others do not
 * -------------------------------------------------------------------- *)

(* TLR letrec, with only some functions going TLR.
   Required optimisations off to hit bug.
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

module PartiallyPolymorphicLetRecTest = begin

  let rec f x = g (fun y -> ())
  and g h = ()


  let rec f2 x = g2 (fun y -> ());  g2 (fun z -> ())
  and g2 h = ()
 
  let rec f3 x = g3 (fun y -> ());  g3 (fun z -> ())
  and g3 h = h3 (fun z -> ())
  and h3 h = ()
end


module InitializationGraphAtTopLevel = begin

    let nyi2 (callback) = callback
    let rec aaa = nyi2 (fun () -> ggg(); )
    and ggg ()  = (bbb = false)
    and bbb  = true
      
end


(*
module RandomStrangeCodeFromLewis = begin

  // This code is erroneuos since it compares function values.
  let fix_memo f n =
    let memo = Hashtbl.create 100 in
    let rec fix_memo0 f n = f_memo (fix_memo0 f_memo) n and
        f_memo v =
            match Hashtbl.tryfind memo v with
                None ->
                    let r = f v in
                    Hashtbl.add memo v r;
                    r
                | Some r ->
            r in
    fix_memo0 f n
end
*)

module GeneralizeObjectExpressions = begin

  type ('a,'b) IPattern = interface 
    abstract Query : ('b -> 'a option);
    abstract Make : ('a -> 'b)
  end

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
 
end
module GeneralizeObjectExpressions2 = begin

  type  IPattern<'a,'b> = interface 
    abstract Query : ('b -> 'a option);
    abstract Make : ('a -> 'b)
  end

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
 
end

module RecursiveInterfaceObjectExpressions = begin
 

  type Expr = App of IOp * Expr | Const of float
  and IOp = interface 
    abstract Name : string;
    abstract Deriv : Expr -> Expr 
  end

  let NegOp = { new IOp with member __.Name = "neg" 
                             member __.Deriv(e) = Const (-1.0) }
  let Neg x = App(NegOp,x)
  let rec CosOp = { new IOp with 
                             member __.Name = "cos" 
                             member __.Deriv(e) = Neg(Sin(e)) }
  and     Cos x = App(CosOp,x)
  and     Sin x = App(SinOp,x)
  and     SinOp = { new IOp with 
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

  let CosOp2,Cos2,Sin2,SinOp2,Neg2,NegOp2 = f "abc"
  let One = Const 1.0
  let x = Cos(One)
  let Two  = Const 2.0
  let y = Cos2(Two)

  do if CosOp.Name <> "cos" then report_failure "RecursiveInterfaceObjectExpressions: test 1"
  do if CosOp2.Name <> "abc" then report_failure "RecursiveInterfaceObjectExpressions: test 2"

  
end

#if !TESTS_AS_APP && !NETCOREAPP
module RecursiveInnerConstrainedGenerics = begin

    open System.Windows.Forms

    let f x = 
      let g (c : #Control) = 
         printf "g!, x = %d\n" x;
         printf "g!, x = %d\n" x;
         printf "g!, x = %d\n" x;
         printf "g!, x = %d\n" x;
         printf "g!, x = %d\n" x;
         printf "g!, x = %d\n" x;
         c  in
      // check it's been generalized and that it compiles without generating
      // unverifiable code!!!!!!
      g (new Form()) |> ignore;
      g (new RichTextBox()) |> ignore

    do f 3
    do f 4

    let f2 x = 
      let rec g1 (c : #Control) y = 
         printf "g1!, x = %d, y = %d\n" x y;
         (g2 c (y-1) |> ignore)
      and g2 (c : #Control) y = 
         printf "g2!, x = %d, y = %d\n" x y;
         (if (y >= 0) then (g1 (c) (y-1) |> ignore)) in
      // check it's been generalized and that it compiles without generating
      // unverifiable code!!!!!!
      g1 (new Form()) 3 |> ignore;
      g2 (new RichTextBox()) 3 |> ignore;
      g1 (new RichTextBox()) 3 |> ignore;
      g2 (new Form()) 3 |> ignore

    do f2 3
    do f2 4

end
#endif

module ClassInitTests = 
    // one initial do bindings - raises exception
    type FooFail1() as this =
        do 
            printfn "hi"
            this.Bar()
        let x = 3
        do 
            printfn "bye"
            this.Bar()

        member this.Bar() = printfn "Bar %d" x

    // two initial do bindings - raises exception
    type FooFail2() as this =
        do 
            printfn "hi"
            this.Bar()
        do 
            printfn "hi"
            this.Bar()
        let x = 3
        do 
            printfn "bye"
            this.Bar()

        member this.Bar() = printfn "Bar %d" x
    
    // one initial let _ bindings - raises exception
    type FooFail3() as this =        
        let _ = 
            printfn "hi"
            this.Bar()
        let x = 3
        do 
            printfn "bye"
            this.Bar()

        member this.Bar() = printfn "Bar %d" x

    // one initial let _ bindings then one initial do binding - raises exception
    type FooFail4() as this =
        let _ =
            printfn "hi"
        do 
            printfn "hi"
            this.Bar()
        let x = 3
        do 
            printfn "bye"
            this.Bar()

        member this.Bar() = printfn "Bar %d" x

    // two initial let _ bindings - raises exception
    type FooFail5() as this =
        let _ =
            printfn "hi"
            this.Bar()
        let _ = 
            printfn "hi"
            this.Bar()
        let x = 3
        do 
            printfn "bye"
            this.Bar()

        member this.Bar() = printfn "Bar %d" x

    // one initial let _ bindings then one initial do binding - raises exception
    type FooFail6() as this =
        let _ =
            printfn "hi"
            this.Bar()
        do 
            printfn "hi"
        let x = 3
        do 
            printfn "bye"
            this.Bar()

        member this.Bar() = printfn "Bar %d" x

    
    // no initial do bindings - succeeds
    type FooSucceeds() as this =
        let x = 3
        do 
            printfn "bye"
            this.Bar()

        member this.Bar() = printfn "Bar %d" x

    test "cneqec21" (try new FooFail1() |> ignore; false with :? System.InvalidOperationException -> true) true
    test "cneqec22" (try new FooFail2() |> ignore; false with :? System.InvalidOperationException -> true) true
    test "cneqec23" (try new FooFail3() |> ignore; false with :? System.InvalidOperationException -> true) true
    test "cneqec24" (try new FooFail4() |> ignore; false with :? System.InvalidOperationException -> true) true
    test "cneqec25" (try new FooFail5() |> ignore; false with :? System.InvalidOperationException -> true) true
    test "cneqec26" (try new FooFail6() |> ignore; false with :? System.InvalidOperationException -> true) true
    test "cneqec27" (try new FooSucceeds() |> ignore; false with :? System.InvalidOperationException -> true) false


module BasicPermutations = 

    module Perm1 = 
        let rec A1 = 1 
        and A2 = A1
        
        test "vsdlknv01" (A1,A2) (1,1)
    
    module Perm2 = 
        let rec A1 = A2
        and A2 = 1
        
        test "vsdlknv02" (A1,A2) (1,1)
    
    module Perm3a = 
        let rec A1 = A2
        and A2 = 1
        and A3 = 1
        
        test "vsdlknv03" (A1,A2,A3) (1,1,1)
        
    module Perm3b = 
        let rec A1 = A2
        and A2 = A3
        and A3 = 1
        
        test "vsdlknv04" (A1,A2,A3) (1,1,1)
        
    module Perm3c = 
        let rec A1 = A2
        and A2 = 1
        and A3 = A2
        
        test "vsdlknv05" (A1,A2,A3) (1,1,1)
        
    module Perm3d = 
        let rec A1 = A3
        and A2 = 1
        and A3 = 1
        
        test "vsdlknv06" (A1,A2,A3) (1,1,1)

    module Perm3e = 
        let rec A1 = A3
        and A2 = A3
        and A3 = 1
        
        test "vsdlknv07" (A1,A2,A3) (1,1,1)

    module Perm3f = 
        let rec A1 = A3
        and A2 = 1
        and A3 = A2
        
        test "vsdlknv08" (A1,A2,A3) (1,1,1)

    module Perm3g = 
        let rec A1 = A3
        and A2 = A3
        and A3 = 1
        
        test "vsdlknv09" (A1,A2,A3) (1,1,1)

    module Perm3h = 
        let rec A1 = 1
        and A2 = A1
        and A3 = A2
        
        test "vsdlknv0q" (A1,A2,A3) (1,1,1)

    module Perm3i = 
        let rec A1 = 1
        and A2 = A3
        and A3 = 1
        
        test "vsdlknv0w" (A1,A2,A3) (1,1,1)

    module Perm4i = 
        let rec A1 = A4
        and A2 = 1
        and A3 = A2
        and A4 = A3
        
        test "vsdlknv0e" (A1,A2,A3,A4) (1,1,1,1)

    module PermMisc = 
        let rec A1 = A4 + 1
        and A2 = 1
        and A3 = A2 + 1
        and A4 = A3 + 1
        
        test "vsdlknv0r" (A1,A2,A3,A4) (4,1,2,3)
        
    module bug162155 =

        // VS2010: This works as expected (fsc.exe version: 4.0.30319.1)

        // Note:  The types are mutually recursive with "and"

        //        plus we're NOT using the implicit constructor

        type SuperType1() =

          abstract Foo : int -> int

          default x.Foo a = a + 1

         

        and SubType1() =

          inherit SuperType1()

          override x.Foo a = base.Foo(a)

         

        // VS2010-SP1: Works as expected (fsc.exe version 4.0.40219.1)

        // Note:  The types are not mutually recursive with "and"

        type SuperType2() =

          abstract Foo : int -> int

          default x.Foo a = a + 1

         

        type SubType2 =

          inherit SuperType2

          new () = { inherit SuperType2() }

          override x.Foo a = base.Foo(a)

         

        // VS2010-SP1: Works as expected (fsc.exe version 4.0.40219.1)

        // Note:  We're using the implicit constructor

        type SuperType3() =

         abstract Foo : int -> int

          default x.Foo a = a + 1

         

        type SubType3() =

          inherit SuperType3()

          override x.Foo a = base.Foo(a)



        // VS2010-SP1: This will not work as expected (fsc.exe version 4.0.40219.1)
        // Note:  The types are mutually recursive with "and"
        //        plus we're NOT using the implicit constructor
        //        this causes the base reference to fail to compile
        type SuperType4() =
          abstract Foo : int -> int
          default x.Foo a = a + 1
         
        and SubType4 =
          inherit SuperType4
          new () = { inherit SuperType4() }
         
          // With visual studio SP1 this will not compile
          // with the following error:
          // 
          // Error    1      'base' values may only be used to make direct calls 
          // to the base implementations of overridden members <file> <line>
          override x.Foo a = base.Foo(a)

module Test2 = 
    let tag = 
        let mutable i = 0
        fun _ -> i <- i+1; i // this should _not_ generalize, see https://github.com/Microsoft/visualfsharp/issues/3358

    test "vwekjwve91" (tag()) 1
    test "vwekjwve92" (tag()) 2
    test "vwekjwve93" (tag()) 3

module Test3 = 
    let rec tag = 
        let mutable i = 0
        fun _ -> i <- i+1; i // this should _not_ generalize, see https://github.com/Microsoft/visualfsharp/issues/3358

    test "vwekjwve94" (tag()) 1
    test "vwekjwve95" (tag()) 2
    test "vwekjwve96" (tag()) 3

module Test12384 =
    type Node =
        {
            Next: Node
            Value: int
        }

    let rec one =
        {
            Next = two
            Value = 1
        }

    and two =
        {
            Next = one
            Value = 2
        }
    printfn "%A" one
    printfn "%A" two
    test "cweewlwne1" one.Value 1
    test "cweewlwne2" one.Next.Value 2
    test "cweewlwne3" one.Next.Next.Value 1
    test "cweewlwne4" two.Value 2
    test "cweewlwne5" two.Next.Value 1
    test "cweewlwne6" two.Next.Next.Value 2

module Test12384b =
    type Node =
        {
            Next: Node
            Value: int
        }

    let rec one =
        {
            Next = two
            Value = 1
        }

    and two =
        {
            Next = one
            Value = 2
        }
    // Also test the case where the two recursive bindings occur with a nested module after
    module M =
        let f x = x + 1
        
    printfn "%A" one
    printfn "%A" two
    test "cweewlwne1a" one.Value 1
    test "cweewlwne2a" one.Next.Value 2
    test "cweewlwne3a" one.Next.Next.Value 1
    test "cweewlwne4a" two.Value 2
    test "cweewlwne5a" two.Next.Value 1
    test "cweewlwne6a" two.Next.Next.Value 2

module rec Test12384c =
    type Node =
        {
            Next: Node
            Value: int
        }

    let one =
        {
            Next = two
            Value = 1
        }

    let two =
        {
            Next = one
            Value = 2
        }
    // Also test the case where the two recursive bindings occur with a nested module after
    module M =
        let f x = x + 1
        
    printfn "%A" one
    printfn "%A" two
    test "cweewlwne1b" one.Value 1
    test "cweewlwne2b" one.Next.Value 2
    test "cweewlwne3b" one.Next.Next.Value 1
    test "cweewlwne4b" two.Value 2
    test "cweewlwne5b" two.Next.Value 1
    test "cweewlwne6b" two.Next.Next.Value 2


//Note, this case doesn't initialize successfully because of the intervening module. Tracked by #12384
  
(*
module rec Test12384d =
    type Node =
        {
            Next: Node
            Value: int
        }

    let one =
        {
            Next = two
            Value = 1
        }

    // An intervening module declaration
    module M =
        let x() = one
        
    let two =
        {
            Next = one
            Value = 2
        }

    printfn "%A" one
    printfn "%A" two
    test "cweewlwne1b" one.Value 1
    test "cweewlwne2b" one.Next.Value 2
    test "cweewlwne3b" one.Next.Next.Value 1
    test "cweewlwne1b" (M.x()).Value 1
    test "cweewlwne2b" (M.x()).Next.Value 2
    test "cweewlwne3b" (M.x()).Next.Next.Value 1
    test "cweewlwne4b" two.Value 2
    test "cweewlwne5b" two.Next.Value 1
    test "cweewlwne6b" two.Next.Next.Value 2
*)

module rec Test12384e =
    type Node =
        {
            Next: Node
            Value: int
        }

    let one =
        {
            Next = two
            Value = 1
        }

    // An intervening type declaration
    type M() =
        static member X() = one
        
    let two =
        {
            Next = one
            Value = 2
        }

    printfn "%A" one
    printfn "%A" two
    test "cweewlwne1b" one.Value 1
    test "cweewlwne2b" one.Next.Value 2
    test "cweewlwne3b" one.Next.Next.Value 1
    test "cweewlwne1b" (M.X()).Value 1
    test "cweewlwne2b" (M.X()).Next.Value 2
    test "cweewlwne3b" (M.X()).Next.Next.Value 1
    test "cweewlwne4b" two.Value 2
    test "cweewlwne5b" two.Next.Value 1
    test "cweewlwne6b" two.Next.Next.Value 2

#if TESTS_AS_APP
let RUN() = !failures
#else
let aa =
  match !failures with 
  | [] -> 
      stdout.WriteLine "Test Passed"
      System.IO.File.WriteAllText("test.ok","ok")
      exit 0
  | _ -> 
      stdout.WriteLine "Test Failed"
      exit 1
#endif

