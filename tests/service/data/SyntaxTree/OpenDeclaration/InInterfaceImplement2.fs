type A() =
    interface System.IDisposable with
        member _.F _ = 3
        open System