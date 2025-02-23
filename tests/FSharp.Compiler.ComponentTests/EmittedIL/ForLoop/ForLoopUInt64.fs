let mutable c = 0UL

module Up =
    let constEmpty () =
        for n = 10UL to 1UL do
            c <- n

    let constNonEmpty () =
        for n = 1UL to 10UL do
            c <- n

    let constFinish start =
        for n = start to 10UL do
            c <- n

    let constStart finish =
        for n = 1UL to finish do
            c <- n

    let annotatedStart (start: uint64) finish =
        for n = start to finish do
            c <- n

    let annotatedFinish start (finish: uint64) =
        for n = start to finish do
            c <- n

    let inferredStartAndFinish start finish =
        for n = start to finish do
            c <- n
