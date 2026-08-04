// RFC FS-1043: Extension member precedence rules.

module ExtensionPrecedence

// ---- Most-recently-opened extension wins ----

module ExtA =
    type System.Int32 with
        static member Combine(a: int, b: int) = a + b

module ExtB =
    type System.Int32 with
        static member Combine(a: int, b: int) = a * b

open ExtA
open ExtB // ExtB is opened last, should win

let inline combine (x: ^T) (y: ^T) = (^T : (static member Combine: ^T * ^T -> ^T) (x, y))

let r1 = combine 3 4
if r1 <> 12 then failwith $"Expected 12 (multiply from ExtB), got {r1}"

// ---- Multiple extensions on same type with different signatures ----

type System.String with
    static member Transform(s: string, n: int) = System.String.Concat(Array.replicate n s)

type System.String with
    static member Transform(s: string, prefix: string) = prefix + s

let inline transformInt (x: ^T) (n: int) = (^T : (static member Transform: ^T * int -> ^T) (x, n))
let inline transformStr (x: ^T) (p: string) = (^T : (static member Transform: ^T * string -> ^T) (x, p))

let r2 = transformInt "ab" 3
if r2 <> "ababab" then failwith $"Expected 'ababab', got '{r2}'"

let r3 = transformStr "world" "hello "
if r3 <> "hello world" then failwith $"Expected 'hello world', got '{r3}'"

// ---- Built-in operators still work ----
// Sanity check: existing built-in operators are unaffected by extension constraints being active.

let r4 = 1 + 2
if r4 <> 3 then failwith $"Expected 3, got {r4}"
