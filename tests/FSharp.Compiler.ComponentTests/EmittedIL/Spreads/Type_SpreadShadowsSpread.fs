type R1 = { A : int; B : int }
type R2 = { A : string }
type R3 = { ...R1; ...R2 }
type R4 = { ...R2; ...R1 }
