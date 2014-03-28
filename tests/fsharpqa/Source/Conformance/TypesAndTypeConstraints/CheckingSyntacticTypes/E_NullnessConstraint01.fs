// #Regression #Conformance #TypeConstraints 
// <Expects status="error" id="FS0001" span="(5,9-5,16)">The type 'int' does not have 'null' as a proper value</Expects>
let mkInput<'d when 'd:null>() () = ()

let r = mkInput<int>()

exit 1
