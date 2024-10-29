seq { 1..10 }

seq { 1..5..10 }

[| seq { 1..10 } |]

[| seq { 1..5..10 } |]

let a = seq { 1..10 }

let a3 = seq { 1..10..20 }

let b = [| seq { 1..10 } |]

let b3 = [| seq { 1..10..20 } |]

let c = [ seq { 1..10 } ]

[| seq { 1..10 } |]

[| yield seq { 1..10 } |]

[ seq { 1..10 } ]

[ seq { 1..10..10 } ]

[ yield seq { 1..10 } ]

[ yield seq { 1..10..20 } ]

ResizeArray(seq { 1..10 })

ResizeArray(seq { 1..10..20 })

let fw start finish = [ for x in seq { start..finish } -> x ]

let fe start finish = [| for x in seq { start..finish } -> x |]

for x in seq { 1..10 }  do ()

for x in seq { 1..5..10 } do ()
    
let f = Seq.head

let a2 = f (seq { 1..6 })

let a23 = f (seq { 1..6..10 })

let b2 = set (seq { 1..6 })

let f10 start finish = for x in seq { start..finish } do ignore (float x ** float x)

let (..) _ _ = "lol"

let lol1 = seq { 1..10 }

seq { 1..5..10 }

let resultInt = Seq.length (seq {1..8})

let resultInt2 funcInt  = Seq.map3 funcInt (seq { 1..8 }) (seq { 2..9 }) (seq { 3..10 })

let verify c = failwith "not implemented"

Seq.splitInto 4 (seq {1..5}) |> verify (seq { 1.. 10 })

seq [ seq {1..4}; seq {5..7}; seq {8..10} ]

Seq.allPairs (seq { 1..7 }) Seq.empty

Seq.allPairs Seq.empty (seq { 1..7 })

let intArr1 = [| yield! seq {1..100}
                 yield! seq {1..100} |]

Array.ofSeq (seq {1..10})