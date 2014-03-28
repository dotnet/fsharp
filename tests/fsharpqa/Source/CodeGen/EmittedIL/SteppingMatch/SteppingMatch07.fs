// #NoMono #NoMT #CodeGen #EmittedIL #NETFX20Only #NETFX40Only 
module SteppingMatch07 // Regression test for FSHARP1.0:4339
open System
type Discr = CaseA | CaseB
let public funcE (n) =
        match n with                                    
        | CaseA->
            Console.WriteLine("A")
        | CaseB ->            
            Console.WriteLine("B")
