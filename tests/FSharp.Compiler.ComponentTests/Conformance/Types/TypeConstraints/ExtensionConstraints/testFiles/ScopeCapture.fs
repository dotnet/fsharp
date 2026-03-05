// RFC FS-1043: Extensions captured at call site, not definition site.
// The SRTP constraint for (+) is resolved using members in scope where the
// generic inline construct is USED (the call site), not where it is DEFINED.
//
// Widget and its (+) augmentation live in TypeDefs. The inline function in
// Lib does NOT open TypeDefs, so Widget.(+) is not in scope there. Consumer
// opens TypeDefs — bringing Widget.(+) into scope — and invokes the inline
// function. The constraint is resolved at the call site.

module ScopeCapture

module TypeDefs =
    type Widget = { V: int }

    type Widget with
        static member (+)(a: Widget, b: Widget) = { V = a.V + b.V }

module Extensions =
    // Extrinsic extension on an external type — requires --langversion:preview.
    // This ensures the test exercises the ExtensionConstraintSolutions feature gate.
    type System.Int32 with
        static member Combine(a: int, b: int) = a * b

module Lib =
    open Extensions

    // Widget.(+) is NOT in scope here — TypeDefs is not opened.
    // Int32.Combine IS in scope via `open Extensions` above.
    let inline add (x: ^T) (y: ^T) = x + y

    let inline combine (x: ^T) (y: ^T) =
        (^T: (static member Combine: ^T * ^T -> ^T) (x, y))

module Consumer =
    open TypeDefs
    open Extensions
    open Lib

    // Widget.(+) IS in scope HERE at the call site, not at Lib.add's definition site.
    let r1 = add { V = 1 } { V = 2 }

    if r1 <> { V = 3 } then
        failwith $"Expected {{V=3}}, got {r1}"

    // Int32.Combine uses an extrinsic extension — verifies the preview feature gate.
    let r2 = combine 3 4

    if r2 <> 12 then
        failwith $"Expected 12, got {r2}"
