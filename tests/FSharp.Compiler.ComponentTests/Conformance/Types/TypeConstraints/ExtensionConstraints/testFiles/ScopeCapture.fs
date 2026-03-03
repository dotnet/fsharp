// RFC FS-1043: Extension methods captured at call site, not definition site.
// The SRTP constraint incorporates extension methods in scope at the point
// the constraint is freshened (when the generic construct is used).

module ScopeCapture

module Lib =
    let inline add (x: ^T) (y: ^T) = x + y

type Widget = { V: int }

type Widget with
    static member (+) (a: Widget, b: Widget) = { V = a.V + b.V }

open Lib

// Widget.(+) is in scope HERE at the call site, not at Lib.add's definition site.
let r = add { V = 1 } { V = 2 }
if r <> { V = 3 } then failwith $"Expected {{V=3}}, got {r}"
