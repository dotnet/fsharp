// Anonymous Union Types
// Duplicate cases hidden behind aliases are allowed. Nesting aliased anon unions is allowed.

type U = (int|string)
let x: (U|int) = 42
