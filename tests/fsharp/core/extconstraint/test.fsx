#if TESTS_AS_APP
module Core_extconstraint
#endif


let failures = ref []

let reportFailure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures := !failures @ [s]


let check s e r = 
    if r = e then stdout.WriteLine (s + ": YES") 
    else (stdout.WriteLine ("\n***** " + s + ": FAIL\n"); reportFailure s)

let test s b =      
    if b then ()
    else (stderr.WriteLine ("failure: " + s); 
        reportFailure s)


type MyType =
    | MyType of int

/// Extending a .NET primitive type with new operator
module DotNetPrimtiveWithNewOperator = 
    type System.Int32 with
        static member (++)(a: int, b: int) = a 
    do check "jfs9dlfdh" 1 (1 ++ 2)

/// Extending a .NET primitive type with new operator
module DotNetPrimtiveWithAmbiguousNewOperator = 
    [<AutoOpen>]
    module Extensions = 
        type System.Int32 with
            static member (++)(a: int, b: int) = a 

    do check "jfs9dlfdhsx" 1 (1 ++ 2)

    [<AutoOpen>]
    module Extensions2 = 
        type System.Int32 with
            static member (++)(a: int, b: string) = a 

    do check "jfs9dlfdhsx1" 1 (1 ++ "2")

    let f (x: string) = 1 ++ x
    
    do check "jfs9dlfdhsx2" 1 (f "2")
    // TODO: this gives an internal error
    // let f x = 1 ++ x

/// Extending a .NET primitive type with new _internal_ operator
module DotNetPrimtiveWithInternalOperator1 = 
    type System.Int32 with
        static member internal (++)(a: int, b: int) = a 

    let result = 1 ++ 2 // this is now allowed
    check "vgfmjsdokfj" result 1


/// Extending a .NET primitive type with new _private_ operator where that operator is accessible at point-of-use
module DotNetPrimtiveWithAccessibleOperator2 = 
    type System.Int32 with
        static member private (++)(a: int, b: int) = a 

    let result = 1 ++ 2 // this is now allowed. 
    check "vgfmjsdokfjc" result 1



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
            static member (^^^)(MyType x, MyType y) = MyType (x ^^^ y)
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
    do check "fsdnjioa1" (MyType 6) result1
    let result2 = v * v
    do check "fsdnjioa2" (MyType 9) result2
    let result3 = v - v
    do check "fsdnjioa3" (MyType 0) result3
    let result4 = v / v
    do check "fsdnjioa4" (MyType 1) result4
    let result5 = -v
    do check "fsdnjioa5" (MyType -3) result5
    let result6 = v ||| v
    do check "fsdnjioa6" (MyType 3) result6
    let result7 = v &&& v
    do check "fsdnjioa7" (MyType 3) result7
    let result8 = v ^^^ v
    do check "fsdnjioa8" (MyType 0) result8
    let result9 = LanguagePrimitives.GenericZero<MyType>
    do check "fsdnjioa9" (MyType 0) result9
    let result10 = LanguagePrimitives.GenericOne<MyType>
    do check "fsdnjioa10" (MyType 1) result10
    let result11 = sign v
    do check "fsdnjioa11" 1 result11
    let result12 = abs v
    do check "fsdnjioa12" (MyType 3) result12
    let result13 = sqrt v
    do check "fsdnjioa13" (MyType 1) result13
    let result14 = sin v
    do check "fsdnjioa14" (MyType 0) result14
    let result15 = cos v
    do check "fsdnjioa15" (MyType 0) result15
    let result16 = tan v
    do check "fsdnjioa16" (MyType 0) result16
    let result17 = LanguagePrimitives.DivideByInt v 4
    do check "fsdnjioa17" (MyType 0) result17


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
        
    /// The check is that the above code compiles OK

/// Extending a generic type with a property
module ExtendingGenericTypeWithProperty  = 

    type List<'T> with
        member x.Count = x.Length

    let inline count (a : ^A  when ^A : (member Count : int)) =
        (^A : (member Count : int) (a))

    let v0 = [3].Count // sanity check 
    do check "opcjdkfdf" 1 v0
    
    let v3 = count [3]
    do check "opcjdkfdfx" 1 v3

    let v5 = count (ResizeArray [| 3 |])
    do check "opcjdkfdfxa" 1 v5

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
    do check "fdoiodjjs" 1 v0

    let v1 = [|3|].Count // sanity check 
    do check "fdoiodxjxjs" 1 v1
    
    let v3 = count [3]
    do check "fdoios" 1 v3

    let v4 = count [| 3 |]
    do check "fddxjxjs" 1 v4

    let v5 = count (dict [| 1,3 |])
    do check "fdoiosdxs" 1 v5

    let v6 = count (ResizeArray [| 3 |]) // intrinsic from .NET
    do check "fdojxxjs" 1 v6




/// Solving using LINQ extensions
module LinqExtensionMethodsProvideSolutions_Count = 

    open System.Linq

    // Note this looks for a _method_ called `Count` taking a single argument
    // It is _not_ considered the same as a property called `Count`
    let inline countm (a : ^A  when ^A : (member Count : unit -> int)) =
        (^A : (member Count : unit -> int) (a))

    let seqv : seq<int> = Seq.singleton 1
    
    let v0 = seqv.Count() // sanity check 
    do check "fivjijvd" 1 v0

    let v1 = countm seqv
    do check "fivjixjvd" 1 v1

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
    do check "cojkicjkc" 1 v

module ExtendingOnConstraint = 
    open System
    type System.Int32 with 
        static member inline (+)(a, b) = Array.map2 (+) a b

    let v1 = [|1;2;3|] + [|2;3;4|] //Okay
    do check "kldjfdo1" [|3;5;7|] v1
    let v2 = [|TimeSpan(52342L)|] + [|TimeSpan(3213L)|] //Okay
    do check "kldjfdo2" ([|TimeSpan(52342L + 3213L)|]) v2
    let v3 = [|1m|] + [|2m|] //Okay
    do check "kldjfdo3" ([|3m|]) v3
    let v4 = [|1uy|] + [|2uy|] //Okay
    do check "kldjfdo4" ([|3uy|]) v4
    let v5 = [|1L|] + [|2L|] //Okay
    do check "kldjfdo5" ([|3L|]) v5
    let v6 = [|1I|] + [|2I|] //Okay
    do check "kldjfdo6" ([|3I|]) v6
    let v7 = [| [|1 ; 1|]; [|2|] |] + [| [|2; 2|]; [|3|] |] //Okay
    do check "kldjfdo7" [| [|3 ; 3|]; [|5|] |] v7
    let v8 = [| [| [| [|2|] |] |] |] + [| [| [| [|5|] |] |] |] //Okay
    do check "kldjfdo8" [| [| [| [|7|] |] |] |] v8
    //Errors:
    //let v9 = [|"1"|] + [|"2"|] //error FS0001
    //let v10 = [|1.f|] + [|2.f|] //error FS0001
    //let v11 = [|1.0|] + [|2.0|] //error FS0001


(*---------------------------------------------------------------------------
!* wrap up
 *--------------------------------------------------------------------------- *)


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

