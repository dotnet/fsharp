
#r "System.Xml.Linq.dll"
#r "System.Xml.XDocument.dll"

let mutable failures : string list = []

open System

module BasicTypeDirectedConversionsToObj =
    // Constant expression 
    let x1 : obj = 1

    // Field literal expressions
    let x2 : obj = System.Int32.MaxValue

    // Field literal expressions
    let x3 : obj = System.Int32.Parse("12")

    // Tuple expressions
    let x4 : obj = (1,2)

    // Arithmetic expressions
    //  These do NOT permit type-directed subsumption nor widening because a generic return type is involved
    //  the is not yet committed
    // let x : obj = 1 + 1

    // Values
    let x8 (s:string) : obj = s

module BasicTypeDirectedConversionsToTuple =

    // Tuple expressions
    let x2 : obj * obj = (1,2)

    // Let expressions
    let x3 : (obj * obj) = (let x = 1 in let y = 2 in (x,y))

    // Struct tuple expressions
    let x4 : struct (obj * obj) = struct (1,2)

module BasicTypeDirectedConversionsForFuncReturn =

    // Return in lambda expressions
    let x5 : (unit -> obj) = (fun () -> 1)
    let x6 : (unit -> obj * obj) = (fun () -> (1,2))

    // Return in function expressions
    let x7 () : obj * obj = (1,2)

type R = { mutable F1: (obj * obj) }

module IntegerWidening =
    let i1 = 0
    let x0 : int64 = i1 // integer value
    let x1 : int64 = 0 // integer constant
    let x2 : int64 list = [1;2;3;4] // within a list
    let x3 : float list = [1;2;3;4]
    let x4 : float32 list = [1;2;3;4]
    let x5 : int64 = System.Int32.MaxValue
    let x6 : int64 list = [System.Int32.MaxValue;System.Int32.MaxValue]
    let f () = 1

    let x7 : obj = f()
    let x8 : int64 = f()
    let x9 : int64 = id (f()) // generic does work 

    // Arithmetic expressions
    //  These do NOT permit type-directed subsumption nor widening because a generic return type is involved
    //  the is not yet committed
    // let x6 : int64 = 1 + 1
    // let x6 : int64 = id (1 + 1)

module Overloads1 =
    type C() = 
        static member M1(x:int) = 1
        static member M1(x:int64) = failwith "nope"
    let x = C.M1(2)

module Overloads2 =
    type C() = 
        static member M1(x:int) = failwith "nope"
        static member M1(x:int64) = printfn "ok at line %s" __LINE__
    let x = C.M1(2L)

module Overloads3 =
    type C() = 
        static member M1(x:string) = failwith "nope"
        static member M1(x:int64) = printfn "ok at line %s" __LINE__
    let x = C.M1(2)

module OverloadedOptionals1 =
    type C() = 
        static member M1(x:string) = failwith "nope"
        static member M1(?x:int64) = printfn "ok at line %s" __LINE__
    let x = C.M1(x=2)

module OverloadedOptionals2 =
    type C() = 
        static member M1(?x:int) = printfn "ok at line %s" __LINE__
        static member M1(?x:int64) = failwith "nope"
    let x = C.M1(x=2)

module OverloadedOptionals3 =
    type C() = 
        static member M1(?x:int) =  failwith "nope"
        static member M1(?x:int64) =  printfn "ok at line %s" __LINE__
    let x = C.M1(x=2L)

module Optionals1 =
    type C() = 
        static member M1(?x:int64) = 1
    let x = C.M1(x=2)

module ParamArray1 =
    type C() = 
        static member M1([<System.ParamArray>] x:int64[]) = Array.sum x
    let x1 = C.M1(2)
    let x2 = C.M1(2, 3, 5L)

module ParamArray2 =
    type C() = 
        static member M1([<System.ParamArray>] x:double[]) = Array.sum x
    let x1 = C.M1(2)
    let x2 = C.M1(2, 3, 5.0)

module ConvertToSealedViaOpImplicit =
    [<Sealed>]
    type C() = 
        static member op_Implicit(x:int) = C()
        static member M1(C:C) = 1
    let x = C.M1(2)

module ConvertNoOverloadin =
    type C() = 
        static member M1(x:int64) = 1
    let x = C.M1(2)

module ConvertNoOverloadingViaOpImplicit =
    type C() = 
        static member M1(x:decimal) = ()
    let x = C.M1(2)

let d: decimal = 3

let ns : System.Xml.Linq.XNamespace = ""

module ConvertViaOpImplicit2 =
    type C() = 
        static member M1(ns:System.Xml.Linq.XNamespace) = 1
    let x = C.M1("")

