module Pos38

type Expression =
    | EndOp
    | BinaryOp of Expression * Expression

let mutable count = 0

[<AutoOpen>]
module Extensions =

    type Expression with

        member this.Foobar2 : unit =
            match this with
            | BinaryOp (_, e2) ->
                e2.Foobar2
            | EndOp -> 
                count <- count + 1
                ()


let c = BinaryOp(EndOp, EndOp)

c.Foobar2

if count <> 1 then failwith "incorrect count"

