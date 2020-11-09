// #Conformance #LexicalAnalysis 
#light

let x =
    #if NOTDEFINED
    1
    (* Comment - *) #else
    // Single line#else
    #else
    2
    #endif
    
    
if x <> 2 then exit 1


let y = 
    #if NOTDEFINED
    1
    (* multi
       line
       #else
    (* Note the previous comment is immediately terminated *)
    2
    #endif
if y <> 2 then exit 1

exit 0
