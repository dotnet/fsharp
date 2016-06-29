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

let CountList list =
     let rec TailCountList list acc =
         match list with
         | [] -> acc
         | h::t -> TailCountList t acc + 1

     TailCountList list 0 // stepping into CountList should step to here.

//printfn "%d" <| CountList [1;2;3;4;5;6]