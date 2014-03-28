
module Test

let negTest1() = 
    let mutable x = 0
    <@ x <- 1 @>


let negTest2() = 
    let mutable x = 0
    <@ &x @>

let posTest1(x:byref<int>) = 
    <@ x @>

