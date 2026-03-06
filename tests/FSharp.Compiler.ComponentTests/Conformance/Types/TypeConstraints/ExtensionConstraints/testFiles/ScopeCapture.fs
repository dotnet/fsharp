// RFC FS-1043: Extrinsic extension members participate in SRTP constraint resolution.
//
// This test uses three modules to demonstrate scope capture:
//
//   StringOps – defines an extrinsic (*) extension on System.String (a BCL type).
//   GenericLib – opens StringOps, then defines 'multiply'. The extension is in scope
//               at the DEFINITION site so it is captured in the SRTP constraint.
//   Consumer  – opens GenericLib only (not StringOps directly). The captured extension
//               travels with the constraint and resolves at the call site.
//
// Without --langversion:preview, extensions do NOT participate in SRTP resolution
// and this code fails to compile (FS0001: string does not support operator '*').

module ScopeCapture

module StringOps =
    type System.String with
        static member (*)(s: string, n: int) = System.String.Concat(Array.replicate n s)

module GenericLib =
    open StringOps

    let inline multiply (x: ^T) (n: int) = x * n

module Consumer =
    open GenericLib

    let r = multiply "ha" 3

    if r <> "hahaha" then
        failwith $"Expected 'hahaha', got '{r}'"
