// #NoMono #NoMT #CodeGen #EmittedIL #Unions 
module CCtorDUWithMember02a
printfn "hello1"

module M =
    printfn "hello2"
    let mutable x = ("1".Length)

let y = (printfn "hello3"; M.x)
