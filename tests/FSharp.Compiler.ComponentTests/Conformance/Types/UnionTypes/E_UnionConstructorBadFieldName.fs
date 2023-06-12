// #Conformance #TypesAndModules #Unions 
// Make sure we properly detect bogus named field in constructors
//<Expects id="FS3174" span="(10,18-10,20)" status="error">The union case 'Case1' does not have a field named 'V3'\.</Expects>
//<Expects id="FS3174" span="(14,9-14,11)" status="error">The union case 'Case1' does not have a field named 'V3'\.</Expects>
//<Expects id="FS3174" span="(17,12-17,14)" status="error">The union case 'Case1' does not have a field named 'V4'\.</Expects>

type MyDU = 
    | Case1 of V1 : int * V2 : string

let x = Case1(1, V3 = "")

let y = Case1(1, "")
match y with
| Case1(V3 = "") -> ()
| _ -> ()

let (Case1(V4 = z)) = y

let z a = Some ("", "", a = "")