
let inline (!!) (x: ^a * ^b) : ^c = ((^a or ^b or ^c): (static member op_Implicit: ^a * ^b -> ^c) x)
