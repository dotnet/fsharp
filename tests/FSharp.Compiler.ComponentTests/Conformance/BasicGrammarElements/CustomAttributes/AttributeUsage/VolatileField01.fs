type InterruptibleLazy (value) =
    [<VolatileField>]
    let mutable valueFactory = value