// #Regression #Conformance #TypesAndModules #Unions 
// Union types NO LONGER implement IStructuralHash
// They used to, but now they don't anymore.
// 
//<Expects id="FS0039" span="(17,15)" status="error">The type 'IStructuralHash' is not defined</Expects>
//<Expects id="FS0072" span="(18,1)"  status="error">Lookup on object of indeterminate type based on information prior to this program point\. A type annotation may be needed prior to this program point to constrain the type of the object\. This may allow the lookup to be resolved</Expects>
//<Expects id="FS0039" span="(21,15)" status="error">The type 'IStructuralHash' is not defined</Expects>
//<Expects id="FS0072" span="(22,1)"  status="error">Lookup on object of indeterminate type based on information prior to this program point\. A type annotation may be needed prior to this program point to constrain the type of the object\. This may allow the lookup to be resolved</Expects>
 
type I = | A
         | B

let p = A
let q = B

let mutable ix = 18
let ip = p :> IStructuralHash              // err: The type 'IStructuralHash' is not defined.
ip.GetStructuralHashCode(&ix)

let mutable iy = 18
let iq = q :> IStructuralHash              // err: The type 'IStructuralHash' is not defined.
iq.GetStructuralHashCode(&iy)
