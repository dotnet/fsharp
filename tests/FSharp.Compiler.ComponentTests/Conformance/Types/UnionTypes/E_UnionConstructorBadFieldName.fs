// #Conformance #TypesAndModules #Unions 
// Make sure we properly detect bogus named field in constructors




type MyDU = 
    | Case1 of V1 : int * V2 : string

let x = Case1(1, V3 = "")

let y = Case1(1, "")
match y with
| Case1(V3 = "") -> ()
| _ -> ()

let (Case1(V4 = z)) = y

let z a = Some ("", "", a = "")