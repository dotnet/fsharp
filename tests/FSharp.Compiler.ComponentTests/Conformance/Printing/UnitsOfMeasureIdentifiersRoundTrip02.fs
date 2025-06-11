// #Regression #NoMT #Printing 
// Regression test for FSHARP1.0:3300
// Verify that Pretty-printing of measure identifiers round-trips, i.e. displays the long identified (Namespace.Module.Type)
// This is an aux dll which defines UoM inside a namespace/module
#light
namespace A.B.C

module M1 =
  [<Measure>] type Kg

module M2 =
  [<Measure>] type Kg


