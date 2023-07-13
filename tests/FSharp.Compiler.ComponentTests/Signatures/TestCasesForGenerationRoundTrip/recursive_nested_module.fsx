module rec Top.Square.Middle.Level

let value = 10

type Foo =
    { Bar: Bar }
    
type Bar = { Meh: string }

module Nested =
       let x = ""