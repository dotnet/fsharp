// #NoMono #NoMT #CodeGen #EmittedIL #NETFX20Only #NETFX40Only 
module SteppingMatch06 // Regression test for FSHARP1.0:4339
open System
type Discr = CaseA | CaseB
let public funcD (n) =
        match n with                                    
        | CaseB ->            
            Console.WriteLine("B")
        | CaseA->
            Console.WriteLine("A")
