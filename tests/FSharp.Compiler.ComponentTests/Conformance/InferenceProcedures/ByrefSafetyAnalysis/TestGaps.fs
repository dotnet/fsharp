module TestGaps

open System
open System.Runtime.InteropServices

// 1. Generic struct with byref field (not possible in F# definition, but possible in C# interop or IL)
// F# doesn't support defining ref fields yet.
// But we can consume.

// 2. Interaction with computation expressions / closures
// Span cannot be captured by closure. This is already enforced by FS-1053.
// But what if the closure *returns* a span that captures a local byref?
// Closures cannot return Spans in F# (compile error usually).
// "The type of a first-class function cannot contain byrefs" (FS-1053).
// So this is likely safe by exclusion.

// 3. Operator overloading
// let (+) (x: byref<int>) (y: byref<int>) : Span<int> = ...
// If we define such operator, does it catch capture?

// 4. Indexers
// type S = member this.Item with get(x: byref<int>) = Span<int>(&x)

// 5. Match expression
// let f (x: byref<int>) (y: byref<int>) =
//     match true with
//     | true -> Span<int>(&x)
//     | false -> Span<int>(&y)

// 6. Generic constraints
// let f<'T when 'T : struct> (x: byref<'T>) : Span<'T> = ...
