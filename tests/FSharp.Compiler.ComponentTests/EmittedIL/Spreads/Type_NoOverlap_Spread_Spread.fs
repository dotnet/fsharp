type R1 = { A : int; B : int }
type R2 = { C : int; D : int }
type R3 = { ...R1; ...R2 }
type R4 = { ...R2; ...R1 }
