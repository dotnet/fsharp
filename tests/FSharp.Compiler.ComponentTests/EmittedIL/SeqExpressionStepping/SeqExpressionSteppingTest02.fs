// #NoMono #NoMT #CodeGen #EmittedIL #Sequences   
module SeqExpressionSteppingTest2 // Regression test for FSHARP1.0:4058
module SeqExpressionSteppingTest2 = 
    let f1 () = 
        seq { printfn "hello"
              yield 1
              printfn "goodbye"
              yield 2 }

    let _ = f1()|> Seq.length
