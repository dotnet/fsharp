// #Conformance #TypesAndModules #Unions 
// Make sure we properly detect field names specified multiple times
//<Expects id="FS3175" span="(10,23-10,25)" status="error">Union case/exception field 'V1' cannot be used more than once\.</Expects>
//<Expects id="FS3175" span="(15,18-15,20)" status="error">Union case/exception field 'V2' cannot be used more than once\.</Expects>
//<Expects id="FS3175" span="(18,20-18,22)" status="error">Union case/exception field 'V1' cannot be used more than once\.</Expects>

type MyDU = 
    | Case1 of V1 : int * V2 : string

let x = Case1(V1 = 1, V1 = "")

let y = Case1(1, "")

match y with
| Case1(V2 = ""; V2 = "") -> ()
| _ -> ()

let (Case1(V1 = z; V1 = zz)) = y