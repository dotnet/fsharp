// #NoMono #NoMT #CodeGen #EmittedIL #Sequences   
module SeqExpressionSteppingTest3 // Regression test for FSHARP1.0:4058
module SeqExpressionSteppingTest3 = 
    let f2 () = 
        let x = ref 0 
        seq { while !x < 4 do 
                 incr x
                 printfn "hello"
                 yield x }

    let _ = f2()|> Seq.length
