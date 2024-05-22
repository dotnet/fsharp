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

[<AttributeUsage(AttributeTargets.Field)>]
type Name(x: string) =
    inherit Attribute()
    member _.value = x

type User =
    { [<Name("id")>]
      Id: int
      [<Name("email")>]
      Email: string
      [<Name("organization_id")>]
      OrganizationId: option<string> }