type A() = class end
type B1() =
    inherit A()
    
let stuff(x: obj) =
    match x with
    | :? A -> 1
    | :? B1 -> 2
    | _ -> 3