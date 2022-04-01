// #NoMono #NoMT #CodeGen #EmittedIL 
#light

let TestFunction1() =
    printfn "Hello";
    printfn "World";
    3+4
    
let TestFunction6() =
    let f() = 
       let y = TestFunction1()
       printfn "Hello";
       y + y
    f() + f()

