// Anonymous Union Types
// Both types are int, but one of them has a unit of measure
type [<Measure>] kg

let x: (int | int<kg>) = 42