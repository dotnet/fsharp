// #Regression #Conformance #LexFilter #Exceptions 
#light

// FSB 1624, LexFilter should consifer infix tokens according to their length

//<Expects status="success"></Expects>

let x = 3
let y = x
      + x
   
let (--) = (-)
let z =    x
        -- x
     
let ( *** ) = (*)
let a = 
                 x
             *** x
    
let (|>>>>>>>) = (|>)
let veryLongIdentifier = (*  *)[1;2;3] 
                            |> fun a -> a 
                      |>>>>>>> fun b -> b
