// RFC FS-1043: Extension methods captured at call site, not definition site.
//
// Lib.add is defined first — at its definition site, Widget does not exist.
// Widget and its (+) extension are defined after Lib. The call site opens Lib
// and uses add with Widget values, proving that SRTP constraints incorporate
// members available at the call site, not just the definition site.

module ScopeCapture

module Lib =
    let inline add (x: ^T) (y: ^T) = x + y

type Widget = { V: int }

type Widget with
    static member (+)(a: Widget, b: Widget) = { V = a.V + b.V }

open Lib

let r = add { V = 1 } { V = 2 }

if r <> { V = 3 } then
    failwith $"Expected {{V=3}}, got {r}"
