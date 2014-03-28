// A simple class in a portable library that exposes
// a function that return a BigInt (which is no longer embedded in FSharp.Core)

module PL

type G() =
    member __.M() = 123I 
