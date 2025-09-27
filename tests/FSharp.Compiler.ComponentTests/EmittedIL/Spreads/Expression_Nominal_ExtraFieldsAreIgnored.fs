type R1 = { A : int; B : int; C : int }
type R2 = { B : int }

let r1 = { A = 1; B = 2; C = 3 }
let r2 : R2 = { ...r1 }
