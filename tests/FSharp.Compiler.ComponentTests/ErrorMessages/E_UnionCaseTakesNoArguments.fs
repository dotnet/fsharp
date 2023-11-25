type A = { X: int }

type B = | B of int

type C = | C

match None with
| None 1 -> ()

match None with
| None (1, 2) -> ()

match None with
| None [] -> ()

match None with
| None [||] -> ()

match None with
| None { X = 1 } -> ()

match None with
| None (B 1) -> ()

match None with
| None (x, y) -> ()

match None with
| None false -> ()

match None with
| None _ -> () // Wildcard pattern raises a warning in F# 8.0

match None with
| None x -> ()

match None with
| None (x, y) -> ()
| Some _ -> ()

match None with
| None x y -> ()
| Some _ -> ()

let c = C

match c with
| C _ _ -> ()

match c with
| C __ -> ()

let myDiscardedArgFunc(C _) = () // Wildcard pattern raises a warning in F# 8.0

let myDiscardedArgFunc2(C c) = ()

let myDiscardedArgFunc3(C __) = 5+5

let myDiscardedArgFunc(None x y) = None