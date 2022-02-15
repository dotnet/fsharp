// #Regression #Conformance #Printing #FSI #Regression 

let repeatId = "A";;
let repeatId = "B";;

#load "testLoadFile.fsx";;                     (* single load *)
#load "testLoadFile.fsx" "testLoadFile2.fsx";; (* multi load *)

let x1 = Seq.initInfinite string  
let x2 = Seq.init 9 string
let x3 = Seq.init System.Int32.MaxValue string

let f1 = new System.Windows.Forms.Form(Text="f1 form")
let fs = Array.init 200 (fun i -> new System.Windows.Forms.Form(Text=sprintf "fs #%d" i))

let xs  = List.init  200 string 
let xa  = Array.init 200 string
let xa2 = Array2D.init 8 8 (fun i j -> string (i*10+j))


let sxs0  = List.init  0 string |> Set.ofList;; 
let sxs1  = List.init  1 string |> Set.ofList;; 
let sxs2  = List.init  2 string |> Set.ofList ;;
let sxs3  = List.init  3 string |> Set.ofList ;;
let sxs4  = List.init  4 string |> Set.ofList ;;
let sxs200  = List.init  200 string |> Set.ofList ;;

let msxs0  = List.init  0 (fun i -> (i,string i)) |> Map.ofList ;;
let msxs1  = List.init  1 (fun i -> (i,string i)) |> Map.ofList ;;
let msxs2  = List.init  2 (fun i -> (i,string i)) |> Map.ofList ;;
let msxs3  = List.init  3 (fun i -> (i,string i)) |> Map.ofList ;;
let msxs4  = List.init  4 (fun i -> (i,string i)) |> Map.ofList ;;
let msxs200  = List.init  200 (fun i -> (i,string i)) |> Map.ofList ;;

module M = begin
  let a = "sub-binding"
  let b = Some (x1,x2,x3,f1),Some(xs,xs,xa2)
end

type T(a,b) =
    member z.AProperty  = a
    member z.AMethod(x) = a*b+x*2
    static member StaticProperty = 101
    static member StaticMethod(x) = 102+x

let f_as_method :  int -> int  = fun x -> x+1
let f_as_thunk  : (int -> int) = f_as_method

// Moved to fsharpqa suite where we can easily handle differences between Dev10 and VS2008
//let lazyExit = lazy (exit 999; "this should never be forced")
let refCell  = ref "value"

module D1 = begin
  let words     = dict ["alpha",1;"beta",2]
  let words2000 = dict (List.init 2000 (fun i -> i,string i))
end;;

// Repeat words, but open namespace, reduces the type name paths
open System.Collections.Generic
;; // <-- close interaction before reduced NS comes into effect

module D2 = begin
  open System.Collections.Generic
  let words     = dict ["alpha",1;"beta",2]
  let words2000 = dict (List.init 2000 (fun i -> i,string i))  
end

let opt1  = None 
let opt1b = None : int option
let opt4  = Some (Some (Some (None)))
let opt4b = Some (Some (Some (None : int option)))
let opt5  = [Some(Some(Some(Some(None))));
             Some(Some(Some(Some(Some [1;2;3;4;5;6]))));
             Some(Some(Some(Some(Some [1;2;3;4;5;6;7;8;9;0;1;2;3;4;5;6;7;8;9;01;2;3;4;5;6;7;8;9;01;2;3;4;5;6;7;8;9;01;2;3;4;5;6;7;8;9;01;2;3;4;5;6;7;8;9;0]))))]


let mkStr n = String.init n (fun _ -> "-")
let strs  = Array.init 100 (fun i -> mkStr (i  ))
let str7s = Array.init 100 (fun i -> mkStr (i*7))
let grids = Array2D.init 50 50 (fun i j -> mkStr (i*j))
;;  

type tree = L | N of tree list
let rec mkT w d = if d=0 then L else N (List.init w (fun i -> mkT w (d-1)))
let tree w d =
  printf "[Building %d %d..." w d    
  let res = mkT w d in
  printfn "done]"
  res
;;  
let tree_2_4  = tree 2 4  ;;
let tree_2_6  = tree 2 6  ;;
let tree_2_8  = tree 2 8  ;;
let tree_2_10 = tree 2 10 ;;
let tree_2_12 = tree 2 12 ;;
let tree_2_14 = tree 2 14 ;;

let tree_3_8  = tree 3 8  ;;
let tree_4_8  = tree 4 8  ;;
let tree_5_8  = tree 5 8  ;;
let tree_6_8  = tree 6 8  ;;
  
let tree_5_3  = tree 5 3  ;;
;;

type X =
  | Var of int
  | Bop of int * X * X
