// #Regression #Conformance #DeclarationElements #ObjectConstructors 
#light

// Test for alternate syntax for declaring generic types
// This notation comes from OCaml and issues an OCaml-compat warning

// 'a, 'b ClassName
// vs.
// ClassName<'a, 'b>

type ('a, 'b) StoreValues(a : 'a, b : 'b) = 
    member this.Value1 = a
    member this.Value2 = b

// (int, string) StoreValues(1024, "ch")
// vs.
// StoreValues<int, string>(1024, "ch")
let t = new (int, string) StoreValues(1024, "ch")
if t.Value1 <> 1024 then exit 1
if t.Value2 <> "ch" then exit 1

exit 0
