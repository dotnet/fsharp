type AUnion = | A of int
    with
        open System
        member _.RandomNumber with get() = Random().Next()
