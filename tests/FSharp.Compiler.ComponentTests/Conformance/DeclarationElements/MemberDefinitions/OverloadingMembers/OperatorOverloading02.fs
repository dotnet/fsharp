// #Regression #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// Regression test for FSHARP1.0:4730 - Overly restrictive - functions not allowed as operator overloads

#light

type public TestType() =
    member public s.Value with get() = 17

    static member public (+++) (a : TestType, b : TestType) = a.Value + b.Value

    static member public (+++) (a : TestType, b : int) = a.Value + b

    static member public (+++) (a : int, b : TestType) = a + b.Value

    static member public (+++) (a : TestType, b : int -> int) = (b 17) + a.Value

    static member public (+++) (a : int -> int, b : TestType) = (a 17) + b.Value
    
    static member public (+++) (a : obj * string -> int, b : TestType) = (a (box 17, "17")) + b.Value

let inline (+++) (a : ^a) (b : ^b) = ((^a or ^b): (static member (+++): ^a * ^b -> ^c) (a,b) )

let tt0 = TestType()
let tt1 = TestType()

let f (x : int) = 18
let g (x, y) = 18

let a0 = tt0 +++ tt1 + 1
let a1 = tt0 +++ 18
let a2 = 18 +++ tt1
let a3 = tt0 +++ f
let a4 = f +++ tt0
let a5 = TestType.(+++)(f, tt0)
let a6 = tt0 +++ (fun x -> 18)
let a7 = g +++ tt1

let res = [a0; a1; a2; a3; a4; a5; a6; a7] |> Set.ofList |> Set.filter (fun a -> a <> 35)
if res <> Set.empty then failwith "Failed: 1"