let rec generate x =
  match x%2 with
    | 0 -> Var x
    | 1 -> Bop (x,generate (x/2),generate (x/3))
    | _ -> failwith "unexpected residue"
;;
let exps = List.map generate [1;2;3;4;5;6;7;8;9;10;
                              213;
                              21342314;
                              3214;
                              1231357;
                              5234547;
                              923759825;
                              2435234;
                              12396777;
                              3333333;
                              1312311237;
                              System.Int32.MaxValue;
                             ]
;;  
module Exprs = begin
  let x1 = generate 213
  let x2 = generate 21342314
  let x3 = generate 3214
  let x4 = generate 1231357
  let x5 = generate 5234547
  let x6 = generate 923759825
  let x7 = generate 2435234
  let x8 = generate 12396777
  let x9 = generate 3333333
  let x10 = generate 1312311237
  let x11 = generate System.Int32.MaxValue
end
;;


type C(x:string) =
    override z.ToString() = failwithf "Trouble_%s" x

let c1  = C "A"
//  cs = Array.init 200 (string >> C) // constructors are not yet first class functions...
let csA = Array.init 200 (string >> (fun x -> C x))
let csB = Array.init 200 (fun x -> C (string x))
let csC = Array.init 999 (fun x -> C (string x))
;;

(* Bug 4045 *)
exception Abc;;
exception AbcInt of int;;
exception AbcString of string;;
exception AbcExn of exn list;;
exception AbcException of System.Exception list;;
let exA1 = Abc
let exA2 = AbcInt 2
let exA3 = AbcString "3"
let exA4 = AbcExn [exA1;exA2;exA3]
let exA5 = AbcException [exA4]

(* Bug 4045 *)
exception Ex0
exception ExUnit of unit
exception ExUnits of unit * unit
exception ExUnitOption of unit option

let ex0  = Ex0
let exU  = ExUnit ()
let exUs = ExUnits ((),())
let exUSome = ExUnitOption (Some ())
let exUNone = ExUnitOption (None)

(* Bug 4063 *)
// ctor case
type 'a T4063 = AT4063 of 'a;;
let valAT3063_12   = AT4063 12;;
let valAT3063_True = AT4063 true;;
let valAT3063_text = AT4063 "text";;
let valAT3063_null = AT4063 (null:System.Object);;

type M4063<'a> = class new(x:'a) = {} end;;
let v4063 =  M4063<int>(1);;

// method case?
type Taaaaa<'a>() = class end;;
type Taaaaa2<'a>() = class inherit Taaaaa<'a>() member x.M() = x end;;

// method case?
type Tbbbbb<'a>(x:'a) = class member this.M() = x end;;
type Tbbbbb2(x) = class inherit Tbbbbb<string>(x) end;;
let t2 = Tbbbbb2("2") in t2.M;;

// Moved to fsharpqa suite where we can easily handle differences between Dev10 and VS2008
(* Bug 4068: printing lazy property values forces the lazy value... *)
//let lazy12 = lazy 12;;
//lazy12;;
//lazy12;;
//lazy 13;;
//it;;

(* Bug 1532 *)
module RepeatedModule = begin let repeatedByteLiteral = [| 12uy; 13uy; 14uy |] end;;
module RepeatedModule = begin let repeatedByteLiteral = [| 12uy; 13uy; 14uy |] end;;

(* Regressions for standard responses... *)
"Check #help";;
#help;;
"Check #time on and then off";;
#time;; (* time on *)
(* no eval in between, since time can vary and look like a regression *)
#time;; (* time off *)
"Check #unknown command";;
#blaaaaaa // blaaaaaa is not a known command;;
"Check #I with a known directory (to avoid a warning, which includes the location of this file, which is fragile...)";;
#I "/";;

