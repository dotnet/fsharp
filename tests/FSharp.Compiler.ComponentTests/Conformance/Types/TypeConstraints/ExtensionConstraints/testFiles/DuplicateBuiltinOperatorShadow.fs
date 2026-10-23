// RFC FS-1043: two same-signature extension operators on the built-in '*' symbol are BOTH opened.
// Most-recently-opened must win at runtime, matching type-checking's lexical-scope decision.

module DuplicateBuiltinOperatorShadow

module A =
    type System.String with
        static member ( * ) (s: string, n: int) : string = System.String.Concat(Array.replicate n s)  // repeat

module B =
    type System.String with
        static member ( * ) (s: string, n: int) : string = s + string n  // append-count

open A
open B // B opened last, must win

let r : string = "ha" * 2
if r <> "ha2" then failwith $"Expected 'ha2' (B append-count), got '{r}'"
