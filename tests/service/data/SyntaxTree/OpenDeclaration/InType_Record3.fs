type ARecord = 
    open System
    { A : int }
    with
        member _.RandomNumber with get() = Random().Next()
