// #NoMono #NoMT #CodeGen #EmittedIL 
module SteppingMatch01 // Regression test for FSHARP1.0:4339
open System
let public funcA (n) =   
        match n with                                    
        | Choice2Of2 _ ->
            Console.WriteLine("A")
        | Choice1Of2 _ ->            
            Console.WriteLine("B")
