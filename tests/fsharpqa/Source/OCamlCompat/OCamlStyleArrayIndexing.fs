// #OCaml 
// Compile with --mlcompatibility --warnaserror ==> should compile clean and run clean
//<Expects status="success"></Expects>
let op_ArrayLookup (a : int []) (i : int) = 
    a.[i]
    
let op_ArrayAssign (a : int []) (i : int) (v : int) = 
    a.[i] <- v

let a = [|1;2;3;4;5|]

a.(1) <- 9 ;
a.(1) |> ignore
