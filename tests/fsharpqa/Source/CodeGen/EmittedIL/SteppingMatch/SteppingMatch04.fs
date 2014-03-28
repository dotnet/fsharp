// #NoMono #NoMT #CodeGen #EmittedIL 
module SteppingMatch04 // Regression test for FSHARP1.0:4339
open System
let public funcC2 (n) =   
        match n with                                    
        | Choice2Of3 _ ->
            Console.WriteLine("B")
        | Choice3Of3 _ ->
            Console.WriteLine("C")
        | Choice1Of3 _ ->            
            Console.WriteLine("A")
