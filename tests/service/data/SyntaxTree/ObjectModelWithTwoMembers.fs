type X() =
    let mutable allowInto = 0
    member _.AllowIntoPattern with get() = allowInto and set v = allowInto <- v