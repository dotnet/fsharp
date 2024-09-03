// #Conformance #TypesAndModules #Unions 
// Make sure we properly detect field names specified multiple times




type MyDU = 
    | Case1 of V1 : int * V2 : string

let x = Case1(V1 = 1, V1 = "")

let y = Case1(1, "")

match y with
| Case1(V2 = ""; V2 = "") -> ()
| _ -> ()

let (Case1(V1 = z; V1 = zz)) = y