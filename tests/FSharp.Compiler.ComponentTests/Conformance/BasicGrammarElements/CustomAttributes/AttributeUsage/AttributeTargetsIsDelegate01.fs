open System

[<AttributeUsage(AttributeTargets.Delegate)>]
type CustomDelegateAttribute() =
    inherit Attribute()
    
[<CustomDelegate>]
type Delegate1 = delegate of int -> int
