open System

open System

[<AttributeUsage(AttributeTargets.Method)>]
type CustomMethodAttribute() =
    inherit Attribute()

type Class1 [<CustomMethod>] () = class end

type Struct1 [<CustomMethod>](c: int) = struct end

[<Class>]
type Class2 [<CustomMethod>]() = class end

[<Struct>]
type Struct2 [<CustomMethod>](c: int) = struct end
