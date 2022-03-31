// #NoMono #NoMT #CodeGen #EmittedIL #Sequences   
module SeqExpressionSteppingTest4 // Regression test for FSHARP1.0:4058
module SeqExpressionSteppingTest4 = 
    let f3 () = 
        seq { let x = ref 0 
              incr x
              let y = ref 0 
              incr y
              yield !x
              let z = !x + !y
              yield z }

    let _ = f3()|> Seq.length
