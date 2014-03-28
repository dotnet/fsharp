// #Regression #Conformance #DeclarationElements #Accessibility 
// Verify error when using a private implicit constructor
//<Expects id="FS0509" span="(24,10-24,18)" status="error">Method or object constructor 'C' not found</Expects>

type A public (x : int) =
    member this.Value = x


type B internal (x : int) =
    member this.Value = x

type C private (x : int) =
    member this.Value = x

// OK: Access public ctor
let t1 = new A(1)
if t1.Value <> 1 then exit 1

// OK: Access internal ctor
let t2 = new B(2)
if t2.Value <> 2 then exit 1

// ERROR: Access private ctor
let t3 = new C(3)
if t3.Value <> 3 then exit 1
