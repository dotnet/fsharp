namespace FSharp.Compiler.Benchmarks

open System
open System.Collections
open System.Collections.Generic
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Order
open BenchmarkDotNet.Mathematics
open FSharp.Benchmarks.Common.Categories

// Standalone copy of QueueList for benchmarking with different optimization strategies
module QueueListVariants =

    /// Original QueueList implementation
    type QueueListOriginal<'T>(firstElementsIn: 'T list, lastElementsRevIn: 'T list, numLastElementsIn: int) =
        let numFirstElements = List.length firstElementsIn
        let push = numLastElementsIn > numFirstElements / 5
        
        let firstElements =
            if push then
                List.append firstElementsIn (List.rev lastElementsRevIn)
            else
                firstElementsIn
        
        let lastElementsRev = if push then [] else lastElementsRevIn
        let numLastElements = if push then 0 else numLastElementsIn
        
        let lastElements () =
            if push then [] else List.rev lastElementsRev
        
        static let empty = QueueListOriginal<'T>([], [], 0)
        
        static member Empty: QueueListOriginal<'T> = empty
        
        new(xs: 'T list) = QueueListOriginal(xs, [], 0)
        
        member x.Length = numFirstElements + numLastElements
        member internal x.LastElementsRev = lastElementsRev
        member x.FirstElements = firstElements
        member x.LastElements = lastElements ()
        
        member x.AppendOne(y) =
            QueueListOriginal(firstElements, y :: lastElementsRev, numLastElements + 1)
        
        member x.Append(ys: seq<_>) =
            let newElements = Seq.toList ys
            let newLength = List.length newElements
            let lastElementsRevIn = List.rev newElements @ lastElementsRev
            QueueListOriginal(firstElements, lastElementsRevIn, numLastElementsIn + newLength)
        
        interface IEnumerable<'T> with
            member x.GetEnumerator() : IEnumerator<'T> =
                ((x.FirstElements @ (lastElements ())) :> IEnumerable<_>).GetEnumerator()
        
        interface IEnumerable with
            member x.GetEnumerator() : IEnumerator =
                ((x :> IEnumerable<'T>).GetEnumerator() :> IEnumerator)

    module QueueListOriginal =
        let rec foldBack f (x: QueueListOriginal<_>) acc =
            List.foldBack f x.FirstElements (List.foldBack f x.LastElements acc)

    /// Variant 1: AppendOptimized (current implementation)
    type QueueListV1<'T>(firstElementsIn: 'T list, lastElementsRevIn: 'T list, numLastElementsIn: int) =
        let numFirstElements = List.length firstElementsIn
        let push = numLastElementsIn > numFirstElements / 5
        
        let firstElements =
            if push then
                List.append firstElementsIn (List.rev lastElementsRevIn)
            else
                firstElementsIn
        
        let lastElementsRev = if push then [] else lastElementsRevIn
        let numLastElements = if push then 0 else numLastElementsIn
        
        let lastElements () =
            if push then [] else List.rev lastElementsRev
        
        static let empty = QueueListV1<'T>([], [], 0)
        
        static member Empty: QueueListV1<'T> = empty
        
        new(xs: 'T list) = QueueListV1(xs, [], 0)
        
        member x.Length = numFirstElements + numLastElements
        member internal x.LastElementsRev = lastElementsRev
        member x.FirstElements = firstElements
        member x.LastElements = lastElements ()
        
        member x.AppendOne(y) =
            QueueListV1(firstElements, y :: lastElementsRev, numLastElements + 1)
        
        member x.AppendOptimized(y: QueueListV1<'T>) =
            if y.Length = 0 then x
            elif x.Length = 0 then y
            else
                let mergedLastRev =
                    y.LastElementsRev @ (List.rev y.FirstElements) @ lastElementsRev
                let tailLen = List.length mergedLastRev
                QueueListV1(firstElements, mergedLastRev, tailLen)
        
        interface IEnumerable<'T> with
            member x.GetEnumerator() : IEnumerator<'T> =
                (seq {
                    yield! firstElements
                    yield! Seq.rev lastElementsRev
                }).GetEnumerator()
        
        interface IEnumerable with
            member x.GetEnumerator() : IEnumerator =
                ((x :> IEnumerable<'T>).GetEnumerator() :> IEnumerator)

    module QueueListV1 =
        let rec foldBack f (x: QueueListV1<_>) acc =
            let accTail = List.fold (fun acc v -> f v acc) acc x.LastElementsRev
            List.foldBack f x.FirstElements accTail

    /// Variant 2: Optimized for single-element appends with known size
    type QueueListV2<'T>(firstElementsIn: 'T list, lastElementsRevIn: 'T list, numLastElementsIn: int) =
        let numFirstElements = List.length firstElementsIn
        let push = numLastElementsIn > numFirstElements / 5
        
        let firstElements =
            if push then
                List.append firstElementsIn (List.rev lastElementsRevIn)
            else
                firstElementsIn
        
        let lastElementsRev = if push then [] else lastElementsRevIn
        let numLastElements = if push then 0 else numLastElementsIn
        
        let lastElements () =
            if push then [] else List.rev lastElementsRev
        
        static let empty = QueueListV2<'T>([], [], 0)
        
        static member Empty: QueueListV2<'T> = empty
        
        new(xs: 'T list) = QueueListV2(xs, [], 0)
        
        member x.Length = numFirstElements + numLastElements
        member internal x.LastElementsRev = lastElementsRev
        member x.FirstElements = firstElements
        member x.LastElements = lastElements ()
        
        member x.AppendOne(y) =
            QueueListV2(firstElements, y :: lastElementsRev, numLastElements + 1)
        
        // Optimized for appending single element from another QueueList
        member x.AppendOptimizedSingle(y: QueueListV2<'T>) =
            if y.Length = 0 then x
            elif x.Length = 0 then y
            elif y.Length = 1 then
                // Common case: appending single element
                match y.FirstElements, y.LastElementsRev with
                | [elem], [] -> x.AppendOne(elem)
                | [], [elem] -> x.AppendOne(elem)
                | _ -> 
                    let mergedLastRev = y.LastElementsRev @ (List.rev y.FirstElements) @ lastElementsRev
                    QueueListV2(firstElements, mergedLastRev, numLastElements + y.Length)
            else
                let mergedLastRev = y.LastElementsRev @ (List.rev y.FirstElements) @ lastElementsRev
                QueueListV2(firstElements, mergedLastRev, numLastElements + y.Length)
        
        interface IEnumerable<'T> with
            member x.GetEnumerator() : IEnumerator<'T> =
                (seq {
                    yield! firstElements
                    yield! Seq.rev lastElementsRev
                }).GetEnumerator()
        
        interface IEnumerable with
            member x.GetEnumerator() : IEnumerator =
                ((x :> IEnumerable<'T>).GetEnumerator() :> IEnumerator)

    module QueueListV2 =
        let rec foldBack f (x: QueueListV2<_>) acc =
            let accTail = List.fold (fun acc v -> f v acc) acc x.LastElementsRev
            List.foldBack f x.FirstElements accTail

    /// Variant 3: Array-backed with preallocation
    type QueueListV3<'T> private (items: 'T[], count: int) =
        
        static let empty = QueueListV3<'T>([||], 0)
        
        static member Empty: QueueListV3<'T> = empty
        
        new(xs: 'T list) = 
            let arr = List.toArray xs
            QueueListV3(arr, arr.Length)
        
        member x.Length = count
        member x.Items = items
        
        member x.AppendOne(y) =
            let newItems = Array.zeroCreate (count + 1)
            Array.blit items 0 newItems 0 count
            newItems.[count] <- y
            QueueListV3(newItems, count + 1)
        
        member x.AppendOptimized(y: QueueListV3<'T>) =
            if y.Length = 0 then x
            elif x.Length = 0 then y
            else
                let newItems = Array.zeroCreate (count + y.Length)
                Array.blit items 0 newItems 0 count
                Array.blit y.Items 0 newItems count y.Length
                QueueListV3(newItems, count + y.Length)
        
        interface IEnumerable<'T> with
            member x.GetEnumerator() : IEnumerator<'T> =
                (items |> Array.take count :> IEnumerable<_>).GetEnumerator()
        
        interface IEnumerable with
            member x.GetEnumerator() : IEnumerator =
                ((x :> IEnumerable<'T>).GetEnumerator() :> IEnumerator)

    module QueueListV3 =
        let rec foldBack f (x: QueueListV3<_>) acc =
            let mutable result = acc
            for i = x.Length - 1 downto 0 do
                result <- f x.Items.[i] result
            result

    /// Variant 4: ResizeArray-backed for better append performance
    type QueueListV4<'T> private (items: ResizeArray<'T>) =
        
        static let empty = QueueListV4<'T>(ResizeArray())
        
        static member Empty: QueueListV4<'T> = empty
        
        new(xs: 'T list) = 
            let arr = ResizeArray(xs)
            QueueListV4(arr)
        
        member x.Length = items.Count
        member x.Items = items
        
        member x.AppendOne(y) =
            let newItems = ResizeArray(items)
            newItems.Add(y)
            QueueListV4(newItems)
        
        member x.AppendOptimized(y: QueueListV4<'T>) =
            if y.Length = 0 then x
            elif x.Length = 0 then y
            else
                let newItems = ResizeArray(items)
                newItems.AddRange(y.Items)
                QueueListV4(newItems)
        
        interface IEnumerable<'T> with
            member x.GetEnumerator() : IEnumerator<'T> =
                (items :> IEnumerable<_>).GetEnumerator()
        
        interface IEnumerable with
            member x.GetEnumerator() : IEnumerator =
                ((x :> IEnumerable<'T>).GetEnumerator() :> IEnumerator)

    module QueueListV4 =
        let rec foldBack f (x: QueueListV4<_>) acc =
            let mutable result = acc
            for i = x.Length - 1 downto 0 do
                result <- f x.Items.[i] result
            result

    /// Variant 5: DList with lazy materialized list (cached iteration)
    type DList<'T> = DList of ('T list -> 'T list)
    
    module DList =
        let empty<'T> : DList<'T> = DList id
        let singleton x = DList (fun xs -> x::xs)
        let append (DList f) (DList g) = DList (f >> g)
        let appendMany xs (DList f) = DList (List.foldBack (fun x acc -> (fun ys -> x :: acc ys)) xs f)
        let cons x (DList f) = DList (fun xs -> x :: f xs)
        let toList (DList f) = f []
    
    type QueueListV5<'T> private (dlist: DList<'T>, cachedList: Lazy<'T list>, count: int) =
        
        static let empty = 
            let dl = DList.empty
            QueueListV5(dl, lazy (DList.toList dl), 0)
        
        static member Empty: QueueListV5<'T> = empty
        
        new(xs: 'T list) = 
            let dl = DList.appendMany xs DList.empty
            QueueListV5(dl, lazy xs, List.length xs)
        
        member x.Length = count
        member internal x.DList = dlist
        
        member x.AppendOne(y) =
            let newDList = DList.cons y dlist
            QueueListV5(newDList, lazy (DList.toList newDList), count + 1)
        
        member x.AppendOptimized(y: QueueListV5<'T>) =
            if y.Length = 0 then x
            elif x.Length = 0 then y
            else
                let newDList = DList.append dlist y.DList
                QueueListV5(newDList, lazy (DList.toList newDList), count + y.Length)
        
        interface IEnumerable<'T> with
            member x.GetEnumerator() : IEnumerator<'T> =
                (cachedList.Value :> IEnumerable<_>).GetEnumerator()
        
        interface IEnumerable with
            member x.GetEnumerator() : IEnumerator =
                ((x :> IEnumerable<'T>).GetEnumerator() :> IEnumerator)

    module QueueListV5 =
        let rec foldBack f (x: QueueListV5<_>) acc =
            // Use cached list for foldBack
            List.foldBack f (x :> IEnumerable<_> |> Seq.toList) acc

    /// Variant 6: DList with native iteration (no caching)
    type QueueListV6<'T> private (dlist: DList<'T>, count: int) =
        
        static let empty = QueueListV6(DList.empty, 0)
        
        static member Empty: QueueListV6<'T> = empty
        
        new(xs: 'T list) = 
            let dl = DList.appendMany xs DList.empty
            QueueListV6(dl, List.length xs)
        
        member x.Length = count
        member x.DList = dlist
        
        member x.AppendOne(y) =
            let newDList = DList.cons y dlist
            QueueListV6(newDList, count + 1)
        
        member x.AppendOptimized(y: QueueListV6<'T>) =
            if y.Length = 0 then x
            elif x.Length = 0 then y
            else
                let newDList = DList.append dlist y.DList
                QueueListV6(newDList, count + y.Length)
        
        interface IEnumerable<'T> with
            member x.GetEnumerator() : IEnumerator<'T> =
                (DList.toList dlist :> IEnumerable<_>).GetEnumerator()
        
        interface IEnumerable with
            member x.GetEnumerator() : IEnumerator =
                ((x :> IEnumerable<'T>).GetEnumerator() :> IEnumerator)

    module QueueListV6 =
        let rec foldBack f (x: QueueListV6<_>) acc =
            // Use DList directly for foldBack
            List.foldBack f (DList.toList x.DList) acc

    /// Variant 7: ImmutableArray-backed implementation
    open System.Collections.Immutable
    
    type QueueListV7<'T> private (items: ImmutableArray<'T>) =
        
        static let empty = QueueListV7(ImmutableArray.Empty)
        
        static member Empty: QueueListV7<'T> = empty
        
        new(xs: 'T list) = 
            let builder = ImmutableArray.CreateBuilder<'T>()
            builder.AddRange(xs)
            QueueListV7(builder.ToImmutable())
        
        member x.Length = items.Length
        member x.Items = items
        
        member x.AppendOne(y) =
            QueueListV7(items.Add(y))
        
        member x.AppendOptimized(y: QueueListV7<'T>) =
            if y.Length = 0 then x
            elif x.Length = 0 then y
            else
                QueueListV7(items.AddRange(y.Items))
        
        interface IEnumerable<'T> with
            member x.GetEnumerator() : IEnumerator<'T> =
                (items :> IEnumerable<_>).GetEnumerator()
        
        interface IEnumerable with
            member x.GetEnumerator() : IEnumerator =
                ((x :> IEnumerable<'T>).GetEnumerator() :> IEnumerator)

    module QueueListV7 =
        let rec foldBack f (x: QueueListV7<_>) acc =
            // Mimic Array.foldBack implementation
            let arr = x.Items
            let mutable state = acc
            for i = arr.Length - 1 downto 0 do
                state <- f arr.[i] state
            state

open QueueListVariants

[<MemoryDiagnoser>]
[<ShortRunJob>]
[<Orderer(SummaryOrderPolicy.FastestToSlowest)>]
[<RankColumn(NumeralSystem.Roman)>]
[<BenchmarkCategory(ShortCategory)>]
type QueueListBenchmarks() =
    
    let iterations = 5000
    
    [<Benchmark(Baseline = true)>]
    [<BenchmarkCategory("AppendOne")>]
    member _.Original_AppendOne_5000() =
        let mutable q = QueueListOriginal<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendOne")>]
    member _.V1_AppendOne_5000() =
        let mutable q = QueueListV1<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendOne")>]
    member _.V2_AppendOne_5000() =
        let mutable q = QueueListV2<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendOne")>]
    member _.V3_AppendOne_5000() =
        let mutable q = QueueListV3<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendOne")>]
    member _.V4_AppendOne_5000() =
        let mutable q = QueueListV4<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendOne")>]
    member _.V5_DListCached_AppendOne_5000() =
        let mutable q = QueueListV5<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendOne")>]
    member _.V6_DListNative_AppendOne_5000() =
        let mutable q = QueueListV6<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendOne")>]
    member _.V7_ImmutableArray_AppendOne_5000() =
        let mutable q = QueueListV7<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
        q.Length
    
    [<Benchmark(Baseline = true)>]
    [<BenchmarkCategory("AppendWithIteration")>]
    member _.Original_AppendWithForLoop() =
        let mutable q = QueueListOriginal<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            // Simulate iteration that happens in real usage
            let mutable sum = 0
            for x in q do
                sum <- sum + x
            sum |> ignore
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendWithIteration")>]
    member _.V1_AppendWithForLoop() =
        let mutable q = QueueListV1<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            let mutable sum = 0
            for x in q do
                sum <- sum + x
            sum |> ignore
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendWithIteration")>]
    member _.V2_AppendWithForLoop() =
        let mutable q = QueueListV2<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            let mutable sum = 0
            for x in q do
                sum <- sum + x
            sum |> ignore
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendWithIteration")>]
    member _.V3_AppendWithForLoop() =
        let mutable q = QueueListV3<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            let mutable sum = 0
            for x in q do
                sum <- sum + x
            sum |> ignore
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendWithIteration")>]
    member _.V4_AppendWithForLoop() =
        let mutable q = QueueListV4<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            let mutable sum = 0
            for x in q do
                sum <- sum + x
            sum |> ignore
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendWithIteration")>]
    member _.V5_DListCached_AppendWithForLoop() =
        let mutable q = QueueListV5<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            let mutable sum = 0
            for x in q do
                sum <- sum + x
            sum |> ignore
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendWithIteration")>]
    member _.V6_DListNative_AppendWithForLoop() =
        let mutable q = QueueListV6<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            let mutable sum = 0
            for x in q do
                sum <- sum + x
            sum |> ignore
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendWithIteration")>]
    member _.V7_ImmutableArray_AppendWithForLoop() =
        let mutable q = QueueListV7<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            let mutable sum = 0
            for x in q do
                sum <- sum + x
            sum |> ignore
        q.Length
    
    [<Benchmark(Baseline = true)>]
    [<BenchmarkCategory("AppendWithFoldBack")>]
    member _.Original_AppendWithFoldBack() =
        let mutable q = QueueListOriginal<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            // Simulate foldBack that happens in real usage
            let sum = QueueListOriginal.foldBack (+) q 0
            sum |> ignore
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendWithFoldBack")>]
    member _.V1_AppendWithFoldBack() =
        let mutable q = QueueListV1<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            let sum = QueueListV1.foldBack (+) q 0
            sum |> ignore
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendWithFoldBack")>]
    member _.V2_AppendWithFoldBack() =
        let mutable q = QueueListV2<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            let sum = QueueListV2.foldBack (+) q 0
            sum |> ignore
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendWithFoldBack")>]
    member _.V3_AppendWithFoldBack() =
        let mutable q = QueueListV3<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            let sum = QueueListV3.foldBack (+) q 0
            sum |> ignore
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendWithFoldBack")>]
    member _.V4_AppendWithFoldBack() =
        let mutable q = QueueListV4<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            let sum = QueueListV4.foldBack (+) q 0
            sum |> ignore
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendWithFoldBack")>]
    member _.V5_DListCached_AppendWithFoldBack() =
        let mutable q = QueueListV5<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            let sum = QueueListV5.foldBack (+) q 0
            sum |> ignore
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendWithFoldBack")>]
    member _.V6_DListNative_AppendWithFoldBack() =
        let mutable q = QueueListV6<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            let sum = QueueListV6.foldBack (+) q 0
            sum |> ignore
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendWithFoldBack")>]
    member _.V7_ImmutableArray_AppendWithFoldBack() =
        let mutable q = QueueListV7<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            let sum = QueueListV7.foldBack (+) q 0
            sum |> ignore
        q.Length
    
    [<Benchmark(Baseline = true)>]
    [<BenchmarkCategory("Combined")>]
    member _.Original_CombinedScenario() =
        let mutable q = QueueListOriginal<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            // Every 100 iterations, do full operations
            if i % 100 = 0 then
                let mutable sum1 = 0
                for x in q do
                    sum1 <- sum1 + x
                let sum2 = QueueListOriginal.foldBack (+) q 0
                (sum1 + sum2) |> ignore
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("Combined")>]
    member _.V1_CombinedScenario() =
        let mutable q = QueueListV1<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            if i % 100 = 0 then
                let mutable sum1 = 0
                for x in q do
                    sum1 <- sum1 + x
                let sum2 = QueueListV1.foldBack (+) q 0
                (sum1 + sum2) |> ignore
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("Combined")>]
    member _.V2_CombinedScenario() =
        let mutable q = QueueListV2<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            if i % 100 = 0 then
                let mutable sum1 = 0
                for x in q do
                    sum1 <- sum1 + x
                let sum2 = QueueListV2.foldBack (+) q 0
                (sum1 + sum2) |> ignore
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("Combined")>]
    member _.V3_CombinedScenario() =
        let mutable q = QueueListV3<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            if i % 100 = 0 then
                let mutable sum1 = 0
                for x in q do
                    sum1 <- sum1 + x
                let sum2 = QueueListV3.foldBack (+) q 0
                (sum1 + sum2) |> ignore
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("Combined")>]
    member _.V4_CombinedScenario() =
        let mutable q = QueueListV4<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            if i % 100 = 0 then
                let mutable sum1 = 0
                for x in q do
                    sum1 <- sum1 + x
                let sum2 = QueueListV4.foldBack (+) q 0
                (sum1 + sum2) |> ignore
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("Combined")>]
    member _.V5_DListCached_CombinedScenario() =
        let mutable q = QueueListV5<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            if i % 100 = 0 then
                let mutable sum1 = 0
                for x in q do
                    sum1 <- sum1 + x
                let sum2 = QueueListV5.foldBack (+) q 0
                (sum1 + sum2) |> ignore
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("Combined")>]
    member _.V6_DListNative_CombinedScenario() =
        let mutable q = QueueListV6<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            if i % 100 = 0 then
                let mutable sum1 = 0
                for x in q do
                    sum1 <- sum1 + x
                let sum2 = QueueListV6.foldBack (+) q 0
                (sum1 + sum2) |> ignore
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("Combined")>]
    member _.V7_ImmutableArray_CombinedScenario() =
        let mutable q = QueueListV7<int>.Empty
        for i = 1 to iterations do
            q <- q.AppendOne(i)
            if i % 100 = 0 then
                let mutable sum1 = 0
                for x in q do
                    sum1 <- sum1 + x
                let sum2 = QueueListV7.foldBack (+) q 0
                (sum1 + sum2) |> ignore
        q.Length
    
    [<Benchmark(Baseline = true)>]
    [<BenchmarkCategory("AppendQueueList")>]
    member _.Original_AppendQueueList() =
        let mutable q = QueueListOriginal<int>.Empty
        for i = 1 to iterations do
            let single = QueueListOriginal([i])
            q <- q.Append(single)
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendQueueList")>]
    member _.V1_AppendOptimized() =
        let mutable q = QueueListV1<int>.Empty
        for i = 1 to iterations do
            let single = QueueListV1([i])
            q <- q.AppendOptimized(single)
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendQueueList")>]
    member _.V2_AppendOptimizedSingle() =
        let mutable q = QueueListV2<int>.Empty
        for i = 1 to iterations do
            let single = QueueListV2([i])
            q <- q.AppendOptimizedSingle(single)
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendQueueList")>]
    member _.V3_AppendOptimized() =
        let mutable q = QueueListV3<int>.Empty
        for i = 1 to iterations do
            let single = QueueListV3([i])
            q <- q.AppendOptimized(single)
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendQueueList")>]
    member _.V4_AppendOptimized() =
        let mutable q = QueueListV4<int>.Empty
        for i = 1 to iterations do
            let single = QueueListV4([i])
            q <- q.AppendOptimized(single)
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendQueueList")>]
    member _.V5_DListCached_AppendOptimized() =
        let mutable q = QueueListV5<int>.Empty
        for i = 1 to iterations do
            let single = QueueListV5([i])
            q <- q.AppendOptimized(single)
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendQueueList")>]
    member _.V6_DListNative_AppendOptimized() =
        let mutable q = QueueListV6<int>.Empty
        for i = 1 to iterations do
            let single = QueueListV6([i])
            q <- q.AppendOptimized(single)
        q.Length
    
    [<Benchmark>]
    [<BenchmarkCategory("AppendQueueList")>]
    member _.V7_ImmutableArray_AppendOptimized() =
        let mutable q = QueueListV7<int>.Empty
        for i = 1 to iterations do
            let single = QueueListV7([i])
            q <- q.AppendOptimized(single)
        q.Length
