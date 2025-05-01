// #Conformance #MemberDefinitions #Mutable #ObjectOrientedTypes #Classes #InterfacesAndImplementations #Recursion 
#if TESTS_AS_APP
module Core_members_incremental
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


//! Address of incremental class local mutable
  
module AddressOfIncrementalClassLocalMutable = begin

    open System.Drawing
    open System.Windows.Forms
    let rr = new Rectangle()               // <-------- value type
    type LocalMutableTestA() = class
      let mutable mvarA  = rr
      member x.MethodA() = mvarA.X         // <-------- require address of it for arg to getter
    end

end

//! Address of mutable record field (related to above)

module AddressOfMutableRecordField = begin
    open System.Drawing
    open System.Windows.Forms

    type RectR = { mutable field : Rectangle }
    let f (x:RectR) = x.field.X <- 10
    let boxR = {field = new Rectangle() }
    do  assert(boxR.field.X=0)  
    do  f boxR
    do  assert(boxR.field.X=10)

end

//! Minor test

module MinorTest = begin
    type A<'a>(x) = 
        class
            let (y:'a) = x
            member this.X = y
        end

    let x1 = new A<string>("abc")    
    let x2 = new A<int>(3)
    let x3 = new A<int64>(3L)
end


//! Misc

module Misc = begin
    type 'a Area9aa(x) =
        class
            let a = (x : 'a)
            let f (y:'a) = y
        end

    type AList(a) = class
      let x1 = a + 1
      let y2 = x1 + 1
      let y3 = 3
      member pairs_this.Pair() = x1,y2
    end
    let a = new AList 12
    do  assert( a.Pair() = (13,14) )
end


//! Wire previously
    
(* accepted *)
module WireOld = begin
    [<AbstractClass>]
    type 'a wire =
        class
          abstract Send   : 'a -> unit
          abstract Listen : ('a -> unit) -> unit
          new () = {}
          member self.Event = self :> 'a IEvent
          interface IEvent<'a> with
              member x.Subscribe(handler) = failwith "nyi"
              member x.AddHandler(handler) = failwith "nyi"
              member x.RemoveHandler(handler) = failwith "nyi"
          end
        end
    let createWire() =
      let listeners = ref [] in
      {new wire<'a>() with 
          member __.Send(x)   = List.iter (fun f -> f x) !listeners
          member __.Listen(f) = listeners := f :: !listeners
      }

end

//! Wire variations

module WireVariations = begin
    (* Accepted *)    
    type wire2(z) =
      class
         let z = z
         member this.Send(x) = 1 + z + x
      end

    (* Accepted *)
    type 'a wire3(z) =
      class
         let listeners = (z:'a)
         member this.Send(x:'a) = x
      end

    (* Accepted *)    
    type wire4<'a>(z) =
      class
         let listeners = let z:'a = z in ref ([] : ('a -> unit) list)
         member this.Send(x:'a)           = List.iter (fun f -> f x) !listeners
         member this.Listen(f:'a -> unit) = listeners := f :: !listeners
      end

    (* Accepted *)    
    type 'a wire5(z) =
      class
         let listeners = ref ([] : ('a -> unit) list)
         member this.Send(x:'a)           = let z:'a = z in List.iter (fun f -> f x) !listeners
         member this.Listen(f:'a -> unit) = listeners := f :: !listeners
      end

    (* Accepted now - fixed tinst missing error *)
    type 'a wire6(z) =
      class
         let mutable listeners = ([] : ('a -> unit) list)
         member this.Send(x:'a)           = let z:'a = z in List.iter (fun f -> f x) listeners
         member this.Listen(f:'a -> unit) = listeners <- f :: listeners
    end

    (* OK, now this types are asserted equal *)
    type 'a wire7(z) =
      class
         let listeners  = 12
         let z : 'a = z
         member this.SendA(x:int) = this,listeners
         member this.SendB(x:int) = this,listeners       
      end

    (* Soundness concern: should uses of let-fields force a member "this" to be ('a wire) typed? *)
    type 'a wire8(z) =
      class
         let z : 'a = z  
         let listeners  = 12
         member this.SendA(x:int) = x,z,(this: 'a wire8)
         member this.SendB(x:int) = x,z,(this: 'a wire8)
      end

    (* Accepted *)    
    type 'a wire9(z) =
      class
         let mutable listeners  = ([] : int list)
         let z : 'a = z
         member this.Send(x:int) =
           let ls : int list = 1 :: 2 :: listeners in
           List.map (fun x -> x+1) ls
    end

    (* Accepted *)    
    type 'a wire10(z) =
      class
         let mutable listeners  = ([] : int list)
         let z : 'a = z
         member this.Send(x:int) =
           let ls : int list = listeners @ [1] in  (* it seems listeners may have the wrong type... *)
           List.map (fun f -> f) ls
    end

    (* Accepted *)    
    type 'a wire11(z) =
      class
         let listeners  = let z:int = z in ([] : int list)
         member this.Send(x:'b) = (* List.iter (fun f -> ()) listeners *)
             let xx : _ wire11 = this in      
             Some (x,List.head listeners)
    end

    (* Accepted *)    
    type 'a wire12(z) =
      class
         let listeners  = let z:'a = z in ([] : int list)
         member this.Send(x:'b) = (* List.iter (fun f -> ()) listeners *)
             Some (this,x,List.head listeners)
    end

    (* OK, after avoid the value restriction *)    
    type wire13(z) =
      class
         member this.Send(x:'b) = (ignore (z:int)); Some (this,x)
    end

    (* Accepted *)
    type 'a wire14(z) =
      class
         let mutable listeners  = let z:int = z in ([] : int list)
         member this.Send(x:'a) = (* List.iter (fun f -> ()) listeners *)
             listeners
    end
end


//! Area variations
  
#if !MONO && !NETCOREAPP
module AreaVariations = begin
    (* Accepted *)
    open System.Drawing
    open System.Windows.Forms
    open WireOld
    
    type Area1(x) =
        class
            inherit Panel()
            let x = x * 2
        end

    type Area2(x) =
        class
            inherit Panel()
            let x = x * 2      
            let mutable Bitmap = new Bitmap(100,100)
            let mutable GC     = Graphics.FromImage(Bitmap)
            let fff (x:Area2) = x.Resize ()      
            member this.Resize  () = Bitmap <- new Bitmap(10,20)
        end

    type Area3(x) =
        class
            let basePanel = let x = x:unit in new Panel()
            let mutable Bitmap = new Bitmap(100,100)
            let mutable GC     = Graphics.FromImage(Bitmap)
            let fff (x:Area3) = x.Resize ()      
            member this.Resize  () = Bitmap <- new Bitmap(10,20)
        end

    type Area4 =
      class
        inherit Panel 
        member this.X() = base.Dock
    end

    type Area5(x) =
        class
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
        end

    type Area5b(x) as self =
        class
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
        end

    type Area6(x) =
        class
            inherit Panel() 
            let x = 1 + x
            do base.BackColor <- Color.Black      
        end

    type Area7(x) =
        class
            inherit Panel() 
            let a,b = x : int * int
            do base.BackColor <- Color.Black      
        end

    open System.Drawing
    open System.Windows.Forms
    type 'a Area8(x) =
        class
            inherit Panel() 
            let a,b = (x : int * int)
            do base.BackColor <- Color.Black
            let f (y:'a) = y
            let xx = a
        end

    type 'a Area9(x) =
        class
            let a = (x : 'a)
        //    let f (y:'a) = y
        end

    type 'a Area10 =
        class
          val f : 'a -> 'a
          new (x:int) = { f = fun x -> x}  
        end
end

#endif
//! Person
  
(* Scala person example *)
module ScalaPersonExample = begin
    type Person1(firstLastSpouse) =
      class
        let firstName,lastName,spouse = firstLastSpouse
        member x.FirstName = firstName : string
        member x.LastName  = lastName  : string
        member x.Spouse    = spouse    : Person1 option
        member x.Introduction() =
          "Hi, my name is " + firstName + " " + lastName +
          (match spouse with
           | None        -> "."
           | Some spouse -> " and this is my spouse, " + spouse.FirstName + " " + spouse.LastName + ".")
        // TODO: the implicit ctor is not in scope for defining alt constructors.
        // new (f,l) = new Person1(f,l,None)
    end
    let pA = new Person1(("bob" ,"smith",None))
    let pB = new Person1(("jill","smith",Some pA))
end



//! Forms
  
#if !MONO && !NETCOREAPP
module Forms1 = begin
    open System.Drawing
    open System.Windows.Forms
    type DrawPanel(w) as x =
        class
          inherit Panel() 
          do  base.Width  <- w
          do  base.Height <- w
          do  base.BackColor <- Color.Black
          let mutable BM = new Bitmap(w,w)
          let refresh() = using (x.CreateGraphics()) (fun gc -> gc.DrawImage(BM,new Point(0,0)))
          let GC' = Graphics.FromImage(BM)
          do  x.Paint.Add(fun _ -> refresh())
          (*external*)
          member this.Redraw() = refresh()
          member this.GC = GC'
        end
end

module Forms2 = begin

    open System.Drawing
    open System.Windows.Forms
    type DrawPanel2 (w,h) as x =
        class
          inherit Panel() 
          do  base.Width  <- w
          do  base.Height <- h
          do  base.BackColor <- Color.DarkBlue
          let mutable BM = new Bitmap(w,w)
          let refresh() = using (x.CreateGraphics()) (fun gc -> gc.DrawImage(BM,new Point(0,0)))
          let GC' = Graphics.FromImage(BM)
          let uuu = ()
          let u2 = uuu,uuu
          do  x.Paint.Add(fun _ -> refresh())
          (*external*)
          member this.Redraw() = refresh()
          member this.GC = GC'
        end

    let form = new Form(Visible=true,AutoScroll=true)
    let dp = new DrawPanel2(800,400)
    do  form.Controls.Add(dp)
    do  dp.GC.DrawLine(Pens.White,10,20,30,40)
    do  dp.Redraw()
end
#endif

module Regression1 = begin
    (* Regression test: local vals of unit type are not given field storage (even if mutable) *)
    type UnitTestA2 (x,y) =
        class
          do  printf "TestA %d\n" (x*y)
          let u = ()
          let mutable mu = ()
          let mutable kept = false
          member this.Unlikely() = mu <- (kept <- true; (* ensure effect is not optimised away *)
                                          printf "Boo\n"
                                         )
          member this.Kept with get() = kept
        end
    let x = new UnitTestA2(1,2)
    do  x.Unlikely()
    do  assert(x.Kept = true)

end

//! typar scoping

module TyparScopeChecks = begin
    (* typar scoping checks 1 *)
    type ('a,'b) classB1(aa,bb) =
        class
          let a = aa : 'a
          let b = bb : 'b
          let f (aaa:'a) (bbb:'b) = aaa,bbb
        end

    (* typar scoping checks *)
    type ('a,'b) classB2(aa:'a,bb:'b) =
        class
          let a = aa
          let b = bb
          let f (aaa:'a) (bbb:'b) = aaa,bbb
        end

end


module LocalLetRecTests = begin
    //! local let rec test
        
    (* let rec tests *)
    type LetRecClassA1 (a,b) =
        class
          let a,b = a,b
          let x = a*3 + b*2
          let rec fact n = if n=1 then 1 else n * fact (n-1)
          member this.Fa() = fact a
          member this.Fb() = fact b
        end
    let lrCA1 = new LetRecClassA1(3,4)
    let xa = lrCA1.Fa()
    let xb = lrCA1.Fb()

    (* let rec tests *)
    type LetRecClassA2 () =
        class
          let rec odd  n = printf "odd  %d?\n" n; if n=0 then false else not(even (n-1))
          and even n = printf "even %d?\n" n; if n=0 then true  else not(odd (n-1))
          member this.Even(x) = even x
          member this.Odd(x)  = odd x
        end
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
        class
          let rec ns = HD (2,(fun () -> HD (3,(fun () -> ns))))
          let rec xs = 1::2::3::xs  (* WHY IS THIS ACCEPT HERE, BUT NOT AT THE TOP LEVEL??? *)
          member this.NS = ns
          member this.XS = xs
        end
    let lrCA3 = new LetRecClassA3()
    let ns123 = lrCA3.NS
    let xs123 = lrCA3.XS

    type LetRecClassA4 () =
        class
          let rec xs = 1::2::3::xs  (* WHY IS THIS ACCEPT HERE, BUT NOT AT THE TOP LEVEL??? *)
          member this.XS = xs
        end
    let lrCA4    = new LetRecClassA4()
    let error123 = lrCA4.XS
end


//! override test
  
module OverrideTest1 = begin
    [<AbstractClass>]
    type AbstractClass =
        class
            new() = {}
            abstract MA : int -> int
            abstract MB : int -> int
        end

    type OverrideClassA1 () =
      class
        inherit AbstractClass()
        override this.MA(x) = x+1
        override this.MB(x) = x+2
    end
end

module OverrideTest2 = begin

    //! abstract test
      
    [<AbstractClass>]
    type ImplicitAbstractClass() =
        class
            abstract MA : int -> int
            abstract MB : int -> int
        end

    type OverrideClassA2 () =
      class
        inherit ImplicitAbstractClass()
        override this.MA(x) = x+1
        override this.MB(x) = x+2
    end
    let oca2 = new OverrideClassA2()
    do  assert(oca2.MA(1)=2)
    do  assert(oca2.MB(1)=3)

end

module ConstructionTests = begin
    //! CC tests
    type CC (x) =
      class
        let mutable z = x+1
        let f = fun () -> z
        member a.F() = f()
        member a.Set x = z <- x
      end

    let cc = new CC(1)
    cc.F()
    cc.Set(20)
    cc.F()


    //! interface
    type CCCCCCCC() = class
      interface System.IDisposable with      
          member x.Dispose() = ()
      end
    end

end

module StaticMemberScopeTest = begin
    //! static expr test
    type StaticTestA1(argA,argB) = class
      let         locval = 1 + argA
      let mutable locmut = 2 + argB
      static member M = 12 (* cannot use: locval + locmut + argA  *)
    end
end

module ConstructorArgTests = begin

    //! ctor args stored in fields
    type CtorArgClassA1(argA,argB) = class
      member x.M = argA + argB*2
     end
end

module SelfReferenceTests = begin
    type SelfReferences(a,b) as self = class
      let f()   = self.M  
      let ab    = a*b+2
      let x =
        let ok =
          try
            let x = self.M in
            printf "Error: self reference got through - should catch these\n";
            false (* not ok here *)
          with :? System.InvalidOperationException ->
            printf "Self reference during construction threw exception, as required\n";
            true (* ok, got exn *)
        in
        assert(ok);
        ok
      member self.M = (ab : int)
      member self.P = f()
     end
    let sr = new SelfReferences(1,2)
end

//! Bug: 878 - type unsoundness - implicit constructor members generalising "free-imperative" typars from let-bindings
(* must be -ve test
type C() = class
   let t = ref (Map.empty<string,_>)
   let f x = (!t).Add("3",x)
   member x.M() = !t
end
*)  


module MixedRecursiveTypeDefinitions = begin

    type ClassType<'a>(x:'a) = 
        class
            member self.X = x
        end
    and RecordType =
        { field1 : int;
          field2 : ClassType<int> }
        with 
            member self.GetField() = self.field2
        end
    and UnionType =
        | Case1 of string * AbbrevType1
        | Case2 of string * AbbrevType2
    and AbbrevType1 = ClassType<int>
    and AbbrevType2 = ClassType<string>
    and AnotherClassType<'a>(x:'a) = 
        class
//            member self.X = x
            interface InterfaceType<'a> with 
                member self.X = x
            end
               
        end
    and InterfaceType<'a> = 
        interface
            abstract X : 'a
        end
    

end


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

