// #NoMono #NoMT #CodeGen #EmittedIL 
module SteppingMatch09
open System
let public funcA n =   
        match n with                                    
        | 1  ->
            Some(10)  // debug range should cover all of "Some(10)"
        | 2  ->            
            None
        | _ ->
                   Some(   22   )  // debug range should cover all of "Some(   22   )"
