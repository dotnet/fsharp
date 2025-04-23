open System

type Disposable() =
    [<DefaultValue>] static val mutable private disposedTimes: int
    [<DefaultValue>] static val mutable private constructedTimes: int

    do Disposable.constructedTimes <- Disposable.constructedTimes + 1

    static member DisposeCallCount() = Disposable.disposedTimes
    static member ConstructorCallCount() = Disposable.constructedTimes

    interface IDisposable with
        member _.Dispose() =
            Disposable.disposedTimes <- Disposable.disposedTimes + 1

type DisposableBuilder() =
    member _.Using(resource: #IDisposable, f) =
        async {
            use res = resource
            return! f res
        }
        
    member _.Bind(disposable: Disposable, f) =
        async {
            let c = disposable
            return! f c
        }
    
    member _.Return(x) = async.Return x
    
    member _.ReturnFrom(x) = x
    
    member _.Bind(task, f) = async.Bind(task, f)

let counterDisposable = DisposableBuilder()

let example() =
    counterDisposable {
        use! res = new Disposable()
        use! __ = new Disposable()
        use! (res1) = new Disposable()
        use! (___) = new Disposable()
        use! _ = new Disposable()
        use! (_) = new Disposable()
        use! _res2 = new Disposable()
        use! (_res3) = new Disposable()
        return ()
    }

example()
|> Async.RunSynchronously

let disposeCalls = Disposable.DisposeCallCount()
if disposeCalls <> 8 then
    failwithf $"unexpected dispose call count: %i{disposeCalls}"
let ctorCalls = Disposable.ConstructorCallCount()
if ctorCalls <> 8 then
    failwithf $"unexpected constructor call count: %i{ctorCalls}"