// Anonymous Union Types
// Anonymous union degenerates to single type

type A = string
type B = string
let x: (A|B|null) = "asd"
let y = x