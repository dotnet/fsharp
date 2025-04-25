let mutable c = '\000'

module Up =
    let constEmpty () =
        for n = 'z' to 'a' do
            c <- n

    let constNonEmpty () =
        for n = 'a' to 'z' do
            c <- n

    let constFinish start =
        for n = start to 'z' do
            c <- n

    let constStart finish =
        for n = 'a' to finish do
            c <- n

    let annotatedStart (start: char) finish =
        for n = start to finish do
            c <- n

    let annotatedFinish start (finish: char) =
        for n = start to finish do
            c <- n

    let inferredStartAndFinish start finish =
        for n = start to finish do
            c <- n
