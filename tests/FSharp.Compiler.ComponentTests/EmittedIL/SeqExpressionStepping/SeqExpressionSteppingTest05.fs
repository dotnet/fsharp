// #NoMono #NoMT #CodeGen #EmittedIL #Sequences   
module SeqExpressionSteppingTest5 // Regression test for FSHARP1.0:4058
module SeqExpressionSteppingTest5 = 
    let f4 () = 
        seq {
            let mutable x = 0
            try
                let mutable y = 0
                y <- y + 1
                yield x
                let z = x + y
                yield z
            finally
                x <- x + 1
                printfn "done"
        }

    let _ = f4()|> Seq.length
