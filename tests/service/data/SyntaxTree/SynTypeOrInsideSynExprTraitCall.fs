
let inline (!!) (x: ^a) : ^b = ((^a or ^b): (static member op_Implicit: ^a -> ^b) x)
