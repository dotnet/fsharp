module Test

type EmptyT =
    static let x = 5
    static do printfn "init"
    static member PrintIt() = printfn "%i" x


EmptyT.PrintIt()