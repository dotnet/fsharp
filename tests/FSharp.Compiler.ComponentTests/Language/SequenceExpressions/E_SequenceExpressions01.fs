{ 1..10 }

{ 1..5..10 }

[| { 1..10 } |]

[| { 1..5..10 } |]

let a = { 1..10 }

let a3 = { 1..10..20 }

let b = [| { 1..10 } |]

let b3 = [| { 1..10..20 } |]

let c = [ { 1..10 } ]

[| { 1..10 } |]

[| yield { 1..10 } |]

[ { 1..10 } ]

[ { 1..10..10 } ]

[ yield { 1..10 } ]

[ yield { 1..10..20 } ]

ResizeArray({ 1..10 })

ResizeArray({ 1..10..20 })

let fw start finish = [ for x in { start..finish } -> x ]

let fe start finish = [| for x in { start..finish } -> x |]

for x in { 1..10 }  do ()

for x in { 1..5..10 } do ()
    
let f = Seq.head

let a2 = f { 1..6 }

let a23 = f { 1..6..10 }

let b2 = set { 1..6 }

let f10 start finish = for x in { start..finish } do ignore (float x ** float x)

let (..) _ _ = "lol"

let lol1 = { 1..10 }

{ 1..5..10 }

let resultInt = Seq.length {1..8}

let resultInt2 funcInt  = Seq.map3 funcInt { 1..8 } { 2..9 } { 3..10 }

let verify c = failwith "not implemented"

Seq.splitInto 4 {1..5} |> verify { 1.. 10 }

seq [ {1..4}; {5..7}; {8..10} ]

Seq.allPairs { 1..7 } Seq.empty

Seq.allPairs Seq.empty { 1..7 }

let intArr1 = [| yield! {1..100}
                 yield! {1..100} |]

Array.ofSeq {1..10}