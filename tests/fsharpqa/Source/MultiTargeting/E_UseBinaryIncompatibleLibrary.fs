// #Conformance #Regression #Multitargeting #Diagnostics
// Regression test for FSHARP1.0:5111
// <Expects status="error" id="FS0219">The referenced or default base CLI library 'mscorlib' is binary-incompatible with the referenced F# core library .*</Expects>

open System

let x = 1

exit 1