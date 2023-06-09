module Test

type MyPlainEnum =
    | A = 0
    | B = 1
    static let isThisPossible = A
    static member IsIt = isThisPossible
    
printfn "%i" (MyPlainEnum.IsIt)