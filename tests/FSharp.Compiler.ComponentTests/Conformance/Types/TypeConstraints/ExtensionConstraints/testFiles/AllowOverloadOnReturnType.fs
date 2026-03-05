// RFC FS-1043: Return-type-based overload resolution through SRTP.
// op_Explicit has built-in return-type disambiguation in the constraint solver
// (same alwaysConsiderReturnType path as AllowOverloadOnReturnType).
// This test verifies the mechanism works when resolution goes through SRTP constraints.

module AllowOverloadOnReturnType

#nowarn "77" // op_Explicit member constraint warning

// ---- Direct call-site resolution via op_Explicit ----

type Converter =
    { Value: string }
    static member op_Explicit(x: Converter) : int = int x.Value
    static member op_Explicit(x: Converter) : float = float x.Value
    static member op_Explicit(x: Converter) : string = x.Value.ToUpper()

let r1: int = Converter.op_Explicit({ Value = "42" })
if r1 <> 42 then failwith $"Expected 42, got {r1}"

let r2: float = Converter.op_Explicit({ Value = "42" })
if r2 <> 42.0 then failwith $"Expected 42.0, got {r2}"

let r3: string = Converter.op_Explicit({ Value = "hello" })
if r3 <> "HELLO" then failwith $"Expected 'HELLO', got '{r3}'"

// ---- Return-type disambiguation through SRTP (H2) ----
// The return-type-based resolution must also work when going through an inline SRTP constraint.

type Converter2 =
    static member op_Explicit(x: int) : float = float x
    static member op_Explicit(x: int) : string = string x

let inline convert (x: int) : ^U =
    ((^U or Converter2) : (static member op_Explicit: int -> ^U) x)

let r4: float = convert 42
if r4 <> 42.0 then failwith $"Expected 42.0, got {r4}"

let r5: string = convert 42
if r5 <> "42" then failwith $"Expected '42', got '{r5}'"
