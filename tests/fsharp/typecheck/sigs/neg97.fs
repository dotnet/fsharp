
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

let pinStackAddressNotAllowed(x: 'T) = 
    let mutable v = 0
    use p = fixed &v
    ()

let pinStructAddressNotAllowed(x: 'T) = 
    let mutable v = { X = 1.0; Y = 1.0 }
    use p = fixed &v.Y
    ()
