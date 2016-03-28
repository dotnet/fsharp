// #Conformance #MemberDefinitions #Overloading #ComputationExpressions 

let failures = ref false
let report_failure () = 
  stderr.WriteLine " NO"; failures := true
let test s b = stderr.Write(s:string);  if b then stderr.WriteLine " OK" else report_failure() 

//let inline (>>) (x:$a) (y:$b) = (($a.(>>) <> <'a,'b,'c> : $t1<'a,'b> * $t2<'b,'c> -> $t2<'a,'c>) (x,y))
//let inline (+) (x:$t1) (y:$t2) = (($a.(+) <...> <> : $t1<...> * $t2 -> $t1<...>) (x,y))
//let inline (>>) (x:^a) (y:^b) : ^b = (^a.(>>) (x,y) )
//let inline (<<) (x:^a) (y:^b) : ^a = (^a.(<<) (x,y) )

module FuncTest = begin

    type func =
      class
        val numCalls: int ref
        val impl: int -> int option
        member x.Invoke(y) = incr x.numCalls; x.impl(y)
        new(f) = {  numCalls = ref 0; impl=f }
        static member (>>>>) ((f: func), (g: func))  = 
          new func(fun x -> match f.Invoke(x) with None -> None | Some b -> g.Invoke(b))
        static member (<<<<) ((f: func), (g: func))  = 
          new func(fun x -> match g.Invoke(x) with None -> None | Some b -> f.Invoke(b))
      end

    let konst a = new func(fun x -> Some a)

    let morph f = new func(fun x -> Some (f x))

    #if UNNAMED_OPS
    let something = (morph (fun x -> x * x)) >>>> (morph (fun x -> x + x))
    let something2 = (morph (fun x -> x * x)) <<<< (morph (fun x -> x + x))

    do test "cn39233" (something.Invoke(3) = Some 18)
    do test "cn39233b" (something2.Invoke(3) = Some 36)
    #endif

end


