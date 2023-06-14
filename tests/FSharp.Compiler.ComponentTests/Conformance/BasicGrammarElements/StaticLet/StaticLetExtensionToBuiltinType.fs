module Test

type System.Random with
    static let getcached() = new System.Random(42)
    static member NextInt() = (getcached()).Next()

printfn "%i" (System.Random.NextInt())