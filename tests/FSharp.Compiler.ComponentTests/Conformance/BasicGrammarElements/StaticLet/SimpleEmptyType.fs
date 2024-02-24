module Test

type EmptyT =
    static let x = 5
    static do printfn "init"
    static let mutable counter = 0
    static member Incr() = 
        counter <- counter  + 1
        counter
    static member PrintIt() = printfn "%i" x


EmptyT.PrintIt()
ignore (EmptyT.Incr())
ignore (EmptyT.Incr())

printfn "%i" (EmptyT.Incr())