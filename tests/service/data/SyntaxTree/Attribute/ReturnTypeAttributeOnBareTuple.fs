module M

type T =
    static member Parenthesized(name: string) :
        [<ReturnDescription("first")>]
        [<ReturnDescription("second")>]
        (string * string) =
            name, name

    static member Bare(name: string) :
        [<ReturnDescription("first")>]
        [<ReturnDescription("second")>]
        string * string =
            name, name
