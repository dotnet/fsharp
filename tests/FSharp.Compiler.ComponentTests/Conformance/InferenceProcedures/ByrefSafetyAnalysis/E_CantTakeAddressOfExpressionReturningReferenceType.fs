module CantTakeAddressOfExpressionReturningReferenceType =
    open System.Collections.Concurrent
    open System.Collections.Generic

    let test1 () =
        let aggregator = 
            new ConcurrentDictionary<
                    string, ConcurrentDictionary<string, float array>
                    >()

        for kvp in aggregator do
        for kvpInner in kvp.Value do
            kvp.Value.TryRemove(
                kvpInner.Key,
                &kvpInner.Value)
            |> ignore

    let test2 () =
        let x = KeyValuePair(1, [||])
        let y = &x.Value
        ()