let mutable c = 0y

module Up =
    let constEmpty () =
        for n = 10y to 1y do
            c <- n

    let constNonEmpty () =
        for n = 1y to 10y do
            c <- n

    let constFinish start =
        for n = start to 10y do
            c <- n

    let constStart finish =
        for n = 1y to finish do
            c <- n

    let annotatedStart (start: sbyte) finish =
        for n = start to finish do
            c <- n

    let annotatedFinish start (finish: sbyte) =
        for n = start to finish do
            c <- n

    let inferredStartAndFinish start finish =
        for n = start to finish do
            c <- n

module Down =
    let constEmpty () =
        for n = 1y downto 10y do
            c <- n

    let constNonEmpty () =
        for n = 10y downto 1y do
            c <- n

    let constFinish start =
        for n = start downto 1y do
            c <- n

    let constStart finish =
        for n = 10y downto finish do
            c <- n

    let annotatedStart (start: sbyte) finish =
        for n = start downto finish do
            c <- n

    let annotatedFinish start (finish: sbyte) =
        for n = start downto finish do
            c <- n

    let inferredStartAndFinish start finish =
        for n = start downto finish do
            c <- n
