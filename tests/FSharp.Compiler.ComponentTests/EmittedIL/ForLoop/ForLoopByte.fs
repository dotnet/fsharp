let mutable c = 0uy

module Up =
    let constEmpty () =
        for n = 10uy to 1uy do
            c <- n

    let constNonEmpty () =
        for n = 1uy to 10uy do
            c <- n

    let constFinish start =
        for n = start to 10uy do
            c <- n

    let constStart finish =
        for n = 1uy to finish do
            c <- n

    let annotatedStart (start: byte) finish =
        for n = start to finish do
            c <- n

    let annotatedFinish start (finish: byte) =
        for n = start to finish do
            c <- n

    let inferredStartAndFinish start finish =
        for n = start to finish do
            c <- n
