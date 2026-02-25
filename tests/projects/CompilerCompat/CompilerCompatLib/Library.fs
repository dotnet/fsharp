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

    // ---- RFC FS-1043 extension operator on external type compat test ----

    /// Extension operator (*) on System.String for string repetition.
    type System.String with
        static member ( * ) (s: string, n: int) = System.String.Concat(System.Linq.Enumerable.Repeat(s, n))

    /// Inline function using extension operator — SRTP constraint resolved via extension.
    let inline repeatStr (s: string) (n: int) = s * n

    /// Concrete wrapper that captures the resolved call.
    let repeatStrConcrete (s: string) (n: int) : string = repeatStr s n

    // ---- RFC FS-1043 numeric widening via extension operators compat test ----

    /// Helper to add two floats using the built-in operator (defined before the extension to avoid self-reference).
    let private addFloats (x: float) (y: float) : float = x + y

    /// Extension operator that widens int to float for addition.
    type System.Double with
        static member (+) (x: float, y: int) = addFloats x (float y)

    /// Inline widening function: uses extension (+) to add int to float.
    let inline addWidened (x: float) (y: int) = x + y

    /// Concrete wrapper that captures the resolved widening call.
    let addWidenedConcrete (x: float) (y: int) : float = addWidened x y
