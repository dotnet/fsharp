// #Conformance #TypesAndModules #Unions
// Make sure we properly detect when field names collide with member names
// Note: this only applies to single-case DUs
//<Expects id="FS0023" span="(12,17-12,22)" status="error">The member 'Item1' can not be defined because the name 'Item1' clashes with the generated property 'Item1' in this type or module</Expects>
//<Expects id="FS0023" span="(13,17-13,22)" status="error">The member 'Item2' can not be defined because the name 'Item2' clashes with the generated property 'Item2' in this type or module</Expects>
//<Expects id="FS0023" span="(14,17-14,19)" status="error">The member 'V3' can not be defined because the name 'V3' clashes with the generated property 'V3' in this type or module</Expects>
//<Expects id="FS0023" span="(19,17-19,21)" status="error">The member 'Item' can not be defined because the name 'Item' clashes with the generated property 'Item' in this type or module</Expects>

type MyDU =
    | Case1 of int * string * V3 : float
    with
    member this.Item1 = ""
    member this.Item2 = 3
    member this.V3 = 'x'

type MyDU2 = 
    | Case1 of int
    with
    member this.Item = ""