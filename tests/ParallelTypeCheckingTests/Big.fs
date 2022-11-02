module FSharp.Compiler.Service.Tests.Big

module A1 = let a = 3
module A2 = let a = 3
module A3 = let a = 3
module A4 =
    
    type AAttribute(name : string) =
        inherit System.Attribute()
    
    let a = 3
    module A1 =
        let a = 3
    type X = int * int
    type Y = Y of int

module B =
    open A2
    let b = [|
        A1.a
        A2.a
        A3.a
    |]
    let c : A4.X = 2,2
    [<A4.A("name")>]
    let d : A4.Y = A4.Y 2
    type Z =
        {
            X : A4.X
            Y : A4.Y
        }

let c = A4.a
let d = A4.A1.a
open A4
let e = A1.a
open A1
let f = a

//module X = B