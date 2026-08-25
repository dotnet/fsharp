// Anonymous Union Types
// Hoist nullness from aliased nullable anon unions

type X = (int|string|null)
type Y = (int|string)
type Z = (int|bool)
let x: (float|X) = null
let y: (X|float) = null
let z: (X|float|null) = null
let a: (Y|null) = 1
let b: Y|null = "asd"
let c: ((Y)|null) = 2
