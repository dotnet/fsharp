// RFC FS-1043: two sibling scopes each open a DIFFERENT same-signature extension operator on the built-in
// '*' symbol and apply it at identical concrete types. Both decisions must survive the inline/optimizer
// boundary simultaneously: M1 keeps A (repeat), M2 keeps B (append-count). The concrete-type sink key is
// identical for both sites, so scope is disambiguated by source range; before that, both sites collided on
// one poisoned key and fell back to FSharp.Core's throwing dynamic '*' stub (NotSupportedException at run).

module DuplicateBuiltinOperatorDistinctScopes

module A =
    type System.String with
        static member ( * ) (s: string, n: int) : string = System.String.Concat(Array.replicate n s)  // repeat

module B =
    type System.String with
        static member ( * ) (s: string, n: int) : string = s + string n  // append-count

module M1 =
    open A
    let f () : string = "ha" * 2

module M2 =
    open B
    let g () : string = "ha" * 2

let a = M1.f ()
let b = M2.g ()
if a <> "haha" then failwith $"M1 (open A, repeat) expected 'haha', got '{a}'"
if b <> "ha2" then failwith $"M2 (open B, append-count) expected 'ha2', got '{b}'"
