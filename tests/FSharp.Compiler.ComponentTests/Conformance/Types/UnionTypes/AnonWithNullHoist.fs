// Anonymous Union Types
// Hoist nullness from aliased nullable anon unions

type X=(int|string|null)
let x: (float|X) = null
let y: (X|float) = null
let z: (X|float|null) = null