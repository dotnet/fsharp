// RFC FS-1043: Extension members become available to solve SRTP constraints.
// Tests that extension operators on various types are picked up by the constraint solver.

module BasicExtensionOperators

// ---- The motivating example from the RFC ----

type System.String with
    static member (*) (s: string, n: int) = System.String.Concat(Array.replicate n s)

let r4 = "ha" * 3

if r4 <> "hahaha" then failwith $"Expected 'hahaha', got '{r4}'"

// ---- New operator on Int32 ----

type System.Int32 with
    static member (++) (a: int, b: int) = a + b + 1

let inline incAdd (x: ^T) (y: ^T) = x ++ y

let r5 = incAdd 3 4
if r5 <> 8 then failwith $"Expected 8, got {r5}"

// ---- Extension on user-defined types (record, DU, struct) ----
// The user types are DEFINED in one module and AUGMENTED in a separate module that must be
// opened, so each (+) augmentation is an OPTIONAL (extrinsic) extension. This proves the SRTP
// constraint on 'addThings' is solved by extension lookup — an augmentation in the type's own
// module would instead be an intrinsic member and never exercise the extension path.

module UserTypes =
    type MyRec = { X: int; Y: int }
    type Amount = Amount of int
    [<Struct>]
    type Vec2 = { Dx: float; Dy: float }

module UserTypeExtensions =
    open UserTypes
    type MyRec with
        static member (+) (a: MyRec, b: MyRec) = { X = a.X + b.X; Y = a.Y + b.Y }
    type Amount with
        static member (+) (Amount a, Amount b) = Amount(a + b)
    type Vec2 with
        static member (+) (a: Vec2, b: Vec2) = { Dx = a.Dx + b.Dx; Dy = a.Dy + b.Dy }

open UserTypes
open UserTypeExtensions

let inline addThings (a: ^T) (b: ^T) = a + b

let r7 = addThings { X = 1; Y = 2 } { X = 3; Y = 4 }
if r7 <> { X = 4; Y = 6 } then failwith $"Expected {{X=4;Y=6}}, got {r7}"

let r8 = addThings (Amount 10) (Amount 20)
if r8 <> Amount 30 then failwith $"Expected Amount 30, got {r8}"

let r9 = addThings { Dx = 1.0; Dy = 2.0 } { Dx = 3.0; Dy = 4.0 }
if r9 <> { Dx = 4.0; Dy = 6.0 } then failwith $"Expected {{Dx=4;Dy=6}}, got {r9}"

// ---- Extension static method via SRTP ----

type System.Int32 with
    static member Negate(x: int) = -x

let inline negateIt (x: ^T) = (^T : (static member Negate: ^T -> ^T) x)

let r10 = negateIt 42
if r10 <> -42 then failwith $"Expected -42, got {r10}"
