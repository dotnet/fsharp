module Pos40

open FSharp.Core.CompilerServices

// Test a number of cases relating to https://github.com/dotnet/fsharp/pull/12202/files
module TestOverloadsWithSrtpThatDoResolve1 =

    type OverloadsWithSrtp() =
        // Note: no attribute
        //[<NoEagerConstraintApplication>]
        static member inline SomeMethod< ^T when ^T : (member Foo: int) > (x: ^T, f: ^T -> int) = 1
        static member SomeMethod(x: string, f: string -> int) = 2

    // Here, 'x' doesn't contain any type information. However the presence of an SRTP-constrained
    // method causes the Foo constraint to be applied to the caller argument type and the
    // method is resolved to the first overload.
    let inline f x = 
        OverloadsWithSrtp.SomeMethod (x, (fun a -> 1)) 

    type C() =
       member x.Foo = 3
       
    let v = f (C()) // this should now resolve 

    // Here, 'x' contains enough type informationto resolve the overload.
    // Lambda propagation applies and the lambda argument 'a' gets known type 'string'.
    let f4 (x: string) = 
        OverloadsWithSrtp.SomeMethod (x, (fun a -> a.Length)) 

module TestOverloadsWithSrtpThatDoResolve2 =

    type OverloadsWithSrtp() =
        [<NoEagerConstraintApplication>]
        static member inline SomeMethod< ^T when ^T : (member Foo: int) > (x: ^T, f: ^T -> int) = 1
        static member SomeMethod(x: string, f: string -> int) = 2

    // Here, 'x' doesn't contain any type information. The presence of an SRTP-constrained
    // method does not causes the Foo constraint to be applied to the caller argument type because NoEagerConstraintApplication is present.
    //
    // Overload resolution proceeds. The second overload is not generic so is preferred according to standard overloading rules.
    let f x = 
        OverloadsWithSrtp.SomeMethod (x, (fun a -> 1)) 

    let v = f "hello" // this should now resolve since 'x' was inferred to have type 'string'

    // Here, 'x' contains enough type informationto resolve the overload.
    // Lambda propagation applies and the lambda argument 'a' gets known type 'string'.
    let f4 (x: string) = 
        OverloadsWithSrtp.SomeMethod (x, (fun a -> a.Length)) 

module TestOverloadsWithSrtpThatDoResolve3 =

    type OverloadsWithSrtp() =
        [<NoEagerConstraintApplication>]
        static member inline SomeMethod< ^T when ^T : (member Length: int) > (x: ^T, f: ^T -> int) = 1
        static member SomeMethod(x: string, f: string -> int) = 2

    // Here, 'x' contains enough type informationto resolve the overload.
    // The lambda argument 'a' gets constrained type
    let inline f2< ^T when ^T : (member Length: int)> (x: ^T) = 
        OverloadsWithSrtp.SomeMethod (x, (fun a -> 1)) 

    let v1 = f2 [1]
    let v2 = f2 [| 1 |]

    // Here, 'x' contains enough type informationto resolve the overload
    // The lambda argument 'a' gets constrained type
    let inline f3< ^T when ^T : (member Length: int) and ^T : (member Length2: int)> (x: ^T) = 
        OverloadsWithSrtp.SomeMethod (x, (fun a -> 2)) 

    // Here, 'x' contains enough type informationto resolve the overload
    // The lambda argument 'a' gets type 'int'
    let f4 (x: int list) = 
        OverloadsWithSrtp.SomeMethod (x, (fun a -> a.Length))

    let v4 = f4 [ 1 ]

printfn "test completed"
exit 0
