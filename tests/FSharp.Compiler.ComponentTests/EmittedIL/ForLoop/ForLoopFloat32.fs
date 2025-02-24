let mutable c = 0.0f

module Up =
    let constEmpty () =
        for n = 10.0f to 1.0f do
            c <- n

    let constNonEmpty () =
        for n = 1.0f to 10.0f do
            c <- n

    let constFinish start =
        for n = start to 10.0f do
            c <- n

    let constStart finish =
        for n = 1.0f to finish do
            c <- n

    let annotatedStart (start: float32) finish =
        for n = start to finish do
            c <- n

    let annotatedFinish start (finish: float32) =
        for n = start to finish do
            c <- n

    let inferredStartAndFinish start finish =
        for n = start to finish do
            c <- n

module Down =
    let constEmpty () =
        for n = 1.0f downto 10.0f do
            c <- n

    let constNonEmpty () =
        for n = 10.0f downto 1.0f do
            c <- n

    let constFinish start =
        for n = start downto 1.0f do
            c <- n

    let constStart finish =
        for n = 10.0f downto finish do
            c <- n

    let annotatedStart (start: float32) finish =
        for n = start downto finish do
            c <- n

    let annotatedFinish start (finish: float32) =
        for n = start downto finish do
            c <- n

    let inferredStartAndFinish start finish =
        for n = start downto finish do
            c <- n
