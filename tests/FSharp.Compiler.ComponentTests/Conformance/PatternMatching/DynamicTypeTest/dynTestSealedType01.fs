// #Conformance #PatternMatching #TypeTests 
#light

// Verify result of dynamic type test on sealed type.

type Foo() =
    member this.FooValue = 42

[<Sealed>]
type Bar() =
    inherit Foo()
    member this.BarValue = 42


let test (x : obj) =
    match x with
    | :? Bar -> 2
    | :? Foo -> 1
    | _      -> 3

let foo = new Foo()
let bar = new Bar()

if test (foo :> obj) <> 1 then exit 1
if test (bar :> obj)  <> 2 then exit 1

exit 0
