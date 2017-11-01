

module M

let ffff ok = ok + 3
let gggg1 (qqqq : int) = qqqq + 3
let gggg2 (qqqq : int) = qqqq + 3
let gggg3 (qqqq : int) = qqqq + 3
let gggg4 (qqqq : int) = qqqq + 3

type C( qqqq2: int) = 
  member __.M1 (qqqq3: int) = qqqq2 + qqqq3 |> ignore
  member __.M2 (qqqq3: int) = qqqq2 + qqqq3 |> ignore
  member __.M3 (qqqq3: int) = qqqq2 + qqqq3 |> ignore
  member __.M4 (qqqq3: int) = qqqq2 + qqqq3 |> ignore

