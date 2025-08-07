type ARecord = 
    { A : int }
    open System
    with
        member _.RandomNumber with get() = Random().Next()
