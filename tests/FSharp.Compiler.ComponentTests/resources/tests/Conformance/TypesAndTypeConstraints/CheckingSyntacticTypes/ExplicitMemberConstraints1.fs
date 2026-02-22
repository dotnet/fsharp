// #Conformance #TypeConstraints

let inline f< ^t when ^t : (static member M : string)>() = 0
type T() = 
    static member M = ""

let _ = f<T>()