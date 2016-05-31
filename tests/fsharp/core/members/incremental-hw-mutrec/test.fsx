// #Conformance #MemberDefinitions #Mutable #ObjectOrientedTypes #Classes #InterfacesAndImplementations #Recursion 

//---------------------------------------------------------------
// Same test as "members\incremental-hw" but with "rec" added 



module rec IncrementalHwMutrec   // <-----  NOTE THE "rec"

// Setup

let failures = ref false
let report_failure () = stderr.WriteLine " NO"; failures := true
let test s b = stderr.Write(s:string);  if b then stderr.WriteLine " OK" else report_failure() 


// Address of incremental  local mutable
  
module AddresOfIncrementalClassLocalMutable = 

    open System.Drawing
    open System.Windows.Forms
    let rr = new Rectangle()               // <-------- value type
    type LocalMutableTestA() = 
      let mutable mvarA  = rr
      member x.MethodA() = mvarA.X         // <-------- require address of it for arg to getter
    



// Address of mutable record field (related to above)

module AddresOfMutableRecordField = 
    open System.Drawing
    open System.Windows.Forms

    type RectR = { mutable field : Rectangle }
    let f (x:RectR) = x.field.X <- 10
    let boxR = {field = new Rectangle() }
    do  assert(boxR.field.X=0)  
    do  f boxR
    do  assert(boxR.field.X=10)



// Minor test

