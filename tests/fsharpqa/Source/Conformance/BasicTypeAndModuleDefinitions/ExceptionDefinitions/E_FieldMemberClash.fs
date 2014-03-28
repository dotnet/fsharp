// #Conformance #TypesAndModules #Exception
// Make sure we properly detect when field names collide with member names
//<Expects id="FS0023" span="(9,17-9,22)" status="error">The member 'Data0' can not be defined because the name 'Data0' clashes with the field 'Data0' in this type or module</Expects>
//<Expects id="FS0023" span="(10,17-10,22)" status="error">The member 'Data1' can not be defined because the name 'Data1' clashes with the field 'Data1' in this type or module</Expects>
//<Expects id="FS0023" span="(11,17-11,19)" status="error">The member 'V3' can not be defined because the name 'V3' clashes with the field 'V3' in this type or module</Expects>

exception AAA of int * string * V3 : float
    with
    member this.Data0 = ""
    member this.Data1 = 3
    member this.V3 = 'x'