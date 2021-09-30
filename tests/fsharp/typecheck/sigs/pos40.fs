module Pos40

module TestOverloadsWithSrtpThatDoResolve2 =

    type OverloadsWithSrtp() =
        static member inline SomeMethod< ^T when ^T : (member Length: int) > (x: ^T, f: ^T -> int) = 1
        static member inline SomeMethod(x: string, f: string -> int) = 2

    // Here, 'x' doesn't contain any type information so the overload doesn't resolve
    // The second overload is not generic so is preferred according to standard overloading rules.
    let inline f x = 
        OverloadsWithSrtp.SomeMethod (x, (fun a -> 1)) 

    // Here, 'x' does contain enough type information so the overload does resolve
    // The lambda argument 'a' gets type 'string'
    let f4 (x: string) = 
        OverloadsWithSrtp.SomeMethod (x, (fun a -> a.Length)) 

module TestOverloadsWithSrtpThatDoResolve3 =

    type OverloadsWithSrtp() =
        static member inline SomeMethod< ^T when ^T : (member Length: int) > (x: ^T, f: ^T -> int) = 1
        static member inline SomeMethod(x: string, f: string -> int) = 2

    // Here, 'x' does contain enough type information so the overload does resolve.
    // The lambda argument 'a' gets constrained type
    let inline f2< ^T when ^T : (member Length: int)> (x: ^T) = 
        OverloadsWithSrtp.SomeMethod (x, (fun a -> 1)) 

    // Here, 'x' does contain enough type information so the overload does resolve
    // The lambda argument 'a' gets constrained type
    let inline f3< ^T when ^T : (member Length: int) and ^T : (member Length2: int)> (x: ^T) = 
        OverloadsWithSrtp.SomeMethod (x, (fun a -> 2)) 

    // Here, 'x' does contain enough type information so the overload does resolve
    // The lambda argument 'a' gets type 'int'
    let f4 (x: int list) = 
        OverloadsWithSrtp.SomeMethod (x, (fun a -> a.Length))

printfn "test completed"
exit 0
