
type QB() =
    [<CustomOperation("test1")>]
    member x.Op(i:int) = 
        printfn "test1: %i" i
        true
    [<CustomOperation("test2")>]
    member x.Op(s:string) = 
        printfn "test2 %s" s
        'c'
    member x.Yield(()) = 2

let q : bool = QB() {
    test2
}
