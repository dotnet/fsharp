module Test

type MyType =
    | MyType of int

/// Extending a .NET primitive type with new operator

module DotNetPrimtiveWithNewOperator = 
    type System.Int32 with
        static member (++)(a: int, b: int) = a 

    let result = 1 ++ 2

/// Extending an F# type with + operator
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

    let v = MyType 3
    let result1 = v + v  
    let result2 = v * v
    let result3 = v - v
    let result4 = v / v
    let result5 = -v
    let result6 = v ||| v
    let result7 = v &&& v
    let result8 = v ^^^ v


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

    let v5 = count (ResizeArray [| 3 |])


/// Solving using LINQ extensions
module LinqExtensionMethodsProvideSolutions = 

    open System.Linq

    let inline count (a : ^A  when ^A : (member Count : int)) =
        (^A : (member Count : int) (a))

    let seqv = seq { yield 1; yield 2 }
    
    let v0 = seqv.Count // sanity check 

    let v1 = count seqv