module ConvertViaOpImplicit3 =
    type C() = 
        static member M1(ns:System.Xml.Linq.XName) = 1
    let x = C.M1("a")

module ConvertViaOpImplicit4 =
    type C() = 
        static member op_Implicit(x:int) = C()
        static member M1(C:C) = 1
    let x = C.M1(2)

module ConvertViaOpImplicit5 =
    type X() = 
        static member M1(x:X) = 1
    type Y() =
        static member op_Implicit(y:Y) = X()
    let x = X.M1(Y())

module ConvertViaOpImplicitGeneric =
    type C<'T>() = 
        static member op_Implicit(x: 'T) = C<'T>()
    
    let c:C<int> = 1
    let f (c:C<int>) = 1
    let x = f 2

module ConvertViaNullable =
    type C<'T>() = 
        static member op_Implicit(x: 'T) = C<'T>()
    
    let c:Nullable<int> = 1
    let f (c:Nullable<int>) = 1
    let x = f 2

let annotations = 
    let _ : obj = (1,2)
    let _ : obj * obj = (1,2)

    // structure through let
    let _ : (obj * obj) = (let x = 1 in let y = 2 in (x,y))

    // structure through let rec
    let _ : (obj * obj) = (let rec f x = x in (3,4.0))

    // structure through sequence
    let _ : (obj * obj) = (); (3,4.0)

    // struct tuple
    let _ : struct (obj * obj) = struct (1,2)
    
    // record (both field and overall result)
    // let _ : obj = { F1 = (1, 2) } // TODO
    
    // record (both field and overall result)
    { F1 = (1, 2uy) }.F1 <- (3.0, 4)
    
    // anon record
    let _ : {| A: obj |} = {| A = 1 |}
    
    // lambda return
    let _ : (unit -> obj) = (fun () -> 1)

    // function lambda return
    let _ : (int -> obj) = (function 1 -> 1 | 2 -> 3.0 | _ -> 4uy)

    let _ : (unit -> obj * obj) = (fun () -> (1,2))

    // constants
    (1 :> System.IComparable) |> ignore

    let _ : System.IComparable = 1 

    // array constants
    let _ : System.Array = [| 1us |] 

    let _ : System.Array = [| 1I |] 

    let _ : System.IComparable = 1I
    
    // property
    let _ : System.IComparable<string> = System.String.Empty 
    
    // method
    let _ : System.IComparable<string> = System.String.Format("")

    let _ : obj = System.String.Format("")

    let _ : System.IComparable = System.String.Format("")
    
    // array constants

    let _ : obj[] = [| 1 |] 
    let _ : (obj * obj)[] = [| (1,1) |]
    let _ : (obj * obj)[] = [| ("abc",1) |]
    let _ : (string * obj)[] = [| ("abc",1) |]
    let _ : (string * obj)[] = [| ("abc",1); ("abc",3.0) |]
    let _ : (string * obj)[] = [| Unchecked.defaultof<_>; ("abc",3.0) |]
    let _ : struct (string * obj)[] = [| Unchecked.defaultof<_>; struct ("abc",3.0) |]

    let _ : obj = 1
    let _ : obj = (1 : int)
    let _ : obj = ""
    let _ : obj = ("" : string)
    let _ : obj = ("" : System.IComparable)
    let _ : obj = ("" : _)
    let _ : obj = ("" : obj)
    let _ : obj = { new System.ICloneable with member x.Clone() = obj() }
    let _ : obj = ""
    let _ : obj = string ""
    let _ : obj = id ""
    
    // conditional
    let _ : obj = if true then 1 else 3.0
    let _ : obj = (if true then 1 else 3.0)
    let _ : obj = (if true then 1 elif true then 2uy else 3.0) 
    
    // try-with
    let _ : obj = try 1 with _ -> 3.0
    
    // try-finally
    let _ : obj = try 1 finally ()

    // match
    let _ : obj = match true with true -> 1 | _ -> 3.0
    ()

let f1 () : obj = 1
let f2 () : obj = if true then 1 else 3.0

#if NEGATIVE
let annotations = 
    (id "" : obj) |> ignore /// note, the 'obj' annotation correctly instantiates the type variable to 'obj'
    ((if true then ()) : obj) |> ignore

let f3 x = if true then 1 else 3.0
#endif

printfn "test done"

let aa =
  match failures with 
  | [] -> 
      stdout.WriteLine "Test Passed"
      System.IO.File.WriteAllText("test.ok","ok")
      exit 0
  | _ -> 
      printfn "Test Failed, failures = %A" failures
      exit 1

