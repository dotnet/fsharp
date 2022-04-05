// #NoMono #NoMT #CodeGen #EmittedIL #Sequences   
module SeqExpressionSteppingTest4 // Regression test for FSHARP1.0:4058
module SeqExpressionSteppingTest4 = 
    let f3 () = 
        seq {
            let mutable x = 0
            x <- x + 1
            let mutable y = 0
            y <- y + 1
            yield x
            let mutable z = x + y
            yield z
        }

    let _ = f3()|> Seq.length
