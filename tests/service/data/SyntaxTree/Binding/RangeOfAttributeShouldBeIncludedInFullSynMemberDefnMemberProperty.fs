
type Bird =
    [<Foo>]
    member this.TheWord
        with get () = myInternalValue
        and set (value) = myInternalValue <- value
