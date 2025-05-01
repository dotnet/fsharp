module Test

type MyPlainEnum =
    | A = 0
    | B = 1
    static let isThisPossible = A
    static let myFavNumber = 42
    static let myFavFunc a b = a + b


let methods = typeof<MyPlainEnum>.GetMethods(System.Reflection.BindingFlags.Static)
    
printfn "%i" (methods.Length)