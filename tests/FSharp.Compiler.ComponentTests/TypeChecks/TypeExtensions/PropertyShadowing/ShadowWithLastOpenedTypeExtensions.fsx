#nowarn "52"
open System

type Foo private () =
    static let mutable x = 0
    static member X
        with get () = x
        and set v = x <- v
    
module Exts =
    type Foo with
        static member X v = Foo.X <- v

open Exts

let todo1 =
    async {
        Foo.X(1)
        if Foo.X <> 1 then
            return Error 1
        else
            return Ok ()
    }
    
match todo1 |> Async.RunSynchronously with
| Ok _ -> ()
| Error e -> System.Environment.Exit e

module Exts2 =
    type Foo with
        static member X v = Foo.X <- v * 2
        
open Exts2

let todo2 =
    async {
        Foo.X(1)
        if Foo.X <> 2 then
            return Error 2
        else
            return Ok ()
    }
    
match todo2 |> Async.RunSynchronously with
| Ok _ -> ()
| Error e -> System.Environment.Exit e
