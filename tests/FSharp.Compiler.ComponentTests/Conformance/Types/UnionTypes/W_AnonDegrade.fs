// Anonymous Union Types
// Anonymous union degenerates to single type

type A = int
type B = int
let x: (A|B) = 5
let y = x + 1