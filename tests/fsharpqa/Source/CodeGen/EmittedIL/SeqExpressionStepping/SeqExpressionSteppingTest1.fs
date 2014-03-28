// #NoMono #NoMT #CodeGen #EmittedIL #Sequences #NETFX20Only #NETFX40Only 
module SeqExpressionSteppingTest1 // Regression test for FSHARP1.0:4058
module SeqExpressionSteppingTest1 = 

    let f0 () = 
        seq { yield 1 }

    let _ = f0()|> Seq.length
