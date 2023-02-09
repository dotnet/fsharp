
type T() =
    new () =
        T ()

    internal new () =
        T ()

    [<Foo>]
    new () =
        T ()
