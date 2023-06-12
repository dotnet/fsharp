module OrderMatters

let f<'a, 'b> (x: 'b) (y: 'a) = ()

type T() =
    member this.i<'a, 'b> (x: 'b) (y: 'a) = printfn "%A %A" x y

// compound types
let h1<'a, 'b> (x: 'b * 'a) = ()
let h2<'a, 'b> (x: 'b -> 'a) = ()
let h3<'a, 'b> (x: {| F1: 'b; F2: 'a|}) = ()
let h4<'a, 'b> (x: seq<'b> * array<int * 'a>) = ()

// Avoid duplicate names
let z<'a, 'z> (z1: 'z) (z2: 'z) (z3: 'a) : 'z = z1
