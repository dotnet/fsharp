// RFC FS-1043: two sibling scopes each open a DIFFERENT same-signature extension operator on the built-in
// '*' symbol, wrapped in a GENERIC 'let inline' function so the '*' trait is solved per call site (SRTP),
// not at the definition. Under --optimize- the compiler builds a debug specialization of each inline body
// via CopyExprForInlining, which keeps the DEFINITION-site range of the trait. Replaying the recorded
// scope-aware solution at that def range finds no match once two scopes solved the same key differently, so
// both sites collided and fell back to FSharp.Core's throwing dynamic '*' stub (NotSupportedException at
// run, Debug-only, previously silent). The optimizer must also try the user call-site range so each inline
// specialization keeps its scope's decision: M1 keeps A (repeat), M2 keeps B (append-count).
//
// The 'mul' functions are intentionally left without explicit type annotations so they carry the
// '^a * ^b -> ^c' member constraint and are resolved at each application.

module DuplicateBuiltinOperatorInlineDistinctScopes

module A =
    type System.String with
        static member ( * ) (s: string, n: int) : string = System.String.Concat(Array.replicate n s)  // repeat

module B =
    type System.String with
        static member ( * ) (s: string, n: int) : string = s + string n  // append-count

module M1 =
    open A
    let inline mul x n = x * n
    let f () : string = mul "ha" 2

module M2 =
    open B
    let inline mul x n = x * n
    let g () : string = mul "ha" 2

let a = M1.f ()
let b = M2.g ()
if a <> "haha" then failwith $"M1 (open A, repeat) expected 'haha', got '{a}'"
if b <> "ha2" then failwith $"M2 (open B, append-count) expected 'ha2', got '{b}'"
