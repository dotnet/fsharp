module StructCtorDebugPoints

// Regression test: a debug point that lands while the 'this' pointer of a value-type
// (struct) constructor is on the IL stack must spill it as a managed pointer (byref),
// not as the value type. Otherwise EmitDebugPoint emits a 'stloc' of an address into a
// value-typed local, producing invalid IL (ILVerify: "found address ... expected value").
[<Struct>]
type Flags =
    val bits: int64
    new (bits: int64) = { bits = bits }
    new (a: bool, b: bool, c: bool) =
        Flags((if a then 1L else 0L) |||
              (if b then 2L else 0L) |||
              (if c then 4L else 0L))
    member x.Bits = x.bits
