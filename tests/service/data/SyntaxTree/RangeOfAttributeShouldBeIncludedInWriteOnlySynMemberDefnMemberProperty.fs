type Crane =
    [<Foo>]
    member this.MyWriteOnlyProperty with set (value) = myInternalValue <- value