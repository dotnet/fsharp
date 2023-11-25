type Foo() = 
    let mutable x = 0
    member _.X
        with get (i: int) = i + x
        and set(i: int) value = x <- i + value

module Exts =
    type Foo with
        member f.X (i: int, j) = f.X i <- j // note: the RFC excludes hiding indexed properties
open Exts
let f = Foo()

if f.X 0 <> 0 then
    System.Environment.Exit 1
f.X 1 <- 1
if f.X 0 <> 2 then
    System.Environment.Exit 2
;;
// This expression was expected to have type ...
f.X(1,2)