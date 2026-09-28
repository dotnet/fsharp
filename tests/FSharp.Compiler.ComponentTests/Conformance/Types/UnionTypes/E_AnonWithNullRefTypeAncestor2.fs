// Anonymous Union Types
// Anon union requires reference common ancestor type to be annotated with "| null"

type X = (bool|int)
let x: (X|null) = 42
