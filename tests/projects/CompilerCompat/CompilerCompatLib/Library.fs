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

    // ---- RFC FS-1043 breaking change compat tests ----

    /// Inline function with operator + literal (T4a)
    let inline addOne x = x + 1
    let addOneConcrete (x: int) : int = addOne x

    /// Inline unary negate (T4b)
    let inline negate x = -x
    let negateConcrete (x: int) : int = negate x

    /// Takes a function int -> int (T4c)
    let applyToInt (f: int -> int) (x: int) = f x

    /// Custom type with intrinsic operator (T5)
    type Num = { V: int }
        with static member (+) (a: Num, b: Num) = { V = a.V + b.V }

    let inline addNums (a: Num) (b: Num) = a + b
    let addNumsConcrete (a: Num) (b: Num) : Num = addNums a b



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

    // ---- RFC FS-1043 extension operator compat tests ----

    /// Type with NO intrinsic operators
    type StringRep = { Value: string }

    /// Extension operator on StringRep: repeat via (<*>)
    type StringRep with
        static member (<*>) (s: StringRep, n: int) =
            { Value = System.String.Concat(System.Linq.Enumerable.Repeat(s.Value, n)) }

    /// Inline SRTP function using the extension operator
    let inline repeatRep (s: ^T) (n: int) =
        (^T : (static member (<*>) : ^T * int -> ^T) (s, n))

    /// Concrete wrapper (extension resolved at definition time)
    let repeatRepConcrete (s: StringRep) (n: int) : StringRep = repeatRep s n

    /// Extension operator on generic Wrapper
    type Wrapper<'T> = { Inner: 'T }

    type Wrapper<'T> with
        static member (++) (a: Wrapper<'T>, b: Wrapper<'T>) = { Inner = a.Inner }

    /// Inline SRTP function using generic extension operator
    let inline mergeWrappers (a: ^T) (b: ^T) =
        (^T : (static member (++) : ^T * ^T -> ^T) (a, b))

    /// Concrete wrapper
    let mergeWrappersConcrete (a: Wrapper<int>) (b: Wrapper<int>) : Wrapper<int> = mergeWrappers a b

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
