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
    else 
        printf "\n***** %s: FAIL, expected %A, got %A\n" s r e
        reportFailure s

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
    //let f x = 1 ++ x

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
    //Compile Errors:
    //let v9 = [|"1"|] + [|"2"|] //error FS0001
    //let v10 = [|1.f|] + [|2.f|] //error FS0001
    //let v11 = [|1.0|] + [|2.0|] //error FS0001

module TupleOps = 
    type System.Int32 with 
        static member inline (+)(struct(a,b), struct(c,d)) = struct(a + c, b + d)
        static member inline (+)((a,b), (c,d)) = (a + c, b + d)
    let v1 = (1,2) + (3,4) 
    do check "fmjkslo1" ((4,6)) v1
    //let v2 = struct(1,2) + struct(3,4) 
    //do check "fmjkslo2" (struct(4,6)) v2
    //Runtime Errors: 
    (* ---------------------
        Unhandled Exception: System.TypeInitializationException: The type initializer for 'AdditionDynamicImplTable`3' threw an exception. ---> System.NotSupportedException: Dynamic invocation of op_Addition involving coercions is not supported.
        at Microsoft.FSharp.Core.LanguagePrimitives.dyn@2578TTT.Invoke(Unit unitVar0)
        at Microsoft.FSharp.Core.LanguagePrimitives.AdditionDynamicImplTable`3..cctor()
        --- End of inner exception stack trace ---
        at Microsoft.FSharp.Core.LanguagePrimitives.AdditionDynamicImplTable`3.get_Impl()
        at Microsoft.FSharp.Core.LanguagePrimitives.AdditionDynamic[T1,T2,TResult](T1 x, T2 y)
        at <StartupCode$test>.$Test$fsx.main@()
    --------------------------*)
(*---------------------------------------------------------------------------
!* wrap up
 *--------------------------------------------------------------------------- *)

