// #Regression #Conformance #TypeInference #ByRef 
module MyModule
open System

let mutable moduleVar = 42
let byrefVar = &moduleVar

// Code shouldn't compile
exit 1