module SpreadInlineLib
// Library compiled to its own assembly. The inline body below is serialized
// into the assembly's pickled TypedTree and re-elaborated at the caller's
// site in another assembly (Spreading_v1.fsx), exercising the spread
// elaboration across the TypedTreePickle boundary.
type Lbl = { A : int; B : int }
let inline bump (x: Lbl) = { ...x; A = 99 }
