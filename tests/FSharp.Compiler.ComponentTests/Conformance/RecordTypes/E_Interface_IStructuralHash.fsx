// #Regression #Conformance #TypesAndModules #Records 
// Record types NO LONGER implement IStructuralHash
// We used to, but now we do not anymore.
// 

//<Expects id="FS0039" span="(20,15)" status="error">'IStructuralHash'</Expects>


type r = { A : int; B : bool }

let p = { A = 10; B = true }

let q = { B = false; A = 11 }

let mutable ix = 18
let ip = p :> IStructuralHash               // err: The type 'IStructuralHash' is not defined.
ip.GetStructuralHashCode(&ix) |> ignore

let mutable iy = 18
let iq = q :> IStructuralHash               // err: The type 'IStructuralHash' is not defined.
iq.GetStructuralHashCode(&iy) |> ignore
