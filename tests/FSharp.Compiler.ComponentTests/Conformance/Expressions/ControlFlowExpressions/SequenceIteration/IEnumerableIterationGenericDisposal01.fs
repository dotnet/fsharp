// #Conformance #ControlFlow #Sequences #Regression #InterfacesAndImplementations
// Ensure generic IEnumerable iteration calls GetEnumerator and disposes enumerators via try/finally.

open System
open System.Collections
open System.Collections.Generic

type CountingEnumerable(values: int[]) =
    static let mutable getEnumeratorCalls = 0
    static let mutable disposeCalls = 0

    static member Reset() =
        getEnumeratorCalls <- 0
        disposeCalls <- 0

    static member GetEnumeratorCalls = getEnumeratorCalls
    static member DisposeCalls = disposeCalls

    interface IEnumerable<int> with
        member _.GetEnumerator() =
            getEnumeratorCalls <- getEnumeratorCalls + 1
            let mutable i = -1

            { new IEnumerator<int> with
                member _.Current = values[i]

              interface IEnumerator with
                  member _.Current = box values[i]
                  member _.MoveNext() =
                      i <- i + 1
                      i < values.Length
                  member _.Reset() = i <- -1

              interface IDisposable with
                  member _.Dispose() = disposeCalls <- disposeCalls + 1 }

    interface IEnumerable with
        member this.GetEnumerator() =
            (this :> IEnumerable<int>).GetEnumerator() :> IEnumerator

let runSuccessfulIteration () =
    CountingEnumerable.Reset()

    let enumerable = CountingEnumerable([| 1; 2; 3 |]) :> IEnumerable<int>
    let mutable sum = 0

    for x in enumerable do
        sum <- sum + x

    sum = 6
    && CountingEnumerable.GetEnumeratorCalls = 1
    && CountingEnumerable.DisposeCalls = 1

let runExceptionalIteration () =
    CountingEnumerable.Reset()

    let enumerable = CountingEnumerable([| 1; 2; 3 |]) :> IEnumerable<int>

    try
        for x in enumerable do
            if x = 2 then
                failwith "boom"
    with _ ->
        ()

    CountingEnumerable.GetEnumeratorCalls = 1
    && CountingEnumerable.DisposeCalls = 1

let ok = runSuccessfulIteration () && runExceptionalIteration ()
exit (if ok then 0 else 1)
