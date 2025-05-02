open System

type private Disposable() =
    [<DefaultValue>] static val mutable private disposedTimes: int
    [<DefaultValue>] static val mutable private constructedTimes: int

    do Disposable.constructedTimes <- Disposable.constructedTimes + 1

    static member DisposeCallCount() = Disposable.disposedTimes
    static member ConstructorCallCount() = Disposable.constructedTimes

    interface System.IDisposable with
        member _.Dispose() =
            Disposable.disposedTimes <- Disposable.disposedTimes + 1

let _scope =
    use _ = new Disposable()
    ()

let disposeCalls = Disposable.DisposeCallCount()
if disposeCalls <> 1 then
    failwith "was not disposed or disposed too many times"

let ctorCalls = Disposable.ConstructorCallCount()
if ctorCalls <> 1 then
    failwithf "unexpected constructor call count: %i" ctorCalls
