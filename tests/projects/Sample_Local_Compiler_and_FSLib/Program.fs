module Program

type CBuilder() =
    [<CustomOperation>]
    member this.Foo _ = "Foo"
    [<CustomOperation>]
    member this.foo _ = "foo"
    member this.Yield _ = ()
    member this.Zero _ = ()


[<EntryPoint>]
let main _ =
    let cb = CBuilder()

    let x = cb { Foo }
    let y = cb { foo }
    printfn $"{x}"
    printfn $"{y}"

    if x <> "Foo" then
        failwith "not Foo"
    if y <> "foo" then
        failwith "not foo"
    0
