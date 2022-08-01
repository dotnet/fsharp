// SDK version 7.0.100-preview.6 or newer has to be installed for this to work
open System.Numerics

[<Measure>] type potato

let f (x: 'T when IMultiplyOperators<'T,'T,'T>) = x 

// f 7.0<potato> 

printfn $"Hello from F# {f 7.0}!"
