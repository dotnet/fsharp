// RFC FS-1043: Extension member accessibility rules.

module ExtensionAccessibility

// ---- Public extension operator is visible everywhere ----

module PublicExt =
    type System.Int32 with
        static member Ping(x: int) = x + 100

open PublicExt

let inline ping (x: ^T) = (^T : (static member Ping: ^T -> ^T) x)

let r1 = ping 5
if r1 <> 105 then failwith $"Expected 105, got {r1}"

// ---- Internal module extension: visible within this compilation unit ----

module internal InternalExt =
    type System.Int32 with
        static member Pong(x: int) = x + 200

open InternalExt

let inline pong (x: ^T) = (^T : (static member Pong: ^T -> ^T) x)

let r2 = pong 5
if r2 <> 205 then failwith $"Expected 205, got {r2}"
