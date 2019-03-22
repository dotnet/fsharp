module Test

type MyType =
    | MyType of int

/// Extending a .NET primitive type with new operator
module DotNetPrimtiveWithNewOperator = 
    type System.Int32 with
        static member (++)(a: int, b: int) = a 

    let result = 1 ++ 2

/// Extending a .NET primitive type with new operator
module DotNetPrimtiveWithAmbiguousNewOperator = 
    [<AutoOpen>]
    module Extensions = 
        type System.Int32 with
            static member (++)(a: int, b: int) = a 

    [<AutoOpen>]
    module Extensions2 = 
        type System.Int32 with
            static member (++)(a: int, b: string) = a 

    let f (x: string) = 1 ++ x
    // TODO: this gives an internal error
    // let f x = 1 ++ x

/// Extending a .NET primitive type with new _internal_ operator
module DotNetPrimtiveWithInternalOperator1 = 
    type System.Int32 with
        static member internal (++)(a: int, b: int) = a 

    let result = 1 ++ 2 // this is now allowed


/// Extending a .NET primitive type with new _private_ operator where that operator is accessible at point-of-use
module DotNetPrimtiveWithAccessibleOperator2 = 
    type System.Int32 with
        static member private (++)(a: int, b: int) = a 

    let result = 1 ++ 2 // this is now allowed. 



#if NEGATIVE_TESTS
module DotNetPrimtiveWithInaccessibleOperator = 
    [<AutoOpen>]
    module Extensions = 
        type System.Int32 with
            static member private (++)(a: int, b: int) = a 

    let result = 1 ++ 2 // This should fail to compile because the private member is not accessible from here
#endif


/// Locally extending an F# type with a wide range of standard operators
module FSharpTypeWithExtrinsicOperators = 

    [<AutoOpen>]
    module Extensions = 
        type MyType with
            static member (+)(MyType x, MyType y) = MyType (x + y)
            static member (*)(MyType x, MyType y) = MyType (x * y)
            static member (/)(MyType x, MyType y) = MyType (x / y)
            static member (-)(MyType x, MyType y) = MyType (x - y)
            static member (~-)(MyType x) = MyType (-x)
            static member (|||)(MyType x, MyType y) = MyType (x ||| y)
            static member (&&&)(MyType x, MyType y) = MyType (x &&& y)
            static member (^^^)(MyType x, MyType y) = MyType (x &&& y)
            static member Zero = MyType 0
            static member One = MyType 1
            member this.Sign = let (MyType x) = this in sign x
            static member Abs (MyType x) = MyType (abs x)
            static member Sqrt (MyType x) = MyType (int (sqrt (float x)))
            static member Sin (MyType x) = MyType (int (sin (float x)))
            static member Cos (MyType x) = MyType (int (cos (float x)))
            static member Tan (MyType x) = MyType (int (tan (float x)))
            static member DivideByInt (MyType x, n: int) = MyType (x / n)

    let v = MyType 3
    let result1 = v + v  
    let result2 = v * v
    let result3 = v - v
    let result4 = v / v
    let result5 = -v
    let result6 = v ||| v
    let result7 = v &&& v
    let result8 = v ^^^ v
    let result9 = LanguagePrimitives.GenericZero<MyType>
    let result10 = LanguagePrimitives.GenericOne<MyType>
    let result11 = sign v
    let result12 = abs v
    let result13 = sqrt v
    let result14 = sin v
    let result15 = cos v
    let result16 = tan v
    let result17 = LanguagePrimitives.DivideByInt v 4


