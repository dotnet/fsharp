module Pos1281

type Cond = Foo | Bar | Baz
let (|SetV|) x _ = x

let c = Cond.Foo

match c with
| Baz -> 
    printfn "Baz"
| Foo & SetV "and" kwd
| Bar & SetV "or" kwd ->
    printfn "Keyword: %s" kwd
| Baz -> failwith "wat"

printfn "test completed"
exit 0
