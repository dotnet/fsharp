#r "../lib.dll"

module Foo =
    let Y = 22
    do
        printfn "%O" (Lib.X())