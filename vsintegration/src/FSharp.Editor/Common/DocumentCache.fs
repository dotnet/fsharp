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

    // A document's own text is not the whole story: an edit elsewhere in the project can change what
    // its names mean while its text stands still, so the key has to cover both versions.
    static let currentVersion (doc: Document) (ct: CancellationToken) =
        task {
            let! textVersion = doc.GetTextVersionAsync ct
            let! semanticVersion = doc.Project.GetDependentSemanticVersionAsync ct
            return textVersion, semanticVersion
        }

    static let tryGetCachedValueAsync (doc: Document, cache: MemoryCache, ct: CancellationToken) =
        if ct.IsCancellationRequested then
            Task.FromCanceled<'Value voption>(ct)
        else
            task {
                let! version = currentVersion doc ct

                match cache.Get(doc.Id.ToString()) with
                | null -> return ValueNone
                | :? ((VersionStamp * VersionStamp) * 'Value) as value ->
                    if fst value = version then
                        return ValueSome(snd value)
                    else
                        return ValueNone
                | _ -> return ValueNone
            }

    static let setCacheValueAsync (doc: Document, value: 'Value, cache: MemoryCache, policy: CacheItemPolicy, ct: CancellationToken) =
        if ct.IsCancellationRequested then
            Task.FromCanceled<unit>(ct)
        else
            task {
                let! version = currentVersion doc ct
                do cache.Set(doc.Id.ToString(), (version, value), policy)
            }

    new(name: string, slidingExpirationSeconds: float) =
        new DocumentCache<'Value>(name, CacheItemPolicy(SlidingExpiration = (TimeSpan.FromSeconds slidingExpirationSeconds)))

    member _.TryGetValueAsync(doc: Document) : CancellableTask<'Value voption> =
        fun ct -> tryGetCachedValueAsync (doc, cache, ct)

    member _.SetAsync(doc: Document, value: 'Value) : CancellableTask<unit> =
        fun ct -> setCacheValueAsync (doc, value, cache, policy, ct)

    interface IDisposable with
        member _.Dispose() = cache.Dispose()
