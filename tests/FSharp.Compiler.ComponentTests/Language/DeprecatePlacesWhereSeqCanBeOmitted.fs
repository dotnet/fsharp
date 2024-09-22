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