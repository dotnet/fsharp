// Anonymous Union Types
// Anon union requires reference common ancestor type to be annotated with "| null"s

type X = (bool|int)
let x: (X|null) = 42
