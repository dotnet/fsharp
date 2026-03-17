// #Conformance #ConstraintCall
let inline h x y z = ((^a or ^b or ^c) : (static member M : ^a * ^b * ^c -> ^d) (x,y,z))
type T() = 
    static member M(_ : T, _ : int, _ : int) = 1
    static member M(_ : int, _ : T, _ : int) = ""
    static member M(_ : int, _ : int, _ : T) = false
    
let _ : int = h (T()) 1 1   
let _ : string = h 1 (T()) 1
let _ : bool = h 1 1 (T())