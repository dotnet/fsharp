let mutable c = 0.0

module Up =
    let constEmpty () =
        for n = 10.0 to 1.0 do
            c <- n

    let constNonEmpty () =
        for n = 1.0 to 10.0 do
            c <- n

    let constFinish start =
        for n = start to 10.0 do
            c <- n

    let constStart finish =
        for n = 1.0 to finish do
            c <- n

    let annotatedStart (start: float) finish =
        for n = start to finish do
            c <- n

    let annotatedFinish start (finish: float) =
        for n = start to finish do
            c <- n

    let inferredStartAndFinish start finish =
        for n = start to finish do
            c <- n

module Down =
    let constEmpty () =
        for n = 1.0 downto 10.0 do
            c <- n

    let constNonEmpty () =
        for n = 10.0 downto 1.0 do
            c <- n

    let constFinish start =
        for n = start downto 1.0 do
            c <- n

    let constStart finish =
        for n = 10.0 downto finish do
            c <- n

    let annotatedStart (start: float) finish =
        for n = start downto finish do
            c <- n

    let annotatedFinish start (finish: float) =
        for n = start downto finish do
            c <- n

    let inferredStartAndFinish start finish =
        for n = start downto finish do
            c <- n
