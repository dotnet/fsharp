// Anonymous Union Types
// null on the last position, nested

type X = (int|string)
let x: (X|null) = 42
