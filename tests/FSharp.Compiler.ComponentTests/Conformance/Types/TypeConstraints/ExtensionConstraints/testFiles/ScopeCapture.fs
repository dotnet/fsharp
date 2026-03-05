// RFC FS-1043: Extrinsic extension members participate in SRTP constraint resolution.
//
// Widget is defined in TypeDefs; its (+) operator is an EXTRINSIC extension
// defined in the separate Extensions module. Without --langversion:preview,
// this extrinsic extension would NOT participate in SRTP resolution (FS1215).
// With preview enabled, the extension is captured when the inline function
// is defined with the extension in scope.

module ScopeCapture

module TypeDefs =
    type Widget = { V: int }

module Extensions =
    open TypeDefs

    type Widget with
        static member (+)(a: Widget, b: Widget) = { V = a.V + b.V }

module Lib =
    open TypeDefs
    open Extensions

    let inline add (x: ^T) (y: ^T) = x + y

module Consumer =
    open TypeDefs
    open Extensions
    open Lib

    let r = add { V = 1 } { V = 2 }

    if r <> { V = 3 } then
        failwith $"Expected {{V=3}}, got {r}"
