let mutable c = 0un

module Up =
    let constEmpty () =
        for n = 10un to 1un do
            c <- n

    let constNonEmpty () =
        for n = 1un to 10un do
            c <- n

    let constFinish start =
        for n = start to 10un do
            c <- n

    let constStart finish =
        for n = 1un to finish do
            c <- n

    let annotatedStart (start: unativeint) finish =
        for n = start to finish do
            c <- n

    let annotatedFinish start (finish: unativeint) =
        for n = start to finish do
            c <- n

    let inferredStartAndFinish start finish =
        for n = start to finish do
            c <- n
