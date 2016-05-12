// #Regression #Conformance #TypeInference #ByRef 
//<Expects id="FS0001" span="(8,12-8,16)" status="error">This expression was expected to have type.    'byref<'a>'    .but here has type.    'int ref'</Expects>

let f (x : byref<'a>) = ()
let z1 = f

let aref = ref 1
let _ = z1 aref


