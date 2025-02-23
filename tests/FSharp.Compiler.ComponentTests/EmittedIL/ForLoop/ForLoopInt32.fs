open System.Runtime.CompilerServices

let mutable c = 0

[<MethodImpl(MethodImplOptions.NoInlining)>]
let f<'a> (x: 'a) = ignore x

[<MethodImpl(MethodImplOptions.NoInlining)>]
let forceBoxing (x: obj) = ignore x

module Up =
    let constEmpty () =
        for n = 10 to 1 do
            c <- n

    let constNonEmpty () =
        for n = 1 to 10 do
            c <- n

    let constFinish start =
        for n = start to 10 do
            c <- n

    let constStart finish =
        for n = 1 to finish do
            c <- n

    let annotatedStart (start: int) finish =
        for n = start to finish do
            c <- n

    let annotatedFinish start (finish: int) =
        for n = start to finish do
            c <- n

    let inferredStartAndFinish start finish =
        for n = start to finish do
            c <- n

    let unconstrainedStartAndFinish start finish =
        for n = start to finish do
            f n

    let boxedLoopVar start finish =
        for n = start to finish do
            forceBoxing n

module Down =
    let constEmpty () =
        for n = 1 downto 10 do
            c <- n

    let constNonEmpty () =
        for n = 10 downto 1 do
            c <- n

    let constFinish start =
        for n = start downto 1 do
            c <- n

    let constStart finish =
        for n = 10 downto finish do
            c <- n

    let annotatedStart (start: int) finish =
        for n = start downto finish do
            c <- n

    let annotatedFinish start (finish: int) =
        for n = start downto finish do
            c <- n

    let inferredStartAndFinish start finish =
        for n = start downto finish do
            c <- n

    let unconstrainedStartAndFinish start finish =
        for n = start downto finish do
            f n

    let boxedLoopVar start finish =
        for n = start downto finish do
            forceBoxing n
