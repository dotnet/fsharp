// This tests that AttributeTargets.Method is not allowed in class and struct types.

open System
open System.Diagnostics

[<AttributeUsage(AttributeTargets.Method)>]
type CustomMethodAttribute() =
    inherit Attribute()

[<CustomMethod>]
type Class() = class end

[<CustomMethod>]
type Struct(x: int) = struct end

