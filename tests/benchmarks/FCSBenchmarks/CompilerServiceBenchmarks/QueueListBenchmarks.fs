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
