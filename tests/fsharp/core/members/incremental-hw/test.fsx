// #Conformance #MemberDefinitions #Mutable #ObjectOrientedTypes #Classes #InterfacesAndImplementations #Recursion 

#light

#if TESTS_AS_APP
module Core_members_incremental_testhw
#endif

//! Setup

let failures = ref []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures := !failures @ [s]

let test (s : string) b = 
    stderr.Write(s)
    if b then stderr.WriteLine " OK"
    else report_failure (s)

let check s b1 b2 = test s (b1 = b2)


//! Address of incremental  local mutable
  
module AddressOfIncrementalClassLocalMutable = 

    open System.Drawing
    open System.Windows.Forms
    let rr = new Rectangle()               // <-------- value type
    type LocalMutableTestA() = 
      let mutable mvarA  = rr
      member x.MethodA() = mvarA.X         // <-------- require address of it for arg to getter
    



//! Address of mutable record field (related to above)

module AddressOfMutableRecordField = 
    open System.Drawing
    open System.Windows.Forms

    type RectR = { mutable field : Rectangle }
    let f (x:RectR) = x.field.X <- 10
    let boxR = {field = new Rectangle() }
    do  assert(boxR.field.X=0)  
    do  f boxR
    do  assert(boxR.field.X=10)



//! Minor test

