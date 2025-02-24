let mutable c = 0us

module Up =
    let constEmpty () =
        for n = 10us to 1us do
            c <- n

    let constNonEmpty () =
        for n = 1us to 10us do
            c <- n

    let constFinish start =
        for n = start to 10us do
            c <- n

    let constStart finish =
        for n = 1us to finish do
            c <- n

    let annotatedStart (start: uint16) finish =
        for n = start to finish do
            c <- n

    let annotatedFinish start (finish: uint16) =
        for n = start to finish do
            c <- n

    let inferredStartAndFinish start finish =
        for n = start to finish do
            c <- n