module SystematicTests = 

    // 1-arg extensions to each primitive type
    // 2-arg extensions to each primitive type
    // 2-arg extensions to each primitive type + new sealed type
    // 2-arg extensions to each primitive type + new unsealed type
    // 2-arg extensions to new sealed type + each primitive type
    // 2-arg extensions to new unsealed type + each primitive type
    // 2-arg extensions to new sealed type + new unsealed type
    let inline CallStaticMethod1 (x: ^T) = ((^T): (static member StaticMethod1: ^T -> ^T) (x))
    let inline CallStaticMethod2 (x: ^T, y: ^T) = ((^T): (static member StaticMethod2: ^T * ^T -> ^T) (x, y))
    let inline CallStaticMethod3 (x: ^T, y: ^U) = ((^T or ^U): (static member StaticMethod3: ^T * ^U -> ^V) (x, y))
    let inline CallOverloadedStaticMethod4 (x: ^T, y: ^U) = ((^T or ^U): (static member OverloadedStaticMethod4: ^T * ^U -> ^V) (x, y))
    let inline CallInstanceMethod1 (x: ^T, y: ^T) = ((^T): (member InstanceMethod1: ^T -> ^T) (x, y))
    let inline CallInstanceProperty1 (x: ^T) = ((^T): (member InstanceProperty1: ^T) (x))
    let inline CallStaticProperty1 () = ((^T): (static member StaticProperty1: ^T) ())

    module MethodsOnStructType =

        [<Struct>]
        type R =
            { F : int }
        
            static member op_UnaryNegation (x: R) = { F = x.F + 4 }
            static member StaticMethod1 (x: R) = { F = x.F + 4 }
            static member StaticMethod2 (x: R, y: R) = { F = x.F + y.F + 4 }
            static member op_Addition (x: R, y: R) = { F = x.F + y.F + 4 }
            static member op_Subtraction (x: R, y: R) = { F = x.F + y.F + 5 }
            static member op_Division (x: R, y: R) = { F = x.F + y.F + 6 }
            static member StaticMethod3 (x: R, y: R) = { F = x.F + y.F + 4 }
            static member OverloadedStaticMethod4 (x: R, y: string) = { F = x.F + y.Length + 4 }
            static member OverloadedStaticMethod4 (x: R, y: int) = { F = x.F + y + 4 }
            member x.InstanceMethod1 (y: R) = { F = x.F + y.F + 5 }
            static member StaticProperty1 = { F = 4 }
            member x.InstanceProperty1 = { F = x.F + 4 }
        
        let r3 = { F = 3 }
        let r4 = { F = 4 }
        check "qvwoiwvoi0" (-r3).F 7
        check "qvwoiwvoi1" (CallStaticMethod1 r3).F 7
        check "qvwoiwvoi2" (CallStaticMethod2 (r3, r4)).F 11
        check "qvwoiwvoi2b" ((+) r3 r4).F 11
        check "qvwoiwvoi2c" ((-) r3 r4).F 12
        check "qvwoiwvoi2c" ((/) r3 r4).F 13
        check "qvwoiwvoi3" (CallStaticMethod3 (r3, r4)).F 11
        check "qvwoiwvoi4" (CallOverloadedStaticMethod4 (r3, 4)).F 11
        check "qvwoiwvoi5" (CallOverloadedStaticMethod4 (r3, "four")).F 11
        check "qvwoiwvoi6" (CallInstanceMethod1 (r3, r4)).F 12
        check "qvwoiwvoi7" (CallInstanceProperty1 (r3)).F 7
        check "qvwoiwvoi8" (CallStaticProperty1().F : int32) 4

    module ExtensionsOnStructType =

        [<Struct>]
        type R =
            { F : int }
        
        [<AutoOpen>]
        module Extensions = 
            type R with
                static member op_UnaryNegation (x: R) = { F = x.F + 4 }
                static member StaticMethod1 (x: R) = { F = x.F + 4 }
                static member StaticMethod2 (x: R, y: R) = { F = x.F + y.F + 4 }
                static member op_Addition (x: R, y: R) = { F = x.F + y.F + 4 }
                static member op_Subtraction (x: R, y: R) = { F = x.F + y.F + 5 }
                static member op_Division (x: R, y: R) = { F = x.F + y.F + 6 }
                static member StaticMethod3 (x: R, y: R) = { F = x.F + y.F + 4 }
                static member OverloadedStaticMethod4 (x: R, y: string) = { F = x.F + y.Length + 4 }
                static member OverloadedStaticMethod4 (x: R, y: int) = { F = x.F + y + 4 }
                member x.InstanceMethod1 (y: R) = { F = x.F + y.F + 5 }
                static member StaticProperty1 = { F = 4 }
                member x.InstanceProperty1 = { F = x.F + 4 }
        
        let r3 = { F = 3 }
        let r4 = { F = 4 }
        check "aqvwoiwvoi0" (-r3).F 7
        check "aqvwoiwvoi1" (CallStaticMethod1 r3).F 7
        check "aqvwoiwvoi2" (CallStaticMethod2 (r3, r4)).F 11
        check "aqvwoiwvoi2b" ((+) r3 r4).F 11
        check "aqvwoiwvoi2c" ((-) r3 r4).F 12
        check "aqvwoiwvoi2c" ((/) r3 r4).F 13
        check "aqvwoiwvoi3" (CallStaticMethod3 (r3, r4)).F 11
        check "aqvwoiwvoi4" (CallOverloadedStaticMethod4 (r3, 4)).F 11
        check "aqvwoiwvoi5" (CallOverloadedStaticMethod4 (r3, "four")).F 11
        //check "aqvwoiwvoi6" (CallInstanceMethod1 (r3, r4)).F 12  // TODO - FAILING
        //check "aqvwoiwvoi7" (CallInstanceProperty1 (r3)).F 7     // TODO - FAILING
        check "aqvwoiwvoi8" (CallStaticProperty1().F : int32) 4


    module MixedOverloadedOperatorMethodsOnStructType =

        [<Struct>]
        type R =
            { F : int }
        
            static member op_Addition (x: R, y: R) = { F = x.F + y.F + 4 }
            static member op_Addition (x: R, y: string) = { F = x.F + y.Length + 6 }
            static member op_Addition (x: R, y: int) = { F = x.F + y + 6 }
            static member op_Addition (x: string, y: R) = { F = x.Length + y.F + 9 }
            static member op_Addition (x: int, y: R) = { F = x + y.F + 102 }
        
        let r3 = { F = 3 }
        let r4 = { F = 4 }
        check "qvwoiwvoi2b" ((+) r3 r4).F 11   
        check "qvwoiwvoi2b" ((+) r3 "four").F 13 
        check "qvwoiwvoi2b" ((+) "four" r3).F 16
        check "qvwoiwvoi2b" ((+) r3 4).F 13
        check "qvwoiwvoi2b" ((+) 4 r3).F 109
        // TODO - more operators here

    module MixedOverloadedOperatorExtensionsOnStructType =

        [<Struct>]
        type R =
            { F : int }
        
        [<AutoOpen>]
        module Extensions = 
            type R with
                static member op_Addition (x: R, y: R) = { F = x.F + y.F + 4 }
                static member op_Addition (x: R, y: string) = { F = x.F + y.Length + 6 }
                static member op_Addition (x: R, y: int) = { F = x.F + y + 6 }
                static member op_Addition (x: string, y: R) = { F = x.Length + y.F + 9 }
                static member op_Addition (x: int, y: R) = { F = x + y.F + 102 }
        
        let r3 = { F = 3 }
        let r4 = { F = 4 }
        check "qvwoiwvoi2b" ((+) r3 r4).F 11   
        check "qvwoiwvoi2b" ((+) r3 "four").F 13  
        check "qvwoiwvoi2b" ((+) "four" r3).F 16 
        check "qvwoiwvoi2b" ((+) r3 4).F 13 
        check "qvwoiwvoi2b" ((+) 4 r3).F 109 
        //check "qvwoiwvoi2c" ((-) r3 r4).F 12
        //check "qvwoiwvoi2c" ((/) r3 r4).F 13
        // TODO - more operators here


    module ExtensionsToPrimitiveType_Int32 =

        [<AutoOpen>]
        module Extensions = 
            type System.Int32 with
                static member StaticMethod1 (x: int32) = x + 4
                static member StaticMethod2 (x: int32, y: int32) = x + y + 4
                static member StaticMethod3 (x: int32, y: int32) = x + y + 4
                static member OverloadedStaticMethod4 (x: int32, y: string) = x + y.Length + 4
                static member OverloadedStaticMethod4 (x: int32, y: int) = x + y + 4
                member x.InstanceMethod1 (y: int32) = x + y + 5
                static member StaticProperty1 = 4
                member x.InstanceProperty1 = x + 4

        check "2vwoiwvoi1" (CallStaticMethod1 3) 7
        check "2vwoiwvoi2" (CallStaticMethod2 (3, 4)) 11
        check "2vwoiwvoi3" (CallStaticMethod3 (3, 4)) 11
        check "2vwoiwvoi4" (CallOverloadedStaticMethod4 (3, 4)) 11
        check "2vwoiwvoi5" (CallOverloadedStaticMethod4 (3, "four")) 11
        //check "2vwoiwvoi6" (CallInstanceMethod1 (3, 4)) 12  // TODO- BUG - CODEGEN
        //check "2vwoiwvoi7" (CallInstanceProperty1 (3)) 7    // TODO- BUG - CODEGEN
        check "2vwoiwvoi8" (CallStaticProperty1 () : int32) 4


