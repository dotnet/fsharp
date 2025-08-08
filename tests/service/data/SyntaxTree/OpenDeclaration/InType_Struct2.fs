[<Struct>]
type ABC =
    val a: Int32
    open System
    val b: Int32
    new (a) = 
        open type System.Int32
        { a = a; b = MinValue }
