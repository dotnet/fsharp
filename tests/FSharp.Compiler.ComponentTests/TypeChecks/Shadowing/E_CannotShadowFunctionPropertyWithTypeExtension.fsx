open System.Runtime.CompilerServices

type Foo() = 
    member val X : int -> int = (+) 1 with get,set

module Exts =
    type Foo with
        member f.X (i: int -> int, j) = f.X i <- j // note: the RFC excludes hiding function typed properties
open Exts
let f = Foo()
f.X 1
if f.X 0 <> 1 then
    System.Environment.Exit 1
f.X <- (-) 1

if f.X 2 <> -1 then
    System.Environment.Exit 2

f.X((-) 1) // This expression was expected to have type 'int' but here has type ''a -> 'c'