(* Regressions for #r and binding responses... *)
// Moved to FSHARPQA - we can't have hardcoded string in tests!

(* Regression 4006 *)
type internal T1 = A | B;;
type internal T2 = {x: int};;
type internal T3 = class end;;
type internal T4 = class new() = {} end;;
type T1 = internal A | B;;
type T2 = internal {x: int};;
type private  T1 = A | B;;
type private  T2 = {x: int};;
type T1 = private  A | B;;
type T2 = private  {x: int};;
type internal T1 = private  A | B;;
type internal T2 = private  {x: int};;
type private  T3 = class end;;
type private  T4 = class new() = {} end;;


(* Regression 4086 *)
exception X1 of int;;
exception private X2 of int;;
exception internal X3 of int;;



(* Regression 3552 *)
type T0         = class new() = {} end
type T1Post<'a> = class new() = {} end
type 'a T1Pre   = class new() = {} end
;;
type T0 with
    member this.M() = [this]
    member this.P   = this,this
    member this.E   = (new Event<int>()).Publish
;;
type T1Post<'a> with
    member this.M() = [this]
    member this.P   = this,this
    member this.E   = (new Event<_>()).Publish
;;
type 'a T1Pre with
    member this.M() = [this]
    member this.P   = this,this
    member this.E   = (new Event<_>()).Publish
;;
(* Mix up pre and post *)
type 'a T1Post with
    member this.M() = [this]
    member this.P   = this,this
    member this.E   = (new Event<_>()).Publish
;;
type T1Pre<'a> with
    member this.M() = [this]
    member this.P   = this,this
    member this.E   = (new Event<_>()).Publish
;;




(* Regression 4090: request for pretty printing to count record fields as one item w.r.t. PrintSize total node count *)
type r = { f0 : int;
           f1 : int;
           f2 : int;
           f3 : int;           
           f4 : int;           
           f5 : int;           
           f6 : int;           
           f7 : int;           
           f8 : int;           
           f9 : int; }
let r10 = { f0 = 0; f1 = 1; f2 = 2; f3 = 3; f4 = 4; f5 = 5; f6 = 6; f7 = 7; f8 = 8; f9 = 9 }
let r10s  = Array.init 37 (fun i -> r10)
let r10s' = "one extra node",Array.init 37 (fun i -> r10)
;;

(* Regression 1564: allow #directives to interleave definitions in fsi without need for ;; *)
let x1564_A1 = 1
#I "\\"
let x1564_A2 = 2
#I "\\"
let x1564_A3 = 3
;;



type internal Foo2() = 
     member public this.Prop1 = 12
     member internal this.Prop2 = 12
     member private this.Prop3 = 12
     public new(x:int) = new Foo2()
     internal new(x:int,y:int) = new Foo2()
     private new(x:int,y:int,z:int) = new Foo2();;
     
module internal InternalM = 
    let internal x = 1

    type internal Foo2() = 
       member public this.Prop1 = 12
       member internal this.Prop2 = 12
       member private this.Prop3 = 12
       public new(x:int) = new Foo2()
       internal new(x:int,y:int) = new Foo2()
       private new(x:int,y:int,z:int) = new Foo2()
       
    type private Foo3() = 
       member public this.Prop1 = 12
       member internal this.Prop2 = 12
       member private this.Prop3 = 12
       public new(x:int) = new Foo3()
       internal new(x:int,y:int) = new Foo3()
       private new(x:int,y:int,z:int) = new Foo3()

    type internal T1 = A | B
    type internal T2 = {x: int}
    type internal T3 = class end
    type internal T4 = class new() = {} end
    type T5 = internal A | B
    type T6 = internal {x: int}
    type private  T7 = A | B
    type private  T8 = {x: int}
    type T9 = private  A | B
    type T10 = private  {x: int}
    type internal T11 = private  A | B
    type internal T12 = private  {x: int}
    type private  T13 = class end
    type private  T14 = class new() = {} end
       
module internal PrivateM = 
    let private x = 1

    type private Foo2() = 
       member public this.Prop1 = 12
       member internal this.Prop2 = 12
       member private this.Prop3 = 12
       public new(x:int) = new Foo2()
       internal new(x:int,y:int) = new Foo2()
       private new(x:int,y:int,z:int) = new Foo2()

    type internal T1 = A | B
    type internal T2 = {x: int}
    type internal T3 = class end
    type internal T4 = class new() = {} end
    type T5 = internal A | B
    type T6 = internal {x: int}
    type private  T7 = A | B
    type private  T8 = {x: int}
    type T9 = private  A | B
    type T10 = private  {x: int}
    type internal T11 = private  A | B
    type internal T12 = private  {x: int}
    type private  T13 = class end
    type private  T14 = class new() = {} end


(* Regression 4412: Do not break before = on val it = <expr> *)
;;[(43, "10/28/2008", 1); (46, "11/18/2008", 1); (56, "1/27/2009", 2); (58, "2/10/2009", 1)] |> Seq.readonly
;;

(* Regression: 4343 a) Long strings print prefix and indication of remaining characters *)
module Test4343a =
  let mk i = String.init i (fun i -> string (i%10))
  let x100 = mk 100
  let x90  = mk 90
  let x80  = mk 80
  let x75  = mk 75
  let x74  = mk 74
  let x73  = mk 73
  let x72  = mk 72
  let x71  = mk 71
  let x70  = mk 70
 
(* Regression: 4343 b) Functions (1st class) not printed *)
module Test4343b =
  let fA (x:int) = x+2
  let fB<'a> (x:'a) (y:'a) = [x;y]
  let gA = fA
  let gB = fB
  let gAB = fA,fB
  let hA,hB = gAB

(* Regression: 4343 c) Type functions, not printed *)
module Test4343c =
  let typename<'a> = typeof<'a>.FullName
  let typename2<'a> = typename<'a>,typename<'a list>

