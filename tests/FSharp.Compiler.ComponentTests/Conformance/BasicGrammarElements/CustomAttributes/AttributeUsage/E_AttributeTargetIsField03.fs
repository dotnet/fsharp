// This tests that AttributeTargets.Field  is not allowed in union-case declaration

open System

[<AttributeUsage(AttributeTargets.Field)>]
type FieldLevelAttribute() =
    inherit Attribute()

[<AttributeUsage(AttributeTargets.Property ||| AttributeTargets.Field)>]
type PropertyOrFieldLevelAttribute() =
    inherit Attribute()

type SomeUnion =
| [<FieldLevel>] Case1 of int  // Should fail
| [<PropertyOrFieldLevel>] Case2 of int  // Should fail

