// #NoMono #NoMT #CodeGen #EmittedIL   
module SteppingMatch07 // Regression test for FSHARP1.0:4339
open System
type Discr = CaseA | CaseB
let public funcE (n) =
        match n with                                    
        | CaseA->
            Console.WriteLine("A")
        | CaseB ->            
            Console.WriteLine("B")
