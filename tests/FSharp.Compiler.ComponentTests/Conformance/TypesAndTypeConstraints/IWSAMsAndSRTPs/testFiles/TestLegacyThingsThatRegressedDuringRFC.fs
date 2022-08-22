#nowarn "62"

module TestLegacyThingsThatRegressedDuringRFC =
    let legacyConcat1 (x: string) (y: string) = x ^ y
    let legacyConcat2 (x: string) (y: string) = x ^y
    let legacyConcat3 (x: string) (y: string) = x^ y
    let legacyConcat4 (x: string) (y: string) = x^y

    let testSlicingOne() =
        let arr = [| 1;2;3;4;5 |]
        arr.[^3..]

    let testSlicingTwo() =
        let arr = [| 1;2;3;4;5 |]
        arr[^3..]

    printfn ""
