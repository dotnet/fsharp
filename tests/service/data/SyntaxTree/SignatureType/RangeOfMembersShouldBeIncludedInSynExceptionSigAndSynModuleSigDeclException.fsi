
module internal FSharp.Compiler.ParseHelpers

exception SyntaxError of obj * range: range with
    member Meh : string -> int

open Foo
