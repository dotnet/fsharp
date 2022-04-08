// #Regression #NoMono #NoMT #CodeGen #EmittedIL 
// Regression test for FSHARP1.0:4400 - Improve codegen to use "natural" names for let-bound fields in classes
// Regression test for FSHARP1.0:432  - Name cleanup needed for TLR generated methods

type C() = 
    let x = System.Console.ReadLine()
    let g() = x
    let x = System.Console.ReadLine()
    member self.M() = x + g()

let f(x) = 
    let g() = 
        printfn "Hello"
        printfn "Hello"
    g(), g()