module OverloadSamples = begin

    open System

    //-----------------------------------------------------------------------
    // Examples 1-5: Simple overloading 

    let f1 x = x + 1.0  // add on floats
    let f2 x = x + 1.0f  // add on 32-bit floats
    let f3 x = x + 2     // add on integers
    let f4 (x:DateTime) = x + new TimeSpan(300L)     // add on dates
    let f5 (x:TimeSpan) = x + new TimeSpan(1000L)     // add on time spans

    //-----------------------------------------------------------------------
    // Example 6: overloading on types you've defined yourself.  Normally
    // we would use the standard F# pattern of defining a type 
    // (i.e. it's data representation), then defining
    // a module that carries the operations associated with the type, then 
    // using an augmentation to assocaite the operations with the type.  However
    // here we've just added the augmentation straight away.

    type IntVector = V of int array
      with 
        static member (+) (V x, V y) = 
          if x.Length <> y.Length then invalidArg "arg" "IntVectorOps.add";
          V(Array.init x.Length (fun i -> x.[i] + y.[i]))
      end

    // Now use the overloading:

    let res6 = V [| 1;2;3 |] + V [| 3;2;1 |]

    // val res6 = V [|4; 4; 4|]


    //==============================================================================
    // Example 7: Generic Vectors (incorrect approach)
    //
    // F# overloading does not propagate as far as you may wish.  In particular,
    // overloading on generic types will often produce unsatisfying results  (extending
    // overloading in this direction is being considered).
    //
    // For example, you can't create a generic vector type and have the overloading
    // on the element type just magically propagate to the new type.

    type 'a BadGenericVector = 
      { arr: 'a array }
      with
         // This function is not as general as you might wish, despite the use of 
         // the overloaded "+" operator.  Each instance of an overloaded operator
         // must relate to one and only one type across the entire scope of
         // type inference.  So this gives rise to an "add" function that will be
         // used to add one as-yet-to-be-determined type of element.
         //
         // So this function gives rise to the error
         //   test.ml(_,_): error: FS0001: The declared type parameter 'a cannot be used in
         //   conjunction with this overloaded operator since the overloading cannot be 
         //   resolved at compile time

         // static member (+) ((x : 'a BadGenericVector),(y :'a BadGenericVector)) = 
         //    if x.arr.Length <> y.arr.Length then invalidArg "Matrix.(+)";
         //    {arr=Array.init x.arr.Length (fun i -> x.arr.[i] + y.arr.[i])}
      end

    //let BGV arr = {arr=arr}
    // This means you cannot use these
    //let f7a (x:BadGenericVector<int>) = x + BGV [| 1;2;3 |]
    //let f7b (x:BadGenericVector<float>) = x + BGV [| 1.0 |]

    //==============================================================================
    // Example 7: Generic Vectors (correct approach)
    //
    // The solution is to have your generic types carry a dictionary of operations.
    // Overloads on the generic type can then be correctly defined.  This is a lot
    // like writing the translation of Haskell type classes by hand, with the advantage
    // that you get complete control over the instantiations and where the 
    // dictionaries are created and passed, but with the disadvantage that
    // it is a little more verbose.

    // Here is the dictionary of operations:

    type 'a NumberOps = 
      { zero: 'a;
        one: 'a;
        add : 'a -> 'a -> 'a;
        mul : 'a -> 'a -> 'a;
        neg : 'a -> 'a; }

    // Here are two instantiations of this dictionary of operations:

    let intOps = 
      { zero = 0;
        one = 1;
        add = (+);
        mul = ( * );
        neg = (~-); }
        
    let floatOps = 
      { zero = 0.0;
        one = 1.0;
        add = (+);
        mul = ( * );
        neg = (~-); }
        
    // Now the GenericVector type itself, its operations and the augmentation
    // giving the operator overload:

    type 'a GenericVector = 
      { ops: 'a NumberOps;
        arr: 'a array }

    let add x y = 
      if x.arr.Length <> y.arr.Length then invalidArg "arg" "Matrix.(+)";
      {ops=x.ops; arr=Array.init x.arr.Length (fun i -> x.ops.add x.arr.[i] y.arr.[i]) }

    let create ops arr = 
      {ops=ops; arr=arr }

    type 'a GenericVector 
      with
        // Nb. For an operator assocaited with a generic type 
        // the the type parameters involved in the operator's definition must be the same 
        // as the type parameters of the enclosing class.
        static member (+) ((x : 'a GenericVector),(y : 'a GenericVector)) = add x y
      end

    let IGV arr = create intOps arr 
    let FGV arr = create floatOps arr 

    // Now the GenericVector type itself, its operations and the augmentation
    // giving the operator overload:

    let f8 (x:GenericVector<int>) = x + IGV [| 1;2;3 |]
    let f9 (x:GenericVector<float>) = x + FGV [| 1.0 |]
    let twice (x:GenericVector<'a>) (y:GenericVector<'a>) = x + y

    let f10 (x:GenericVector<float>) = twice x
    let f11 (x:GenericVector<int>) = twice x


end


module StateMonadTest = begin

 type 'a IO = 
    { impl: unit -> 'a }
    with 
      member f.Invoke() = f.impl()
      static member Result(r) = { impl = (fun () -> r) }
      member f.Bind(g) : 'b IO = g(f.impl())
      static member BindOp((f : 'a IO), (g : 'a -> 'b IO)) = f.Bind(g)
    end

 let (>>=) (f: 'a IO) g  = f.Bind(g)
 let result x  = {impl = (fun () -> x) }
 let mcons (p: 'a IO) (q : 'a list IO) = p >>= (fun x -> q >>= (fun y -> result (x::y)))
 let sequence (l: 'a IO list) : 'a list IO = 
      List.foldBack mcons  l (result [])


//These run into problems because we don't support higher-kinded polymorphism.
// For example we really wish to write:
//
//let inline result (x:'a) : ^f<'a> = (^f<'a>).Result(x)
//let inline (>>=) (x:^f<'a>) (y:'a -> ^f<'b>) = (^f<'a>).BindOp<'b> (x,y)


//This is ok:
//let inline result (x:^a) : ^b = ^b.Result(x)
//This is not enough:
//let inline (>>>=) (x:^f) (y:'a -> ^g) = (^f.BindOp(x,y))

end


module StreamMonadTest = begin

    type 'a NumberOps = 
      { zero: 'a;
        one: 'a;
        add : 'a -> 'a -> 'a;
        mul : 'a -> 'a -> 'a;
        neg : 'a -> 'a; }

    let intOps = 
      { zero = 0;
        one = 1;
        add = (+);
        mul = ( * );
        neg = (~-); }
        
    let floatOps = 
      { zero = 0.0;
        one = 1.0;
        add = (+);
        mul = ( * );
        neg = (~-); }
        


end


module AnotherFuncTest = begin

    type func =
      class
        val numCalls: int ref
        val impl: int -> int option
        member x.Invoke(y) = incr x.numCalls; x.impl(y)
        new(f) = {  numCalls = ref 0; impl=f }
        static member (++) ((f: func), (g: func))  = 
          new func(fun x -> match f.Invoke(x) with None -> None | Some b -> g.Invoke(b))
      end
     

end


(*
module GenericFunc = begin

type ('a,'b) func =
  class
    val numCalls: int ref
    val impl: 'a -> 'b option
    member x.Invoke(y) = incr x.numCalls; x.impl(y)
    new(f) = { inherit obj(); numCalls = ref 0; impl=f }
    static member FF (f: func<'a,'b>)  (g: func<'b,'c>)  = 
      new func<'a,'c>(fun x -> match f.Invoke(x) with None -> None | Some b -> g.Invoke(b))
  end


end

module GenericFunc2 = begin

type func<'a,'b> =
  class
    val numCalls: int ref
    val impl: 'a -> 'b option

    new(f) = { inherit obj(); numCalls = ref 0; impl=f }

    static member FF (f: func<'a,'b>)  (g: func<'b,'c>)  = 
      new func<'a,'c>(fun x -> match f.Invoke(x) with None -> None | Some b -> g.Invoke(b))

    member x.Invoke(y : 'a) : 'b option = incr x.numCalls; x.impl(y)
  end


end

module AnotherGenericFunc = begin

type func<'a,'b> =
  class
    abstract Invoke : 'a -> 'b option
    static member FF (f: func<'a,'b>)  (g: func<'b,'c>)  = 
      {new func<'a,'c>() with Invoke(x) = match f.Invoke(x) with None -> None | Some b -> g.Invoke(b)}
    // FEATURE REQUEST: inherit should never be needed for base classes that have a default constructor and no other
    // constructors
    new() = { }
  end

let konst a = {new func<_,_>() with Invoke(x) = Some a}

let morph f = {new func<_,_>() with Invoke(x) = Some (f x)}

let something = func.FF (morph (fun x -> x * x)) (morph (fun x -> x + x))

end


module UsingPolymorphicRecursion1 = begin

type func<'a,'b> = { impl : 'a -> 'b option }

let invoke<'a,'b,..> (f:func<'a,'b>) x = f.impl x 

let rec invoke2<'a,..> (f:func<'a,'b>) x = f.impl x 
and compose (f: func<'a,'b>)  (g: func<'b,'c>)  = 
     {impl=(fun x -> match invoke f x with None -> None | Some b -> invoke g b)}


end

module UsingPolymorphicRecursion2 = begin

type func<'a,'b> = { impl : 'a -> 'b option }

let invoke<'a,'b> (f:func<'a,'b>) x = f.impl x 

let rec invoke2<'a,..> (f:func<'a,'b>) x = f.impl x 
and compose (f: func<'a,'b>)  (g: func<'b,'c>)  = 
     {impl=(fun x -> match invoke f x with None -> None | Some b -> invoke g b)}


end


module UsingPolymorphicRecursion3 = begin

type func<'a,'b> = { impl : 'a -> 'b option }

let invoke<'a,..> (f:func<'a,'b>) x = f.impl x 

let rec invoke2<'a,..> (f:func<'a,'b>) x = f.impl x 
and compose (f: func<'a,'b>)  (g: func<'b,'c>)  = 
     {impl=(fun x -> match invoke f x with None -> None | Some b -> invoke g b)}


end

module UsingPolymorphicRecursion4 = begin

type func<'a,'b> = { impl : 'a -> 'b option }

let invoke< .. > (f:func<'a,'b>) x = f.impl x 

let rec invoke2<'a,..> (f:func<'a,'b>) x = f.impl x 
and compose (f: func<'a,'b>)  (g: func<'b,'c>)  = 
     {impl=(fun x -> match invoke f x with None -> None | Some b -> invoke g b)}


end

module UsingPolymorphicRecursion5 = begin

type func<'a,'b> = { impl : 'a -> 'b option }

let rec invoke2<'a,'b,..> (f:func<'a,'b>) x = f.impl x 
and compose (f: func<'a,'b>)  (g: func<'b,'c>)  = 
     {impl=(fun x -> match invoke2 f x with None -> None | Some b -> invoke2 g b)}


end

module UsingPolymorphicRecursion6 = begin

type func<'a,'b> = { impl : 'a -> 'b option }

let rec invoke2<'a,'b> (f:func<'a,'b>) x = f.impl x 
and compose (f: func<'a,'b>)  (g: func<'b,'c>)  = 
     {impl=(fun x -> match invoke2 f x with None -> None | Some b -> invoke2 g b)}


end
*)




open Microsoft.FSharp.Math

module BasicOverloadTests = begin

    let f4 x = 1 + x

    // This gets type int -> int
    let f5 x = 1 - x

    // This gets type DateTime -> DateTime -> TimeSpan, through non-conservative resolution.
    let f6 x1 (x2:System.DateTime) = x1 - x2

    // This gets type TimeSpan -> TimeSpan -> TimeSpan, through non-conservative resolution.
    let f7 x1 (x2:System.TimeSpan) = x1 - x2

    // This gets type TimeSpan -> TimeSpan -> TimeSpan, through non-conservative resolution.
    let f8 x1 (x2:System.TimeSpan) = x2 - x1

    // This gets type TimeSpan -> TimeSpan -> TimeSpan, through non-conservative resolution.
    let f9 (x1:System.TimeSpan) x2 = x1 - x2


    // This gets type TimeSpan -> TimeSpan -> TimeSpan
    let f10 x1 (x2:System.TimeSpan) = x1 + x2

    // Note this gets a different type to f10 as more possible overloads become available through further
    // type annotations
    let f11 (x1:System.DateTime) (x2:System.TimeSpan) = x1 + x2


    let f17 (x1: float) x2 = x1 * x2
    let f18 (x1: int) x2 = x1 * x2
    let f19 x1 (x2:int) = x1 * x2
    let f20 x1 (x2:float) = x1 * x2
    let f21 x1 (x2:string) = x1 + x2
    let f22 (x1:string) x2 = x1 + x2



    // This one is tricky because the type of "s" is unknown, and could in theory resolve to a
    // nominal type which has an extra interesting "op_Addition(string,C)" overload.
    // However, non-conservative resolution is applied prior to method overload resolution in 
    // expressions. This means the overload constraint is resolved OK to op_Addition(string,string).
    let f26 s = stdout.Write("-"+s)

    let f27 x = x + x

    let f28 x = 
        let g x = x + x in
        g x

    let f29 x = 
        let g x = x + 1.0 in
        g x
        
    let f30 x = 
        let g x = x + "a" in
        g x + "3"
end

    
module SubtypingAndOperatorOverloads = begin
    type C() =
        class
            static member (+) (x:C,y:C) = new C()
        end
        
    type D() =
        class
            inherit C()
            static member (+) (x:D,y:D) = new D()
        end   

    let f201 (x1: C) (x2: C) = D.op_Addition(x1,x2)
    let f202 (x1: D) (x2: C) = D.op_Addition(x1,x2)
    let f203 (x1: D) (x2: D) = D.op_Addition(x1,x2)
    let f204 (x1: C) (x2: D) = D.op_Addition(x1,x2)
    let f205 (x1: C) (x2: _) = D.op_Addition(x1,x2)
    let f206 (x1: _) (x2: C) = D.op_Addition(x1,x2)
    
    let f301 (x1: C) (x2: C) = x1 + x2

    let f302 (x1: D) (x2: C) = x1 + x2

    let f303 (x1: D) (x2: D) = x1 + x2

    let f304 (x1: C) (x2: D) = x1 + x2

    let f305 (x1: C) (x2: _) = x1 + x2

    let f306 (x1: _) (x2: C) = x1 + x2

    // TODO: investigate
    let f307 (x1: D) (x2: _) = x1 + x2
    // TODO: investigate
    let f308 (x1: _) (x2: D) = x1 + x2
end
    
module OperatorOverloadsWithFloat = begin
    type C() =
        class
            static member (+) (x:C,y:float) = new C()

            static member (+) (x:float,y:C) = new C()

            static member (+) (x:C,y:C) = new C()
        end
        

    let f201 (x1: C) (x2: C)         = C.op_Addition(x1,x2)
    let f202 (x1: float) (x2: C)     = C.op_Addition(x1,x2)
    let f204 (x1: C) (x2: float)     = C.op_Addition(x1,x2)
#if NEGATIVE
    let f205 (x1: C) (x2: _)         = C.op_Addition(x1,x2)
    let f206 (x1: _) (x2: C)         = C.op_Addition(x1,x2)
#endif
    let f207 (x1: float) (x2: _)     = C.op_Addition(x1,x2)
    let f208 (x1: _) (x2: float)     = C.op_Addition(x1,x2)
    
    let f301 (x1: C) (x2: C)         = x1 + x2
    let f302 (x1: float) (x2: C)     = x1 + x2
    let f304 (x1: C) (x2: float)     = x1 + x2
    
    // TODO: investigate
    let f305 (x1: C) (x2: _)         = x1 + x2
    // TODO: investigate
    let f306 (x1: _) (x2: C)         = x1 + x2

end
    

//let f3 (a:matrix) (b:string) = a * b

module MiscOperatorOverloadTests = begin

    let rec findBounding2Power b tp = if b<=tp then tp else findBounding2Power b (tp*2) 
    let leastBounding2Power b =
        findBounding2Power b 1

    let inline sumfR f (a:int,b:int) =
        let mutable res = 0.0 in
        for i = a to b do
            res <- res + f i
        done;
        res

end


module EnumerationOperatorTests = begin
    let x1 : System.DateTimeKind =  enum 3
    let x2 : System.DateTimeKind =  enum<_> 3
    let x3 =  enum<System.DateTimeKind> 3
    let x4 =  int32 (enum<System.DateTimeKind> 3)
    let inline f5 x = x |> int32 |> enum

end

module TraitCallsAndConstructors = 
    open System
    let inline clone (v : ^a) = (^a : (new : string * Exception -> ^a) ("", v))
    let _ : InvalidOperationException = clone (InvalidOperationException())
    type Base(x : float) =
        member this.x = x
     
    let inline (~-) (v:^a) = (^a:(new : float -> ^a)(-(v:>Base).x))
     
    type Inherited(x : float) =
        inherit Base(x)
     
    let aBase = Base(5.0)
    let aInherited = Inherited(5.0)
     
     
    let _ : Inherited = -aInherited


let _ = 
  if !failures then (stdout.WriteLine "Test Failed"; exit 1) 
  else (stdout.WriteLine "Test Passed"; 
        System.IO.File.WriteAllText("test.ok","ok"); 
        exit 0)

