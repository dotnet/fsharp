module Neg131

open System
open FSharp.Core.CompilerServices
module TestOverloadsWithSrtpThatDontResolve1 =

    type OverloadsWithSrtp() =
        [<NoEagerConstraintApplication>]
        static member inline SomeMethod< ^T when ^T : (member Length: int) > (x: ^T, f: ^T -> int) = 1
        static member  SomeMethod(x: 'T list, f: 'T list -> int) = 2

    // 'x' doesn't contain any type information so the overload doesn't resolve.

    let inline f x = 
        OverloadsWithSrtp.SomeMethod (x, (fun a -> 1)) 
