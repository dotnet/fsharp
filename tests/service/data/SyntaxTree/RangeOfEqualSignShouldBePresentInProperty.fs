
type Y() =
    member this.MyReadWriteProperty
        with get () = myInternalValue
        and set (value) = myInternalValue <- value
