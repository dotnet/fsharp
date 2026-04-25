module FsiOrder.Main

[<EntryPoint>]
let main _ =
    let shapes = [
        FsiOrder.Types.Circle 1.0
        FsiOrder.Types.Square 2.0
    ]
    let total = FsiOrder.Consumer.totalArea shapes
    printfn "kinds: %s" (shapes |> List.map FsiOrder.Consumer.describe |> String.concat ", ")
    printfn "total area: %f" total
    0
