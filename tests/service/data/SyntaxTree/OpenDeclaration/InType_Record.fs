type ARecord = { A : int }
    with
        open System
        member _.RandomNumber with get() = Random().Next()
