
module Neg50

module EnumPatternWithFunkyTypes_Dev11_13904 = 
    [<Struct>]

    type Struct =
        val x:int
        val y:int

    type En<'a>(mvNext:'a) =
        member x.Current = 1
        member x.MoveNext() = mvNext

    type T<'a>(mvNext:'a) =
        member x.GetEnumerator() = En mvNext

    // This is not allowed:
    let t = seq { for i in T "test" -> i }

    // This is not allowed:
    let u = seq { for i in T (Struct()) -> i }

    // This is not allowed - 'a (and the type of "a") are not known to be "bool"
    let v a = seq { for i in T a -> i }

module EnumPatternWithFunkyTypes2_Dev11_13904 = 
    [<Struct>]

    type Struct =
        val x:int
        val y:int

    type En() =
        member x.Current with get () = 1
        member x.MoveNext(v:int) = true

    type T() =
        member x.GetEnumerator() = En()

    // This is not allowed - MoveNext has an arg
    let t = seq { for i in T() -> i }


module EnumPatternWithFunkyTypes3_Dev11_13904 = 
    [<Struct>]

    type Struct =
        val x:int
        val y:int

    type En() =
        member x.Current with get (v:int) = 1
        member x.MoveNext() = true

    type T() =
        member x.GetEnumerator() = En()

    // This is not allowed - Current has an arg
    let t = seq { for i in T() -> i }

module EnumPatternWithFunkyTypes4_Dev11_13904 = 
    [<Struct>]

    type Struct =
        val x:int
        val y:int

    type En() =
        member x.Current with get () = 1
        member x.MoveNext() = true

    type T() =
        member x.GetEnumerator(c:int) = En()

    // This is not allowed - GetEnumerator has an arg
    let t = seq { for i in T() -> i }

