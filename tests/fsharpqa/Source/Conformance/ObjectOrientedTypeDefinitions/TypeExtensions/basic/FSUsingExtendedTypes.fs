// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions
// Regression for Dev11:51674
//<Expects status="success"></Expects>

open FSLib
open CSLib

try
    let foo = Foo()
    foo.Delay() // used to ICE here calling a C# defined extension for an F# defined type
    exit 0
with
    _ -> exit 1

