// #Conformance #Quotations 
// Verify bad casts throw at runtime and valid ones succeed
open System
open Microsoft.FSharp.Quotations

let r1 =
    try
        let uq = <@@ let x = 1 in x @@>
        let tq : Expr<obj> = Expr.Cast uq
        1
    with
        | :? ArgumentException -> 0

let r2 =
    try
        let uq2= <@@ let x = new ArgumentException() in x @@>
        let tq2 : Expr<Exception> = Expr.Cast uq2
        1
    with
        | :? ArgumentException -> 0
        
let r3 =
    try
        let uq = <@@ let x = 1 in x @@>
        let tq : Expr<int> = Expr.Cast uq
        0
    with
        | :? ArgumentException -> 1
        
exit <| if r1 = 0 && r2 = 0 && r3 = 0 then 0 else 1
