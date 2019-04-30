// #Regression #Conformance #SyntacticSugar 
#light

// Verify errors related to ambiguous slicing overloads

type Foo<'a>() =
    member this.GetSlice(x : int, y1 : int option, y2 : int option) = ()
    member this.GetSlice(x : int, y1 : int option, y2 : float option) = ()
    member this.GetSlice(x1 : float option, x2 : int option, y : int) = ()
    member this.GetSlice(x1 : int option,   x2 : int option, y : int) = ()


let f = new Foo<char>()

let _ = f.[2, 1..]
let _ = f.[2, *]
let _ = f.[..1, 2]
let _ = f.[*, 2]
let _ = f.[3, 4]
let _ = f.[1, *, Some(5)]
let _ = f.[1, *, *]

exit 1