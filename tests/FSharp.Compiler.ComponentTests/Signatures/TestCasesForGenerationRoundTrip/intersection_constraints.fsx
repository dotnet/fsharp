module Foo

open System
open System.Threading.Tasks

type I =
    abstract g: unit -> unit
    abstract h: #IDisposable & #seq<int> -> unit

let x (f: 't & #I) = ()

let y (f: 't & #I & #IDisposable, name: string) = ()

let z (f: #I & #IDisposable & #Task<int> & #seq<string>, name: string) = ()

type C<'t & #seq<int> & #IDisposable, 'y & #seq<'t>> = class end
