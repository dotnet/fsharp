// #Regression #Conformance #UnitsOfMeasure #Operators 
// Regression tests for FSHARP1.0:3427
// The strategy here is just to make sure all the
// "reasonable" operators are implemented. We are
// not really looking at the results.
//<Expects status="success"></Expects>
module M

module WithFloat =
    [<Measure>] type A
    type Ab = float<A>          // Type abbreviation
    let b : Ab = 100.0<A>

    let _ = b + 2.0<A>
    let _ = b - 2.0<A>
    let _ = b * 2.0
    let _ = b / 2.0
    let _ = -b
    let _ = b < 2.0<A>
    let _ = b <= 2.0<A>
    let _ = b > 2.0<A>
    let _ = b >= 2.0<A>
    let _ = b <> 2.0<A>
    let _ = max b b
    let _ = min b b
    let _ = abs b
    let _ = sign b
    let _ = b |> ignore
    let _ = ignore <| b
    let _ = ignore b
    let _ = box b
    let _ = hash b
    let _ = sizeof<Ab>
    let _ = typeof<Ab>
    let _ = typedefof<Ab>
    let _ = unbox b
    let _ = ref b
    let _ = byte b
    let _ = sbyte b
    let _ = int16 b
    let _ = uint16 b
    let _ = int32 b
    let _ = int b
    let _ = uint32 b
    let _ = int64 b
    let _ = uint64 b
    let _ = nativeint b
    let _ = unativeint b
    let _ = float b
    let _ = double b
    let _ = double b
    let _ = float32 b
    let _ = single b
    let _ = single b
    let _ = decimal b
    let _ = char 
