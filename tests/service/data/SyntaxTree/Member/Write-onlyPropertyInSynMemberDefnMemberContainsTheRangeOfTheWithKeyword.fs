
type Foo() =
    // A write-only property.
    member this.MyWriteOnlyProperty with set (value) = myInternalValue <- value
