// #Conformance #ConstraintCall

let inline h x = (^a: (static member M : ^a -> ^b) (x))
type T() = 
    static member M(_ : T) = 1
    static member M(_ : int) = ""
    
let _ : int = h (T())
