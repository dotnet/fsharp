let mutable c = 0u

module Up =
    let constEmpty () =
        for n = 10u to 1u do
            c <- n

    let constNonEmpty () =
        for n = 1u to 10u do
            c <- n

    let constFinish start =
        for n = start to 10u do
            c <- n

    let constStart finish =
        for n = 1u to finish do
            c <- n

    let annotatedStart (start: uint) finish =
        for n = start to finish do
            c <- n

    let annotatedFinish start (finish: uint) =
        for n = start to finish do
            c <- n

    let inferredStartAndFinish start finish =
        for n = start to finish do
            c <- n
