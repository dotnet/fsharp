
open System

[<AttributeUsage(AttributeTargets.Property)>]
type PropertyLevelAttribute() =
    inherit Attribute()

type U =
    | [<PropertyLevel>] A
    | [<PropertyLevel>] B

[<AttributeUsage(AttributeTargets.Property)>]
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