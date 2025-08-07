type ARecord = { A : int }
    with
        member _.RandomNumber with get() = Random().Next()
        open System
