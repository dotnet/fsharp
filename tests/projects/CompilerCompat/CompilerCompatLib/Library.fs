namespace CompilerCompatLib

module Library =
    /// Returns an anonymous record to test compiler compatibility
    let getAnonymousRecord () = {| X = 42; Y = "hello" |}
    
    /// Returns a more complex anonymous record with nested structure
    let getComplexAnonymousRecord () = 
        {| 
            Simple = {| A = 1; B = "test" |};
            List = [ {| Id = 1; Name = "first" |}; {| Id = 2; Name = "second" |} ];
            Tuple = (42, {| Value = 3.14; Label = "pi" |})
        |}
    
    /// Function that takes an anonymous record as parameter
    let processAnonymousRecord (record: {| X: int; Y: string |}) =
        sprintf "Processed: X=%d, Y=%s" record.X record.Y

    /// Type with Sealed attribute for compatibility testing
    [<Sealed>]
    type SealedType() =
        member _.Value = 42

    /// Type with Struct attribute for compatibility testing
    [<Struct>]
    type StructRecord = { X: int; Y: float }

    /// Type with DefaultAugmentation(false) for compatibility testing
    [<DefaultAugmentation(false)>]
    type NoHelpersUnion = Case1 | Case2 of int

    /// Value with RequireQualifiedAccess for compatibility testing
    [<RequireQualifiedAccess>]
    type QualifiedEnum = A = 0 | B = 1

    /// Value with Literal attribute
    [<Literal>]
    let LiteralValue = 42

    /// Function with ReflectedDefinition
    [<ReflectedDefinition>]
    let reflectedFunction x = x + 1
