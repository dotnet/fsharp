// #Regression #NoMono #NoMT #CodeGen #EmittedIL 
// Regression for FSHARP1.0:5782 (Methods with MethodImplAttribute(MethodImplOptions.NoInlining) should not be inlined)
// compile with optimizations turned on: this will force an attempt to inline g()
open System.Runtime.CompilerServices

[<MethodImpl(MethodImplOptions.NoInlining)>]
let inline g() = printfn "Hey!"

let inline f() = g()

f()
