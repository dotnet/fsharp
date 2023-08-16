
module Test

[<Struct;CLIMutable>]
type StructRecord =
    {
        X: float
        mutable Y: float
    }

let x = { X = 1.; Y = 1. }

x.Y <- 5.

let pinIntNotAllowed() = 
    use p = fixed 3
    ()

let pinAnyNotAllowed(x: 'T) = 
    use p = fixed x
    ()


module Example1 = 
    type X<'T> = Y of 'T
    type X<'T when 'T :> string> with
        static member X = 2
        static member take (s: 'T) = s

module Example2 = 
    type X<'T> = Y of 'T
    type X<'U when 'T :> string> with
        static member X = 2
        static member take (s: 'T) = s
