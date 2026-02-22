// #Conformance #TypeConstraints

let inline g< ^t, ^u, ^v when (^t or ^u or ^v) : (static member M : string)>() = 0

type T() = 
    static member M = ""

let _ = g<T, int, string>()
let _ = g<int, T, string>()
let _ = g<int, string, T>()
let _ = g<T, string, T>()