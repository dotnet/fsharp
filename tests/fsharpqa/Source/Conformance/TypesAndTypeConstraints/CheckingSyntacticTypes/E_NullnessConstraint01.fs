// #Regression #Conformance #TypeConstraints 
// <Expects status="error" id="FS0001" span="(7,9-7,16)">The type 'int' does not have 'null' as a proper value</Expects>
// <Expects status="error" id="FS0001" span="(14,9-14,16)">The type 'StructRecd' does not have 'null' as a proper value</Expects>
// <Expects status="error" id="FS0001" span="(15,9-15,16)">The type 'StdRecd' does not have 'null' as a proper value</Expects>
let mkInput<'d when 'd:null>() () = ()

let r = mkInput<int>()

[<Struct>]
type StructRecd = { A : float }

type StdRecd = { B : float }

let _ = mkInput<StructRecd>()
let _ = mkInput<StdRecd>()

exit 1
