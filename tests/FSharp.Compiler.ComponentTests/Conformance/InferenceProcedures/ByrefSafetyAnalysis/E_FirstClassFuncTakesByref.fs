// #Regression #Conformance #TypeInference #ByRef 


let f (x : byref<'a>) = ()
let z1 = f

let aref = ref 1
let _ = z1 aref