module MinorTest = 
    type A<'a>(x) = 
        
            let (y:'a) = x
            member this.X = y
        

    let x1 = new A<string>("abc")    
    let x2 = new A<int>(3)
    let x3 = new A<int64>(3L)



//! Misc

module Misc = 
    type 'a Area9aa(x) =
    
        let a = (x : 'a)
        let f (y:'a) = y
    

    type AList(a) = 
      let x1 = a + 1
      let y2 = x1 + 1
      let y3 = 3
      member pairs_this.Pair() = x1,y2
    
    let a = new AList 12
    do  assert( a.Pair() = (13,14) )



//! Wire previously
    
(* accepted *)
module WireOld = 
    [<AbstractClass>]
    type 'a wire =
    
      abstract Send   : 'a -> unit
      abstract Listen : ('a -> unit) -> unit
      new () = {}
      member self.Event = self :> 'a IEvent
      interface IEvent<'a> with
          member x.Subscribe(handler) = failwith "nyi"
          member x.AddHandler(handler) = failwith "nyi"
          member x.RemoveHandler(handler) = failwith "nyi"
      
    
    let createWire() =
      let listeners = ref [] in
      {new wire<'a>() with
         member __.Send(x)   = List.iter (fun f -> f x) !listeners
         member __.Listen(f) = listeners := f :: !listeners
      }



//! Wire variations

module WireVariations = 
    (* Accepted *)    
    type wire2(z) =
      
         let z = z
         member this.Send(x) = 1 + z + x
      

    (* Accepted *)
    type 'a wire3(z) =
      
         let listeners = (z:'a)
         member this.Send(x:'a) = x
      

    (* Accepted *)    
    type wire4<'a>(z) =
      
         let listeners = let z:'a = z in ref ([] : ('a -> unit) list)
         member this.Send(x:'a)           = List.iter (fun f -> f x) !listeners
         member this.Listen(f:'a -> unit) = listeners := f :: !listeners
      

    (* Accepted *)    
    type 'a wire5(z) =
      
         let listeners = ref ([] : ('a -> unit) list)
         member this.Send(x:'a)           = let z:'a = z in List.iter (fun f -> f x) !listeners
         member this.Listen(f:'a -> unit) = listeners := f :: !listeners
      

    (* Accepted now - fixed tinst missing error *)
    type 'a wire6(z) =
      
         let mutable listeners = ([] : ('a -> unit) list)
         member this.Send(x:'a)           = let z:'a = z in List.iter (fun f -> f x) listeners
         member this.Listen(f:'a -> unit) = listeners <- f :: listeners
    

    (* OK, now this types are asserted equal *)
    type 'a wire7(z) =
      
         let listeners  = 12
         let z : 'a = z
         member this.SendA(x:int) = this,listeners
         member this.SendB(x:int) = this,listeners       
      

    (* Soundness concern: should uses of let-fields force a member "this" to be ('a wire) typed? *)
    type 'a wire8(z) =
      
         let z : 'a = z  
         let listeners  = 12
         member this.SendA(x:int) = x,z,(this: 'a wire8)
         member this.SendB(x:int) = x,z,(this: 'a wire8)
      

    (* Accepted *)    
    type 'a wire9(z) =
      
         let mutable listeners  = ([] : int list)
         let z : 'a = z
         member this.Send(x:int) =
           let ls : int list = 1 :: 2 :: listeners in
           List.map (fun x -> x+1) ls
    

    (* Accepted *)    
    type 'a wire10(z) =
      
         let mutable listeners  = ([] : int list)
         let z : 'a = z
         member this.Send(x:int) =
           let ls : int list = listeners @ [1] in  (* it seems listeners may have the wrong type... *)
           List.map (fun f -> f) ls
    

    (* Accepted *)    
    type 'a wire11(z) =
      
         let listeners  = let z:int = z in ([] : int list)
         member this.Send(x:'b) = (* List.iter (fun f -> ()) listeners *)
             let xx : 'b wire11 = this in      
             Some (x,List.head listeners)
    

    (* Accepted *)    
    type 'a wire12(z) =
      
         let listeners  = let z:'a = z in ([] : int list)
         member this.Send(x:'b) = (* List.iter (fun f -> ()) listeners *)
             Some (this,x,List.head listeners)
    

    (* OK, after avoid the value restriction *)    
    type wire13(z) =
      
         member this.Send(x:'b) = (ignore (z:int)); Some (this,x)
    

    (* Accepted *)
    type 'a wire14(z) =
      
         let mutable listeners  = let z:int = z in ([] : int list)
         member this.Send(x:'a) = (* List.iter (fun f -> ()) listeners *)
             listeners
    



//! Area variations
  
#if !NETCOREAPP
module AreaVariations = 
    (* Accepted *)
    open System.Drawing
    open System.Windows.Forms
    open WireOld
    
    type Area1(x) =
    
        inherit Panel()
        let x = x * 2
    

    type Area2(x) =
    
        inherit Panel()
        let x = x * 2      
        let mutable Bitmap = new Bitmap(100,100)
        let mutable GC     = Graphics.FromImage(Bitmap)
        let fff (x:Area2) = x.Resize ()      
        member this.Resize  () = Bitmap <- new Bitmap(10,20)
    

    type Area3(x) =
    
        let basePanel = let x = x:unit in new Panel()
        let mutable Bitmap = new Bitmap(100,100)
        let mutable GC     = Graphics.FromImage(Bitmap)
        let fff (x:Area3) = x.Resize ()      
        member this.Resize  () = Bitmap <- new Bitmap(10,20)
    

    type Area4 =
      
        inherit Panel 
        member this.X() = base.Dock
    

    type Area5(x) =
    
        inherit Panel() 
        let x = 1 + x
          (**)
        let mutable Bitmap = new Bitmap(100,100)
        let mutable GC     = Graphics.FromImage(Bitmap)   (* GC for bitmap *)
        let resizedWire    = createWire() : (int*int) wire
        let mutable Count  = 0
          (**)
        do base.BackColor <- Color.Black
        do base.Dock      <- DockStyle.Fill
        do base.Cursor    <- Cursors.Cross
    //  do base.Resize.Add(fun arg  -> area.resize ())                    (* use before defn risk *)
    //  do base.Paint .Add(fun args -> area.redrawDrawArea args.Graphics) (* use before defn risk *)
          (**)
        member this.ResizedE = resizedWire.Event
          (**)
        member this.Resize () =
          let ww,hh = base.Width,base.Height in
          if ww>0 && hh>0 then (
                Bitmap <- new Bitmap(ww,hh);
                GC     <- Graphics.FromImage(Bitmap);
                resizedWire.Send((ww,hh))
          )
          (**)
    //  TODO: need to be able to call this.PriorMethod without this.qualification
    //  do base.Resize.Add(fun arg  -> Resize ())                         (* no risk *)  
          (**)
        member this.RedrawDrawArea (gc:Graphics) = gc.DrawImage(Bitmap,new Point(0,0))
    //  member this.Refresh() = using (base.CreateGraphics()) (fun gc ->      RedrawDrawArea gc)   
        member this.Refresh2() = using (base.CreateGraphics()) (fun gc -> this.RedrawDrawArea gc)        
        (**)
    //  do base.Paint.Add(fun args -> RedrawDrawArea args.Graphics)       (* no risk *)
    //  do base.Paint.Add(fun args -> <impliedThisVariable>.RedrawDrawArea args.Graphics)       (* no risk *)
    

    type Area5b(x) as self =
    
        inherit Panel() 
        let x = 1 + x
          (**)
        let mutable Bitmap = new Bitmap(100,100)
        let mutable GC     = Graphics.FromImage(Bitmap)   (* GC for bitmap *)
        let resizedWire    = createWire() : (int*int) wire
        let mutable Count  = 0
          (**)
        do self.BackColor <- Color.Black
        do self.Dock      <- DockStyle.Fill
        do self.Cursor    <- Cursors.Cross
    //  do self.Resize.Add(fun arg  -> area.resize ())                    (* use before defn risk *)
    //  do self.Paint .Add(fun args -> area.redrawDrawArea args.Graphics) (* use before defn risk *)
          (**)
        member this.ResizedE = resizedWire.Event
          (**)
        member this.Resize () =
          let ww,hh = self.Width,self.Height in
          if ww>0 && hh>0 then (
                Bitmap <- new Bitmap(ww,hh);
                GC     <- Graphics.FromImage(Bitmap);
                resizedWire.Send((ww,hh))
          )
          (**)
    //  TODO: need to be able to call this.PriorMethod without this.qualification
    //  do self.Resize.Add(fun arg  -> Resize ())                         (* no risk *)  
          (**)
        member this.RedrawDrawArea (gc:Graphics) = gc.DrawImage(Bitmap,new Point(0,0))
    //  member this.Refresh() = using (self.CreateGraphics()) (fun gc ->      RedrawDrawArea gc)   
        member this.Refresh2() = using (self.CreateGraphics()) (fun gc -> this.RedrawDrawArea gc)        
        (**)
    //  do self.Paint.Add(fun args -> RedrawDrawArea args.Graphics)       (* no risk *)
    //  do self.Paint.Add(fun args -> <impliedThisVariable>.RedrawDrawArea args.Graphics)       (* no risk *)
    

    type Area6(x) =
    
        inherit Panel() 
        let x = 1 + x
        do base.BackColor <- Color.Black      
    

    type Area7(x) =
    
        inherit Panel() 
        let a,b = x : int * int
        do base.BackColor <- Color.Black      
    

    open System.Drawing
    open System.Windows.Forms
    type 'a Area8(x) =
    
        inherit Panel() 
        let a,b = (x : int * int)
        do base.BackColor <- Color.Black
        let f (y:'a) = y
        let xx = a
    

    type 'a Area9(x) =
    
        let a = (x : 'a)
    //    let f (y:'a) = y
    

    type 'a Area10 =
    
      val f : 'a -> 'a
      new (x:int) = { f = fun x -> x}  
    
#endif


//! Person
  
(* Scala person example *)
module ScalaPersonExample = 
    type Person1(firstLastSpouse) =
      
        let firstName,lastName,spouse = firstLastSpouse
        member x.FirstName = firstName : string
        member x.LastName  = lastName  : string
        member x.Spouse    = spouse    : Person1 option
        member x.Introduction() =
          "Hi, my name is " ^ firstName ^ " " ^ lastName ^
          (match spouse with
           | None        -> "."
           | Some spouse -> " and this is my spouse, " ^ spouse.FirstName ^ " " ^ spouse.LastName ^ ".")
        // TODO: the implicit ctor is not in scope for defining alt constructors.
        // new (f,l) = new Person1(f,l,None)
    
    let pA = new Person1(("bob" ,"smith",None))
    let pB = new Person1(("jill","smith",Some pA))




//! Forms
  
#if !NETCOREAPP
module Forms1 = 
    open System.Drawing
    open System.Windows.Forms
    type DrawPanel(w) as c =
    
      inherit Panel() 
      do  base.Width  <- w
      do  base.Height <- w
      do  base.BackColor <- Color.Black
      let mutable BM = new Bitmap(w,w)
      let GC' = Graphics.FromImage(BM)
      let refresh() = using (c.CreateGraphics()) (fun gc -> gc.DrawImage(BM,new Point(0,0)))
      do  c.Paint.Add(fun _ -> refresh())
      (*external*)
      member this.Redraw() = refresh()
      member this.GC = GC'
    


module Forms2 = 

    open System.Drawing
    open System.Windows.Forms
    type DrawPanel2 (w,h) as c =
    
      inherit Panel() 
      do  base.Width  <- w
      do  base.Height <- h
      do  base.BackColor <- Color.DarkBlue
      let mutable BM = new Bitmap(w,w)
      let refresh() = using (c.CreateGraphics()) (fun gc -> gc.DrawImage(BM,new Point(0,0)))
      let GC' = Graphics.FromImage(BM)
      let uuu = ()
      let u2 = uuu,uuu
      (*external*)
      do  c.Paint.Add(fun _ -> refresh())
      member this.Redraw() = refresh()
      member this.GC = GC'
    

    //let form = new Form(Visible=true,AutoScroll=true)
    //let dp = new DrawPanel2(800,400)
    //do  form.Controls.Add(dp)
    //do  dp.GC.DrawLine(Pens.White,10,20,30,40)
    //do  dp.Redraw()

#endif

module Regression1 = 
    (* Regression test: local vals of unit type are not given field storage (even if mutable) *)
    type UnitTestA2 (x,y) =
    
      do  printf "TestA %d\n" (x*y)
      let u = ()
      let mutable mu = ()
      let mutable kept = false
      member this.Unlikely() = mu <- (kept <- true; (* ensure effect is not optimised away *)
                                      printf "Boo\n"
                                     )
      member this.Kept with get() = kept
    
    let x = new UnitTestA2(1,2)
    do  x.Unlikely()
    do  assert(x.Kept = true)



//! typar scoping

module TyparScopeChecks = 
    (* typar scoping checks 1 *)
    type ('a,'b) classB1(aa,bb) =
    
      let a = aa : 'a
      let b = bb : 'b
      let f (aaa:'a) (bbb:'b) = aaa,bbb
    

    (* typar scoping checks *)
    type ('a,'b) classB2(aa:'a,bb:'b) =
    
      let a = aa
      let b = bb
      let f (aaa:'a) (bbb:'b) = aaa,bbb
    




module LocalLetRecTests = 
    //! local let rec test
        
    (* let rec tests *)
    type LetRecClassA1 (a,b) =
    
      let a,b = a,b
      let x = a*3 + b*2
      let rec fact n = if n=1 then 1 else n * fact (n-1)
      member this.Fa() = fact a
      member this.Fb() = fact b
    
    let lrCA1 = new LetRecClassA1(3,4)
    let xa = lrCA1.Fa()
    let xb = lrCA1.Fb()

    (* let rec tests *)
    type LetRecClassA2 () =
    
      let rec odd  n = printf "odd  %d?\n" n; if n=0 then false else not(even (n-1))
      and even n = printf "even %d?\n" n; if n=0 then true  else not(odd (n-1))
      member this.Even(x) = even x
      member this.Odd(x)  = odd x
    
    let lrCA2 = new LetRecClassA2()
    let ex2 = lrCA2.Even(2)
    let ex3 = lrCA2.Odd(3)
    let ox2 = lrCA2.Odd(2)
    let ox3 = lrCA2.Even(3)


    //! local let rec test
        
    type nats = HD of int * (unit -> nats)
    let rec ns = HD (2,(fun () -> HD (3,(fun () -> ns))))
    let rec take n (HD (x,xf)) = if n=0 then [] else x :: take (n-1) (xf())
      
    type LetRecClassA3 () =
    
      let rec ns = HD (2,(fun () -> HD (3,(fun () -> ns))))
      let rec xs = 1::2::3::xs  (* WHY IS THIS ACCEPT HERE, BUT NOT AT THE TOP LEVEL??? *)
      member this.NS = ns
      member this.XS = xs
    
    let lrCA3 = new LetRecClassA3()
    let ns123 = lrCA3.NS
    let xs123 = lrCA3.XS

    type LetRecClassA4 () =
    
      let rec xs = 1::2::3::xs  (* WHY IS THIS ACCEPT HERE, BUT NOT AT THE TOP LEVEL??? *)
      member this.XS = xs
    
    let lrCA4    = new LetRecClassA4()
    let error123 = lrCA4.XS



//! override test
  
module OverrideTest1 = 
    [<AbstractClass>]
    type AbstractClass =
    
        new() = {}
        abstract MA : int -> int
        abstract MB : int -> int
    

    type OverrideClassA1 () =
      
        inherit AbstractClass()
        override this.MA(x) = x+1
        override this.MB(x) = x+2
    


module OverrideTest2 = 

    //! abstract test
      
    [<AbstractClass>]
    type ImplicitAbstractClass() =
    
        abstract MA : int -> int
        abstract MB : int -> int
    

    type OverrideClassA2 () =
      
        inherit ImplicitAbstractClass()
        override this.MA(x) = x+1
        override this.MB(x) = x+2
    
    let oca2 = new OverrideClassA2()
    do  assert(oca2.MA(1)=2)
    do  assert(oca2.MB(1)=3)



module ConstructionTests = 
    //! CC tests
    type CC (x) =
      
        let mutable z = x+1
        let f = fun () -> z
        member a.F() = f()
        member a.Set x = z <- x
      

    let cc = new CC(1)
    cc.F()
    cc.Set(20)
    cc.F()


    //! interface
    type CCCCCCCC() = 
      interface System.IDisposable with      
          member x.Dispose() = ()
      
    



module StaticMemberScopeTest = 
    //! static expr test
    type StaticTestA1(argA,argB) = 
      let         locval = 1 + argA
      let mutable locmut = 2 + argB
      static member M = 12 (* cannot use: locval + locmut + argA  *)
    


module ConstructorArgTests = 

    //! ctor args stored in fields
    type CtorArgClassA1(argA,argB) = 
      member x.M = argA + argB*2
     


module SelfReferenceTests = 
    type SelfReferences(a,b) as self = 
      let f()   = self.M  
      let ab    = a*b+2
      let x =
        let ok =
          try
            let x = self.M in
            printf "Error: self reference got through - should catch these\n";
            false (* not ok here *)
          with _ ->
            printf "Self reference during construction threw exception, as required\n";
            true (* ok, got exn *)
        in
        assert(ok);
        ok
      member self.M = (ab : int)
      member self.P = f()
     
    let sr = new SelfReferences(1,2)


//! Bug: 878 - type unsoundness - implicit constructor members generalising "free-imperative" typars from let-bindings
(* must be -ve test
type C() = 
   let t = ref (Map.empty<string,_>)
   let f x = (!t).Add("3",x)
   member x.M() = !t

*)  


module MixedRecursiveTypeDefinitions = 

    type ClassType<'a>(x:'a) =         
        member self.X = x
        
    and RecordType =
        { field1 : int;
          field2 : ClassType<int> }
        member self.GetField() = self.field2
        
    and UnionType =
        | Case1 of string * AbbrevType1
        | Case2 of string * AbbrevType2

    and AbbrevType1 = ClassType<int>

    and AbbrevType2 = ClassType<string>

    and AnotherClassType<'a>(x:'a) = 
        member self.X = x
        interface InterfaceType<'a> with 
            member self.X = x
        
    and InterfaceType<'a> = 
        abstract X : 'a
        
    

module ExceptionsWithAugmentations = 
    exception E of string * string
            with 
               override x.Message = "A"
            end


    let x = E("3","3")

    1+1
    
    test "ckwh98u" ((try raise x with e -> e.Message) = "A")

    exception E2Exception of string * string
            with 
               override x.Message = "A"
               member x.Member2 = "E"
            end


    let x2 = E2Exception("3","3")

    1+1
    
    test "ckwh98v" ((try raise x2 with :? E2Exception as e -> e.Member2) = "E")


//! Test cases:
(*  
  [ ] - self-references - allowed and trapped
  [ ] - direct calls to most derived base override
  [ ] - calls on "super" object - via self and super vars
*)

//! Finish


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

