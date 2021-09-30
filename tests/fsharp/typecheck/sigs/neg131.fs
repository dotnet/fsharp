module Neg131

open System

module TestOverloadsWithSrtpThatDontResolve1 =

    type OverloadsWithSrtp() =
        static member inline SomeMethod< ^T when ^T : (member Length: int) > (x: ^T, f: ^T -> int) = 1
        static member inline SomeMethod(x: 'T list, f: 'T list -> int) = 2

    // Here, 'x' doesn't contain any type information so the overload doesn't resolve
    // The second overload is generic so is not preferred
    let inline f x = 
        OverloadsWithSrtp.SomeMethod (x, (fun a -> 1)) 
