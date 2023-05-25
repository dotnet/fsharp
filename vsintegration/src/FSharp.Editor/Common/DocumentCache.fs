namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Runtime.Caching
open Microsoft.CodeAnalysis
open CancellableTasks

[<Sealed; NoComparison; NoEquality>]
type DocumentCache<'Value when 'Value: not struct>(name: string, ?cacheItemPolicy: CacheItemPolicy) =

    [<Literal>]
    let defaultSlidingExpiration = 2.

    let cache = new MemoryCache(name)

    let policy =
        defaultArg cacheItemPolicy (CacheItemPolicy(SlidingExpiration = (TimeSpan.FromSeconds defaultSlidingExpiration)))

    member _.TryGetValueAsync(doc: Document) =
        cancellableTask {
            let! ct = CancellableTask.getCurrentCancellationToken ()
            let! currentVersion = doc.GetTextVersionAsync ct

            match cache.Get(doc.Id.ToString()) with
            | null -> return ValueNone
            | :? (VersionStamp * 'Value) as value ->
                if fst value = currentVersion then
                    return ValueSome(snd value)
                else
                    return ValueNone
            | _ -> return ValueNone
        }

    member _.SetAsync(doc: Document, value: 'Value) =
        cancellableTask {
            let! ct = CancellableTask.getCurrentCancellationToken ()
            let! currentVersion = doc.GetTextVersionAsync ct
            cache.Set(doc.Id.ToString(), (currentVersion, value), policy)
        }

    interface IDisposable with
        member _.Dispose() = cache.Dispose()