module Test1 = 

    open System
    type Foo = A | B

    module Extensions = 
        type Foo with
            static member (+) (foo1: Foo, foo2: Foo) = B

    open Extensions

    let result = A + A

    type System.String with
        member this.Foo (x: string) = this + x

module Test2 = 

    open System
    type Foo = A | B

    module Extensions = 
        type Foo with
            static member (+) (foo1: Foo, foo2: Foo) = B

        type Foo with
            static member (+) (foo1: Foo, foo2: string) = B
            static member (+) (foo1: string, foo2: Foo) = B

    open Extensions

    let result = A + A
    let result2 = A + ""
    let result3 = "" + A
    let result4 : string = "" + ""

    type System.String with
        member this.Foo (x: string) = this + x

    type System.String with
        member this.Foo2 x = this + x

    type Bar = Bar of String
        with 
        member this.Foo (x: string) = 
            match this with
            | Bar y -> y + x

    let z = "Bar".Foo("foo")
    let z0 = (Bar "Bar").Foo("foo")

module FSharpPlus_Applicatives =
    open System

    type Ap = Ap with
        static member inline Invoke (x:'T) : '``Applicative<'T>`` =
            let inline call (mthd : ^M, output : ^R) = ((^M or ^R) : (static member Return: _*_ -> _) output, mthd)
            call (Ap, Unchecked.defaultof<'``Applicative<'T>``>) x 
        static member inline InvokeOnInstance (x:'T) = (^``Applicative<'T>`` : (static member Return: ^T -> ^``Applicative<'T>``) x)
        static member inline Return (r:'R       , _:obj) = Ap.InvokeOnInstance      :_ -> 'R
        static member        Return (_:seq<'a>  , Ap   ) = fun x -> Seq.singleton x : seq<'a>
        static member        Return (_:Tuple<'a>, Ap   ) = fun x -> Tuple x         : Tuple<'a>
        static member        Return (_:'r -> 'a , Ap   ) = fun k _ -> k             : 'a  -> 'r -> _

    let inline result (x:'T) = Ap.Invoke x

    let inline (<*>) (f:'``Applicative<'T->'U>``) (x:'``Applicative<'T>``) : '``Applicative<'U>`` = 
        (( ^``Applicative<'T->'U>`` or ^``Applicative<'T>`` or ^``Applicative<'U>``) : (static member (<*>): _*_ -> _) f, x)

    let inline (+) (a:'Num) (b:'Num) :'Num = a + b

    type ZipList<'s> = ZipList of 's seq with
        static member Return (x:'a)                              = ZipList (Seq.initInfinite (fun _ -> x))
        static member (<*>) (ZipList (f:seq<'a->'b>), ZipList x) = ZipList (Seq.zip f x |> Seq.map (fun (f, x) -> f x)) :ZipList<'b>

    type Ii = Ii
    type Idiomatic = Idiomatic with
        static member inline ($) (Idiomatic, si) = fun sfi x -> (Idiomatic $ x) (sfi <*> si)
        static member        ($) (Idiomatic, Ii) = id
    let inline idiomatic a b = (Idiomatic $ b) a
    let inline iI x = (idiomatic << result) x

    let res1n2n3 = iI (+) (result          0M                  ) (ZipList [1M;2M;3M]) Ii
    let res2n3n4 = iI (+) (result LanguagePrimitives.GenericOne) (ZipList [1 ;2 ;3 ]) Ii

module FSharpPlus_FoldArgs =
    type FoldArgs<'t> = FoldArgs of ('t -> 't -> 't)

    let inline foldArgs f (x:'t) (y:'t) :'rest = (FoldArgs f $ Unchecked.defaultof<'rest>) x y

    type FoldArgs<'t> with
        static member inline ($) (FoldArgs f, _:'t-> 'rest) = fun (a:'t) -> f a >> foldArgs f
        static member        ($) (FoldArgs f, _:'t        ) = f

    let test1() =
        let x:int     = foldArgs (+) 2 3 
        let y:int     = foldArgs (+) 2 3 4
        let z:int     = foldArgs (+) 2 3 4 5
        let d:decimal = foldArgs (+) 2M 3M 4M
        let e:string  = foldArgs (+) "h" "e" "l" "l" "o"
        let f:float   = foldArgs (+) 2. 3. 4.

        let mult3Numbers a b c = a * b * c
        let res2 = mult3Numbers 3 (foldArgs (+) 3 4) (foldArgs (+) 2 2 3 3)
        ()

    // Run the test
    test1() 

// From https://github.com/dotnet/fsharp/issues/4171#issuecomment-528063764
module TypeInferenceChangeWithSealedType1 =
    // [<Sealed>]
    type Id<'t> (v: 't) =
       let value = v
       member __.getValue = value

    [<RequireQualifiedAccess>]
    module Id =
        let run   (x: Id<_>) = x.getValue
        let map f (x: Id<_>) = Id (f x.getValue)
        let create x = Id x


    type Bind =
        static member        (>>=) (source: Lazy<'T>   , f: 'T -> Lazy<'U>    ) = lazy (f source.Value).Value                                   : Lazy<'U>
        static member        (>>=) (source: Task<'T>   , f: 'T -> Task<'U>    ) = source.ContinueWith(fun (x: Task<_>) -> f x.Result).Unwrap () : Task<'U>
        static member        (>>=) (source             , f: 'T -> _           ) = Option.bind   f source                                        : option<'U>
        static member        (>>=) (source             , f: 'T -> _           ) = async.Bind (source, f)  
        static member        (>>=) (source : Id<_>     , f: 'T -> _           ) = f source.getValue                                 : Id<'U>

        static member inline Invoke (source: '``Monad<'T>``) (binder: 'T -> '``Monad<'U>``) : '``Monad<'U>`` =
            let inline call (_mthd: 'M, input: 'I, _output: 'R, f) = ((^M or ^I or ^R) : (static member (>>=) : _*_ -> _) input, f)
            call (Unchecked.defaultof<Bind>, source, Unchecked.defaultof<'``Monad<'U>``>, binder)

    let inline (>>=) (x: '``Monad<'T>``) (f: 'T->'``Monad<'U>``) : '``Monad<'U>`` = Bind.Invoke x f

    type Return =
        static member inline Invoke (x: 'T) : '``Applicative<'T>`` =
            let inline call (mthd: ^M, output: ^R) = ((^M or ^R) : (static member Return : _*_ -> _) output, mthd)
            call (Unchecked.defaultof<Return>, Unchecked.defaultof<'``Applicative<'T>``>) x

        static member        Return (_: Lazy<'a>       , _: Return  ) = fun x -> Lazy<_>.CreateFromValue x : Lazy<'a>
        static member        Return (_: 'a Task        , _: Return  ) = fun x -> Task.FromResult x : 'a Task
        static member        Return (_: option<'a>     , _: Return  ) = fun x -> Some x                : option<'a>
        static member        Return (_: 'a Async       , _: Return  ) = fun (x: 'a) -> async.Return x
        static member        Return (_: 'a Id          , _: Return  ) = fun (x: 'a) -> Id x

    let inline result (x: 'T) : '``Functor<'T>`` = Return.Invoke x


    type TypeT<'``monad<'t>``> = TypeT of obj
    type Node<'``monad<'t>``,'t> = A | B of 't * TypeT<'``monad<'t>``>

    let inline wrap (mit: 'mit) =
            let _mnil  = (result Unchecked.defaultof<'t> : 'mt) >>= fun (_:'t) -> (result Node<'mt,'t>.A ) : 'mit
            TypeT mit : TypeT<'mt>

    let inline unwrap (TypeT mit : TypeT<'mt>) =
        let _mnil  = (result Unchecked.defaultof<'t> : 'mt) >>= fun (_:'t) ->  (result Node<'mt,'t>.A ) : 'mit
        unbox mit : 'mit

    let inline empty () = wrap ((result Node<'mt,'t>.A) : 'mit) : TypeT<'mt>

    let inline concat l1 l2 =
            let rec loop (l1: TypeT<'mt>) (lst2: TypeT<'mt>) =
                let (l1, l2) = unwrap l1, unwrap lst2
                TypeT (l1 >>= function A ->  l2 | B (x: 't, xs) -> ((result (B (x, loop xs lst2))) : 'mit))
            loop l1 l2 : TypeT<'mt>


    let inline bind f (source: TypeT<'mt>) : TypeT<'mu> =
        // let _mnil = (result Unchecked.defaultof<'t> : 'mt) >>= fun (_: 't) -> (result Unchecked.defaultof<'u>) : 'mu
        let rec loop f input =
            TypeT (
                (unwrap input : 'mit) >>= function
                        | A -> result <| (A : Node<'mu,'u>) : 'miu
                        | B (h:'t, t: TypeT<'mt>) ->
                            let res = concat (f h: TypeT<'mu>) (loop f t)
                            unwrap res  : 'miu) 
        loop f source : TypeT<'mu>


    let inline map (f: 'T->'U) (x: '``Monad<'T>`` ) = Bind.Invoke x (f >> Return.Invoke) : '``Monad<'U>``


    let inline unfold (f:'State -> '``M<('T * 'State) option>``) (s:'State) : TypeT<'MT> =
            let rec loop f s = f s |> map (function
                    | Some (a, s) -> B (a, loop f s)
                    | None -> A) |> wrap
            loop f s

    let inline create (al: '``Monad<list<'T>>``) : TypeT<'``Monad<'T>``> =
            unfold (fun i -> map (fun (lst:list<_>) -> if lst.Length > i then Some (lst.[i], i+1) else None) al) 0

    let inline run (lst: TypeT<'MT>) : '``Monad<list<'T>>`` =
        let rec loop acc x = unwrap x >>= function
            | A         -> result (List.rev acc)
            | B (x, xs) -> loop (x::acc) xs
        loop [] lst

    let c0 = create (Id ([1..10]))
    let res0 = c0 |> run |> create |> run

// From https://github.com/dotnet/fsharp/issues/4171#issuecomment-528063764
// This case is where the type gets labelled as Sealed
module TypeInferenceChangeWithSealedType2 =
    [<Sealed>]
    type Id<'t> (v: 't) =
       let value = v
       member __.getValue = value

    [<RequireQualifiedAccess>]
    module Id =
        let run   (x: Id<_>) = x.getValue
        let map f (x: Id<_>) = Id (f x.getValue)
        let create x = Id x


    type Bind =
        static member        (>>=) (source: Lazy<'T>   , f: 'T -> Lazy<'U>    ) = lazy (f source.Value).Value                                   : Lazy<'U>
        static member        (>>=) (source: Task<'T>   , f: 'T -> Task<'U>    ) = source.ContinueWith(fun (x: Task<_>) -> f x.Result).Unwrap () : Task<'U>
        static member        (>>=) (source             , f: 'T -> _           ) = Option.bind   f source                                        : option<'U>
        static member        (>>=) (source             , f: 'T -> _           ) = async.Bind (source, f)  
        static member        (>>=) (source : Id<_>     , f: 'T -> _           ) = f source.getValue                                 : Id<'U>

        static member inline Invoke (source: '``Monad<'T>``) (binder: 'T -> '``Monad<'U>``) : '``Monad<'U>`` =
            let inline call (_mthd: 'M, input: 'I, _output: 'R, f) = ((^M or ^I or ^R) : (static member (>>=) : _*_ -> _) input, f)
            call (Unchecked.defaultof<Bind>, source, Unchecked.defaultof<'``Monad<'U>``>, binder)

    let inline (>>=) (x: '``Monad<'T>``) (f: 'T->'``Monad<'U>``) : '``Monad<'U>`` = Bind.Invoke x f

    type Return =
        static member inline Invoke (x: 'T) : '``Applicative<'T>`` =
            let inline call (mthd: ^M, output: ^R) = ((^M or ^R) : (static member Return : _*_ -> _) output, mthd)
            call (Unchecked.defaultof<Return>, Unchecked.defaultof<'``Applicative<'T>``>) x

        static member        Return (_: Lazy<'a>       , _: Return  ) = fun x -> Lazy<_>.CreateFromValue x : Lazy<'a>
        static member        Return (_: 'a Task        , _: Return  ) = fun x -> Task.FromResult x : 'a Task
        static member        Return (_: option<'a>     , _: Return  ) = fun x -> Some x                : option<'a>
        static member        Return (_: 'a Async       , _: Return  ) = fun (x: 'a) -> async.Return x
        static member        Return (_: 'a Id          , _: Return  ) = fun (x: 'a) -> Id x

    let inline result (x: 'T) : '``Functor<'T>`` = Return.Invoke x


    type TypeT<'``monad<'t>``> = TypeT of obj
    type Node<'``monad<'t>``,'t> = A | B of 't * TypeT<'``monad<'t>``>

    let inline wrap (mit: 'mit) =
            let _mnil  = (result Unchecked.defaultof<'t> : 'mt) >>= fun (_:'t) -> (result Node<'mt,'t>.A ) : 'mit
            TypeT mit : TypeT<'mt>

    let inline unwrap (TypeT mit : TypeT<'mt>) =
        let _mnil  = (result Unchecked.defaultof<'t> : 'mt) >>= fun (_:'t) ->  (result Node<'mt,'t>.A ) : 'mit
        unbox mit : 'mit

    let inline empty () = wrap ((result Node<'mt,'t>.A) : 'mit) : TypeT<'mt>

    let inline concat l1 l2 =
            let rec loop (l1: TypeT<'mt>) (lst2: TypeT<'mt>) =
                let (l1, l2) = unwrap l1, unwrap lst2
                TypeT (l1 >>= function A ->  l2 | B (x: 't, xs) -> ((result (B (x, loop xs lst2))) : 'mit))
            loop l1 l2 : TypeT<'mt>


    let inline bind f (source: TypeT<'mt>) : TypeT<'mu> =
        // let _mnil = (result Unchecked.defaultof<'t> : 'mt) >>= fun (_: 't) -> (result Unchecked.defaultof<'u>) : 'mu
        let rec loop f input =
            TypeT (
                (unwrap input : 'mit) >>= function
                        | A -> result <| (A : Node<'mu,'u>) : 'miu
                        | B (h:'t, t: TypeT<'mt>) ->
                            let res = concat (f h: TypeT<'mu>) (loop f t)
                            unwrap res  : 'miu) 
        loop f source : TypeT<'mu>


    let inline map (f: 'T->'U) (x: '``Monad<'T>`` ) = Bind.Invoke x (f >> Return.Invoke) : '``Monad<'U>``


    let inline unfold (f:'State -> '``M<('T * 'State) option>``) (s:'State) : TypeT<'MT> =
            let rec loop f s = f s |> map (function
                    | Some (a, s) -> B (a, loop f s)
                    | None -> A) |> wrap
            loop f s

    let inline create (al: '``Monad<list<'T>>``) : TypeT<'``Monad<'T>``> =
            unfold (fun i -> map (fun (lst:list<_>) -> if lst.Length > i then Some (lst.[i], i+1) else None) al) 0

    let inline run (lst: TypeT<'MT>) : '``Monad<list<'T>>`` =
        let rec loop acc x = unwrap x >>= function
            | A         -> result (List.rev acc)
            | B (x, xs) -> loop (x::acc) xs
        loop [] lst

    let c0 = create (Id ([1..10]))
    let res0 = c0 |> run |> create |> run


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

