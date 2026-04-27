// RFC FS-1043: AllowOverloadOnReturnType attribute.
// Enables return-type-based overload resolution for arbitrary methods,
// extending behavior previously reserved for op_Explicit and op_Implicit.

module AllowOverloadOnReturnType

type Converter =
    [<AllowOverloadOnReturnType>]
    static member Convert(x: string) : int = int x
    [<AllowOverloadOnReturnType>]
    static member Convert(x: string) : float = float x
    [<AllowOverloadOnReturnType>]
    static member Convert(x: string) : string = x.ToUpper()

let r1: int = Converter.Convert("42")
if r1 <> 42 then failwith $"Expected 42, got {r1}"

let r2: float = Converter.Convert("42")
if r2 <> 42.0 then failwith $"Expected 42.0, got {r2}"

let r3: string = Converter.Convert("hello")
if r3 <> "HELLO" then failwith $"Expected 'HELLO', got '{r3}'"

// ---- AllowOverloadOnReturnType through SRTP (H2) ----
// The attribute must also work when resolution goes through an inline SRTP constraint.

type Converter2 =
    [<AllowOverloadOnReturnType>]
    static member Convert(x: int) : float = float x
    [<AllowOverloadOnReturnType>]
    static member Convert(x: int) : string = string x

let inline convert (x: int) : ^U =
    ((^U or Converter2) : (static member Convert: int -> ^U) x)

let r4: float = convert 42
if r4 <> 42.0 then failwith $"Expected 42.0, got {r4}"

let r5: string = convert 42
if r5 <> "42" then failwith $"Expected '42', got '{r5}'"
