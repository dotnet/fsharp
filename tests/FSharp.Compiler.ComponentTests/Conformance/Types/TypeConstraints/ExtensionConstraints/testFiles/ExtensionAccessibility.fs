// RFC FS-1043: Extension member accessibility rules.

module ExtensionAccessibility

// A public extension member solves an SRTP constraint everywhere, because the
// inline function that carries the solution can be used from any scope.

module PublicExt =
    type System.Int32 with
        static member Ping(x: int) = x + 100

open PublicExt

let inline ping (x: ^T) = (^T : (static member Ping: ^T -> ^T) x)

let r1 = ping 5
if r1 <> 105 then failwith $"Expected 105, got {r1}"
