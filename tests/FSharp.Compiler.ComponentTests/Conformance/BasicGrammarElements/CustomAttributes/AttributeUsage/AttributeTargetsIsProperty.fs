
open System

[<AttributeUsage(AttributeTargets.Property)>]
type PropertyLevelAttribute() =
    inherit Attribute()

type U =
    | [<PropertyLevel>] A
    | [<PropertyLevel>] B