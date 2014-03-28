// #Regression #NoMono #NoMT #CodeGen #EmittedIL #NETFX20Only #NETFX40Only 
// Regression for Dev11:12545, we were incorrectly inlining calls to LazyExtensions::Force instead of Lazy.Value from mscorlib on 4.0

open Microsoft.FSharp.Control.LazyExtensions

let x = lazy(1) 

x.Force() |> ignore
