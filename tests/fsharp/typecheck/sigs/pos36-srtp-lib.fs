

module Lib

let inline RequireM< ^Witnesses, ^T when (^Witnesses or ^T): (static member M : ^T -> string) > (x: ^T) : string = 
    ((^Witnesses or ^T): (static member M : ^T -> string) x)

type C(p:int) = 
    member x.P = p

type Witnesses() =

    static member M (x: C) : string = sprintf "M(C), x = %d"  x.P

    static member M (x: int64) : string = sprintf "M(int64), x = %d"  x

type StaticMethods =

    static member inline M< ^T when (Witnesses or  ^T): (static member M: ^T -> string)>  (x: ^T) : string =

        RequireM< Witnesses, ^T> (x)