/// Extending two types with the static member 'Add'
module TwoTypesWithExtensionOfSameName = 

    [<AutoOpen>]
    module Extensions = 
        type System.Int32 with
            static member Add(a: int, b: int) = a 

        type MyType with
            static member Add(MyType x, MyType y) = MyType (x + y)

    let inline addGeneric< ^A when ^A : (static member Add : ^A * ^A -> ^A) > (a,b) : ^A =
        (^A : (static member Add : ^A * ^A -> ^A) (a,b))

    let inline (+++) a b = addGeneric(a,b)

    let inline addGeneric2  (a,b) : ^A when ^A : (static member Add : ^A * ^A -> ^A) =
        (^A : (static member Add : ^A * ^A -> ^A) (a,b))

    let inline (++++) a b = addGeneric2(a,b)


    let f () =
        let v1 = addGeneric (MyType(1), MyType(2))
        let v2 = addGeneric (1,1)
        ()

    let f2 () =
        let v1 = MyType(1) +++ MyType(2)
        let v2 = 1 +++ 1
        1

    let f3 () =
        let v1 = addGeneric2 (MyType(1), MyType(2))
        let v2 = addGeneric2 (1,1)
        ()

    let f4 () =
        let v1 = MyType(1) ++++ MyType(2)
        let v2 = 1 ++++ 1
        ()  


/// Extending a generic type with a property
module ExtendingGenericTypeWithProperty  = 

    type List<'T> with
        member x.Count = x.Length

    let inline count (a : ^A  when ^A : (member Count : int)) =
        (^A : (member Count : int) (a))

    let v0 = [3].Count // sanity check 

    let v3 = count [3]

    let v5 = count (ResizeArray [| 3 |])

/// Extending a generic type with a property
/// Extending the .NET array type with a property
module ExtendingGenericTypeAndArrayWithProperty  = 

    type List<'T> with
        member x.Count = x.Length

    type ``[]``<'T> with
        member x.Count = x.Length

    let inline count (a : ^A  when ^A : (member Count : int)) =
        (^A : (member Count : int) (a))

    let v0 = [3].Count // sanity check 

    let v1 = [|3|].Count // sanity check 

    let v3 = count [3]

    let v4 = count [| 3 |]

    let v5 = count (dict [| 1,3 |])

    let v6 = count (ResizeArray [| 3 |]) // intrinsic from .NET




/// Solving using LINQ extensions
module LinqExtensionMethodsProvideSolutions_Count = 

    open System.Linq

    // Note this looks for a _method_ called `Count` taking a single argument
    // It is _not_ considered the same as a proprty called `Count`
    let inline countm (a : ^A  when ^A : (member Count : unit -> int)) =
        (^A : (member Count : unit -> int) (a))

    let seqv : seq<int> = Seq.singleton 1
    
    let v0 = seqv.Count // sanity check 

    let v1 = countm seqv

/// A random example
module ContainsKeyExample   = 
    let inline containsKey (k: ^Key) (a : ^A  when ^A : (member ContainsKey : ^Key -> bool)) =
        (^A : (member ContainsKey : ^Key -> bool) (a,k))

    let v5 = containsKey 1 (dict [| 1,3 |])

    // Note that without 'inline' this doesn't become generic
    let inline f x = containsKey x (dict [| (x,1) |])

(*
/// Not implemented
module MapExample   = 
    let inline map (f: ^T -> ^U) (a : ^A when ^A : (val map : (^T -> ^U) -> ^A -> ^A2)) =
        (^A : (val map : (^T -> ^U) -> ^A -> ^A2) (f, a))

    let v5 = map (fun x -> x + 1) [ 1 .. 100 ]

*)
module ExtenstionAttributeMembers = 
    open System.Runtime.CompilerServices
    [<Extension>]
    type Ext2() = 
        [<Extension>]
        static member Bleh(a : string) = a.Length

    let inline bleh s = (^a : (member Bleh : unit -> int) s)

    let v = bleh "a"

module Errors = 
    open System
    type System.Int32 with 
        static member inline (+)(a, b) = Array.map2 (+) a b

    let _ = [|1;2;3|] + [|2;3;4|] //Okay
    let _ = [|TimeSpan.Zero|] + [|TimeSpan.Zero|] //Okay
    let _ = [|1m|] + [|2m|] //Okay
    let _ = [|1uy|] + [|2uy|] //Okay
    let _ = [|1L|] + [|2L|] //Okay
    let _ = [|1I|] + [|2I|] //Okay
    let _ = [| [|1 ; 1|]; [|2|] |] + [| [|2; 2|]; [|3|] |] //Okay
    let _ = [|"1"|] + [|"2"|] //error FS0001
    let _ = [|1.f|] + [|2.f|] //error FS0001
    let _ = [|1.0|] + [|2.0|] //error FS0001
