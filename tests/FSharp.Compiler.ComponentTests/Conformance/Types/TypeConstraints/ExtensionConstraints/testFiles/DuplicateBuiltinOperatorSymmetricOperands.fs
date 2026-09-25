// RFC FS-1043: two sibling scopes each open a DIFFERENT same-signature extension operator on the built-in
// '*' symbol, this time with EQUAL operand types (string * string -> string). The built-in '*' constraint
// is (^T1 or ^T2 : ...), so its support types are [^T1; ^T2]; when both operands are the same concrete
// type the checker deduplicates support to [string] before recording the scope-aware solution, while the
// inlined trait node the optimizer replays still carries [string; string]. The sink key must canonicalize
// support types identically on both sides, or the recorded decision is lost and '*' falls back to its
// throwing dynamic stub (NotSupportedException at run). M1 keeps A (concat), M2 keeps B (reverse-concat).

module DuplicateBuiltinOperatorSymmetricOperands

module A =
    type System.String with
        static member ( * ) (s1: string, s2: string) : string = s1 + s2         // concat

module B =
    type System.String with
        static member ( * ) (s1: string, s2: string) : string = s2 + s1         // reverse-concat

module M1 =
    open A
    let f () : string = "ha" * "ho"

module M2 =
    open B
    let g () : string = "ha" * "ho"

let a = M1.f ()
let b = M2.g ()
if a <> "haho" then failwith $"M1 (open A, concat) expected 'haho', got '{a}'"
if b <> "hoha" then failwith $"M2 (open B, reverse-concat) expected 'hoha', got '{b}'"
