module Attributes

open System

type XAttribute() =
    inherit Attribute()

let a ([<X>] b: int) : int = 0

type YAttribute(y: string) =
    inherit Attribute()

let a2 ([<X; Y "meh">] g: string) ([<Y "buzz">] h) ([<X; Y "ii">] i: int) =
    printfn "h is %i" h
    g

type Boo() =
    member _.Do ([<X; Y "meh">] g: string) ([<Y "buzz">] h) =
        printfn "h is %i" h
        g

    member _.Maybe([<X>] ?s: int) = ()
    member _.Forever([<X>][<Y "zzz">][<ParamArray>] args: string array) = ()
