type R1 = { A : int; B : int }
type R2 = { A : int; B : int; C : int }

let r1 = { A = 1; B = 2 }
let r2 = { ...r1; C = 3 }

let r1' = {| A = 1; B = 2 |}
let r2' = { ...r1; C = 3 }
