// #Conformance #SignatureFiles 
module Test

type G<'a when 'a : struct> = X
let h (x : 'a) : G<'a> = X
