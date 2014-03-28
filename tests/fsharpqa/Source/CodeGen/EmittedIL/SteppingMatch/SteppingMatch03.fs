// #NoMono #NoMT #CodeGen #EmittedIL 
module SteppingMatch03 // Regression test for FSHARP1.0:4339
open System
let public funcC (n) =   
        match n with                                    
        | Choice1Of3 _ ->            
            Console.WriteLine("A")
        | Choice2Of3 _ ->
            Console.WriteLine("B")
        | Choice3Of3 _ ->
            Console.WriteLine("C")
