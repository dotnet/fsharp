// RFC FS-1043: Weak resolution changes for inline code.
// Tests that inline generic code stays generic when extension constraints are active.

module WeakResolution

open System

// ---- DateTime + y stays generic for inline code ----
// Prior to RFC-1043, weak resolution eagerly applied overload resolution here.
// Since DateTime has only one op_Addition overload (DateTime + TimeSpan -> DateTime),
// weak resolution succeeded and f1 was inferred as non-generic: DateTime -> TimeSpan -> DateTime.
// With RFC-1043, weak resolution no longer forces overload resolution for inline code,
// so f1 stays generic.

let inline f1 (x: DateTime) y = x + y

// Verify the function is usable with TimeSpan (the only intrinsic overload)
let r1 = f1 DateTime.MinValue (TimeSpan.FromHours(1.0))
if r1 <> DateTime.MinValue.Add(TimeSpan.FromHours(1.0)) then
    failwith "DateTime + TimeSpan failed"

// ---- FSharpPlus-style InvokeMap pattern (sequentialized workaround) ----
// When `InvokeMap` uses return types in the support type set (^I or ^R),
// the return type of the inner call isn't known until weak resolution runs.
// With RFC-1043, weak resolution is deferred for inline code, so nesting
// InvokeMap calls directly (InvokeMap f (InvokeMap g x)) fails because
// the inner return type isn't resolved before generalization.
// The workaround is to sequentialize with a let-binding, which forces
// weak resolution of the inner call before the outer call is checked.

let inline InvokeMap (mapping: ^F) (source: ^I) : ^R =
    ((^I or ^R) : (static member Map : ^I * ^F -> ^R) source, mapping)

type Coll<'T>(x: 'T) =
    member _.X = x
    static member Map (source: Coll<'a>, mapping: 'a -> 'b) : Coll<'b> = Coll<'b>(mapping source.X)

// Sequentialized version (RFC-recommended workaround)
let inline AddTwice (x: Coll<'a>) (v: 'a) : Coll<'a> =
    let step1 = InvokeMap ((+) v) x
    InvokeMap ((+) v) step1

let r2 = AddTwice (Coll(3)) 2
if r2.X <> 7 then failwith $"Expected 7, got {r2.X}"

let r3 = AddTwice (Coll(10uy)) 5uy
if r3.X <> 20uy then failwith $"Expected 20, got {r3.X}"
