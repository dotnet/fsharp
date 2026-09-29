module FSharp11Kernels

open System

let mutable private escaped = id

let private addOffset offset value = value + offset

let Create length kernel =
    let values = Array.init length (fun i -> (i * 17 % 97) + 1)
    let items = values |> Array.mapi (fun i value -> struct (value, 1 + i % 5)) |> Array.toList
    let weights = Array.init length (fun i -> 1 + i % 7)
    let numbers = Array.toList values
    let groups = values |> Array.chunkBySize 4 |> Array.map Array.toList |> Array.toList
    let configuration = Array.tryHead values

    let operation =
        match kernel with
        | "Cart" ->
            fun state ->
                let discount = state &&& 31
                items
                |> List.fold (fun total struct (price, quantity) -> total + price * quantity * (100 - discount) / 100) 0
        | "Rules" ->
            fun state ->
                let threshold = state &&& 127
                let limit = threshold + 50
                let any = numbers |> List.exists (fun value -> value > threshold)
                let all = numbers |> List.forall (fun value -> value < limit)
                (if any then 1 else 0) + (if all then 2 else 0)
        | "Telemetry" ->
            fun state ->
                let scale = (state &&& 31) + 1
                let total = values |> Array.fold (fun sum value -> sum + value * scale) 0
                let weighted = Array.fold2 (fun sum value weight -> sum + value * weight * scale) 0 values weights
                total + weighted
        | "Option" ->
            fun state -> configuration |> Option.map (addOffset (state &&& 31)) |> Option.defaultValue 0
        | "Nested" ->
            fun state ->
                let scale = (state &&& 31) + 1
                groups
                |> List.fold
                    (fun total group -> total + (group |> List.fold (fun sum value -> sum + value * scale) 0))
                    0
        | "FilterMap" ->
            fun state ->
                let threshold = state &&& 63
                let offset = state &&& 31
                numbers |> List.filter (fun value -> value > threshold) |> List.map (fun value -> value + offset) |> List.sum
        | "Escaping" ->
            fun state ->
                let offset = state &&& 31
                escaped <- fun value -> value + offset
                escaped length
        | "NonCapturing" ->
            fun state -> numbers |> List.fold (+) (state &&& 31)
        | _ -> invalidArg (nameof kernel) kernel

    Func<int, int>(operation)
