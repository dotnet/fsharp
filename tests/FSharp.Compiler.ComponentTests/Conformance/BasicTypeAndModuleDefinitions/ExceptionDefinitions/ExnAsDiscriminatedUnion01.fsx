// #Conformance #TypesAndModules #Exceptions 
// Exception definition define new discriminated union cases
// Verify that we can use misc types (notice that the "sig-spec" cannot be used [covered in another testcase]
//<Expects status="success"></Expects>
#light

exception E1
exception E2 of int
exception E3 of int * int
exception E4 of (int * int) 
[<NoEquality; NoComparison>]
exception E5 of (int -> int)
[<NoEquality; NoComparison>]
exception E6 of (int -> int) * (int -> int)
exception E7 of (int * int) * int

let e1 = E1
let e2 = E2(10)
let e3 = E3(11,22)
let e4 = E4((1,2))
let e5 = E5(fun x -> x + 1)
let e6 = E6( (fun x -> x + 1), (fun x -> x - 1) )
let e7 = E7( (1,2), 3 ) 

let m e = match e with
                | E1        -> true
                | E2(x)     -> x=10
                | E3(x,y)   -> x=11 && y = 22
                | E4(x)     -> x = (1,2)
                | E5(f)     -> (f 1) = 2
                | E6(f,g)   -> (10 |> f |> g) = 10
                | E7(x,y)   -> x = (1,2) && y = 3
                | _         -> false

if not((m e1) && (m e2) && (m e3) && (m e4) && (m e5) && (m e6) && (m e7)) then failwith "Failed: 1"
