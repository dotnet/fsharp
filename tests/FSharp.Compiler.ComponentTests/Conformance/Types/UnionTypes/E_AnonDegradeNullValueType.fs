// Anonymous Union Types
// Anon union requires reference common ancestor type to be annotated with "| null" + int cases include each other

[<Measure>] type kg
let x: (int|int<kg>|null) = 42