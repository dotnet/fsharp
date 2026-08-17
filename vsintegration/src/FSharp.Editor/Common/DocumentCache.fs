namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Runtime.Caching
open System.Threading
open System.Threading.Tasks
open Microsoft.CodeAnalysis
open CancellableTasks

[<Sealed; NoComparison; NoEquality>]
type DocumentCache<'Value when 'Value: not struct>(name: string, ?cacheItemPolicy: CacheItemPolicy) =

    [<Literal>]
    let defaultSlidingExpiration = 2.

    let cache = new MemoryCache(name)

    let policy =
        defaultArg cacheItemPolicy (CacheItemPolicy(SlidingExpiration = (TimeSpan.FromSeconds defaultSlidingExpiration)))

    static let tryGetCachedValueAsync (doc: Document, cache: MemoryCache, ct: CancellationToken) =
        task {
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

    static let setCacheValueAsync (doc: Document, value: 'Value, cache: MemoryCache, policy: CacheItemPolicy, ct: CancellationToken) =
        task {
            let! currentVersion = doc.GetTextVersionAsync ct
            do cache.Set(doc.Id.ToString(), (currentVersion, value), policy)
        }

    new(name: string, slidingExpirationSeconds: float) =
        new DocumentCache<'Value>(name, CacheItemPolicy(SlidingExpiration = (TimeSpan.FromSeconds slidingExpirationSeconds)))

    member _.TryGetValueAsync(doc: Document) : CancellableTask<'Value voption> = fun ct -> tryGetCachedValueAsync (doc, cache, ct)

    member _.SetAsync(doc: Document, value: 'Value) : CancellableTask<unit> =
        fun ct -> setCacheValueAsync (doc, value, cache, policy, ct)

    interface IDisposable with
        member _.Dispose() = cache.Dispose()
