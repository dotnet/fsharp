module M
open System

type MyAttribute<^T>() =
    inherit Attribute()

type AliasedAttribute = MyAttribute<int>

[<Aliased>]
let x = 1