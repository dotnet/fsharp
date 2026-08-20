namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Runtime.Caching
open Microsoft.CodeAnalysis
open Internal.Utilities.Library
open Internal.Utilities.Library

[<Sealed; NoComparison; NoEquality>]
type DocumentCache<'Value when 'Value: not struct>(name: string, ?cacheItemPolicy: CacheItemPolicy) =

    [<Literal>]
    let defaultSlidingExpiration = 2.

    let cache = new MemoryCache(name)

    let policy =
        defaultArg cacheItemPolicy (CacheItemPolicy(SlidingExpiration = (TimeSpan.FromSeconds defaultSlidingExpiration)))

    new(name: string, slidingExpirationSeconds: float) =
        new DocumentCache<'Value>(name, CacheItemPolicy(SlidingExpiration = (TimeSpan.FromSeconds slidingExpirationSeconds)))

    member _.TryGetValueAsync(doc: Document) =
        async2 {
            let! ct = Async2.CancellationToken
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
        async2 {
            let! ct = Async2.CancellationToken
            let! currentVersion = doc.GetTextVersionAsync ct
            do cache.Set(doc.Id.ToString(), (currentVersion, value), policy)
        }

    interface IDisposable with
        member _.Dispose() = cache.Dispose()
