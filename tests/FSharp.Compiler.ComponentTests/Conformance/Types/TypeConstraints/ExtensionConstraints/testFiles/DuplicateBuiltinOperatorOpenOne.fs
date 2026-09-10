// RFC FS-1043: two same-signature extension operators on the built-in '*' symbol exist in the
// same compilation, distinguished only by which module is opened. Opening exactly one must make
// the concrete call '"ha" * 2' dispatch through FSharp.Core's inline (*) to THAT extension at
// runtime (not crash with a NotSupportedException, not pick the other one).

module DuplicateBuiltinOperatorOpenOne

module A =
    type System.String with
        static member ( * ) (s: string, n: int) : string = System.String.Concat(Array.replicate n s)  // repeat

module B =
    type System.String with
        static member ( * ) (s: string, n: int) : string = s + string n  // append-count

open A // only A is opened

let r : string = "ha" * 2
if r <> "haha" then failwith $"Expected 'haha' (A repeat), got '{r}'"
