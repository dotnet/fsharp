type Foo private () =
    static let mutable x = 0
    static member X
        with get () = x
        and set v = x <- v
    
module Exts =
    type Foo with
        static member X v = Foo.X <- v

open Exts
Foo.X(1)

if Foo.X <> 1 then
    System.Environment.Exit 1

module Exts2 =
    type Foo with
        static member X v = Foo.X <- v * 2
        
open Exts2

Foo.X 2

if Foo.X <> 4 then
    System.Environment.Exit 2
