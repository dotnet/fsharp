type AUnion = | A of int
    with
        member _.RandomNumber with get() = Random().Next()
        open System
