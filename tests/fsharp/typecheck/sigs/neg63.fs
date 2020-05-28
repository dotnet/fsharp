
module Test

let negTest1() =
    let mutable x = 0
    <@ x <- 1 @>


let negTest2() = 
    let mutable x = 0
    <@ &x @>

let posTest1(x:byref<int>) = 
    <@ x @>

let negTest3() = 
    let mutable x = 0
    &x

let posTest4(addr:byref<int>) = 
    &addr

let negTest5() = 
    let mutable x = 0
    let addr = &x
    &addr

[<Struct>]
type Struct(x:int) = 
    member __.AddrX = &x