module MinorTest = 
    type A<'T>(x:'T) = 
        
            let (y:'T) = x
            member this.X = y
        

    let x1 = new A<string>("abc")    
    let x2 = new A<int>(3)
    let x3 = new A<int64>(3L)



// Misc

module Misc = 
    type 'T Area9aa(x) =
    
        let a = (x : 'T)
        let f (y:'T) = y
    

    type AList(a) = 
      let x1 = a + 1
      let y2 = x1 + 1
      let y3 = 3
      member pairs_this.Pair() = x1,y2
    
    let a = new AList 12
    do  assert( a.Pair() = (13,14) )



// Wire prevously
    
(* accepted *)
module WireOld = 
    [<AbstractClass>]
    type wire<'T> =
    
      abstract Send   : 'T -> unit
      abstract Listen : ('T -> unit) -> unit
      new () = {}
      member self.Event = self :> IEvent<'T>
      interface IEvent<'T> with
          member x.Subscribe(handler) = failwith "nyi"
          member x.AddHandler(handler) = failwith "nyi"
          member x.RemoveHandler(handler) = failwith "nyi"
      
    
    let createWire() =
      let listeners = ref [] in
      {new wire<'T>() with
         member __.Send(x)   = List.iter (fun f -> f x) !listeners
         member __.Listen(f) = listeners := f :: !listeners
      }



// Wire variations

module WireVariations = 
    // Accepted
    type wire2(z) =
      
         let z = z
         member this.Send(x) = 1 + z + x
      

    // Accepted 
    type wire3<'T>(z) =
      
         let listeners = (z:'T)
         member this.Send(x:'T) = x
      

    // Accepted     
    type wire4<'T>(z) =
      
         let listeners = let z:'T = z in ref ([] : ('T -> unit) list)
         member this.Send(x:'T)           = List.iter (fun f -> f x) !listeners
         member this.Listen(f:'T -> unit) = listeners := f :: !listeners
      

    // Accepted
    type wire5<'T>(z) =
      
         let listeners = ref ([] : ('T -> unit) list)
         member this.Send(x:'T)           = let z:'T = z in List.iter (fun f -> f x) !listeners
         member this.Listen(f:'T -> unit) = listeners := f :: !listeners
      

    // Accepted now - fixed tinst missing error 
    type wire6<'T>(z) =
      
         let mutable listeners = ([] : ('T -> unit) list)
         member this.Send(x:'T)           = let z:'T = z in List.iter (fun f -> f x) listeners
         member this.Listen(f:'T -> unit) = listeners <- f :: listeners
    

    // OK, now this types are asserted equal 
    type wire7<'T>(z) =
      
         let listeners  = 12
         let z : 'T = z
         member this.SendA(x:int) = this,listeners
         member this.SendB(x:int) = this,listeners       
      

    // Soundness concern: should uses of let-fields force a member "this" to be ('T wire) typed? 
    type wire8<'T>(z) =
      
         let z : 'T = z  
         let listeners  = 12
         member this.SendA(x:int) = x,z,(this: 'T wire8)
         member this.SendB(x:int) = x,z,(this: 'T wire8)
      

    // Accepted
    type wire9<'T>(z) =
      
         let mutable listeners  = ([] : int list)
         let z : 'T = z
         member this.Send(x:int) =
           let ls : int list = 1 :: 2 :: listeners in
           List.map (fun x -> x+1) ls
    

    // Accepted
    type wire10<'T>(z) =
      
         let mutable listeners  = ([] : int list)
         let z : 'T = z
         member this.Send(x:int) =
           let ls : int list = listeners @ [1] in  
           List.map (fun f -> f) ls
    

    // Accepted
    type wire11<'T>(z) =
      
         let listeners  = let z:int = z in ([] : int list)
         member this.Send(x:'T) = 
             let xx : 'T wire11 = this in      
             Some (x,List.head listeners)
    

    // Accepted 
    type wire12<'T>(z) =
      
         let listeners  = let z:'T = z in ([] : int list)
         member this.Send(x:'b) = 
             Some (this,x,List.head listeners)
    

    // OK, after avoid the value restriction
    type wire13(z) =
      
         member this.Send(x:'b) = (ignore (z:int)); Some (this,x)
    

    // Accepted 
    type wire14<'T>(z) =
      
         let mutable listeners  = let z:int = z in ([] : int list)
         member this.Send(x:'T) = (* List.iter (fun f -> ()) listeners *)
             listeners
    



// Area variations
  
module AreaVariations = 
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
        let mutable Bitmap = new Bitmap(100,100)
        let mutable GC     = Graphics.FromImage(Bitmap)   (* GC for bitmap *)
        let resizedWire    = createWire() : (int*int) wire
        let mutable Count  = 0
        do base.BackColor <- Color.Black
        do base.Dock      <- DockStyle.Fill
        do base.Cursor    <- Cursors.Cross
        member this.ResizedE = resizedWire.Event
        member this.Resize () =
          let ww,hh = base.Width,base.Height in
          if ww>0 && hh>0 then (
                Bitmap <- new Bitmap(ww,hh);
                GC     <- Graphics.FromImage(Bitmap);
                resizedWire.Send((ww,hh))
          )
        member this.RedrawDrawArea (gc:Graphics) = gc.DrawImage(Bitmap,new Point(0,0))
        member this.Refresh2() = using (base.CreateGraphics()) (fun gc -> this.RedrawDrawArea gc)        

    type Area5b(x) as self =
    
        inherit Panel() 
        let x = 1 + x
        let mutable Bitmap = new Bitmap(100,100)
        let mutable GC     = Graphics.FromImage(Bitmap)   (* GC for bitmap *)
        let resizedWire    = createWire() : (int*int) wire
        let mutable Count  = 0
        do self.BackColor <- Color.Black
        do self.Dock      <- DockStyle.Fill
        do self.Cursor    <- Cursors.Cross
        member this.ResizedE = resizedWire.Event
        member this.Resize () =
          let ww,hh = self.Width,self.Height in
          if ww>0 && hh>0 then (
                Bitmap <- new Bitmap(ww,hh);
                GC     <- Graphics.FromImage(Bitmap);
                resizedWire.Send((ww,hh))
          )
        member this.RedrawDrawArea (gc:Graphics) = gc.DrawImage(Bitmap,new Point(0,0))
        member this.Refresh2() = using (self.CreateGraphics()) (fun gc -> this.RedrawDrawArea gc)        
    

    type Area6(x) =
    
        inherit Panel() 
        let x = 1 + x
        do base.BackColor <- Color.Black      
    

    type Area7(x) =
    
        inherit Panel() 
        let a,b = x : int * int
        do base.BackColor <- Color.Black      
    



    type 'T Area8(x) =
    
        inherit Panel() 
        let a,b = (x : int * int)
        do base.BackColor <- Color.Black
        let f (y:'T) = y
        let xx = a
    

    type 'T Area9(x) =
    
        let a = (x : 'T)
    //    let f (y:'T) = y
    

    type 'T Area10 =
    
      val f : 'T -> 'T
      new (x:int) = { f = fun x -> x}  
    



// Person
  
// Scala person example 
module ScalaPersonExample = 
    type Person1(firstLastSpouse) =
      
        let firstName,lastName,spouse = firstLastSpouse
        member x.FirstName = firstName : string
        member x.LastName  = lastName  : string
        member x.Spouse    = spouse    : Person1 option
        member x.Introduction() =
          "Hi, my name is " + firstName + " " + lastName +
          (match spouse with
           | None        -> "."
           | Some spouse -> " and this is my spouse, " + spouse.FirstName + " " + spouse.LastName + ".")
    
    let pA = new Person1(("bob" ,"smith",None))
    let pB = new Person1(("jill","smith",Some pA))




// Forms
  
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
    


module Regression1 = 
    // Regression test: local vals of unit type are not given field storage (even if mutable) 
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



// typar scoping

module TyparScopeChecks = 
    (* typar scoping checks 1 *)
    type ('T,'b) classB1(aa,bb) =
    
      let a = aa : 'T
      let b = bb : 'b
      let f (aaa:'T) (bbb:'b) = aaa,bbb
    

    (* typar scoping checks *)
    type ('T,'b) classB2(aa:'T,bb:'b) =
    
      let a = aa
      let b = bb
      let f (aaa:'T) (bbb:'b) = aaa,bbb
    




module LocalLetRecTests = 
    // local let rec test
        
    type LetRecClassA1 (a,b) =
    
      let a,b = a,b
      let x = a*3 + b*2
      let rec fact n = if n=1 then 1 else n * fact (n-1)
      member this.Fa() = fact a
      member this.Fb() = fact b
    
    let lrCA1 = new LetRecClassA1(3,4)
    let xa = lrCA1.Fa()
    let xb = lrCA1.Fb()

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


    // local let rec test
        
    type nats = HD of int * (unit -> nats)
    let rec ns = HD (2,(fun () -> HD (3,(fun () -> ns))))
    let rec take n (HD (x,xf)) = if n=0 then [] else x :: take (n-1) (xf())
      
    type LetRecClassA3 () =
    
      let rec ns = HD (2,(fun () -> HD (3,(fun () -> ns))))
      let rec xs = 1::2::3::xs  
      member this.NS = ns
      member this.XS = xs
    
    let lrCA3 = new LetRecClassA3()
    let ns123 = lrCA3.NS
    let xs123 = lrCA3.XS

    type LetRecClassA4 () =
    
      let rec xs = 1::2::3::xs  
      member this.XS = xs
    
    let lrCA4    = new LetRecClassA4()
    let error123 = lrCA4.XS



// override test
  
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
    // CC tests
    type CC (x) =
      
        let mutable z = x+1
        let f = fun () -> z
        member a.F() = f()
        member a.Set x = z <- x
      

    let cc = new CC(1)
    cc.F()
    cc.Set(20)
    cc.F()

    type CCCCCCCC() = 
      interface System.IDisposable with      
          member x.Dispose() = ()
      
    



module StaticMemberScopeTest = 
    type StaticTestA1(argA,argB) = 
      let         locval = 1 + argA
      let mutable locmut = 2 + argB
      static member M = 12 (* can not use: locval + locmut + argA  *)
    


module ConstructorArgTests = 

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


module MixedRecursiveTypeDefinitions = 

    type ClassType<'T>(x:'T) =         
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

    and AnotherClassType<'T>(x:'T) = 
        member self.X = x
        interface InterfaceType<'T> with 
            member self.X = x
        
    and InterfaceType<'T> = 
        abstract X : 'T
        
    

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



let _ = 
  if !failures then (stdout.WriteLine "Test Failed"; exit 1) 
  else (stdout.WriteLine "Test Passed"; 
        System.IO.File.WriteAllText("test.ok","ok"); 
        exit 0)