(* Regression: 4343 d) Sequences not printed, unless a known sequence type, like, list, array etc, which take precedence *)
module Test4343d =
  let xList   = [1;2;3]    // ienumerable
  let xArray  = [|1;2;3|]  // ienumerable
  let xString = "abcdef"   // ienumerable
  let xOption = Some 12    // could be ienumerable, but not currently
  let xArray2 = Array2D.init 2 2 (fun x y -> x,y)
  let xSeq    = [1;2;3] |> Seq.readonly // seq proper, expect no RHS value to be printed

(* Regression: 4343 e) Values that would use the default System.Object.ToString() are not printed, since this only prints the type, i.e. val x : T = T *)
module Test4343e =
  // C uses default .ToString
  type C(x:int) = class end
  let cA = C(1)
  let cB = C(2)
  let cAB = cA,cB,[cA;cB] (* note: these print with FSI_xxxx prefix. That is bug 4299. *)
  // D defines it's own .ToString
  type D(x:int) = class override this.ToString() = "D(" + string x + ")" end
  let dA = D(1)
  let dB = D(2)
  let dAB = dA,dB,[dA;dB]

  module Generic =
    // C<Generic> uses default .ToString
    type CGeneric<'a>(x:'a) = class end
    let cA = C(1)
    let cB = C(2)
    let cAB = cA,cB,[cA;cB] (* note: these print with FSI_xxxx prefix. That is bug 4299. *)
    // D<Generic> defines it's own .ToString
    type D<'a>(x:'a) = class override this.ToString() = "D(" + string (box x) + ")" end
    let dA = D<int>(1)
    let dB = D<int>(2)
    let dAB = dA,dB,[dA;dB ]
    let dC = D<bool>(true)
    let boxed_dABC = [box dA;box dB;box dC]

