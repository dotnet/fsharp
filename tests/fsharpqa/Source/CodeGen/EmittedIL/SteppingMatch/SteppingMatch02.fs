// #NoMono #NoMT #CodeGen #EmittedIL 
module SteppingMatch02 // Regression test for FSHARP1.0:4339
open System
let public funcB (n) =   
        match n with                                    
        | Choice1Of2 _ ->            
            Console.WriteLine("B")
        | Choice2Of2 _ ->
            Console.WriteLine("A")
