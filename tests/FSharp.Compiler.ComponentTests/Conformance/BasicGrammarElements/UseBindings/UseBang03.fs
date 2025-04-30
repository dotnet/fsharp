open System

type Disposable(id: int) =
    static let mutable disposedIds = Map.empty<int, int>
    static let mutable constructedIds = Set.empty<int>
    
    do constructedIds <- constructedIds.Add(id)
    
    member _.Id = id
    
    static member GetDisposed() = disposedIds
    static member GetConstructed() = constructedIds
    static member Reset() =
        disposedIds <- Map.empty
        constructedIds <- Set.empty
        
    interface IDisposable with
        member this.Dispose() = 
            let currentCount = 
                match Map.tryFind this.Id disposedIds with
                | Some count -> count
                | None -> 0
            disposedIds <- Map.add this.Id (currentCount + 1) disposedIds

type DisposableBuilder() =
    member _.Using(resource: #IDisposable, f) =
        async {
            use res = resource
            return! f res
        }
        
    member _.Bind(disposable: Disposable, f) = async.Bind(async.Return(disposable), f)
    member _.Return(x) = async.Return x
    member _.ReturnFrom(x) = x
    member _.Bind(task, f) = async.Bind(task, f)

let counterDisposable = DisposableBuilder()

let testBindingPatterns() =
    Disposable.Reset()
    
    counterDisposable {
        use! res = new Disposable(1) 
        use! __ = new Disposable(2)
        use! (res1) = new Disposable(3)
        use! _ = new Disposable(4)
        use! (_) = new Disposable(5)
        return ()
    } |> Async.RunSynchronously
    
    let constructed = Disposable.GetConstructed()
    let disposed = Disposable.GetDisposed()
    
    let disposedSet = Set.ofSeq (Map.keys disposed)
    let undisposed = constructed - disposedSet
    
    if not undisposed.IsEmpty then
        printfn $"Undisposed instances: %A{undisposed}"
        failwithf "Not all disposables were properly disposed"
    
    let multipleDisposed = 
        disposed
        |> Map.filter (fun _ count -> count > 1)
    
    if not multipleDisposed.IsEmpty then
        printfn $"Objects disposed multiple times: %A{multipleDisposed}"
        failwithf "Some disposables were disposed multiple times"
        
    printfn $"Success! All %d{constructed.Count} disposables were properly disposed exactly once"

testBindingPatterns()