(* Regression: 4624. printing C# and F# classes can sort their member, inherit, implements, fields, ctors, instance, statics etc. *)
[<AbstractClass>]
type F1 =
    class
        inherit System.Windows.Forms.Form                         (* inherit *)
        abstract AAA : int  with get                              (* abstract getter *)
        abstract ZZZ : int  with get                              (* abstract setter *)
        abstract BBB : bool with set
        abstract MMM : bool -> bool                               (* abstract method *)
        val x : F1                                                (* instance field *)
        [<DefaultValue>]
        static val mutable private sx : F1                        (* static field *)
        static member A() = 12                                    (* static method *)
        member this.B() = 12                                      (* instance method *)
        static member C() = 12
        member this.D() = 12
        member this.D2 with get() = 12 and set(x:int) = ()        (* instance getter setter *)
        member this.D(x:int,y:int) = 12
        member this.D(x:int) = 12
     //   member this.D x y z = [1;x;y;z]  // you can't define a curried member with same name as tupled member, even with different signature
        member this.E with get() = 12                             (* instance getter *)
        member this.E with set(x:int) = ()                        (* instance setter *)                  
        val x2 : F1                                               (* instance field *)
        [<DefaultValue>]
        static val mutable private sx2 : F1                       (* static field *)      
        override this.ToString() = ""                             (* instance override *)
        interface System.IDisposable with                         (* interface implementation *)
            override this.Dispose() = ()
        end
    end

(* Regression 4643: not a regression, but a corner case: static fields on struct *)
[<Struct>]
type IP(x:int,y:int) =
  [<DefaultValue>]
  static val mutable private AA : IP


(* Bug 4643: infinite loop in typechecker - caused by recursive struct check via self typed static field *)  
module Regression4643 =  
   [<Struct>]
   type RIP(x:int) =
      [<DefaultValue>]
      static val mutable private y : RIP

   [<Struct>]
   type arg_unused_is_RIP(x:RIP) = struct end

   [<Struct>]
   type arg_used_is_RIP(x:RIP) =
        member this.X = x

   [<Struct>]
   type field_is_RIP =
      val x :RIP

(* Bug 5152: Provide appropriate message for the InvalidOperation exns thrown by Seq find, pick and findIndex *)
type Either<'a,'b> = This of 'a | That of 'b
let catch f = try This (f ()) with e -> That (e.GetType().FullName,e.Message)
let seqFindIndexFailure = catch (fun () -> Seq.findIndex (fun _ -> false) [1;2])
let seqFindFailure      = catch (fun () -> Seq.find      (fun _ -> false) [1;2])
let seqPickFailure      = catch (fun () -> Seq.pick      (fun _ -> None : int option)  [1;2])
        
(* Bug 5218. Tuple printing of arity beyond maxTuple representation exercises F# tuple reflection in reflect.fs *)
module Regression5218 =
    let t1  = 1
    let t2  = 1,2
    let t3  = 1,2,3
    let t4  = 1,2,3,4
    let t5  = 1,2,3,4,5
    let t6  = 1,2,3,4,5,6
    let t7  = 1,2,3,4,5,6,7
    let t8  = 1,2,3,4,5,6,7,8
    let t9  = 1,2,3,4,5,6,7,8,9
    let t10 = 1,2,3,4,5,6,7,8,9,10
    let t11 = 1,2,3,4,5,6,7,8,9,10,11
    let t12 = 1,2,3,4,5,6,7,8,9,10,11,12
    let t13 = 1,2,3,4,5,6,7,8,9,10,11,12,13
    let t14 = 1,2,3,4,5,6,7,8,9,10,11,12,13,14
    let t15 = 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15
;;

(* Bug 3739: ICE in fsi with interface constraint on type generic parameter (for interface defined in the same interaction) *)
module Regression3739 =    
    type IB = 
       abstract AbstractMember : int -> int
    // Expected: type C accepted
    // Observed: Could not load type 'IB' from assembly 'FSI-ASSEMBLY, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'.
    type C<'a when 'a :> IB>() = 
        static member StaticMember(x:'a) = x.AbstractMember(1)
;; (* intentional ;; *)

    
(* Bug 3739: ICE in fsi with interface constraint on type generic parameter (for interface defined in the same interaction) *)
module Regression3739 =    
    type IB = 
       abstract AbstractMember : int -> int
    // Expected: type C accepted
    // Observed: Could not load type 'IB' from assembly 'FSI-ASSEMBLY, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null'.
    type C<'a when 'a :> IB>() = 
        static member StaticMember(x:'a) = x.AbstractMember(1)
;; (* intentional ;; *)

(* Bug 4368: FSI throws ICE for interface with override name colliding with method name [was: when running test fsharp\samples\mort3] *)
module Regression3740 =        
    type Writer<'a> = 
      interface
        abstract member get_path : unit -> string
      end
      
    type MyClass =
      class
        val path: string
        interface Writer<int> with
          member self.get_path () = "hello"
        end
      end
;; (* intentional ;; *)

(* Bug 4319: Sanity checks for overloaded and user defined operators? *)
// EXPECT: T2 accepted
type Regression4319_T2  = static member (+-+-+) (x,y)            = "2 arguments";;

// EXPECT: warnings on all the following. The ;; are intentional, so warnings and output interleave.
type Regression4319_T0  = static member (+-+-+)             = "0 arguments";;
type Regression4319_T1  = static member (+-+-+) x           = "1 argument";;
type Regression4319_T1b = static member (+-+-+) (x)         = "1 (argument) [brackets make no diff]";;
type Regression4319_T1c = static member (+-+-+) x           = let a,b = x in "1 argument, tuple typed from RHS. Still not OK";;
type Regression4319_T1d = static member (+-+-+) (x:int*int) = "1 argument, tuple typed from LHS. Still not OK";;

type Regression4319_T3  = static member (+-+-+) (x,y,z)          = "3 arguments";;
type Regression4319_U1  = static member (+-+-+) x       moreArgs = "1 argument and further args";;
type Regression4319_U1b = static member (+-+-+) (x)     moreArgs = "1 (argument) [brackets make no diff] and further args";;
type Regression4319_U2  = static member (+-+-+) (x,y)   moreArgs = "1 argument and further args";;
type Regression4319_U3  = static member (+-+-+) (x,y,z) moreArgs = "1 argument and further args";;

type Regression4319_check =
    static member (:=)             = "COLON_EQUALS"
    //static member (||)           = "BAR_BAR"
    //static member (|)            = "INFIX_BAR_OP"                                              
    //static member ($..$|)          = "INFIX_BAR_OP"
    static member (&)              = "AMP"
    static member (&^)             = "AMP_AMP"
    static member (=)              = "EQUALS"
    //static member (=)            = "INFIX_COMPARE_OP"    
    static member (!=)             = "INFIX_COMPARE_OP"
    //static member (<)            = "INFIX_COMPARE_OP"
    //static member (>)            = "INFIX_COMPARE_OP"
    //static member ($)            = "INFIX_COMPARE_OP"                    
    static member (...=)          = "INFIX_COMPARE_OP" // with $. prefix    
    static member (...!=)         = "INFIX_COMPARE_OP" // with $. prefix    
    static member (...<)          = "INFIX_COMPARE_OP" // with $. prefix    
    static member (...>)          = "INFIX_COMPARE_OP" // with $. prefix    
    //static member (...$)          = "INFIX_COMPARE_OP" // with $ suffix
    static member ($)              = "DOLLAR"
    static member (<)              = "LESS"
    static member (>)              = "GREATER"
    static member (@)              = "INFIX_AT_HAT_OP"
    static member (^)              = "INFIX_AT_HAT_OP"        
    static member (...@)          = "INFIX_AT_HAT_OP" // with $. prefix    
    static member (...^)          = "INFIX_AT_HAT_OP" // with $. prefix    
    static member (%)              = "PERCENT_OP"        
    //static member (::)           = "COLON_COLON"        
    static member (-)              = "MINUS"
    static member ( * )            = "STAR"
    //static member ( * )          = "INFIX_STAR_DIV_MOD_OP"
    static member (/)              = "INFIX_STAR_DIV_MOD_OP"
    //static member (%)            = "INFIX_STAR_DIV_MOD_OP"
    static member ( ...* )        = "INFIX_STAR_DIV_MOD_OP" // with $. prefix    
    static member ( .../ )        = "INFIX_STAR_DIV_MOD_OP" // with $. prefix    
    static member ( ...% )        = "INFIX_STAR_DIV_MOD_OP" // with $. prefix
    static member ( ** )           = "INFIX_STAR_STAR_OP"
;; (* intentional ;; *)


type Regression4469() =
    member this.ToString() = "ABC"
let r4469 = Regression4469()
printfn "Expect ABC = %s" (r4469.ToString());; // <-- semi here, but not before, is important to split the interactions
printfn "Expect ABC = %s" (r4469.ToString());;

module Regression1019_short =
  let double_nan      = nan
  let double_infinity = infinity
  let single_nan      = nanf
  let single_infinity = infinityf

module Regression1019_long =
  let double_nan      = System.Double.NaN
  let double_infinity = System.Double.PositiveInfinity
  let single_nan      = System.Single.NaN
  let single_infinity = System.Single.PositiveInfinity
    
// check 'it' doesn't get collected if it is referenced
;;
ref 1;;
let x,f = it, (fun () -> !it);; // this will read from the static storage for 'it'
f();; // expect 1
x := 3;;
f();; // expect 3

// check 'it' does get collected if it is not referenced, 400K * 100 = 40000MB = 40GB, so we'd better have some collection here
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K
(Array.zeroCreate 100000 : int[]);; // 400K

(* Check [] and some other values can be generalized when used as 'it' expressions *)
[];;
[[]];;
None;;
([],[]);;
(fun x -> x);;
let fff x = x;;
fff;;

let note_ExpectDupMethod = "Regression4927: Expect error due to duplicate methods in the following definition";; (* <-- semi-semi matters *)
type ExpectDupMethod() =
  class
    member this.M() = 12
    member this.M() = "string"
end;;  (* <-- semi-semi matters, -ve test *)

let note_ExpectDupProperty = "Regression4927: Expect error due to duplicate properties in the following definition";; (* <-- semi-semi matters *)
type ExpectDupProperty() =
  class
    member this.P = 12
    member this.P = "string"
end;; (* <-- semi-semi matters, -ve test *)

;;
"NOTE: Expect IAPrivate less accessible IBPublic";;
module Regression5265_PriPub =
    type private  IAPrivate  = interface abstract P: int end
    type public   IBPublic   = interface inherit IAPrivate abstract Q : int end
;; (* <-- semi-semi matters, -ve test *)
"NOTE: Expect IAPrivate less accessible IBInternal";;
module Regression5265_PriInt =
    type private  IAPrivate  = interface abstract P: int end
    type internal IBInternal = interface inherit IAPrivate abstract Q : int end
;; (* <-- semi-semi matters, -ve test *)
module Regression5265_PriPri =
    type private  IAPrivate  = interface abstract P: int end
    type private  IBPrivate  = interface inherit IAPrivate abstract Q : int end
;;
"NOTE: Expect IAInternal less accessible IBPublic";;
module Regression5265_IntPub =
    type internal IAInternal = interface abstract P: int end
    type public   IBPublic   = interface inherit IAInternal abstract Q : int end
;; (* <-- semi-semi matters, -ve test *)
module Regression5265_IntInt =
    type internal IAInternal = interface abstract P: int end
    type internal IBInternal = interface inherit IAInternal abstract Q : int end
;;
module Regression5265_IntPri =
    type internal IAInternal = interface abstract P: int end
    type private  IBPrivate  = interface inherit IAInternal abstract Q : int end
;;
module Regression5265_PubPub =
    type public   IAPublic   = interface abstract P: int end
    type public   IBPublic   = interface inherit IAPublic abstract Q : int end
;;
module Regression5265_PubInt =
    type public   IAPublic   = interface abstract P: int end
    type internal IBInternal = interface inherit IAPublic abstract Q : int end
;;
module Regression5265_PubPri =
    type public   IAPublic   = interface abstract P: int end
    type private  IBPrivate  = interface inherit IAPublic abstract Q : int end
;; (* ;; needed, to issolate error regressions *)

"Regression4232: Expect an error about duplicate virtual methods from parent type";;
module Regression4232 =
    [<AbstractClass>]
    type D<'T,'U>() = 
        abstract M : 'T  -> int
        abstract M : 'U -> int

    type E() = 
        inherit D<string,string>()
        override x.M(a:string) = 1
;; (* ;; needed, to issolate error regressions *)

"** Expect AnAxHostSubClass to be accepted. AxHost has a newslot virtual RightToLeft property outscope RightToLeft on Control";;
type AnAxHostSubClass(x) = class inherit System.Windows.Forms.AxHost(x) end;;

(*Regression 5590*)
"** Expect error because the active pattern result contains free type variables";;
let (|A|B|) (x:int) = A x;;

"** Expect error because the active pattern result contains free type variables (match value generic)";;
let (|A|B|) (x:'a) = A x;;

"** Expect error because the active pattern result contains free type variables (when active pattern also has parameters)";;
let (|A|B|) (p:'a) (x:int) = A p;;

"** Expect OK, since error message says constraint should work!";;
let (|A|B|) (x:int) : Choice<int,unit> = A x;;

"** Expect error since active pattern is not a function!";;
let (|A|B|) = failwith "" : Choice<int,int>;;

"** Expect OK since active pattern result is not too generic, typars depend on match val";;
let (|A|B|) (p:bool) (x : 'a * 'b) = if p then A (fst x) else B (snd x);;

"** Expect OK since active pattern result is not too generic, typars depend on parameters";;
let (|A|B|) (aval:'a) (bval:'b) (x : bool) = if x then A aval else B bval;;

"** Expect OK since active pattern result is generic, but it typar from closure, so OK";;
let outer (x:'a) =
    let (|A|B|) (k:int) = if k = 0 then A else B (x:'a)
    fun x -> match x with A -> None | B res -> Some res;;

"** Expect OK, BUG 472278: revert unintended breaking change to Active Patterns in F# 3.0";;
let (|Check1|) (a : int) = a, None;;

;; 
module ReflectionEmit =
    // Interfaces cross-constrained via method gps - targets reflection emit in fsi
    type IA = 
        abstract M : 'a -> int when 'a :> IB 
    and  IB = 
        abstract M : 'b -> int when 'b :> IA

    // Interfaces cross-constrained via method gps - targets reflection emit in fsi
    type IA2<'a when 'a :> IB2<'a> and 'a :> IA2<'a>> = 
        abstract M : int
    and  IB2<'b when 'b :> IA2<'b> and 'b :> IB2<'b>> = 
        abstract M : int
;;
"Regression_139182: Expect the follow code to be accepted without error";;
[<Struct>]
type S =
  member x.TheMethod() = x.TheMethod() : int64
let theMethod (s:S) = s.TheMethod()
type T() =
  static let s = S()
  static let str = "test"
  let s2 = S()
  static member Prop1 = s.TheMethod()                // error FS0422
  static member Prop2 = theMethod s                  // ok
  static member Prop3 = let s' = s in s'.TheMethod() // ok
  static member Prop4 = str.ToLower()                // ok
  member x.Prop5      = s2.TheMethod()               // ok
;;

// BCL type with Value property that is annotated with DebuggerBrowsable(Never)
[new System.Threading.ThreadLocal<int>()]
;;

// named DU fields.
// only fields whose names do not match auto-generated field names (Item1, Item2, ...) should be printed
type MyDU =
  | Case1 of Val1 : int * Val2 : string
  | Case2 of string * V2 : bool * float
  | Case3 of Item : int
  | Case4 of Item1 : bool
  | Case5 of Item1 : bool * Item2 : string
  | Case6 of Val1 : int * bool * Item3 : string
  | Case7 of ``Big Name`` : int

let namedFieldVar1 = Case1(5, "")
let namedFieldVar2 = Case7(25)
;;

// F# exception types use similar printing logic for named fields as DUs
// auto-generated names use "Data" instead of "Item"
exception MyNamedException1 of Val1 : int * Val2 : string
exception MyNamedException2 of string * V2 : bool * float
exception MyNamedException3 of Data : int
exception MyNamedException4 of Data0 : bool
exception MyNamedException5 of Data0 : int * Data1 : string
exception MyNamedException6 of Val1 : int * bool * Data2 : string * Data8 : float
exception MyNamedException7 of ``Big Named Field`` : int

let namedEx1 = MyNamedException1(5, "")
let namedEx2 = MyNamedException7(25)
;;

type optionRecord = { x: int option }
let x = { x = None }
;;

type optionRecord = { x: obj }
let x = { x = null }
;;

type RecordWithMembers =
    { x: obj }
    member a.Property = 1
    member a.Method() = 2
;;
type UnionWithMembers =
    /// This is Case1
    | Case1
    /// This is Case2
    | Case2 of int
    /// This is Property
    member a.Property = 1
    /// This is Method
    member a.Method() = 2
;;
type OneFieldRecordNoXmlDoc =
    { OneField: obj}
;;
type OneFieldRecordXmlDoc =
    { /// Hello!
      OneField: obj}
;;
type TwoFieldRecordNoXmlDoc =
    { TwoFields1: obj; TwoFields2: obj }
;;
type TwoFieldRecordXmlDoc =
    { /// Goog
      TwoFields1: obj;
      /// Even more good
      TwoFields2: obj }
;;
type System.Int32 with 
    member a.ExtrinsicExtensionProperty = 1
    member a.ExtrinsicExtensionMethod() = 2
;;

let ``value with spaces in name``   = true
;;

let functionWhichTakesLongNameMixedParameters 
        (aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa: int, 
         bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb: int) 
        (ccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc: int, 
         dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd: int) = 1 + 1
;;

let functionWhichTakesLongNameTupledParameters 
         (aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa: int, 
          bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb: int, 
          ccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc: int, 
          ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd: int) = 1 + 1
;;

let functionWhichTakesLongNameCurriedParameters 
        (aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa: int)
        (bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb: int)
        (cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc: int)
        (dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd: int) = 1 + 1
;;

let functionWhichTakesMixedLengthCurriedParametersA a b c ddddddddddddddddddddddddddddddddddddddddddddd = 1 + 1
;;

let functionWhichTakesMixedLengthCurriedParametersB aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa b c d = 1 + 1
;;


let f (``parameter with spaces in name``: int) = 1
;;

let functionWhichTakesAParameterPeeciselyPlusButNotOpAddition (``+``: int -> int -> int) = ``+`` 1 2
;;

let functionWhichTakesAParameterOpAddition ((+): int -> int -> int) = 1 + 1
;;

let functionWhichTakesAParameterCalled_land (``land``: int -> int -> int) = 1 + 1
;;

type RecordWithStrangeNames =
    { 
       ``funky name`` : obj 
       op_Addition : obj 
       ``+`` : obj 
       ``land`` : obj 
       ``base`` : obj 
       }
;;

type UnionWithSpacesInNamesOfCases =
    | ``Funky name``   // Check this gets double ticks
    | ``Funky name 2``  // Check this gets double ticks
;;

type ``Type with spaces in name`` = // Check this gets double ticks
    | A
    | B
;;

type op_Addition = // Check this doesn't go to (+) for types
    | A
    | B
;;

type ``land`` = // Check this doesn't go to (land) for types, it gets double ticks because (land) is deprecated
    | A
    | B
;;

module ``Module with spaces in name`` = // Check this gets double ticks
    let x = 1
;;

module op_Addition = // Check this doesn't go to (+) for modules, nor get double ticks
    let x = 1
;;

module ``land`` = // Check this doesn't go to (land) for modules, it gets double ticks because (land) is deprecated
    let x = 1
;;

let ``+`` x y = 1  // This is not op_Addition but a function called '+'
;;

let (+) x y = x + y + 1  // This is op_Addition not a function called '+'
;;

let ``base`` = 2  // This is not a base value but a value called 'base'
;;

let ``mod`` = 2  // This is a value called 'mod' in .NET IL, but we can't distinguish from (mod), so print it as (mod)
;;

let ``or`` = 2  // This is a value called 'or' in .NET IL, legacy, but we can't distinguish from  (or), so print it as ``or``
;;

let ``land`` = 2  // This is a value called 'land' in .NET IL, legacy, but we can't distinguish from legacy unused (land), so print it as ``land``
;;

let ``.ctor`` = 2  // This is a value called '.ctor' in .NET IL, and has no special properties
;;

let ``.cctor`` = 2  // This is a value called '.cctor' in .NET IL, and has no special properties
;;

// Check line wrapping of very long literals
[<Literal>]
let SomeLiteralWithASomewhatLongName = "SomeVeryLongLiteralValueWithLotsOfCharacters"

[<Literal>]
let SomeLiteralWithASomewhatLongName2 = "SomeVeryLongLiteralValueWithLotsOfCharactersSomeVeryLongLiteralValueWithLotsOfCharactersSomeVeryLongLiteralValueWithLotsOfCharacters"

[<Literal>]
let ShortName = "hi"
;;

System.DayOfWeek.Tuesday
;;
let internal f() = 1;; f();; // should give a warning in multi-assembly interactive emit
;; (* ;; needed, to isolate error regressions *)

;;exit 0;; (* piped in to enable error regressions *)