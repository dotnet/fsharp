// RFC FS-1043: Extrinsic extension members participate in SRTP constraint resolution.
//
// System.String has no built-in (*) operator. The extension below is extrinsic
// (String is a BCL type, not defined here). The inline function 'multiply' is
// defined after the extension, so the extension is in scope when the SRTP
// constraint is created and captured. Without --langversion:preview, extensions
// do NOT participate in SRTP resolution and this code fails to compile.

module ScopeCapture

type System.String with
    static member (*)(s: string, n: int) = System.String.Concat(Array.replicate n s)

let inline multiply (x: ^T) (n: int) = x * n

let r = multiply "ha" 3

if r <> "hahaha" then
    failwith $"Expected 'hahaha', got '{r}'"
