
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

type Struct(initial:int) = 
    let mutable x = initial
    member __.AddrX = &x
    member __.IncrX() = x <- x + 1

let posTest6(iterator: byref<Struct>) = 
    iterator.AddrX

let negTest7() = 
    let mutable iterator = Struct(0)
    iterator.AddrX
