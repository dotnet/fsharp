
type Foo() =
    // A read-write property.
    member this.MyReadWriteProperty
        with get () = myInternalValue
        and set (value) = myInternalValue <- value
