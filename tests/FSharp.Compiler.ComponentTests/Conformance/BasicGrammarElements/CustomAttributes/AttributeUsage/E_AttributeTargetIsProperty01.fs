// This tests that AttributeTargets.Property  is not allowed in union-case declaration

open System

[<AttributeUsage(AttributeTargets.Property)>]
type PropertyLevelAttribute() =
    inherit Attribute()

[<AttributeUsage(AttributeTargets.Property ||| AttributeTargets.Field)>]
type PropertyOrFieldLevelAttribute() =
    inherit Attribute()

type SomeUnion =
| [<PropertyLevel>] Case1 of int // Should fail
| [<PropertyOrFieldLevel>] Case2 of int // Should fail

