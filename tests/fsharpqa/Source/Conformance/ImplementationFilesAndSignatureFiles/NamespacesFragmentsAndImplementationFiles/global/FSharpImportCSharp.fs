// #Regression #Conformance #SignatureFiles #Namespaces #ReqNOMT 
// Regression test for FSHARP1.0:4932

// This is really the case behind the implementation
// of the 'global' keyword in F#. I.e. being able to
// access types hidden by some other definitions.

// My C type 'hides' the one imported from the C# DLL
type C() = member y.M(f : float) = f + 1.

// We can't get to the C# C...
let c = new C()
c.M(1.) |> ignore

// ... unless we use the global keyword/namespace
(if global.C.M() = 1 then 0 else 1) |> ignore
