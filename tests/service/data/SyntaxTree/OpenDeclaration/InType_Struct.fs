[<Struct>]
type ABC =
    open System
    val a: Int32
    val b: Int32
    new (a) = 
        open type System.Int32
        { a = a; b = MinValue }
