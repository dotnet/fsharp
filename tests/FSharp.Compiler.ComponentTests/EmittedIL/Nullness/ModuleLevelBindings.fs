module MyTestModule
[<Literal>]
let nullableLiteral : string | null = ""
let notNullStringField : string = ""
let nullableStringField : string | null = null
let mutable nullableMutableStringField : string | null = null
let nullableInt : System.Nullable<int> = System.Nullable()
let regularInt = 42