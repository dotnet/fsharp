// RFC FS-1043: Return-type-based overload resolution via op_Explicit.
// op_Explicit/op_Implicit have built-in return-type disambiguation in F#.
// The AllowOverloadOnReturnTypeAttribute (defined in local FSharp.Core) extends
// this to arbitrary methods, but these tests only exercise the op_Explicit path
// since the attribute is not available in the SDK's FSharp.Core.

module AllowOverloadOnReturnType

// ---- op_Explicit has built-in return-type disambiguation ----

type MyNum =
    { Value: int }
    static member op_Explicit(x: MyNum) : int = x.Value
    static member op_Explicit(x: MyNum) : float = float x.Value
    static member op_Explicit(x: MyNum) : string = string x.Value

let r1: int = MyNum.op_Explicit({ Value = 42 })
if r1 <> 42 then failwith $"Expected 42, got {r1}"

let r2: float = MyNum.op_Explicit({ Value = 42 })
if r2 <> 42.0 then failwith $"Expected 42.0, got {r2}"

let r3: string = MyNum.op_Explicit({ Value = 42 })
if r3 <> "42" then failwith $"Expected '42', got '{r3}'"
