type InterruptibleLazy (value) =
    [<VolatileField>]
    let mutable valueFactory = value

    [<VolatileField>]
    static let mutable valueFactory2 = Unchecked.defaultof<_>

    [<VolatileField>]
    let mutable add1 = fun x -> x + 1

    [<VolatileField>]
    static let mutable add2 = fun x -> x + 1
