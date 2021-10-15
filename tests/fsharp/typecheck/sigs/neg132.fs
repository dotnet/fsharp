module Neg132

open System
open FSharp.Core.CompilerServices
module TestOverloadsWithSrtpThatDontResolve1 =

    type OverloadsWithSrtp() =
        [<NoEagerConstraintApplication>]
        static member inline SomeMethod< ^T when ^T : (member Length: int) > (x: ^T, f: ^T -> int) = 1
        static member  SomeMethod(x: 'T list, f: 'T list -> int) = 2

    // this will give a "requires version 6.0 or greater" error

    let inline f x = 
        OverloadsWithSrtp.SomeMethod (x, (fun a -> 1)) 
