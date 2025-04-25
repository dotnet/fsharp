let mutable c = 0.0m

module Up =
    let constEmpty () =
        for n = 10.0m to 1.0m do
            c <- n

    let constNonEmpty () =
        for n = 1.0m to 10.0m do
            c <- n

    let constFinish start =
        for n = start to 10.0m do
            c <- n

    let constStart finish =
        for n = 1.0m to finish do
            c <- n

    let annotatedStart (start: decimal) finish =
        for n = start to finish do
            c <- n

    let annotatedFinish start (finish: decimal) =
        for n = start to finish do
            c <- n

    let inferredStartAndFinish start finish =
        for n = start to finish do
            c <- n

module Down =
    let constEmpty () =
        for n = 1.0m downto 10.0m do
            c <- n

    let constNonEmpty () =
        for n = 10.0m downto 1.0m do
            c <- n

    let constFinish start =
        for n = start downto 1.0m do
            c <- n

    let constStart finish =
        for n = 10.0m downto finish do
            c <- n

    let annotatedStart (start: decimal) finish =
        for n = start downto finish do
            c <- n

    let annotatedFinish start (finish: decimal) =
        for n = start downto finish do
            c <- n

    let inferredStartAndFinish start finish =
        for n = start downto finish do
            c <- n
