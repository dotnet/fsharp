let mutable c = 0L

module Up =
    let constEmpty () =
        for n = 10L to 1L do
            c <- n

    let constNonEmpty () =
        for n = 1L to 10L do
            c <- n

    let constFinish start =
        for n = start to 10L do
            c <- n

    let constStart finish =
        for n = 1L to finish do
            c <- n

    let annotatedStart (start: int64) finish =
        for n = start to finish do
            c <- n

    let annotatedFinish start (finish: int64) =
        for n = start to finish do
            c <- n

    let inferredStartAndFinish start finish =
        for n = start to finish do
            c <- n

module Down =
    let constEmpty () =
        for n = 1L downto 10L do
            c <- n

    let constNonEmpty () =
        for n = 10L downto 1L do
            c <- n

    let constFinish start =
        for n = start downto 1L do
            c <- n

    let constStart finish =
        for n = 10L downto finish do
            c <- n

    let annotatedStart (start: int64) finish =
        for n = start downto finish do
            c <- n

    let annotatedFinish start (finish: int64) =
        for n = start downto finish do
            c <- n

    let inferredStartAndFinish start finish =
        for n = start downto finish do
            c <- n
