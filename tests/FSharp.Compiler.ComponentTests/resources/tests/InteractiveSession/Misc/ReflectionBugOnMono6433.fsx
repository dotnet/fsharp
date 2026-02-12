// Regression test for FSHARP1.0:6433
// This is really a bug in Mono
//<Expects status="success">val mul: MM</Expects>
//<Expects status="success">val factorial: x: int -> int</Expects>
//<Expects status="success">val k: int = 120</Expects>

type MM() = 
 member x.Combine(a,b) = a * b
 member x.Yield(a) = a
 member x.Zero() = 1
 member x.For(e,f) = Seq.fold (fun s n -> x.Combine(s, f n)) (x.Zero()) e

let mul = new MM()

let factorial x = mul { for x in 1 .. x do yield x }

let k = factorial 5

printfn "%A" k

#q;;