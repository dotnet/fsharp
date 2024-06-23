module M
open System

type MyAttribute<^T>() =
    inherit Attribute()

[<My<AClass<AClass<AClass<MyAttribute<int>>>>>>]
type AClass<'a>(arg) = class end