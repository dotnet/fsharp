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

    /// Literal string used as an attribute argument.
    /// Tests that Expr.Val in AttribExpr.source pickles/unpickles across compiler versions.
    [<Literal>]
    let LiteralAttrArg = "compat-test-value"

    /// Custom attribute for cross-version literal attribute arg testing
    type TestAttrAttribute(value: string) =
        inherit System.Attribute()
        member _.Value = value

    /// Type decorated with an attribute whose argument is a literal val reference
    [<TestAttr(LiteralAttrArg)>]
    type TypeWithLiteralAttrArg() =
        member _.GetValue() = LiteralAttrArg

    /// Plain record used to test the FS-1073 positional record constructor across compiler versions.
    /// The record itself is ordinary and compiles on any compiler; only the *construction* syntax is new.
    type RecordCtorPoint = { A: int; B: int }

    /// inline function that constructs the record. When this library is built with the local compiler
    /// (RECORD_CTOR_FEATURE defined), it uses the new positional constructor; otherwise classic record
    /// syntax. Both elaborate to the same record-allocation node, so the pickled inline body must be
    /// consumable by an app built with an older compiler (the FS-1073 cross-compiler concern).
    let inline makeRecordCtorPoint a b =
#if RECORD_CTOR_FEATURE
        RecordCtorPoint(a, b)
#else
        { RecordCtorPoint.A = a; RecordCtorPoint.B = b }
#endif
