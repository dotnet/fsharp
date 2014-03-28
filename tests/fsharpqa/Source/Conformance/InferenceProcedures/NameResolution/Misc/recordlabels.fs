// #Regression #Conformance #TypeInference 
#light

// bug 3246

type r1 =  
    { x : int }
    static member Empty = { x = 3 } 
   
and r2 = 
    { x : int }
    static member Empty = { x = 3 }

let r1 : r1 = r1.Empty
let r2 : r2 = r2.Empty
