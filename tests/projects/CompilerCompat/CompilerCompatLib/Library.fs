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

    /// Inline function using SRTP member constraint that can resolve to a field or property
    let inline getMemberValue (x: ^T) : string = (^T: (member Value: string) x)

    // F# struct with explicit public field for SRTP field constraint testing.
    // Note: This exercises the F# record-field SRTP path (FSRecdFieldSln), not the
    // IL field path (ILFieldSln). The ILFieldSln path requires the preview-only
    // feature SupportILFieldsInSRTP and a non-F# type (e.g., C# class with public field),
    // which cannot be tested cross-version because released SDK compilers don't support
    // the preview feature. The ILFieldSln pickle tag (8) is verified by 26+ component
    // tests in tests/FSharp.Compiler.ComponentTests (ConstraintSolver/MemberConstraints).
    [<Struct>]
    type FieldHolder =
        val mutable FieldValue: int
        new(v) = { FieldValue = v }

    /// Inline function using SRTP to read a struct field value
    let inline getFieldValue (x: ^T) : int = (^T: (member FieldValue: int) x)