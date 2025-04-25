let mutable c = 0n

module Up =
    let constEmpty () =
        for n = 10n to 1n do
            c <- n

    let constNonEmpty () =
        for n = 1n to 10n do
            c <- n

    let constFinish start =
        for n = start to 10n do
            c <- n

    let constStart finish =
        for n = 1n to finish do
            c <- n

    let annotatedStart (start: nativeint) finish =
        for n = start to finish do
            c <- n

    let annotatedFinish start (finish: nativeint) =
        for n = start to finish do
            c <- n

    let inferredStartAndFinish start finish =
        for n = start to finish do
            c <- n

module Down =
    let constEmpty () =
        for n = 1n downto 10n do
            c <- n

    let constNonEmpty () =
        for n = 10n downto 1n do
            c <- n

    let constFinish start =
        for n = start downto 1n do
            c <- n

    let constStart finish =
        for n = 10n downto finish do
            c <- n

    let annotatedStart (start: nativeint) finish =
        for n = start downto finish do
            c <- n

    let annotatedFinish start (finish: nativeint) =
        for n = start downto finish do
            c <- n

    let inferredStartAndFinish start finish =
        for n = start downto finish do
            c <- n
