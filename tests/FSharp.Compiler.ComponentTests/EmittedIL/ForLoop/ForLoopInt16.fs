let mutable c = 0s

module Up =
    let constEmpty () =
        for n = 10s to 1s do
            c <- n

    let constNonEmpty () =
        for n = 1s to 10s do
            c <- n

    let constFinish start =
        for n = start to 10s do
            c <- n

    let constStart finish =
        for n = 1s to finish do
            c <- n

    let annotatedStart (start: int16) finish =
        for n = start to finish do
            c <- n

    let annotatedFinish start (finish: int16) =
        for n = start to finish do
            c <- n

    let inferredStartAndFinish start finish =
        for n = start to finish do
            c <- n

module Down =
    let constEmpty () =
        for n = 1s downto 10s do
            c <- n

    let constNonEmpty () =
        for n = 10s downto 1s do
            c <- n

    let constFinish start =
        for n = start downto 1s do
            c <- n

    let constStart finish =
        for n = 10s downto finish do
            c <- n

    let annotatedStart (start: int16) finish =
        for n = start downto finish do
            c <- n

    let annotatedFinish start (finish: int16) =
        for n = start downto finish do
            c <- n

    let inferredStartAndFinish start finish =
        for n = start downto finish do
            c <- n
