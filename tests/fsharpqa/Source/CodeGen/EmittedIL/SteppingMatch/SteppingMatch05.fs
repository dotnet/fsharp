// #NoMono #NoMT #CodeGen #EmittedIL 
module SteppingMatch05 // Regression test for FSHARP1.0:4339
open System
let public funcC3 (n) =   
        match n with                                    
        | Choice3Of3 _ ->
            Console.WriteLine("C")
        | Choice2Of3 _ ->
            Console.WriteLine("B")
        | Choice1Of3 _ ->            
            Console.WriteLine("A")
