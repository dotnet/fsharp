// RFC FS-1043: Extrinsic extension methods participate in SRTP resolution.
//
// This test verifies that an extension method defined in a separate module
// (extrinsic to the type) is found by the SRTP constraint solver when it
// is in scope. The extension is on System.Int32 (an externally-defined type),
// making it a genuinely EXTRINSIC extension that REQUIRES --langversion:preview.
//
// The inline function is defined in Lib (where the extension IS in scope via
// the top-level open). Consumer calls it without needing its own open — the
// constraint was already solved using the extension at the definition site.

module ScopeCapture

module Extensions =
    type System.Int32 with
        static member Combine(a: int, b: int) = a * b

open Extensions

module Lib =
    // Int32.Combine is in scope here via the top-level open of Extensions.
    let inline combine (x: ^T) (y: ^T) = (^T: (static member Combine: ^T * ^T -> ^T) (x, y))

module Consumer =
    open Lib

    let r = combine 3 4

    if r <> 12 then
        failwith $"Expected 12, got {r}"
