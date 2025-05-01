module Meh

type Foo() =
    member inline this.Item
        with get (i:int,j: char) : string = ""
        and set (i:int,j: char) (x:string) = printfn "%i %c" i j
