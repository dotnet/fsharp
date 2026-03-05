// RFC FS-1043: Extrinsic extension methods in SRTP constraint resolution.
//
// IMPORTANT: Widget and its (+) extension are in SEPARATE modules.
// This is an EXTRINSIC extension — the feature RFC FS-1043 actually enables.
// An intrinsic extension (same module) would work even without the feature flag.
//
// The Extensions module is [<AutoOpen>], so its extrinsic (+) operator is
// available throughout the file without explicit opens. Lib.add does NOT
// explicitly open Extensions — the extension is resolved via auto-open scope.

module ScopeCapture

module TypeDefs =
    type Widget = { V: int }

[<AutoOpen>]
module Extensions =
    open TypeDefs

    type Widget with
        static member (+)(a: Widget, b: Widget) = { V = a.V + b.V }

module Lib =
    let inline add (x: ^T) (y: ^T) = x + y

module Consumer =
    open TypeDefs
    open Extensions
    open Lib

    let r = add { V = 1 } { V = 2 }

    if r <> { V = 3 } then
        failwith $"Expected {{V=3}}, got {r}"
