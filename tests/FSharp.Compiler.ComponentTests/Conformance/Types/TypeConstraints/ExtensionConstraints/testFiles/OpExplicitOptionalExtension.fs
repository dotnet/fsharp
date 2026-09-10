// RFC FS-1043: a public OPTIONAL (extrinsic) op_Explicit extension member solves an SRTP
// constraint, disambiguated by return type. Distinct from OpExplicitReturnType.fs, whose
// op_Explicit overloads are INTRINSIC members called directly (no SRTP): here the op_Explicit
// overloads live in a separate module that must be opened, and are reached only through the
// inline SRTP function 'convert'. Removing 'open Conversions' makes every call fail with FS0001
// "does not support a conversion", which proves the witness is the extension, not a built-in.

module OpExplicitOptionalExtension

#nowarn "77" // op_Explicit has special member-constraint status; driving it through SRTP is intentional here

module Domain =
    type Money = { Cents: int }

module Conversions =
    open Domain
    type Money with
        static member op_Explicit (m: Money) : int = m.Cents
        static member op_Explicit (m: Money) : float = float m.Cents / 100.0
        static member op_Explicit (m: Money) : string = sprintf "$%.2f" (float m.Cents / 100.0)

open Domain
open Conversions

let inline convert (x: ^T) : ^U = ((^T or ^U) : (static member op_Explicit : ^T -> ^U) x)

let asInt: int = convert { Cents = 1234 }
if asInt <> 1234 then failwith $"Expected 1234, got {asInt}"

let asFloat: float = convert { Cents = 1234 }
if asFloat <> 12.34 then failwith $"Expected 12.34, got {asFloat}"

let asString: string = convert { Cents = 1234 }
if asString <> "$12.34" then failwith $"Expected '$12.34', got '{asString}'"
