// #Regression #NoMono #NoMT #CodeGen #EmittedIL 
type C() = 
    [<VolatileFieldAttribute>]
    let mutable x = 1

    member this.X with get() = x and set v = x <- v


type StaticC() = 
    [<VolatileFieldAttribute>]
    static let mutable x = 1

    static member X with get() = x and set v = x <- v
