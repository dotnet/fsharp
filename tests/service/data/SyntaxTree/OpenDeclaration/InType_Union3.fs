type AUnion = 
    open System
    | A of int
    with
        member _.RandomNumber with get() = Random().Next()
