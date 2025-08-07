type AUnion = 
    | A of int
    open System
    with
        member _.RandomNumber with get() = Random().Next()
