// #NoMono #NoMT #CodeGen #EmittedIL #Tuples

[<EntryPoint>]
let main argv =
    let p v = printfn "%A" v

    let dic = System.Collections.Generic.Dictionary<int, int>()
    // Tests that the Tuple is eliminated as intended for instance methods
    let (b : bool, i : int) = dic.TryGetValue 1
    p b
    p i

    // Tests that the Tuple is eliminated as intended for static methods
    let (b : bool, l : int64) as t = System.Int64.TryParse "123"
    p b
    p l

    let tt : bool*int64 = t 

    // Tests that a Tuple is created as needed when calling p
    p tt

    0
