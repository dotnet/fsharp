{ new System.IDisposable with
    open System
    member _.F _ = 3
}

type A() =
    interface System.IDisposable with
        open System
        member _.F _ = 3
